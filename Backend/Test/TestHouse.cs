using Logic;
using DataHouse = Logic.Models.House;
using DataUser = Logic.Models.User;
using EatingPerson = Logic.House.EatingPerson;
using LogicHouse = Logic.House;

namespace PotluckTest
{
    [TestClass]
    public class TestHouse
    {
        private DataUser CreateUserWithHouse()
        {
            var thisUser = new DataUser
            {
                UserName = "Alice",
                AtHomeStatus = 0,
                EatingTotalPeople = 3,
                Diet = "Vegetarian",
            };
            thisUser.House = new DataHouse
            {
                Name = "Hoogh",
                Users =
                [
                    thisUser,
                    new DataUser
                    {
                        UserName = "Bob",
                        AtHomeStatus = 1,
                        EatingTotalPeople = 0,
                        Diet = "No gluten",
                    },
                    new DataUser
                    {
                        UserName = "Charlie",
                        AtHomeStatus = 2,
                        EatingTotalPeople = 1,
                        Diet = "",
                    },
                ],
                CookingUser = thisUser,
                CookingDescription = "Pasta",
                CookingPrice = 1234,
                ShoppingList = "Apples, Bananas, Oranges",
            };
            return thisUser;
        }

        private static readonly string[] allPeople = ["Alice", "Bob", "Charlie"];

        [TestMethod]
        public void AllPeople_Valid()
        {
            // Arrange
            var user = CreateUserWithHouse();
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            var result = logic.AllPeople();

            // Assert
            CollectionAssert.AreEqual(allPeople, result);
        }

