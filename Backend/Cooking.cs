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
            app.MapOut("cookingUser", user => user.House?.CookingUser?.UserName ?? "");

            app.MapInOut(
                "cooking",
                user => user.House?.CookingUser == user,
                (cooking, user) => user.SetCookingStatus(cooking) || true
            );

            app.MapInOut(
                "cookingTotal",
                user => user.House?.CookingPrice.ToMoney() ?? 0,
                (total, user) =>
                {
                    if (user.IsCooking())
                        user.House!.CookingPrice = total.ToCents();
                }
            );

            app.MapInOut(
                "cookingDescription",
                user => user.House?.CookingDescription ?? "",
                (desc, user) =>
                {
                    if (user.IsCooking())
                        user.House!.CookingDescription = desc;
                }
            );

            app.MapOut(
                "eatingList",
                user =>
                    user.House?.Users.Where(u => u.EatingTotalPeople > 0)
                        .Select(u => new EatingPerson(
                            u.UserName ?? "",
                            u.EatingTotalPeople,
                            u.CookingPoints(),
                            u.Diet
                        ))
                        .ToArray() ?? []
            );
        }
    }
}
