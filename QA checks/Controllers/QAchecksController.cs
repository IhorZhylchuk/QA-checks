using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QA_checks.DtoModels;
using QA_checks.Interfaces;
using QA_checks.Models;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Pdf;
using Syncfusion.Drawing;
using Microsoft.AspNetCore.Mvc.Routing;

namespace QA_checks.Controllers
{
    [ApiController]
    public class QAchecksController : ControllerBase
    {
        private readonly ISqlInterface _repo;
        private readonly IMapper _maper;
        public QAchecksController(ISqlInterface repo, IMapper maper)
        {
            _repo = repo;
            _maper = maper;
        }
        [HttpPost("Create order")]
        public async Task<ActionResult<DtoOrderModel>> CreateOrderAsync(DtoOrderModel order)
        {
            if (ModelState.IsValid)
            {
                var check = _repo.Checking(order.OrdersNumber);
                if (check == false)
                {
                    try
                    {
                        var newOrder = _maper.Map<Order>(order);
                        await _repo.CreateOrderAsync(newOrder);

                        return Ok(order);
                    }
                    catch (Exception ex)
                    {
                        return NotFound("Some error occured - " + ex.Message.ToString());
                    }
                }
                else
                {
                    return NotFound("Make sure the orders number is correct!");
                }
            }
            return NotFound("Please, enter correct values!");
        }
        [HttpGet("List of orders")]
        public async Task<ActionResult<IEnumerable<DtoOrderModel>>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _repo.GetAllOrdersAsync();

                return Ok(_maper.Map<IEnumerable<DtoOrderModel>>(orders));
            }
            catch (Exception ex)
            {
                return NotFound("Some exception occured - " + ex.Message.ToString());
            }
        }
        [HttpGet("Get order")]
        public async Task<ActionResult<DtoOrderModel>> GetOrderByNumAsync(long ordersNumber)
        {
            if (ordersNumber != 0)
            {
                try
                {
                    var order = await _repo.GetOrderByNumAsync(ordersNumber);
                    var dtoOrder = _maper.Map<DtoOrderModel>(order);
                    return Ok(dtoOrder);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return NotFound("Please, make sure the order exist or the order's number is correct!");
        }
        [HttpPut("Update order")]
        public async Task<ActionResult<DtoOrderModel>> OrdersUpdateAsync(long ordersNumber, [FromBody] DtoUpdateOrderModel dtoOrder)
        {
            if (ModelState.IsValid && ordersNumber != 0)
            {
                try
                {
                    var order = await _repo.GetOrderByNumAsync(ordersNumber);
                    order.OrdersName = dtoOrder.OrdersName;
                    order.Count = dtoOrder.Count;
                    await _repo.UpdateOrderAsync(order);

                    var returnedOrder = _maper.Map<DtoOrderModel>(order);
                    return Ok(returnedOrder);

                } catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
            return BadRequest("Please, make sure you entered correct values!");
        }
        [HttpPatch("Partial order's update")]
        public async Task<ActionResult<DtoOrderModel>> OrdersPartialUpdateAsync(long ordersNumber, JsonPatchDocument<DtoOrderModel> json)
        {
            if (ordersNumber != 0)
            {
                try
                {
                    var order = await _repo.GetOrderByNumAsync(ordersNumber);
                    var orderTopatch = _maper.Map<DtoOrderModel>(order);
                    json.ApplyTo(orderTopatch, ModelState);

                    if (!TryValidateModel(orderTopatch))
                    {
                        return ValidationProblem(ModelState);
                    }
                    _maper.Map(orderTopatch, order);
                    await _repo.UpdateOrderAsync(order);

                    return Ok(_maper.Map<DtoOrderModel>(order));
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest("Please, make sure you entered correct values!");
        }

        [HttpDelete ("Delete order")]
        public async Task<ActionResult> DeleteOrderAsync(long ordersNumber)
        {
            if(ordersNumber != 0)
            {
                try
                {
                    var order = await _repo.GetOrderByNumAsync(ordersNumber);
                    await _repo.DeleteOrderAsync(order);

                    return Ok("The order "+ ordersNumber + " has been deleted from the database!");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return NotFound("Please, make sure you entered correct 'ordersNumber'");
        }

        [HttpPost("QA checks")]
        public async Task<ActionResult> CreateChecksAsync(DtoQaChecks qAChecks)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    List<bool> returnedTuples = new List<bool>();
                    var pasteryzacja = _repo.ReturnTuple(qAChecks.Pasteryzacja, qAChecks.PasteryzacjaKomentarz);
                    var metalDetektor = _repo.ReturnTuple(qAChecks.MetalDetektor, qAChecks.MetalDetektorKomentarz);
                    var ciałaObce = _repo.ReturnTuple(qAChecks.CiałaObce, qAChecks.CiałaObceKomentarz);
                    var dataOpakowania = _repo.ReturnTuple(qAChecks.DataOpakowania, qAChecks.DataOpakowaniaKomentarz);
                    var receptura = _repo.ReturnTuple(qAChecks.Receptura, qAChecks.RecepturaKomentarz);
                    var opakowanie = _repo.ReturnTuple(qAChecks.Opakowanie, qAChecks.OpakowanieKomentarz);
                    var test = _repo.ReturnTuple(qAChecks.TestWodny, qAChecks.TestKomentarz);
                    returnedTuples.Add(pasteryzacja);
                    returnedTuples.Add(metalDetektor);

                    var check = returnedTuples.Any(i => i == false);
                    var checkForOrder = _repo.GetAllOrdersAsync().Result.Select(n => n.OrdersNumber).Any(i => i == qAChecks.OrdersNumber);
                    if (check == false && checkForOrder == true)
                    {
                        try
                        {
                            await _repo.AddQAcheckAsync(_maper.Map<QAchecks>(qAChecks));
                            return Ok(qAChecks);
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.Message);
                        }
                    }
                    return BadRequest("Please, make sure you entered coorect values or the necessary comments, where value is 002!");

                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest("Please, make sure you entered correct values or filled all necessary fields!");
        }
        [HttpGet ("Generate tests")]
        public IActionResult GeneretaPdfFile(long ordersNumber)
        {
            try
            {

                PdfDocument pdfDocument = new PdfDocument();
                PdfPage currentPage = pdfDocument.Pages.Add();
                SizeF clientSize = currentPage.GetClientSize();
                MemoryStream stream = new MemoryStream();
                pdfDocument.Save(stream);
                pdfDocument.Close(true);
                stream.Position = 0;

                FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf");
                fileStreamResult.FileDownloadName = "Report.pdf";

                return fileStreamResult;
            }
            catch (Exception e)
            {
                return BadRequest("Some error occurred - " + e.Message);
            }
        }

    }
}
