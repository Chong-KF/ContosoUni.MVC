using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace ContosoUni.Services
{
    public class MyEmailSender : IEmailSender
    {
        private readonly ILogger<MyEmailSender> _logger;

        public MyEmailSender(
            ILogger<MyEmailSender> logger)
        {
            _logger = logger;
        }

        #region Microsoft.AspNetCore.Identity.UI.Services.IEmailSender menbers
        /// <summary>
        ///     Send the email using the IEmailSender Service.
        /// </summary>
        /// <param name="email">From Email Address</param>
        /// <param name="subject">Subject of the Email</param>
        /// <param name="htmlMessage">HTML Text for the Email</param>
        /// <returns>Task to indicate if email was successfully sent.</returns>
        /// <exception cref="Grocery.WebApp.Services.MyEmailSenderException" /> 
        /// <example>
        /// <![CDATA[
        ///     SendEmailAsync("demo@abc.com", "hello", "<p>Hello World</p>");
        ///     SendEmailAsync("demo@abc.com; demo2@abc.com", "hello", "<p>Hello World</p>");
        /// ]]>     
        /// </example>
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpServer = MyEmailSetting.SmtpServer;
            var smtpServerSSL = MyEmailSetting.SmtpServerSSL;
            var smtpPort = MyEmailSetting.SmtpPort;
            var smtpFromEmail = MyEmailSetting.FromEmail;
            var smtpFromEmailAlias = MyEmailSetting.FromEmailAlias;
            var smtpUsername = MyEmailSetting.Username;
            var smtpPassword = MyEmailSetting.Password;

            var client = new SmtpClient(smtpServer)
            {
                UseDefaultCredentials = false,
                EnableSsl = smtpServerSSL,
                Port = smtpPort,
                DeliveryMethod = SmtpDeliveryMethod.Network,

                Credentials = new NetworkCredential(
                    userName: smtpUsername,
                    password: smtpPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpFromEmail, smtpFromEmailAlias),
                Subject = subject
            };

            // note: split multiple email by ;
            //string[] emails = email.Split(';');
            
            //use dummy for testing, actual please use code above
            string[] emails = {MyEmailSetting.TestEmail};
            foreach (string e in emails)
            {
                mailMessage.To.Add(e);
            }
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = htmlMessage;

            MyEmailSenderException myexception;
            try
            {
                client.SendMailAsync(mailMessage).Wait();
                return Task.CompletedTask;
            }
            catch (SmtpFailedRecipientsException exp)
            {
                myexception = new MyEmailSenderException(
                    $"Unable to send email to {exp.FailedRecipient}", exp);
            }
            catch (SmtpFailedRecipientException exp)
            {
                myexception = new MyEmailSenderException(
                    $"Unable to send email to {exp.FailedRecipient}", exp);
            }
            catch (SmtpException exp)
            {
                myexception = new MyEmailSenderException(
                    $"There was problem sending email : {exp.Message}", exp);
            }
            catch (Exception exp)
            {
                myexception = new MyEmailSenderException(
                    $"Something went wrong : {exp.Message}", exp);
            }

            return Task.FromException<MyEmailSenderException>(myexception);
        }
        #endregion
    }
}