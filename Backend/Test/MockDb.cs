using Logic;
using Logic.Models;

namespace PotluckTest;

internal class MockDb : IPotluckDb
{
    public User? User { get; set; } = null;

    public string? GetUserPassedName { get; private set; }

    public int SaveChangesTimesCalled { get; private set; }

    public House? House { get; set; } = null;

    public string? GetHousePassedName { get; private set; }

    public int AddHouseCalledTimes { get; private set; }

    public User? GetUser(string? name)
    {
        GetUserPassedName = name;
        return User;
    }

    public House? GetHouse(string name)
    {
        GetHousePassedName = name;
        return House;
    }

    public void AddHouse(House house)
    {
        AddHouseCalledTimes++;
    }

    public int SaveChanges()
    {
        SaveChangesTimesCalled++;
        return 1;
    }
}