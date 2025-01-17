namespace Logic;

public class House(
    IPotluckDb db,
    int id,
    string name,
    int cookingPrice,
    string cookingDescription,
    string? cookingUser,
    string shoppingList)
{
    private readonly IPotluckDb db = db;

    public int Id { get; } = id;
    public string Name { get; } = name;
    public int CookingPrice { get; private set; } = cookingPrice;
    public string CookingDescription { get; private set; } = cookingDescription;
    public string? CookingUser { get; private set; } = cookingUser;
    public string ShoppingList { get; private set; } = shoppingList;

    public bool IsMember(string user)
    {
        return db.GetUsersInHouse(Name).Any(u => u.Name == user);
    }

    public string[] GetMembers()
    {
        return db.GetUsersInHouse(Name).Select(u => u.Name).ToArray();
    }

    public bool AddUser(string user)
    {
        return db.AddUserToHouse(user, Name);
    }

    public void RemoveUser(string user)
    {
        db.RemoveUserFromHouse(user, Name);
    }

    public Dictionary<string, string> GetHomeStatusList()
    {
        return db.GetUsersInHouse(Name).ToDictionary(u => u.Name, u => u.GetHomeStatus());
    }

    public bool IsUserCooking(string user)
    {
        return CookingUser == user;
    }

    public bool SetCookingUser(string? user)
    {
        if (!db.SetHouseCookingUser(Name, user)) return false;
        CookingUser = user;
        return true;
    }

    public bool SetCookingPrice(int price, string user)
    {
        if (!IsUserCooking(user)) return false;
        if (!db.SetHouseCookingPrice(Name, price)) return false;
        CookingPrice = price;
        return true;
    }

    public bool SetCookingDescription(string description, string user)
    {
        if (!IsUserCooking(user)) return false;
        if (!db.SetHouseCookingDescription(Name, description)) return false;
        CookingDescription = description;
        return true;
    }

    public bool SetShoppingList(string list)
    {
        if (!db.SetHouseShoppingList(Name, list)) return false;
        ShoppingList = list;
        return true;
    }

    public EatingPerson[] GetEatingList()
    {
        return db.GetUsersInHouse(Name)
            .Where(u => u.EatingTotalPeople > 0)
            .Select(u => new EatingPerson(
                u.Name,
                u.EatingTotalPeople,
                Transaction.GetTotalForUser(db, u.Name).cookingPoints,
                u.Diet
            ))
            .ToArray();
    }

    public static bool CreateNew(IPotluckDb db, string forUser, string name)
    {
        var house = db.GetHouse(name);
        if (house != null)
            return false;
        var user = db.GetUser(forUser);
        if (user == null)
            return false;
        db.AddHouse(name);
        return db.AddUserToHouse(forUser, name);
    }
}

public class EatingPerson(string name, int count, int cookingPoints, string diet)
{
    public string Name { get; } = name;
    public int Count { get; } = count;
    public int CookingPoints { get; } = cookingPoints;
    public string Diet { get; } = diet;
}