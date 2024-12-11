namespace Potluck.ViewModels;

public class NamedItem(string name)
{
    public string Name { get; set; } = name;
}