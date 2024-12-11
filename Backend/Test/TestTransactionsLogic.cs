using Logic;
using Logic.Models;
using DataTransaction = Logic.Models.Transaction;
using LogicTransaction = Logic.TransactionsLogic.Transaction;

[assembly: TestDataSourceDiscovery(TestDataSourceDiscoveryOption.DuringExecution)]

namespace PotluckTest;

[TestClass]
public class TestTransactionsLogic
{
    private static readonly User alice = new() { UserName = "Alice" };

    private static IEnumerable<object[]> TransactionTests { get; } =
    [
        [
            new DataTransaction
            {
                CookingPoints = 5,
                EuroCents = 1234,
                Users =
                [
                    new User { UserName = "Bob" },
                    new User { UserName = "Charlie" }
                ],
                ToUser = alice
            },
            1234,
            5
        ],
        [
            new DataTransaction
            {
                CookingPoints = 5,
                EuroCents = 1234,
                Users = [alice, new User { UserName = "Bob" }],
                ToUser = new User { UserName = "Charlie" }
            },
            -1234 / 2,
            -5 / 2
        ],
        [
            new DataTransaction
            {
                CookingPoints = 5,
                EuroCents = 1234,
                Users = [alice, alice, new User { UserName = "Bob" }],
                ToUser = new User { UserName = "Charlie" }
            },
            -1234 / 3 * 2,
            -5 / 3 * 2
        ],
        [
            new DataTransaction
            {
                CookingPoints = 5,
                EuroCents = 1234,
                Users = [alice, new User { UserName = "Bob" }],
                ToUser = alice
            },
            1234 - 1234 / 2,
            5 - 5 / 2
        ]
    ];

    [TestMethod]
    [DynamicData(nameof(TransactionTests))]
    public void BalanceForTransaction_Parameterized(
        DataTransaction transaction,
        int expectedEuroCents,
        int expectedCookingPoints
    )
    {
        // Arrange

        // Act
        var (euroCents, cookingPoints) = TransactionsLogic.BalanceFor(alice, transaction);

        // Assert
        Assert.AreEqual(expectedEuroCents, euroCents);
        Assert.AreEqual(expectedCookingPoints, cookingPoints);
    }

    [TestMethod]
    public void BalanceForUser_Valid()
    {
        // Arrange
        var user = new User { UserName = "Alice" };
        user.Transactions =
        [
            new DataTransaction
            {
                CookingPoints = 5,
                EuroCents = 1234,
                Users = [user, new User { UserName = "Bob" }],
                ToUser = user
            },
            new DataTransaction
            {
                CookingPoints = 12,
                EuroCents = 850,
                Users = [new User { UserName = "Charlie" }],
                ToUser = user
            },
            new DataTransaction
            {
                CookingPoints = 4,
                EuroCents = 456,
                Users =
                [
                    user,
                    user,
                    new User { UserName = "Bob" },
                    new User { UserName = "Charlie" }
                ],
                ToUser = new User { UserName = "Bob" }
            }
        ];

        // Act
        var (cookingPoints, euros) = user.Balance();

        // Assert
        var expectedEuroCents = 12.34m - 12.34m / 2 + 8.50m + -4.56m / 4 * 2;
        Assert.AreEqual(expectedEuroCents, euros);
        var expectedCookingPoints = 5 - 5 / 2 + 12 + -4 / 4 * 2;
        Assert.AreEqual(expectedCookingPoints, cookingPoints);
    }

    [TestMethod]
    public void AddTransaction_Valid()
    {
        // Arrange
        var user = new User { UserName = "Alice" };
        var bob = new User { UserName = "Bob" };
        var charlie = new User { UserName = "Charlie" };
        user.House = new House { Users = [user, bob, charlie], Transactions = [] };
        var db = new MockDb();
        var transactions = new HouseLogic(db);

        // Act
        transactions.GetHouse("")!.AddTransaction(
            new LogicTransaction("Bob", ["Alice", "Charlie", "Charlie"], "Shopping", 6.80m, 3)
        );

        // Assert
        var transaction = user.House.Transactions.First();
        Assert.AreEqual(bob, transaction.ToUser);
        CollectionAssert.AreEqual(
            new List<User> { user, charlie, charlie },
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
        var user = new User { UserName = "Alice" };
        var bob = new User { UserName = "Bob" };
        var charlie = new User { UserName = "Charlie" };
        user.House = new House { Users = [user, bob, charlie], Transactions = [] };
        var db = new MockDb();
        var transactions = new HouseLogic(db);

        // Act
        transactions.GetHouse("")!.AddTransaction(
            new LogicTransaction("Bob", ["Alice", "Charlie", "David"], "Shopping", 6.80m, 3)
        );

        // Assert
        Assert.AreEqual(0, user.House.Transactions.Count);
        Assert.AreEqual(0, db.SaveChangesTimesCalled);
    }
}