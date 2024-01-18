using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Microsoft.Net.Http.Headers;
using System.Collections.Specialized;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Security.Policy;

namespace HINOSystem.Libs
{
    public class EmailClass
    {

        private readonly IConfiguration _config;
        private readonly WarrantyClaimConnect _wrtConnect;

        private string _DB = "";
        private string _SQL = "";


        public EmailClass(IConfiguration configuration, WarrantyClaimConnect wrtConnect)
        {
            _config = configuration;
            _wrtConnect = wrtConnect;
        }

        public void Send_ForgotPassword(string _email)
        {
            //MailMessage _Mail = new MailMessage();
            //WebClient Client = new WebClient();
            //string _message;


            //_message = Client.DownloadString("wwwroot\\assets\\template\\Mail\\ForgotPassword.html");
            //_message = _message.Replace("{#NAME#}", "abc");
            //_message = _message.Replace("{#URL#}", "https://localhost:7265/Authen/Login?ref=" + Id);
            //_message = "Test";

            //_Mail.From = new MailAddress("noreply@hinothailand.com");
            //_Mail.To.Add(new MailAddress(_email));
            //_Mail.Subject = "[HINO Warranty Claim] Password recovery.";


            //Send(_Mail, _message);
        }





        public void Send(MailMessage _Mail, string _Message)
        {
            string _result = "";
            try
            {
                _Mail.BodyEncoding = Encoding.UTF8;

                AlternateView HtmlView = AlternateView.CreateAlternateViewFromString(_Message, null, "text/html");
                _Mail.AlternateViews.Add(HtmlView);

                SmtpClient _STMP = new SmtpClient();
                _STMP.Host = _config.GetValue<string>("Mail:Server");
                _STMP.Send(_Mail);

                _result = "SEND";

            }
            catch(Exception ex)
            {
                _result = "FAILED";
            }
            finally
            {

                string _SQL_Log = @"INSERT INTO [log].[Mail] ([FromMail]
                                  ,[ToMail]
                                  ,[Subject]
                                  ,[Message]
                                  ,[Result]
                                  ,[SendAt]
                                )VALUES('" + _Mail.From.ToString() + @"'
                                  , '" + _Mail.To.ToString() + @"'
                                  , '" + _Mail.Subject + @"'
                                  , '" + _Message + @"'
                                  , '" + _result + @"'
                                  , GETDATE()
                                )";
                _wrtConnect.executeNonQuery(_SQL_Log, skipLog: true);
            }
        }




        private void example() {
            MailMessage myMail = new MailMessage();
            WebClient Client = new WebClient();
            string BodyMail;
            string Header;

            //try
            //{

            Header = "[IT Asset] Delivery plan | Notice to users";

            //BodyMail = Client.DownloadString(Server.MapPath("Template\\Email\\DeliveryPlanTemplate.htm"));
            //BodyMail = BodyMail.Replace("#Year", "20" + Year);
            //BodyMail = BodyMail.Replace("#Link", "https://hmmtweb01.hinothailand.com/ITAsset/DeliveryPlanReport.aspx?Id=" + Id + "&DepartmentCode=" + DepartmentCode + "&Company=" + Company);
            BodyMail = "Test";

            myMail.From = new MailAddress("noreply@hinothailand.com");

            //string[] EmailReceive = oReceive.Split(';');

            //for (int i = 0; i < EmailReceive.Length; i++)
            //{
            //    myMail.To.Add(new MailAddress(EmailReceive[i]));
            //}

            myMail.To.Add(new MailAddress("prachaya_c@hinothailand.com"));
            //myMail.CC.Add(new MailAddress(oSender));

            myMail.BodyEncoding = Encoding.UTF8;

            AlternateView HtmlView;
            HtmlView = AlternateView.CreateAlternateViewFromString(BodyMail, null, "text/html");
            myMail.AlternateViews.Add(HtmlView);

            myMail.Subject = Header;

            SmtpClient Clients = new SmtpClient();

            Clients.Host = _config.GetValue<string>("Mail:Server");

            Clients.Send(myMail);
        }
    }

}
