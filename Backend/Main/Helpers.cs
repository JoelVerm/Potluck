using Data;
using Logic;
using Microsoft.AspNetCore.Mvc;

namespace Potluck
{
    public static class Helpers
    {
        public static RouteHandlerBuilder UseGet<TL, T>(
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
                        var user = db.GetUser(context.User.Identity!.Name);
                        if (user == null)
                            return Results.Json(default(T));
                        var logic = LogicBase.Create<TL>(user, db);
                        return Results.Json(getter(logic));
                    }
                )
                .WithName(path[0].ToString().ToUpper() + path[1..]);
        }

        public static RouteHandlerBuilder UsePost<TL, T>(
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
                        var user = db.GetUser(context.User.Identity!.Name);
                        if (user == null)
                            return Results.Json(false);
                        var logic = LogicBase.Create<TL>(user, db);
                        var result = setter(value, logic);
                        return Results.Json(result);
                    }
                )
                .WithName(path[0].ToString().ToUpper() + path[1..]);
        }

        public static RouteHandlerBuilder UsePost<TL, T>(
            this IEndpointRouteBuilder app,
            string path,
            Action<T, TL> setter
        )
            where TL : LogicBase, new() =>
            app.UsePost<TL, T>(
                path,
                (value, logic) =>
                {
                    setter(value, logic);
                    return true;
                }
            );

        public static (RouteHandlerBuilder get, RouteHandlerBuilder post) UseGetPost<TL, T>(
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
                            var user = db.GetUser(context.User.Identity!.Name);
                            if (user == null)
                                return Results.Json(default(T));
                            var logic = LogicBase.Create<TL>(user, db);
                            return Results.Json(getter(logic));
                        }
                    )
                    .WithName(path[0].ToString().ToUpper() + path[1..]),
                app.MapPost(
                        $"/{path}",
                        ([FromBody] T value, HttpContext context, PotluckDb db) =>
                        {
                            var user = db.GetUser(context.User.Identity!.Name);
                            if (user == null)
                                return Results.Json(value);
                            var logic = LogicBase.Create<TL>(user, db);
                            var result = setter(value, logic);
                            return Results.Json(result ? getter(logic) : value);
                        }
                    )
                    .WithName("Set" + path[0].ToString().ToUpper() + path[1..])
            );
        }

        public static (RouteHandlerBuilder get, RouteHandlerBuilder post) UseGetPost<TL, T>(
            this IEndpointRouteBuilder app,
            string path,
            Func<TL, T> getter,
            Action<T, TL> setter
        )
            where TL : LogicBase, new() =>
            app.UseGetPost(
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
