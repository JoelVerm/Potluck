using System.Text.Json.Serialization;
using Logic;

namespace Potluck.ViewModels;

public class TransactionsList(IEnumerable<Transaction> transactions)
{
    [JsonInclude] public List<Transaction> Transactions { get; set; } = transactions.ToList();
}