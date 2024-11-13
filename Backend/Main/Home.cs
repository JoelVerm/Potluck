using Logic;

namespace Potluck
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

            app.UseGetPost(
                "eatingTotal",
                (User user) => user.EatingTotalPeople(),
                (eatingTotal, user) => user.SetEatingTotalPeople(eatingTotal)
            );

            app.UseGetPost(
                "homeStatus",
                (User user) => user.HomeStatus(),
                (homeStatus, user) => user.SetHomeStatus(homeStatus)
            );

            app.UseGet("homeStatusList", (House house) => house.HomeStatusList());
        }
    }
}
