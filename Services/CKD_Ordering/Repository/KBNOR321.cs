using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.CKD_Ordering.IRepository;
using Microsoft.Data.SqlClient;
using System.Data;

namespace KANBAN.Services.CKD_Ordering.Repository
{
    public class KBNOR321 : IKBNOR321
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;


        public KBNOR321
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

        public static DateTime DateLogin;

        public async Task Onload(DateTime _loginDate)
        {
            try
            {
                DateLogin = _loginDate;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<List<string>> GetSupplier()
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Plant", _BearerClass.Plant),
                    new SqlParameter("@Store_Code_FROM", "3C"),
                    new SqlParameter("@Store_Code_TO", "3C"),
                };

                var dt = _FillDT.ExecuteSQL_Param($"exec sp_NormalOrder_getSupplier @Plant,@Store_Code_FROM,@Store_Code_TO", parameters);

                List<string> Supplier = new List<string> { "All" };

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Supplier.Add(dt.Rows[i]["F_Supplier"].ToString());
                }

                return Supplier;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<List<string>> GetKanban(string? F_Supplier_Code)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Plant", _BearerClass.Plant),
                    new SqlParameter("@OrderType", "Daily"),
                };

                if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                {
                    parameters[3] = new SqlParameter("@Supplier_Code", F_Supplier_Code.Split("-")[0]);
                    parameters[4] = new SqlParameter("@Supplier_Plant", F_Supplier_Code.Split("-")[1]);
                }

                var dt = _FillDT.ExecuteSQL_Param($"exec sp_NormalOrder_getKanban @Plant,@OrderType,@Supplier_Code,@Supplier_Plant", parameters);

                List<string> Kanban = new List<string> { "All" };

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Kanban.Add(dt.Rows[i]["F_Kanban_No"].ToString());
                }

                return Kanban;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task<List<string>> GetStore(string? F_Supplier_Code)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Plant", _BearerClass.Plant),
                    new SqlParameter("@OrderType", "Daily"),
                };

                if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                {
                    parameters[3] = new SqlParameter("@Supplier_Code", F_Supplier_Code.Split("-")[0]);
                    parameters[4] = new SqlParameter("@Supplier_Plant", F_Supplier_Code.Split("-")[1]);
                }

                var dt = _FillDT.ExecuteSQL_Param($"exec sp_NormalOrder_getStoreCode @Plant,@OrderType,@Supplier_Code,@Supplier_Plant", parameters);

                List<string> Store = new List<string> { "All" };

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Store.Add(dt.Rows[i]["F_Store_CD"].ToString());
                }

                return Store;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task<List<string>> GetPartNo(string? F_Supplier_Code, string? F_Store_Code)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Plant", _BearerClass.Plant),
                    new SqlParameter("@OrderType", "Daily"),
                };

                if (!string.IsNullOrWhiteSpace(F_Supplier_Code))
                {
                    parameters[3] = new SqlParameter("@Supplier_Code", F_Supplier_Code.Split("-")[0]);
                    parameters[4] = new SqlParameter("@Supplier_Plant", F_Supplier_Code.Split("-")[1]);
                }

                if (!string.IsNullOrWhiteSpace(F_Store_Code) && F_Store_Code != "All")
                {
                    parameters[5] = new SqlParameter("@Store_Code_FROM", F_Store_Code);
                    parameters[6] = new SqlParameter("@Store_Code_TO", F_Store_Code);
                }

                var dt = _FillDT.ExecuteSQL_Param($"exec sp_NormalOrder_getPartNo @Plant,@OrderType,@Supplier_Code,@Supplier_Plant,@Store_Code_FROM,@Store_Code_TO", parameters);

                List<string> Part = new List<string> { "All" };

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Part.Add(dt.Rows[i]["F_Part_No"].ToString());
                }

                return Part;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        private async Task Find_StartEnd_Date(string action, string F_Supplier_Code)
        {
            try
            {
                DataTable DT;
                if (action == "Preview")
                {
                    SqlParameter[] sqlParams = new SqlParameter[]
                    {
                        new SqlParameter("@Plant",_BearerClass.Plant),
                        new SqlParameter("@Supplier_Code",F_Supplier_Code.Split("-")[0]),
                        new SqlParameter("@Supplier_Plant",F_Supplier_Code.Split("-")[1]),
                        new SqlParameter("@Store_Code","3C"),
                        new SqlParameter("@Date",DateLogin.ToString("yyyyMMdd"))
                    };
                }
                DT = _FillDT.ExecuteStoreSQL("[CKD_Inhouse].[sp_NumberOfDayToPreview]");
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}
