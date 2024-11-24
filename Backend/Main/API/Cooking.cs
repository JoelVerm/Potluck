using Logic;
using Potluck.Helpers;

namespace Potluck.API;

public static class Cooking
{
    public static void SetupCookingRoutes(this IEndpointRouteBuilder app)
    {
        app.UseGetAndWebsocket(
            "cooking",
            house => house.CookingUser(),
            (House house, bool cooking) =>
            {
                house.SetUserCooking(cooking);
                return house.CookingUser();
            }
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