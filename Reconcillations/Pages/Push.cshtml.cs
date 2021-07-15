using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Reconcillations.Entity;
using Reconcillations.Repository;
using Reconcillations.Services;

namespace Reconcillations.Pages
{
    public class PushModel : PageModel
    {
        ITransactionRepository _transactionRepository;

        private IHostEnvironment _hostingEnvironment; private readonly IEmailSender _emailSender;

        public SelectList PushSelectlist { get; set; }

        [BindProperty]
        public List<PushException> _translist { get; set; }

        public PushModel(IHostEnvironment hostingEnvironment, ITransactionRepository transactionRepository, IEmailSender emailSender)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment; _emailSender = emailSender;
        }

        public void OnGet()
        {
            _translist = _transactionRepository.GetPushExceptions();
        }

        public ActionResult OnPostPushRequest([FromBody] PushExceptRequest pushExceptRequest)
        {
            //DoInsertPushtoReemsRequest

            pushExceptRequest.Userid = HttpContext.Session.GetString("Usernames").ToString();

            var count = _transactionRepository.DoInsertPushtoReemsRequest(pushExceptRequest);
            return new JsonResult(count);
        }
    }
}
