using FinanceManager.Services;
using FinanceManager.ViewModels;
using Xunit;

namespace FinanceManager.Tests.ViewModels;

public class TransactionViewModelTests
{
    private static DataService CreateDataService() => new DataService(inMemory: true);

    [Fact]
    public void SaveCommand_ValidData_AddsTransactionAndRaisesRequestClose()
    {
        var dataService = CreateDataService();
        var vm = new TransactionViewModel(dataService);
        vm.AmountText = "100"; // целое число — надёжно в любой культуре
        vm.SelectedCategory = "Еда";
        vm.SelectedType = "Расход";

        var closed = false;
        vm.RequestClose += () => closed = true;

        vm.SaveCommand.Execute(null);

        Assert.True(vm.IsSaved);
        Assert.True(closed);
        var all = dataService.GetAll();
        Assert.Single(all);
        Assert.Equal(100m, all[0].Amount);
        Assert.Equal("Еда", all[0].Category);
    }

    [Fact]
    public void SaveCommand_InvalidAmount_SetsErrorMessageAndDoesNotAdd()
    {
        var dataService = CreateDataService();
        var vm = new TransactionViewModel(dataService);
        vm.AmountText = "abc";
        vm.SelectedCategory = "Еда";

        vm.SaveCommand.Execute(null);

        Assert.False(vm.IsSaved);
        Assert.False(string.IsNullOrEmpty(vm.ErrorMessage));
        Assert.Contains("сумму", vm.ErrorMessage.ToLowerInvariant());
        Assert.Empty(dataService.GetAll());
    }

    [Fact]
    public void SaveCommand_ZeroAmount_SetsErrorMessage()
    {
        var dataService = CreateDataService();
        var vm = new TransactionViewModel(dataService);
        vm.AmountText = "0";
        vm.SelectedCategory = "Еда";

        vm.SaveCommand.Execute(null);

        Assert.False(vm.IsSaved);
        Assert.False(string.IsNullOrEmpty(vm.ErrorMessage));
    }

    [Fact]
    public void CancelCommand_RaisesRequestClose()
    {
        var vm = new TransactionViewModel(CreateDataService());
        var closed = false;
        vm.RequestClose += () => closed = true;

        vm.CancelCommand.Execute(null);

        Assert.True(closed);
    }

    [Fact]
    public void SelectedType_Expense_ShowsExpenseCategories()
    {
        var vm = new TransactionViewModel(CreateDataService());
        vm.SelectedType = "Расход";

        Assert.Contains("Еда", vm.Categories);
        Assert.Contains("Транспорт", vm.Categories);
        Assert.Contains("Развлечения", vm.Categories);
        Assert.DoesNotContain("Зарплата", vm.Categories);
    }

    [Fact]
    public void SelectedType_Income_ShowsIncomeCategories()
    {
        var vm = new TransactionViewModel(CreateDataService());
        vm.SelectedType = "Доход";

        Assert.Contains("Зарплата", vm.Categories);
        Assert.Contains("Подработка", vm.Categories);
        Assert.DoesNotContain("Еда", vm.Categories);
    }

    [Fact]
    public void SaveCommand_IncomeType_SavesAsIncome()
    {
        var dataService = CreateDataService();
        var vm = new TransactionViewModel(dataService);
        vm.AmountText = "1000";
        vm.SelectedType = "Доход";
        vm.SelectedCategory = "Зарплата";

        vm.SaveCommand.Execute(null);

        var all = dataService.GetAll();
        Assert.Single(all);
        Assert.Equal(FinanceManager.Models.TransactionType.Income, all[0].Type);
    }

    [Fact]
    public void SaveCommand_ExpenseType_SavesAsExpense()
    {
        var dataService = CreateDataService();
        var vm = new TransactionViewModel(dataService);
        vm.AmountText = "500";
        vm.SelectedType = "Расход";
        vm.SelectedCategory = "Транспорт";

        vm.SaveCommand.Execute(null);

        var all = dataService.GetAll();
        Assert.Single(all);
        Assert.Equal(FinanceManager.Models.TransactionType.Expense, all[0].Type);
    }

    [Fact]
    public void SaveCommand_CanSave_InvalidAmount_ReturnsFalse()
    {
        var vm = new TransactionViewModel(CreateDataService());
        vm.AmountText = "";
        vm.SelectedCategory = "Еда";
        Assert.False(vm.SaveCommand.CanExecute(null));

        vm.AmountText = "0";
        Assert.False(vm.SaveCommand.CanExecute(null));
    }

    [Fact]
    public void SaveCommand_CanSave_ValidData_ReturnsTrue()
    {
        var vm = new TransactionViewModel(CreateDataService());
        vm.AmountText = "100";
        vm.SelectedCategory = "Еда";
        Assert.True(vm.SaveCommand.CanExecute(null));
    }
}
