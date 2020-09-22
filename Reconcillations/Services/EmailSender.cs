using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;
using Reconcillations.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Reconcillations.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        private readonly IHostEnvironment _env;
        public EmailSender(IOptions<EmailSettings> emailSettings, IHostEnvironment env)
        {
            _emailSettings = emailSettings.Value;
            _env = env;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {

                var mimeMessage = new MimeMessage();

                mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.Sender));

                mimeMessage.To.Add(new MailboxAddress(email));

                mimeMessage.Subject = subject;

                mimeMessage.Body = new TextPart("html")
                {
                    Text = message
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    if (_env.IsDevelopment())
                    {
                        // The third parameter is useSSL (true if the client should make an SSL-wrapped
                        // connection to the server; otherwise, false).
                        await client.ConnectAsync(_emailSettings.MailServer, _emailSettings.MailPort, true);
                    }
                    else
                    {
                        await client.ConnectAsync(_emailSettings.MailServer);
                    }

                    // Note: only needed if the SMTP server requires authentication
                    await client.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);

                    await client.SendAsync(mimeMessage);

                    await client.DisconnectAsync(true);
                }
                //// Credentials
                //var credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password);

                //// Mail message
                //var mail = new MailMessage()
                //{
                //    From = new MailAddress(_emailSettings.Sender, _emailSettings.SenderName),
                //    Subject = subject,
                //    Body = message,
                //    IsBodyHtml = true
                //};

                //mail.To.Add(new MailAddress(email));

                //// Smtp client
                //var client = new System.Net.Mail.SmtpClient()
                //{
                //    Port = _emailSettings.MailPort,
                //    DeliveryMethod = SmtpDeliveryMethod.Network,
                //    UseDefaultCredentials = false,
                //    Host = _emailSettings.MailServer,
                //    EnableSsl = true,
                //    Credentials = credentials
                //};

                //// Send it...         
                //client.Send(mail);

                //From Address  
                //                string FromAddress = _emailSettings.Sender;//"myname@company.com";
                //                string FromAdressTitle = "ASP.NET Core DemoApp";
                //                //To Address  
                //                string ToAddress = email;
                //                string ToAdressTitle = "Nishan Aryal";
                //                string Subject = subject;
                //                string BodyContent = message;

                //                //Smtp Server  
                //                string SmtpServer = _emailSettings.MailServer;//"smtp.office365.com";
                //                //Smtp Port Number  
                //                int SmtpPortNumber = _emailSettings.MailPort;//587;

                //                var mimeMessage = new MimeMessage();
                //                mimeMessage.From.Add(new MailboxAddress
                //                                        (FromAdressTitle,
                //                                         FromAddress
                //                                         ));
                //                mimeMessage.To.Add(new MailboxAddress
                //                                         (ToAdressTitle,
                //                                         ToAddress
                //                                         ));
                //                mimeMessage.Subject = Subject; //Subject
                //                mimeMessage.Body = new TextPart("html")
                //                {
                //                    Text = BodyContent
                //                };

                //                using (var client = new SmtpClient())
                //                {
                //                    client.Connect(SmtpServer, SmtpPortNumber, false);
                //                    client.Authenticate(
                // _emailSettings.Sender,  // "myemail@mydomain.com",  //Enter your email here
                //_emailSettings.Password//      "MyPassword" //Enter your Password here.
                //    );
                //                    await client.SendAsync(mimeMessage);
                //                    Console.WriteLine("The mail has been sent successfully !!");
                //                    Console.ReadLine();
                //                    await client.DisconnectAsync(true);
            }

            catch (System.Exception ex)
            {
                //throw ex;
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
            //return Task.CompletedTask;
        }
    }

    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
