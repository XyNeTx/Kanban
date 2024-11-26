using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.UrgentOrder;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Import.Interface;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Services.Import.Repository
{
    public class KBNIM007 : IKBNIM007
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;


        public KBNIM007
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

        public string SetCalendar(string YM ,string StoreCD)
        {
            try
            {
                string sql = $@"Select C.*,isnull(V.F_Date,'') as F_Date From TB_Calendar C 
                    CROSS JOIN (select F_YM + F_Day collate THAI_CI_AS as F_Date,F_YM,row_number() Over(Order by F_YM,F_Day) as F_NO from VW_Calendar 
                    Where F_WorkCd ='1' and F_YM + F_Day collate THAI_CI_AS >= convert(Char(8),getdate(),112))V  
                    Where C.F_Store_Cd='1A' and C.F_YM='{YM}'";

                if(StoreCD.Substring(0,1) == "0")
                {
                    sql += $@"And V.F_NO = 4 ";
                }
                else
                {
                    sql += $@"And V.F_NO = 1 ";
                }

                DataTable dt = _FillDT.ExecuteSQL(sql);

                return JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500,ex.Message);
            }
        }

        public async Task<List<TB_Transaction_TMP>> GetPO(string YM)
        {
            try
            {
                var data = await _kbContext.TB_Transaction_TMP.Where(x => x.F_Type == "Special"
                && x.F_Type_Spc == "Special" && x.F_Delivery_Date.Substring(0, 6) == YM)
                    .OrderBy(x=>x.F_PDS_No).ToListAsync();

                return data.DistinctBy(x=>x.F_PDS_No).ToList();
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public string GetStoreCD(string YM, string PO, bool isNew)
        {
            try
            {
                if (!isNew)
                {
                    var data = _kbContext.TB_Transaction_TMP.Where(x => x.F_Type == "Special"
                                   && x.F_Type_Spc == "Special" && x.F_Delivery_Date.Substring(0, 6) == YM && x.F_PDS_No == PO)
                        .OrderBy(x => x.F_Store_Order)
                        .Select(x => new { F_Store_CD = x.F_Store_Order }).ToList();

                    return JsonConvert.SerializeObject(data.DistinctBy(x => x.F_Store_CD).ToList());
                }
                string getDB = _FillDT.ppmConnect();

                string sql = $@"Select F_Store_CD from(Select distinct F_Store_Cd as F_Store_Cd 
                    From {getDB}.dbo.T_Parent_Part Where substring(F_TC_STR,1,6) <='{YM}' 
                    and substring(F_TC_END,1,6) >='{YM}' and substring(F_Store_CD,1,1) ='0' 
                    Union all Select distinct F_Store_Cd as F_Store_Cd 
                    From {getDB}.dbo.T_Construction Where substring(F_Local_Str,1,6) <='{YM}' 
                    and substring(F_Local_End,1,6) >='{YM}' and F_Store_CD like '{_BearerClass.Plant}%' ) MAIN 
                    Order by F_Store_Cd ";

                DataTable dt = _FillDT.ExecuteSQL(sql);

                return JsonConvert.SerializeObject(dt);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public string GetPartNo(string YM,string PO, string StoreCD, bool isNew)
        {
            try
            {
                string sql = "";
                string getDB = _FillDT.ppmConnect();
                if (isNew)
                {
                    if (StoreCD.StartsWith("0"))
                    {
                        sql = $@"Select distinct rtrim(F_Parent_Part) +'-'+ rtrim(F_Ruibetsu) as F_Part_No From {getDB}.dbo.T_Parent_Part
                        Where substring(F_TC_Str,1,6) <='{YM}' 
                        and substring(F_TC_End,1,6) >='{YM}' 
                        and F_Store_CD ='{StoreCD}' Order by rtrim(F_Parent_Part) +'-'+ rtrim(F_Ruibetsu) ";
                    }
                    else
                    {
                        sql = $@"Select distinct rtrim(F_Part_No) +'-'+ rtrim(F_Ruibetsu) as F_Part_No From {getDB}.dbo.T_Construction
                        Where substring(F_CKD_Str,1,6) <='{YM}' 
                        and substring(F_CKD_End,1,6) >='{YM}' 
                        and F_Store_CD ='{StoreCD}' Order by rtrim(F_Part_No) +'-'+ rtrim(F_Ruibetsu) ";
                    }
                }
                else
                {
                    sql = $@"Select distinct rtrim(F_Part_Order)+'-'+rtrim(F_Ruibetsu_Order) as F_Part_No From TB_Transaction_Tmp 
                        Where F_TYpe='Special' and F_TYpe_Spc ='Special' 
                         and Substring(F_Delivery_Date,1,6) = '{YM}' and F_PDS_NO='{PO}' ";

                    if (StoreCD.StartsWith("0"))
                    {
                        sql += $@"and F_Store_Order ='{StoreCD}' ";
                    }
                    else
                    {
                        sql += $@"and F_Store_CD ='{StoreCD}' ";
                    }

                    sql += $@"Order by rtrim(F_Part_Order)+'-'+rtrim(F_Ruibetsu_Order) ";
                }

                DataTable dt = _FillDT.ExecuteSQL(sql);

                return JsonConvert.SerializeObject(dt);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public string PartNoSelected(string YM, string PO, string StoreCD,string PartNo,bool isNew)
        {
            try
            {
                string sql = "";
                string now = DateTime.Now.ToString("yyyyMMdd");
                var data = _kbContext.TB_MS_OldPart.Where(x=>x.F_Plant == _BearerClass.Plant
                && x.F_Start_Date.CompareTo(now) <= 0
                && x.F_End_Date.CompareTo(now) >= 0
                && x.F_Parent_Part.Trim() + "-" + x.F_Ruibetsu.Trim() == PartNo).ToList();
                DataTable dt = new DataTable();
                if (!string.IsNullOrWhiteSpace(StoreCD))
                {
                    data = data.Where(x => x.F_Store_Cd == StoreCD).ToList();
                }

                if(data.Count > 0)
                {
                    throw new CustomHttpException(400, "Please Check Part Again. This Part is Old Part!");
                }

                if (!isNew)
                {
                    if (StoreCD.StartsWith("0"))
                    {
                        sql = $@"Select distinct  C.F_Parent_Part,C.F_Name  as F_Part_NM,C.F_Store_Cd,'' as F_Remark,'' as F_Supplier_Code,'1' as F_round,'' as F_PDS_Issued_Date 
                            from T_Parent_Part C Where (substring(C.F_TC_Str,1,6) <='{YM}' 
                            and substring(C.F_TC_End,1,6) >='{YM}') 
                            and F_Store_CD ='{StoreCD}' and rtrim(F_parent_Part)+'-'+rtrim(F_RUibetsu)  ='{PartNo}' 
                            order by C.F_Parent_Part,C.F_Store_Cd ";
                    }
                    else
                    {
                        sql = $@"Select distinct  C.F_Part_No,C.F_Part_NM as F_Part_NM,C.F_Store_Cd,'' as F_Remark,rtrim(F_supplier_cd)+'-'+ rtrim(F_plant) as F_Supplier_Code,'1' as F_Round,'' as F_PDS_Issued_Date
                            from T_Construction C Where (substring(C.F_CKD_Str,1,6) <='{YM}' 
                            and substring(C.F_CKD_End,1,6) >='{YM}') 
                            and F_Store_CD ='{StoreCD}' and rtrim(F_Part_No)+'-'+rtrim(F_RUibetsu)  ='{PartNo}' 
                            order by C.F_Part_No,C.F_Store_Cd ";
                    }
                    dt = _FillDT.ExecuteSQLPPMDB(sql);
                }
                else
                {
                    sql = $@"Select distinct rtrim(T.F_Part_order)+'-'+rtrim(T.F_Ruibetsu_Order) as F_Part_No,T.F_Name_order as F_Part_NM,T.F_Store_Order as F_Store_CD,T.F_Remark, rtrim(F_Supplier_Cd)+'-'+rtrim(F_Supplier_Plant) as F_Supplier_Code, F_Round,F_PDS_Issued_Date
                        from TB_Transaction_Tmp T Where F_TYpe='Special' and F_TYpe_Spc ='Special' 
                        and Substring(F_Delivery_Date,1,6) = '{YM}' and F_PDS_NO='{PO}' 
                        and rtrim(F_Part_order)+'-'+rtrim(F_Ruibetsu_Order) ='{PartNo}' 
                        order by T.F_Part_order,T.F_Store_Order ";

                    dt = _FillDT.ExecuteSQL(sql);
                }

                return JsonConvert.SerializeObject(dt);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }
    }
}
