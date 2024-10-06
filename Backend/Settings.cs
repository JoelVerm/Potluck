using System.Diagnostics;
using Backend_Example.Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_Example
{
    public static class Settings
    {
        public static void SetupSettingsRoutes(this IEndpointRouteBuilder app)
        {
            app.MapInOut(
                "dietPreferences",
                (User user) => user.Diet(),
                (preference, user) => user.SetDiet(preference)
            );

            app.MapInOut(
                "houseName",
                (House house) => house.HouseName(),
                (name, house) => house.SetHouseName(name)
            );

            app.MapOut("houseMembers", (House house) => house.AllPeople());

            app.MapIn("createHouse", (string name, House house) => house.CreateNew(name));

            app.MapIn("addHouseMember", (string name, House house) => house.AddUser(name));

            app.MapIn("removeHouseMember", (string name, House house) => house.RemoveUser(name));
        }
    }
}
