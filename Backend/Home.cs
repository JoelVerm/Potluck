using Backend_Example.Database;
using Microsoft.AspNetCore.Mvc;
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
                return Results.Json(user?.EatingTotalPeople ?? 0);
            }).WithName("EatingTotal").WithOpenApi();

            app.MapPost("/eatingTotal", ([FromBody] int eatingTotal, HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                if (user == null)
                    return Results.Json(0);
                user.EatingTotalPeople = eatingTotal;
                db.SaveChanges();
                return Results.Json(user.EatingTotalPeople);
            }).WithName("SetEatingTotal").WithOpenApi();

            app.MapGet("/homeStatus", (HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                return Results.Json(HomeStatus[user?.AtHomeStatus ?? 0]);
            }).WithName("HomeStatus").WithOpenApi();

            app.MapPost("/homeStatus", ([FromBody] string homeStatus, HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                if (user == null)
                    return Results.Json(HomeStatus[0]);
                user.AtHomeStatus = HomeStatus.ToImmutableList().IndexOf(homeStatus);
                db.SaveChanges();
                return Results.Json(HomeStatus[user.AtHomeStatus]);
            }).WithName("SetHomeStatus").WithOpenApi();

            app.MapGet("/homeStatusList", (HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                return Results.Json(user?.House?.Users.ToDictionary(u => u.UserName!, u => HomeStatus[u.AtHomeStatus]) ?? []);
            }).WithName("HomeStatusList").WithOpenApi();
        }
    }
}
