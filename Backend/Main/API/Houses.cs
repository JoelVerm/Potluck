using Logic;
using Potluck.Helpers;

namespace Potluck.API;

public static class Houses
{
    public static void SetupHousesRoutes(this IEndpointRouteBuilder app)
    {
        WebsocketController<House, string, string>? houseName = null;

        // Many

        app.UsePost("houses", "", (string name, House house) =>
        {
            var id = house.CreateNew(name);
            houseName?.BroadcastMessage(name, id).Wait();
        });

        // One

        app.UseGet("houses", "current/users", (House house) => house.AllPeople());
        app.UsePost("houses", "current/users", (string name, House house) => house.AddUser(name));
        app.UseDelete("houses", "current/users/{name}", (string name, House house) => house.RemoveUser(name));

        app.UseGet(
            "houses", "current/transactions",
            (Transactions transactions) => transactions.AllTransactions()
        );
        app.UsePost(
            "houses", "current/transactions",
            (Transactions.Transaction transaction, Transactions transactions) =>
                transactions.AddTransaction(transaction)
        );

        app.UseGet("houses", "current/users/eating", (House house) => house.EatingList());

        app.UseGetPut(
            "houses", "current/dinner/price",
            (House house) => house.CookingPrice(),
            (total, house) => house.SetCookingPrice(total)
        );
        app.UseGetPut(
            "houses", "current/dinner/description",
            (House house) => house.CookingDescription(),
            (desc, house) => house.SetCookingDescription(desc)
        );

        app.UseGetAndWebsocket(
            "houses", "current/users/cooking",
            house => house.CookingUser(),
            (House house, bool cooking) =>
            {
                house.SetUserCooking(cooking);
                return house.CookingUser();
            }
        );

        houseName = app.UseGetAndWebsocket(
            "houses", "current/name",
            house => house.HouseName(),
            (House house, string name) =>
            {
                house.SetHouseName(name);
                return name;
            }
        );

        app.UseGetAndWebsocket(
            "houses", "current/shoppingList",
            house => house.ShoppingList(),
            (House house, string shoppingList) =>
            {
                house.SetShoppingList(shoppingList);
                return shoppingList;
            }
        );
    }
}