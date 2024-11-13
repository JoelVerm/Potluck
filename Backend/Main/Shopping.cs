using Logic;

namespace Potluck
{
    public static class Shopping
    {
        public static void SetupShoppingRoutes(this IEndpointRouteBuilder app)
        {
            app.UseGetPost(
                "shoppingList",
                (House house) => house.ShoppingList(),
                (shoppingList, house) => house.SetShoppingList(shoppingList)
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
}
