using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.PPM3;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Master.IRepository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Services.Master.Repository
{
    public class KBNMS016 : IKBNMS016
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;


        public KBNMS016
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            IAutoMapService autoMapService
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _automapService = autoMapService;
        }

        public static string strDateNow = DateTime.Now.ToString("yyyyMMdd");


        public async Task<string> List_Data(string? F_Supplier_Cd, string? F_Kanban_No, string? F_Part_No, string? F_Store_Cd, string? F_Supplier_Plant, string? F_Ruibetsu, string? F_Group)
        {
            try
            {
                string ppmCon = _FillDT.ppmConnect();

                string sql = $@"SELECT RTRIM(P.F_Group) AS F_Group 
                    ,SUBSTRING(P.F_Start_Date,7,2)+'/'+SUBSTRING(P.F_Start_Date,5,2)+'/'+SUBSTRING(P.F_Start_Date,1,4) AS F_Start_Date 
                    ,SUBSTRING(P.F_End_Date,7,2)+'/'+SUBSTRING(P.F_End_Date,5,2)+'/'+SUBSTRING(P.F_End_Date,1,4) AS F_End_Date 
                    ,RTRIM(P.F_Supplier_Cd)+'-'+RTRIM(P.F_Supplier_Plant) AS F_Supplier_Cd 
                    ,RTRIM(S.F_name) AS F_name 
                    ,RIGHT('00000'+ CONVERT(VARCHAR,S.F_Cycle_A),2) 
                    +'-'+RIGHT('00000'+ CONVERT(VARCHAR,S.F_cycle_B),2) 
                    +'-'+RIGHT('00000'+ CONVERT(VARCHAR,S.F_cycle_C),2) AS F_Cycle 
                    ,RTRIM(P.F_Kanban_No) AS F_Kanban_No 
                    ,RTRIM(C.F_Part_nm) AS F_Part_nm 
                    ,RTRIM(P.F_Store_Cd) AS F_Store_Code 
                    ,RTRIM(P.F_Part_No)+'-'+RTRIM(P.F_Ruibetsu) AS F_Part_No 
                    ,RTRIM(P.F_Qty) AS F_Qty 
                    FROM TB_MS_PairOrder P INNER JOIN {ppmCon}.[dbo].[T_Construction] C 
                    ON P.F_Part_No = C.F_Part_no collate Thai_CI_AS 
                    AND P.F_Ruibetsu = C. F_Ruibetsu collate Thai_CI_AS 
                    AND P.F_Store_Cd = C.F_Store_cd collate Thai_CI_AS 
                    INNER JOIN {ppmCon}.[dbo].[T_Supplier_ms] S 
                    ON P.F_Supplier_Cd = S.F_supplier_cd COLLATE SQL_Latin1_General_CP1_CI_AI 
                    AND P.F_Supplier_Plant = S.F_Plant_cd COLLATE SQL_Latin1_General_CP1_CI_AI 
                    AND P.F_Store_Cd = S.F_Store_cd collate Thai_CI_AS 
                    AND C.F_Local_Str <= convert(char(8),getdate(),112) 
                    AND C.F_Local_End >= convert(char(8),getdate(),112) 
                    WHERE P.F_Plant = '{_BearerClass.Plant}' ";

                if (!string.IsNullOrWhiteSpace(F_Supplier_Cd))
                {
                    sql += $"AND P.F_Supplier_Cd = '{F_Supplier_Cd}' AND P.F_Supplier_Plant = '{F_Supplier_Plant}' ";
                }

                if (!string.IsNullOrWhiteSpace(F_Kanban_No))
                {
                    sql += $"AND P.F_Kanban_No = '{F_Kanban_No}'";
                }

                if (!string.IsNullOrWhiteSpace(F_Part_No))
                {
                    sql += $"AND P.F_Part_No = '{F_Part_No}' AND P.F_Ruibetsu = '{F_Ruibetsu}' ";
                }

                if (!string.IsNullOrWhiteSpace(F_Store_Cd))
                {
                    sql += $"AND P.F_Store_Cd = '{F_Store_Cd}' ";
                }

                if (!string.IsNullOrWhiteSpace(F_Group))
                {
                    sql += $"AND P.F_Group = '{F_Group}' ";
                }

                sql += $@"GROUP BY RTRIM(P.F_Group),SUBSTRING(P.F_Start_Date,7,2)+'/'+SUBSTRING(P.F_Start_Date,5,2)+'/'+SUBSTRING(P.F_Start_Date,1,4) 
                    ,SUBSTRING(P.F_End_Date,7,2)+'/'+SUBSTRING(P.F_End_Date,5,2)+'/'+SUBSTRING(P.F_End_Date,1,4) 
                    ,RTRIM(P.F_Supplier_Cd)+'-'+RTRIM(P.F_Supplier_Plant),RTRIM(S.F_name) 
                    ,RIGHT('00000'+ CONVERT(VARCHAR,S.F_Cycle_A),2) +'-'+RIGHT('00000'+ CONVERT(VARCHAR,S.F_cycle_B),2) +'-'+RIGHT('00000'+ CONVERT(VARCHAR,S.F_cycle_C),2) 
                    ,RTRIM(P.F_Kanban_No),RTRIM(C.F_Part_nm),RTRIM(P.F_Store_Cd),RTRIM(P.F_Part_No)+'-'+RTRIM(P.F_Ruibetsu) 
                    ,RTRIM(P.F_Qty) 
                    ORDER BY F_Group,F_Supplier_Cd,F_Cycle,F_Kanban_No,F_Store_Code,F_Part_No,F_Qty ";

                var dt = _FillDT.ExecuteSQL(sql);

                return JsonConvert.SerializeObject(dt);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<List<T_Construction>> GetDropDownInq(string? F_Supplier_Cd, string? F_Kanban_No, string? F_Part_No, string? F_Store_Cd, string? F_Supplier_Plant, string? F_Ruibetsu)
        {
            try
            {
                var data = await _PPM3Context.T_Construction.AsNoTracking()
                    .Where(x => x.F_Local_Str.CompareTo(strDateNow) <= 0
                    && x.F_Local_End.CompareTo(strDateNow) >= 0
                    && x.F_Store_cd.StartsWith(_BearerClass.Plant)).ToListAsync();

                if (!string.IsNullOrWhiteSpace(F_Supplier_Cd))
                {
                    data = data.Where(x => x.F_supplier_cd == F_Supplier_Cd
                        && x.F_plant == F_Supplier_Plant[0]).ToList();
                }

                if (!string.IsNullOrWhiteSpace(F_Part_No))
                {
                    data = data.Where(x => x.F_Part_no == F_Part_No
                        && x.F_Ruibetsu == F_Ruibetsu).ToList();
                }

                if (!string.IsNullOrWhiteSpace(F_Store_Cd))
                {
                    data = data.Where(x => x.F_Store_cd == F_Store_Cd).ToList();
                }

                if (!string.IsNullOrWhiteSpace(F_Kanban_No))
                {
                    data = data.Where(x => x.F_Sebango == F_Kanban_No.Substring(1, 3)).ToList();
                }

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<List<TB_MS_PairOrder>> GetDropDownNew(string? F_Supplier_Cd, string? F_Kanban_No, string? F_Part_No, string? F_Store_Cd, string? F_Supplier_Plant, string? F_Ruibetsu, string? F_Group)
        {
            try
            {
                var data = await _kbContext.TB_MS_PairOrder.AsNoTracking()
                    .Where(x => x.F_Plant == _BearerClass.Plant).ToListAsync();

                if (!string.IsNullOrWhiteSpace(F_Supplier_Cd))
                {
                    data = data.Where(x => x.F_Supplier_Cd == F_Supplier_Cd
                        && x.F_Supplier_Plant == F_Supplier_Plant).ToList();
                }

                if (!string.IsNullOrWhiteSpace(F_Part_No))
                {
                    data = data.Where(x => x.F_Part_No == F_Part_No
                        && x.F_Ruibetsu == F_Ruibetsu).ToList();
                }

                if (!string.IsNullOrWhiteSpace(F_Store_Cd))
                {
                    data = data.Where(x => x.F_Store_Cd == F_Store_Cd).ToList();
                }

                if (!string.IsNullOrWhiteSpace(F_Kanban_No))
                {
                    data = data.Where(x => x.F_Kanban_No == F_Kanban_No).ToList();
                }

                if (!string.IsNullOrWhiteSpace(F_Group))
                {
                    data = data.Where(x => x.F_Group == F_Group).ToList();
                }

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}
