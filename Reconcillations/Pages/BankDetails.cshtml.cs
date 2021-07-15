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

            var perYear = HttpContext.Session.GetString("perYear");

            var perMonths = HttpContext.Session.GetString("perMonth");

            var gh = gt.Split("-");

            var ngh = gh[0];


            DataTable dt = _transactionRepository.viewBankdetails(ngh.ToString(), Convert.ToInt64(perMonths), Convert.ToInt64(perYear));

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
