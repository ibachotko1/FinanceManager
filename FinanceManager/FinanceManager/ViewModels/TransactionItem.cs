using System;
using FinanceManager.Models;

namespace FinanceManager.ViewModels
{
    public class TransactionItem
    {
        private readonly Transaction _transaction;

        public TransactionItem(Transaction transaction)
        {
            _transaction = transaction;
        }

        public Guid Id => _transaction.Id;
        public decimal Amount => _transaction.Amount;
        public DateTime Date => _transaction.Date;
        public string Category => _transaction.Category;
        public string Comment => _transaction.Comment;
        public TransactionType Type => _transaction.Type;

        public string TypeDisplay =>
            _transaction.Type == TransactionType.Income ? "Доход" : "Расход";
    }
}
