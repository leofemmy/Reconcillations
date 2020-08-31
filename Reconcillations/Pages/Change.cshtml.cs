using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reconcillations.Entity;
using Reconcillations.Repository;
using Microsoft.Extensions.Hosting;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Reconcillations.Pages
{
    public class ChangeModel : PageModel
    {
        IHostingEnvironment _hostingEnvironment; ITransactionRepository _transactionRepository;

        [TempData]
        public string Message { get; set; }
        public ChangeModel(IHostingEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        [BindProperty(SupportsGet = true)]
        public Changepasss passchng { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            passchng.Email = HttpContext.Session.GetString("UserEmail");

            //if (ModelState.IsValid)
            //{
            DataSet dsusers = _transactionRepository.GetUserLogin(HttpContext.Session.GetString("UserEmail"));

            if (dsusers != null && dsusers.Tables[0].Rows.Count > 0)
            {
                string dbPassword = dsusers.Tables[1].Rows[0]["Password"].ToString();
                string dbUserGuid = dsusers.Tables[1].Rows[0]["UserGuid"].ToString();
                string dbUserype = dsusers.Tables[1].Rows[0]["TypeCode"].ToString();
                string strusername = dsusers.Tables[1].Rows[0]["username"].ToString();


                string hashedPassword = Security.HashSHA1(passchng.currentPassword + dbUserGuid);

                if (dbPassword == hashedPassword)
                {
                    Guid userGuid = System.Guid.NewGuid();

                    string hashedPass = Security.HashSHA1(passchng.Password + userGuid);
                    //ResetUserPassword
                    //var count = _transactionRepository.UserConfirmation(cuserpass.Email, cuserpass.Code, hashedPassword, userGuid.ToString());
                    int count = _transactionRepository.ResetUserPassword(passchng.Email, hashedPass, userGuid.ToString());

                    if (count == 1)
                    {
                        Message = "PassWord Reset Sucessfully";

                        await Task.CompletedTask;

                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        Message = "Error occur while reseting ! Try Again"; return Page();
                    }
                }
                else
                {
                    Message = "Wrong Current Password";
                }
            }
            //}


            await Task.CompletedTask;

            return Page();
        }
    }
}
