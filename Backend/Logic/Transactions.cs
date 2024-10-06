using Backend_Example.Database;

namespace Backend_Example.Logic
{
    public class Transactions : LogicBase
    {
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

        public Transaction[] AllTransactions()
        {
            return _user
                    .House?.Transactions.Select(t => new Transaction(
                        t.ToUser?.UserName ?? (t.IsPenalty ? "Penalty" : ""),
                        t.Users.Select(u => u?.UserName ?? "").ToArray(),
                        t.Description,
                        t.EuroCents.ToMoney(),
                        t.CookingPoints
                    ))
                    .ToArray() ?? [];
        }

        public void AddTransaction(Transaction transaction)
        {
            if (_user.House == null)
                return;
            var toUser = _user.House.Users.FirstOrDefault(u => u.UserName == transaction.To);
            if (transaction.To != null && toUser == null)
                return;
            var fromUsers = transaction
                .From.Select(n => _user.House.Users.FirstOrDefault(u => u.UserName == n))
                .ToList();
            if (fromUsers.Any(u => u == null))
                return;
            _user.House.Transactions.Add(
                new Database.Transaction
                {
                    ToUser = toUser,
                    Users = fromUsers,
                    Description = transaction.Description,
                    EuroCents = transaction.Money.ToCents(),
                    CookingPoints = transaction.Points,
                    IsPenalty = toUser == null,
                }
            );
            _db.SaveChanges();
        }

        public (int cookingPoints, decimal euros) Balance() => BalanceFor(_user);

        public static (int cookingPoints, decimal euros) BalanceFor(Database.User user)
        {
            int euroCents = 0;
            int cookingPoints = 0;
            foreach (var transaction in user.Transactions)
            {
                var (euroCentsDelta, cookingPointsDelta) = BalanceFor(user, transaction);
                euroCents += euroCentsDelta;
                cookingPoints += cookingPointsDelta;
            }
            return (cookingPoints, euroCents.ToMoney());
        }

        public static (int euroCents, int cookingPoints) BalanceFor(
            Database.User user,
            Database.Transaction transaction
        )
        {
            int euroCents = 0;
            int cookingPoints = 0;
            if (transaction.ToUser == user)
            {
                euroCents += transaction.EuroCents;
                cookingPoints += transaction.CookingPoints;
            }
            int fromCount = transaction.Users.Count(u => u == user);
            if (fromCount > 0)
            {
                var count = transaction.Users.Count;
                if (count > 0)
                {
                    euroCents -= (transaction.EuroCents / count) * fromCount;
                    cookingPoints -= (transaction.CookingPoints / count) * fromCount;
                }
            }
            return (euroCents, cookingPoints);
        }
    }
}
