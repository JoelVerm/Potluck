using System.Text.Json.Serialization;

namespace Potluck.ViewModels;

public class NamedItem(string name)
{
    [JsonInclude] public string Name { get; } = name;
}