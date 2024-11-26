using Logic;

namespace Potluck.Helpers;

public static class UseWebsocketExtensions
{
    public static WebsocketController<TLogic, TReceive, ValueForUser<TReceive>> UseWebsocket<
        TLogic,
        TReceive
    >(this IEndpointRouteBuilder app, string group, string path, Action<TLogic, TReceive> handler)
        where TLogic : LogicBase, new()
    {
        return new WebsocketController<TLogic, TReceive, ValueForUser<TReceive>>(
            app,
            group,
            path,
            (logic, username, receive) =>
            {
                handler(logic, receive);
                return new ValueForUser<TReceive>(username, receive);
            }
        );
    }

    public static WebsocketController<TLogic, TReceive, TSend> UseWebsocket<
        TLogic,
        TReceive,
        TSend
    >(this IEndpointRouteBuilder app, string group, string path, Func<TLogic, TReceive, TSend?> handler)
        where TLogic : LogicBase, new()
    {
        return new WebsocketController<TLogic, TReceive, TSend>(
            app,
            group,
            path,
            (logic, _, receive) => handler(logic, receive)
        );
    }

    public static WebsocketController<
        User,
        TWebsocket,
        ValueForUser<TWebsocket>
    > UseGetAndWebsocket<TWebsocket>(
        this IEndpointRouteBuilder app,
        string group,
        string path,
        Func<User, TWebsocket> getter,
        Action<User, TWebsocket> handler
    )
    {
        app.UseGet(group, path, (User logic) => new ValueForUser<TWebsocket>(logic.Name(), getter(logic)));
        return app.UseWebsocket(group, path, handler);
    }

    public static WebsocketController<
        TLogic,
        TWebsocket,
        ValueForUser<TWebsocket>
    > UseGetAndWebsocket<TLogic, TGet, TWebsocket>(
        this IEndpointRouteBuilder app,
        string group,
        string path,
        Func<TLogic, TGet> getter,
        Action<TLogic, TWebsocket> handler
    )
        where TLogic : LogicBase, new()
    {
        app.UseGet(group, path, getter);
        return app.UseWebsocket(group, path, handler);
    }

    public static WebsocketController<TLogic, TReceive, TSend> UseGetAndWebsocket<
        TLogic,
        TReceive,
        TSend
    >(
        this IEndpointRouteBuilder app,
        string group,
        string path,
        Func<TLogic, TSend> getter,
        Func<TLogic, TReceive, TSend?> handler
    )
        where TLogic : LogicBase, new()
    {
        app.UseGet(group, path, getter);
        return app.UseWebsocket(group, path, handler);
    }
}

public class ValueForUser<T>(string user, T value)
{
    public string User { get; set; } = user;
    public T Value { get; set; } = value;
}