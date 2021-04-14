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
using Microsoft.Extensions.Hosting;
using Reconcillations.Reports;
using Reconcillations.Repository;
using Reconcillations.Services;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Reconcillations.Pages.Approval
{
    public class ViewModel : PageModel
    {
        ITransactionRepository _transactionRepository; private IHostingEnvironment _hostingEnvironment; private readonly IEmailSender _emailSender;
        public XtraReport Report { get; set; }
        public ViewModel(IHostingEnvironment hostingEnvironment, ITransactionRepository transactionRepository, IEmailSender emailSender)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment; _emailSender = emailSender;
        }
        public void OnGet(long id)
        {
            if (id != 0)
            {
                TempData["ID"] = id;

                DataTable dt = _transactionRepository.ViewPostingRequest(id);

                Report = createreport(dt);
            }
        }

        public IActionResult OnPostApproval()
        {
            string retval = string.Empty; double recperiodid = 0;

            //posttransactionapproval

            if (TempData.ContainsKey("ID"))
            {
                recperiodid = Convert.ToDouble(TempData["ID"].ToString());
            }

            if (recperiodid != 0)
            {
                string emial = HttpContext.Session.GetString("UserEmail").ToString();

                retval = _transactionRepository.posttransactionapproval(emial, recperiodid);


            }

            return new JsonResult(retval);


        }
        XtraReport createreport(DataTable dts)
        {
            XtraRepPostingRequestlist report = new XtraRepPostingRequestlist();
            report.DataSource = dts;
            report.DataMember = "vwpostingrequestlist";
            report.CreateDocument();
            return report;
        }
    }
}
