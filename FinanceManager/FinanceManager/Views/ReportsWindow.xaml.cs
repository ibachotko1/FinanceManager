using System.Windows;
using FinanceManager.Services;
using FinanceManager.ViewModels;

namespace FinanceManager.Views
{
    public partial class ReportsWindow : Window
    {
        public ReportsWindow(DataService dataService)
        {
            InitializeComponent();
            DataContext = new ReportsViewModel(dataService);
        }
    }
}
