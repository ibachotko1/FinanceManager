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
        // Вся математика по отчётам живёт в сервисе
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

        // Таблица справа/слева обновляется от этой коллекции
        public ObservableCollection<TransactionItem> FilteredTransactions { get; } =
            new ObservableCollection<TransactionItem>();

        // Данные для круговой диаграммы
        public ObservableCollection<CategorySummary> ExpensesByCategory { get; } =
            new ObservableCollection<CategorySummary>();

        public ICommand RefreshCommand { get; }
        public ICommand SetDayCommand { get; }
        public ICommand SetWeekCommand { get; }
        public ICommand SetMonthCommand { get; }
        public ICommand SetAllCommand { get; }

        public ReportsViewModel(DataService dataService)
        {
            // dataService приходит с главного окна, чтобы данные были общие
            _reportService = new ReportService(dataService);

            RefreshCommand = new RelayCommand(_ => Refresh());

            SetDayCommand = new RelayCommand(
                _ => SetPeriodAndRefresh(DateTime.Today, DateTime.Today)
            );

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
            // Каждый refresh пересобираем коллекции, так проще держать консистентность
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
