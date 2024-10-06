using Backend_Example.Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Example
{
    public static class Shopping
    {
        public static void SetupShoppingRoutes(this IEndpointRouteBuilder app)
        {
            app.MapInOut(
                "shoppingList",
                (House house) => house.ShoppingList(),
                (shoppingList, house) => house.SetShoppingList(shoppingList)
            );

            app.MapOut("allPeople", (House house) => house.AllPeople());

            app.MapOut(
                "transactions",
                (Transactions transactions) => transactions.AllTransactions()
            );

            app.MapIn(
                "addTransaction",
                (Transactions.Transaction transaction, Transactions transactions) =>
                    transactions.AddTransaction(transaction)
            );
        }
    }
}
