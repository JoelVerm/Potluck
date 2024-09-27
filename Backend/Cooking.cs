using Backend_Example.Database;
using Microsoft.AspNetCore.Mvc;

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
                return Results.Json(user?.House?.CookingUser == user);
            }).WithName("Cooking").WithOpenApi();

            app.MapPost("/cooking", ([FromBody] bool cooking, HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                cooking = user?.SetCookingStatus(cooking) ?? false;
                db.SaveChanges();
                return Results.Json(cooking);
            }).WithName("SetCooking").WithOpenApi();

            app.MapGet("/cookingTotal", (HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                return Results.Json(user?.House?.CookingPrice.ToMoney() ?? 0);
            }).WithName("CookingTotal").WithOpenApi();

            app.MapPost("/cookingTotal", ([FromBody] decimal total, HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                if (user == null)
                    return Results.Json(0);
                var house = user?.House;
                if (house == null)
                    return Results.Json(0);
                if (!user!.IsCooking())
                    return Results.Json(house.CookingPrice.ToMoney());
                house.CookingPrice = total.ToCents();
                db.SaveChanges();
                return Results.Json(house.CookingPrice.ToMoney());
            }).WithName("SetCookingTotal").WithOpenApi();

            app.MapGet("/cookingDescription", (HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                return Results.Json(user?.House?.CookingDescription ?? "");
            }).WithName("CookingDescription").WithOpenApi();

            app.MapPost("/cookingDescription", ([FromBody] string desc, HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                var house = user?.House;
                if (house == null)
                    return Results.Json("");
                if (user?.IsCooking() != true)
                    return Results.Json(house.CookingDescription);
                house.CookingDescription = desc;
                db.SaveChanges();
                return Results.Json(house.CookingDescription);
            }).WithName("SetCookingDescription").WithOpenApi();

            app.MapGet("/eatingList", (HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                return Results.Json(user?.House?.Users
                    .Where(u => u.EatingTotalPeople > 0)
                    .Select(u => new EatingPerson(
                        u.UserName ?? "",
                        u.EatingTotalPeople,
                        u.CookingPoints(),
                        u.Diet
                    )
                ).ToArray() ?? []);
            }).WithName("EatingList").WithOpenApi();
        }
    }
}
