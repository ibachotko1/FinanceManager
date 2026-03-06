namespace FinanceManager.Models
{
    // Небольшая DTO для группировки по категориям (для диаграммы/сводки)
    public class CategorySummary
    {
        public string Category { get; set; }
        public decimal Amount { get; set; }
    }
}
