using Microsoft.EntityFrameworkCore;

namespace Logic.Models;

[Index(nameof(Name), IsUnique = true)]
public class House
{
    public int Id { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string Name { get; set; }
    public virtual List<User> Users { get; set; } = [];
    public virtual List<Transaction> Transactions { get; set; } = [];

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? CookingUserId { get; set; }
    public virtual User? CookingUser { get; set; }
    public int CookingPrice { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string CookingDescription { get; set; } = "";

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string ShoppingList { get; set; } = "";
}