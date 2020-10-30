using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Reconcillations.Entity;
using Reconcillations.Repository;
using Microsoft.Extensions.Hosting;
//using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
//using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;//;:IHostEnvironment;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;

namespace Reconcillations.Pages.ImpBank
{
    public class CreateModel : PageModel
    {
        ITransactionRepository _transactionRepository;

        private IHostingEnvironment _hostingEnvironment;

        public SelectList BankSelectList { get; set; }

        public bool _isBankstatementimport = false;

        [BindProperty]
        public Reconcillations.Entity.BankImport bankimport { get; set; }

        public CreateModel(IHostingEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        public void OnGet()
        {
        }

        public IActionResult OnGetCompareStatement(long reconileID)
        {
            //long lngid = 10003;

            //bc.StatusMessage = response.Tables[0].Rows[0]["returnMessage"].ToString()
            DataSet dt = _transactionRepository.CompareBankStatement(reconileID);
            //ViewData["BankOnly"] = dt.Tables[2];
            //ViewData["Closebal"] = dt.Tables[4].Rows[0]["closingBal"];

            var serializeBankDt = JsonConvert.SerializeObject(dt.Tables[2]);
            TempData["BankData"] = serializeBankDt;

            var serialzeCollDt = JsonConvert.SerializeObject(dt.Tables[3]);
            TempData["CollectionData"] = serialzeCollDt;

            var serialzeMatechedDt = JsonConvert.SerializeObject(dt.Tables[1]);
            TempData["MatchedData"] = serialzeMatechedDt;

            //if (dt.Tables[2] != null && dt.Tables[2].Rows.Count > 0)
            //{

            //}

            //if (dt.Tables[3] != null && dt.Tables[3].Rows.Count > 0)
            //{

            //}
            //Console.Write(TempData["BankData"]);
            //if (dt.Tables[1] != null && dt.Tables[1].Rows.Count > 0)
            //{

            //}
            //else
            //{
            //    TempData["MatchedData"] = dt.Tables[1];
            //}

            //Console.Write(TempData["CollectionData"]);



            //Console.Write(TempData["MatchedData"]);

            return new JsonResult(dt);

        }

        public IActionResult OnGetAccountBal(long accountid)
        {
            var gbh = _transactionRepository.GetAccountBalance(accountid);

            AccountBal data = new AccountBal
            {
                OpenBal = gbh.OpenBal,
                Startdate = gbh.Startdate
            };

            return new JsonResult(data);
        }

        public IActionResult OnGetSelectByCode(long reconileID)
        {
          var _reconcile = _transactionRepository.GetReconcileid(reconileID);

            Reconcileperiodid reconciles = new Reconcileperiodid
            {
                accountname = _reconcile.accountname,
                OpeningBal = _reconcile.OpeningBal,
                startdate = _reconcile.startdate,
                enddate = _reconcile.enddate,
                ClosingBal = _reconcile.ClosingBal,
                AccountID = _reconcile.AccountID
            };
            //GetReconcileid
            return new JsonResult(reconciles);
        }

        public IActionResult OnGetBankAll()
        {
            List<Postinglist> data = (from d in _transactionRepository.GetRecocileperiod()
                                      select d).ToList();
            return new JsonResult(data);
        }

        public ActionResult OnPostImport()
        {
            DataTable tbimport = new DataTable();

            DataTable dt = new DataTable();

            var emails = HttpContext.Session.GetString("UserEmail");

            IFormFile file = Request.Form.Files[0];
            string folderName = "Upload";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            StringBuilder sb = new StringBuilder();

            //declaring datatable

            tbimport.Clear();

            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (file.Length > 0)
            {
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                ISheet sheet;
                string fullPath = Path.Combine(newPath, file.FileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;

                    if (sFileExtension == ".xls")
                    {
                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        HSSFFormulaEvaluator formula = new HSSFFormulaEvaluator(hssfwb);
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
                        formula.EvaluateAll();
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        XSSFFormulaEvaluator formula = new XSSFFormulaEvaluator(hssfwb);
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
                        formula.EvaluateAll();
                    }
                    IRow headerRow = sheet.GetRow(0); //Get Header Row
                    int cellCount = headerRow.LastCellNum;
                    sb.Append("<table class='table'><tr>");
                    for (int j = 0; j < cellCount; j++)
                    {
                        NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
                        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                        DataColumn column = new DataColumn(cell.ToString(), typeof(String));
                        tbimport.Columns.Add(column);
                        sb.Append("<th>" + cell.ToString() + "</th>");
                    }
                    sb.Append("</tr>");
                    sb.AppendLine("<tr>");

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = sheet.GetRow(i);

                        DataRow dataRow = tbimport.NewRow();

                        if (row == null) continue;

                        //if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            var cell = row.GetCell(j);

                            if (row.GetCell(j) == null)
                            {
                                sb.Append("<td>" + DBNull.Value + "</td>");
                                dataRow[j] = DBNull.Value;
                            }
                            else
                            {
                                // fe.EvaluateInCell(row.GetCell(j));
                                var gh = row.GetCell(j).ToString();

                                //formula.EvaluateInCell(row.GetCell(j));

                                //NumberEval ne = (NumberEval)row.GetCell(j);
                                //var g = NPOI.SS.UserModel.CellValue(ne.NumberValue);
                                //var bh = null; var ght;
                                //DateTime bh;
                                //DateTime bh = DateTime.Today;
                                switch (row.GetCell(j).CellType)
                                {

                                    case NPOI.SS.UserModel.CellType.Numeric:
                                        //DateFormat df = new SimpleDateFormat("dd/MM/yyyy");

                                        //if (DateUtil.IsCellDateFormatted(row.GetCell(j)))
                                        //{

                                        //DateTime date = row.GetCell(j).DateCellValue();
                                        ////cellWrapper.setValue(date);
                                        //DateTime bh = Convert.ToDateTime(row.GetCell(j).DateCellValue.ToString());
                                        ////row.GetCell(j).DateCellValue.ToString("dd/MM/yyyy");
                                        //}
                                        //else
                                        //{
                                        //    //var ght  cellWrapper.setValue(row.GetCell(j).NumericCellValue());
                                        //    //string.Format("{0:N2}", row.GetCell(j).NumericCellValue);
                                        //    double ght = Convert.ToDouble(row.GetCell(j).NumericCellValue);
                                        //}

                                        if (DateUtil.IsCellDateFormatted(cell))
                                        {
                                            DateTime date = cell.DateCellValue;
                                            ICellStyle style = cell.CellStyle;
                                            // Excel uses lowercase m for month whereas .Net uses uppercase
                                            string format = style.GetDataFormatString().Replace('m', 'M');
                                            //return date.ToString(format);

                                            //sb.Append("<td>" + date.ToString(format) + "</td>");
                                            //dataRow[j] = date.ToString(format);
                                            sb.Append("<td>" + date + "</td>");
                                            dataRow[j] = date;
                                        }
                                        else
                                        {
                                            //return cell.NumericCellValue.ToString();
                                            sb.Append("<td>" + string.Format("{0:N2}", row.GetCell(j).NumericCellValue) + "</td>");
                                            dataRow[j] = string.Format("{0:N2}", row.GetCell(j).NumericCellValue);
                                        }


                                        //var bh = DateUtil.IsCellDateFormatted(row.GetCell(j)) ?
                                        //    row.GetCell(j).DateCellValue.ToString("dd/MM/yyyy") : string.Format("{0:N2}", row.GetCell(j).NumericCellValue);
                                        //row.GetCell(j).NumericCellValue.ToString(CultureInfo.InvariantCulture);
                                        //var ght = string.Format("{0:N2}", row.GetCell(j).NumericCellValue);

                                        //                                            dr[j] = DateUtil.IsCellDateFormatted(gh)
                                        //? cell.DateCellValue.ToString(CultureInfo.InvariantCulture)
                                        //: cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);
                                        //if (HSSFDateUtil.isCellDateFormatted(row.GetCell(j)))
                                        //{

                                        //   // return df.format(cell.getDateCellValue());
                                        //sb.Append("<td>" + bh + "</td>");
                                        //dataRow[j] = bh;
                                        //}
                                        //else
                                        //{
                                        //    DecimalFormat df = new DecimalFormat("##.###############");
                                        //    //return df.format(cell.getNumericCellValue());
                                        //    sb.Append("<td>" + df.format(cell.getNumericCellValue()) + "</td>");
                                        //    dataRow[j] = df.format(cell.getNumericCellValue());
                                        //}
                                        //sb.Append("<td>" + (double)row.GetCell(j).NumericCellValue + "</td>");



                                        //dataRow[j] = row.GetCell(j).NumericCellValue;
                                        //var g = NPOI.SS.UserModel.CellValue;
                                        //string.Format("{0:N}", row.GetCell(j).NumericCellValue);
                                        break;
                                    case NPOI.SS.UserModel.CellType.String:
                                        sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                                        dataRow[j] = row.GetCell(j).ToString();
                                        break;
                                }

                            }

                        }
                        tbimport.Rows.Add(dataRow);
                        sb.AppendLine("</tr>");

                    }

                    //if (sFileExtension == ".xls")
                    //{
                    //    HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                    //                                                    //HSSFFormulaEvaluator.EvaluateAllFormulaCells(hssfwb);
                    //    HSSFFormulaEvaluator formula = new HSSFFormulaEvaluator(hssfwb);
                    //    sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
                    //    //HSSFFormulaEvaluator fe = new HSSFFormulaEvaluator(hssfwb);
                    //    //workbook.GetCreationHelper().CreateFormulaEvaluator().EvaluateFormulaCell(cell);
                    //    formula.EvaluateAll();

                    //    IRow headerRow = sheet.GetRow(0); //Get Header Row
                    //    int cellCount = headerRow.LastCellNum;
                    //    sb.Append("<table class='table'><tr>");
                    //    for (int j = 0; j < cellCount; j++)
                    //    {
                    //        NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
                    //        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                    //        DataColumn column = new DataColumn(cell.ToString(), typeof(String));
                    //        tbimport.Columns.Add(column);
                    //        sb.Append("<th>" + cell.ToString() + "</th>");
                    //    }
                    //    sb.Append("</tr>");
                    //    sb.AppendLine("<tr>");

                    //    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    //    {
                    //        IRow row = sheet.GetRow(i);

                    //        DataRow dataRow = tbimport.NewRow();

                    //        if (row == null) continue;

                    //        //if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                    //        for (int j = row.FirstCellNum; j < cellCount; j++)
                    //        {
                    //            var cell = row.GetCell(j);

                    //            if (row.GetCell(j) == null)
                    //            {
                    //                sb.Append("<td>" + DBNull.Value + "</td>");
                    //                dataRow[j] = DBNull.Value;
                    //            }
                    //            else
                    //            {
                    //                // fe.EvaluateInCell(row.GetCell(j));
                    //                var gh = row.GetCell(j).ToString();

                    //                formula.EvaluateInCell(row.GetCell(j));

                    //                //NumberEval ne = (NumberEval)row.GetCell(j);
                    //                //var g = NPOI.SS.UserModel.CellValue(ne.NumberValue);
                    //                //var bh = null; var ght;
                    //                //DateTime bh;
                    //                //DateTime bh = DateTime.Today;
                    //                switch (row.GetCell(j).CellType)
                    //                {

                    //                    case NPOI.SS.UserModel.CellType.Numeric:
                    //                        //DateFormat df = new SimpleDateFormat("dd/MM/yyyy");

                    //                        //if (DateUtil.IsCellDateFormatted(row.GetCell(j)))
                    //                        //{

                    //                        //DateTime date = row.GetCell(j).DateCellValue();
                    //                        ////cellWrapper.setValue(date);
                    //                        //DateTime bh = Convert.ToDateTime(row.GetCell(j).DateCellValue.ToString());
                    //                        ////row.GetCell(j).DateCellValue.ToString("dd/MM/yyyy");
                    //                        //}
                    //                        //else
                    //                        //{
                    //                        //    //var ght  cellWrapper.setValue(row.GetCell(j).NumericCellValue());
                    //                        //    //string.Format("{0:N2}", row.GetCell(j).NumericCellValue);
                    //                        //    double ght = Convert.ToDouble(row.GetCell(j).NumericCellValue);
                    //                        //}

                    //                        if (DateUtil.IsCellDateFormatted(cell))
                    //                        {
                    //                            DateTime date = cell.DateCellValue;
                    //                            ICellStyle style = cell.CellStyle;
                    //                            // Excel uses lowercase m for month whereas .Net uses uppercase
                    //                            string format = style.GetDataFormatString().Replace('m', 'M');
                    //                            //return date.ToString(format);

                    //                            sb.Append("<td>" + date.ToString(format) + "</td>");
                    //                            dataRow[j] = date.ToString(format);
                    //                        }
                    //                        else
                    //                        {
                    //                            //return cell.NumericCellValue.ToString();
                    //                            sb.Append("<td>" + string.Format("{0:N2}", row.GetCell(j).NumericCellValue) + "</td>");
                    //                            dataRow[j] = string.Format("{0:N2}", row.GetCell(j).NumericCellValue);
                    //                        }


                    //                        //var bh = DateUtil.IsCellDateFormatted(row.GetCell(j)) ?
                    //                        //    row.GetCell(j).DateCellValue.ToString("dd/MM/yyyy") : string.Format("{0:N2}", row.GetCell(j).NumericCellValue);
                    //                        //row.GetCell(j).NumericCellValue.ToString(CultureInfo.InvariantCulture);
                    //                        //var ght = string.Format("{0:N2}", row.GetCell(j).NumericCellValue);

                    //                        //                                            dr[j] = DateUtil.IsCellDateFormatted(gh)
                    //                        //? cell.DateCellValue.ToString(CultureInfo.InvariantCulture)
                    //                        //: cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);
                    //                        //if (HSSFDateUtil.isCellDateFormatted(row.GetCell(j)))
                    //                        //{

                    //                        //   // return df.format(cell.getDateCellValue());
                    //                        //sb.Append("<td>" + bh + "</td>");
                    //                        //dataRow[j] = bh;
                    //                        //}
                    //                        //else
                    //                        //{
                    //                        //    DecimalFormat df = new DecimalFormat("##.###############");
                    //                        //    //return df.format(cell.getNumericCellValue());
                    //                        //    sb.Append("<td>" + df.format(cell.getNumericCellValue()) + "</td>");
                    //                        //    dataRow[j] = df.format(cell.getNumericCellValue());
                    //                        //}
                    //                        //sb.Append("<td>" + (double)row.GetCell(j).NumericCellValue + "</td>");



                    //                        //dataRow[j] = row.GetCell(j).NumericCellValue;
                    //                        //var g = NPOI.SS.UserModel.CellValue;
                    //                        //string.Format("{0:N}", row.GetCell(j).NumericCellValue);
                    //                        break;
                    //                    case NPOI.SS.UserModel.CellType.String:
                    //                        sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                    //                        dataRow[j] = row.GetCell(j).ToString();
                    //                        break;
                    //                }

                    //            }

                    //        }
                    //        tbimport.Rows.Add(dataRow);
                    //        sb.AppendLine("</tr>");

                    //    }
                    //}
                    //else
                    //{
                    //    XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                    //                                                    //XSSFFormulaEvaluator.EvaluateAllFormulaCells(hssfwb);
                    //    sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   

                    //    XSSFFormulaEvaluator formula = new XSSFFormulaEvaluator(hssfwb);
                    //    formula.EvaluateAll();

                    //    //FormulaEvaluator evaluator = wb.getCreationHelper().createFormulaEvaluator();

                    //    IRow headerRow = sheet.GetRow(0); //Get Header Row
                    //    int cellCount = headerRow.LastCellNum;
                    //    sb.Append("<table class='table'><tr>");
                    //    for (int j = 0; j < cellCount; j++)
                    //    {
                    //        NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
                    //        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                    //        DataColumn column = new DataColumn(cell.ToString(), typeof(String));
                    //        tbimport.Columns.Add(column);
                    //        sb.Append("<th>" + cell.ToString() + "</th>");
                    //    }
                    //    sb.Append("</tr>");
                    //    sb.AppendLine("<tr>");

                    //    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    //    {
                    //        IRow row = sheet.GetRow(i);

                    //        DataRow dataRow = tbimport.NewRow();

                    //        if (row == null) continue;

                    //        //if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                    //        for (int j = row.FirstCellNum; j < cellCount; j++)
                    //        {
                    //            var cell = row.GetCell(j);

                    //            if (row.GetCell(j) == null)
                    //            {
                    //                sb.Append("<td>" + DBNull.Value + "</td>");
                    //                dataRow[j] = DBNull.Value;
                    //            }
                    //            else
                    //            {
                    //                // fe.EvaluateInCell(row.GetCell(j));
                    //                var gh = row.GetCell(j).ToString();

                    //                formula.EvaluateInCell(row.GetCell(j));

                    //                //NumberEval ne = (NumberEval)row.GetCell(j);
                    //                //var g = NPOI.SS.UserModel.CellValue(ne.NumberValue);

                    //                switch (row.GetCell(j).CellType)
                    //                {

                    //                    case NPOI.SS.UserModel.CellType.Numeric:
                    //                        //DateFormat df = new SimpleDateFormat("dd/MM/yyyy");

                    //                        //var bh = DateUtil.IsCellDateFormatted(row.GetCell(j)) ?
                    //                        //    row.GetCell(j).DateCellValue.ToString("dd/MM/yyyy") : string.Format("{0:N2}", row.GetCell(j).NumericCellValue);
                    //                        ////row.GetCell(j).NumericCellValue.ToString(CultureInfo.InvariantCulture);
                    //                        //var ght = string.Format("{0:N2}", row.GetCell(j).NumericCellValue);

                    //                        ////                                            dr[j] = DateUtil.IsCellDateFormatted(gh)
                    //                        ////? cell.DateCellValue.ToString(CultureInfo.InvariantCulture)
                    //                        ////: cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);
                    //                        ////if (HSSFDateUtil.isCellDateFormatted(row.GetCell(j)))
                    //                        ////{

                    //                        ////   // return df.format(cell.getDateCellValue());
                    //                        //sb.Append("<td>" + bh + "</td>");
                    //                        //dataRow[j] = bh;
                    //                        //}
                    //                        //else
                    //                        //{
                    //                        //    DecimalFormat df = new DecimalFormat("##.###############");
                    //                        //    //return df.format(cell.getNumericCellValue());
                    //                        //    sb.Append("<td>" + df.format(cell.getNumericCellValue()) + "</td>");
                    //                        //    dataRow[j] = df.format(cell.getNumericCellValue());
                    //                        //}
                    //                        //sb.Append("<td>" + (double)row.GetCell(j).NumericCellValue + "</td>");

                    //                        if (DateUtil.IsCellDateFormatted(cell))
                    //                        {
                    //                            DateTime date = cell.DateCellValue;
                    //                            //string.Format("", cell.DateCellValue);

                    //                            ICellStyle style = cell.CellStyle;
                    //                            // Excel uses lowercase m for month whereas .Net uses uppercase
                    //                            string format = "dd/MM/yyyy";//style.GetDataFormatString().Replace('m', 'M');
                    //                            string dvt = date.ToString(format);

                    //                            //if (cell.CellStyle.GetDataFormatString = 14)
                    //                            //{
                    //                            //    dateFmt = "dd/mm/yyyy";
                    //                            //}

                    //                            //System.out.println("dateFmt " + dateFmt);


                    //                            //return date.ToString(format);

                    //                            sb.Append("<td>" + date.ToString(format) + "</td>");
                    //                            dataRow[j] = date.ToString(format);
                    //                        }
                    //                        else
                    //                        {
                    //                            //return cell.NumericCellValue.ToString();
                    //                            sb.Append("<td>" + string.Format("{0:N2}", row.GetCell(j).NumericCellValue) + "</td>");
                    //                            dataRow[j] = string.Format("{0:N2}", row.GetCell(j).NumericCellValue);
                    //                        }

                    //                        //dataRow[j] = row.GetCell(j).NumericCellValue;
                    //                        //var g = NPOI.SS.UserModel.CellValue;
                    //                        //string.Format("{0:N}", row.GetCell(j).NumericCellValue);
                    //                        break;
                    //                    case NPOI.SS.UserModel.CellType.String:
                    //                        sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                    //                        dataRow[j] = row.GetCell(j).ToString();
                    //                        break;
                    //                }

                    //            }

                    //        }
                    //        tbimport.Rows.Add(dataRow);
                    //        sb.AppendLine("</tr>");

                    //    }


                    //}


                    sb.Append("</table>");
                    tbimport.AcceptChanges();

                    //TempData["MyexcelData"] = dt;
                }

                decimal dbcredit = 0m; decimal dbopening = 0m;
                decimal dbdebit = 0m;


                var gety = (from DataRow row in tbimport.Rows
                            select new ExcelEment
                            {
                                Date = !DBNull.Value.Equals(row["Date"]) ? Convert.ToDateTime(row["Date"]) : Convert.ToDateTime(row["Date"]),
                                Debit = !DBNull.Value.Equals(row["Debit"]) ? Convert.ToDecimal(row["Debit"]) : 0,
                                Credit = !DBNull.Value.Equals(row["Credit"]) ? Convert.ToDecimal(row["Credit"]) : 0,
                                Balance = !DBNull.Value.Equals(row["Balance"]) ? Convert.ToDecimal(row["Balance"]) : 0,
                                Tellerno = !DBNull.Value.Equals(row["Tellerno"]) ? row["Tellerno"].ToString() : null,
                                Revenuecode = !DBNull.Value.Equals(row["RevenueCode"]) ? row["RevenueCode"].ToString() : null,
                                Description = !DBNull.Value.Equals(row["PayerName"]) ? row["PayerName"].ToString() : null
                                //RecperID
                            }).ToList();



                dbopening = gety.AsEnumerable().FirstOrDefault().Balance;
                dbcredit = gety.AsEnumerable().Sum(x => x.Credit);
                dbdebit = gety.AsEnumerable().Sum(x => x.Debit);
                bankimport.CalClosingBal = dbcredit;

                /////
                dt = ConvertToDataTable(gety);

                sb.Append("+");
                //sb.AppendFormat("{0}", tbimport);
                //sb.Append("+");
                sb.AppendFormat("{0:n2}", dbopening);
                sb.Append("+");
                sb.AppendFormat("{0:n2}", dbcredit);
                sb.Append("+");
                sb.AppendFormat("{0:n2}", dbdebit);

                var serializeReponse = JsonConvert.SerializeObject(dt);
                sb.Append("+");
                sb.AppendFormat("{0}", serializeReponse);

                TempData["MyexcelData"] = serializeReponse;
            }


