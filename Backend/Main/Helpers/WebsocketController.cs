using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Data;
using Logic;
using Microsoft.Extensions.Options;
using NJsonSchema;
using Saunter;

namespace Potluck.Helpers;

public class WebsocketController<TLogic, TReceive, TSend>
    where TLogic : LogicBase, new()
{
    private const int TWO_KB = 1024 * 2;
    private readonly Func<TLogic, string, TReceive, TSend?> handler;

    private readonly string path;

    private readonly Dictionary<int, List<WebSocket>> webSockets = [];

    public WebsocketController(
        IEndpointRouteBuilder app,
        string path,
        Func<TLogic, string, TReceive, TSend?> handler
    )
    {
        this.path = path;
        this.handler = handler;

        AddSchemas(app);
        AddGetHandler(app);
    }

    private void AddSchemas(IEndpointRouteBuilder app)
    {
        var apiDoc = app
            .ServiceProvider.GetRequiredService<IOptions<AsyncApiOptions>>()
            .Value.AsyncApi;
        var receive = JsonSchema.FromType<TReceive>();
        var send = JsonSchema.FromType<TSend>();
        apiDoc.Components.Schemas.TryAdd($"/{path}_pub", receive);
        apiDoc.Components.Schemas.TryAdd($"/{path}_sub", send);
    }

    private void AddGetHandler(IEndpointRouteBuilder app)
    {
        app.MapGet(
                $"/ws/{path}",
                async (HttpContext context, PotluckDb db) =>
                {
                    var user = db.GetUser(context.User.Identity!.Name);
                    if (user == null)
                        return Results.Unauthorized();
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        if (user.House == null)
                            return Results.BadRequest();
                        var houseId = user.House.Id;
                        var logic = LogicBase.Create<TLogic>(user, db);
                        await HandleWebsocket(webSocket, houseId, logic, user.UserName ?? "");
                        return Results.Ok();
                    }

                    return Results.BadRequest();
                }
            )
            .WithGroupName("WebSockets")
            .WithTags("WebSockets")
            .WithName($"WebSocket {path}")
            .WithOpenApi();
    }

    private async Task HandleWebsocket(WebSocket ws, int houseId, TLogic logic, string userName)
    {
        if (!webSockets.ContainsKey(houseId))
            webSockets.Add(houseId, []);
        webSockets[houseId].Add(ws);
        await RunReadLoop(ws, houseId, logic, userName);
        webSockets[houseId].Remove(ws);
    }

    private async Task RunReadLoop(WebSocket ws, int houseId, TLogic logic, string userName)
    {
        WebSocketReceiveResult receiveResult;
        do
        {
            var buffer = new byte[TWO_KB];
            receiveResult = await ws.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None
            );
            var message = Encoding.UTF8.GetString(buffer);
            try
            {
                var messageObject = JsonSerializer.Deserialize<TReceive>(message);
                if (messageObject != null)
                {
                    var result = handler(logic, userName, messageObject);
                    if (result != null)
                        await BroadcastMessage(result, houseId);
                }
            }
            catch (JsonException)
            {
            }
        } while (!receiveResult.CloseStatus.HasValue);

        await ws.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None
        );
    }

    public async Task BroadcastMessage(TSend message, int houseId)
    {
        if (webSockets.TryGetValue(houseId, out var websockets))
        {
            var messageString = JsonSerializer.Serialize(message);
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
}