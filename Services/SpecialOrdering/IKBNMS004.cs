using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Receive_Process;
using Newtonsoft.Json;

namespace KANBAN.Services.SpecialOrdering
{
    public interface IKBNMS004
    {
        string List_Data(string? Supplier);
        string SupplierDropDown(bool isNew);
        string SupplierChanged(string Supplier);
    }


    public class KBNMS004 : IKBNMS004
    {

        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;


        public KBNMS004
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
        }

        public string List_Data(string? Supplier)
        {
            try
            {

                string sql = $@"SELECT RTRIM(F_Supplier_Code) + '-' + RTRIM(F_Supplier_Plant) AS F_Supplier_Code 
                            ,RTRIM(F_Short_Name) AS F_Short_Name 
                            ,RTRIM(F_Attention) AS F_Attention 
                            ,RTRIM(F_Telephone) AS F_Telephone 
                            ,RTRIM(F_Fax) AS F_Fax 
                            FROM TB_MS_SupplierAttn ";

                if (!string.IsNullOrEmpty(Supplier))
                {
                    sql += $" WHERE F_Supplier_Code + '-' + F_Supplier_Plant = '{Supplier}' ";
                }

                sql += "ORDER BY F_Supplier_Code, F_Short_Name, F_Attention, F_Telephone, F_Fax ";

                var _dt = _FillDT.ExecuteSQL(sql);

                if(_dt.Rows.Count == 0)
                {
                    throw new Exception("No data found");
                }

                return JsonConvert.SerializeObject(_dt);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string SupplierDropDown(bool isNew)
        {
            try
            {
                string now = DateTime.Now.ToString("yyyyMMdd");
                string data = "";

                if (isNew)
                {
                    var dbList = _PPM3Context.T_Supplier_MS.Where(x => x.F_TC_Str.CompareTo(now) <= 0
                        && x.F_TC_End.CompareTo(now) >= 0).Select(x => new
                        {
                            F_Supplier_Code = x.F_supplier_cd + "-" + x.F_Plant_cd,
                        }).OrderBy(x => x.F_Supplier_Code).ToList();

                    if (dbList.Count == 0)
                    {
                        throw new Exception("No data found");
                    }

                    data = JsonConvert.SerializeObject(dbList);
                }
                else
                {
                    string sql = $@"SELECT RTRIM(F_Supplier_Code) + '-' + RTRIM(F_Supplier_Plant) AS F_Supplier_Code 
                            FROM TB_MS_SupplierAttn 
                            ORDER BY F_Supplier_Code ";

                    var _dt = _FillDT.ExecuteSQL(sql);

                    if (_dt.Rows.Count == 0)
                    {
                        throw new Exception("No data found");
                    }

                    data = JsonConvert.SerializeObject(_dt);
                }

                return data;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string SupplierChanged(string Supplier)
        {
            try
            {
                string sql = $@"SELECT RTRIM(F_Short_Name) AS F_Short_Name, F_Attention, F_Telephone, F_Fax 
                            FROM TB_MS_SupplierAttn 
                            WHERE F_Supplier_Code + '-' + F_Supplier_Plant = '{Supplier}'";

                var _dt = _FillDT.ExecuteSQL(sql);

                if (_dt.Rows.Count > 0)
                {
                    return JsonConvert.SerializeObject(_dt);
                }
                else
                {
                    sql = $@"SELECT RTRIM(F_short_name) AS F_Short_Name 
                            FROM T_Supplier_ms 
                            WHERE F_TC_Str <= convert(char(8),getdate(),112) 
                            AND F_TC_End >= convert(char(8),getdate(),112) 
                            AND F_supplier_cd + '-' + F_Plant_cd = '{Supplier}'";

                    _dt = _FillDT.ExecuteSQL(sql);

                    if (_dt.Rows.Count > 0)
                    {
                        return JsonConvert.SerializeObject(_dt);
                    }
                    else
                    {
                        return "";
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }

}
