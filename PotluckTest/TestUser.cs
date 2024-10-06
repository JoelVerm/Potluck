using Backend_Example.Database;
using Backend_Example.Logic;
using DataUser = Backend_Example.Database.User;
using LogicUser = Backend_Example.Logic.User;

namespace PotluckTest
{
    [TestClass]
    public class TestUser
    {
        [TestMethod]
        public void SetEatingTotalPeople_Valid()
        {
            // Arrange
            var user = new DataUser { };
            var db = new MockDb();
            var logic = LogicBase.Create<LogicUser>(user, db);

            // Act
            logic.SetEatingTotalPeople(5);

            // Assert
            Assert.AreEqual(5, user.EatingTotalPeople);
            Assert.AreEqual(1, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void SetEatingTotalPeople_Invalid()
        {
            // Arrange
            var user = new DataUser { };
            var db = new MockDb();
            var logic = LogicBase.Create<LogicUser>(user, db);

            // Act
            logic.SetEatingTotalPeople(-1);

            // Assert
            Assert.AreEqual(0, user.EatingTotalPeople);
            Assert.AreEqual(0, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void SetHomeStatus_Valid()
        {
            // Arrange
            var user = new DataUser { };
            var db = new MockDb();
            var logic = LogicBase.Create<LogicUser>(user, db);

            // Act
            logic.SetHomeStatus("Away for a bit");

            // Assert
            Assert.AreEqual(1, user.AtHomeStatus);
            Assert.AreEqual(1, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void SetHomeStatus_Invalid()
        {
            // Arrange
            var user = new DataUser { };
            var db = new MockDb();
            var logic = LogicBase.Create<LogicUser>(user, db);

            // Act
            logic.SetHomeStatus("Invalid status");

            // Assert
            Assert.AreEqual(0, user.AtHomeStatus);
            Assert.AreEqual(0, db.SaveChangesTimesCalled);
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
