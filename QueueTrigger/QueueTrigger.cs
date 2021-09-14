using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace QueueTrigger
{
    public static class QueueTrigger
    {
        //Actual queue trigger SendGrid function for the project
        //Sendgrid Method 1
        //Please take note "SendgridAPIKey" must key in Azure Portal function app -> configuration
        //for deployment in Azure to be able to work
        [FunctionName("QueueTrigger")]
        public static void Run(
            [QueueTrigger("contoso", Connection = "AzureWebJobsStorage")] string myQueueItem,
            [SendGrid(ApiKey = "SendgridAPIKey")] out SendGridMessage message,
            ILogger log)
        {
            log.LogInformation($"C# QueueTrigger processed: {myQueueItem}");
            message = new SendGridMessage();
            message.SetFrom(MyEmailSetting.FromEmail);
            message.AddTo(MyEmailSetting.ToEmail);
            message.SetSubject("Data Modified");
            message.AddContent("text/plain", $"Database modified!!!\n{myQueueItem}");
            log.LogInformation($"C# SendGrid processed");
        }

        ////Simple QueueTrigger
        //[FunctionName("QueueTrigger")]
        //public static void Run(
        //    [QueueTrigger("contoso", Connection = "AzureWebJobsStorage")] string myQueueItem,
        //    ILogger log)
        //{
        //    log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        //}

        //// Sendgrid Method 2
        ////Please take note "SendgridAPIKey" must key in Azure Portal function app -> configuration
        ////for deployment in Azure to be able to work
        //[FunctionName("QueueTrigger")]
        //[return: SendGrid(ApiKey = "SendgridAPIKey", To = "yawahi8183@sc2hub.com", From = "chongkf@outlook.sg")]
        //public static SendGridMessage Run(
        //    [QueueTrigger("contoso", Connection = "AzureWebJobsStorage")] string myQueueItem,
        //    ILogger log)
        //{
        //    log.LogInformation($"C# SendGrid processed: {myQueueItem}");
        //    var msg = new SendGridMessage();
        //    msg.SetSubject("Data Modified");
        //    msg.AddContent("text/plain", $"Database modified\n{myQueueItem}");
        //    return msg;
        //}


        ////Using Outlook for sending email. 
        //[FunctionName("QueueTrigger")]
        //public static void Run([QueueTrigger("contoso", Connection = "AzureWebJobsStorage")] string myQueueItem, ILogger log)
        //{
        //    log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

        //    //If don't want hard coded setting can be update in
        //    //Azure Portal function app -> configuration
        //    var smtpServer = MyEmailSetting.SmtpServer;
        //    var smtpServerSSL = MyEmailSetting.SmtpServerSSL;
        //    var smtpPort = MyEmailSetting.SmtpPort;
        //    var smtpFromEmail = MyEmailSetting.FromEmail;
        //    var smtpFromEmailAlias = MyEmailSetting.FromEmailAlias;
        //    var smtpUsername = MyEmailSetting.Username;
        //    var smtpPassword = MyEmailSetting.Password;
        //    var client = new SmtpClient(smtpServer)
        //    {
        //        UseDefaultCredentials = false,
        //        EnableSsl = smtpServerSSL,
        //        Port = smtpPort,
        //        //DeliveryMethod = SmtpDeliveryMethod.Network,

        //        Credentials = new NetworkCredential(
        //            userName: smtpUsername,
        //            password: smtpPassword)
        //    };

        //    var mailMessage = new MailMessage
        //    {
        //        From = new MailAddress(smtpFromEmail, smtpFromEmailAlias),
        //        Subject = "Data Modified from Azure"
        //    };

        //    mailMessage.To.Add(MyEmailSetting.ToEmail);
        //    // mailMessage.IsBodyHtml = true;
        //    mailMessage.IsBodyHtml = false;
        //    mailMessage.Body = myQueueItem;

        //    try
        //    {
        //        client.SendMailAsync(mailMessage).Wait();
        //        log.LogInformation($"Mail Sent to {MyEmailSetting.ToEmail}");
        //        //return Task.CompletedTask;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.LogInformation(ex.Message);
        //    }
        //}
    }
}
