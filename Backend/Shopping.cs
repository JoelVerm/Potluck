using Backend_Example.Database;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Example
{
    public static class Shopping
    {
        private class Transaction(
            string to,
            string[] from,
            string description,
            decimal money,
            int points
        )
        {
            public string To { get; set; } = to;
            public string[] From { get; set; } = from;
            public string Description { get; set; } = description;
            public decimal Money { get; set; } = money;
            public int Points { get; set; } = points;
        }

        public static void SetupShoppingRoutes(this IEndpointRouteBuilder app)
        {
            app.MapInOut(
                "shoppingList",
                user => user.House?.ShoppingList ?? "",
                (shoppingList, user) =>
                {
                    if (user.House != null)
                        user.House.ShoppingList = shoppingList;
                }
            );

            app.MapOut(
                "allPeople",
                user => user.House?.Users.Select(u => u.UserName).ToArray() ?? []
            );

            app.MapOut(
                "transactions",
                user =>
                    user.House?.Transactions.Select(t => new Transaction(
                            t.ToUser?.UserName ?? (t.IsPenalty ? "Penalty" : ""),
                            t.Users.Select(u => u?.UserName ?? "").ToArray(),
                            t.Description,
                            t.EuroCents.ToMoney(),
                            t.CookingPoints
                        ))
                        .ToArray() ?? []
            );

            app.MapIn<Transaction>(
                "addTransaction",
                (transaction, user) =>
                {
                    if (user.House == null)
                        return false;
                    return user.House.AddTransaction(
                        user.UserName,
                        transaction.From,
                        transaction.Description,
                        transaction.Money,
                        transaction.Points
                    );
                }
            );
        }
    }
}
