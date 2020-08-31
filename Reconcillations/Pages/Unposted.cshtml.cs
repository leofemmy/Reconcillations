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
using Reconcillations.Entity;
using Reconcillations.Reports;
using Reconcillations.Repository;
using Reconcillations.Services;
using Microsoft.Extensions.Hosting;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Reconcillations.Pages
{
    public class UnpostedModel : PageModel
    {
        ITransactionRepository _transactionRepository;

        private IHostingEnvironment  _hostingEnvironment; private readonly IEmailSender _emailSender;

        public SelectList PostingSelectlist { get; set; }
        [BindProperty]
        public Postinglist pstlist { get; set; }
        public XtraReport Report { get; set; }

        public UnpostedModel(IHostingEnvironment  hostingEnvironment, ITransactionRepository transactionRepository, IEmailSender emailSender)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment; _emailSender = emailSender;
        }
        public void OnGet()
        {
            TempData["UserEmail"] = HttpContext.Session.GetString("UserEmail");

            var gh = (from g in _transactionRepository.GetReconciledClosed() select g).ToList();
            PostingSelectlist = new SelectList(gh, "RecperID", "Description");
            //PostListing = gh;
            HttpContext.Session.Set("psList", gh);
        }

        public void OnPost()
        {
            ViewData["UserEmail"] = HttpContext.Session.GetString("UserEmail");

            //var select = document.getElementById("country");
            if (pstlist.RecperID != 0)
            {
                long bg = pstlist.RecperID;
                DataTable dt = _transactionRepository.Viewunpostedcollection(bg);

                Report = createreport(dt);
            }

        }

        XtraReport createreport(DataTable dts)
        {
            XtraRepUnpostedCollections report = new XtraRepUnpostedCollections();
            report.DataSource = dts;
            report.DataMember = "vwBankAllocation";
            report.CreateDocument();
            return report;
        }
    }
}
