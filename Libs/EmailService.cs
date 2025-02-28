using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Services;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Mail;

namespace KANBAN.Libs
{

    public interface IEmailService
    {
        Task SendEmailAsync(string subject, string program, string message, string processDate, string processShift);
        Task SendEmailUnlock(string orderType, string program, string detail, string processDate, string processShift, string nSupplier);
        Task SendEmailToPA2(string ProdYM, string nRev, string nVer);
        Task SendEmailSurvey(string? Type = "", string? sumSurvey = "", string? ProcessShift = "");
        Task SendEmailApprover(string sPDS, string approver);
    }


    public class EmailService : IEmailService
    {
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly IHttpContextAccessor _http;
        private readonly BearerClass _bearerClass;
        private readonly SerilogLibs _log;

        public EmailService(KB3Context kB3Context, FillDataTable FillDT
            , IHttpContextAccessor http, BearerClass bearerClass, SerilogLibs log)
        {
            _KB3Context = kB3Context;
            _FillDT = FillDT;
            _http = http;
            _bearerClass = bearerClass;
            _log = log;
        }

        public async Task SendEmailAsync(string orderType, string program, string detail, string processDate, string processShift)
        {
            try
            {
                var smtpClient = new SmtpClient("156.71.5.8");
                string plantCode = _http.HttpContext.Request.Cookies["plantCode"];

                string Message = $"Dear All Concern,<br/><br/>" +
                    $"Auto Process for warning not have unit price.<br/>" +
                    $"System : Hino Kanban for Fac.{plantCode} <br/>" +
                    $"Type : {orderType} : {program} <br/>" +
                    $"Process Date : {processDate} <br/>" +
                    $"For detail please see at below :-<br/><br/>" +
                    $"{detail}" +
                    $"Best Regards,<br/><br/>" +
                    $"This E-Mail auto sending by Hino Kanban for Fac.{plantCode} (If you have any question please contact to PAP) <br/><br/>";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("AlertPrice@hinothailand.com"),
                    Subject = $"Price Not Found : Hino Kanban for Fac.{plantCode} ({orderType}) Process Date : {processDate} Shift : {processShift}",
                    Body = Message,
                    IsBodyHtml = true,
                    Priority = MailPriority.High,
                };

                var toEmails = await SendToEmail(orderType);

                foreach (var toEmail in toEmails)
                {
                    mailMessage.To.Add(toEmail);
                }

                var ccEmails = await CcToEmail(orderType);

                foreach (var ccEmail in ccEmails)
                {
                    mailMessage.CC.Add(ccEmail);
                }
                mailMessage.Bcc.Add("Sitthiporn_P@hinothailand.com");
                mailMessage.Bcc.Add("Chirawan_C@hinothailand.com");

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("SendEmailAsync Error " + ex.Message);
            }
        }

