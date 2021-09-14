using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUni.Services
{
    public class MyEmailSetting
    {
        public const string SmtpServer = "smtp.office365.com";
        public const bool SmtpServerSSL = true;
        public const int SmtpPort = 587;
        public const string FromEmail = "chongkf@outlook.sg";
        public const string FromEmailAlias = "ContosoUni Support Desk";
        public const string Username = "chongkf@outlook.sg";
        public const string Password = "yunting$5030";
        public const string TestEmail = "yawahi8183@sc2hub.com"; //use for testing only
    }
}