            return this.Content(sb.ToString());
        }
        //public ActionResult OnPostBankstatement([FromBody] BankImport objBankImport)
        public ActionResult OnPostBankstatement([FromBody] JObject objBankImport)
        {
            var _bankrecod = bankimport;

            List<BankImport> bn = new List<BankImport>();

            ResponseMessage<BankImport> hc = new ResponseMessage<BankImport>();

            if (!string.IsNullOrWhiteSpace(objBankImport.ToString()))
            {

                foreach (var item in objBankImport)
                {
                    Console.WriteLine(item.Key + " " + item.Value.ToString());

                    if (item.Key.ToString() == "accountID")
                    {
                        _bankrecod.AccountID = item.Value.ToString();
                    }
                    else if (item.Key.ToString() == "startDate")
                    {
                        _bankrecod.StartDate = DateTime.ParseExact(item.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    else if (item.Key.ToString() == "openingBal")
                    {
                        _bankrecod.OpeningBal = Convert.ToDecimal(item.Value.ToString());
                    }
                    else if (item.Key.ToString() == "dateEnd")
                    {
                        _bankrecod.DateEnd = DateTime.ParseExact(item.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    else if (item.Key.ToString() == "recperID")
                    {
                        _bankrecod.RecperID = Convert.ToInt64(item.Value.ToString());
                    }
                    else if (item.Key.ToString() == "closebal")
                    {
                        _bankrecod.ClosingBal = Convert.ToDecimal(item.Value.ToString());
                    }
                }


                if (TempData.ContainsKey("MyexcelData"))
                {
                    var hgb = TempData["MyexcelData"];
                    var hb = JsonConvert.DeserializeObject(hgb.ToString());

                    DataTable dtexcel = (DataTable)JsonConvert.DeserializeObject(hgb.ToString(), (typeof(DataTable)));

                    int gh = _transactionRepository.SaveBankmport(dtexcel, _bankrecod.RecperID);

                    if (gh == 1)
                    {
                        hc = _transactionRepository.Savebankstatement(_bankrecod, dtexcel);
                    }

                }

            }

            return new JsonResult(hc);
        }

        public ActionResult OnPostPostringRequest([FromBody] Posting posting)
        {
            DataSet dstresult = new DataSet();
            //posting.reconileID = 10003;
            dstresult = _transactionRepository.FinishedReconcile(posting.reconileID, HttpContext.Session.GetString("UserEmail"));
            return new JsonResult(dstresult);
        }

        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            var properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (T item in data)
            {
                var row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }

        public ActionResult OnPostSaveCompare([FromBody] ReconcileEntity reconcile)
        {
            DataTable dtbank = new DataTable(); DataTable dtCD = new DataTable(); DataTable dtMD = new DataTable();

            dtbank.Clear(); dtCD.Clear(); dtMD.Clear();

            //reconileID = 10003;
            // reconcile.reconileID = 10003;

            if (TempData.ContainsKey("BankData"))
            {
                var bn = TempData["BankData"];
                if (bn.Equals("[]"))
                {
                    dtbank.Columns.Add("DATE");
                    dtbank.Columns.Add("Amount");
                    dtbank.Columns.Add("bsid");
                }
                else
                {
                    dtbank = (DataTable)JsonConvert.DeserializeObject(bn.ToString(), (typeof(DataTable)));
                }

            }
            if (TempData.ContainsKey("CollectionData"))
            {
                var cd = TempData["CollectionData"];
                if (cd.Equals("[]"))
                {
                    dtCD.Columns.Add("CDATE");
                    dtCD.Columns.Add("CAmount");
                    dtCD.Columns.Add("paymentref");
                }
                else
                {
                    dtCD = (DataTable)JsonConvert.DeserializeObject(cd.ToString(), (typeof(DataTable)));
                }


            }
            if (TempData.ContainsKey("MatchedData"))
            {
                var md = TempData["MatchedData"];

                if (md.Equals("[]"))
                {
                    dtMD.Columns.Add("DATE");
                    dtMD.Columns.Add("pAmountDr");
                    dtMD.Columns.Add("pPaymentRef");
                    dtMD.Columns.Add("pBsid");
                }
                else
                {
                    dtMD = (DataTable)JsonConvert.DeserializeObject(md.ToString(), (typeof(DataTable)));
                }



            }

            DataSet dstcompare = _transactionRepository.SaveCompareStatement(reconcile.reconileID, reconcile.closebal, dtbank, dtCD, dtMD);
            return new JsonResult(dstcompare);
        }

        public ActionResult OnPostAllocateTransaction([FromBody] Allocate allocate)
        {
            //DataTable dtexcel = (DataTable)JsonConvert.DeserializeObject(hgb.ToString(), (typeof(DataTable)));
            DataSet dstresult = new DataSet();
            // allocate.reconileID = 10003;
            //DataTable dtdeb = JsonStringToDataTable(allocate.dtDebit.ToString());

            //DataTable dtcred = JsonStringToDataTable(allocate.dtCredit.ToString());

            //int gh = _transactionRepository.SaveAllocatetrans();

            //HttpContext.Session.SetString("Usernames", strusername);

            DataTable dtsdebit = allocate.dtDebit;
            DataTable dtscredit = allocate.dtCredit;

            dtsdebit.Columns.Add("IsCredit");
            dtsdebit.Columns.Add("AllocateBy");
            dtsdebit.Columns.Add("AllocateDate");

            dtscredit.Columns.Add("IsCredit");
            dtscredit.Columns.Add("AllocateBy");
            dtscredit.Columns.Add("AllocateDate");

            foreach (DataRow d in dtsdebit.Rows)
            {
                d["IsCredit"] = false;
                d["AllocateBy"] = HttpContext.Session.GetString("Usernames").ToString();
                d["AllocateDate"] = DateTime.Now;
            }

            foreach (DataRow d in dtscredit.Rows)
            {
                d["IsCredit"] = true;
                d["AllocateBy"] = HttpContext.Session.GetString("Usernames").ToString();
                d["AllocateDate"] = DateTime.Now;
            }

            dtsdebit.AcceptChanges();

            dtscredit.AcceptChanges();


            int val = _transactionRepository.SaveAllocatetrans(dtscredit);

            int vals = _transactionRepository.SaveAllocatetrans(dtsdebit);

            DataTable dtsval = allocate.dtDebit;

            dstresult = _transactionRepository.AllocateTransaction(allocate.reconileID, HttpContext.Session.GetString("UserEmail"));

            return new JsonResult(dstresult);
        }

        public ActionResult OnPostRequestSent(long reconileID)
        {
            ResponseInfo respone = _transactionRepository.RequestComplet(reconileID, HttpContext.Session.GetString("UserEmail"));
            return new JsonResult(respone);
        }
    }
}
