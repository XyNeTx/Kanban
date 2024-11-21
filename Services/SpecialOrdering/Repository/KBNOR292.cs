using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.SpecialOrdering.Interface;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Services.SpecialOrdering.Repository
{
    public class KBNOR292 : IKBNOR292
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly ISpecialLibs _specialLibs;
        private readonly IAutoMapService _automapService;


        public KBNOR292
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            ISpecialLibs specialLibs,
            IAutoMapService autoMapService
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _specialLibs = specialLibs;
            _automapService = autoMapService;
        }

        public string GetSupplierSurvey(string IssueDate, string? SupplierCD = "")
        {
            try
            {
                string sql = $@"Select F_Supplier_CD,F_Supplier_INT,F_Supplier_Name From V_KBNOR_292R 
                    Where F_Issued_YM = '{IssueDate}' ";
                if (!string.IsNullOrEmpty(SupplierCD))
                {
                    sql = sql + $" And F_Supplier_CD = '{SupplierCD}' ";
                }
                sql = sql + $"and F_Factory_Code  in ('{_BearerClass.Plant}') ";
                sql += "Group by  F_Supplier_CD,F_Supplier_INT,F_Supplier_Name ";
                sql += "Order by F_Supplier_CD ";

                DataTable dt = _FillDT.ExecuteSQL(sql);

                return JsonConvert.SerializeObject(dt);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        


    }
}
