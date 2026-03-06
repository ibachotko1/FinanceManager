using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using FinanceManager.Helpers;
using FinanceManager.Models;
using FinanceManager.Services;

namespace FinanceManager.ViewModels
{
    public class ReportsViewModel : BaseViewModel
    {
        private readonly ReportService _reportService;

        private DateTime _dateFrom = DateTime.Today.AddMonths(-1);
        public DateTime DateFrom
        {
            get => _dateFrom;
            set { _dateFrom = value; OnPropertyChanged(); }
        }

        private DateTime _dateTo = DateTime.Today;
        public DateTime DateTo
        {
            get => _dateTo;
            set { _dateTo = value; OnPropertyChanged(); }
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

        private decimal _total;
        public decimal Total
        {
            get => _total;
            set { _total = value; OnPropertyChanged(); }
        }

        public ObservableCollection<TransactionItem> FilteredTransactions { get; } =
            new ObservableCollection<TransactionItem>();

        public ObservableCollection<CategorySummary> ExpensesByCategory { get; } =
            new ObservableCollection<CategorySummary>();

        public ICommand RefreshCommand { get; }
        public ICommand SetWeekCommand { get; }
        public ICommand SetMonthCommand { get; }
        public ICommand SetAllCommand { get; }

        public ReportsViewModel(DataService dataService)
        {
            _reportService = new ReportService(dataService);

            RefreshCommand = new RelayCommand(_ => Refresh());

            SetWeekCommand = new RelayCommand(
                _ => SetPeriodAndRefresh(DateTime.Today.AddDays(-7), DateTime.Today)
            );

            SetMonthCommand = new RelayCommand(
                _ => SetPeriodAndRefresh(DateTime.Today.AddMonths(-1), DateTime.Today)
            );

            SetAllCommand = new RelayCommand(
                _ => SetPeriodAndRefresh(new DateTime(2000, 1, 1), DateTime.Today)
            );

            Refresh();
        }

        private void SetPeriodAndRefresh(DateTime from, DateTime to)
        {
            DateFrom = from;
            DateTo = to;
            Refresh();
        }

        private void Refresh()
        {
            FilteredTransactions.Clear();
            ExpensesByCategory.Clear();

            var (income, expense) = _reportService.GetSummary(DateFrom, DateTo);
            TotalIncome = income;
            TotalExpense = expense;
            Total = income - expense;

            foreach (var t in _reportService.GetFiltered(DateFrom, DateTo))
            {
                FilteredTransactions.Add(new TransactionItem(t));
            }

            foreach (var kvp in _reportService.GetExpensesByCategory(DateFrom, DateTo))
            {
                ExpensesByCategory.Add(
                    new CategorySummary { Category = kvp.Key, Amount = kvp.Value }
                );
            }
        }
    }
}
