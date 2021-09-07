using System;
using System.Collections.Generic;
using System.Configuration;
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

        //private readonly int PasswordExpireDays ;

        [TempData]
        public string Message { get; set; }

        [TempData]
        public string MessageCode { get; set; }

        [BindProperty]
        public Pass cuserpass { get; set; }

        public const string MessageKey = nameof(MessageKey);

        private bool blFirstlogin = false;

        public IndexModel(ILogger<IndexModel> logger, IHostingEnvironment hostingEnvironment, ITransactionRepository transactionRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        public void OnGet()
        {
            //var url = Security.GetRawTarget(this.Request);
            if (blFirstlogin)
            {
                _transactionRepository.UpdateUserlog(cuserpass.Email, 0);
            }
        }
        public void OnPost()
        {
            Guid userGuid = System.Guid.NewGuid();

            _logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(cuserpass));


            if (ModelState.IsValid)
            {

                int PasswordExpireDays = Convert.ToInt32(ConfigurationManager.AppSettings["PasswordExpireDays"]);

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

                        DataSet dbCheckUserid = _transactionRepository.CheckUserid(cuserpass.Email);

                        if (dbCheckUserid != null && dbCheckUserid.Tables[0].Rows.Count > 0)
                        {
                            //Boolean blflag = !string.IsNullOrWhiteSpace(dbCheckUserid.Tables[0].Rows[0]["Flag"].ToString()) ? Convert.ToBoolean(dbCheckUserid.Tables[0].Rows[0]["Flag"].ToString()) : false;

                            //if (blflag)
                            //{
                            //    //already log in using another browser
                            //    Message = "User have previously log in with another browser, Log out to continue";
                            //    MessageCode = "1";
                            //}
                            //else
                            {
                                Int32 kj = _transactionRepository.UpdateUserlog(cuserpass.Email, 1);

                                MessageCode = "0";

                                blFirstlogin = true;

                                //HttpContext.Session["Useremail"] = cuserpass.Email.ToString();
                                HttpContext.Session.SetString("UserEmail", cuserpass.Email);
                                HttpContext.Session.SetString("FirstLogin", "1");
                                HttpContext.Session.SetString("UserType", dbUserype);
                                HttpContext.Session.SetString("Usernames", strusername);

                                //ViewData["Mess"] = "Login Successfull";
                                Message = "Login Successfull";


                                _logger.LogInformation(Message);

                                TempData[MessageKey] = "Login Successfull!";
                                //return RedirectToAction(Request.Path);

                                ViewData["JavaScript"] = "window.location = '" + Url.Page("Main") + "'";
                            }

                        }

                        //  return RedirectToPage("/Main");
                    }
                    else
                    {
                        //ViewData["Mess"] = "Login Failed due to wrong user ID or Password";
                        Message = "Login Failed due to wrong user ID or Password";
                        MessageCode = "-1";
                        //  return Page();
                    }
                }
            }
            else
            {
                ViewData["Mess"] = "Login Failed";
                Message = "Login Failed"; MessageCode = "-1";
                // return Page();
            }
            //await Task.CompletedTask;
            //return Page();
        }
    }
}
