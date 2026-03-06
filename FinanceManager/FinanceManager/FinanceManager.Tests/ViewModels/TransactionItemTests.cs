using FinanceManager.Models;
using FinanceManager.ViewModels;
using Xunit;

namespace FinanceManager.Tests.ViewModels;

public class TransactionItemTests
{
    private static Transaction CreateTransaction(TransactionType type) =>
        new()
        {
            Id = Guid.NewGuid(),
            Amount = 100,
            Category = "Test",
            Type = type,
            Date = DateTime.Today,
            Comment = ""
        };

    [Fact]
    public void TypeDisplay_Income_ReturnsDohod()
    {
        var transaction = CreateTransaction(TransactionType.Income);
        var item = new TransactionItem(transaction);
        Assert.Equal("Доход", item.TypeDisplay);
    }

    [Fact]
    public void TypeDisplay_Expense_ReturnsRashod()
    {
        var transaction = CreateTransaction(TransactionType.Expense);
        var item = new TransactionItem(transaction);
        Assert.Equal("Расход", item.TypeDisplay);
    }

    [Fact]
    public void Properties_MirrorTransaction()
    {
        var transaction = CreateTransaction(TransactionType.Expense);
        transaction.Amount = 250.50m;
        transaction.Category = "Еда";
        transaction.Comment = "Обед";

        var item = new TransactionItem(transaction);

        Assert.Equal(transaction.Id, item.Id);
        Assert.Equal(250.50m, item.Amount);
        Assert.Equal("Еда", item.Category);
        Assert.Equal("Обед", item.Comment);
        Assert.Equal(transaction.Date, item.Date);
        Assert.Equal(TransactionType.Expense, item.Type);
    }
}
