using System.Text.Json.Serialization;
using Logic;

namespace Potluck.ViewModels;

public class EatingUsers(IEnumerable<HouseLogic.EatingPerson> eatingList)
{
    [JsonInclude] public List<HouseLogic.EatingPerson> EatingList { get; set; } = eatingList.ToList();
}