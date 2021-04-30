using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
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
    public class ReversedModel : PageModel
    {
        ITransactionRepository _transactionRepository; IHostingEnvironment _hostingEnvironment;

        public XtraReport Report { get; set; }

        [BindProperty]
        public Summarys summarys { get; set; }

        public string summarytype { get; set; }

        public string[] summarytypes = new[] { "Summary", "Reversals" };

        public ReversedModel(IHostingEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment;

            
        }


        public void OnGet()
        {
        }

        XtraReport createreport(DataTable dts)
        {

            XtraRepReversed report = new XtraRepReversed();
            report.DataSource = dts;
            report.DataMember = "vwreconcilesummary";
            report.CreateDocument();
            return report;
        }
       
        public async Task<IActionResult> OnPostLoading([FromBody] JObject objBankImport)
        {

            if (!string.IsNullOrWhiteSpace(objBankImport.ToString()))
            {
                foreach (var item in objBankImport)
                {
                    Console.WriteLine(item.Key + " " + item.Value.ToString());

                    if (item.Key.ToString() == "agencyname")
                    {
                        HttpContext.Session.SetString("AccountNumber", item.Value.ToString());
                    }
                }
            }

            return new JsonResult(true);
        }

        public void OnPost()
        {
            var _summary = summarys;

            HttpContext.Session.SetString("StartDate", _summary.Startdate.ToString());

            HttpContext.Session.SetString("EndDate", _summary.Enddate.ToString());

            Console.WriteLine(_summary);

            if (ModelState.IsValid)
            {
                var rdg = summarytype;

                DataTable dt = _transactionRepository.ViewSummary(_summary);

                Report = createreport(dt);

            }


        }
    }
}
