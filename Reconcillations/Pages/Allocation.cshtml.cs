using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Reconcillations.Entity;
using Reconcillations.Reports;
using Reconcillations.Repository;
using Reconcillations.Services;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Reconcillations.Pages
{
    public class AllocationModel : PageModel
    {
        ITransactionRepository _transactionRepository;

        IHostingEnvironment _hostingEnvironment; private readonly IEmailSender _emailSender;

        public SelectList PostingSelectlist { get; set; }
        [BindProperty]
        public Postinglist pstlist { get; set; }
        public XtraReport Report { get; set; }

        public AllocationModel(IHostingEnvironment hostingEnvironment, ITransactionRepository transactionRepository, IEmailSender emailSender)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment;

            _emailSender = emailSender;
        }
        public void OnGet()
        {
            TempData["UserEmail"] = HttpContext.Session.GetString("UserEmail");

            var gh = (from g in _transactionRepository.GetReconciledClosed() select g).ToList();

            PostingSelectlist = new SelectList(gh, "RecperID", "Description");

            HttpContext.Session.Set("psList", gh);

        }

        public void OnPost()
        {
            ViewData["UserEmail"] = HttpContext.Session.GetString("UserEmail");

            //var select = document.getElementById("country");
            if (pstlist.RecperID != 0)
            {
                long bg = pstlist.RecperID;

                DataTable dt = _transactionRepository.ViewBankAllocation(bg);

                ////Report = report;
                Report= createreport(dt);
            }

        }

        XtraReport createreport(DataTable dts)
        {
            XtraRepAllocation report = new XtraRepAllocation();
            report.DataSource = dts;
            report.DataMember = string.Empty;
            report.CreateDocument();
            return report;
        }
    }
}
