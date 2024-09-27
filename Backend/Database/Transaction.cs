using Microsoft.EntityFrameworkCore;

namespace Backend_Example.Database
{
    public class Transaction
    {
        public int Id { get; set; }
        public House House { get; set; }
        public int EuroCents { get; set; } = 0;
        public int CookingPoints { get; set; } = 0;
        public string Description { get; set; } = "";
        public User? ToUser { get; set; }
        public bool IsPenalty { get; set; } = false;
        public List<TransactionUser> TransactionUsers { get; set; } = [];
        public List<User?> Users { get; set; } = [];

        public (int euroCents, int cookingPoints) PerUser()
        {
            var count = Users.Count;
            if (count == 0)
                return (0, 0);
            return (EuroCents / count, CookingPoints / count);
        }
    }
}
