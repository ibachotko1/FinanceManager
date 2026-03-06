using Microsoft.VisualStudio.TestTools.UnitTesting;
using FinanceManager.Models;
using FinanceManager.Services;
using System;
using System.Transactions;

namespace FinanceManager.Tests
{
    [TestClass]
    public class DataServiceTests
    {
        private DataService CreateService() => new DataService(inMemory: true);

        /// <summary>
        /// Проверяет, что добавление дохода увеличивает баланс на сумму дохода.
        /// </summary>
        [TestMethod]
        public void AddIncome_ShouldIncreaseBalance()
        {
            var service = CreateService();
            var t = new Transaction
            {
                Amount = 5000,
                Type = TransactionType.Income,
                Category = "Зарплата",
                Date = DateTime.Today
            };
            service.AddTransaction(t);
            Assert.AreEqual(5000, service.GetBalance());
        }

        /// <summary>
        /// Проверяет, что добавление расхода уменьшает баланс на сумму расхода.
        /// </summary>
        [TestMethod]
        public void AddExpense_ShouldDecreaseBalance()
        {
            var service = CreateService();
            service.AddTransaction(new Transaction
            {
                Amount = 10000,
                Type = TransactionType.Income,
                Category = "Зарплата",
                Date = DateTime.Today
            });
            service.AddTransaction(new Transaction
            {
                Amount = 3000,
                Type = TransactionType.Expense,
                Category = "Еда",
                Date = DateTime.Today
            });
            Assert.AreEqual(7000, service.GetBalance());
        }

        /// <summary>
        /// Проверяет, что при попытке добавить транзакцию с отрицательной суммой выбрасывается ArgumentException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddTransaction_NegativeAmount_ShouldThrow()
        {
            var service = CreateService();
            service.AddTransaction(new Transaction
            {
                Amount = -100,
                Type = TransactionType.Expense,
                Category = "Еда",
                Date = DateTime.Today
            });
        }

        /// <summary>
        /// Проверяет, что при попытке добавить транзакцию с пустой категорией выбрасывается ArgumentException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddTransaction_EmptyCategory_ShouldThrow()
        {
            var service = CreateService();
            service.AddTransaction(new Transaction
            {
                Amount = 100,
                Type = TransactionType.Expense,
                Category = "",
                Date = DateTime.Today
            });
        }

        /// <summary>
        /// Проверяет, что после удаления транзакции она отсутствует в списке.
        /// </summary>
        [TestMethod]
        public void DeleteTransaction_ShouldRemoveIt()
        {
            var service = CreateService();
            var t = new Transaction
            {
                Amount = 500,
                Type = TransactionType.Expense,
                Category = "Еда",
                Date = DateTime.Today
            };
            service.AddTransaction(t);
            service.DeleteTransaction(t.Id);
            Assert.AreEqual(0, service.GetAll().Count);
        }

        /// <summary>
        /// Проверяет, что при отсутствии транзакций баланс равен нулю.
        /// </summary>
        [TestMethod]
        public void GetBalance_EmptyList_ShouldReturnZero()
        {
            var service = CreateService();
            Assert.AreEqual(0, service.GetBalance());
        }

        /// <summary>
        /// Проверяет, что при попытке добавить транзакцию с нулевой суммой выбрасывается ArgumentException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddTransaction_ZeroAmount_ShouldThrow()
        {
            var service = CreateService();
            service.AddTransaction(new Transaction
            {
                Amount = 0,
                Type = TransactionType.Expense,
                Category = "Еда",
                Date = DateTime.Today
            });
        }

        /// <summary>
        /// Проверяет, что добавление очень большой суммы (почти 10 миллионов) работает корректно.
        /// </summary>
        [TestMethod]
        public void AddTransaction_VeryLargeAmount_ShouldWork()
        {
            var service = CreateService();
            var largeAmount = 9999999.99m;
            service.AddTransaction(new Transaction
            {
                Amount = largeAmount,
                Type = TransactionType.Income,
                Category = "Лотерея",
                Date = DateTime.Today
            });
            Assert.AreEqual(largeAmount, service.GetBalance());
        }

        /// <summary>
        /// Проверяет, что метод GetTotalIncome возвращает сумму только доходных транзакций.
        /// </summary>
        [TestMethod]
        public void GetTotalIncome_ReturnsCorrectSum()
        {
            var service = CreateService();
            service.AddTransaction(new Transaction { Amount = 1000, Type = TransactionType.Income, Category = "A", Date = DateTime.Today });
            service.AddTransaction(new Transaction { Amount = 2000, Type = TransactionType.Income, Category = "B", Date = DateTime.Today });
            service.AddTransaction(new Transaction { Amount = 500, Type = TransactionType.Expense, Category = "C", Date = DateTime.Today });

            Assert.AreEqual(3000, service.GetTotalIncome());
        }

        /// <summary>
        /// Проверяет, что метод GetTotalExpense возвращает сумму только расходных транзакций.
        /// </summary>
        [TestMethod]
        public void GetTotalExpense_ReturnsCorrectSum()
        {
            var service = CreateService();
            service.AddTransaction(new Transaction { Amount = 1000, Type = TransactionType.Expense, Category = "A", Date = DateTime.Today });
            service.AddTransaction(new Transaction { Amount = 2000, Type = TransactionType.Expense, Category = "B", Date = DateTime.Today });
            service.AddTransaction(new Transaction { Amount = 500, Type = TransactionType.Income, Category = "C", Date = DateTime.Today });

            Assert.AreEqual(3000, service.GetTotalExpense());
        }

        /// <summary>
        /// Проверяет, что после нескольких операций баланс рассчитывается правильно (сумма доходов минус сумма расходов).
        /// </summary>
        [TestMethod]
        public void AddMultipleTransactions_BalanceCalculatedCorrectly()
        {
            var service = CreateService();
            service.AddTransaction(new Transaction { Amount = 10000, Type = TransactionType.Income, Category = "Зарплата", Date = DateTime.Today });
            service.AddTransaction(new Transaction { Amount = 2500, Type = TransactionType.Expense, Category = "Аренда", Date = DateTime.Today });
            service.AddTransaction(new Transaction { Amount = 500, Type = TransactionType.Expense, Category = "Еда", Date = DateTime.Today });
            service.AddTransaction(new Transaction { Amount = 3000, Type = TransactionType.Income, Category = "Фриланс", Date = DateTime.Today });

            Assert.AreEqual(10000 + 3000 - 2500 - 500, service.GetBalance());
        }
    }
}