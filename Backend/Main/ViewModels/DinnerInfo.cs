namespace Potluck.ViewModels;

public class DinnerInfo(decimal price, string description)
{
    public decimal Price { get; set; } = price;
    public string Description { get; set; } = description;
}