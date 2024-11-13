namespace Potluck
{
    public class HomeStatusWebsocket(WebApplicationBuilder builder)
        : WebsocketController<string, FooBox>(builder)
    {
        protected override async Task OnMessage(string message, int houseId)
        {
            await BroadcastMessage(new FooBox(message), houseId);
        }
    }

    public class FooBox(string message)
    {
        public string Message { get; set; } = message;
    }
}
