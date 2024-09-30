using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend_Example.Database
{
    public class User : IdentityUser
    {
        public int AtHomeStatus { get; set; } = 0;
        public int EatingTotalPeople { get; set; } = 0;
        public string Diet { get; set; } = "";
        public virtual House? House { get; set; }
        public virtual List<TransactionUser> TransactionUsers { get; set; } = [];
        public virtual List<Transaction> Transactions { get; set; } = [];
        public virtual List<Transaction> GottenTransactions { get; set; } = [];

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

        public int CookingPoints() =>
            Transactions.Where(t => t.ToUser == this).Sum(t => t.CookingPoints)
            - Transactions.Sum(t => t.PerUser().cookingPoints);

        public int EuroCents() =>
            Transactions.Where(t => t.ToUser == this).Sum(t => t.EuroCents)
            - Transactions.Sum(t => t.PerUser().euroCents);
    }
}
