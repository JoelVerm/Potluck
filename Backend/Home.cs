using Backend_Example.Database;
using System.Collections.Immutable;

namespace Backend_Example
{
    public static class Home
    {
        private static readonly string[] HomeStatus = ["At home", "Away for a bit", "Out of town"];

        public static void SetupHomeRoutes(this IEndpointRouteBuilder app)
        {
            app.MapGet("/eatingTotal", (HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                return user?.EatingTotalPeople ?? 0;
            }).WithName("EatingTotal").WithOpenApi();

            app.MapPost("/eatingTotal", (int eatingTotal, HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                if (user == null)
                    return 0;
                user.EatingTotalPeople = eatingTotal;
                db.SaveChanges();
                return user.EatingTotalPeople;
            }).WithName("SetEatingTotal").WithOpenApi();

            app.MapGet("/homeStatus", (HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                return HomeStatus[user?.AtHomeStatus ?? 0];
            }).WithName("HomeStatus").WithOpenApi();

            app.MapPost("/homeStatus", (string homeStatus, HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                if (user == null)
                    return HomeStatus[0];
                user.AtHomeStatus = HomeStatus.ToImmutableList().IndexOf(homeStatus);
                db.SaveChanges();
                return HomeStatus[user.AtHomeStatus];
            }).WithName("SetHomeStatus").WithOpenApi();

            app.MapGet("/homeStatusList", (HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                return user?.House?.Users.Select(u => HomeStatus[u.AtHomeStatus]).ToArray() ?? [];
            }).WithName("HomeStatusList").WithOpenApi();
        }
    }
}
