using Backend_Example.Database;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Example
{
    public static class Helpers
    {
        public static int ToCents(this decimal value) => (int)(value * 100);

        public static decimal ToMoney(this int value) => value / 100m;

        public static RouteHandlerBuilder MapOut<T>(
            this IEndpointRouteBuilder app,
            string path,
            Func<User, T> getter
        )
        {
            return app.MapGet(
                    $"/{path}",
                    (HttpContext context, PotluckDb db) =>
                    {
                        var user = db.GetUser(context);
                        if (user == null)
                            return Results.Json(default(T));
                        return Results.Json(getter(user));
                    }
                )
                .WithName(path[0].ToString().ToUpper() + path[1..])
                .WithOpenApi();
        }

        public static RouteHandlerBuilder MapIn<T>(
            this IEndpointRouteBuilder app,
            string path,
            Func<T, User, bool> setter
        )
        {
            return app.MapPost(
                    $"/{path}",
                    ([FromBody] T value, HttpContext context, PotluckDb db) =>
                    {
                        var user = db.GetUser(context);
                        if (user == null)
                            return Results.Json(false);
                        var result = setter(value, user);
                        db.SaveChanges();
                        return Results.Json(result);
                    }
                )
                .WithName(path[0].ToString().ToUpper() + path[1..])
                .WithOpenApi();
        }

        public static (RouteHandlerBuilder get, RouteHandlerBuilder post) MapInOut<T>(
            this IEndpointRouteBuilder app,
            string path,
            Func<User, T> getter,
            Func<T, User, bool> setter
        )
        {
            return (
                app.MapGet(
                        $"/{path}",
                        (HttpContext context, PotluckDb db) =>
                        {
                            var user = db.GetUser(context);
                            if (user == null)
                                return Results.Json(default(T));
                            return Results.Json(getter(user));
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
                            var result = setter(value, user);
                            db.SaveChanges();
                            return Results.Json(result ? getter(user) : value);
                        }
                    )
                    .WithName("Set" + path[0].ToString().ToUpper() + path[1..])
                    .WithOpenApi()
            );
        }

        public static (RouteHandlerBuilder get, RouteHandlerBuilder post) MapInOut<T>(
            this IEndpointRouteBuilder app,
            string path,
            Func<User, T> getter,
            Action<T, User> setter
        ) =>
            app.MapInOut(
                path,
                getter,
                (value, user) =>
                {
                    setter(value, user);
                    return true;
                }
            );
    }
}
