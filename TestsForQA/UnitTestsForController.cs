using AutoMapper;
using Azure;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using QA_checks.Controllers;
using QA_checks.DtoModels;
using QA_checks.Models;
using QA_checks.Profiles;
using System.Web.Http.ModelBinding;

namespace TestsForQA
{
    public class UnitTestsForController
    {
        public DbContextOptions<ApplicationDbContex> Options()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContex>()
                .UseInMemoryDatabase(databaseName: "QAchecksDB")
                .Options;
            return dbContextOptions;
        }
        public List<Order> ReturnOrders()
        {
            List<Order> orders = new List<Order>()
            {
                new Order { Id = 1, Count = 1000, OrdersName = "Sos krówka", OrdersNumber = 2999999 },
                new Order { Id = 2, Count = 1000, OrdersName = "Sos truskawka", OrdersNumber = 2777777 },
                new Order { Id = 3, Count = 1000, OrdersName = "Sos malina", OrdersNumber = 2888888 }
            };
            return orders;
        }

        [Fact]
        public async Task CreateOrderAsync_Return_OK()
        {
            using (var db = new ApplicationDbContex(Options()))
            {
                var countBefore = await db.Orders.CountAsync();

                SqlRepo _sqlRepo = new SqlRepo(db);
                DefaultMethods defaultMethods = new DefaultMethods(db);
                var mockMapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<DtoOrderModel, Order>();
                });
                var mapper = mockMapper.CreateMapper();

                IWebHostEnvironment hostingEnvironment = new Mock<IWebHostEnvironment>().Object;

                QAchecksController controller = new QAchecksController(_sqlRepo, mapper, hostingEnvironment, defaultMethods);
                DtoOrderModel dtoOrder = new DtoOrderModel() { Count = 300, OrdersName = "Sos", OrdersNumber = 2345678 };
                var order = mapper.Map<Order>(dtoOrder);
                var result = await controller.CreateOrderAsync(dtoOrder);

                var countAfter = await db.Orders.CountAsync();

                Assert.NotNull(result);
                Assert.IsAssignableFrom<ActionResult<DtoOrderModel>>(result);
                Assert.NotEqual(countBefore, countAfter);
                Assert.True(countAfter > countBefore);

