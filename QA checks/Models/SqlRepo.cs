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
            qChecks.OrderId = await _dbContext.Orders.Where(n => n.OrdersNumber == qAchecks.OrdersNumber).Select(i => i.Id).FirstAsync();
            
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

        public bool Checking(long ordersNumber)
        {
            var check = _dbContext.Orders.Select(n => n.OrdersNumber).Any(n => n == ordersNumber);
            return check;
        }
        public bool ReturnTuple(int value, string comment)
        {
            Tuple<int, string> returnedTuple = new Tuple<int, string>(value, comment);
            switch (returnedTuple.Item1)
            {
                case 002:
                    if(returnedTuple.Item2 == "" || returnedTuple.Item2 == "string" || returnedTuple.Item2.Length < 10){
                        return false; 
                    } 
                    else { return true; }
                case 001:
                    return true;
                default: 
                    return false;
            }
        }
    }
}
