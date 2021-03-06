using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reconcillations.Entity;
using Reconcillations.Reports;
using Reconcillations.Repository;
using Microsoft.Extensions.Hosting;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reconcillations.Services;

namespace Reconcillations.Pages
{
    public class SummaryModel : PageModel
    {

        public SelectList PeriodSelectList
        {
            get; set;
        }

        public SelectList peryear
        {
            get; set;
        }

        ITransactionRepository _transactionRepository; IHostingEnvironment _hostingEnvironment;


        public XtraReport Report
        {
            get; set;
        }

        [BindProperty]
        public Summarys summarys
        {
            get; set;
        }

        public string summarytype
        {
            get; set;
        }

        [BindProperty]
        public Periodlist Periodlist
        {
            get; set;
        }

        [BindProperty]
        public PeriodYear PeriodYear
        {
            get; set;
        }


        public string[] summarytypes = new[] { "Summary", "Reversals" };

        public SummaryModel(IHostingEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
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
       
        XtraReport createreport(DataTable dts)
        {

            XtraRepSummary report = new XtraRepSummary();
            report.DataSource = dts;
            report.DataMember = "vwreconcilesummary";
            report.CreateDocument();
            return report;
        }
        
        public void OnPost()
        {
            var _summary = summarys;

            Console.WriteLine(_summary);

            if (ModelState.IsValid)
            {
                var rdg = summarytype;

                DataTable dt = _transactionRepository.ViewSummary(_summary);

                Report = createreport(dt);

            }


        }

        XtraReport createreport2(DataTable dts)
        {
            XtraRepReversals report = new XtraRepReversals();
            report.DataSource = dts;
            report.DataMember = "vwreconcilesummary";
            report.CreateDocument();
            return report;

        }

        public ActionResult OnPostSummary([FromBody] JObject summary)
        {
            var summar = summarys;

            if (!string.IsNullOrWhiteSpace(summary.ToString()))
            {
                foreach (var item in summary)
                {
                    Console.WriteLine(item.Key + " " + item.Value.ToString());


                    if (item.Key.ToString() == "startdate")
                    {
                        summar.Startdate = DateTime.ParseExact(item.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    else if (item.Key.ToString() == "enddate")
                    {
                        summar.Enddate = DateTime.ParseExact(item.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                }

            }
            DataTable dt = _transactionRepository.ViewSummary(summar);


            return new JsonResult(Report = createreport(dt) );
        }
    }
}
