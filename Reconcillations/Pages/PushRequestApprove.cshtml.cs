using System;
using System.Collections.Generic;
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

namespace Reconcillations.Pages
{
    public class PushRequestApproveModel : PageModel
    {
        ITransactionRepository _transactionRepository;

        private IHostEnvironment _hostingEnvironment; private readonly IEmailSender _emailSender;

        public PushRequestApproveModel(IHostEnvironment hostingEnvironment,
            ITransactionRepository transactionRepository, IEmailSender emailSender)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment; _emailSender = emailSender;
        }

        public async Task OnGet()
        {
            string stremail = HttpContext.Session.GetString("UserEmail");

            string strtoken = _transactionRepository.Insertusertoken(stremail);

            string messageBody = "Dear <p></p><br/> " + stremail + ",<p></p><br/> <br/>OTP Request<p></p>IReconcile Solution daily OTP <b>" + strtoken + " </b><p>.</p><br/> Thanks <br/><p></p><p></p>Regards,<p></p><p></p><br/><br/>Mail  Team";

            await _emailSender.SendEmailAsync(stremail, "OTP", messageBody);
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

        //GetRequestPushReemLists

        public IActionResult OnGetRequsetAll()
        {
            List<RequestPushReemList> data = (from d in _transactionRepository.GetRequestPushReemLists()
                                              select d).ToList();
            return new JsonResult(data);
        }

        public ActionResult OnPostApprove([FromBody] PushToReemsExceution reemsExceution)
        {

            reemsExceution.Userid = HttpContext.Session.GetString("Usernames").ToString();


            var count = _transactionRepository.DoExecuatePushRequesttoReems(reemsExceution);


            return new JsonResult(count);
        }

    }
}
