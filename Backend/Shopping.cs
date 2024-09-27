using Backend_Example.Database;

namespace Backend_Example
{
    public static class Shopping
    {
        private class Transaction(string to, string[] from, string description, decimal money, int points)
        {
            public string To { get; set; } = to;
            public string[] From { get; set; } = from;
            public string Description { get; set; } = description;
            public decimal Money { get; set; } = money;
            public int Points { get; set; } = points;
        }

        public static void SetupShoppingRoutes(this IEndpointRouteBuilder app)
        {
            app.MapGet("/shoppingList", (HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                return user?.House?.ShoppingList ?? "";
            }).WithName("ShoppingList").WithOpenApi();

            app.MapPost("/shoppingList", (string shoppingList, HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                var house = user?.House;
                if (house == null)
                    return "";
                house.ShoppingList = shoppingList;
                db.SaveChanges();
                return house.ShoppingList;
            }).WithName("SetShoppingList").WithOpenApi();

            app.MapGet("/allPeople", (HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                return user?.House?.Users.Select(u => u.UserName).ToArray() ?? [];
            }).WithName("AllPeople").WithOpenApi();

            app.MapGet("/transactions", (HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                return user?.House?.Transactions
                    .Select(t => new Transaction(
                        t.ToUser?.UserName ?? (t.IsPenalty ? "Penalty" : ""),
                        t.Users.Select(u => u?.UserName ?? "").ToArray(),
                        t.Description, (decimal)t.EuroCents / 100,
                        t.CookingPoints
                    )
                ).ToArray() ?? [];
            }).WithName("Transactions").WithOpenApi();

            app.MapPost("/addTransaction", (Transaction transaction, HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                if (user == null)
                    return false;
                var house = user?.House;
                if (house == null)
                    return false;
                var success = house.AddTransaction(user.UserName, transaction.From, transaction.Description, transaction.Money, transaction.Points);
                db.SaveChanges();
                return success;
            }).WithName("AddTransaction").WithOpenApi();

        }
    }
}
