using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using FinanceManager.Helpers;
using FinanceManager.Services;

namespace FinanceManager.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly DataService _dataService;

        // Список транзакций — ObservableCollection автоматически
        // обновляет таблицу когда добавляем или удаляем элементы
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

        // Команды для кнопок — пока заглушки, откроют окна в следующих этапах
        public ICommand AddTransactionCommand { get; }
        public ICommand DeleteTransactionCommand { get; }
        public ICommand OpenReportsCommand { get; }

        public MainViewModel()
        {
            _dataService = new DataService();
            Transactions = new ObservableCollection<TransactionItem>();

            AddTransactionCommand = new RelayCommand(_ => OpenAddWindow());
            DeleteTransactionCommand = new RelayCommand(t => DeleteTransaction(t));
            OpenReportsCommand = new RelayCommand(_ => OpenReports());

            RefreshAll();
        }

        // Единственный метод обновления — список и суммы всегда синхронизированы
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
                _dataService.DeleteTransaction(item.Id);
                RefreshAll();
            }
        }

        // Заглушки — другие участники команды реализуют эти окна
        private void OpenAddWindow()
        {
            // TODO: открыть форму добавления транзакции
        }

        private void OpenReports()
        {
            // TODO: открыть окно отчётов
        }
    }
}
