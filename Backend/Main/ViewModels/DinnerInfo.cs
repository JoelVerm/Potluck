using System.Text.Json.Serialization;

namespace Potluck.ViewModels;

public class DinnerInfo(decimal price, string description)
{
    [JsonInclude] public decimal Price { get; } = price;

    [JsonInclude] public string Description { get; } = description;
}