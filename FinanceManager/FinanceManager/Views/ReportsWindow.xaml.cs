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
            // В отчёты прокидываем тот же сервис, что и у главного окна
            DataContext = new ReportsViewModel(dataService);
        }
    }
}
