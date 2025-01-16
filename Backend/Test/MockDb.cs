using Logic;
using Transaction = Data.Models.Transaction;
using User = Data.Models.User;

namespace PotluckTest;

internal class MockDb : IPotluckDb
{
    public User? TransactionToUser { get; set; } = null;

    public List<User?> TransactionUsers { get; set; } = [];

    public List<Transaction> TransactionsForUser { get; set; } = [];

    public Logic.User? GetUser(string name)
    {
        throw new NotImplementedException();
    }

    public bool SetUserEatingTotalPeople(string name, int eatingTotal)
    {
        throw new NotImplementedException();
    }

    public bool SetUserAtHomeStatus(string name, AtHomeStatus status)
    {
        throw new NotImplementedException();
    }

    public bool SetUserDiet(string name, string diet)
    {
        throw new NotImplementedException();
    }

    public House? GetHouseForUser(string userName)
    {
        throw new NotImplementedException();
    }

    public House? GetHouse(string name)
    {
        throw new NotImplementedException();
    }

    public void AddHouse(string name)
    {
        throw new NotImplementedException();
    }

    public List<Logic.User> GetUsersInHouse(string house)
    {
        throw new NotImplementedException();
    }

    public bool AddUserToHouse(string user, string house)
    {
        throw new NotImplementedException();
    }

    public void RemoveUserFromHouse(string user, string house)
    {
        throw new NotImplementedException();
    }

    public bool SetHouseCookingUser(string house, string? user)
    {
        throw new NotImplementedException();
    }

    public bool SetHouseCookingPrice(string house, int price)
    {
        throw new NotImplementedException();
    }

    public bool SetHouseCookingDescription(string house, string description)
    {
        throw new NotImplementedException();
    }

    public bool SetHouseShoppingList(string house, string list)
    {
        throw new NotImplementedException();
    }

    public List<Logic.Transaction> GetTransactionsForUser(string user)
    {
        return TransactionsForUser
            .Select(t => new Logic.Transaction(t.EuroCents, t.CookingPoints, t.Description, t.IsPenalty,
                t.ToUser?.UserName, t.Users.Select(u => u!.UserName!).ToArray()))
            .ToList();
    }

    public List<Logic.Transaction> GetTransactionsForHouse(string house)
    {
        throw new NotImplementedException();
    }

    public bool AddTransactionToHouse(Logic.Transaction transaction, string house)
    {
        throw new NotImplementedException();
    }
}