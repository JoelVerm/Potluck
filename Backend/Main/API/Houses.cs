using Logic;
using Microsoft.AspNetCore.Mvc;
using Potluck.Helpers;
using Potluck.ViewModels;
using static Potluck.Helpers.RestHelpers;

namespace Potluck.API;

public static class Houses
{
    public static T SetupHousesRoutes<T>(this T app)
        where T : IEndpointRouteBuilder, IEndpointConventionBuilder
    {
        var houseNameWS = new WebsocketController<string, string>(app, "/houses/{name}/nameWS");
        app.MapGet(
            houseNameWS.Path,
            async (string name, HttpContext context, HouseLogic house) =>
            {
                var h = house.GetHouse(name);
                if (h == null)
                    return Results.NotFound();
                if (!h.IsAllowed(GetUserName())) return Results.Forbid();
                return await houseNameWS.Handle(context, h.GetId(), newName =>
                        h.SetHouseName(newName),
                    () => h.HouseName()
                );
            }
        );

        app.MapPost("/houses", (HouseLogic house, [FromBody] NamedItem newHouse) =>
            {
                var username = GetUserName();
                var status = house.CreateNew(username, newHouse.Name);
                return status switch
                {
                    HouseLogic.CreateHouseStatus.Exists => Results.Conflict(),
                    HouseLogic.CreateHouseStatus.UserNotFound => Results.NotFound(),
                    HouseLogic.CreateHouseStatus.Success => Results.Created(),
                    _ => throw new Exception("This is literally impossible")
                };
            }).Accepts<NamedItem>("application/json")
            .Produces(201);

        app.MapGet("/houses/{name}/users", (string name, HouseLogic house) =>
            {
                var h = house.GetHouse(name);
                if (h == null)
                    return Results.NotFound();
                if (!h.IsAllowed(GetUserName()))
                    return Results.Forbid();
                return JSON(new HouseNames(h.AllPeople().Select(i => new NamedItem(i))));
            }).Produces<HouseNames>()
            .Produces(403)
            .Produces(404);
        app.MapPost("/houses/{name}/users", (string name, HouseLogic house, [FromBody] NamedItem user) =>
            {
                var h = house.GetHouse(name);
                if (h == null)
                    return Results.NotFound();
                if (!h.IsAllowed(GetUserName())) return Results.Forbid();
                h.AddUser(user.Name);
                return Results.Created();
            }).Accepts<NamedItem>("application/json")
            .Produces(403)
            .Produces(404)
            .Produces(201);
        app.MapDelete("/houses/{name}/users/{username}",
                (string name, string username, HouseLogic house) =>
                {
                    var h = house.GetHouse(name);
                    if (h == null)
                        return Results.NotFound();
                    if (!h.IsAllowed(GetUserName())) return Results.Forbid();
                    h.RemoveUser(username);
                    return Results.NoContent();
                }).Produces(204)
            .Produces(403)
            .Produces(404);

        app.MapGet("/houses/{name}/users/homeStatus", (string name, HouseLogic house) =>
            {
                var h = house.GetHouse(name);
                if (h == null)
                    return Results.NotFound();
                if (!h.IsAllowed(GetUserName()))
                    return Results.Forbid();
                return JSON(h.HomeStatusList());
            }).Produces<Dictionary<string, string>>()
            .Produces(403)
            .Produces(404);

        app.MapGet(
                "/houses/{name}/transactions",
                (string name, HouseLogic house) =>
                {
                    var h = house.GetHouse(name);
                    if (h == null)
                        return Results.NotFound();
                    if (!h.IsAllowed(GetUserName()))
                        return Results.Forbid();
                    return JSON(new TransactionsList(h.AllTransactions()));
                }
            ).Produces<TransactionsList>()
            .Produces(403)
            .Produces(404);
        app.MapPost(
                "/houses/{name}/transactions",
                (string name, [FromBody] TransactionsLogic.Transaction transaction, HouseLogic house) =>
                {
                    var h = house.GetHouse(name);
                    if (h == null)
                        return Results.NotFound();
                    if (!h.IsAllowed(GetUserName())) return Results.Forbid();
                    h.AddTransaction(transaction);
                    return Results.Created();
                }
            ).Accepts<TransactionsLogic.Transaction>("application/json")
            .Produces(403)
            .Produces(404)
            .Produces(201);

        app.MapGet("/houses/{name}/users/eating", (string name, HouseLogic house) =>
            {
                var h = house.GetHouse(name);
                if (h == null)
                    return Results.NotFound();
                if (!h.IsAllowed(GetUserName()))
                    return Results.Forbid();
                return JSON(new EatingUsers(h.EatingList()));
            }).Produces<EatingUsers>()
            .Produces(403)
            .Produces(404);

        app.MapGet("/houses/{name}/dinner", (string name, HouseLogic house) =>
            {
                var h = house.GetHouse(name);
                if (h == null)
                    return Results.NotFound();
                if (!h.IsAllowed(GetUserName()))
                    return Results.Forbid();
                return JSON(new DinnerInfo(h.CookingPrice(), h.CookingDescription()));
            }).Produces<DinnerInfo>()
            .Produces(403)
            .Produces(404);
        app.MapPut("/houses/{name}/dinner", (string name, HouseLogic house, DinnerInfo info) =>
            {
                var h = house.GetHouse(name);
                if (h == null)
                    return Results.NotFound();
                var username = GetUserName();
                if (!h.IsAllowed(username)) return Results.Forbid();
                h.SetCookingPrice(username, info.Price);
                h.SetCookingDescription(username, info.Description);
                return Results.Created();
            }).Accepts<DinnerInfo>("application/json")
            .Produces(403)
            .Produces(404)
            .Produces(201);

        var cookingUserWS = new WebsocketController<bool, string>(app, "/houses/{name}/cookingUserWS");
        app.MapGet(
            cookingUserWS.Path,
            async (string name, HttpContext context, HouseLogic house) =>
            {
                var h = house.GetHouse(name);
                if (h == null)
                    return Results.NotFound();
                var username = GetUserName();
                if (!h.IsAllowed(username)) return Results.Forbid();
                return await cookingUserWS.Handle(context, h.GetId(), cooking =>
                        h.SetUserCooking(username, cooking),
                    () => h.CookingUser()
                );
            }
        );

        var shoppingListWS = new WebsocketController<string, string>(app, "/houses/{name}/shoppingListWS");
        app.MapGet(
            shoppingListWS.Path,
            async (string name, HttpContext context, HouseLogic house) =>
            {
                var h = house.GetHouse(name);
                if (h == null)
                    return Results.NotFound();
                var username = GetUserName();
                if (!h.IsAllowed(username)) return Results.Forbid();
                return await shoppingListWS.Handle(context, h.GetId(), list =>
                        h.SetShoppingList(list),
                    () => h.ShoppingList()
                );
            }
        );

        return app;
    }
}