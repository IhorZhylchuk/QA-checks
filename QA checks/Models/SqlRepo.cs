using Microsoft.EntityFrameworkCore;
using QA_checks.Interfaces;

namespace QA_checks.Models
{
    public class SqlRepo : ISqlInterface
    {
        private readonly ApplicationDbContex _dbContext;
        public SqlRepo(ApplicationDbContex dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddQAcheckAsync(QAchecks qAchecks)
        {
            
            QAchecks qChecks = new QAchecks();
            qChecks.Pasteryzacja = qAchecks.Pasteryzacja;
            qChecks.PasteryzacjaKomentarz = qAchecks.PasteryzacjaKomentarz;
            qChecks.CiałaObce = qAchecks.CiałaObce;
            qChecks.CiałaObceKomentarz = qAchecks.CiałaObceKomentarz;
            qChecks.DataOpakowania = qAchecks.DataOpakowania;
            qChecks.DataOpakowaniaKomentarz = qAchecks.DataOpakowaniaKomentarz;
            qChecks.Ekstrakt = qAchecks.Ekstrakt;
            qChecks.Receptura = qAchecks.Receptura;
            qChecks.Temperatura = qAchecks.Temperatura;
            qChecks.RecepturaKomentarz = qAchecks.RecepturaKomentarz;
            qChecks.Lepkość = qAchecks.Lepkość;
            qChecks.MetalDetektor = qAchecks.MetalDetektor;
            qChecks.MetalDetektorKomentarz = qAchecks.MetalDetektorKomentarz;
            qChecks.Opakowanie = qAchecks.Opakowanie;
            qChecks.OpakowanieKomentarz = qAchecks.OpakowanieKomentarz;
            qChecks.OrdersNumber = qAchecks.OrdersNumber;
            qChecks.TestWodny = qAchecks.TestWodny;
            qChecks.TestKomentarz = qAchecks.TestKomentarz;
            qChecks.Ph = qAchecks.Ph;
            qChecks.OrderId = await _dbContext.Orders.Where(n => n.OrdersNumber == qAchecks.OrdersNumber).Select(i => i.Id).FirstAsync();
            qChecks.Date = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");


            await _dbContext.QAchecks.AddAsync(qChecks);
            await _dbContext.SaveChangesAsync();

        }

        public async Task CreateOrderAsync(Order order)
        {
            var newOrder = new Order();
            newOrder.OrdersName = order.OrdersName;
            newOrder.OrdersNumber = order.OrdersNumber;
            newOrder.Count = order.Count;

            await _dbContext.Orders.AddAsync(newOrder);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(Order order)
        {
             _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            var orders = await _dbContext.Orders.ToListAsync();
            return orders;
        }

        public async Task<Order> GetOrderByNumAsync(long ordersNumber)
        {
            var order = await _dbContext.Orders.Where(n => n.OrdersNumber == ordersNumber).FirstAsync();
            return  order;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();
        }
        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<QAchecks>> GetQAchecksAsync(long ordersNumber)
        {
            return await _dbContext.QAchecks.Where(n => n.OrdersNumber == ordersNumber).ToListAsync();

        }
    }
}
