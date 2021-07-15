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
using Reconcillations.Reports;
using Reconcillations.Repository;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Reconcillations.Pages
{
    public class DetailsModel : PageModel
    {
        ITransactionRepository _transactionRepository; IHostEnvironment _hostingEnvironment;
        public XtraReport Report { get; set; }

        public DetailsModel(IHostEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment;
        }
        public void OnGet()
        {
        }

        public void OnPost()
        {
            var gt = HttpContext.Session.GetString("RevenueName");
            //var gtstart = HttpContext.Session.GetString("StartDate");
            //var gtend = HttpContext.Session.GetString("EndDate");

            var pMonths = HttpContext.Session.GetString("perMonth");
            var pYear = HttpContext.Session.GetString("perYear");

            DataTable dt = _transactionRepository.Viewdetails(gt.ToString(), Convert.ToInt64(pMonths), Convert.ToInt64(pYear));

            Report = createreport(dt);
        }
        XtraReport createreport(DataTable dts)
        {

            XtraRepDetailsCollection report = new XtraRepDetailsCollection();
            report.DataSource = dts;
            report.DataMember = "vwCollectionDetails";
            report.CreateDocument();
            return report;
        }
    }
}
