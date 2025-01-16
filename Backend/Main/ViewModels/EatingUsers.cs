using System.Text.Json.Serialization;
using Logic;

namespace Potluck.ViewModels;

public class EatingUsers(IEnumerable<EatingPerson> eatingList)
{
    [JsonInclude] public List<EatingPerson> EatingList { get; set; } = eatingList.ToList();
}