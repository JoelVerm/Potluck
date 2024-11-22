using Logic;
using Potluck.Helpers;

namespace Potluck.API
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
            app.UseGet(
                "totalBalance",
                (Transactions transactions) => new TotalBalanceResponse(transactions.Balance())
            );

            app.UseGetAndWebsocket(
                "eatingTotal",
                (User user) => user.EatingTotalPeople(),
                (User user, int total) => user.SetEatingTotalPeople(total)
            );

            app.UseGetAndWebsocket(
                "homeStatus",
                (User user) => user.HomeStatus(),
                (User user, string status) => user.SetHomeStatus(status)
            );

            app.UseGet("homeStatusList", (House house) => house.HomeStatusList());
        }
    }
}
