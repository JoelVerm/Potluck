using Microsoft.EntityFrameworkCore;

namespace Backend_Example.Database
{
    public class Transaction
    {
        public int Id { get; set; }
        public House House { get; set; }
        public int EuroCents { get; set; } = 0;
        public int CookingPoints { get; set; } = 0;
        public List<TransactionUser> TransactionUsers { get; set; } = [];
        public List<User?> Users { get; set; } = [];
    }
}
