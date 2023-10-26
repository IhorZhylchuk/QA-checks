using QA_checks.Models;

namespace QA_checks.Interfaces
{
    public interface ISqlInterface
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task CreateOrderAsync(Order order);
        Task DeleteOrderAsync(Order order);
        Task<Order> GetOrderByNumAsync(long ordersNumber);
        Task UpdateOrderAsync(Order order);
        Task AddQAcheckAsync(QAchecks qAcheks);
        Task<IEnumerable<QAchecks>> GetQAchecksAsync(long ordersNumber);
        Task SaveAsync();
    }
}
