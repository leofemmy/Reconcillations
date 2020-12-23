using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Reconcillations.Entity;
using Reconcillations.Reports;
using Reconcillations.Repository;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
namespace Reconcillations.Pages
{

    public class AgenciesModel : PageModel
    {
        ITransactionRepository _transactionRepository; IHostEnvironment _hostingEnvironment;
        public XtraReport Report { get; set; }

        [BindProperty]
        public Summarys summarys { get; set; }

        public AgenciesModel(IHostEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment;
        }
      
        public void OnGet()
        {
        }
      
        public void OnPost()
        {
            var _summary = summarys;

            HttpContext.Session.SetString("StartDate", _summary.Startdate.ToString());

            HttpContext.Session.SetString("EndDate", _summary.Enddate.ToString());

            Console.WriteLine(_summary);

            if (ModelState.IsValid)
            {
                DataTable dt = _transactionRepository.ViewAgencies(_summary);

                Report = createreport(dt);

            }


        }
       
        XtraReport createreport(DataTable dts)
        {

            XtraRepAgencies report = new XtraRepAgencies();
            report.DataSource = dts;
            report.DataMember = "vwAgencyCollection";
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
                        HttpContext.Session.SetString("AgencyName", item.Value.ToString());
                    }
                }
            }

            return new JsonResult(true);
        }

    }
}
