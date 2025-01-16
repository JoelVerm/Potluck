namespace Potluck.ViewModels;

public class NewTransactionVM
{
    public int EuroCents { get; set; }
    public int CookingPoints { get; set; }
    public string Description { get; set; } = "";
    public bool IsPenalty { get; set; }
    public string? ToUser { get; set; }
    public string[] FromUsers { get; set; } = [];
}