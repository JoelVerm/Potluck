namespace Data.Models;

// ReSharper disable file PropertyCanBeMadeInitOnly.Global
// ReSharper disable file EntityFramework.ModelValidation.UnlimitedStringLength
// ReSharper disable file EntityFramework.ModelValidation.CircularDependency
// ReSharper disable file ClassWithVirtualMembersNeverInherited.Global
public class Transaction
{
    public int Id { get; set; }
    public virtual required House House { get; set; }
    public int EuroCents { get; set; }
    public int CookingPoints { get; set; }
    public string Description { get; set; } = "";
    public virtual User? ToUser { get; set; }
    public bool IsPenalty { get; set; }
    public virtual List<TransactionUser> TransactionUsers { get; set; } = [];
    public virtual List<User?> Users { get; set; } = [];
}