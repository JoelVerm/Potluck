using System.Text.Json;
using Data;
using Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Potluck.Helpers;

public static class UseGetPostExtensions
{
    private static readonly JsonSerializerOptions jsonSerializerOptions =
        new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

    public static RouteHandlerBuilder UseGet<TLogic, TGet>(
        this IEndpointRouteBuilder app,
        string group,
        string path,
        Func<TLogic, TGet> getter
    )
        where TLogic : LogicBase, new()
    {
        return app.MapGet(
                $"/{group}/{path}",
                (HttpContext context, PotluckDb db) =>
                {
                    var user = db.GetUser(context.User.Identity!.Name);
                    if (user == null)
                        return Results.Unauthorized();
                    var logic = LogicBase.Create<TLogic>(user, db);
                    return Results.Json(getter(logic), jsonSerializerOptions);
                }
            )
            .WithName(Name("Get", group, path))
            .WithTags(Capital(group))
            .Produces<TGet>()
            .ProducesProblem(401)
            .WithOpenApi();
    }

    public static RouteHandlerBuilder UsePost<TLogic, TPost>(
        this IEndpointRouteBuilder app,
        string group,
        string path,
        Func<TPost, TLogic, bool> setter
    )
        where TLogic : LogicBase, new()
        where TPost : notnull
    {
        return app.MapPost(
                $"/{group}/{path}",
                ([FromBody] TPost value, HttpContext context, PotluckDb db) =>
                {
                    var user = db.GetUser(context.User.Identity!.Name);
                    if (user == null)
                        return Results.Unauthorized();
                    var logic = LogicBase.Create<TLogic>(user, db);
                    var result = setter(value, logic);
                    return Results.Json(result, jsonSerializerOptions);
                }
            )
            .WithName(Name("Post", group, path))
            .WithTags(Capital(group))
            .Accepts<TPost>("application/json")
            .Produces<bool>()
            .ProducesProblem(401)
            .WithOpenApi();
    }

    public static RouteHandlerBuilder UsePost<TLogic, TPost>(
        this IEndpointRouteBuilder app,
        string group,
        string path,
        Action<TPost, TLogic> setter
    )
        where TLogic : LogicBase, new()
        where TPost : notnull
    {
        return app.UsePost<TLogic, TPost>(
            group,
            path,
            (value, logic) =>
            {
                setter(value, logic);
                return true;
            }
        );
    }

    public static (RouteHandlerBuilder get, RouteHandlerBuilder post) UseGetPut<
        TLogic,
        TGetPost
    >(
        this IEndpointRouteBuilder app,
        string group,
        string path,
        Func<TLogic, TGetPost> getter,
        Func<TGetPost, TLogic, bool> setter
    )
        where TLogic : LogicBase, new()
        where TGetPost : notnull
    {
        return (
            app.MapGet(
                    $"/{group}/{path}",
                    (HttpContext context, PotluckDb db) =>
                    {
                        var user = db.GetUser(context.User.Identity!.Name);
                        if (user == null)
                            return Results.Unauthorized();
                        var logic = LogicBase.Create<TLogic>(user, db);
                        return Results.Json(getter(logic), jsonSerializerOptions);
                    }
                )
                .WithName(Name("Get", group, path))
                .WithTags(Capital(group))
                .Produces<TGetPost>()
                .ProducesProblem(401)
                .WithOpenApi(),
            app.MapPut(
                    $"/{group}/{path}",
                    ([FromBody] TGetPost value, HttpContext context, PotluckDb db) =>
                    {
                        var user = db.GetUser(context.User.Identity!.Name);
                        if (user == null)
                            return Results.Unauthorized();
                        var logic = LogicBase.Create<TLogic>(user, db);
                        var result = setter(value, logic);
                        return Results.Json(result ? getter(logic) : value, jsonSerializerOptions);
                    }
                )
                .WithName(Name("Put", group, path))
                .WithTags(Capital(group))
                .Accepts<TGetPost>("application/json")
                .Produces<TGetPost>()
                .ProducesProblem(401)
                .WithOpenApi()
        );
    }

    public static (RouteHandlerBuilder get, RouteHandlerBuilder post) UseGetPut<
        TLogic,
        TGetPost
    >(
        this IEndpointRouteBuilder app,
        string group,
        string path,
        Func<TLogic, TGetPost> getter,
        Action<TGetPost, TLogic> setter
    )
        where TLogic : LogicBase, new()
        where TGetPost : notnull
    {
        return app.UseGetPut(
            group,
            path,
            getter,
            (value, logic) =>
            {
                setter(value, logic);
                return true;
            }
        );
    }

    public static RouteHandlerBuilder UseDelete<TLogic, TDelete>(
        this IEndpointRouteBuilder app,
        string group,
        string path,
        Func<TDelete, TLogic, bool> setter
    )
        where TLogic : LogicBase, new()
        where TDelete : notnull
    {
        return app.MapDelete(
                $"/{group}/{path}",
                ([FromRoute] TDelete name, HttpContext context, PotluckDb db) =>
                {
                    var user = db.GetUser(context.User.Identity!.Name);
                    if (user == null)
                        return Results.Unauthorized();
                    var logic = LogicBase.Create<TLogic>(user, db);
                    var result = setter(name, logic);
                    return Results.Json(result, jsonSerializerOptions);
                }
            )
            .WithName(Name("Delete", group, path))
            .WithTags(Capital(group))
            .Accepts<TDelete>("application/json")
            .Produces<bool>()
            .ProducesProblem(401)
            .WithOpenApi();
    }

    public static RouteHandlerBuilder UseDelete<TLogic, TDelete>(
        this IEndpointRouteBuilder app,
        string group,
        string path,
        Action<TDelete, TLogic> setter
    )
        where TLogic : LogicBase, new()
        where TDelete : notnull
    {
        return app.UseDelete<TLogic, TDelete>(
            group,
            path,
            (value, logic) =>
            {
                setter(value, logic);
                return true;
            }
        );
    }

    public static string Name(string method, string group, string path)
    {
        return $"{method}{Capital(group)}{string.Join("", path.Split("/").Select(Capital))}";
    }

    public static string Capital(string value)
    {
        if (value.IsNullOrEmpty())
            return "";
        if (value.Length == 1)
            return value.ToUpper();
        return value[0].ToString().ToUpper() + value[1..];
    }
}