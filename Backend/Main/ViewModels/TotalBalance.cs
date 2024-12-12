using System.Text.Json.Serialization;

namespace Potluck.ViewModels;

public class TotalBalance((int cookingPoints, decimal euros) balance)
{
    [JsonInclude] public int CookingPoints { get; set; } = balance.cookingPoints;

    [JsonInclude] public decimal Euros { get; set; } = balance.euros;
}