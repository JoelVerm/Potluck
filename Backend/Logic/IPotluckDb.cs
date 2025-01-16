namespace Logic;

public interface IPotluckDb
{
    public User? GetUser(string name);

    public bool SetUserEatingTotalPeople(string name, int eatingTotal);

    public bool SetUserAtHomeStatus(string name, AtHomeStatus status);

    public bool SetUserDiet(string name, string diet);

    public House? GetHouseForUser(string userName);

    public House? GetHouse(string name);

    public void AddHouse(string name);

    public List<User> GetUsersInHouse(string house);

    public bool AddUserToHouse(string user, string house);

    public void RemoveUserFromHouse(string user, string house);

    public bool SetHouseCookingUser(string house, string? user);

    public bool SetHouseCookingPrice(string house, int price);

    public bool SetHouseCookingDescription(string house, string description);

    public bool SetHouseShoppingList(string house, string list);

    public List<Transaction> GetTransactionsForUser(string user);

    public List<Transaction> GetTransactionsForHouse(string house);

    public bool AddTransactionToHouse(Transaction transaction, string house);
}