namespace Logic.Models;

public class Transaction
{
    public int Id { get; set; }
    public virtual House House { get; set; }
    public int EuroCents { get; set; }
    public int CookingPoints { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string Description { get; set; } = "";
    public virtual User? ToUser { get; set; }
    public bool IsPenalty { get; set; } = false;
    public virtual List<TransactionUser> TransactionUsers { get; set; } = [];
    public virtual List<User?> Users { get; set; } = [];
}