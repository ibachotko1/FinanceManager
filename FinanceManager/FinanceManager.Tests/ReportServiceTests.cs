using Microsoft.VisualStudio.TestTools.UnitTesting;
using FinanceManager.Models;
using FinanceManager.Services;
using System;
using System.Linq;
using System.Transactions;

namespace FinanceManager.Tests
{
    [TestClass]
    public class ReportServiceTests
    {
        /// <summary>
        /// Проверяет, что метод GetExpensesByCategory правильно группирует расходы по категориям.
        /// </summary>
        [TestMethod]
        public void GetExpensesByCategory_ShouldGroupCorrectly()
        {
            var dataService = new DataService(inMemory: true);
            dataService.AddTransaction(new Transaction
            {
                Amount = 1000,
                Type = TransactionType.Expense,
                Category = "Еда",
                Date = DateTime.Today
            });
            dataService.AddTransaction(new Transaction
            {
                Amount = 500,
                Type = TransactionType.Expense,
                Category = "Еда",
                Date = DateTime.Today
            });
            dataService.AddTransaction(new Transaction
            {
                Amount = 800,
                Type = TransactionType.Expense,
                Category = "Транспорт",
                Date = DateTime.Today
            });

            var reportService = new ReportService(dataService);
            var result = reportService.GetExpensesByCategory(
                DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));

            Assert.AreEqual(1500, result["Еда"]);
            Assert.AreEqual(800, result["Транспорт"]);
        }

        /// <summary>
        /// Проверяет, что метод GetSummary возвращает корректные суммы доходов и расходов за указанный период.
        /// </summary>
        [TestMethod]
        public void GetSummary_ShouldReturnCorrectTotals()
        {
            var dataService = new DataService(inMemory: true);
            dataService.AddTransaction(new Transaction
            {
                Amount = 50000,
                Type = TransactionType.Income,
                Category = "Зарплата",
                Date = DateTime.Today
            });
            dataService.AddTransaction(new Transaction
            {
                Amount = 1200,
                Type = TransactionType.Expense,
                Category = "Еда",
                Date = DateTime.Today
            });

            var reportService = new ReportService(dataService);
            var (income, expense) = reportService.GetSummary(
                DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));

            Assert.AreEqual(50000, income);
            Assert.AreEqual(1200, expense);
        }

        /// <summary>
        /// Проверяет, что фильтрация по дате исключает транзакции вне указанного диапазона.
        /// </summary>
        [TestMethod]
        public void GetFiltered_ByDateRange_ShouldExcludeOutOfRange()
        {
            var dataService = new DataService(inMemory: true);
            dataService.AddTransaction(new Transaction
            {
                Amount = 1000,
                Type = TransactionType.Expense,
                Category = "Еда",
                Date = DateTime.Today
            });
            dataService.AddTransaction(new Transaction
            {
                Amount = 2000,
                Type = TransactionType.Expense,
                Category = "Транспорт",
                Date = DateTime.Today.AddDays(-30)
            });

            var reportService = new ReportService(dataService);
            var result = reportService.GetFiltered(
                DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Еда", result[0].Category);
        }

        /// <summary>
        /// Проверяет, что фильтрация по типу возвращает только транзакции указанного типа.
        /// </summary>
        [TestMethod]
        public void GetFiltered_ByType_ShouldReturnOnlyIncome()
        {
            var dataService = new DataService(inMemory: true);
            dataService.AddTransaction(new Transaction
            {
                Amount = 50000,
                Type = TransactionType.Income,
                Category = "Зарплата",
                Date = DateTime.Today
            });
            dataService.AddTransaction(new Transaction
            {
                Amount = 500,
                Type = TransactionType.Expense,
                Category = "Еда",
                Date = DateTime.Today
            });

            var reportService = new ReportService(dataService);
            var result = reportService.GetFiltered(
                DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1),
                TransactionType.Income);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(TransactionType.Income, result[0].Type);
        }
    }
}