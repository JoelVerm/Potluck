using Logic;
using Logic.Models;

namespace PotluckTest;

internal class MockDb : IPotluckDb
{
    public User? User { get; init; }

    public int SaveChangesTimesCalled { get; private set; }

    public House? House { get; init; }

    public int AddHouseTimesCalled { get; private set; }

    public User? GetUser(string? name)
    {
        return User;
    }

    public House? GetHouse(string name)
    {
        return House;
    }

    public void AddHouse(House house)
    {
        AddHouseTimesCalled++;
    }

    public int SaveChanges()
    {
        SaveChangesTimesCalled++;
        return 1;
    }

    public static MockDb Create(User user)
    {
        return new MockDb { User = user, House = user.House };
    }
}