        public async Task<List<string>> SendToEmail(string orderType)
        {
            try
            {
                string _sql = $"select distinct U.F_User_ID,U.F_User_Name,U.F_Email " +
                    $" from TB_REC_HEADER H INNER JOIN [HMMT-APP07].[Price_Approval_Part].dbo.TB_MS_Supplier_Buyer B ON " +
                    $" H.F_SUpplier_Code = B.F_Supplier_Code INNER JOIN [HMMT-APP07].[Price_Approval_Part].dbo.TB_CTL_USER U ON " +
                    $" B.F_User_Normal = U.F_User_ID  Where H.F_OrderTYpe = '{orderType.Substring(0, 1)}' and F_Status='P' ";

                var _dt = _FillDT.ExecuteSQL(_sql);
                List<string> toEmailList = new List<string>();

                if (_dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in _dt.Rows)
                    {
                        toEmailList.Add(dr["F_Email"].ToString().Trim());
                    }
                }

                return toEmailList;
            }
            catch (Exception ex)
            {
                throw new Exception("SendToEmail Error " + ex.Message);
            }
        }

        public async Task<List<string>> CcToEmail(string orderType)
        {
            try
            {
                string _sql = $"select distinct U1.F_User_ID,U1.F_User_Name,U1.F_Email " +
                    $" from TB_REC_HEADER H INNER JOIN [HMMT-APP07].[Price_Approval_Part].dbo.TB_MS_Supplier_Buyer B ON " +
                    $" H.F_SUpplier_Code = B.F_Supplier_Code INNER JOIN [HMMT-APP07].[Price_Approval_Part].dbo.TB_CTL_USER U ON " +
                    $" B.F_User_Normal = U.F_User_ID INNER JOIN [HMMT-APP07].[Price_Approval_Part].dbo.TB_CTL_USER U1 ON " +
                    $" U.F_Team_ID = U1.F_User_ID Where H.F_OrderTYpe = '{orderType.Substring(0, 1)}' and F_Status='P' " +
                    $" Union all Select distinct F_User_ID,F_User_name,F_EMAIL from [New_Kanban].dbo.TB_USER Where Isnull(F_Email,'') <>'' ";

                var _dt = _FillDT.ExecuteSQL(_sql);
                List<string> toEmailList = new List<string>();

                if (_dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in _dt.Rows)
                    {
                        toEmailList.Add(dr["F_Email"].ToString().Trim());
                    }
                }

                return toEmailList;
            }
            catch (Exception ex)
            {
                throw new Exception("CcToEmail Error " + ex.Message);
            }
        }



        public async Task SendEmailUnlock(string orderType, string program, string detail, string processDate, string processShift, string nSupplier)
        {
            try
            {
                var smtpClient = new SmtpClient("156.71.5.8");
                string plantCode = _http.HttpContext.Request.Cookies["plantCode"];

                string Message = $"Dear All Concern,<br/><br/>" +
                    $"Auto Process for inform about pds have price zero interface into E-Procurement System (Case Urgently Data). <br/>" +
                    $"System : Hino Kanban for Fac.{plantCode} <br/>" +
                    $"Type : {orderType} : {program} <br/>" +
                    $"Process Date : {processDate} <br/>" +
                    $"For detail please see at below :-<br/><br/>" +
                    $"{detail}" +
                    $"Best Regards,<br/><br/>" +
                    $"This E-Mail auto sending by Hino Kanban for Fac.{plantCode} (If you have any question please contact to PAP) <br/><br/>";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("AlertPrice@hinothailand.com"),
                    Subject = $"Un-Lock PDS : Hino Kanban for IMV ({orderType}) Process Date : {processDate} : Shift : {processShift}",
                    Body = Message,
                    IsBodyHtml = true,
                    Priority = MailPriority.High,
                };

                var toEmails = await SendToEmailUnlock(nSupplier);

                foreach (var toEmail in toEmails)
                {
                    List<string> _checkDupe = new List<string>();
                    if (_checkDupe.Contains(toEmail))
                    {
                        continue;
                    }
                    else
                    {
                        mailMessage.To.Add(toEmail);
                        _checkDupe.Add(toEmail);
                    }
                }

                //mailMessage.To.Add("Sitthiporn_P@hinothailand.com");

                var ccEmails = await CcToEmailUnlock(nSupplier);

                foreach (var ccEmail in ccEmails)
                {
                    List<string> _checkDupe = new List<string>();
                    if (_checkDupe.Contains(ccEmail))
                    {
                        continue;
                    }
                    else
                    {
                        mailMessage.CC.Add(ccEmail);
                        _checkDupe.Add(ccEmail);
                    }
                }
                mailMessage.Bcc.Add("Sitthiporn_P@hinothailand.com");
                mailMessage.Bcc.Add("Chirawan_C@hinothailand.com");

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("SendEmailUnlock Error " + ex.Message);
            }
        }

        public async Task<List<string>> SendToEmailUnlock(string nSupplier)
        {
            try
            {
                string _sql = $"Select U.F_User_ID,U.F_User_Name,U.F_Email " +
                    $" from [HMMT-APP07].[Price_Approval_Part].dbo.TB_MS_Supplier_Buyer B " +
                    $" INNER JOIN [HMMT-APP07].[Price_Approval_Part].dbo.TB_CTL_USER U ON B.F_User_Normal = U.F_User_ID " +
                    $" Where B.F_SUpplier_Code in ('{nSupplier}') ";

                var _dt = _FillDT.ExecuteSQL(_sql);
                List<string> toEmailList = new List<string>();

                if (_dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in _dt.Rows)
                    {
                        toEmailList.Add(dr["F_Email"].ToString().Trim());
                    }
                }

                return toEmailList;
            }
            catch (Exception ex)
            {
                throw new Exception("SendToEmailUnlock Error " + ex.Message);
            }
        }

        public async Task<List<string>> CcToEmailUnlock(string nSupplier)
        {
            try
            {
                string _sql = $"select distinct U1.F_User_ID,U1.F_User_Name,U1.F_Email " +
                    $" from [HMMT-APP07].[Price_Approval_Part].dbo.TB_MS_Supplier_Buyer B " +
                    $" INNER JOIN [HMMT-APP07].[Price_Approval_Part].dbo.TB_CTL_USER U ON " +
                    $" B.F_User_Normal = U.F_User_ID INNER JOIN [HMMT-APP07].[Price_Approval_Part].dbo.TB_CTL_USER U1 ON  " +
                    $" U.F_Team_ID = U1.F_User_ID Where B.F_SUpplier_Code in ('{nSupplier}') Union all " +
                    $" Select distinct F_User_ID,F_User_name,F_EMAIL from [New_Kanban].dbo.TB_USER Where Isnull(F_Email,'') <>'' ";

                var _dt = _FillDT.ExecuteSQL(_sql);
                List<string> toEmailList = new List<string>();

                if (_dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in _dt.Rows)
                    {
                        toEmailList.Add(dr["F_Email"].ToString().Trim());
                    }
                }

                return toEmailList;
            }
            catch (Exception ex)
            {
                throw new Exception("CcToEmailUnlock Error " + ex.Message);
            }
        }

        public async Task SendEmailToPA2(string ProdYM, string nRev, string nVer)
        {
            try
            {
                var smtpClient = new SmtpClient("156.71.5.8");
                string plantCode = _http.HttpContext.Request.Cookies["plantCode"];

                string Message = $"Dear All Concern, <br/><br/>" +
                    $"Auto Process for inform about PA1 interface forecast already finished. <br/>" +
                    $"Production Month : {ProdYM} <br/>" +
                    $"Revision : {nRev} <br/>" +
                    $"Type Forecast : {nVer} <br/><br/>" +
                    $"Best Regards, <br/>" +
                    $"This E-Mail auto sending by Hino Kanban F.{plantCode}. (If you have any question please contact to PA1) <br/>";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("InterfaceForecast@hinothailand.com"),
                    Subject = $"Interface Forecast for Production Month : {ProdYM} Rev : {nRev} ({nVer})",
                    Body = Message,
                    IsBodyHtml = true,
                    Priority = MailPriority.Normal,
                };

                var toEmails = GetPA2Email();
                foreach (var toEmail in toEmails)
                {
                    mailMessage.To.Add(toEmail);
                }

                var ccEmails = GetPA2CCEmail();
                foreach (var ccEmail in ccEmails)
                {
                    mailMessage.CC.Add(ccEmail);
                }

                mailMessage.Bcc.Add("Sitthiporn_P@hinothailand.com");
                mailMessage.Bcc.Add("Chirawan_C@hinothailand.com");

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("SendEmailToPA2 Error " + ex.Message);
            }
        }

        public List<string> GetPA2Email()
        {
            try
            {
                string _sql = "Select distinct F_User_ID,F_User_name,F_EMAIL from [New_Kanban_F2].dbo.TB_USER Where Isnull(F_Email,'') <>'' and F_Remark <> 'IT '";

                var _dt = _FillDT.ExecuteSQL(_sql);
                List<string> toEmailList = new List<string>();

                if (_dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in _dt.Rows)
                    {
                        toEmailList.Add(dr["F_Email"].ToString().Trim());
                    }
                }

                return toEmailList;
            }
            catch (Exception ex)
            {
                throw new Exception("GetPA2Email Error " + ex.Message);
            }
        }

        public List<string> GetPA2CCEmail()
        {
            try
            {
                string _sql = "Select F_User_ID,F_User_name,F_EMAIL from TB_USER Where F_EMail <> ''";

                var _dt = _FillDT.ExecuteSQL(_sql);
                List<string> toEmailList = new List<string>();

                if (_dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in _dt.Rows)
                    {
                        toEmailList.Add(dr["F_Email"].ToString().Trim());
                    }
                }

                return toEmailList;
            }
            catch (Exception ex)
            {
                throw new Exception("GetPA2CCEmail Error " + ex.Message);
            }
        }

        public async Task SendEmailSurvey(string? Type = "", string? sumSurvey = "", string? ProcessShift = "")
        {
            string sBody = "";
            string strSurveyDoc = "";
            string strSupplierCD = "";
            string strShortName = "";
            string strSupplierName = "";
            string strEmail = "";
            string strTeamEmail = "";
            string strInChargeEmail = "";
            string sSubject = "";
            string SupCode = "";
            string SupName = "";
            string PartNo = "";
            string PartName = "";
            string Delivery = "";
            string sProcess = "";
            string sPDS = "";

            int i, j = 0, k = 0;

            try
            {
                var smtpClient = new SmtpClient("156.71.5.8");
                if (sumSurvey != "")
                {
                    string sumEmail = "";
                    string _sql = $"Select F_User_name,F_Email From  TB_MS_Operator Where F_User_ID ='{_bearerClass.UserCode}'";

                    var _dt = _FillDT.ExecuteSQL(_sql);

                    if (_dt.Rows.Count > 0)
                    {
                        strInChargeEmail = _dt.Rows[0]["F_Email"].ToString();
                    }

                    string Approver = "";

                    _sql = $"Select  F_Survey_Doc,Rtrim(F_Supplier_CD)+'-'+Rtrim(F_Supplier_Plant) AS F_Supplier_CD,F_Short_Name,F_Name " +
                        $",B.F_User_Special, B.F_Email_spc As F_Email ,B.F_Team_Email_spc As  F_Team_Email " +
                        $"From fnSURVEYNOTPRICE_SPC()  P inner join VW_supplierforbuyerprice B " +
                        $"on Rtrim(P.F_Supplier_CD) = B.F_Supplier_code  " +
                        $"Where F_Survey_Doc in ({sumSurvey.Substring(2, sumSurvey.Length - 2)}) " +
                        $"Group by  F_Survey_Doc,Rtrim(F_Supplier_CD),Rtrim(F_Supplier_Plant),F_Short_Name,F_Name,B.F_User_Special, B.F_Team_Email_spc,B.F_Email_spc " +
                        $"Order by F_Survey_Doc ";

                    _dt = _FillDT.ExecuteSQL(_sql);

                    if (_dt.Rows.Count > 0)
                    {
                        for (i = 0; i < _dt.Rows.Count; i++)
                        {
                            strSurveyDoc = _dt.Rows[i]["F_Survey_Doc"].ToString();
                            strSupplierCD = _dt.Rows[i]["F_Supplier_CD"].ToString();
                            strShortName = _dt.Rows[i]["F_Short_Name"].ToString();
                            strSupplierName = _dt.Rows[i]["F_Name"].ToString();
                            strEmail = _dt.Rows[i]["F_Email"].ToString();
                            strTeamEmail = _dt.Rows[i]["F_Team_Email"].ToString();

                            MailMessage mailMessage = new MailMessage
                            {
                                From = new MailAddress("System_Notification@hinothailand.com"),
                                Subject = $"Price Not Found : Hino Kanban for Fac 3 (Special Order : {ProcessShift} ) Process Date : {DateTime.Now.ToString("dd/MM/yyyy")}",
                                To = { strEmail },
                                CC = { strTeamEmail, strInChargeEmail },
                                IsBodyHtml = true,
                                Bcc = { "Sitthiporn_P@hinothailand.com", "Chirawan_C@hinothailand.com" }
                            };

                            if (strEmail != "")
                            {
                                sBody = "<span style='font-family:Arial;font-size:12px;'>Dear Sirs,</span><br><br>";
                                sBody += "<span style='font-family:Arial;font-size:12px;'>Auto Process for warning not have unit price.</span><br> ";
                                sBody += "<span style='font-family:Arial;font-size:12px;'>System : Hino Kanban for IMV (Special Order) </span><br> ";
                                sBody += $"<span style='font-family:Arial;font-size:12px;'>Process Date : <font color='red'>{DateTime.Now.ToString("dd/MM/yyyy")}</font></span><br>";
                                sBody += $"<span style='font-family:Arial;font-size:12px;'>Process : {Type}</span><br>";
                                sBody += $"<span style='font-family:Arial;font-size:12px;'>For Detail please see below </span><br><br>";
                                sBody += $"<span style='font-family:Arial;font-size:12px;'>Survey Doc. : {strSurveyDoc}</span><br>";

                                _sql = $"Select  F_Survey_Doc,Rtrim(F_Supplier_CD)+'-'+Rtrim(F_Supplier_Plant) AS F_Supplier_CD,F_Short_Name,F_Name " +
                                    $" , Rtrim(F_Part_no)+'-'+Rtrim(F_Ruibetsu) as F_Part_No, F_Part_Name, F_Delivery_Date " +
                                    $"From fnSURVEYNOTPRICE_SPC() " +
                                    $"Where F_Survey_Doc in ('{strSurveyDoc}') " +
                                    $"Group by  F_Survey_Doc,F_Supplier_CD,F_Supplier_Plant,F_Short_Name,F_Name, Rtrim(F_Part_no)+'-'+Rtrim(F_Ruibetsu), F_Part_Name, F_Delivery_Date " +
                                    $"Order by F_Survey_Doc,F_Supplier_CD,F_Supplier_Plant ";

                                var DTM = _FillDT.ExecuteSQL(_sql);
                                if (DTM.Rows.Count > 0)
                                {
                                    for (i = 0; i < DTM.Rows.Count; i++)
                                    {
                                        j++;
                                        SupCode = DTM.Rows[i]["F_Supplier_CD"].ToString().Trim();
                                        SupName = DTM.Rows[i]["F_Short_Name"].ToString().Trim() + " : " + DTM.Rows[i]["F_Name"].ToString().Trim();
                                        PartNo = DTM.Rows[i]["F_Part_No"].ToString().Trim();
                                        PartName = DTM.Rows[i]["F_Part_Name"].ToString().Trim();
                                        Delivery = DTM.Rows[i]["F_Delivery_Date"].ToString().Trim();

                                        sBody += $"<span style='font-family:Arial;font-size:12px;'> {j}. Supplier :<font color='blue'> {SupCode} </font>, Name : <font color='blue'> {SupName}</font>, PartNo : <font color='blue'> {PartNo} </font>, Part Name :<font color='blue'> {PartName}</font>, Delivery Date :<font color='blue'>{Delivery}</font></span><br>";
                                    }
                                }
                                sBody += "<br><br><span style='font-family:Arial;font-size:12px;color:red;text-decoration:italic;'>This E-Mail auto sending by Hino Kanban for IMV (Special Order).</span>";

                                mailMessage.Body = sBody;
                                mailMessage.Priority = MailPriority.High;
                                mailMessage.Bcc.Add("Sitthiporn_P@hinothailand.com");
                                mailMessage.Bcc.Add("Chirawan_C@hinothailand.com");
                                smtpClient.Send(mailMessage);

                                if (Type?.ToLower() == "generate pds")
                                {
                                    string sql = $"Update TB_Survey_Detail Set F_Price_Flg = 1 Where  F_Survey_Doc in ('{strSurveyDoc}') ";
                                    await _KB3Context.Database.ExecuteSqlRawAsync(sql);
                                    _log.WriteLogMsg("Unlock Price " + sql);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SendEmailSurvey Error " + ex.Message);
            }

        }

        public async Task SendEmailApprover(string sPDS, string approver)
        {
            try
            {
                var ObjUserName = await _KB3Context.User.Where(x => x.Code.Trim() == _bearerClass.UserCode).Select(x => new
                {
                    Name = (x.Title_ID == 1 ? "Ms. " : x.Title_ID == 3 ? "Mr. " : "Mrs. ") + x.Name + " " + x.Surname,
                }).FirstOrDefaultAsync();

                string UserName = ObjUserName!.Name;

                MailMessage myMail = new MailMessage();
                var dt = _FillDT.ExecuteSQL($"Select F_User_name,F_Email From  TB_MS_Operator Where F_User_ID ='{_bearerClass.UserCode}'");

                var dtSpcApp = _FillDT.ExecuteSQL($"Select F_Email From  TB_MS_SpcApprover Where F_User_ID ='{approver}'");
                var smtpClient = new SmtpClient("156.71.5.8");

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(dt.Rows[0]["F_Email"].ToString().Trim()),
                    To = { dtSpcApp.Rows[0]["F_Email"].ToString().Trim() },
                    Subject = "Please approve pds data",
                    Body = @$"Dear All Concern, <br/><br/>
                        Auto generate please approve pds data :- <br/>
                        {sPDS}<br/>
                        Best Regards, <br/>
                        {UserName}<br/>",
                    IsBodyHtml = true,
                    Priority = MailPriority.High,
                    Bcc = { "Sitthiporn_P@hinothailand.com", "Chirawan_C@hinothailand.com" }
                };
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Port = 25;

                await smtpClient.SendMailAsync(mailMessage);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    throw;
                }
                throw new CustomHttpException(500, "SendEmailApprover Error " + ex.Message);
            }
        }
    }
}
