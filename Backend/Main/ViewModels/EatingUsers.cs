using Logic;

namespace Potluck.ViewModels;

public class EatingUsers(IEnumerable<HouseLogic.EatingPerson> eatingList)
{
    public List<HouseLogic.EatingPerson> EatingList { get; set; } = eatingList.ToList();
}