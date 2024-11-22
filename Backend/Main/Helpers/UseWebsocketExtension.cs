using Logic;

namespace Potluck.Helpers
{
    public static class UseWebsocketExtension
    {
        public static WebsocketController<TLogic, TReceive, ValueForUser<TReceive>> UseWebsocket<
            TLogic,
            TReceive
        >(this IEndpointRouteBuilder app, string path, Action<TLogic, TReceive> handler)
            where TLogic : LogicBase, new() =>
            new WebsocketController<TLogic, TReceive, ValueForUser<TReceive>>(
                app,
                path,
                (TLogic logic, string username, TReceive receive) =>
                {
                    handler(logic, receive);
                    return new ValueForUser<TReceive>(username, receive);
                }
            );

        public static WebsocketController<TLogic, TReceive, TSend> UseWebsocket<
            TLogic,
            TReceive,
            TSend
        >(this IEndpointRouteBuilder app, string path, Func<TLogic, TReceive, TSend?> handler)
            where TLogic : LogicBase, new() =>
            new WebsocketController<TLogic, TReceive, TSend>(
                app,
                path,
                (TLogic logic, string username, TReceive receive) => handler(logic, receive)
            );

        public static WebsocketController<
            TLogic,
            TReceive,
            ValueForUser<TReceive>
        > UseGetAndWebsocket<TLogic, TGet, TReceive>(
            this IEndpointRouteBuilder app,
            string path,
            Func<TLogic, TGet> getter,
            Action<TLogic, TReceive> handler
        )
            where TLogic : LogicBase, new()
        {
            app.UseGet(path, getter);
            return app.UseWebsocket(path, handler);
        }

        public static WebsocketController<TLogic, TReceive, TSend> UseGetAndWebsocket<
            TLogic,
            TGet,
            TReceive,
            TSend
        >(
            this IEndpointRouteBuilder app,
            string path,
            Func<TLogic, TGet> getter,
            Func<TLogic, TReceive, TSend?> handler
        )
            where TLogic : LogicBase, new()
        {
            app.UseGet(path, getter);
            return app.UseWebsocket(path, handler);
        }
    }

    public class ValueForUser<T>(string user, T value)
    {
        public string User { get; set; } = user;
        public T Value { get; set; } = value;
    }
}
