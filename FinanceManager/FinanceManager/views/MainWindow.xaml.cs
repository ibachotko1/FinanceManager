using System.Windows;
using FinanceManager.ViewModels;

namespace FinanceManager.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
