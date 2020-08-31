using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Reconcillations.Entity;
using Reconcillations.Repository;
using Microsoft.Extensions.Hosting;
//using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;

namespace Reconcillations.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private IHostingEnvironment _hostingEnvironment;

        ITransactionRepository _transactionRepository;

        [TempData] 
        public string Message { get; set; }

        [BindProperty]
        public Pass cuserpass { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IHostingEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        public void OnGet()
        {
            var url = Security.GetRawTarget(this.Request);
        }
        public async Task<IActionResult> OnPost()
        {
            Guid userGuid = System.Guid.NewGuid();

            _logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(cuserpass));

            if (ModelState.IsValid)
            {
                DataSet dsusers = _transactionRepository.GetUserLogin(cuserpass.Email);

                if (dsusers != null && dsusers.Tables[0].Rows.Count > 0)
                {
                    string dbPassword = dsusers.Tables[1].Rows[0]["Password"].ToString();
                    string dbUserGuid = dsusers.Tables[1].Rows[0]["UserGuid"].ToString();
                    string dbUserype = dsusers.Tables[1].Rows[0]["TypeCode"].ToString();
                    string strusername = dsusers.Tables[1].Rows[0]["username"].ToString();


                    string hashedPassword = Security.HashSHA1(cuserpass.Userpass + dbUserGuid);

                    if (dbPassword == hashedPassword)
                    {
                        //assign user detail to session

                        //HttpContext.Session["Useremail"] = cuserpass.Email.ToString();
                        HttpContext.Session.SetString("UserEmail", cuserpass.Email);
                        HttpContext.Session.SetString("UserType", dbUserype);
                        HttpContext.Session.SetString("Usernames", strusername);

                        Message = "Login Successfull";

                        _logger.LogInformation(Message);
                        return RedirectToPage("/Main");
                    }
                    else
                    {
                        Message = "Login Failed due to wrong user ID or Password";
                        return Page();
                    }
                }
            }
            else
            {
                Message = "Login Failed";
                return Page();
            }
            await Task.CompletedTask;
            return Page();
        }
    }
}
