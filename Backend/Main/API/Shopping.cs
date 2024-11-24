using Logic;
using Potluck.Helpers;

namespace Potluck.API;

public static class Shopping
{
    public static void SetupShoppingRoutes(this IEndpointRouteBuilder app)
    {
        app.UseGetAndWebsocket(
            "shoppingList",
            house => house.ShoppingList(),
            (House house, string shoppingList) =>
            {
                house.SetShoppingList(shoppingList);
                return shoppingList;
            }
        );

        app.UseGet("allPeople", (House house) => house.AllPeople());

        app.UseGet(
            "transactions",
            (Transactions transactions) => transactions.AllTransactions()
        );

        app.UsePost(
            "addTransaction",
            (Transactions.Transaction transaction, Transactions transactions) =>
                transactions.AddTransaction(transaction)
        );
    }
}