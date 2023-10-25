using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System.Reflection;

namespace QA_checks.Models
{
    public class DefaultMethods
    {
        private readonly ApplicationDbContex _dbContext;
        public DefaultMethods(ApplicationDbContex dbContext)
        {
            _dbContext = dbContext;
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
                    if (returnedTuple.Item2 == "" || returnedTuple.Item2 == "string" || returnedTuple.Item2.Length < 10)
                    {
                        return false;
                    }
                    else { return true; }
                case 001:
                    return true;
                default:
                    return false;
            }
        }
        public string ReplaceChar(string a)
        {
            switch (a)
            {
                case "Ś":
                    return "S";
                case "ś":
                    return "s";
                case "ą":
                    return "a";
                case "Ł":
                    return "Ł";
                case "ł":
                    return "l";
                case "ć":
                    return "c";
                case "ę":
                    return "e";
                case "ó":
                    return "o";
            }
            return a;

        }
        public string ReturnValue(string value)
        {
            string newValue = "";
            for (var i = 0; i < value.Length; i++)
            {
                newValue += ReplaceChar(value[i].ToString());
            }
            return newValue;
        }
        public List<QAchecks> ReturnTests(long ordersNumber)
        {
            return _dbContext.QAchecks.Where(n => n.OrdersNumber == ordersNumber).ToList();
        }
        public void CreateGridCells(PdfGrid qAgrid,PdfTrueTypeFont font, PdfGridStyle gridStyle, PdfStringFormat stringFormat, List<QAchecks> qChecks)
        {
            for (var i = 0; i < qChecks.Count; i++)
            {

                PdfGridRow qaRow = qAgrid.Rows.Add();

                qaRow.Cells[0].Value = qChecks[i].Date;
                qaRow.Cells[0].StringFormat = stringFormat;

                if (qChecks[i].TestWodny == 2) {
                    qaRow.Cells[1].Value = "NOK";
                    qaRow.Cells[1].StringFormat = stringFormat;
                }
                else
                {
                    qaRow.Cells[1].Value = "OK";
                    qaRow.Cells[1].StringFormat = stringFormat;
                }

                qaRow.Cells[2].Value = qChecks[i].Lepkość.ToString()!;
                qaRow.Cells[2].StringFormat = stringFormat;

                qaRow.Cells[3].Value = qChecks[i].Ekstrakt.ToString()!;
                qaRow.Cells[3].StringFormat = stringFormat;

                qaRow.Cells[4].Value = qChecks[i].Ph.ToString();
                qaRow.Cells[4].StringFormat = stringFormat;

                qaRow.Cells[5].Value = qChecks[i].Temperatura.ToString();
                qaRow.Cells[5].StringFormat = stringFormat;

                if(qChecks[i].TestWodny.ToString() == "2")
                {
                    qaRow.Cells[6].Value = qChecks[i].TestKomentarz.ToString()!;
                    qaRow.Cells[6].StringFormat = stringFormat;
                }
                
                qAgrid.ApplyBuiltinStyle(PdfGridBuiltinStyle.GridTable4Accent5);
                qAgrid.Style = gridStyle;
            }
        }
        public void CreateGrid(PdfGrid qAgrid, int count)
        {
            qAgrid.Columns.Add(count);
            for(var i = 0; i < count; i++)
            {
                qAgrid.Columns[i].Width = 60;
                if (i == count)
                {
                    qAgrid.Columns[i].Width = 120;

                }
            }
        }
        public void CreateCell(PdfGridRow QAheader, PdfStringFormat stringFormat, List<string> tests)
        {
            for(var i = 0; i < tests.Count;i++) {
                QAheader.Cells[i].Value = tests[i];
                QAheader.Cells[i].StringFormat = stringFormat;
            }
        }
        public List<bool> ReturnBool(List<QAchecks> values)
        {
            List<bool> result = new List<bool>();
            var qAchecks = new List<List<int>>();

            foreach(var v in values)
            {
                Type objectType = v.GetType();
                PropertyInfo[] properties = objectType.GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    var list = new List<int>();
                    if (property.PropertyType == typeof(int))
                    {
                        var value = (int)property.GetValue(v);
                        list.Add(value);
                    }
                    qAchecks.Add(list);
                }
            }
            for(var i = 0;i < qAchecks.Count;i++)
            {
                for(var j = 0; j < qAchecks.Count;j++)
                {
                    if (qAchecks[i][j] == 2)
                    {
                        result.Add(true);
                    }
                }
            }
            return result;
        }
        public bool CheckForComments(List<int> values)
        {
            var check = values.Any(v => v == 2);
            if(check ) {
                return true;
            }
            return false;

        }

    }
}
