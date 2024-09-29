using Backend_Example.Database;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Backend_Example
{
    public static class Settings
    {
        public static void SetupSettingsRoutes(this IEndpointRouteBuilder app)
        {
            app.MapGet("/dietPreferences", (HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                return Results.Json(user?.Diet ?? "");
            }).WithName("DietPreferences").WithOpenApi();

            app.MapPost("/dietPreferences", ([FromBody] string preference, HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                if (user == null)
                    return Results.Json("");
                user.Diet = preference;
                db.SaveChanges();
                return Results.Json(user.Diet);
            }).WithName("SetDietPreferences").WithOpenApi();

            app.MapGet("/houseName", (HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                return Results.Json(user?.House?.Name ?? "");
            }).WithName("HouseName").WithOpenApi();

            app.MapPost("/houseName", ([FromBody] string name, HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                if (user == null)
                    return Results.Json("");
                if (user.House == null)
                    return Results.Json("");
                user.House.Name = name;
                db.SaveChanges();
                return Results.Json(user.House.Name);
            }).WithName("SetHouseName").WithOpenApi();

            app.MapGet("/houseMembers", (HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                return Results.Json(user?.House?.Users.Select(u => u.UserName).ToArray() ?? []);
            }).WithName("HouseMembers").WithOpenApi();

            app.MapPost("/createHouse", ([FromBody] string name, HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                if (user == null)
                    return Results.Json(false);
                if (user.House != null)
                    return Results.Json(false);
                user.House = new House { Name = name };
                user.House.Users.Add(user);
                db.SaveChanges();
                return Results.Json(user.House.Name);
            }).WithName("CreateHouse").WithOpenApi();

            app.MapPost("/removeHouseMember", ([FromBody] string name, HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                if (user == null)
                    return Results.Json(false);
                if (user.House == null)
                    return Results.Json(false);
                var removeUser = user.House.Users.Find(u => u.UserName == name);
                if (removeUser == null)
                    return Results.Json(false);
                user.House.Users.Remove(removeUser);
                db.SaveChanges();
                return Results.Json(true);
            }).WithName("RemoveHouseMember").WithOpenApi();

            app.MapPost("/addHouseMember", ([FromBody] string name, HttpContext context, PotluckDb db) =>
            {
                var user = db.GetUser(context);
                if (user == null)
                    return Results.Json(false);
                if (user.House == null)
                    return Results.Json(false);
                var addUser = db.Users.Find(name);
                if (addUser == null)
                    return Results.Json(false);
                user.House.Users.Add(addUser);
                db.SaveChanges();
                return Results.Json(true);
            }).WithName("AddHouseMember").WithOpenApi();
        }
    }
}
