using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Data;
using Logic.Models;
using Saunter;
using static Logic.Helpers;

namespace Potluck
{
    public abstract class WebsocketController<TReceive, TSend>
    {
        const int TWO_KB = 1024 * 2;

        private readonly Dictionary<int, List<(User u, WebSocket ws)>> webSockets = [];

        public WebsocketController(WebApplicationBuilder builder)
        {
            builder.Services.Configure(
                (AsyncApiOptions o) =>
                {
                    o.AssemblyMarkerTypes.Add(GetType());
                    o.AsyncApi.Components.Schemas.Add(
                        typeof(TReceive).Name,
                        NJsonSchema.JsonSchema.FromType<TReceive>()
                    );
                    o.AsyncApi.Components.Schemas.Add(
                        typeof(TSend).Name,
                        NJsonSchema.JsonSchema.FromType<TSend>()
                    );
                }
            );
            builder.Services.AddSingleton(this);
        }

        public void Activate(WebApplication app)
        {
            string className = GetType().Name;
            app.MapGet(
                    $"/{string.Join('-', className.Split(c => c.ToString().ToUpperInvariant()[0] == c))}",
                    async (HttpContext context, PotluckDb db) =>
                    {
                        var user = db.GetUser(context.User.Identity!.Name);
                        if (user == null)
                            return Results.Unauthorized();
                        if (context.WebSockets.IsWebSocketRequest)
                        {
                            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                            await HandleWebsocket(webSocket, user);
                            return Results.Ok();
                        }
                        else
                        {
                            return Results.BadRequest();
                        }
                    }
                )
                .WithName(className);
        }

        public async Task HandleWebsocket(WebSocket ws, User user)
        {
            if (user.House == null)
                return;
            int houseId = user.House.Id;
            if (!webSockets.ContainsKey(houseId))
                webSockets.Add(houseId, []);
            webSockets[houseId].Add((user, ws));
            await RunReadLoop(ws, houseId);
            webSockets[houseId].RemoveAll(item => item.u.Id == user.Id);
        }

        private async Task RunReadLoop(WebSocket ws, int houseId)
        {
            WebSocketReceiveResult receiveResult;
            do
            {
                var buffer = new byte[TWO_KB];
                receiveResult = await ws.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None
                );
                string message = Encoding.UTF8.GetString(buffer);
                try
                {
                    var messageObject = JsonSerializer.Deserialize<TReceive>(message);
                    if (messageObject != null)
                        await OnMessage(messageObject, houseId);
                }
                catch (JsonException) { }
            } while (!receiveResult.CloseStatus.HasValue);

            await ws.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None
            );
        }

        protected abstract Task OnMessage(TReceive message, int houseId);

        protected async Task BroadcastMessage(TSend message, int houseId)
        {
            if (webSockets.TryGetValue(houseId, out List<(User u, WebSocket ws)>? websockets))
            {
                var messageString = JsonSerializer.Serialize(message);
                var messageBytes = Encoding.UTF8.GetBytes(messageString);
                await Task.WhenAll(
                    websockets.Select(ws =>
                        ws.ws.SendAsync(
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
}
