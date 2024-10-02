using System.Collections.Immutable;
using Backend_Example.Database;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Example
{
    public static class Home
    {
        private class TotalBalanceResponse(int cookingPoints, decimal euroCents)
        {
            public int CookingPoints { get; set; } = cookingPoints;
            public decimal EuroCents { get; set; } = euroCents;
        }

        private static readonly string[] HomeStatus = ["At home", "Away for a bit", "Out of town"];

        public static void SetupHomeRoutes(this IEndpointRouteBuilder app)
        {
            app.MapOut(
                "totalBalance",
                user => new TotalBalanceResponse(user.CookingPoints(), user.EuroCents().ToMoney())
            );

            app.MapInOut(
                "eatingTotal",
                user => user.EatingTotalPeople,
                (eatingTotal, user) => user.EatingTotalPeople = eatingTotal
            );

            app.MapInOut(
                "homeStatus",
                user => HomeStatus[user.AtHomeStatus],
                (homeStatus, user) =>
                    user.AtHomeStatus = HomeStatus.ToImmutableList().IndexOf(homeStatus)
            );

            app.MapOut(
                "homeStatusList",
                user =>
                    user.House?.Users.ToDictionary(
                        u => u.UserName!,
                        u => HomeStatus[u.AtHomeStatus]
                    ) ?? []
            );
        }
    }
}
