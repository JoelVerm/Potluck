using System.Text.Json.Serialization;

namespace Potluck.ViewModels;

public class DinnerInfo(int centsPrice, string description)
{
    [JsonInclude] public int CentsPrice { get; } = centsPrice;

    [JsonInclude] public string Description { get; } = description;
}