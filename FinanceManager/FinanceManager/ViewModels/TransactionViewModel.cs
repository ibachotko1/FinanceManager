using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using FinanceManager.Helpers;
using FinanceManager.Models;
using FinanceManager.Services;

namespace FinanceManager.ViewModels
{
    public class TransactionViewModel : BaseViewModel
    {
        private readonly DataService _dataService;

        private string _amountText = "";
        public string AmountText
        {
            get => _amountText;
            set { _amountText = value; OnPropertyChanged(); }
        }

        private string _selectedCategory;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); }
        }

        private string _selectedType = "Расход";
        public string SelectedType
        {
            get => _selectedType;
            set
            {
                _selectedType = value;
                OnPropertyChanged();
                UpdateCategories();
            }
        }

        // Преобразование строки-типа в enum для сохранения
        private TransactionType CurrentTransactionType =>
            _selectedType == "Доход" ? TransactionType.Income : TransactionType.Expense;

        private DateTime _date = DateTime.Today;
        public DateTime Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(); }
        }

        private string _comment = "";
        public string Comment
        {
            get => _comment;
            set { _comment = value; OnPropertyChanged(); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> Categories { get; } =
            new ObservableCollection<string>();

        public ObservableCollection<string> TransactionTypes { get; } =
            new ObservableCollection<string> { "Расход", "Доход" };

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public bool IsSaved { get; private set; } = false;

        // Событие для закрытия окна без прямой ссылки на Window
        public event Action RequestClose;

        public TransactionViewModel(DataService dataService)
        {
            _dataService = dataService;
            SaveCommand = new RelayCommand(_ => Save(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => Cancel());
            UpdateCategories();
        }

        private void UpdateCategories()
        {
            Categories.Clear();
            if (CurrentTransactionType == TransactionType.Income)
            {
                Categories.Add("Зарплата"); Categories.Add("Подработка");
                Categories.Add("Инвестиции"); Categories.Add("Прочее");
            }
            else
            {
                Categories.Add("Еда"); Categories.Add("Транспорт");
                Categories.Add("Развлечения"); Categories.Add("ЖКХ");
                Categories.Add("Здоровье"); Categories.Add("Прочее");
            }
            SelectedCategory = Categories[0];
        }

        // Единственное место парсинга суммы — нет дублирования
        private bool TryGetAmount(out decimal amount) =>
            decimal.TryParse(_amountText, out amount) && amount > 0;

        private bool CanSave() =>
            TryGetAmount(out _) && !string.IsNullOrEmpty(SelectedCategory);

        private void Save()
        {
            ErrorMessage = "";
            if (!TryGetAmount(out decimal amount))
            {
                ErrorMessage = "Введите корректную сумму больше нуля";
                return;
            }
            try
            {
                _dataService.AddTransaction(new Transaction
                {
                    Amount = amount,
                    Date = Date,
                    Category = SelectedCategory,
                    Type = CurrentTransactionType,
                    Comment = Comment
                });
                IsSaved = true;
                RequestClose?.Invoke();
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private void Cancel() => RequestClose?.Invoke();
    }
}

