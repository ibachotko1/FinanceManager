using System.Windows;
using FinanceManager.Services;
using FinanceManager.ViewModels;

namespace FinanceManager.Views
{
    public partial class AddTransactionWindow : Window
    {
        public AddTransactionWindow(DataService dataService)
        {
            InitializeComponent();
            var vm = new TransactionViewModel(dataService);
            vm.RequestClose += () => this.Close();
            DataContext = vm;
        }
    }
}
