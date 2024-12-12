using Logic.Models;

namespace Logic;

public interface IPotluckDb
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public int SaveChanges();

    public User? GetUser(string? name);

    public House? GetHouse(string name);

    public void AddHouse(House house);
}