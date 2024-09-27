using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_Example.Database
{
    public class User: IdentityUser
    {
        public int AtHomeStatus { get; set; } = 0;
        public int EatingTotalPeople { get; set; } = 0;
        public string Diet { get; set; } = "";
        public House? House { get; set; }
        public List<TransactionUser> TransactionUsers { get; set; } = [];
        public List<Transaction> Transactions { get; set; } = [];
        public List<Transaction> GottenTransactions { get; set; } = [];

        public bool IsCooking()
        {
            return House?.CookingUser == this;
        }

        public bool SetCookingStatus(bool cooking)
        {
            if (House == null)
                return false;
            if (House.CookingUser != null && House.CookingUser != this)
                return false;
            if (cooking)
                House.CookingUser = this;
            else
                House.CookingUser = null;
            return cooking;
        }

        public int CookingPoints() => Transactions.Sum(t => t.PerUser().cookingPoints);
        public int EuroCents() => Transactions.Sum(t => t.PerUser().euroCents);
    }
}
