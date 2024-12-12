using System.Text.Json.Serialization;
using Logic;

namespace Potluck.ViewModels;

public class TransactionsList(IEnumerable<TransactionsLogic.Transaction> transactions)
{
    [JsonInclude] public List<TransactionsLogic.Transaction> Transactions { get; set; } = transactions.ToList();
}