using Logic;
using Logic.Models;
using EatingPerson = Logic.HouseLogic.EatingPerson;

namespace PotluckTest;

[TestClass]
public class TestHouseLogic
{
    private static readonly string[] allPeople = ["Alice", "Bob", "Charlie"];

    private static User CreateUserWithHouse()
    {
        var thisUser = new User
        {
            UserName = "Alice",
            AtHomeStatus = 0,
            EatingTotalPeople = 3,
            Diet = "Vegetarian"
        };
        thisUser.House = new House
        {
            Name = "Hoogh",
            Users =
            [
                thisUser,
                new User
                {
                    UserName = "Bob",
                    AtHomeStatus = 1,
                    EatingTotalPeople = 0,
                    Diet = "No gluten"
                },
                new User
                {
                    UserName = "Charlie",
                    AtHomeStatus = 2,
                    EatingTotalPeople = 1,
                    Diet = ""
                }
            ],
            CookingUser = thisUser,
            CookingDescription = "Pasta",
            CookingPrice = 1234,
            ShoppingList = "Apples, Bananas, Oranges"
        };
        return thisUser;
    }

    [TestMethod]
    public void AllPeople_Valid()
    {
        // Arrange
        var user = CreateUserWithHouse();
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        var result = logic.GetHouse("")!.AllPeople();

        // Assert
        CollectionAssert.AreEqual(allPeople, result);
    }

    [TestMethod]
    public void AddUser_Valid()
    {
        // Arrange
        var user = CreateUserWithHouse();
        var db = new MockDb { User = new User { UserName = "David" }, House = user.House };
        var logic = new HouseLogic(db);

        // Act
        logic.GetHouse("")!.AddUser("David");

        // Assert
        Assert.AreEqual(4, user.House!.Users.Count);
        Assert.IsTrue(user.House.Users.Any(u => u.UserName == "David"));
        Assert.AreEqual(1, db.SaveChangesTimesCalled);
    }

    [TestMethod]
    public void AddUser_Invalid_UserNotFound()
    {
        // Arrange
        var user = CreateUserWithHouse();
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        logic.GetHouse("")!.AddUser("NonsenseUser");

        // Assert
        Assert.AreEqual(3, user.House!.Users.Count);
        Assert.AreEqual(0, db.SaveChangesTimesCalled);
    }

    [TestMethod]
    public void AddUser_Invalid_UserAlreadyInHouse()
    {
        // Arrange
        var user = CreateUserWithHouse();
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        logic.GetHouse("")!.AddUser(user.UserName!);

        // Assert
        Assert.AreEqual(3, user.House!.Users.Count);
        Assert.AreEqual(0, db.SaveChangesTimesCalled);
    }

    [TestMethod]
    public void RemoveUser_Valid()
    {
        // Arrange
        var user = CreateUserWithHouse();
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        logic.GetHouse("")!.RemoveUser("Bob");

        // Assert
        Assert.AreEqual(2, user.House!.Users.Count);
        Assert.IsFalse(user.House.Users.Any(u => u.UserName == "Bob"));
        Assert.AreEqual(1, db.SaveChangesTimesCalled);
    }

    [TestMethod]
    public void RemoveUser_Invalid_UserNotFound()
    {
        // Arrange
        var user = CreateUserWithHouse();
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        logic.GetHouse("")!.RemoveUser("NonsenseUser");

        // Assert
        Assert.AreEqual(3, user.House!.Users.Count);
        Assert.AreEqual(0, db.SaveChangesTimesCalled);
    }

    [TestMethod]
    public void HomeStatusList_Valid()
    {
        // Arrange
        var user = CreateUserWithHouse();
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        var result = logic.GetHouse("")!.HomeStatusList();

        // Assert
        CollectionAssert.AreEqual(
            new Dictionary<string, string>
            {
                ["Alice"] = "At home",
                ["Bob"] = "Away for a bit",
                ["Charlie"] = "Out of town"
            },
            result
        );
    }

    [TestMethod]
    public void CookingUser_Valid()
    {
        // Arrange
        var user = CreateUserWithHouse();
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        var result = logic.GetHouse("")!.CookingUser();

        // Assert
        Assert.AreEqual("Alice", result);
    }

