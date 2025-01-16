namespace Data.Models;

// ReSharper disable file PropertyCanBeMadeInitOnly.Global
// ReSharper disable file EntityFramework.ModelValidation.UnlimitedStringLength
// ReSharper disable file EntityFramework.ModelValidation.CircularDependency
// ReSharper disable file ClassWithVirtualMembersNeverInherited.Global
public class TransactionUser
{
    public int Id { get; set; }
    public virtual required Transaction Transaction { get; set; }
    public virtual User? User { get; set; }
    public int Count { get; set; }
}