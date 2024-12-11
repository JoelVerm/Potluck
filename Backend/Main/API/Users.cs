using Logic;
using Potluck.Helpers;
using Potluck.ViewModels;
using static Potluck.Helpers.RestHelpers;

namespace Potluck.API;

public static class Users
{
    public static T SetupUsersRoutes<T>(this T app)
        where T : IEndpointRouteBuilder, IEndpointConventionBuilder
    {
        app.MapGet(
                "/users/{name}/house",
                (string name, UserLogic user) =>
                {
                    var u = user.GetUser(name);
                    if (u == null)
                        return Results.NotFound();
                    if (GetUserName() != name)
                        return Results.Forbid();
                    return JSON(new HouseResponse(u.user.House?.Name ?? ""));
                }
            ).Produces<HouseResponse>()
            .Produces(403)
            .Produces(404);

        app.MapGet(
                "/users/{name}/balance",
                (string name, UserLogic user) =>
                {
                    var u = user.GetUser(name);
                    if (u == null)
                        return Results.NotFound();
                    if (GetUserName() != name)
                        return Results.Forbid();
                    return JSON(new TotalBalance(u.user.Balance()));
                }
            ).Produces<TotalBalance>()
            .Produces(403)
            .Produces(404);

        var eatingTotalPeopleWS = new WebsocketController<int, int>(app, "/users/{name}/eatingTotalPeopleWS");
        app.MapGet(
            eatingTotalPeopleWS.Path,
            async (string name, HttpContext context, UserLogic user) =>
            {
                var u = user.GetUser(name);
                if (u == null)
                    return Results.NotFound();
                if (GetUserName() != name)
                    return Results.Forbid();
                var houseId = u.HouseId();
                return await eatingTotalPeopleWS.Handle(context, houseId, total =>
                        u.SetEatingTotalPeople(total),
                    () => u.EatingTotalPeople()
                );
            }
        );

        var homeStatusWS = new WebsocketController<string, string>(app, "/users/{name}/homeStatusWS");
        app.MapGet(
            homeStatusWS.Path,
            async (string name, HttpContext context, UserLogic user) =>
            {
                var u = user.GetUser(name);
                if (u == null)
                    return Results.NotFound();
                if (GetUserName() != name)
                    return Results.Forbid();
                var houseId = u.HouseId();
                return await homeStatusWS.Handle(context, houseId, status =>
                        u.SetHomeStatus(status),
                    () => u.HomeStatus()
                );
            }
        );

        var dietWS = new WebsocketController<string, string>(app, "/users/{name}/dietWS");
        app.MapGet(
            dietWS.Path,
            async (string name, HttpContext context, UserLogic user) =>
            {
                var u = user.GetUser(name);
                if (u == null)
                    return Results.NotFound();
                if (GetUserName() != name)
                    return Results.Forbid();
                var houseId = u.HouseId();
                return await dietWS.Handle(context, houseId, diet =>
                        u.SetDiet(diet),
                    () => u.Diet()
                );
            }
        );

        return app;
    }
}