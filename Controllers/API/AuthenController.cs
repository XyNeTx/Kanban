using Microsoft.AspNetCore.Mvc;
using System.Data;
//using System.Net.Mail;
using System.Net;
//using EASendMail;
using System.Net.Mail;
using HINOSystem.Libs;

namespace HINOSystem.Controllers.API
{
    public class AuthenController : Controller
    {
        private readonly WarrantyClaimConnect _wrtConnect;
        private readonly EmailClass _email;


        private readonly DbConnect  _dbConnect;

        public AuthenController(WarrantyClaimConnect wrtConnect, EmailClass email, DbConnect dbConnect)
        {
            _wrtConnect = wrtConnect;
            _email = email;

            _wrtConnect.setContext(HttpContext);



            _dbConnect = dbConnect;
        }


        [HttpPost]  //Forgot Password
        public IActionResult ForgotPassword()
        {
            //return View();

            string _statement = @"
                UPDATE [erp].[User] 
                SET ResetToken = CONVERT(VARCHAR(100), HashBytes('MD5', 
	                'RESETTOKEN' + Code + 
	                isnull(Name,'HINO') + 
	                isnull(Surname,'HINO') + 
	                isnull(NameTH,'HINO') + 
	                isnull(SurnameTH,'HINO')+
	                REPLACE(CONVERT(VARCHAR(20) , GETDATE() , 112), ':', '') +
	                REPLACE(CONVERT(VARCHAR(20) , GETDATE() , 114), ':', '') 
	                ), 1)
                    , LastLogin = GETDATE()
	                , UpdateAt = GETDATE()
                WHERE [Code] = '" + Request.Form["txtResetUserName"].ToString() + @"';

                SELECT * FROM [erp].[User] WHERE _ID NOT IN (1,2) AND isDelete=0 AND [Code] = '" + Request.Form["txtResetUserName"].ToString() + @"';
            ";
            DataTable _dataTable = _wrtConnect.ExecuteSQL(_statement);

            string _result = "";
            if (_dataTable.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(_dataTable.Rows[0]["Email"].ToString()))
                {
                    MailMessage _Mail = new MailMessage();
                    WebClient Client = new WebClient();
                    string _message;

                    if (_dataTable.Rows[0]["UILanguage"].ToString()=="TH")
                    {
                        _message = Client.DownloadString("wwwroot\\assets\\template\\Mail\\Login\\ForgotPasswordTH.html");
                        _message = _message.Replace("{#NAME#}", 
                            (string.IsNullOrEmpty(_dataTable.Rows[0]["NameTH"].ToString()) ? 
                            _dataTable.Rows[0]["Name"].ToString() : 
                            _dataTable.Rows[0]["NameTH"].ToString()) + 
                            (string.IsNullOrEmpty(_dataTable.Rows[0]["SurnameTH"].ToString()) ?
                            " " + _dataTable.Rows[0]["Surname"].ToString() : " " +
                            _dataTable.Rows[0]["SurnameTH"].ToString())
                            );

                        _Mail.Subject = "[HINO Warranty Claim] กู้คืนบัญชีผู้ใช้งาน.";
                    }
                    else
                    {
                        _message = Client.DownloadString("wwwroot\\assets\\template\\Mail\\Login\\ForgotPassword.html");
                        _message = _message.Replace("{#NAME#}", _dataTable.Rows[0]["Name"].ToString() + (string.IsNullOrEmpty(_dataTable.Rows[0]["Surname"].ToString()) ? "" : " " + _dataTable.Rows[0]["Surname"].ToString()));

                        _Mail.Subject = "[HINO Warranty Claim] Password recovery.";
                    }

                    _message = _message.Replace("{#URL#}", "http://warranty.hinommt.com/Login/Recovery?ref=" + _dataTable.Rows[0]["ResetToken"].ToString());                    
                    //_message = _message.Replace("{#URL#}", "https://localhost:7265/Login/Recovery?ref=" + _dataTable.Rows[0]["ResetToken"].ToString());

                    _Mail.From = new MailAddress("noreply@hinothailand.com");
                    _Mail.To.Add(new MailAddress(_dataTable.Rows[0]["Email"].ToString()));


                    _email.Send(_Mail, _message);


                    _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""Email has been send.""
                    }";

                    return Content(_result, "application/json");
                }
            }

