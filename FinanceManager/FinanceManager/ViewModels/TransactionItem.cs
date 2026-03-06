using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceManager.Models;

namespace FinanceManager.ViewModels
{
    // Обёртка над Transaction для отображения в таблице.
    // TypeDisplay здесь, а не в модели — UI-логика не принадлежит данным.
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