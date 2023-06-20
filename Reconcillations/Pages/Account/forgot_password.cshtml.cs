using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using MimeKit;
using Reconcillations.Entity;
using Reconcillations.Repository;
using Reconcillations.Services;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Reconcillations.Pages.Account
{
    public class forgot_passwordModel : PageModel
    {

        private IHostingEnvironment _hostingEnvironment; private readonly IEmailSender _emailSender;

        ITransactionRepository _transactionRepository;

        [BindProperty]
        public fpass _fpas { get; set; }

        public forgot_passwordModel(IHostingEnvironment hostingEnvironment, ITransactionRepository transactionRepository, IEmailSender emailSender)
        {
            _transactionRepository = transactionRepository;

            _hostingEnvironment = hostingEnvironment; _emailSender = emailSender;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPost()
        {

            if (ModelState.IsValid)
            {
                var intvalid = _transactionRepository.ValidateEmailAddress(_fpas);

                if (intvalid > 0)
                {
                    var code = Security.EncodeTo64(DateTime.Now.ToShortDateString() + Guid.NewGuid().ToString().Replace("-", string.Empty) + _fpas.Email);

                    var ghtcoe = _transactionRepository.UpdateUserActivationCode(_fpas.Email, code);

                    if (ghtcoe > 0)
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
                                userId = Security.EncodeTo64(_fpas.Email),
                                code = code
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
                                + Path.DirectorySeparatorChar
                                + "Templates"
                        + Path.DirectorySeparatorChar
                        + "EmailTemplate"
                        + Path.DirectorySeparatorChar
                        + "Confirm_Account_Registration.html";

                        var subject = "Password Reset Mail";

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

                        string messageBody = string.Format(builder.HtmlBody, subject, String.Format("{0:dddd, d MMMM yyyy}", DateTime.Now), _fpas.Email, _fpas.Email, "null", Message, callbackUrl);

                        //await _emailSender.SendEmailAsync("Femi.onabote@icmaservices.com", "Confirm your account",
                        //          $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");

                        await _emailSender.SendEmailAsync(_fpas.Email,
                            //"cc",
                            subject, messageBody);
                    }


                    return RedirectToPage("/index");
                }
            }
            return Page();
        }
    }
}