            _result = @"{
                ""status"":""202"",
                ""response"":""FAILED"",
                ""message"": ""Not found this account"" " + @"
            }";

            return Content(_result, "application/json");
        }


        [HttpPost]  //Reset Password
        public IActionResult ResetPassword()
        {
            //return View();


            string _statement = @"
                UPDATE [erp].[User] 
                SET [Password] ='" + Request.Form["txtRecoveryPassword"].ToString() + @"'
                WHERE [ResetToken] = '" + Request.Form["ref"].ToString() + @"';


                UPDATE [erp].[User] 
                SET [Password] = ENCRYPTBYPASSPHRASE ('HinoStyle', [Password])
                    , ResetToken = NULL
                WHERE [ResetToken] = '" + Request.Form["ref"].ToString() + @"';

                SELECT * FROM [erp].[User] WHERE _ID NOT IN (1,2) AND isDelete=0 AND [Code] = '" + Request.Form["txtRecoveryUserName"].ToString() + @"';
            ";
            DataTable _dataTable = _wrtConnect.ExecuteSQL(_statement);

            string _result = "";
            if (_dataTable.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(_dataTable.Rows[0]["Email"].ToString()))
                {
                    MailMessage _Mail = new MailMessage();
                    WebClient Client = new WebClient();
                    string _message;

                    if (_dataTable.Rows[0]["UILanguage"].ToString() == "TH")
                    {
                        _message = Client.DownloadString("wwwroot\\assets\\template\\Mail\\Login\\ConfirmPasswordTH.html");
                        _message = _message.Replace("{#NAME#}",
                            (string.IsNullOrEmpty(_dataTable.Rows[0]["NameTH"].ToString()) ?
                            _dataTable.Rows[0]["Name"].ToString() :
                            _dataTable.Rows[0]["NameTH"].ToString()) +
                            (string.IsNullOrEmpty(_dataTable.Rows[0]["SurnameTH"].ToString()) ?
                            " " + _dataTable.Rows[0]["Surname"].ToString() : " " +
                            _dataTable.Rows[0]["SurnameTH"].ToString())
                            );

                        _Mail.Subject = "[HINO Warranty Claim] คุณได้ทำการเปลี่ยนรหัสผ่านเรียบร้อยแล้ว.";
                    }
                    else
                    {
                        _message = Client.DownloadString("wwwroot\\assets\\template\\Mail\\Login\\ConfirmPassword.html");
                        _message = _message.Replace("{#NAME#}", _dataTable.Rows[0]["Name"].ToString() + (string.IsNullOrEmpty(_dataTable.Rows[0]["Surname"].ToString()) ? "" : " " + _dataTable.Rows[0]["Surname"].ToString()));

                        _Mail.Subject = "[HINO Warranty Claim] Your assword has been reset.";
                    }

                    _message = _message.Replace("{#URL#}", "http://warranty.hinommt.com/Login");
                    //_message = _message.Replace("{#URL#}", "https://localhost:7265/Login");

                    _Mail.From = new MailAddress("noreply@hinothailand.com");
                    _Mail.To.Add(new MailAddress(_dataTable.Rows[0]["Email"].ToString()));


                    _email.Send(_Mail, _message);


                    _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""Recovery is done.""
                    }";

                    return Content(_result, "application/json");
                }
            }

            _result = @"{
                ""status"":""202"",
                ""response"":""FAILED"",
                ""message"": ""Not found this account"" " + @"
            }";

            return Content(_result, "application/json");
        }



    }
}
