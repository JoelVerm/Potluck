using Logic;
using DataUser = Logic.Models.User;
using LogicUser = Logic.User;

namespace PotluckTest
{
    [TestClass]
    public class TestUser
    {
        [TestMethod]
        [DataRow(5, 5, 1)]
        [DataRow(-1, 0, 0)]
        public void SetEatingTotalPeople_Parameterized(int people, int result, int dbCalled)
        {
            // Arrange
            var user = new DataUser { };
            var db = new MockDb();
            var logic = LogicBase.Create<LogicUser>(user, db);

            // Act
            logic.SetEatingTotalPeople(people);

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
            var user = new DataUser { };
            var db = new MockDb();
            var logic = LogicBase.Create<LogicUser>(user, db);

            // Act
            logic.SetHomeStatus(status);

            // Assert
            Assert.AreEqual(result, user.AtHomeStatus);
            Assert.AreEqual(dbCalled, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void SetDiet_Valid()
        {
            // Arrange
            var user = new DataUser { };
            var db = new MockDb();
            var logic = LogicBase.Create<LogicUser>(user, db);

            // Act
            logic.SetDiet("Vegetarian");

            // Assert
            Assert.AreEqual("Vegetarian", user.Diet);
            Assert.AreEqual(1, db.SaveChangesTimesCalled);
        }
    }
}
