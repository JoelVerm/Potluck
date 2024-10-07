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
    }
}
