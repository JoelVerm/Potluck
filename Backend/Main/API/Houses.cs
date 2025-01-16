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
        app.MapPost("/houses", (IPotluckDb db, [FromBody] NamedItem newHouse) =>
            {
                var username = GetUserName();
                var status = House.CreateNew(db, username, newHouse.Name);
                return status ? Results.Created() : Results.Conflict();
            }).Accepts<NamedItem>("application/json")
            .Produces(201);

        app.MapGet("/houses/{name}/users", (string name, IPotluckDb db) =>
            {
                var h = db.GetHouse(name);
                if (h == null)
                    return Results.NotFound();
                if (!h.IsMember(GetUserName()))
                    return Results.Forbid();
                return JSON(new HouseNames(h.GetMembers().Select(i => new NamedItem(i))));
            }).Produces<HouseNames>()
            .Produces(403)
            .Produces(404);
        app.MapPost("/houses/{name}/users", (string name, IPotluckDb db, [FromBody] NamedItem user) =>
            {
                var h = db.GetHouse(name);
                if (h == null)
                    return Results.NotFound();
                if (!h.IsMember(GetUserName())) return Results.Forbid();
                h.AddUser(user.Name);
                return Results.Created();
            }).Accepts<NamedItem>("application/json")
            .Produces(403)
            .Produces(404)
            .Produces(201);
        app.MapDelete("/houses/{name}/users/{username}",
                (string name, string username, IPotluckDb db) =>
                {
                    var h = db.GetHouse(name);
                    if (h == null)
                        return Results.NotFound();
                    if (!h.IsMember(GetUserName())) return Results.Forbid();
                    h.RemoveUser(username);
                    return Results.NoContent();
                }).Produces(204)
            .Produces(403)
            .Produces(404);

        app.MapGet("/houses/{name}/users/homeStatus", (string name, IPotluckDb db) =>
            {
                var h = db.GetHouse(name);
                if (h == null)
                    return Results.NotFound();
                if (!h.IsMember(GetUserName()))
                    return Results.Forbid();
                return JSON(h.GetHomeStatusList());
            }).Produces<Dictionary<string, string>>()
            .Produces(403)
            .Produces(404);

        app.MapGet(
                "/houses/{name}/transactions",
                (string name, IPotluckDb db) =>
                {
                    var h = db.GetHouse(name);
                    if (h == null)
                        return Results.NotFound();
                    if (!h.IsMember(GetUserName()))
                        return Results.Forbid();
                    return JSON(new TransactionsList(db.GetTransactionsForHouse(name)));
                }
            ).Produces<TransactionsList>()
            .Produces(403)
            .Produces(404);
        app.MapPost(
                "/houses/{name}/transactions",
                (string name, [FromBody] NewTransactionVM newTransaction, IPotluckDb db) =>
                {
                    var h = db.GetHouse(name);
                    if (h == null)
                        return Results.NotFound();
                    if (!h.IsMember(GetUserName())) return Results.Forbid();
                    var transaction = new Transaction(newTransaction.EuroCents, newTransaction.CookingPoints,
                        newTransaction.Description, newTransaction.IsPenalty, newTransaction.ToUser,
                        newTransaction.FromUsers);
                    db.AddTransactionToHouse(transaction, name);
                    return Results.Created();
                }
            ).Accepts<Transaction>("application/json")
            .Produces(403)
            .Produces(404)
            .Produces(201);

        app.MapGet("/houses/{name}/users/eating", (string name, IPotluckDb db) =>
            {
                var h = db.GetHouse(name);
                if (h == null)
                    return Results.NotFound();
                if (!h.IsMember(GetUserName()))
                    return Results.Forbid();
                return JSON(new EatingUsers(h.GetEatingList()));
            }).Produces<EatingUsers>()
            .Produces(403)
            .Produces(404);

        app.MapGet("/houses/{name}/dinner", (string name, IPotluckDb db) =>
            {
                var h = db.GetHouse(name);
                if (h == null)
                    return Results.NotFound();
                if (!h.IsMember(GetUserName()))
                    return Results.Forbid();
                return JSON(new DinnerInfo(h.CookingPrice, h.CookingDescription));
            }).Produces<DinnerInfo>()
            .Produces(403)
            .Produces(404);
        app.MapPut("/houses/{name}/dinner", (string name, IPotluckDb db, DinnerInfo info) =>
            {
                var h = db.GetHouse(name);
                if (h == null)
                    return Results.NotFound();
                var username = GetUserName();
                if (!h.IsMember(username)) return Results.Forbid();
                h.SetCookingPrice(info.CentsPrice);
                h.SetCookingDescription(info.Description);
                return Results.Created();
            }).Accepts<DinnerInfo>("application/json")
            .Produces(403)
            .Produces(404)
            .Produces(201);

        var cookingUserWS = new WebsocketController<bool, string>(app, "/houses/{name}/cookingUserWS");
        app.MapGet(
            cookingUserWS.Path,
            async (string name, HttpContext context, IPotluckDb db) =>
            {
                var h = db.GetHouse(name);
                if (h == null)
                    return Results.NotFound();
                var username = GetUserName();
                if (!h.IsMember(username)) return Results.Forbid();
                return await cookingUserWS.Handle(context, h.Id, cooking =>
                        h.SetCookingUser(cooking ? username : null),
                    () => h.CookingUser ?? ""
                );
            }
        );

        var shoppingListWS = new WebsocketController<string, string>(app, "/houses/{name}/shoppingListWS");
        app.MapGet(
            shoppingListWS.Path,
            async (string name, HttpContext context, IPotluckDb db) =>
            {
                var h = db.GetHouse(name);
                if (h == null)
                    return Results.NotFound();
                var username = GetUserName();
                if (!h.IsMember(username)) return Results.Forbid();
                return await shoppingListWS.Handle(context, h.Id, list =>
                        h.SetShoppingList(list),
                    () => h.ShoppingList
                );
            }
        );

        return app;
    }
}