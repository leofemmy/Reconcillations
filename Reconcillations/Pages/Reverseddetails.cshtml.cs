using System;
using  System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Reconcillations.Entity;
using Reconcillations.Reports;
using Reconcillations.Repository;

namespace Reconcillations.Pages
{
    public class ReverseddetailsModel : PageModel
    {
        ITransactionRepository _transactionRepository; IHostEnvironment _hostingEnvironment;

        public XtraReport Report { get; set; }

        public Summarys summarys { get; set; }

        public ReverseddetailsModel(IHostEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            var gt = HttpContext.Session.GetString("AccountNumber");
            var gtstart = HttpContext.Session.GetString("StartDate");
            var gtend = HttpContext.Session.GetString("EndDate");

            DataTable dt = _transactionRepository.viewReversedTransaction(gt.ToString(), Convert.ToDateTime(gtstart), Convert.ToDateTime(gtend));

            Report = createreport(dt);

        }

        XtraReport createreport(DataTable dts)
        {

            XtraRepreverseddetails report = new XtraRepreverseddetails();
            report.DataSource = dts;
            report.DataMember = "vwreversedtransaction";
            report.CreateDocument();
            return report;
        }
    }
}
