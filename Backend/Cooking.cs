using Backend_Example.Database;

namespace Backend_Example
{
    public static class Cooking
    {
        private class EatingPerson(string name, int count, int cookingPoints, string diet)
        {
            public string Name { get; set; } = name;
            public int Count { get; set; } = count;
            public int CookingPoints { get; set; } = cookingPoints;
            public string Diet { get; set; } = diet;
        }

        public static void SetupCookingRoutes(this IEndpointRouteBuilder app)
        {
            app.MapGet("/cooking", (HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                return user?.House?.CookingUser == user;
            }).WithName("Cooking").WithOpenApi();

            app.MapPost("/cooking", (bool cooking, HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                cooking = user?.SetCookingStatus(cooking) ?? false;
                db.SaveChanges();
                return cooking;
            }).WithName("SetCooking").WithOpenApi();

            app.MapGet("/cookingTotal", (HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                return user?.House?.CookingPrice.ToMoney() ?? 0;
            }).WithName("CookingTotal").WithOpenApi();

            app.MapPost("/cookingTotal", (decimal total, HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                var house = user?.House;
                if (house == null)
                    return 0;
                if (house.CookingUser != user)
                    return house.CookingPrice.ToMoney();
                house.CookingPrice = total.ToCents();
                db.SaveChanges();
                return house.CookingPrice.ToMoney();
            }).WithName("SetCookingTotal").WithOpenApi();

            app.MapGet("/cookingDescription", (HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                return user?.House?.CookingDescription ?? "";
            }).WithName("CookingDescription").WithOpenApi();

            app.MapPost("/cookingDescription", (string desc, HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                var house = user?.House;
                if (house == null)
                    return "";
                if (user?.IsCooking() != true)
                    return house.CookingDescription;
                house.CookingDescription = desc;
                db.SaveChanges();
                return house.CookingDescription;
            }).WithName("SetCookingDescription").WithOpenApi();

            app.MapGet("/eatingList", (HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                return user?.House?.Users
                    .Where(u => u.EatingTotalPeople > 0)
                    .Select(u => new EatingPerson(
                        u.UserName ?? "",
                        u.EatingTotalPeople,
                        u.CookingPoints(),
                        u.Diet
                    )
                ).ToArray();
            }).WithName("EatingList").WithOpenApi();
        }
    }
}
