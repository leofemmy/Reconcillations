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
using Newtonsoft.Json.Linq;
using Reconcillations.Entity;
using Reconcillations.Reports;
using Reconcillations.Repository;
using Reconcillations.Services;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Reconcillations.Pages.Request
{
    public class IndexModel : PageModel
    {
        ITransactionRepository _transactionRepository;

        private IHostEnvironment _hostingEnvironment; private readonly IEmailSender _emailSender;

        string stremail = string.Empty; string subject = string.Empty;
        //[BindProperty]
        //public List<Postinglist> PostListing { get; set; }
        public SelectList PostingSelectlist { get; set; }

        [BindProperty]

        public Postinglist pstlist { get; set; }

        public XtraReport Report { get; set; }

        [BindProperty]

        public List<RequestApprove> _translist { get; set; }

        public IndexModel(IHostEnvironment hostingEnvironment, ITransactionRepository transactionRepository, IEmailSender emailSender)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment; _emailSender = emailSender;
        }

        public IActionResult OnGetPostingRequestlist()
        {
            List<Postinglist> data = (from g in _transactionRepository.GetAllPostingRequesting() select g).ToList();
            return new JsonResult(data);
        }

        public void OnPost()
        {
            ViewData["UserEmail"] = HttpContext.Session.GetString("UserEmail");

            //var select = document.getElementById("country");
            if (pstlist.RecperID != 0)
            {
                long bg = pstlist.RecperID;

                DataTable dt = _transactionRepository.ViewPostingRequest(bg);

                Report = createreport(dt);
            }

        }

        public IActionResult OnPostValidteotp([FromBody] JObject strOtp)
        {
            string retval = string.Empty; string strotpval = string.Empty;

            if (!string.IsNullOrWhiteSpace(strOtp.ToString()))
            {
                foreach (var item in strOtp)
                {
                    Console.WriteLine(item.Key + " " + item.Value.ToString());

                    if (item.Key.ToString() == "strOtp")
                    {
                        strotpval = item.Value.ToString();
                    }
                }
            }
            string emial = HttpContext.Session.GetString("UserEmail").ToString();

            retval = _transactionRepository.validateuserotp(emial, strotpval);

            return new JsonResult(retval);
        }

        public async Task<IActionResult> OnPostToken()
        {
            String retval = string.Empty;

            var strtoken = Security.GenerateToken();

            if (TempData.ContainsKey("UserEmail"))
            {
                stremail = TempData["UserEmail"].ToString();
            }
            await Task.CompletedTask;
            return new JsonResult(retval);
        }

        XtraReport createreport(DataTable dts)
        {
            XtraReport1 report = new XtraReport1();
            report.DataSource = dts;
            report.DataMember = "vwpostingrequestlist";
            report.CreateDocument();
            return report;
        }

        public IActionResult OnPostApproval([FromBody] JObject apprec)
        {
            string retval = string.Empty; double recperiodid = 0;

            if (!string.IsNullOrWhiteSpace(apprec.ToString()))
            {
                foreach (var item in apprec)
                {
                    Console.WriteLine(item.Key + " " + item.Value.ToString());

                    if (item.Key.ToString() == "recperid")
                    {
                        recperiodid = Convert.ToDouble(item.Value.ToString());
                    }
                }
            }
            string emial = HttpContext.Session.GetString("UserEmail").ToString();

            retval = _transactionRepository.posttransactionapproval(emial, recperiodid);

            return new JsonResult(retval);

        }

        public void OnGet()
        {
            TempData["UserEmail"] = HttpContext.Session.GetString("UserEmail");
            //var reconcileId = 10003;
            //DataTable dt = _transactionRepository.ViewPostingRequest(reconcileId);
            _translist = _transactionRepository.GetRequestApprove();
            //Report = CreateReport(dt);

            //load currency into a combo
            //var ele = from d in _transactionRepository.GetCurrencies() select d;
            //CurrencySelectList = new SelectList(ele, "CurrencyID", "CurrencyName");

            var gh = (from g in _transactionRepository.GetAllPostingRequesting() select g).ToList();
            PostingSelectlist = new SelectList(gh, "RecperID", "Description");
            //PostListing = gh;
            HttpContext.Session.Set("psList", gh);

            string host = HttpContext.Request.Host.Value;
        }

        public IActionResult OnGetBankAll()
        {
            //List<Banklist> data = (from d in _transactionRepository.GeBanklists()
            //                       select d).ToList();
            var gh = (from g in _transactionRepository.GetAllPostingRequesting() select g).ToList();
            PostingSelectlist = new SelectList(gh, "RecperID", "Description");
            return new JsonResult(PostingSelectlist);
        }

        public IActionResult OnPostRequest(long id)
        {
            DataSet dstresult = new DataSet();
            //posting.reconileID = 10003;
            dstresult = _transactionRepository.PostingRequestSent(id, HttpContext.Session.GetString("UserEmail"));
            return RedirectToPage("/Request/Index");
        }

    }
}
