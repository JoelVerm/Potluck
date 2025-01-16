using System.Text.Json.Serialization;

namespace Potluck.ViewModels;

public class TotalBalance((int euroCents, int cookingPoints) balance)
{
    [JsonInclude] public int CookingPoints { get; set; } = balance.cookingPoints;

    [JsonInclude] public int EuroCents { get; set; } = balance.euroCents;
}