using FinanceManager.Models;
using FinanceManager.Services;
using Xunit;

namespace FinanceManager.Tests.Services;

public class DataServiceTests
{
    private static DataService CreateInMemory() => new DataService(inMemory: true);

    private static Transaction Income(decimal amount, string category = "Зарплата", DateTime? date = null) =>
        new()
        {
            Amount = amount,
            Category = category,
            Type = TransactionType.Income,
            Date = date ?? DateTime.Today
        };

    private static Transaction Expense(decimal amount, string category = "Еда", DateTime? date = null) =>
        new()
        {
            Amount = amount,
            Category = category,
            Type = TransactionType.Expense,
            Date = date ?? DateTime.Today
        };

    [Fact]
    public void GetAll_Empty_ReturnsEmptyList()
    {
        var service = CreateInMemory();
        var all = service.GetAll();
        Assert.Empty(all);
    }

    [Fact]
    public void GetAll_ReturnsCopy_NotInternalList()
    {
        var service = CreateInMemory();
        service.AddTransaction(Income(100));
        var first = service.GetAll();
        var second = service.GetAll();
        Assert.NotSame(first, second);
        Assert.Single(first);
        Assert.Single(second);
    }

    [Fact]
    public void AddTransaction_Valid_Succeeds()
    {
        var service = CreateInMemory();
        var t = Income(100);
        service.AddTransaction(t);
        var all = service.GetAll();
        Assert.Single(all);
        Assert.Equal(100, all[0].Amount);
        Assert.Equal("Зарплата", all[0].Category);
        Assert.Equal(TransactionType.Income, all[0].Type);
    }

    [Fact]
    public void AddTransaction_AmountZero_Throws()
    {
        var service = CreateInMemory();
        var t = Income(0);
        var ex = Assert.Throws<ArgumentException>(() => service.AddTransaction(t));
        Assert.Contains("Сумма", ex.Message);
    }

    [Fact]
    public void AddTransaction_AmountNegative_Throws()
    {
        var service = CreateInMemory();
        var t = Income(-10);
        var ex = Assert.Throws<ArgumentException>(() => service.AddTransaction(t));
        Assert.Contains("Сумма", ex.Message);
    }

    [Fact]
    public void AddTransaction_EmptyCategory_Throws()
    {
        var service = CreateInMemory();
        var t = Income(100);
        t.Category = "";
        var ex = Assert.Throws<ArgumentException>(() => service.AddTransaction(t));
        Assert.Contains("Категория", ex.Message);
    }

    [Fact]
    public void AddTransaction_WhiteSpaceCategory_Throws()
    {
        var service = CreateInMemory();
        var t = Income(100);
        t.Category = "   ";
        var ex = Assert.Throws<ArgumentException>(() => service.AddTransaction(t));
        Assert.Contains("Категория", ex.Message);
    }

    [Fact]
    public void DeleteTransaction_ExistingId_Removes()
    {
        var service = CreateInMemory();
        var t = Income(100);
        service.AddTransaction(t);
        service.DeleteTransaction(t.Id);
        Assert.Empty(service.GetAll());
    }

    [Fact]
    public void DeleteTransaction_NonExistingId_NoException()
    {
        var service = CreateInMemory();
        service.AddTransaction(Income(100));
        service.DeleteTransaction(Guid.NewGuid());
        Assert.Single(service.GetAll());
    }

    [Fact]
    public void GetTotalIncome_ReturnsSumOfIncomeOnly()
    {
        var service = CreateInMemory();
        service.AddTransaction(Income(100));
        service.AddTransaction(Income(50));
        service.AddTransaction(Expense(30));
        Assert.Equal(150, service.GetTotalIncome());
    }

    [Fact]
    public void GetTotalExpense_ReturnsSumOfExpenseOnly()
    {
        var service = CreateInMemory();
        service.AddTransaction(Expense(100));
        service.AddTransaction(Expense(50));
        service.AddTransaction(Income(200));
        Assert.Equal(150, service.GetTotalExpense());
    }

    [Fact]
    public void GetBalance_ReturnsIncomeMinusExpense()
    {
        var service = CreateInMemory();
        service.AddTransaction(Income(300));
        service.AddTransaction(Expense(100));
        service.AddTransaction(Expense(50));
        Assert.Equal(150, service.GetBalance());
    }
}
