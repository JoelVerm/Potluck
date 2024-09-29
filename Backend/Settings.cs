using System.Diagnostics;
using Backend_Example.Database;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Example
{
    public static class Settings
    {
        public static void SetupSettingsRoutes(this IEndpointRouteBuilder app)
        {
            app.MapInOut(
                "dietPreferences",
                user => user.Diet,
                (preference, user) => user.Diet = preference
            );

            app.MapInOut(
                "houseName",
                user => user.House?.Name ?? "",
                (name, user) =>
                {
                    if (user.House == null)
                        return;
                    user.House.Name = name;
                }
            );

            app.MapOut(
                "houseMembers",
                user => user.House?.Users.Select(u => u.UserName).ToArray() ?? []
            );

            app.MapIn<string>(
                "createHouse",
                (name, user) =>
                {
                    if (user.House != null)
                        return false;
                    user.House = new House { Name = name };
                    user.House.Users.Add(user);
                    return true;
                }
            );

            app.MapIn<string>(
                "removeHouseMember",
                (name, user) =>
                {
                    if (user.House == null)
                        return false;
                    var removeUser = user.House.Users.Find(u => u.UserName == name);
                    if (removeUser == null)
                        return false;
                    user.House.Users.Remove(removeUser);
                    return true;
                }
            );

            app.MapIn<string>(
                "addHouseMember",
                (name, user) =>
                {
                    if (user.House == null)
                        return false;
                    var db = app.ServiceProvider.GetRequiredService<PotluckDb>();
                    var addUser = db.Users.Find(name);
                    if (addUser == null)
                        return false;
                    user.House.Users.Add(addUser);
                    return true;
                }
            );
        }
    }
}
