using Logic.Models;

namespace Logic;

public static class TransactionsLogic
{
    public static Transaction[] AllTransactions(this HouseLogic.LogicHouse house)
    {
        return house.house.Transactions.Select(t => new Transaction(
                t.ToUser?.UserName ?? (t.IsPenalty ? "Penalty" : ""),
                t.Users.Select(u => u?.UserName ?? "").ToArray(),
                t.Description,
                t.EuroCents.ToMoney(),
                t.CookingPoints
            ))
            .ToArray() ?? [];
    }

    public static void AddTransaction(this HouseLogic.LogicHouse house, Transaction transaction)
    {
        var toUser = house.house.Users.FirstOrDefault(u => u.UserName == transaction.To);
        var fromUsers = transaction
            .From.Select(n => house.house.Users.FirstOrDefault(u => u.UserName == n))
            .ToList();
        if (fromUsers.Any(u => u == null))
            return;
        house.house.Transactions.Add(
            new Models.Transaction
            {
                ToUser = toUser,
                Users = fromUsers,
                Description = transaction.Description,
                EuroCents = transaction.Money.ToCents(),
                CookingPoints = transaction.Points,
                IsPenalty = toUser == null
            }
        );
        house.db.SaveChanges();
    }

    public static (int cookingPoints, decimal euros) Balance(this User userLogic)
    {
        var euroCents = 0;
        var cookingPoints = 0;
        foreach (var transaction in userLogic.Transactions)
        {
            var (euroCentsDelta, cookingPointsDelta) = BalanceFor(userLogic, transaction);
            euroCents += euroCentsDelta;
            cookingPoints += cookingPointsDelta;
        }

        return (cookingPoints, euroCents.ToMoney());
    }

    public static (int euroCents, int cookingPoints) BalanceFor(
        User userLogic,
        Models.Transaction transaction
    )
    {
        var euroCents = 0;
        var cookingPoints = 0;
        if (transaction.ToUser == userLogic)
        {
            euroCents += transaction.EuroCents;
            cookingPoints += transaction.CookingPoints;
        }

        var fromCount = transaction.Users.Count(u => u == userLogic);
        if (fromCount > 0)
        {
            var count = transaction.Users.Count;
            if (count > 0)
            {
                euroCents -= transaction.EuroCents / count * fromCount;
                cookingPoints -= transaction.CookingPoints / count * fromCount;
            }
        }

        return (euroCents, cookingPoints);
    }

    public class Transaction(
        string to,
        string[] from,
        string description,
        decimal money,
        int points
    )
    {
        public string To { get; set; } = to;
        public string[] From { get; set; } = from;
        public string Description { get; set; } = description;
        public decimal Money { get; set; } = money;
        public int Points { get; set; } = points;
    }
}