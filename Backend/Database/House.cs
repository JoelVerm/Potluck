using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace Backend_Example.Database
{
    [Index(nameof(Name), IsUnique = true)]
    public class House
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<User> Users { get; set; } = [];
        public virtual List<Transaction> Transactions { get; set; } = [];
        public string? CookingUserId { get; set; }
        public virtual User? CookingUser { get; set; }
        public int CookingPrice { get; set; } = 0;
        public string CookingDescription { get; set; } = "";
        public string ShoppingList { get; set; } = "";

        public bool AddTransaction(string? toUserName, string[] fromUserNames, string description, decimal money, int points)
        {
            var toUser = Users.FirstOrDefault(u => u.UserName == toUserName);
            if (toUserName != null && toUser == null)
                return false;
            var fromUsers = fromUserNames.Select(n => Users.FirstOrDefault(u => u.UserName == n)).ToList();
            if(fromUsers.Any(u => u == null))
                return false;
            Transactions.Add(new Transaction
            {
                ToUser = toUser,
                Users = fromUsers,
                Description = description,
                EuroCents = money.ToCents(),
                CookingPoints = points,
                IsPenalty = toUserName == null
            });
            return true;
        }
    }
}
