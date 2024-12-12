using Logic;
using Logic.Models;

namespace PotluckTest;

[TestClass]
public class TestUserLogic
{
    [TestMethod]
    [DataRow(5, 5, 1)]
    [DataRow(-1, 0, 0)]
    public void SetEatingTotalPeople_Parameterized(int people, int result, int dbCalled)
    {
        // Arrange
        var user = new User();
        var db = new MockDb { User = user };
        var logic = new UserLogic(db);

        // Act
        logic.GetUser("")!.SetEatingTotalPeople(people);

        // Assert
        Assert.AreEqual(result, user.EatingTotalPeople);
        Assert.AreEqual(dbCalled, db.SaveChangesTimesCalled);
    }

    [TestMethod]
    [DataRow("Away for a bit", 1, 1)]
    [DataRow("Invalid status", 0, 0)]
    public void SetHomeStatus_Parameterized(string status, int result, int dbCalled)
    {
        // Arrange
        var user = new User();
        var db = new MockDb { User = user };
        var logic = new UserLogic(db);

        // Act
        logic.GetUser("")!.SetHomeStatus(status);

        // Assert
        Assert.AreEqual(result, user.AtHomeStatus);
        Assert.AreEqual(dbCalled, db.SaveChangesTimesCalled);
    }

    [TestMethod]
    public void SetDiet_Valid()
    {
        // Arrange
        var user = new User();
        var db = new MockDb { User = user };
        var logic = new UserLogic(db);

        // Act
        logic.GetUser("")!.SetDiet("Vegetarian");

        // Assert
        Assert.AreEqual("Vegetarian", user.Diet);
        Assert.AreEqual(1, db.SaveChangesTimesCalled);
    }
}