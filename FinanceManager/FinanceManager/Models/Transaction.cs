using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    // Модель данных, как она хранится/сериализуется (без UI-логики)
    public class Transaction
    {
        // Id генерим сразу, чтобы не думать об этом в UI
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public TransactionType Type { get; set; }
        public string Comment { get; set; } = "";
    }
}