using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NJsonSchema;
using NJsonSchema.Generation;
using Saunter;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Potluck.Helpers;

public class WebsocketController<TReceive, TSend>
{
    private const int TWO_KB = 1024 * 2;

    // ReSharper disable once StaticMemberInGenericType
    private static readonly JsonSerializerOptions jsonSerializerOptions =
        new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

    // ReSharper disable once StaticMemberInGenericType
    private static readonly JsonSchemaGeneratorSettings jsonGeneratorSettings =
        new SystemTextJsonSchemaGeneratorSettings
        {
            SerializerOptions = jsonSerializerOptions
        };

    private readonly Dictionary<int, List<WebSocket>> webSockets = [];

    public WebsocketController(
        IEndpointRouteBuilder app,
        string path
    )
    {
        Path = path;
        var apiDoc = app
            .ServiceProvider.GetRequiredService<IOptions<AsyncApiOptions>>()
            .Value.AsyncApi;
        var receive = JsonSchema.FromType<TReceive>(jsonGeneratorSettings);
        var send = JsonSchema.FromType<TSend>(jsonGeneratorSettings);
        apiDoc.Components.Schemas.TryAdd($"{path}_pub", receive);
        apiDoc.Components.Schemas.TryAdd($"{path}_sub", send);
    }

    public string Path { get; }

    public async Task<IResult> Handle(
        HttpContext context,
        int? houseId,
        Action<TReceive> set,
        Func<TSend> get
    )
    {
        if (!context.WebSockets.IsWebSocketRequest) return Results.BadRequest();
        var ws = await context.WebSockets.AcceptWebSocketAsync();
        await HandleWebsocket(ws, houseId, set, get);
        return Results.NoContent();
    }

    private async Task HandleWebsocket(
        WebSocket ws,
        int? houseId,
        Action<TReceive> set,
        Func<TSend> get
    )
    {
        var hasId = houseId.HasValue;
        var id = houseId ?? -1;

        if (hasId)
        {
            if (!webSockets.ContainsKey(id))
                webSockets.Add(id, []);
            webSockets[id].Add(ws);
        }

        try
        {
            await SendMessage(ws, get());
            await RunReadLoop(ws, houseId, set, get);
        }
        finally
        {
            if (hasId)
                webSockets[id].Remove(ws);
        }
    }

    private async Task RunReadLoop(
        WebSocket ws,
        int? houseId,
        Action<TReceive> set,
        Func<TSend> get
    )
    {
        WebSocketReceiveResult receiveResult;
        do
        {
            var buffer = new byte[TWO_KB];
            receiveResult = await ws.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None
            );
            var message = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
            try
            {
                var messageObject = JsonSerializer.Deserialize<TReceive>(message, jsonSerializerOptions);
                if (messageObject != null)
                {
                    set(messageObject);
                    if (houseId.HasValue)
                        await BroadcastMessage(get(), houseId.Value);
                }
            }
            catch (JsonException)
            {
                Console.WriteLine($"Couldn't parse {message} into {typeof(TReceive)}");
            }
        } while (!receiveResult.CloseStatus.HasValue);

        await ws.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None
        );
    }

    private async Task BroadcastMessage(TSend message, int houseId)
    {
        if (webSockets.TryGetValue(houseId, out var websockets))
        {
            var messageString = JsonSerializer.Serialize(message, jsonSerializerOptions);
            var messageBytes = Encoding.UTF8.GetBytes(messageString);
            await Task.WhenAll(
                websockets.Select(ws =>
                    ws.SendAsync(
                        new ArraySegment<byte>(messageBytes),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    )
                )
            );
        }
    }

    private static async Task SendMessage(WebSocket ws, TSend message)
    {
        var messageString = JsonSerializer.Serialize(message, jsonSerializerOptions);
        var messageBytes = Encoding.UTF8.GetBytes(messageString);
        await ws.SendAsync(
            new ArraySegment<byte>(messageBytes),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );
    }
}