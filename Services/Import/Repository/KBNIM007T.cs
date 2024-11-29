using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.UrgentOrder;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Import.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Services.Import.Repository
{
    public class KBNIM007T : IKBNIM007T
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;


        public KBNIM007T
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

        public async Task<List<TB_Transaction_TMP>> GetPO(string YM,string TypeSpc)
        {
            try
            {
                var data = await _kbContext.TB_Transaction_TMP
                    .Where(x=>x.F_Type == "Special"
                    && x.F_Delivery_Date.Substring(0,6) == YM
                    && x.F_Type_Spc.ToUpper() == TypeSpc)
                    .OrderBy(x=>x.F_PDS_No).ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<string> GetParentStore(string YM,string? PO,bool isNew)
        {
            try
            {
                if (isNew)
                {
                    string ppm = _FillDT.ppmConnect();
                    string query = $@"Select distinct F_Store_Cd From 
                        {ppm}.dbo.T_Parent_part 
                        Where substring(F_TC_Str,1,6) <='{YM}' 
                        and substring(F_TC_End,1,6) >='{YM}' 
                        and F_Store_Cd like '0%' 
                        Order by F_Store_Cd ";

                    DataTable dt = _FillDT.ExecuteSQL(query);

                    return JsonConvert.SerializeObject(dt);
                }

                var data = await _kbContext.TB_Transaction_TMP
                    .Where(x => x.F_PDS_No == PO
                    && x.F_Type == "Special"
                    && x.F_Delivery_Date.Substring(0, 6) == YM)
                   .Select(x => new
                   {
                       F_Store_CD = x.F_Store_Order
                   }).Distinct().ToListAsync();

                return JsonConvert.SerializeObject(data);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<string> GetParentPart(string YM, string? PO, string? StoreCD, bool isNew)
        {
            try
            {
                if(isNew)
                {
                    string ppm = _FillDT.ppmConnect();
                    string query = $@"Select distinct rtrim(F_Parent_part) +'-'+ rtrim(F_Ruibetsu) as F_Part_No 
                        From {ppm}.dbo.T_Parent_part 
                        Where substring(F_TC_Str,1,6) <='{YM}' 
                        and substring(F_TC_End,1,6) >='{YM}' 
                        and F_Store_Cd = '{StoreCD}' 
                        Order by rtrim(F_Parent_part) +'-'+ rtrim(F_Ruibetsu) ";

                    DataTable dt = _FillDT.ExecuteSQL(query);

                    return JsonConvert.SerializeObject(dt);
                }

                var data = await _kbContext.TB_Transaction_TMP
                    .Where(x => x.F_PDS_No == PO
                    && x.F_Store_Order == StoreCD
                    && x.F_Type == "Special")
                   .Select(x => new
                   {
                       F_Part_No = x.F_Part_Order.Trim() + "-" + x.F_Ruibetsu_Order.Trim()
                   }).Distinct().ToListAsync();

                return JsonConvert.SerializeObject(data);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<string> GetParentPartDetail(string YM, string? StoreCD,string PartNo, bool isNew)
        {
            try
            {
                if (isNew)
                {
                    string ppm = _FillDT.ppmConnect();
                    string query = $@"Select distinct  C.F_Parent_Part as F_Part_No,C.F_Name as F_Part_NM,C.F_Store_Cd as F_Store_Order,'' as F_Supplier,'' as F_Sup_name,'' as F_Remark,'' as F_Sebango,'' as F_PDS_Issued_Date 
                        {ppm}.dbo.T_Parent_part 
                        Where (substring(C.F_TC_Str,1,6) <='{YM}' 
                        and substring(C.F_TC_End,1,6) >='{YM}' ) 
                        and C.F_Store_Cd = '{StoreCD}' 
                        and rtrim(F_Parent_Part)+'-'+rtrim(F_RUibetsu)  ='{PartNo}' 
                        order by C.F_Parent_Part,C.F_Store_Cd ";

                    DataTable dt = _FillDT.ExecuteSQL(query);

                    return JsonConvert.SerializeObject(dt);
                }
                else
                {
                    var data = await _kbContext.TB_Transaction_TMP
                        .Where(x=> x.F_Type == "Special"
                        && x.F_Delivery_Date.Substring(0, 6) == YM
                        && x.F_Part_Order.Trim() + "-" + x.F_Ruibetsu_Order.Trim() == PartNo
                        ).Select(x => new
                        {
                            x.F_Part_No,
                            F_Part_NM = x.F_Name_Order,
                            x.F_Store_Order,
                            F_Supplier = "",
                            F_Sup_name = "",
                            x.F_Remark,
                            F_Sebango = x.F_Kanban_No,
                            x.F_PDS_Issued_Date,
                        }).Distinct().ToListAsync();

                    return JsonConvert.SerializeObject(data);
                }

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<string> GetComponentStore(string YM,string? PO,string? ParentPartNo, bool isNew)
        {
            try
            {
                if (isNew)
                {
                    string ppm = _FillDT.ppmConnect();
                    string query = $@"Select distinct F_Store_Cd From 
                        {ppm}.dbo.T_Construction
                        Where substring(F_CKD_STR,1,6) <='{YM}' 
                        and substring(F_CKD_END,1,6) >='{YM}' 
                        and F_Store_CD like '{_BearerClass.Plant}%' 
                        Order by F_Store_Cd ";

                    DataTable dt = _FillDT.ExecuteSQL(query);

                    return JsonConvert.SerializeObject(dt);
                }

                var data = await _kbContext.TB_Transaction_TMP
                    .Where(x => x.F_PDS_No == PO
                    && x.F_Part_Order.Trim() + "-" + x.F_Ruibetsu_Order.Trim() == ParentPartNo
                    && x.F_Type == "Special"
                    && x.F_Delivery_Date.Substring(0, 6) == YM)
                   .Select(x => new
                   {
                       F_Store_CD = x.F_Store_CD
                   }).Distinct().ToListAsync();

                return JsonConvert.SerializeObject(data);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<string> GetComponentPartNo(string YM, string? PO, string? CompStoreCD, string? ParentPartNo, bool isNew)
        {
            try
            {
                if (isNew)
                {
                    string ppm = _FillDT.ppmConnect();
                    string query = $@"Select distinct rtrim(F_Part_No) +'-'+ rtrim(F_Ruibetsu) as F_Part_No
                        From {ppm}.dbo.T_Construction 
                        Where substring(F_CKD_STR,1,6) <='{YM}' 
                        and substring(F_CKD_END,1,6) >='{YM}' 
                        and F_Store_CD = '{CompStoreCD}' 
                        Order by rtrim(F_Part_No) +'-'+ rtrim(F_Ruibetsu)";

                    DataTable dt = _FillDT.ExecuteSQL(query);

                    return JsonConvert.SerializeObject(dt);
                }
                else
                {
                    var data = await _kbContext.TB_Transaction_TMP
                        .Where(x => x.F_PDS_No == PO
                        && x.F_Store_CD == CompStoreCD
                        && x.F_Part_Order.Trim() + "-" + x.F_Ruibetsu.Trim() == ParentPartNo)
                        .Select(x => new
                        {
                            F_Part_No = x.F_Part_No.Trim() + "-" + x.F_Ruibetsu.Trim()
                        }).Distinct().ToListAsync();

                    return JsonConvert.SerializeObject(data);
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

    }
}
