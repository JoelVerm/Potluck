using System.Text.Json.Serialization;

namespace Potluck.ViewModels;

public class HouseResponse(string name)
{
    [JsonInclude] public string Name { get; set; } = name;
}