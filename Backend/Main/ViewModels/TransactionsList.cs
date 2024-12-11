using Logic;

namespace Potluck.ViewModels;

public class TransactionsList(IEnumerable<TransactionsLogic.Transaction> transactions)
{
    public List<TransactionsLogic.Transaction> Transactions { get; set; } = transactions.ToList();
}