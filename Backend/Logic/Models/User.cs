using Microsoft.AspNetCore.Identity;

namespace Logic.Models;

public class User : IdentityUser
{
    public int AtHomeStatus { get; set; }
    public int EatingTotalPeople { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string Diet { get; set; } = "";
    public virtual House? House { get; set; }
    public virtual List<TransactionUser> TransactionUsers { get; set; } = [];
    public virtual List<Transaction> Transactions { get; set; } = [];
    public virtual List<Transaction> GottenTransactions { get; set; } = [];
}