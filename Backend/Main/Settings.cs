using Logic;

namespace Potluck
{
    public static class Settings
    {
        public static void SetupSettingsRoutes(this IEndpointRouteBuilder app)
        {
            app.UseGetPost(
                "dietPreferences",
                (User user) => user.Diet(),
                (preference, user) => user.SetDiet(preference)
            );

            app.UseGetPost(
                "houseName",
                (House house) => house.HouseName(),
                (name, house) => house.SetHouseName(name)
            );

            app.UseGet("houseMembers", (House house) => house.AllPeople());

            app.UsePost("createHouse", (string name, House house) => house.CreateNew(name));

            app.UsePost("addHouseMember", (string name, House house) => house.AddUser(name));

            app.UsePost("removeHouseMember", (string name, House house) => house.RemoveUser(name));
        }
    }
}
