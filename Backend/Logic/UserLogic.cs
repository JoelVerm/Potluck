using System.Collections.Immutable;
using Logic.Models;

namespace Logic;

public class UserLogic : LogicBase
{
    public static readonly ImmutableArray<string> homeStatus =
    [
        "At home",
        "Away for a bit",
        "Out of town"
    ];

    public UserLogic(IServiceProvider sp) : base(sp)
    {
    }

    public UserLogic(IPotluckDb db) : base(db)
    {
    }

    public LogicUser? GetUser(string username)
    {
        var user = db.GetUser(username);
        return user == null ? null : new LogicUser(user, db);
    }

    public class LogicUser(User user, IPotluckDb db)
    {
        public readonly User user = user;

        public int EatingTotalPeople()
        {
            return user.EatingTotalPeople;
        }

        public void SetEatingTotalPeople(int eatingTotal)
        {
            if (eatingTotal < 0)
                return;
            user.EatingTotalPeople = eatingTotal;
            db.SaveChanges();
        }

        public string HomeStatus()
        {
            return homeStatus[user.AtHomeStatus];
        }

        public void SetHomeStatus(string status)
        {
            var statusNo = homeStatus.IndexOf(status);
            if (statusNo == -1)
                return;
            user.AtHomeStatus = statusNo;
            db.SaveChanges();
        }

        public string Diet()
        {
            return user.Diet;
        }

        public void SetDiet(string diet)
        {
            user.Diet = diet;
            db.SaveChanges();
        }

        public int? HouseId()
        {
            return user.House?.Id;
        }
    }
}