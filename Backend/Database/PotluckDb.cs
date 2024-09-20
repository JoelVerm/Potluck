using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend_Example.Database
{
    public class PotluckDb : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<House> Houses { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionUser> TransactionUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Potluck;Integrated Security=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.TransactionUsers)
                .WithOne(tu => tu.User)
                .OnDelete(DeleteBehavior.ClientSetNull);
            modelBuilder.Entity<Transaction>()
                .HasMany(t => t.TransactionUsers)
                .WithOne(tu => tu.Transaction)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>()
                .HasMany(u => u.Transactions)
                .WithMany(t => t.Users!)
                .UsingEntity<TransactionUser>();
        }
    }
}
