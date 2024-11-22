namespace Logic;

public class House : LogicBase
{
    public string[] AllPeople()
    {
        return _user.House?.Users.Select(u => u.UserName!).ToArray() ?? [];
    }

    public void AddUser(string name)
    {
        if (_user.House == null)
            return;
        var addUser = _db.GetUser(name);
        if (addUser == null)
            return;
        if (_user.House.Users.Contains(addUser))
            return;
        _user.House.Users.Add(addUser);
        _db.SaveChanges();
    }

    public void RemoveUser(string name)
    {
        if (_user.House == null)
            return;
        var removeUser = _user.House.Users.Find(u => u.UserName == name);
        if (removeUser == null)
            return;
        _user.House.Users.Remove(removeUser);
        _db.SaveChanges();
    }

    public Dictionary<string, string> HomeStatusList()
    {
        return _user.House?.Users.ToDictionary(
            u => u.UserName!,
            u => User.homeStatus[u.AtHomeStatus]
        ) ?? [];
    }

    public string CookingUser()
    {
        return _user.House?.CookingUser?.UserName ?? "";
    }

    public bool IsUserCooking()
    {
        return _user.House?.CookingUser == _user;
    }

    public void SetUserCooking(bool status)
    {
        if (_user.House == null)
            return;
        if (_user.House.CookingUser != null && !IsUserCooking())
            return;
        if (status)
            _user.House.CookingUser = _user;
        else
            _user.House.CookingUser = null;
        _db.SaveChanges();
    }

    public decimal CookingPrice()
    {
        return _user.House?.CookingPrice.ToMoney() ?? 0;
    }

    public void SetCookingPrice(decimal price)
    {
        if (_user.House == null)
            return;
        if (!IsUserCooking())
            return;
        _user.House.CookingPrice = price.ToCents();
        _db.SaveChanges();
    }

    public string CookingDescription()
    {
        return _user.House?.CookingDescription ?? "";
    }

    public void SetCookingDescription(string description)
    {
        if (_user.House == null)
            return;
        if (!IsUserCooking())
            return;
        _user.House.CookingDescription = description;
        _db.SaveChanges();
    }

    public EatingPerson[] EatingList()
    {
        return _user
            .House?.Users.Where(u => u.EatingTotalPeople > 0)
            .Select(u => new EatingPerson(
                u.UserName ?? "",
                u.EatingTotalPeople,
                Transactions.BalanceFor(u).cookingPoints,
                u.Diet
            ))
            .ToArray() ?? [];
    }

    public string ShoppingList()
    {
        return _user.House?.ShoppingList ?? "";
    }

    public void SetShoppingList(string list)
    {
        if (_user.House == null)
            return;
        _user.House.ShoppingList = list;
        _db.SaveChanges();
    }

    public string HouseName()
    {
        return _user.House?.Name ?? "";
    }

    public void SetHouseName(string name)
    {
        if (_user.House == null)
            return;
        _user.House.Name = name;
        _db.SaveChanges();
    }

    public int CreateNew(string name)
    {
        if (_user.House != null)
            return _user.House.Id;
        _user.House = new Models.House { Name = name };
        _user.House.Users.Add(_user);
        _db.SaveChanges();
        return _user.House.Id;
    }

    public class EatingPerson(string name, int count, int cookingPoints, string diet)
    {
        public string Name { get; set; } = name;
        public int Count { get; set; } = count;
        public int CookingPoints { get; set; } = cookingPoints;
        public string Diet { get; set; } = diet;
    }
}