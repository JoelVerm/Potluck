using Logic;

namespace PotluckTest;

[TestClass]
public class TestUserLogic
{
    [TestMethod]
    public void SetHomeStatusStringCount()
    {
        // Arrange

        // Act
        var homeStatusEnumSize = Enum.GetNames(typeof(AtHomeStatus)).Length;
        var homeStatusStringSize = User.homeStatus.Length;

        // Assert
        Assert.AreEqual(homeStatusEnumSize, homeStatusStringSize);
    }
}