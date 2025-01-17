using System.Collections.Immutable;

namespace Logic;

public enum AtHomeStatus
{
    AtHome,
    AwayForABit,
    OutOfTown
}

public class User(
    IPotluckDb db,
    string name,
    AtHomeStatus atHomeStatus,
    int eatingTotalPeople,
    string diet)
{
    public static readonly ImmutableArray<string> homeStatuses =
    [
        "At home",
        "Away for a bit",
        "Out of town"
    ];

    private readonly IPotluckDb db = db;

    public string Name { get; } = name;
    public AtHomeStatus AtHomeStatus { get; private set; } = atHomeStatus;
    public int EatingTotalPeople { get; private set; } = eatingTotalPeople;
    public string Diet { get; private set; } = diet;

    public bool SetEatingTotalPeople(int eatingTotal)
    {
        if (eatingTotal < 0)
            return false;
        if (!db.SetUserEatingTotalPeople(Name, eatingTotal))
            return false;
        EatingTotalPeople = eatingTotal;
        return true;
    }

    public string GetHomeStatus()
    {
        return homeStatuses[(int)AtHomeStatus];
    }

    public bool SetHomeStatus(string homeStatus)
    {
        var status = homeStatuses.IndexOf(homeStatus);
        if (status == -1)
            return false;
        if (!db.SetUserAtHomeStatus(Name, (AtHomeStatus)status))
            return false;
        AtHomeStatus = (AtHomeStatus)status;
        return true;
    }

    public bool SetDiet(string diet)
    {
        if (!db.SetUserDiet(Name, diet))
            return false;
        Diet = diet;
        return true;
    }

    public House? GetHouse()
    {
        return db.GetHouseForUser(Name);
    }
}