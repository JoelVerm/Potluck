using Microsoft.EntityFrameworkCore;

namespace Backend_Example.Database
{
    [Index(nameof(Name), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public int AtHomeStatus { get; set; } = 0;
        public int EatingTotalPeople { get; set; } = 0;
        public string Diet { get; set; } = "";
        public House? House { get; set; }
        public List<TransactionUser> TransactionUsers { get; set; } = [];
        public List<Transaction> Transactions { get; set; } = [];
    }
}
