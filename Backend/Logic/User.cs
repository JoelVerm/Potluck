using System.Collections.Immutable;
using Backend_Example.Database;

namespace Backend_Example.Logic
{
    public class User : LogicBase
    {
        public static readonly ImmutableArray<string> homeStatus =
        [
            "At home",
            "Away for a bit",
            "Out of town",
        ];

        public int EatingTotalPeople() => _user.EatingTotalPeople;

        public void SetEatingTotalPeople(int eatingTotal)
        {
            if (eatingTotal < 0)
                return;
            _user.EatingTotalPeople = eatingTotal;
            _db.SaveChanges();
        }

        public string HomeStatus() => homeStatus[_user.AtHomeStatus];

        public void SetHomeStatus(string status)
        {
            int statusNo = homeStatus.IndexOf(status);
            if (statusNo == -1)
                return;
            _user.AtHomeStatus = statusNo;
            _db.SaveChanges();
        }

        public string Diet() => _user.Diet;

        public void SetDiet(string diet)
        {
            _user.Diet = diet;
            _db.SaveChanges();
        }
    }
}
