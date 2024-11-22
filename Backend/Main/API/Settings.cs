using Logic;
using Potluck.Helpers;

namespace Potluck.API;

public static class Settings
{
    public static void SetupSettingsRoutes(this IEndpointRouteBuilder app)
    {
        app.UseGetAndWebsocket(
            "dietPreferences",
            user => user.Diet(),
            (User user, string preference) => user.SetDiet(preference)
        );

        var houseName = app.UseGetAndWebsocket(
            "houseName",
            house => house.HouseName(),
            (House house, string name) =>
            {
                house.SetHouseName(name);
                return name;
            }
        );

        app.UseGet("houseMembers", (House house) => house.AllPeople());

        app.UsePost("createHouse", (string name, House house) =>
        {
            var id = house.CreateNew(name);
            houseName.BroadcastMessage(name, id);
        });

        app.UsePost("addHouseMember", (string name, House house) => house.AddUser(name));

        app.UsePost("removeHouseMember", (string name, House house) => house.RemoveUser(name));
    }
}