using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeKit;
using Reconcillations.Entity;
using Reconcillations.Repository;
using Reconcillations.Services;
using Microsoft.Extensions.Hosting;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Reconcillations.Pages.Users
{
    public class CreateModel : PageModel
    {
        ITransactionRepository _transactionRepository;

        private readonly IEmailSender _emailSender;

        private IHostingEnvironment _hostingEnvironment;
        public SelectList UsertypeSelectlist { get; set; }
        public Usertypelist pstlist { get; set; }

        [BindProperty]
        public CUsers cUsers { get; set; }

        [TempData]
        public string Message { get; set; }

        public CreateModel(IHostingEnvironment hostingEnvironment, ITransactionRepository transactionRepository, IEmailSender emailSender)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment;

            _emailSender = emailSender;
        }
        public void OnGet()
        {
            var gh = (from g in _transactionRepository.GetUsertypelist() select g).ToList();
            UsertypeSelectlist = new SelectList(gh, "TypeCode", "Description");

            HttpContext.Session.Set("psList", gh);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {  //generate action code per user
                var code = DateTime.Now.ToShortDateString() + Guid.NewGuid().ToString().Replace("-", string.Empty) + cUsers.Email.ToString();

                var count = _transactionRepository.AddUserAccount(cUsers, code);

                if (count > 0)
                {
                    //send confirmation mail here
                    var webRoot = _hostingEnvironment.WebRootPath;
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string contentRootPath = _hostingEnvironment.ContentRootPath;

                    //var gt = (webRootPath + "\n" + contentRootPath);

                    //var location = new Uri($"{Request.Scheme}://{Request.Host}");
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(cUsers);
                    //var callbackUrl = location.AbsoluteUri + strpath.ToString();
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new
                        {
                            userId = Security.EncodeTo64(cUsers.Email),
                            code = Security.EncodeTo64(code)
                        },
                        protocol: HttpContext.Request.Scheme);

                    if (string.IsNullOrWhiteSpace(_hostingEnvironment.WebRootPath))
                    {
                        webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    }

                    //var callbackUrl = Url.Action("Account/ConfirmEmail", "Account", null, protocol: HttpContext.Request.Scheme);

                    //Email from Email Template
                    //string Message = "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>";
                    string Message = "Please confirm your account by clicking ";
                    // string body;

                    //Get TemplateFile located at wwwroot/Templates/EmailTemplate/Register_EmailTemplate.html
                    var pathToFile = webRoot
                            + Path.DirectorySeparatorChar.ToString()
                            + "Templates"
                    + Path.DirectorySeparatorChar.ToString()
                    + "EmailTemplate"
                    + Path.DirectorySeparatorChar.ToString()
                    + "Confirm_Account_Registration.html";

                    var subject = "Confirm Account Registration";

                    var builder = new BodyBuilder();

                    using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                    {
                        builder.HtmlBody = SourceReader.ReadToEnd();
                    }
                    //{0} : Subject
                    //{1} : DateTime
                    //{2} : Email
                    //{3} : Username
                    //{4} : Password
                    //{5} : Message
                    //{6} : callbackURL

                    string messageBody = string.Format(builder.HtmlBody,
                        subject,
                        String.Format("{0:dddd, d MMMM yyyy}", DateTime.Now),
                       cUsers.Email,// "Iiii1986@rhyta.com",
                       cUsers.Email,// "Iiii1986@rhyta.com",
                        "null",
                        Message,
                        callbackUrl
                        );

                    //await _emailSender.SendEmailAsync("Femi.onabote@icmaservices.com", "Confirm your account",
                    //          $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");

                    await _emailSender.SendEmailAsync(cUsers.Email,
                        //"cc",
                        subject, messageBody);
                }
                return RedirectToPage("/Users/Index");
                //return RedirectToPage("/index");
            }
            else
            {
                return Page();
            }



        }
    }
}
