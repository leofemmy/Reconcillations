using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Reconcillations.Entity;
using Reconcillations.Repository;
using Reconcillations.Services;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Reconcillations.Pages
{
    public class AccreditModel : PageModel
    {
        ITransactionRepository _transactionRepository;

        private IHostEnvironment _hostingEnvironment; private readonly IEmailSender _emailSender;

        [BindProperty]
        public List<ModifyRecords> _translist { get; set; }

        public AccreditModel(IHostEnvironment hostingEnvironment, ITransactionRepository transactionRepository, IEmailSender emailSender)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment; _emailSender = emailSender;
        }

        //public void OnGet()
        //{
        //    // _translist = _transactionRepository.GetModifyRecords();
        //}

        public IActionResult OnGetModifyAll()
        {
            List<ModifyRecords> data = (from d in _transactionRepository.GetModifyRecords()
                                        select d).ToList();
            return new JsonResult(data);
        }

        public async Task OnGet()
        {

            string stremail = HttpContext.Session.GetString("UserEmail");

            string strtoken = _transactionRepository.Insertusertoken(stremail);

            string messageBody = "Dear <p></p><br/> " + stremail + ",<p></p><br/> <br/>OTP Request<p></p>IReconcile Solution daily OTP <b>" + strtoken + " </b><p>.</p><br/> Thanks <br/><p></p><p></p>Regards,<p></p><p></p><br/><br/>Mail  Team";

            await _emailSender.SendEmailAsync(stremail, "OTP", messageBody);
        }

        public IActionResult OnPostApprove(long ModifyID)
        {
            if (ModifyID > 0)
            {
                var count = _transactionRepository.DisableBankAccount(ModifyID);
                if (count > 0)
                {
                    //Message = "Record Disable Successfully !";
                    //return RedirectToPage("/Bank/Index");
                }
            }

            return Page();
        }

        public ActionResult OnPostApproves([FromBody] Disapprove approve)
        {
            ResponseInfo respone = _transactionRepository.ApproveReclassified(approve.modifyid, HttpContext.Session.GetString("UserEmail"));

            return new JsonResult(respone);
        }

        public ActionResult OnPostDisapprove([FromBody] Disapprove disapprove)
        {

            DataSet dstresult = new DataSet();

            dstresult = _transactionRepository.Disapprovemodify(HttpContext.Session.GetString("UserEmail"), disapprove.modifyid, disapprove.reasons);

            return new JsonResult(dstresult);
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
    }
}
