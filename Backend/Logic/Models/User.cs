using Microsoft.AspNetCore.Identity;

namespace Logic.Models
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
    }
}
