using Backend_Example.Database;
using Backend_Example.Logic;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Example
{
    public static class Helpers
    {
        public static int ToCents(this decimal value) => (int)(value * 100);

        public static decimal ToMoney(this int value) => value / 100m;

        public static RouteHandlerBuilder MapOut<TL, T>(
            this IEndpointRouteBuilder app,
            string path,
            Func<TL, T> getter
        )
            where TL : LogicBase, new()
        {
            return app.MapGet(
                    $"/{path}",
                    (HttpContext context, PotluckDb db) =>
                    {
                        var user = db.GetUser(context);
                        if (user == null)
                            return Results.Json(default(T));
                        var logic = LogicBase.Create<TL>(user, db);
                        return Results.Json(getter(logic));
                    }
                )
                .WithName(path[0].ToString().ToUpper() + path[1..])
                .WithOpenApi();
        }

        public static RouteHandlerBuilder MapIn<TL, T>(
            this IEndpointRouteBuilder app,
            string path,
            Func<T, TL, bool> setter
        )
            where TL : LogicBase, new()
        {
            return app.MapPost(
                    $"/{path}",
                    ([FromBody] T value, HttpContext context, PotluckDb db) =>
                    {
                        var user = db.GetUser(context);
                        if (user == null)
                            return Results.Json(false);
                        var logic = LogicBase.Create<TL>(user, db);
                        var result = setter(value, logic);
                        return Results.Json(result);
                    }
                )
                .WithName(path[0].ToString().ToUpper() + path[1..])
                .WithOpenApi();
        }

        public static RouteHandlerBuilder MapIn<TL, T>(
            this IEndpointRouteBuilder app,
            string path,
            Action<T, TL> setter
        )
            where TL : LogicBase, new() =>
            app.MapIn<TL, T>(
                path,
                (value, logic) =>
                {
                    setter(value, logic);
                    return true;
                }
            );

        public static (RouteHandlerBuilder get, RouteHandlerBuilder post) MapInOut<TL, T>(
            this IEndpointRouteBuilder app,
            string path,
            Func<TL, T> getter,
            Func<T, TL, bool> setter
        )
            where TL : LogicBase, new()
        {
            return (
                app.MapGet(
                        $"/{path}",
                        (HttpContext context, PotluckDb db) =>
                        {
                            var user = db.GetUser(context);
                            if (user == null)
                                return Results.Json(default(T));
                            var logic = LogicBase.Create<TL>(user, db);
                            return Results.Json(getter(logic));
                        }
                    )
                    .WithName(path[0].ToString().ToUpper() + path[1..])
                    .WithOpenApi(),
                app.MapPost(
                        $"/{path}",
                        ([FromBody] T value, HttpContext context, PotluckDb db) =>
                        {
                            var user = db.GetUser(context);
                            if (user == null)
                                return Results.Json(value);
                            var logic = LogicBase.Create<TL>(user, db);
                            var result = setter(value, logic);
                            return Results.Json(result ? getter(logic) : value);
                        }
                    )
                    .WithName("Set" + path[0].ToString().ToUpper() + path[1..])
                    .WithOpenApi()
            );
        }

        public static (RouteHandlerBuilder get, RouteHandlerBuilder post) MapInOut<TL, T>(
            this IEndpointRouteBuilder app,
            string path,
            Func<TL, T> getter,
            Action<T, TL> setter
        )
            where TL : LogicBase, new() =>
            app.MapInOut(
                path,
                getter,
                (value, logic) =>
                {
                    setter(value, logic);
                    return true;
                }
            );
    }
}
