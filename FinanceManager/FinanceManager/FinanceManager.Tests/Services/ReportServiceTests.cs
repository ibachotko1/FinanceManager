using FinanceManager.Models;
using FinanceManager.Services;
using Xunit;

namespace FinanceManager.Tests.Services;

public class ReportServiceTests
{
    private static DataService CreateInMemory() => new DataService(inMemory: true);

    private static Transaction Income(decimal amount, DateTime date, string category = "Зарплата") =>
        new()
        {
            Amount = amount,
            Category = category,
            Type = TransactionType.Income,
            Date = date
        };

    private static Transaction Expense(decimal amount, DateTime date, string category = "Еда") =>
        new()
        {
            Amount = amount,
            Category = category,
            Type = TransactionType.Expense,
            Date = date
        };

    [Fact]
    public void GetExpensesByCategory_Empty_ReturnsEmpty()
    {
        var data = CreateInMemory();
        var report = new ReportService(data);
        var result = report.GetExpensesByCategory(DateTime.Today.AddDays(-7), DateTime.Today);
        Assert.Empty(result);
    }

    [Fact]
    public void GetExpensesByCategory_GroupsByCategory()
    {
        var data = CreateInMemory();
        var d = DateTime.Today;
        data.AddTransaction(Expense(100, d, "Еда"));
        data.AddTransaction(Expense(50, d, "Еда"));
        data.AddTransaction(Expense(200, d, "Транспорт"));

        var report = new ReportService(data);
        var result = report.GetExpensesByCategory(d, d);

        Assert.Equal(2, result.Count);
        Assert.Equal(150, result["Еда"]);
        Assert.Equal(200, result["Транспорт"]);
    }

    [Fact]
    public void GetExpensesByCategory_IgnoresIncome()
    {
        var data = CreateInMemory();
        var d = DateTime.Today;
        data.AddTransaction(Expense(100, d));
        data.AddTransaction(Income(500, d));

        var report = new ReportService(data);
        var result = report.GetExpensesByCategory(d, d);

        Assert.Single(result);
        Assert.Equal(100, result["Еда"]);
    }

    [Fact]
    public void GetExpensesByCategory_InclusiveBoundaries()
    {
        var data = CreateInMemory();
        var from = new DateTime(2024, 1, 1);
        var to = new DateTime(2024, 1, 10);
        data.AddTransaction(Expense(100, from));
        data.AddTransaction(Expense(50, to));
        data.AddTransaction(Expense(200, new DateTime(2024, 1, 15)));

        var report = new ReportService(data);
        var result = report.GetExpensesByCategory(from, to);

        Assert.Equal(150, result["Еда"]);
    }

    [Fact]
    public void GetFiltered_AllTypes_ReturnsAllInPeriod()
    {
        var data = CreateInMemory();
        var d = DateTime.Today;
        data.AddTransaction(Income(100, d));
        data.AddTransaction(Expense(50, d));

        var report = new ReportService(data);
        var result = report.GetFiltered(d, d);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetFiltered_TypeIncome_ReturnsOnlyIncome()
    {
        var data = CreateInMemory();
        var d = DateTime.Today;
        data.AddTransaction(Income(100, d));
        data.AddTransaction(Expense(50, d));

        var report = new ReportService(data);
        var result = report.GetFiltered(d, d, TransactionType.Income);

        Assert.Single(result);
        Assert.Equal(TransactionType.Income, result[0].Type);
    }

    [Fact]
    public void GetFiltered_TypeExpense_ReturnsOnlyExpense()
    {
        var data = CreateInMemory();
        var d = DateTime.Today;
        data.AddTransaction(Income(100, d));
        data.AddTransaction(Expense(50, d));

        var report = new ReportService(data);
        var result = report.GetFiltered(d, d, TransactionType.Expense);

        Assert.Single(result);
        Assert.Equal(TransactionType.Expense, result[0].Type);
    }

    [Fact]
    public void GetSummary_ReturnsIncomeAndExpense()
    {
        var data = CreateInMemory();
        var d = DateTime.Today;
        data.AddTransaction(Income(300, d));
        data.AddTransaction(Expense(100, d));

        var report = new ReportService(data);
        var (income, expense) = report.GetSummary(d, d);

        Assert.Equal(300, income);
        Assert.Equal(100, expense);
    }
}
