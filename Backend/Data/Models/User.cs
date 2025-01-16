using Microsoft.AspNetCore.Identity;

namespace Data.Models;

// ReSharper disable file PropertyCanBeMadeInitOnly.Global
// ReSharper disable file EntityFramework.ModelValidation.UnlimitedStringLength
// ReSharper disable file EntityFramework.ModelValidation.CircularDependency
// ReSharper disable file ClassWithVirtualMembersNeverInherited.Global
public class User : IdentityUser
{
    public int AtHomeStatus { get; set; }
    public int EatingTotalPeople { get; set; }
    public string Diet { get; set; } = "";
    public virtual House? House { get; set; }
    public virtual List<TransactionUser> TransactionUsers { get; set; } = [];
    public virtual List<Transaction> Transactions { get; set; } = [];
    public virtual List<Transaction> GottenTransactions { get; set; } = [];
}