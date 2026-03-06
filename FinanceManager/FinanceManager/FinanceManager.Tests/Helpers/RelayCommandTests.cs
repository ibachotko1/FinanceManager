using FinanceManager.Helpers;
using Xunit;

namespace FinanceManager.Tests.Helpers;

public class RelayCommandTests
{
    [Fact]
    public void Execute_CallsAction()
    {
        var executed = false;
        var command = new RelayCommand(_ => executed = true);

        command.Execute(null);

        Assert.True(executed);
    }

    [Fact]
    public void Execute_PassesParameter()
    {
        object? captured = null;
        var command = new RelayCommand(p => captured = p);

        command.Execute("test");

        Assert.Equal("test", captured);
    }

    [Fact]
    public void CanExecute_WhenPredicateNull_ReturnsTrue()
    {
        var command = new RelayCommand(_ => { });

        Assert.True(command.CanExecute(null));
        Assert.True(command.CanExecute("anything"));
    }

    [Fact]
    public void CanExecute_WhenPredicateReturnsTrue_ReturnsTrue()
    {
        var command = new RelayCommand(_ => { }, _ => true);

        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public void CanExecute_WhenPredicateReturnsFalse_ReturnsFalse()
    {
        var command = new RelayCommand(_ => { }, _ => false);

        Assert.False(command.CanExecute(null));
    }

    [Fact]
    public void CanExecute_WithParameter_PassesToPredicate()
    {
        var command = new RelayCommand(_ => { }, p => (int?)p == 42);

        Assert.True(command.CanExecute(42));
        Assert.False(command.CanExecute(0));
    }

    [Fact]
    public void Execute_WhenCanExecuteFalse_StillExecutes()
    {
        var executed = false;
        var command = new RelayCommand(_ => executed = true, _ => false);

        command.Execute(null);

        Assert.True(executed);
    }
}
