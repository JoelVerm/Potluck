using System.Text.Json.Serialization;

namespace Potluck.ViewModels;

public class HouseNames(IEnumerable<NamedItem> names)
{
    [JsonInclude] public List<NamedItem> Names { get; set; } = names.ToList();
}