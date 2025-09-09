using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.LogisticCondition;
using KANBAN.Services.Logistical.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace KANBAN.Services.Logistical.Repository
{
    public class KBNLC150 : IKBNLC150
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public KBNLC150
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public static string publicYM = "";
        public static string publicRev = "";

        public static int processBar = 0;

        public string ShowRevision(string YM)
        {
            try
            {
                string _sql = $"SELECT Distinct  F_Plant, F_YM, F_Rev " +
                    $" FROM  TB_Import_Delivery " +
                    $"WHERE  F_YM='{YM}' " +
                    $"AND  F_Plant='{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value}' " +
                    $"Order by F_Rev ";

                publicYM = YM;

                var dt = _FillDT.ExecuteSQL(_sql);
                if (dt.Rows.Count > 0)
                {
                    dt.Rows[dt.Rows.Count - 1]["F_Rev"] = int.Parse(dt.Rows[dt.Rows.Count - 1]["F_Rev"].ToString()) + 1;
                    publicRev = dt.Rows[dt.Rows.Count - 1]["F_Rev"].ToString();
                    return JsonConvert.SerializeObject(publicRev);
                }

                //List<object> list = new List<object>();
                //object obj = new
                //{
                //    F_Plant = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value,
                //    F_YM = YM,
                //    F_Rev = 0
                //};
                //list.Add(obj);
                publicRev = "0";

                return JsonConvert.SerializeObject(publicRev);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> Import(List<VM_TB_Import_Delivery> listObj)
        {
            processBar = 0;
            try
            {
                string _sql = "";

                await _kbContext.Database.ExecuteSqlRawAsync($"Delete From KBNLC_150 Where F_Import_By='{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}'");

                await _kbContext.Database.ExecuteSqlRawAsync($"Delete From TB_Import_Error Where" +
                    $" F_Update_BY='{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}'AND F_Type='IDT' ");

                int i = 2;

                processBar = 10;

                foreach (var obj in listObj)
                {
                    if (obj.F_YM != publicYM)
                    {
                        _sql = $"Insert into TB_IMPORT_ERROR(F_PDS_CD, F_Row, F_Field, F_Remark, F_Update_By, F_Update_Date, F_Type)values(" +
                            $"'F_YM','{i}','{obj.F_YM}','ProductinYM<>Screen', " +
                            $"'{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}',getdate(),'IDT')";

                        await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                    }

                    processBar = 20;

                    _sql = $"Select Distinct F_short_Logistic, F_short_name,F_Supplier_CD, F_Supplier_Plant  From TB_MS_Matching_Supplier " +
                        $" Where F_short_Logistic='{obj.F_Short_Logistic}' ";

                    var dt = _FillDT.ExecuteSQL(_sql);
                    if (dt.Rows.Count == 0)
                    {
                        _sql = $"Insert into TB_IMPORT_ERROR(F_PDS_CD, F_Row, F_Field, F_Remark, F_Update_By, F_Update_Date, F_Type)values(" +
                            $"'F_Short_Logistic','{i}','{obj.F_Short_Logistic}','Supplier not found', " +
                            $"'{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}',getdate(),'IDT')";

                        await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                    }

                    i++;
                }

                processBar = 40;

                _sql = $"SELECT F_Plant, F_YM, F_Rev, F_Delivery_Trip, F_Dock_Cd, F_short_Logistic, F_Import_By, F_Error " +
                    $" FROM VW_CheckImport_Delivery  Where F_YM='{publicYM}' and F_Rev='{publicRev}' " +
                    $" and F_Plant='{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value}'";

                var dt2 = _FillDT.ExecuteSQL(_sql);
                string _error = "";

                if (dt2.Rows.Count > 0)
                {
                    for (int j = 0; j < dt2.Rows.Count; j++)
                    {
                        _error += dt2.Rows[j]["F_Short_Logistic"].ToString() + " - " + dt2.Rows[j]["F_Error"].ToString() + ",";
                    }
                }
                if (_error != "")
                {
                    throw new Exception("พบข้อมูลผิดปรกติ. => " + _error);
                }

                processBar = 50;

                _sql = $"Select * from TB_IMPORT_ERROR Where F_Update_BY='{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}' AND F_Type='IDT'";

                var dt3 = _FillDT.ExecuteSQL(_sql);
                if (dt3.Rows.Count > 0)
                {
                    _log.WriteLogMsg("Import Delivery Time Table Error | SQL : " + _sql);
                    throw new Exception("Import Delivery Time Table Error");
                }

                foreach (var obj in listObj)
                {
                    _sql = $"Select Distinct F_short_Logistic, F_short_name,F_Supplier_CD, F_Supplier_Plant  From TB_MS_Matching_Supplier " +
                        $" Where F_short_Logistic='{obj.F_Short_Logistic}' ";

                    var dt4 = _FillDT.ExecuteSQL(_sql);

                    if (dt4.Rows.Count > 0)
                    {
                        obj.F_Supplier_Code = dt4.Rows[0]["F_Supplier_CD"].ToString();
                        obj.F_Supplier_Plant = dt4.Rows[0]["F_Supplier_Plant"].ToString();
                        obj.F_Supplier_Name = dt4.Rows[0]["F_short_name"].ToString();
                        obj.F_Import_By = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value;
                        obj.F_Import_Date = DateTime.Now;
                        obj.F_Remark = obj.F_Remark == null ? "" : obj.F_Remark;
                    }

                    KBNLC_150 kBNLC_150 = JsonConvert.DeserializeObject<KBNLC_150>(JsonConvert.SerializeObject(obj));

                    if (kBNLC_150 != null) _kbContext.KBNLC_150.Add(kBNLC_150);
                }

                processBar = 70;

                await _kbContext.SaveChangesAsync();

                _sql = $"INSERT INTO TB_Import_Delivery(F_Plant, F_YM, F_Rev, F_Delivery_Trip, F_Dock_Cd, F_Truck_Card, F_Tran_Type, F_Wheel, F_Supplier_Code, F_Supplier_Plant, F_short_Logistic, " +
                    $"F_Supplier_Name, F_Arrival_Sup, F_Depart_Sup, F_Arrival_HMMT, F_Depart_HMMT, F_Cycle_Time, F_Remark, F_Import_By, F_Import_Date,F_Flag) " +
                    $" SELECT     F_Plant, F_YM, F_Rev, F_Delivery_Trip, F_Dock_Cd, F_Truck_Card, F_Tran_Type, F_Wheel, F_Supplier_Code, F_Supplier_Plant, F_short_Logistic, " +
                    $"F_Supplier_Name, F_Arrival_Sup, F_Depart_Sup, F_Arrival_HMMT, F_Depart_HMMT, F_Cycle_Time, F_Remark, F_Import_By, F_Import_Date, " +
                    $"Case When F_Supplier_Code='9999' Then '2' Else '0' End F_Flag " +
                    $"FROM   KBNLC_150 L " +
                    $"WHERE F_Plant ='{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value}' " +
                    $"AND F_YM = '{publicYM}' " +
                    $"AND F_Rev = '{publicRev}' " +
                    $"AND F_Import_By= '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}' ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);

                processBar = 80;

                if (publicRev.CompareTo("0") > 0)
                {
                    string revPrevious = "";
                    _sql = $"Select Distinct Top 1  F_Plant, F_YM, F_Rev  From   TB_Import_Delivery  " +
                        $"WHERE F_Plant ='{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value}' AND F_YM = '{publicYM}' " +
                        $"AND F_Rev<'{publicRev}' Order by F_Rev Desc ";

                    var dt5 = _FillDT.ExecuteSQL(_sql);

                    if (dt5.Rows.Count > 0)
                    {
                        revPrevious = dt5.Rows[0]["F_Rev"].ToString();
                    }
                    else
                    {
                        revPrevious = "0";
                    }

                    _sql = $"UPDATE TB_Import_Delivery SET  F_Flag='2' " +
                        $" Where F_YM = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value}' AND F_Rev = '{publicRev}' " +
                        $"AND F_YM+F_Plant+convert(char,F_Delivery_Trip)+F_Dock_Cd+F_Truck_Card IN " +
                        $"(Select I.F_YM+I.F_Plant+convert(char,I.F_Delivery_Trip)+I.F_Dock_Cd+I.F_Truck_Card " +
                        $" FROM TB_Import_Delivery I INNER JOIN TB_Import_Delivery D ON I.F_YM=D.F_YM AND I.F_Plant=D.F_Plant " +
                        $"AND I.F_Delivery_Trip=D.F_Delivery_Trip AND I.F_Dock_Cd=D.F_Dock_Cd AND I.F_Truck_Card=D.F_Truck_Card " +
                        $"AND I.F_Arrival_Sup=D.F_Arrival_Sup AND I.F_Depart_Sup=D.F_Depart_Sup AND I.F_Arrival_HMMT=D.F_Arrival_HMMT " +
                        $"AND I.F_Depart_HMMT=D.F_Depart_HMMT AND I.F_Cycle_Time=D.F_Cycle_Time " +
                        $"WHERE I.F_Plant = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value}' " +
                        $"AND I.F_YM = '{publicYM}' " +
                        $"AND I.F_Rev='{revPrevious}' " +
                        $"AND D.F_Rev = '{publicRev}' AND I.F_Flag='2' ) ";

                    await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                    processBar = 90;
                    _log.WriteLogMsg("Import TB_Import_Delivery | SQL : " + _sql);

                    _sql = $"UPDATE TB_Import_Delivery SET  F_Flag='5' " +
                        $" Where F_YM = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value}' AND F_Rev = '{publicRev}' " +
                        $"AND F_YM+F_Plant+convert(char,F_Delivery_Trip)+F_Dock_Cd+F_Truck_Card IN " +
                        $"(Select I.F_YM+I.F_Plant+convert(char,I.F_Delivery_Trip)+I.F_Dock_Cd+I.F_Truck_Card " +
                        $"FROM TB_Import_Delivery I INNER JOIN TB_Import_Delivery D ON I.F_YM=D.F_YM AND I.F_Plant=D.F_Plant " +
                        $"AND I.F_Delivery_Trip=D.F_Delivery_Trip AND I.F_Dock_Cd=D.F_Dock_Cd AND I.F_Truck_Card=D.F_Truck_Card " +
                        $"AND I.F_Arrival_Sup=D.F_Arrival_Sup AND I.F_Depart_Sup=D.F_Depart_Sup AND I.F_Arrival_HMMT=D.F_Arrival_HMMT " +
                        $"AND I.F_Depart_HMMT=D.F_Depart_HMMT AND I.F_Cycle_Time=D.F_Cycle_Time  " +
                        $"WHERE I.F_Plant ='{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value}' AND I.F_YM = '{publicYM}' " +
                        $"AND I.F_Rev='{revPrevious}' AND D.F_Rev = '{publicRev}' AND I.F_Flag='5' ) ";

                    await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                    _log.WriteLogMsg("Import TB_Import_Delivery | SQL : " + _sql);
                }

                _sql = $"Update TB_IMPORT_DELIVERY Set F_Flag ='2' " +
                    $" Where F_YM = '{publicYM}' AND F_Rev = '{publicRev}' " +
                    $" and F_Flag ='0' ";

                await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                _log.WriteLogMsg("Update Status to 2 for already Interface | SQL : " + _sql);

                processBar = 100;

                return "Success";
            }
            catch (Exception ex)
            {
                string _sql = $"DELETE  FROM TB_Import_Delivery WHERE F_YM='{publicYM}' AND F_Rev='{publicRev}' AND F_Plant='{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value}'";
                await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                _log.WriteLogMsg("Import Delivery Time Table Error | SQL : " + _sql);
                processBar = 0;
                throw new Exception(ex.Message);
            }
        }

        public int GetProcessBar()
        {
            return processBar;
        }
    }
}
