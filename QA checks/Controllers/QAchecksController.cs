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
using System.Reflection.Metadata;

namespace QA_checks.Controllers
{
    [ApiController]
    public class QAchecksController : ControllerBase
    {
        private readonly ISqlInterface _repo;
        private readonly IMapper _maper;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly DefaultMethods _defaultMethods;

        public QAchecksController(ISqlInterface repo, IMapper maper, IWebHostEnvironment hostingEnvironment, DefaultMethods defaultMethods)
        {
            _repo = repo;
            _maper = maper;
            _hostingEnvironment = hostingEnvironment;
            _defaultMethods = defaultMethods;   
        }
        [HttpPost("Create order")]
        public async Task<ActionResult<DtoOrderModel>> CreateOrderAsync(DtoOrderModel order)
        {
            if (ModelState.IsValid)
            {
                var check = _defaultMethods.Checking(order.OrdersNumber);
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
                    var pasteryzacja = _defaultMethods.ReturnTuple(qAChecks.Pasteryzacja, qAChecks.PasteryzacjaKomentarz);
                    var metalDetektor = _defaultMethods.ReturnTuple(qAChecks.MetalDetektor, qAChecks.MetalDetektorKomentarz);
                    var ciałaObce = _defaultMethods.ReturnTuple(qAChecks.CiałaObce, qAChecks.CiałaObceKomentarz);
                    var dataOpakowania = _defaultMethods.ReturnTuple(qAChecks.DataOpakowania, qAChecks.DataOpakowaniaKomentarz);
                    var receptura = _defaultMethods.ReturnTuple(qAChecks.Receptura, qAChecks.RecepturaKomentarz);
                    var opakowanie = _defaultMethods.ReturnTuple(qAChecks.Opakowanie, qAChecks.OpakowanieKomentarz);
                    var test = _defaultMethods.ReturnTuple(qAChecks.TestWodny, qAChecks.TestKomentarz);
                    returnedTuples.Add(pasteryzacja);
                    returnedTuples.Add(metalDetektor);
                    returnedTuples.Add(receptura);
                    returnedTuples.Add(ciałaObce);
                    returnedTuples.Add(opakowanie);
                    returnedTuples.Add(test);
                    returnedTuples.Add(dataOpakowania); 
                    returnedTuples.Add(receptura);

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
                    return BadRequest("Please, make sure you entered correct values or the necessary comments, where value is 002!");

                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest("Please, make sure you entered correct values or filled all necessary fields!");
        }
        [HttpGet ("Generate tests")]
        public ActionResult GeneretaPdfFile(long ordersNumber)
        {
            try
            {
                var relisedOrder = _repo.GetOrderByNumAsync(ordersNumber).Result;
                var qaCheks = _repo.GetQAchecksAsync(ordersNumber).Result.ToList();

                PdfDocument pdfDocument = new PdfDocument();
                pdfDocument.PageSettings.Orientation = PdfPageOrientation.Portrait;

                PdfPage currentPage = pdfDocument.Pages.Add();
                SizeF clientSize = currentPage.GetClientSize();
                FileStream imageStream = new FileStream("C:\\Users\\Igor\\source\\repos\\QA checks\\QA checks\\img\\e71683ae7d8746f3a35414b74dad1cd4.png", FileMode.Open, FileAccess.Read);
                PdfImage icon = new PdfBitmap(imageStream);
                SizeF iconSize = new SizeF(70, 70);
                PointF iconLocation = new PointF(14, 13);
                PdfGraphics graphics = currentPage.Graphics;
                graphics.DrawImage(icon, iconLocation, iconSize);

                var path = _hostingEnvironment.WebRootPath + "C:\\Users\\Igor\\source\\repos\\QA checks\\QA checks\\Font\\Helvetica.ttf";
                Stream fontStream = new FileStream(path, FileMode.Open, FileAccess.Read);

                PdfTrueTypeFont font = new PdfTrueTypeFont(fontStream, 12, PdfFontStyle.Regular);
                var text = new PdfTextElement("Data realizacji: " + DateTime.Now.ToString("dd/MM/yyyy"), font, new PdfSolidBrush(Color.Black));
                text.StringFormat = new PdfStringFormat(PdfTextAlignment.Right);
                PdfLayoutResult result = text.Draw(currentPage, new PointF(clientSize.Width - 25, iconLocation.Y + 100));
                PdfTrueTypeFont fontBold = new PdfTrueTypeFont(fontStream, 13, PdfFontStyle.Bold);
                text = new PdfTextElement("Quality Control Report", fontBold);
                text.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                result = text.Draw(currentPage, new PointF(clientSize.Width - 250, result.Bounds.Y + 60));

                List<string> zlecenieInfo = new List<string>() { "Numer zlecenia", "Nazwa produktu", "Wyprodukowana " + _defaultMethods.ReturnValue("iłość kg.") };
                List<string> values = new List<string>() { relisedOrder.OrdersNumber.ToString(), relisedOrder.OrdersName, relisedOrder.Count.ToString() };

                PdfGrid gridForOrder = new PdfGrid();
                gridForOrder.Style.Font = font;
                gridForOrder.Columns.Add(3);
                gridForOrder.Columns[0].Width = 160;
                gridForOrder.Columns[1].Width = 160;
                gridForOrder.Columns[2].Width = 160;

                gridForOrder.Headers.Add(1);
                PdfStringFormat stringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                PdfGridRowStyle style = new PdfGridRowStyle();
                PdfGridRow ordersHeader = gridForOrder.Headers[0];

                ordersHeader.Cells[0].Value = zlecenieInfo[0];
                ordersHeader.Cells[0].StringFormat = stringFormat;
                ordersHeader.Cells[1].Value = zlecenieInfo[1];
                ordersHeader.Cells[1].StringFormat = stringFormat;
                ordersHeader.Cells[2].Value = zlecenieInfo[2];
                ordersHeader.Cells[2].StringFormat = stringFormat;

                ordersHeader.Style = style;

                PdfGridRow ordersRow = gridForOrder.Rows.Add();
                ordersRow.Cells[0].Value = values[0];
                ordersRow.Cells[0].StringFormat = stringFormat;

                ordersRow.Cells[1].Value = values[1];
                ordersRow.Cells[1].StringFormat = stringFormat;

                ordersRow.Cells[2].Value = values[2];
                ordersRow.Cells[2].StringFormat = stringFormat;

                gridForOrder.ApplyBuiltinStyle(PdfGridBuiltinStyle.GridTable4Accent5);
                PdfGridStyle gridStyle = new PdfGridStyle();
                gridStyle.CellPadding = new PdfPaddings(5, 5, 10, 10);
                gridForOrder.Style = gridStyle;

                PdfGridLayoutFormat layoutFormat = new PdfGridLayoutFormat();
                layoutFormat.Layout = PdfLayoutType.Paginate;
                result = gridForOrder.Draw(currentPage, 14, result.Bounds.Bottom + 20, clientSize.Width - 35, layoutFormat);

                List<string> qaTestNames = new List<string>() {"Data/Godzina", "Test wodny", "Lepkosc", "Ekstrakt", "Ph", "t °C", "Komentarze"};
                var pasteryzacja = _defaultMethods.ReturnValue("Pasteryzacja przeprowadzona według receptury?");
                var ciałaObce = _defaultMethods.ReturnValue("Czy znaleziono w surowcach lub w gotowym produkcie ciała obce? Czy był poinformowany przełozony/dział jakości? ");
                var dataOpakowania = "Data umieszczona na opakowaniu odpowiada karcie produktu?";
                var zgodnośćZRecepturą = _defaultMethods.ReturnValue("Gotowy produkt jest zgony z recepturą?");
                var metaldetektor = _defaultMethods.ReturnValue(" Czy był sprawdzony metaldetektor? Czy w ciągu zmiany wystepowały jakieś problemy z metaldetektorem?");
                var opakowanie = "Opakowanie zgodne ze standardem?";

                List<string> qa = new List<string> {pasteryzacja, ciałaObce, dataOpakowania,
                zgodnośćZRecepturą, metaldetektor, opakowanie};

                var ordersChecks = _repo.GetQAchecksAsync(ordersNumber).Result.ToList();
                var tests = _defaultMethods.ReturnTests(ordersNumber);

                text = new PdfTextElement("Przebieg procesu produkcyjnego", fontBold);
                text.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                result = text.Draw(currentPage, new PointF(clientSize.Width - 250, result.Bounds.Y + 80));

                PdfGrid gridForProcess = new PdfGrid();
                gridForProcess.Style.Font = font;
                gridForProcess.Columns.Add(3);
                gridForProcess.Columns[0].Width = 240;
                gridForProcess.Columns[1].Width = 40;
                gridForProcess.Columns[2].Width = 200;

                gridForProcess.Headers.Add(1);
                stringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                PdfGridRow qAheader = gridForProcess.Headers[0];

                qAheader.Cells[0].Value = "";
                qAheader.Cells[0].StringFormat = stringFormat;
                qAheader.Cells[1].Value = "";
                qAheader.Cells[1].StringFormat = stringFormat;
                qAheader.Cells[2].Value = "";
                qAheader.Cells[2].StringFormat = stringFormat;

                qAheader.Style = style;

                var repo = _repo.GetQAchecksAsync(ordersNumber).Result.ToList();
                List<bool> checks = new List<bool>()
                {
                    _defaultMethods.CheckForComments(repo.Select(v => v.Pasteryzacja).ToList()),
                    _defaultMethods.CheckForComments(repo.Select(v => v.CiałaObce).ToList()),
                    _defaultMethods.CheckForComments(repo.Select(v => v.DataOpakowania).ToList()),
                    _defaultMethods.CheckForComments(repo.Select(v => v.Receptura).ToList()),
                    _defaultMethods.CheckForComments(repo.Select(v => v.MetalDetektor).ToList()),
                    _defaultMethods.CheckForComments(repo.Select(v => v.Opakowanie).ToList())
                };
                List<List<string>> comments = new List<List<string>>()
                {
                    repo.Select(v => v.PasteryzacjaKomentarz).ToList(),
                    repo.Select(v => v.CiałaObceKomentarz).ToList(),
                    repo.Select(v => v.DataOpakowaniaKomentarz).ToList(),
                    repo.Select(v => v.RecepturaKomentarz).ToList(),
                    repo.Select(v => v.MetalDetektorKomentarz).ToList(),
                    repo.Select(v => v.OpakowanieKomentarz).ToList(),
                };
                
                for (var i = 0; i < qa.Count; i++)
                {

                    PdfGridRow qaRow = gridForProcess.Rows.Add();
                    qaRow.Cells[0].Value = qa[i];
                    qaRow.Cells[0].StringFormat = stringFormat;

                    if (checks[i] == true)
                    {
                        qaRow.Cells[1].Value = "NOK";
                        qaRow.Cells[1].StringFormat = stringFormat;

                        qaRow.Cells[2].Value = string.Join("; ", comments[i]);
                        qaRow.Cells[2].StringFormat = stringFormat;
                    }
                    else
                    { 
                        qaRow.Cells[1].Value = "OK";
                        qaRow.Cells[1].StringFormat = stringFormat;

                        qaRow.Cells[2].Value = "-";
                        qaRow.Cells[2].StringFormat = stringFormat;
                    }
                    gridForProcess.ApplyBuiltinStyle(PdfGridBuiltinStyle.GridTable4Accent5);
                    gridForProcess.Style = gridStyle;

                } 
                layoutFormat.Layout = PdfLayoutType.Paginate;
                var newCordY = result.Bounds.Y + result.Bounds.Height;
                result = gridForProcess.Draw(currentPage, 14, newCordY + 20, clientSize.Width - 35, layoutFormat);

                text = new PdfTextElement("QA tests", fontBold);
                text.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                float textHeight = result.Bounds.Height;
                float newY = result.Bounds.Y + textHeight;
                result = text.Draw(currentPage, new PointF(clientSize.Width - 250, newY + 20));

                PdfGrid qAgrid = new PdfGrid();
                qAgrid.Style.Font = font;
                qAgrid.Columns.Add();
                _defaultMethods.CreateGrid(qAgrid, qaTestNames.Count - 1);

                qAgrid.Headers.Add(1);
                stringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                PdfGridRow QAheader = qAgrid.Headers[0];
                _defaultMethods.CreateCell(QAheader, stringFormat, qaTestNames);
                QAheader.Style = style;
                _defaultMethods.CreateGridCells(qAgrid,font, gridStyle, stringFormat, ordersChecks);
                layoutFormat.Layout = PdfLayoutType.Paginate;

                result = qAgrid.Draw(currentPage, 14, newY + 50, clientSize.Width - 35, layoutFormat);

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
                return BadRequest("Some error occurred - " + e.Message.ToString());
            }
            
        }
    }
}
