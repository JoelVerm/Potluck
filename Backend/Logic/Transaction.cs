using System.Diagnostics;

namespace Logic;

public class Transaction(
    int euroCents,
    int cookingPoints,
    string description,
    bool isPenalty,
    string? toUser,
    string[] fromUsers)
{
    public int EuroCents { get; } = euroCents;
    public int CookingPoints { get; } = cookingPoints;
    public string Description { get; private set; } = description;
    public bool IsPenalty { get; } = isPenalty;
    public string? ToUser { get; } = toUser;
    public string[] FromUsers { get; } = fromUsers;

    public (int euroCents, int cookingPoints) GetForUser(string user)
    {
        var euroCents = 0;
        var cookingPoints = 0;
        if (ToUser == user)
        {
            euroCents += EuroCents;
            cookingPoints += CookingPoints;
        }

        Debug.WriteLine($"{user} {euroCents} {ToUser} {string.Join(", ", FromUsers)}");

        var fromCount = FromUsers.Count(u => u == user);
        if (fromCount <= 0) return (euroCents, cookingPoints);
        var count = FromUsers.Length;
        if (count <= 0) return (euroCents, cookingPoints);
        euroCents -= EuroCents / count * fromCount;
        cookingPoints -= CookingPoints / count * fromCount;

        Debug.WriteLine($"{euroCents}, {fromCount}, {count}");

        return (euroCents, cookingPoints);
    }

    public static (int euroCents, int cookingPoints) GetTotalForUser(IPotluckDb db, string userName)
    {
        var transactions = db.GetTransactionsForUser(userName);

        var euroCents = 0;
        var cookingPoints = 0;
        foreach (var transaction in transactions)
        {
            var (euroCentsDelta, cookingPointsDelta) = transaction.GetForUser(userName);
            euroCents += euroCentsDelta;
            cookingPoints += cookingPointsDelta;
        }

        return (euroCents, cookingPoints);
    }
}