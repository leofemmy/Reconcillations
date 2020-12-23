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
using Reconcillations.Entity;
using Reconcillations.Reports;
using Reconcillations.Repository;

namespace Reconcillations.Pages
{
    public class BankDetailsModel : PageModel
    {
        ITransactionRepository _transactionRepository; IHostEnvironment _hostingEnvironment;

        public XtraReport Report { get; set; }

        public Summarys summarys { get; set; }

        public BankDetailsModel(IHostEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            var gt = HttpContext.Session.GetString("AgencyName").ToString();

            var gh = gt.Split("-");

            var ngh = gh[0];

            var gtstart = HttpContext.Session.GetString("StartDate");

            var gtend = HttpContext.Session.GetString("EndDate");

            DataTable dt = _transactionRepository.viewBankdetails(ngh.ToString(), Convert.ToDateTime(gtstart), Convert.ToDateTime(gtend));

            Report = createreport(dt);

        }

        XtraReport createreport(DataTable dts)
        {
            XtraRepBankDetails report = new XtraRepBankDetails();
            report.DataSource = dts;
            report.DataMember = "vwRevenueCollection";
            report.CreateDocument();
            return report;
        }

    }
}
