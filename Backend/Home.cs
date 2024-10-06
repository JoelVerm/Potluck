using System.Collections.Immutable;
using Backend_Example.Logic;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Example
{
    public static class Home
    {
        private class TotalBalanceResponse((int cookingPoints, decimal euros) balance)
        {
            public int CookingPoints { get; set; } = balance.cookingPoints;
            public decimal Euros { get; set; } = balance.euros;
        }

        public static void SetupHomeRoutes(this IEndpointRouteBuilder app)
        {
            app.MapOut(
                "totalBalance",
                (Transactions transactions) => new TotalBalanceResponse(transactions.Balance())
            );

            app.MapInOut(
                "eatingTotal",
                (User user) => user.EatingTotalPeople(),
                (eatingTotal, user) => user.SetEatingTotalPeople(eatingTotal)
            );

            app.MapInOut(
                "homeStatus",
                (User user) => user.HomeStatus(),
                (homeStatus, user) => user.SetHomeStatus(homeStatus)
            );

            app.MapOut("homeStatusList", (House house) => house.HomeStatusList());
        }
    }
}
