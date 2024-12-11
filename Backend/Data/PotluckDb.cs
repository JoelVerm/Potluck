using Logic;
using Logic.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class PotluckDb : IdentityDbContext<User>, IPotluckDb
{
    public new DbSet<User> Users { get; set; }
    public DbSet<House> Houses { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<TransactionUser> TransactionUsers { get; set; }

    public User? GetUser(string? name)
    {
        return Users.FirstOrDefault(u => u.UserName == name);
    }

    public House? GetHouse(string? name)
    {
        return Houses.FirstOrDefault(h => h.Name == name);
    }

    public void AddHouse(House house)
    {
        Houses.Add(house);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var conn = Environment.GetEnvironmentVariable("ConnectionString");
        if (conn != null)
            Console.WriteLine($"Using connection string: {conn}");
        optionsBuilder.UseSqlServer(
            conn ?? "Server=(localdb)\\MSSQLLocalDB;Database=Potluck;Integrated Security=True;"
        );
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<User>()
            .HasMany(u => u.TransactionUsers)
            .WithOne(tu => tu.User)
            .OnDelete(DeleteBehavior.ClientSetNull);
        modelBuilder
            .Entity<Transaction>()
            .HasMany(t => t.TransactionUsers)
            .WithOne(tu => tu.Transaction)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder
            .Entity<User>()
            .HasMany(u => u.Transactions)
            .WithMany(t => t.Users!)
            .UsingEntity<TransactionUser>();
        modelBuilder.Entity<User>().ToTable("Users");

        modelBuilder
            .Entity<House>()
            .HasOne(h => h.CookingUser)
            .WithOne()
            .HasForeignKey<House>(h => h.CookingUserId)
            .IsRequired(false);
        modelBuilder.Entity<House>().HasMany(h => h.Users).WithOne(u => u.House);

        modelBuilder
            .Entity<Transaction>()
            .HasOne(t => t.ToUser)
            .WithMany(u => u.GottenTransactions);
    }
}