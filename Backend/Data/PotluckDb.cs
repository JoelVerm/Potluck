using Data.Models;
using Logic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using House = Data.Models.House;
using Transaction = Data.Models.Transaction;
using User = Data.Models.User;

namespace Data;

public class PotluckDb : IdentityDbContext<User>, IPotluckDb
{
    internal new DbSet<User> Users { get; init; }
    internal DbSet<House> Houses { get; init; }
    internal DbSet<Transaction> Transactions { get; init; }
    internal DbSet<TransactionUser> TransactionUsers { get; init; }

    public Logic.User? GetUser(string name)
    {
        return Users.FirstOrDefault(u => u.UserName == name)
            .Map(u => new Logic.User(this, u.UserName ?? "", (AtHomeStatus)u.AtHomeStatus, u.EatingTotalPeople,
                u.Diet));
    }

    public bool SetUserEatingTotalPeople(string name, int eatingTotal)
    {
        var user = Users.FirstOrDefault(u => u.UserName == name);
        if (user == null)
            return false;
        user.EatingTotalPeople = eatingTotal;
        SaveChanges();
        return true;
    }

    public bool SetUserAtHomeStatus(string name, AtHomeStatus status)
    {
        var user = Users.FirstOrDefault(u => u.UserName == name);
        if (user == null)
            return false;
        user.AtHomeStatus = (int)status;
        SaveChanges();
        return true;
    }

    public bool SetUserDiet(string name, string diet)
    {
        var user = Users.FirstOrDefault(u => u.UserName == name);
        if (user == null)
            return false;
        user.Diet = diet;
        SaveChanges();
        return true;
    }

    public Logic.House? GetHouseForUser(string userName)
    {
        return Users.FirstOrDefault(u => u.UserName == userName)?.House.Map(h =>
            new Logic.House(this, h.Id, h.Name, h.CookingPrice, h.CookingDescription, h.CookingUser?.UserName,
                h.ShoppingList));
    }

    public Logic.House? GetHouse(string? name)
    {
        return Houses.FirstOrDefault(h => h.Name == name).Map(h =>
            new Logic.House(this, h.Id, h.Name, h.CookingPrice, h.CookingDescription, h.CookingUser?.UserName,
                h.ShoppingList));
    }

    public void AddHouse(string name)
    {
        if (Houses.Any(h => h.Name == name))
            return;
        Houses.Add(new House
        {
            Name = name
        });
        SaveChanges();
    }

    public List<Logic.User> GetUsersInHouse(string house)
    {
        return Houses.FirstOrDefault(h => h.Name == house)?.Users.Select(u =>
                       new Logic.User(this, u.UserName ?? "", (AtHomeStatus)u.AtHomeStatus, u.EatingTotalPeople,
                           u.Diet))
                   .ToList() ??
               [];
    }

    public bool AddUserToHouse(string user, string house)
    {
        var houseEntity = Houses.FirstOrDefault(h => h.Name == house);
        if (houseEntity == null)
            return false;
        var userEntity = Users.FirstOrDefault(u => u.UserName == user);
        if (userEntity == null)
            return false;
        houseEntity.Users.Add(userEntity);
        userEntity.House = houseEntity;
        SaveChanges();
        return true;
    }

    public void RemoveUserFromHouse(string user, string house)
    {
        var houseEntity = Houses.FirstOrDefault(h => h.Name == house);
        if (houseEntity == null)
            return;
        var userEntity = Users.FirstOrDefault(u => u.UserName == user);
        if (userEntity == null)
            return;
        houseEntity.Users.Remove(userEntity);
        userEntity.House = null;
        SaveChanges();
    }

    public bool SetHouseCookingUser(string house, string? user)
    {
        var houseEntity = Houses.FirstOrDefault(h => h.Name == house);
        if (houseEntity == null)
            return false;
        var userEntity = user == null ? null : Users.FirstOrDefault(u => u.UserName == user);
        houseEntity.CookingUser = userEntity;
        SaveChanges();
        return true;
    }

    public bool SetHouseCookingPrice(string house, int price)
    {
        var houseEntity = Houses.FirstOrDefault(h => h.Name == house);
        if (houseEntity == null)
            return false;
        houseEntity.CookingPrice = price;
        SaveChanges();
        return true;
    }

    public bool SetHouseCookingDescription(string house, string description)
    {
        var houseEntity = Houses.FirstOrDefault(h => h.Name == house);
        if (houseEntity == null)
            return false;
        houseEntity.CookingDescription = description;
        SaveChanges();
        return true;
    }

    public bool SetHouseShoppingList(string house, string list)
    {
        var houseEntity = Houses.FirstOrDefault(h => h.Name == house);
        if (houseEntity == null)
            return false;
        houseEntity.ShoppingList = list;
        SaveChanges();
        return true;
    }

    public List<Logic.Transaction> GetTransactionsForUser(string user)
    {
        var userEntity = Users.FirstOrDefault(u => u.UserName == user);
        if (userEntity == null)
            return [];
        return userEntity.Transactions.Select(t =>
            new Logic.Transaction(t.EuroCents, t.CookingPoints, t.Description, t.IsPenalty, t.ToUser?.UserName,
                t.Users.Select(u => u?.UserName ?? "").ToArray())
        ).ToList();
    }

    public List<Logic.Transaction> GetTransactionsForHouse(string house)
    {
        var houseEntity = Houses.FirstOrDefault(h => h.Name == house);
        if (houseEntity == null)
            return [];
        return houseEntity.Transactions.Select(t =>
            new Logic.Transaction(t.EuroCents, t.CookingPoints, t.Description, t.IsPenalty, t.ToUser?.UserName,
                t.Users.Select(u => u?.UserName ?? "").ToArray())
        ).ToList();
    }

    public bool AddTransactionToHouse(Logic.Transaction transaction, string house)
    {
        var houseEntity = Houses.FirstOrDefault(h => h.Name == house);
        if (houseEntity == null)
            return false;
        var toUserEntity = transaction.ToUser == null
            ? null
            : Users.FirstOrDefault(u => u.UserName == transaction.ToUser);
        var fromUserEntities = transaction.FromUsers.Select(u => Users.FirstOrDefault(ue => ue.UserName == u)).ToList();
        if (fromUserEntities.Any(u => u == null))
            return false;
        var transactionEntity = new Transaction
        {
            House = houseEntity,
            EuroCents = transaction.EuroCents,
            CookingPoints = transaction.CookingPoints,
            Description = transaction.Description,
            ToUser = toUserEntity,
            IsPenalty = transaction.IsPenalty
        };
        var transactionUsers = fromUserEntities.GroupBy(u => u?.UserName)
            .Select(u => new TransactionUser
            {
                Count = u.Count(),
                Transaction = transactionEntity,
                User = u.First()
            }).ToList();
        transactionEntity.TransactionUsers = transactionUsers;
        houseEntity.Transactions.Add(transactionEntity);
        SaveChanges();
        return true;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var conn = Environment.GetEnvironmentVariable("ConnectionString");
        if (conn != null)
            Console.WriteLine($"Using connection string: {conn}");
        optionsBuilder.UseSqlServer(
            conn ??
            "Server=localhost; Encrypt=false; Database=Potluck; Integrated Security=false; User=SA; Password=SuperSecretPasswordNoOneWillGuessProbably#123456789"
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