                await db.Database.EnsureDeletedAsync();
            }
        }

        [Fact]
        public async Task CreateOrderAsync_Return_NotFound()
        {
            using (var db = new ApplicationDbContex(Options()))
            {

                SqlRepo _sqlRepo = new SqlRepo(db);
                DefaultMethods defaultMethods = new DefaultMethods(db);
                var mockMapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<DtoOrderModel, Order>();
                });
                var mapper = mockMapper.CreateMapper();

                IWebHostEnvironment hostingEnvironment = new Mock<IWebHostEnvironment>().Object;

                QAchecksController controller = new QAchecksController(_sqlRepo, mapper, hostingEnvironment, defaultMethods);
                DtoOrderModel dtoOrder = new DtoOrderModel() { Count = 300, OrdersName = "Sos", OrdersNumber = 2345678 };
                var order = mapper.Map<Order>(dtoOrder);
                await db.Orders.AddAsync(order);
                await db.SaveChangesAsync();
                var countBefore = await db.Orders.CountAsync();

                var result = await controller.CreateOrderAsync(dtoOrder);

                var countAfter = await db.Orders.CountAsync();

                Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
                Assert.Equal(countBefore, countAfter);
                
                await db.Database.EnsureDeletedAsync();
            }
        }

        [Fact]
        public async Task Return_All_Orders()
        {
            using (var db = new ApplicationDbContex(Options()))
            {
                SqlRepo _sqlRepo = new SqlRepo(db);
                DefaultMethods defaultMethods = new DefaultMethods(db);
                var mockMapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<DtoOrderModel, Order>();
                });
                var mapper = mockMapper.CreateMapper();
                IWebHostEnvironment hostingEnvironment = new Mock<IWebHostEnvironment>().Object;
                QAchecksController controller = new QAchecksController(_sqlRepo, mapper, hostingEnvironment, defaultMethods);
                var order = new Order { Id = 1, Count = 1000, OrdersName = "Sos krówka", OrdersNumber = 2999999 };

                await _sqlRepo.CreateOrderAsync(order);

                var result = await _sqlRepo.GetAllOrdersAsync();
                
                Assert.NotNull(result);
                Assert.IsAssignableFrom<IEnumerable<Order>>(result);
                Assert.True(result.ToList().Count() > 0);

                await db.Database.EnsureDeletedAsync();
            }
        }

        [Fact]
        public async Task GetOrderByNumAsync_Ok()
        {
            using (var db = new ApplicationDbContex(Options()))
            {

                SqlRepo _sqlRepo = new SqlRepo(db);
                DefaultMethods defaultMethods = new DefaultMethods(db);
                var mockMapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<DtoOrderModel, Order>();
                    cfg.CreateMap<Order, DtoOrderModel>();
                });
                var mapper = mockMapper.CreateMapper();

                long ordersNumber = 2999999;

                var order = new Order { Id = 1, Count = 1000, OrdersName = "Sos krówka", OrdersNumber = 2999999 };
                IWebHostEnvironment hostingEnvironment = new Mock<IWebHostEnvironment>().Object;

                QAchecksController controller = new QAchecksController(_sqlRepo, mapper, hostingEnvironment, defaultMethods);
                DtoOrderModel dtoOrder = new DtoOrderModel() { Count = 300, OrdersName = "Sos", OrdersNumber = ordersNumber };

                await _sqlRepo.CreateOrderAsync(order);
                var count = _sqlRepo.GetAllOrdersAsync().Result.ToList().Count;

                var orderFromDb = await controller.GetOrderByNumAsync(ordersNumber);

                Assert.True(count > 0);
                Assert.NotNull(orderFromDb);
                Assert.IsAssignableFrom<ActionResult<DtoOrderModel>>(orderFromDb);
                Assert.IsType<OkObjectResult>(orderFromDb.Result);

                await db.Database.EnsureDeletedAsync();
            }

        }

        [Fact]
        public async Task GetOrderByNumAsync_NotFound()
        {
            using (var db = new ApplicationDbContex(Options()))
            {

                SqlRepo _sqlRepo = new SqlRepo(db);
                DefaultMethods defaultMethods = new DefaultMethods(db);
                var mockMapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<DtoOrderModel, Order>();
                    cfg.CreateMap<Order, DtoOrderModel>();
                });
                var mapper = mockMapper.CreateMapper();

                long ordersNumber = 0;

                IWebHostEnvironment hostingEnvironment = new Mock<IWebHostEnvironment>().Object;
                QAchecksController controller = new QAchecksController(_sqlRepo, mapper, hostingEnvironment, defaultMethods);

                var orderFromDb = await controller.GetOrderByNumAsync(ordersNumber);

                Assert.IsType<NotFoundObjectResult>(orderFromDb.Result);

                await db.Database.EnsureDeletedAsync();
            }

        }

        [Fact]
        public async Task OrdersUpdateAsync_Ok()
        {
            using (var db = new ApplicationDbContex(Options()))
            {

                SqlRepo _sqlRepo = new SqlRepo(db);
                DefaultMethods defaultMethods = new DefaultMethods(db);
                var mockMapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<DtoOrderModel, Order>();
                    cfg.CreateMap<Order, DtoOrderModel>();
                });
                var mapper = mockMapper.CreateMapper();

                long ordersNumber = 2999999;

                var order = new Order { Id = 1, Count = 1000, OrdersName = "Sos krówka", OrdersNumber = 2999999 };
                IWebHostEnvironment hostingEnvironment = new Mock<IWebHostEnvironment>().Object;

                QAchecksController controller = new QAchecksController(_sqlRepo, mapper, hostingEnvironment, defaultMethods);
                DtoUpdateOrderModel dtoOrder = new DtoUpdateOrderModel() { Count = 300, OrdersName = "Czekolada"};

                await _sqlRepo.CreateOrderAsync(order);

                var orderFromDb = await controller.OrdersUpdateAsync(ordersNumber, dtoOrder);

                var result = await _sqlRepo.GetOrderByNumAsync(ordersNumber);

                Assert.NotNull(orderFromDb);
                Assert.IsAssignableFrom<ActionResult<DtoOrderModel>>(orderFromDb);
                Assert.IsType<OkObjectResult>(orderFromDb.Result);
                Assert.Equal(dtoOrder.OrdersName, result.OrdersName);
                Assert.True(result.Count == dtoOrder.Count);

                await db.Database.EnsureDeletedAsync();
            }

        }

        [Fact]
        public async Task OrdersUpdateAsync_BadRequest()
        {
            using (var db = new ApplicationDbContex(Options()))
            {

                SqlRepo _sqlRepo = new SqlRepo(db);
                DefaultMethods defaultMethods = new DefaultMethods(db);
                var mockMapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<DtoOrderModel, Order>();
                    cfg.CreateMap<Order, DtoOrderModel>();
                });
                var mapper = mockMapper.CreateMapper();

                int ordersNumber = 0;

                IWebHostEnvironment hostingEnvironment = new Mock<IWebHostEnvironment>().Object;

                QAchecksController controller = new QAchecksController(_sqlRepo, mapper, hostingEnvironment, defaultMethods);
                DtoUpdateOrderModel dtoOrder = new DtoUpdateOrderModel() { Count = 0, OrdersName = "" };

                var result = await controller.OrdersUpdateAsync(ordersNumber, dtoOrder);

                Assert.IsType<BadRequestObjectResult>(result.Result);

                await db.Database.EnsureDeletedAsync();
            }

        }

        [Fact] 
        public async Task DeleteOrderAsync()
        {
            using (var db = new ApplicationDbContex(Options()))
            {

                SqlRepo _sqlRepo = new SqlRepo(db);
                DefaultMethods defaultMethods = new DefaultMethods(db);
                var mockMapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<DtoOrderModel, Order>();
                    cfg.CreateMap<Order, DtoOrderModel>();
                });
                var mapper = mockMapper.CreateMapper();

                long ordersNumber = 2999999;

                var order = new Order { Id = 1, Count = 1000, OrdersName = "Sos krówka", OrdersNumber = 2999999 };
                IWebHostEnvironment hostingEnvironment = new Mock<IWebHostEnvironment>().Object;

                QAchecksController controller = new QAchecksController(_sqlRepo, mapper, hostingEnvironment, defaultMethods);

                await _sqlRepo.CreateOrderAsync(order);

                var countBefor = _sqlRepo.GetAllOrdersAsync().Result.Count();

                var orderDeleted = await controller.DeleteOrderAsync(ordersNumber);

                var countAfter = _sqlRepo.GetAllOrdersAsync().Result.Count();

                var tryToGetOrder = await controller.GetOrderByNumAsync(ordersNumber);

                Assert.IsType<OkObjectResult>(orderDeleted);
                Assert.NotEqual(countBefor, countAfter);
                Assert.IsType<BadRequestObjectResult>(tryToGetOrder.Result);

                await db.Database.EnsureDeletedAsync();
            }

        }

        [Fact] 
        public async Task CreateChecksAsync_Ok()
        {
            using (var db = new ApplicationDbContex(Options()))
            {

                SqlRepo _sqlRepo = new SqlRepo(db);
                DefaultMethods defaultMethods = new DefaultMethods(db);
                var mockMapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<QAchecks, DtoQaChecks>();
                    cfg.CreateMap<DtoQaChecks, QAchecks>();
                });
                var mapper = mockMapper.CreateMapper();

                var order = new Order { Id = 1, Count = 1000, OrdersName = "Sos krówka", OrdersNumber = 2999999 };
                await _sqlRepo.CreateOrderAsync(order);

                var check = new QAchecks()
                {
                    Id = 1,
                    CiałaObce = 1,
                    CiałaObceKomentarz = "",
                    DataOpakowania = 1,
                    DataOpakowaniaKomentarz = "",
                    Date = DateTime.Now.Date.ToString(),
                    Opakowanie = 1,
                    OpakowanieKomentarz = "",
                    Ekstrakt = 45,
                    Lepkość = 5002,
                    MetalDetektor = 1,
                    MetalDetektorKomentarz = "",
                    OrderId = 1,
                    OrdersNumber = 2999999,
                    Pasteryzacja = 1,
                    PasteryzacjaKomentarz = "",
                    Ph = 6,
                    Receptura = 1,
                    RecepturaKomentarz = "",
                    Temperatura = 55,
                    TestKomentarz = "",
                    TestWodny = 1
                };

                IWebHostEnvironment hostingEnvironment = new Mock<IWebHostEnvironment>().Object;

                QAchecksController controller = new QAchecksController(_sqlRepo, mapper, hostingEnvironment, defaultMethods);

                var result = await controller.CreateChecksAsync(mapper.Map<DtoQaChecks>(check));

                var returnedCheck = await _sqlRepo.GetQAchecksAsync(check.OrdersNumber);

                Assert.NotNull(result);
                Assert.IsAssignableFrom<ActionResult<DtoQaChecks>>(result);
                Assert.IsType<OkObjectResult>(result.Result);
                Assert.IsType<List<QAchecks>>(returnedCheck.ToList());
                Assert.True(returnedCheck.ToList().Count() > 0);

                await db.Database.EnsureDeletedAsync();
            }

        }

        [Fact]
        public async Task CreateChecksAsync_BadRequest()
        {
            using (var db = new ApplicationDbContex(Options()))
            {
                SqlRepo _sqlRepo = new SqlRepo(db);
                DefaultMethods defaultMethods = new DefaultMethods(db);
                var mockMapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<QAchecks, DtoQaChecks>();
                    cfg.CreateMap<DtoQaChecks, QAchecks>();
                });
                var mapper = mockMapper.CreateMapper();

                var check = new QAchecks()
                {
                    Id = 1,
                    CiałaObce = 1,
                    CiałaObceKomentarz = "",
                    DataOpakowania = 1,
                    DataOpakowaniaKomentarz = "",
                    Date = DateTime.Now.Date.ToString(),
                    Opakowanie = 1,
                    OpakowanieKomentarz = "",
                    Ekstrakt = 45,
                    Lepkość = 5002,
                    MetalDetektor = 1,
                    MetalDetektorKomentarz = "",
                    OrderId = 1,
                    OrdersNumber = 2999999,
                    Pasteryzacja = 1,
                    PasteryzacjaKomentarz = "",
                    Ph = 6,
                    Receptura = 1,
                    RecepturaKomentarz = "",
                    Temperatura = 55,
                    TestKomentarz = "",
                    TestWodny = 1
                };

                IWebHostEnvironment hostingEnvironment = new Mock<IWebHostEnvironment>().Object;

                QAchecksController controller = new QAchecksController(_sqlRepo, mapper, hostingEnvironment, defaultMethods);

                var result = await controller.CreateChecksAsync(mapper.Map<DtoQaChecks>(check));

                var returnedCheck = await _sqlRepo.GetQAchecksAsync(check.OrdersNumber);

                Assert.IsType<BadRequestObjectResult>(result.Result);
                Assert.True(returnedCheck.ToList().Count() == 0);

                await db.Database.EnsureDeletedAsync();
            }

        }
    }
}