using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Reconcillations.Entity;
using Reconcillations.Reports;
using Reconcillations.Repository;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Reconcillations.Pages
{
    public class AgencyDetailsModel : PageModel
    {
        ITransactionRepository _transactionRepository; IHostingEnvironment _hostingEnvironment;
        public XtraReport Report { get; set; }
        public Summarys summarys { get; set; }

        public AgencyDetailsModel(IHostingEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment;
        }
        public void OnGet()
        {
       

        }
        public async Task<IActionResult> OnPostLoading([FromBody] JObject objBankImport)
        {
            //HttpContext.Session.SetString("AgencyName", e.Brick.text());
            if (!string.IsNullOrWhiteSpace(objBankImport.ToString()))
            {
                foreach (var item in objBankImport)
                {
                    Console.WriteLine(item.Key + " " + item.Value.ToString());

                    if (item.Key.ToString() == "revenuename")
                    {
                        //_bankrecod.AccountID = item.Value.ToString();
                        HttpContext.Session.SetString("RevenueName", item.Value.ToString());
                    }
                }
            }
            // return RedirectToPage("./AgencyDetails");
            return new JsonResult(true);
        }

        public void OnPost()
        {
            var gt = HttpContext.Session.GetString("AgencyName");
            var gtstart = HttpContext.Session.GetString("StartDate");
            var gtend = HttpContext.Session.GetString("EndDate");

            DataTable dt = _transactionRepository.ViewAgencydetails(gt.ToString(), Convert.ToDateTime(gtstart), Convert.ToDateTime(gtend));

            Report = createreport(dt);

        }
        XtraReport createreport(DataTable dts)
        {

            XtraRepagencydetails report = new XtraRepagencydetails();
            report.DataSource = dts;
            report.DataMember = "vwRevenueCollection";
            report.CreateDocument();
            return report;
        }
    }
}