        [TestMethod]
        public void AddUser_Valid()
        {
            // Arrange
            var user = CreateUserWithHouse();
            var db = new MockDb { User = new DataUser { UserName = "David" } };
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.AddUser("David");

            // Assert
            Assert.AreEqual(4, user.House!.Users.Count);
            Assert.IsTrue(user.House.Users.Any(u => u.UserName == "David"));
            Assert.AreEqual(1, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void AddUser_Invalid_NoHouse()
        {
            // Arrange
            var user = new DataUser { UserName = "Alice" };
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.AddUser("SomeUser");

            // Assert
            Assert.AreEqual(0, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void AddUser_Invalid_UserNotFound()
        {
            // Arrange
            var user = CreateUserWithHouse();
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.AddUser("NonsenseUser");

            // Assert
            Assert.AreEqual(3, user.House!.Users.Count);
            Assert.AreEqual(0, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void AddUser_Invalid_UserAlreadyInHouse()
        {
            // Arrange
            var user = CreateUserWithHouse();
            var db = new MockDb { User = user };
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.AddUser(user.UserName!);

            // Assert
            Assert.AreEqual(3, user.House!.Users.Count);
            Assert.AreEqual(0, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void RemoveUser_Valid()
        {
            // Arrange
            var user = CreateUserWithHouse();
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.RemoveUser("Bob");

            // Assert
            Assert.AreEqual(2, user.House!.Users.Count);
            Assert.IsFalse(user.House.Users.Any(u => u.UserName == "Bob"));
            Assert.AreEqual(1, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void RemoveUser_Invalid_NoHouse()
        {
            // Arrange
            var user = new DataUser { UserName = "Alice" };
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.RemoveUser("Bob");

            // Assert
            Assert.AreEqual(0, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void RemoveUser_Invalid_UserNotFound()
        {
            // Arrange
            var user = CreateUserWithHouse();
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.RemoveUser("NonsenseUser");

            // Assert
            Assert.AreEqual(3, user.House!.Users.Count);
            Assert.AreEqual(0, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void HomeStatusList_Valid()
        {
            // Arrange
            var user = CreateUserWithHouse();
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            var result = logic.HomeStatusList();

            // Assert
            CollectionAssert.AreEqual(
                new Dictionary<string, string>
                {
                    ["Alice"] = "At home",
                    ["Bob"] = "Away for a bit",
                    ["Charlie"] = "Out of town",
                },
                result
            );
        }

        [TestMethod]
        public void CookingUser_Valid()
        {
            // Arrange
            var user = CreateUserWithHouse();
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            var result = logic.CookingUser();

            // Assert
            Assert.AreEqual("Alice", result);
        }

        [TestMethod]
        public void IsUserCooking_Valid()
        {
            // Arrange
            var user = CreateUserWithHouse();
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            var result = logic.IsUserCooking();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SetUserCooking_Valid_True()
        {
            // Arrange
            var user = CreateUserWithHouse();
            user.House!.CookingUser = null;
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.SetUserCooking(true);

            // Assert
            Assert.AreEqual(user, user.House!.CookingUser);
            Assert.AreEqual(1, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void SetUserCooking_Valid_False()
        {
            // Arrange
            var user = CreateUserWithHouse();
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.SetUserCooking(false);

            // Assert
            Assert.IsNull(user.House!.CookingUser);
            Assert.AreEqual(1, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void SetUserCooking_Invalid_NoHouse()
        {
            // Arrange
            var user = new DataUser { UserName = "Alice" };
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.SetUserCooking(true);

            // Assert
            Assert.AreEqual(0, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void SetUserCooking_Invalid_NotCookingUser()
        {
            // Arrange
            var user = CreateUserWithHouse();
            user.House!.CookingUser = new DataUser { UserName = "Bob" };
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.SetUserCooking(true);

            // Assert
            Assert.AreEqual("Bob", user.House!.CookingUser!.UserName);
            Assert.AreEqual(0, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void CookingPrice_Valid()
        {
            // Arrange
            var user = CreateUserWithHouse();
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            var result = logic.CookingPrice();

            // Assert
            Assert.AreEqual(12.34m, result);
        }

        [TestMethod]
        public void SetCookingPrice_Valid()
        {
            // Arrange
            var user = CreateUserWithHouse();
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.SetCookingPrice(56.78m);

            // Assert
            Assert.AreEqual(5678, user.House!.CookingPrice);
            Assert.AreEqual(1, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void SetCookingPrice_Invalid_NoHouse()
        {
            // Arrange
            var user = new DataUser { UserName = "Alice" };
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.SetCookingPrice(56.78m);

            // Assert
            Assert.AreEqual(0, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void SetCookingPrice_Invalid_NotCookingUser()
        {
            // Arrange
            var user = CreateUserWithHouse();
            user.House!.CookingUser = new DataUser { UserName = "Bob" };
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.SetCookingPrice(56.78m);

            // Assert
            Assert.AreEqual(1234, user.House!.CookingPrice);
            Assert.AreEqual(0, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void SetCookingDescription_Valid()
        {
            // Arrange
            var user = CreateUserWithHouse();
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.SetCookingDescription("Pizza");

            // Assert
            Assert.AreEqual("Pizza", user.House!.CookingDescription);
            Assert.AreEqual(1, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void SetCookingDescription_Invalid_NoHouse()
        {
            // Arrange
            var user = new DataUser { UserName = "Alice" };
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.SetCookingDescription("Pizza");

            // Assert
            Assert.AreEqual(0, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void SetCookingDescription_Invalid_NotCookingUser()
        {
            // Arrange
            var user = CreateUserWithHouse();
            user.House!.CookingUser = new DataUser { UserName = "Bob" };
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.SetCookingDescription("Pizza");

            // Assert
            Assert.AreEqual("Pasta", user.House!.CookingDescription);
            Assert.AreEqual(0, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void EatingList_Valid()
        {
            // Arrange
            var user = CreateUserWithHouse();
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            var result = logic.EatingList();

            // Assert
            var expected = new[]
            {
                new EatingPerson("Alice", 3, 0, "Vegetarian"),
                new EatingPerson("Charlie", 1, 0, ""),
            };
            var compare = (EatingPerson a, EatingPerson b) =>
                a.CookingPoints == b.CookingPoints
                && a.Count == b.Count
                && a.Diet == b.Diet
                && a.Name == b.Name;
            Assert.IsTrue(result.Zip(expected).All(p => compare(p.First, p.Second)));
        }

        [TestMethod]
        public void SetShoppingList_Valid()
        {
            // Arrange
            var user = CreateUserWithHouse();
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.SetShoppingList("Apples, Bananas, Oranges, Pears");

            // Assert
            Assert.AreEqual("Apples, Bananas, Oranges, Pears", user.House!.ShoppingList);
            Assert.AreEqual(1, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void SetShoppingList_Invalid_NoHouse()
        {
            // Arrange
            var user = new DataUser { UserName = "Alice" };
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.SetShoppingList("Apples, Bananas, Oranges, Pears");

            // Assert
            Assert.AreEqual(0, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void SetHouseName_Valid()
        {
            // Arrange
            var user = CreateUserWithHouse();
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.SetHouseName("NewHouse");

            // Assert
            Assert.AreEqual("NewHouse", user.House!.Name);
            Assert.AreEqual(1, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void SetHouseName_Invalid_NoHouse()
        {
            // Arrange
            var user = new DataUser { UserName = "Alice" };
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.SetHouseName("NewHouse");

            // Assert
            Assert.AreEqual(0, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void CreateNew_Valid()
        {
            // Arrange
            var user = new DataUser { UserName = "Alice" };
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.CreateNew("NewHouse");

            // Assert
            Assert.AreEqual("NewHouse", user.House?.Name);
            Assert.IsTrue(user.House!.Users.Contains(user));
            Assert.AreEqual(1, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void CreateNew_Invalid_AlreadyHoused()
        {
            // Arrange
            var user = CreateUserWithHouse();
            var db = new MockDb();
            var logic = LogicBase.Create<LogicHouse>(user, db);

            // Act
            logic.CreateNew("NewHouse");

            // Assert
            Assert.AreEqual("Hoogh", user.House?.Name);
            Assert.AreEqual(0, db.SaveChangesTimesCalled);
        }
    }
}