    [TestMethod]
    public void IsUserCooking_Valid()
    {
        // Arrange
        var user = CreateUserWithHouse();
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        var result = logic.GetHouse("")!.IsUserCooking("");

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void SetUserCooking_Valid_True()
    {
        // Arrange
        var user = CreateUserWithHouse();
        user.House!.CookingUser = null;
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        logic.GetHouse("")!.SetUserCooking("", true);

        // Assert
        Assert.AreEqual(user, user.House!.CookingUser);
        Assert.AreEqual(1, db.SaveChangesTimesCalled);
    }

    [TestMethod]
    public void SetUserCooking_Valid_False()
    {
        // Arrange
        var user = CreateUserWithHouse();
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        logic.GetHouse("")!.SetUserCooking("", false);

        // Assert
        Assert.IsNull(user.House!.CookingUser);
        Assert.AreEqual(1, db.SaveChangesTimesCalled);
    }

    [TestMethod]
    public void SetUserCooking_Invalid_NotCookingUser()
    {
        // Arrange
        var user = CreateUserWithHouse();
        user.House!.CookingUser = new User { UserName = "Bob" };
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        logic.GetHouse("")!.SetUserCooking("", true);

        // Assert
        Assert.AreEqual("Bob", user.House!.CookingUser!.UserName);
        Assert.AreEqual(0, db.SaveChangesTimesCalled);
    }

    [TestMethod]
    public void CookingPrice_Valid()
    {
        // Arrange
        var user = CreateUserWithHouse();
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        var result = logic.GetHouse("")!.CookingPrice();

        // Assert
        Assert.AreEqual(12.34m, result);
    }

    [TestMethod]
    public void SetCookingPrice_Valid()
    {
        // Arrange
        var user = CreateUserWithHouse();
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        logic.GetHouse("")!.SetCookingPrice("", 56.78m);

        // Assert
        Assert.AreEqual(5678, user.House!.CookingPrice);
        Assert.AreEqual(1, db.SaveChangesTimesCalled);
    }

    [TestMethod]
    public void SetCookingPrice_Invalid_NotCookingUser()
    {
        // Arrange
        var user = CreateUserWithHouse();
        user.House!.CookingUser = new User { UserName = "Bob" };
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        logic.GetHouse("")!.SetCookingPrice("", 56.78m);

        // Assert
        Assert.AreEqual(1234, user.House!.CookingPrice);
        Assert.AreEqual(0, db.SaveChangesTimesCalled);
    }

    [TestMethod]
    public void SetCookingDescription_Valid()
    {
        // Arrange
        var user = CreateUserWithHouse();
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        logic.GetHouse("")!.SetCookingDescription("", "Pizza");

        // Assert
        Assert.AreEqual("Pizza", user.House!.CookingDescription);
        Assert.AreEqual(1, db.SaveChangesTimesCalled);
    }

    [TestMethod]
    public void SetCookingDescription_Invalid_NotCookingUser()
    {
        // Arrange
        var user = CreateUserWithHouse();
        user.House!.CookingUser = new User { UserName = "Bob" };
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        logic.GetHouse("")!.SetCookingDescription("", "Pizza");

        // Assert
        Assert.AreEqual("Pasta", user.House!.CookingDescription);
        Assert.AreEqual(0, db.SaveChangesTimesCalled);
    }

    [TestMethod]
    public void EatingList_Valid()
    {
        // Arrange
        var user = CreateUserWithHouse();
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        var result = logic.GetHouse("")!.EatingList();

        // Assert
        var expected = new[]
        {
            new EatingPerson("Alice", 3, 0, "Vegetarian"),
            new EatingPerson("Charlie", 1, 0, "")
        };

        Assert.IsTrue(result.Zip(expected).All(p => CompareEatingPeople(p.First, p.Second)));
    }

    private static bool CompareEatingPeople(EatingPerson a, EatingPerson b)
    {
        return a.CookingPoints == b.CookingPoints && a.Count == b.Count && a.Diet == b.Diet && a.Name == b.Name;
    }

    [TestMethod]
    public void SetShoppingList_Valid()
    {
        // Arrange
        var user = CreateUserWithHouse();
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        logic.GetHouse("")!.SetShoppingList("Apples, Bananas, Oranges, Pears");

        // Assert
        Assert.AreEqual("Apples, Bananas, Oranges, Pears", user.House!.ShoppingList);
        Assert.AreEqual(1, db.SaveChangesTimesCalled);
    }

    [TestMethod]
    public void SetHouseName_Valid()
    {
        // Arrange
        var user = CreateUserWithHouse();
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        logic.GetHouse("")!.SetHouseName("NewHouse");

        // Assert
        Assert.AreEqual("NewHouse", user.House!.Name);
        Assert.AreEqual(1, db.SaveChangesTimesCalled);
    }

    [TestMethod]
    public void CreateNew_Valid()
    {
        // Arrange
        var user = new User { UserName = "Alice" };
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        logic.CreateNew("Alice", "NewHouse");

        // Assert
        Assert.AreEqual("NewHouse", user.House?.Name);
        Assert.IsTrue(user.House!.Users.Contains(user));
        Assert.AreEqual(1, db.AddHouseTimesCalled);
        Assert.AreEqual(1, db.SaveChangesTimesCalled);
    }

    [TestMethod]
    public void CreateNew_Invalid_AlreadyHoused()
    {
        // Arrange
        var user = CreateUserWithHouse();
        var db = MockDb.Create(user);
        var logic = new HouseLogic(db);

        // Act
        logic.CreateNew("", "NewHouse");

        // Assert
        Assert.AreEqual("Hoogh", user.House?.Name);
        Assert.AreEqual(0, db.AddHouseTimesCalled);
        Assert.AreEqual(0, db.SaveChangesTimesCalled);
    }
}