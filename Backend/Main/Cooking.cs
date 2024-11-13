using Logic;

namespace Potluck
{
    public static class Cooking
    {
        public static void SetupCookingRoutes(this IEndpointRouteBuilder app)
        {
            app.UseGet("cookingUser", (House house) => house.CookingUser());

            app.UseGetPost(
                "cooking",
                (House house) => house.IsUserCooking(),
                (cooking, house) => house.SetUserCooking(cooking)
            );

            app.UseGetPost(
                "cookingTotal",
                (House house) => house.CookingPrice(),
                (total, house) => house.SetCookingPrice(total)
            );

            app.UseGetPost(
                "cookingDescription",
                (House house) => house.CookingDescription(),
                (desc, house) => house.SetCookingDescription(desc)
            );

            app.UseGet("eatingList", (House house) => house.EatingList());
        }
    }
}
