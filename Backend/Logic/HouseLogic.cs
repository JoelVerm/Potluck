using Logic.Models;

namespace Logic;

public class HouseLogic : LogicBase
{
    public enum CreateHouseStatus
    {
        Exists,
        UserNotFound,
        Success
    }

    public HouseLogic(IServiceProvider sp) : base(sp)
    {
    }

    public HouseLogic(IPotluckDb db) : base(db)
    {
    }

    public LogicHouse? GetHouse(string name)
    {
        var house = db.GetHouse(name);
        return house == null ? null : new LogicHouse(house, db);
    }

    public CreateHouseStatus CreateNew(string username, string name)
    {
        var house = db.GetHouse(name);
        if (house != null)
            return CreateHouseStatus.Exists;
        var user = db.GetUser(username);
        if (user == null)
            return CreateHouseStatus.UserNotFound;
        house = new House { Name = name };
        house.Users.Add(user);
        user.House = house;
        db.AddHouse(house);
        db.SaveChanges();
        return CreateHouseStatus.Success;
    }

    public class LogicHouse(House house, IPotluckDb db)
    {
        public readonly IPotluckDb db = db;
        public readonly House house = house;

        public bool IsAllowed(string username)
        {
            return house.Users.Any(u => u.UserName == username);
        }

        public int GetId()
        {
            return house.Id;
        }

        public string[] AllPeople()
        {
            return house.Users.Select(u => u.UserName!).ToArray();
        }

        public void AddUser(string name)
        {
            var addUser = db.GetUser(name);
            if (addUser == null)
                return;
            if (house.Users.Contains(addUser))
                return;
            house.Users.Add(addUser);
            db.SaveChanges();
        }

        public void RemoveUser(string name)
        {
            var removeUser = house.Users.Find(u => u.UserName == name);
            if (removeUser == null)
                return;
            house.Users.Remove(removeUser);
            db.SaveChanges();
        }

        public Dictionary<string, string> HomeStatusList()
        {
            return house.Users.ToDictionary(
                u => u.UserName!,
                u => UserLogic.homeStatus[u.AtHomeStatus]
            );
        }

        public string CookingUser()
        {
            return house.CookingUser?.UserName ?? "";
        }

        public bool IsUserCooking(string username)
        {
            return house.CookingUser == db.GetUser(username);
        }

        public void SetUserCooking(string username, bool status)
        {
            if (house.CookingUser != null && !IsUserCooking(username))
                return;
            house.CookingUser = status ? db.GetUser(username) : null;
            db.SaveChanges();
        }

        public decimal CookingPrice()
        {
            return house.CookingPrice.ToMoney();
        }

        public void SetCookingPrice(string username, decimal price)
        {
            if (!IsUserCooking(username))
                return;
            house.CookingPrice = price.ToCents();
            db.SaveChanges();
        }

        public string CookingDescription()
        {
            return house.CookingDescription;
        }

        public void SetCookingDescription(string username, string description)
        {
            if (!IsUserCooking(username))
                return;
            house.CookingDescription = description;
            db.SaveChanges();
        }

        public EatingPerson[] EatingList()
        {
            return house.Users.Where(u => u.EatingTotalPeople > 0)
                .Select(u => new EatingPerson(
                    u.UserName ?? "",
                    u.EatingTotalPeople,
                    u.Balance().cookingPoints,
                    u.Diet
                ))
                .ToArray();
        }

        public string ShoppingList()
        {
            return house.ShoppingList;
        }

        public void SetShoppingList(string list)
        {
            house.ShoppingList = list;
            db.SaveChanges();
        }

        public string HouseName()
        {
            return house.Name;
        }

        public void SetHouseName(string name)
        {
            house.Name = name;
            db.SaveChanges();
        }
    }

    public class EatingPerson(string name, int count, int cookingPoints, string diet)
    {
        public string Name { get; } = name;
        public int Count { get; } = count;
        public int CookingPoints { get; } = cookingPoints;
        public string Diet { get; } = diet;
    }
}