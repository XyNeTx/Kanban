using HINOSystem.Context;
using HINOSystem.Libs;
using System.Data;
using System.Net.Mail;

namespace KANBAN.Libs
{

    public interface IEmailService
    {
        Task SendEmailAsync(string subject, string program, string message, string processDate, string processShift);
    }


    public class EmailService : IEmailService
    {
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly IHttpContextAccessor _http;

        public EmailService(KB3Context kB3Context, FillDataTable FillDT, IHttpContextAccessor http)
        {
            _KB3Context = kB3Context;
            _FillDT = FillDT;
            _http = http;
        }

        public async Task SendEmailAsync(string orderType, string program, string detail,string processDate, string processShift)
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

            mailMessage.To.Add("Sitthiporn_P@hinothailand.com");

            var ccEmails = await CcToEmail(orderType);

            foreach (var ccEmail in ccEmails)
            {
                mailMessage.CC.Add(ccEmail);
            }
            mailMessage.CC.Add("Sitthiporn_P@hinothailand.com");
            mailMessage.CC.Add("Chirawan_C@hinothailand.com");

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task<List<string>> SendToEmail(string orderType)
        {
            string _sql = $"select distinct U.F_User_ID,U.F_User_Name,U.F_Email " +
                $" from TB_REC_HEADER H INNER JOIN [HMMT-APP07].[Price_Approval_Part].dbo.TB_MS_Supplier_Buyer B ON " +
                $" H.F_SUpplier_Code = B.F_Supplier_Code INNER JOIN [HMMT-APP07].[Price_Approval_Part].dbo.TB_CTL_USER U ON " +
                $" B.F_User_Normal = U.F_User_ID  Where H.F_OrderTYpe = '{orderType.Substring(0, 1)}' and F_Status='P' ";

            var _dt = _FillDT.ExecuteSQL(_sql);
            List<string> toEmailList = new List<string>();

            if (_dt.Rows.Count > 0)
            {
                foreach(DataRow dr in _dt.Rows)
                {
                    toEmailList.Add(dr["F_Email"].ToString().Trim());
                }
            }

            return toEmailList;
        }

        public async Task<List<string>> CcToEmail(string orderType)
        {
            string _sql = $"select distinct U1.F_User_ID,U1.F_User_Name,U1.F_Email " +
                $" from TB_REC_HEADER H INNER JOIN [HMMT-APP07].[Price_Approval_Part].dbo.TB_MS_Supplier_Buyer B ON " +
                $" H.F_SUpplier_Code = B.F_Supplier_Code INNER JOIN [HMMT-APP07].[Price_Approval_Part].dbo.TB_CTL_USER U ON " +
                $" B.F_User_Normal = U.F_User_ID INNER JOIN [HMMT-APP07].[Price_Approval_Part].dbo.TB_CTL_USER U1 ON " +
                $" U.F_Team_ID = U1.F_User_ID Where H.F_OrderTYpe = '{orderType.Substring(0,1)}' and F_Status='P' " +
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

    }
}
