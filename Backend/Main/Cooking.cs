using Backend_Example.Logic;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Example
{
    public static class Cooking
    {
        public static void SetupCookingRoutes(this IEndpointRouteBuilder app)
        {
            app.MapOut("cookingUser", (House house) => house.CookingUser());

            app.MapInOut(
                "cooking",
                (House house) => house.IsUserCooking(),
                (cooking, house) => house.SetUserCooking(cooking)
            );

            app.MapInOut(
                "cookingTotal",
                (House house) => house.CookingPrice(),
                (total, house) => house.SetCookingPrice(total)
            );

            app.MapInOut(
                "cookingDescription",
                (House house) => house.CookingDescription(),
                (desc, house) => house.SetCookingDescription(desc)
            );

            app.MapOut("eatingList", (House house) => house.EatingList());
        }
    }
}
