using System.Windows;
using FinanceManager.Services;
using FinanceManager.Views;

namespace FinanceManager
{
    public partial class MainWindow : Window
    {
        private readonly DataService _dataService = new DataService();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenReports_Click(object sender, RoutedEventArgs e)
        {
            var window = new ReportsWindow(_dataService);
            window.Show();
        }
    }
}