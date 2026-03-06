using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using FinanceManager.Models;

namespace FinanceManager.Services
{
    // Простой сервис хранения/чтения транзакций (json-файл рядом с exe)
    public class DataService
    {
        // Файл в рабочей папке приложения
        private readonly string _filePath = "transactions.json";
        private List<Transaction> _transactions;
        private readonly bool _inMemory;

        public DataService(bool inMemory = false)
        {
            // inMemory полезен для тестов/демо, чтобы не трогать файл
            _inMemory = inMemory;
            _transactions = inMemory ? new List<Transaction>() : LoadFromFile();
        }

        // Возвращаем копию, чтобы внешним кодом список случайно не ломали
        public List<Transaction> GetAll() => new List<Transaction>(_transactions);

        public void AddTransaction(Transaction t)
        {
            if (t.Amount <= 0)
                throw new ArgumentException("Сумма должна быть больше нуля");
            if (string.IsNullOrWhiteSpace(t.Category))
                throw new ArgumentException("Категория не может быть пустой");

            _transactions.Add(t);
            if (!_inMemory) SaveToFile();
        }

        public void DeleteTransaction(Guid id)
        {
            _transactions.RemoveAll(t => t.Id == id);
            if (!_inMemory) SaveToFile();
        }

        public decimal GetTotalIncome() =>
            _transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

        public decimal GetTotalExpense() =>
            _transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

        public decimal GetBalance() => GetTotalIncome() - GetTotalExpense();

        private List<Transaction> LoadFromFile()
        {
            // Если файла нет — начинаем с пустого списка
            if (!File.Exists(_filePath)) return new List<Transaction>();
            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Transaction>>(json)
                   ?? new List<Transaction>();
        }

        private void SaveToFile()
        {
            // Сохраняем с отступами, чтобы удобно было смотреть руками
            string json = JsonSerializer.Serialize(_transactions,
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}