using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Reconcillations.Entity;
using Reconcillations.Reports;
using Reconcillations.Repository;
using Reconcillations.Services;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
namespace Reconcillations.Pages
{

    public class AgenciesModel : PageModel
    {
        ITransactionRepository _transactionRepository; IHostEnvironment _hostingEnvironment;

        public SelectList PeriodSelectList { get; set; }

        public SelectList peryear { get; set; }

        public XtraReport Report { get; set; }

        [BindProperty]
        public Summarys summarys { get; set; }

        [BindProperty]
        public Periodlist Periodlist { get; set; }

        [BindProperty]
        public PeriodYear PeriodYear { get; set; }

        public AgenciesModel(IHostEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        public void OnGet()
        {
            var per = (from e in _transactionRepository.GetPeriodlist() select e).ToList();
            PeriodSelectList = new SelectList(per, "PeriodMonth", "PeriodName");
            HttpContext.Session.Set("Periodlist", per);

            var yer = (from y in _transactionRepository.GetPeriodYear() select y).ToList();
            peryear = new SelectList(yer, "PeridYear", "PeridYear");
            HttpContext.Session.Set("PeriodYear", yer);
        }

        public void OnPost()
        {
            var _summary = summarys;

            //HttpContext.Session.SetString("StartDate", _summary.Startdate.ToString());

            //HttpContext.Session.SetString("PeriodMonth", _summary.periodMonth.ToString());

            //HttpContext.Session.SetString("PeriodYear", _summary.periodYear.ToString());

            //HttpContext.Session.SetString("EndDate", _summary.Enddate.ToString());


            HttpContext.Session.SetString("perYear", _summary.periodYear.ToString());

            HttpContext.Session.SetString("perMonth", _summary.periodMonth.ToString());

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
