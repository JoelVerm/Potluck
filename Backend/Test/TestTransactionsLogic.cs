using Data.Models;
using DataTransaction = Data.Models.Transaction;
using House = Data.Models.House;
using Transaction = Logic.Transaction;

[assembly: TestDataSourceDiscovery(TestDataSourceDiscoveryOption.DuringExecution)]

namespace PotluckTest;

[TestClass]
public class TestTransactionsLogic
{
    private static readonly User alice = User("Alice");
    private static readonly House randomHouse = new();

    private static IEnumerable<object[]> TransactionTests { get; } =
    [
        [
            new DataTransaction
            {
                House = randomHouse,
                CookingPoints = 5,
                EuroCents = 1234,
                Users =
                [
                    User("Bob"),
                    User("Charlie")
                ],
                ToUser = alice
            },
            1234,
            5
        ],
        [
            new DataTransaction
            {
                House = randomHouse,
                CookingPoints = 5,
                EuroCents = 1234,
                Users = [alice, User("Bob")],
                ToUser = User("Charlie")
            },
            -1234 / 2,
            -5 / 2
        ],
        [
            new DataTransaction
            {
                House = randomHouse,
                CookingPoints = 5,
                EuroCents = 1234,
                Users = [alice, alice, User("Bob")],
                ToUser = User("Charlie")
            },
            -1234 / 3 * 2,
            -5 / 3 * 2
        ],
        [
            new DataTransaction
            {
                House = randomHouse,
                CookingPoints = 5,
                EuroCents = 1234,
                Users = [alice, User("Bob")],
                ToUser = alice
            },
            1234 - 1234 / 2,
            5 - 5 / 2
        ]
    ];

    private static User User(string name)
    {
        return new User { UserName = name };
    }

    [TestMethod]
    [DynamicData(nameof(TransactionTests))]
    public void BalanceForTransaction(
        DataTransaction dataTransaction,
        int expectedEuroCents,
        int expectedCookingPoints
    )
    {
        // Arrange
        var transaction = new Transaction(dataTransaction.EuroCents,
            dataTransaction.CookingPoints, dataTransaction.Description, dataTransaction.IsPenalty,
            dataTransaction.ToUser!.UserName!, dataTransaction.Users.Select(u => u!.UserName!).ToArray());

        // Act
        var (euroCents, cookingPoints) = transaction.GetForUser(alice.UserName!);

        // Assert
        Assert.AreEqual(expectedEuroCents, euroCents);
        Assert.AreEqual(expectedCookingPoints, cookingPoints);
    }

    [TestMethod]
    public void BalanceForUser()
    {
        // Arrange

        MockDb db = new()
        {
            TransactionsForUser =
            [
                new DataTransaction
                {
                    House = randomHouse,
                    CookingPoints = 5,
                    EuroCents = 1234,
                    Users = [alice, User("Bob")],
                    ToUser = alice
                },
                new DataTransaction
                {
                    House = randomHouse,
                    CookingPoints = 12,
                    EuroCents = 850,
                    Users = [User("Charlie")],
                    ToUser = alice
                },
                new DataTransaction
                {
                    House = randomHouse,
                    CookingPoints = 4,
                    EuroCents = 456,
                    Users =
                    [
                        alice,
                        alice,
                        User("Bob"),
                        User("Charlie")
                    ],
                    ToUser = User("Bob")
                }
            ]
        };

        // Act
        var (euroCents, cookingPoints) = Transaction.GetTotalForUser(db, alice.UserName!);

        // Assert
        const int expectedEuroCents = 1234 - 1234 / 2 + 850 + -456 / 4 * 2;
        Assert.AreEqual(expectedEuroCents, euroCents);
        const int expectedCookingPoints = 5 - 5 / 2 + 12 + -4 / 4 * 2;
        Assert.AreEqual(expectedCookingPoints, cookingPoints);
    }
}