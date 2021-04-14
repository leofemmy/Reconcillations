using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Reconcillations.Repository;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;

namespace Reconcillations.Pages
{
    public class LogOutModel : PageModel
    {
        private IWebHostEnvironment _hostingEnvironment;

        ITransactionRepository _transactionRepository;

        public LogOutModel(IHostingEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        public void OnGet()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("FirstLogin").ToString()))
            {
                if (HttpContext.Session.GetString("FirstLogin").ToString() == "1")
                {
                    _transactionRepository.UpdateUserlog(HttpContext.Session.GetString("UserEmail").ToString(), 0);

                    RedirectToPage("/Index");
                }
            }
        }
    }
}
