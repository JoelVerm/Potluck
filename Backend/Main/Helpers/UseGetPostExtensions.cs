using Data;
using Logic;
using Microsoft.AspNetCore.Mvc;

namespace Potluck.Helpers;

public static class UseGetPostExtensions
{
    public static RouteHandlerBuilder UseGet<TLogic, TGet>(
        this IEndpointRouteBuilder app,
        string path,
        Func<TLogic, TGet> getter
    )
        where TLogic : LogicBase, new()
    {
        return app.MapGet(
                $"/{path}",
                (HttpContext context, PotluckDb db) =>
                {
                    var user = db.GetUser(context.User.Identity!.Name);
                    if (user == null)
                        return Results.Unauthorized();
                    var logic = LogicBase.Create<TLogic>(user, db);
                    return Results.Json(getter(logic));
                }
            )
            .WithName(path[0].ToString().ToUpper() + path[1..])
            .Produces<TGet>()
            .ProducesProblem(401)
            .WithOpenApi();
    }

    public static RouteHandlerBuilder UsePost<TLogic, TPost>(
        this IEndpointRouteBuilder app,
        string path,
        Func<TPost, TLogic, bool> setter
    )
        where TLogic : LogicBase, new()
    {
        return app.MapPost(
                $"/{path}",
                ([FromBody] TPost value, HttpContext context, PotluckDb db) =>
                {
                    var user = db.GetUser(context.User.Identity!.Name);
                    if (user == null)
                        return Results.Unauthorized();
                    var logic = LogicBase.Create<TLogic>(user, db);
                    var result = setter(value, logic);
                    return Results.Json(result);
                }
            )
            .WithName(path[0].ToString().ToUpper() + path[1..])
            .Accepts<TPost>("application/json")
            .Produces<bool>()
            .ProducesProblem(401)
            .WithOpenApi();
    }

    public static RouteHandlerBuilder UsePost<TLogic, TPost>(
        this IEndpointRouteBuilder app,
        string path,
        Action<TPost, TLogic> setter
    )
        where TLogic : LogicBase, new()
    {
        return app.UsePost<TLogic, TPost>(
            path,
            (value, logic) =>
            {
                setter(value, logic);
                return true;
            }
        );
    }

    public static (RouteHandlerBuilder get, RouteHandlerBuilder post) UseGetPost<
        TLogic,
        TGetPost
    >(
        this IEndpointRouteBuilder app,
        string path,
        Func<TLogic, TGetPost> getter,
        Func<TGetPost, TLogic, bool> setter
    )
        where TLogic : LogicBase, new()
    {
        return (
            app.MapGet(
                    $"/{path}",
                    (HttpContext context, PotluckDb db) =>
                    {
                        var user = db.GetUser(context.User.Identity!.Name);
                        if (user == null)
                            return Results.Unauthorized();
                        var logic = LogicBase.Create<TLogic>(user, db);
                        return Results.Json(getter(logic));
                    }
                )
                .WithName(path[0].ToString().ToUpper() + path[1..])
                .Produces<TGetPost>()
                .ProducesProblem(401)
                .WithOpenApi(),
            app.MapPost(
                    $"/{path}",
                    ([FromBody] TGetPost value, HttpContext context, PotluckDb db) =>
                    {
                        var user = db.GetUser(context.User.Identity!.Name);
                        if (user == null)
                            return Results.Unauthorized();
                        var logic = LogicBase.Create<TLogic>(user, db);
                        var result = setter(value, logic);
                        return Results.Json(result ? getter(logic) : value);
                    }
                )
                .WithName("Set" + path[0].ToString().ToUpper() + path[1..])
                .Accepts<TGetPost>("application/json")
                .Produces<TGetPost>()
                .ProducesProblem(401)
                .WithOpenApi()
        );
    }

    public static (RouteHandlerBuilder get, RouteHandlerBuilder post) UseGetPost<
        TLogic,
        TGetPost
    >(
        this IEndpointRouteBuilder app,
        string path,
        Func<TLogic, TGetPost> getter,
        Action<TGetPost, TLogic> setter
    )
        where TLogic : LogicBase, new()
    {
        return app.UseGetPost(
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