using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using FinanceManager.Helpers;
using FinanceManager.Services;
using FinanceManager.Views;

namespace FinanceManager.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        // Один источник данных для главного окна и отчётов
        private readonly DataService _dataService;

        // Список транзакций — ObservableCollection автоматически
        // обновляет таблицу, когда меняется состав коллекции
        public ObservableCollection<TransactionItem> Transactions { get; set; }

        private decimal _balance;
        public decimal Balance
        {
            get => _balance;
            set { _balance = value; OnPropertyChanged(); }
        }

        private decimal _totalIncome;
        public decimal TotalIncome
        {
            get => _totalIncome;
            set { _totalIncome = value; OnPropertyChanged(); }
        }

        private decimal _totalExpense;
        public decimal TotalExpense
        {
            get => _totalExpense;
            set { _totalExpense = value; OnPropertyChanged(); }
        }

        private TransactionItem _selectedTransaction;
        public TransactionItem SelectedTransaction
        {
            get => _selectedTransaction;
            set { _selectedTransaction = value; OnPropertyChanged(); }
        }

        // Команды с кнопок внизу окна
        public ICommand AddTransactionCommand { get; }
        public ICommand DeleteTransactionCommand { get; }
        public ICommand OpenReportsCommand { get; }

        public MainViewModel()
        {
            _dataService = new DataService();
            Transactions = new ObservableCollection<TransactionItem>();

            // Привязки команд, без кода-behind в окне
            AddTransactionCommand = new RelayCommand(_ => OpenAddWindow());
            DeleteTransactionCommand = new RelayCommand(
                param => DeleteTransaction(param),
                param => param != null);
            OpenReportsCommand = new RelayCommand(_ => OpenReports());

            RefreshAll();
        }

        // Обновляем и таблицу, и верхние итоги одним проходом
        private void RefreshAll()
        {
            Transactions.Clear();
            foreach (var t in _dataService.GetAll())
                Transactions.Add(new TransactionItem(t));

            TotalIncome = _dataService.GetTotalIncome();
            TotalExpense = _dataService.GetTotalExpense();
            Balance = _dataService.GetBalance();
        }

        private void DeleteTransaction(object param)
        {
            if (param is TransactionItem item)
            {
                // Удаление по Id, чтобы не зависеть от состояния UI
                _dataService.DeleteTransaction(item.Id);
                RefreshAll();
            }
        }

        private void OpenAddWindow()
        {
            var addWindow = new AddTransactionWindow(_dataService);
            addWindow.Owner = Application.Current.MainWindow;
            addWindow.ShowDialog();
            RefreshAll();
        }

        private void OpenReports()
        {
            // Открываем отчёты на тех же данных (один DataService)
            var reportsWindow = new ReportsWindow(_dataService);
            reportsWindow.ShowDialog();
        }
    }
}
