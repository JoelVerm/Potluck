using Backend_Example;
using Backend_Example.Database;
using Backend_Example.Logic;
using DataHouse = Backend_Example.Database.House;
using DataTransaction = Backend_Example.Database.Transaction;
using DataUser = Backend_Example.Database.User;
using LogicTransaction = Backend_Example.Logic.Transactions.Transaction;
using LogicTransactions = Backend_Example.Logic.Transactions;

namespace PotluckTest
{
    [TestClass]
    public class TestTransactions
    {
        [TestMethod]
        public void BalanceForTransaction_Valid_OnlyTo()
        {
            // Arrange
            var user = new DataUser { UserName = "Alice" };
            var transaction = new DataTransaction
            {
                CookingPoints = 5,
                EuroCents = 1234,
                Users = [new DataUser { UserName = "Bob" }, new DataUser { UserName = "Charlie" }],
                ToUser = user,
            };

            // Act
            var (euroCents, cookingPoints) = LogicTransactions.BalanceFor(user, transaction);

            // Assert
            Assert.AreEqual(1234, euroCents);
            Assert.AreEqual(5, cookingPoints);
        }

        [TestMethod]
        public void BalanceForTransaction_Valid_OnlyFrom()
        {
            // Arrange
            var user = new DataUser { UserName = "Alice" };
            var transaction = new DataTransaction
            {
                CookingPoints = 5,
                EuroCents = 1234,
                Users = [user, new DataUser { UserName = "Bob" }],
                ToUser = new DataUser { UserName = "Charlie" },
            };

            // Act
            var (euroCents, cookingPoints) = LogicTransactions.BalanceFor(user, transaction);

            // Assert
            Assert.AreEqual(-1234 / 2, euroCents);
            Assert.AreEqual(-5 / 2, cookingPoints);
        }

        [TestMethod]
        public void BalanceForTransaction_Valid_FromMultiple()
        {
            // Arrange
            var user = new DataUser { UserName = "Alice" };
            var transaction = new DataTransaction
            {
                CookingPoints = 5,
                EuroCents = 1234,
                Users = [user, user, new DataUser { UserName = "Bob" }],
                ToUser = new DataUser { UserName = "Charlie" },
            };

            // Act
            var (euroCents, cookingPoints) = LogicTransactions.BalanceFor(user, transaction);

            // Assert
            Assert.AreEqual(-1234 / 3 * 2, euroCents);
            Assert.AreEqual(-5 / 3 * 2, cookingPoints);
        }

        [TestMethod]
        public void BalanceForTransaction_Valid_FromAndTo()
        {
            // Arrange
            var user = new DataUser { UserName = "Alice" };
            var transaction = new DataTransaction
            {
                CookingPoints = 5,
                EuroCents = 1234,
                Users = [user, new DataUser { UserName = "Bob" }],
                ToUser = user,
            };

            // Act
            var (euroCents, cookingPoints) = LogicTransactions.BalanceFor(user, transaction);

            // Assert
            var expectedEuroCents = 1234 - 1234 / 2;
            Assert.AreEqual(expectedEuroCents, euroCents);
            var expectedCookingPoints = 5 - 5 / 2;
            Assert.AreEqual(expectedCookingPoints, cookingPoints);
        }

        [TestMethod]
        public void BalanceForUser_Valid()
        {
            // Arrange
            var user = new DataUser { UserName = "Alice" };
            user.Transactions =
            [
                new DataTransaction
                {
                    CookingPoints = 5,
                    EuroCents = 1234,
                    Users = [user, new DataUser { UserName = "Bob" }],
                    ToUser = user,
                },
                new DataTransaction
                {
                    CookingPoints = 12,
                    EuroCents = 850,
                    Users = [new DataUser { UserName = "Charlie" }],
                    ToUser = user,
                },
                new DataTransaction
                {
                    CookingPoints = 4,
                    EuroCents = 456,
                    Users =
                    [
                        user,
                        user,
                        new DataUser { UserName = "Bob" },
                        new DataUser { UserName = "Charlie" },
                    ],
                    ToUser = new DataUser { UserName = "Bob" },
                },
            ];

            // Act
            var (cookingPoints, euros) = LogicTransactions.BalanceFor(user);

            // Assert
            var expectedEuroCents = (12.34m - 12.34m / 2) + (8.50m) + (-4.56m / 4 * 2);
            Assert.AreEqual(expectedEuroCents, euros);
            var expectedCookingPoints = (5 - 5 / 2) + (12) + (-4 / 4 * 2);
            Assert.AreEqual(expectedCookingPoints, cookingPoints);
        }

        [TestMethod]
        public void AddTransaction_Valid()
        {
            // Arrange
            var user = new DataUser { UserName = "Alice" };
            var bob = new DataUser { UserName = "Bob" };
            var charlie = new DataUser { UserName = "Charlie" };
            user.House = new DataHouse { Users = [user, bob, charlie], Transactions = [] };
            var db = new MockDb();
            var transactions = LogicBase.Create<LogicTransactions>(user, db);

            // Act
            transactions.AddTransaction(
                new LogicTransaction("Bob", ["Alice", "Charlie", "Charlie"], "Shopping", 6.80m, 3)
            );

            // Assert
            var transaction = user.House.Transactions.First();
            Assert.AreEqual(bob, transaction.ToUser);
            CollectionAssert.AreEqual(
                new List<DataUser> { user, charlie, charlie },
                transaction.Users
            );
            Assert.AreEqual("Shopping", transaction.Description);
            Assert.AreEqual(680, transaction.EuroCents);
            Assert.AreEqual(3, transaction.CookingPoints);
            Assert.AreEqual(1, db.SaveChangesTimesCalled);
        }

        [TestMethod]
        public void AddTransaction_Invalid_UserNotInHouse()
        {
            // Arrange
            var user = new DataUser { UserName = "Alice" };
            var bob = new DataUser { UserName = "Bob" };
            var charlie = new DataUser { UserName = "Charlie" };
            user.House = new DataHouse { Users = [user, bob, charlie], Transactions = [] };
            var db = new MockDb();
            var transactions = LogicBase.Create<LogicTransactions>(user, db);

            // Act
            transactions.AddTransaction(
                new LogicTransaction("Bob", ["Alice", "Charlie", "David"], "Shopping", 6.80m, 3)
            );

            // Assert
            Assert.AreEqual(0, user.House.Transactions.Count);
            Assert.AreEqual(0, db.SaveChangesTimesCalled);
        }
    }
}
