using System;
using System.Collections.Generic;
using System.Linq;
using FinanceManager.Models;

namespace FinanceManager.Services
{
    // Подготовка данных для отчётов (фильтры, группировки, суммы)
    public class ReportService
    {
        private readonly DataService _dataService;

        public ReportService(DataService dataService)
        {
            _dataService = dataService;
        }

        public Dictionary<string, decimal> GetExpensesByCategory(DateTime from, DateTime to)
        {
            return _dataService
                .GetAll()
                .Where(
                    t =>
                        t.Type == TransactionType.Expense
                        && t.Date >= from
                        && t.Date <= to // границы включительно
                )
                .GroupBy(t => t.Category)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));
        }

        public List<Transaction> GetFiltered(DateTime from, DateTime to, TransactionType? type = null)
        {
            // По умолчанию возвращаем всё за период, а type — опционально
            var query = _dataService.GetAll().Where(t => t.Date >= from && t.Date <= to);

            if (type.HasValue)
            {
                query = query.Where(t => t.Type == type.Value);
            }

            return query.OrderByDescending(t => t.Date).ToList();
        }

        public (decimal TotalIncome, decimal TotalExpense) GetSummary(DateTime from, DateTime to)
        {
            var transactions = GetFiltered(from, to);

            decimal income = transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            decimal expense = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            return (income, expense);
        }
    }
}
