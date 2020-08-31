using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reconcillations.Entity;
using Reconcillations.Reports;
using Reconcillations.Repository;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
namespace Reconcillations.Pages
{
    public class VarianceAgencyModel : PageModel
    {
        ITransactionRepository _transactionRepository; IHostingEnvironment _hostingEnvironment;
        public XtraReport Report { get; set; }

        [BindProperty]
        public Summarys summarys { get; set; }

        public VarianceAgencyModel(IHostingEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
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
                DataTable dt = _transactionRepository.ViewVaricesAgencies(_summary);

                Report = createreport(dt);

            }

        }
        XtraReport createreport(DataTable dts)
        {

            XtraRepVarianceAgency report = new XtraRepVarianceAgency();
            report.DataSource = dts;
            report.DataMember = "vw•VariancesbyAgency";
            report.CreateDocument();
            return report;
        }
    }
}
