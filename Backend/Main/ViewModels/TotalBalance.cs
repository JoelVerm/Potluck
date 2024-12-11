namespace Potluck.ViewModels;

public class TotalBalance((int cookingPoints, decimal euros) balance)
{
    public int CookingPoints { get; set; } = balance.cookingPoints;
    public decimal Euros { get; set; } = balance.euros;
}