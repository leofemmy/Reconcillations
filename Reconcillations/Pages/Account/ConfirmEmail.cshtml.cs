using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Reconcillations.Entity;
using Reconcillations.Repository;
//using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;

namespace Reconcillations.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private IHostingEnvironment _hostingEnvironment;

        ITransactionRepository _transactionRepository;
        public ConfirmEmailModel(IHostingEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment;
        }


        [BindProperty(SupportsGet = true)]
        public Cuserpass cuserpass { get; set; }

        public string testid; string _userid; string _userCode;

        public IActionResult OnGet(string userId, string code)
        {
            cuserpass = new Cuserpass
            {
                Code = Security.DecodeFrom64(code),
                Email = Security.DecodeFrom64(userId)
            };
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            Guid userGuid = System.Guid.NewGuid();

            if (ModelState.IsValid)
            {

                string hashedPassword = Security.HashSHA1(cuserpass.Password.ToString() + userGuid.ToString());

                var count = _transactionRepository.UserConfirmation(cuserpass.Email, cuserpass.Code, hashedPassword, userGuid.ToString());

                await Task.CompletedTask;

                return RedirectToPage("/Index");
            }
            else
            {
                await Task.CompletedTask;
                return Page();
            }
        }
    }
}
