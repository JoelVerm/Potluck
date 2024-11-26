using Logic;
using Potluck.Helpers;

namespace Potluck.API;

public static class Users
{
    public static void SetupUsersRoutes(this IEndpointRouteBuilder app)
    {
        // Many

        app.UseGet("users", "homeStatus", (House house) => house.HomeStatusList());

        // One

        app.UseGet(
            "users", "current/balance",
            (Transactions transactions) => new TotalBalanceResponse(transactions.Balance())
        );
        app.UseGetAndWebsocket(
            "users", "current/eatingTotalPeople",
            user => user.EatingTotalPeople(),
            (user, total) => user.SetEatingTotalPeople(total)
        );
        app.UseGetAndWebsocket(
            "users", "current/homeStatus",
            user => user.HomeStatus(),
            (user, status) => user.SetHomeStatus(status)
        );
        app.UseGetAndWebsocket(
            "users", "current/diet",
            user => user.Diet(),
            (user, preference) => user.SetDiet(preference)
        );
    }

    private class TotalBalanceResponse((int cookingPoints, decimal euros) balance)
    {
        public int CookingPoints { get; set; } = balance.cookingPoints;
        public decimal Euros { get; set; } = balance.euros;
    }
}