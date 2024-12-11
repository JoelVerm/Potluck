using Potluck.Helpers;

namespace Potluck.ViewModels;

public class HouseNames(IEnumerable<NamedItem> names)
{
    public List<NamedItem> Names { get; set; } = names.ToList();
}