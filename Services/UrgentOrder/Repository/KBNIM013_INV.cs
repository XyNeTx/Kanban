using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.UrgentOrder;
using KANBAN.Models.PPM;
using KANBAN.Models.PPM3;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.UrgentOrder.IRepository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Services.UrgentOrder.Repository;

public class KBNIM013_INV : IKBNIM013_INV
{
    private readonly KB3Context _kbContext;
    private readonly BearerClass _BearerClass;
    private readonly PPM3Context _PPM3Context;
    private readonly PPMInvenContext _InvenContext;
    private readonly FillDataTable _FillDT;
    private readonly SerilogLibs _log;
    private readonly IEmailService _emailService;
    private readonly IAutoMapService _automapService;


    public KBNIM013_INV
        (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            PPMInvenContext InvenContext,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            IAutoMapService autoMapService
        )
    {
        _kbContext = kbContext;
        _BearerClass = BearerClass;
        _PPM3Context = PPM3Context;
        _InvenContext = InvenContext;
        _FillDT = FillDT;
        _log = log;
        _emailService = emailService;
        _automapService = autoMapService;
    }

    public async Task<string> GetList_Header()
    {
        try
        {
            if (_BearerClass.Plant == "3")
            {
                var _dt = await _FillDT.ExecuteSQLAsync
                (@" SELECT F_Declare_No, F_Pds_No, F_Delivery_Date
                FROM [HMMT-PPM].[SaleOrder].dbo.T_TPCAP_Delivery_List_Declare 
                Where F_Declare_No like 'INB%' 
                    and F_ID ='0' 
                    and F_Flag = '1'
                    AND  F_Remark <> 'Raw Material' 
                GROUP BY F_Declare_No, F_Pds_No, F_Delivery_Date
                ORDER BY F_Declare_No, F_Pds_No, F_Delivery_Date");

                return JsonConvert.SerializeObject(_dt, Formatting.Indented);
            }
            else if (_BearerClass.Plant == "1")
            {
                var _dt = await _FillDT.ExecuteSQLAsync
                (@" SELECT F_Declare_No, F_Pds_No, F_Delivery_Date
                FROM [HMMT-PPM].[SaleOrder].dbo.T_TPCAP_Delivery_List_Declare 
                Where F_Declare_No like 'INS%'
                    and F_ID ='0'
                    and F_Flag = '1'
                    AND  F_Remark <> 'Raw Material' 
                GROUP BY F_Declare_No, F_Pds_No, F_Delivery_Date
                ORDER BY F_Declare_No, F_Pds_No, F_Delivery_Date");

                return JsonConvert.SerializeObject(_dt, Formatting.Indented);
            }
            else
            {
                throw new CustomHttpException(400, "Invalid Plant Code");
            }

        }
        catch (Exception ex)
        {
            if (ex is CustomHttpException) throw;
            throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
        }
    }

    public async Task<string> GetList_Detail(string inDeclareNo)
    {
        try
        {
            string _ppmCon = _FillDT.ppmConnect();
            string _now = DateTime.Now.ToString("yyyyMMdd");
            string sql = @$"SELECT     D.F_Declare_No, D.F_Pds_No, D.F_Delivery_Date, rtrim(C.F_Part_No) +'-'+ C.F_Ruibetsu as F_Part_No,'0'+rtrim(C.F_Sebango) as F_Kanban_NO,
                isnull(C.F_Supplier_Cd +'-'+ C.F_Plant,'') as F_SUpplier, SUM(D.F_QTY) as F_QTY
                FROM [HMMT-PPM].[SaleOrder].dbo.T_TPCAP_Delivery_List_Declare D LEFT OUTER JOIN (Select F_Part_No,F_Ruibetsu,F_Sebango,F_Supplier_Cd,F_Plant from {_ppmCon}.dbo.T_Construction 
                Where F_Local_Str <='{_now}' 
                    and F_Local_End >='{_now}' 
                    and F_Supplier_Cd='9999' 
                    and F_Store_Cd='3P'
                Group by F_Part_No,F_Ruibetsu,F_Sebango,F_Supplier_Cd,F_Plant)C ON 
                SUBSTRING(D.F_Part_No,1,10) = rtrim(C.F_PART_NO collate Thai_CI_AS) and SUBSTRING(D.F_Part_No,11,2) = rtrim(C.F_Ruibetsu collate Thai_CI_AS)  
                WHERE     (F_Flag = '1') 
                    AND (F_ID = '0') 
                    and D.F_Declare_NO IN ({inDeclareNo})
                    AND D.F_Remark <> 'Raw Material' 
                Group by D.F_Declare_No, D.F_Pds_No, D.F_Delivery_Date,rtrim(C.F_Part_No) +'-'+ C.F_Ruibetsu,'0'+rtrim(C.F_Sebango),isnull(C.F_Supplier_Cd +'-'+ C.F_Plant,'')
                Order by D.F_Declare_No, D.F_Pds_No, D.F_Delivery_Date, rtrim(C.F_Part_No) +'-'+ C.F_Ruibetsu";

            var _dt = await _FillDT.ExecuteSQLAsync(sql);

            return JsonConvert.SerializeObject(_dt, Formatting.Indented);
        }
        catch (Exception ex)
        {
            if (ex is CustomHttpException) throw;
            throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
        }
    }

    public async Task InterfaceDataToTransactionTemp(List<VM_KBNIM013_INV> listObj,string PDS)
    {
        try
        {
            var addList = new List<TB_Transaction_TMP>();
            foreach(var obj in listObj)
            {
                var T_Constuction = await _PPM3Context.T_Construction.AsNoTracking()
                    .Where(x => x.F_Part_no.Trim() == obj.F_Part_No.Trim().Substring(0,10)
                        && x.F_Ruibetsu == obj.F_Part_No.Trim().Substring(11,2)
                        && x.F_Sebango == obj.F_Kanban_NO.Substring(1,3)
                        && x.F_supplier_cd.Trim() == obj.F_SUpplier.Substring(0,4)
                        && x.F_plant == obj.F_SUpplier.Substring(5, 1)[0]
                    )
                    .FirstOrDefaultAsync();

                if(T_Constuction == null)
                {
                    throw new CustomHttpException(404, "Constuction Part Not Found");
                }

                string sqlQ = @$"
                    UPDATE [HMMT-PPM].[SaleOrder].dbo.T_TPCAP_Delivery_List_Declare
                        SET F_Flag='2'
                    WHERE (F_Declare_No = '{obj.F_Declare_No}'
                        AND F_PDS_NO = '{obj.F_Pds_No}'
                        AND F_Part_No = '{obj.F_Part_No.Replace("-",string.Empty)}'
                        AND F_KB_No = '{obj.F_Kanban_NO.Substring(1,3)}'
                        AND F_Flag ='1')
                    ";

                int rowAffect = await _kbContext.Database.ExecuteSqlRawAsync(sqlQ);

                _log.WriteLogMsg("UPDATE TO T_TPCAP_Delivery_List_Declare => " + sqlQ);

                var addObj = new TB_Transaction_TMP
                {
                    F_Type = "Urgent",
                    F_Type_Spc = "INVENTORY",
                    F_Plant = _BearerClass.Plant[0],
                    F_PDS_No = obj.F_Declare_No,
                    F_PDS_Issued_Date = DateTime.Now.ToString("yyyyMMdd"),
                    F_Store_CD = T_Constuction.F_Store_cd,
                    F_Part_No = T_Constuction.F_Part_no,
                    F_Ruibetsu = T_Constuction.F_Ruibetsu,
                    F_Kanban_No = "0" + T_Constuction.F_Sebango,
                    F_Part_Name = T_Constuction.F_Part_nm,
                    F_Qty_Pack = T_Constuction.F_qty_box.Value,
                    F_Part_Code = " ",
                    F_Part_Order = T_Constuction.F_Part_no,
                    F_Ruibetsu_Order = T_Constuction.F_Ruibetsu,
                    F_Store_Order = T_Constuction.F_Store_cd,
                    F_Name_Order = T_Constuction.F_Part_nm,
                    F_Qty = obj.F_QTY,
                    F_Qty_Level1 = obj.F_QTY,
                    F_Seq_No = " ",
                    F_Seq_Type = " ",
                    F_Cut_Flag = ' ',
                    F_Delivery_Date = obj.F_Delivery_Date,
                    F_Adv_Deli_Date = "",
                    F_OrderType = 'U',
                    F_Country = PDS,
                    F_Reg_Flg = '0',
                    F_Inventory_Flg = '0',
                    F_Supplier_CD = T_Constuction.F_supplier_cd,
                    F_Supplier_Plant = T_Constuction.F_plant.Value,
                    F_Cycle_Time = "",
                    F_Safty_Stock = 0,
                    F_Part_Refer = "",
                    F_Ruibetsu_Refer = "",
                    F_Update_By = _BearerClass.UserCode,
                    F_Update_Date = DateTime.Now,
                    F_Remark = obj.F_Declare_No + ":" + obj.F_Pds_No,
                    F_Parent_Level2 = "",
                    F_Qty_Level2 = 0,
                    F_Parent_Level3 = "",
                    F_Qty_Level3 = 0,
                    F_Parent_Level4 = "",
                    F_Qty_Level4 = 0,
                    F_Org_Store_CD = "",
                    F_Round = 1,
                    F_Ratio = "0",
                    F_Survey_DOC = "",
                };
                addList.Add(addObj);
            }
            await _kbContext.TB_Transaction_TMP.AddRangeAsync(addList);
            await _kbContext.SaveChangesAsync();

            _log.WriteLogMsg("INSERT INTO TB_Transaction_TMP => " + 
                JsonConvert.SerializeObject(addList, Formatting.Indented));

        }
        catch (Exception ex)
        {
            if (ex is CustomHttpException) throw;
            throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
        }
    }

    public async Task Delete(List<VM_KBNIM013_INV> listObj)
    {
        try
        {
            foreach(var obj in listObj)
            {
                string sqlQ = @$"
                    UPDATE [HMMT-PPM].[SaleOrder].dbo.T_TPCAP_Delivery_List_Declare
                        SET F_Flag='9'
                    WHERE (F_Declare_No = '{obj.F_Declare_No}'
                        AND F_Part_No = '{obj.F_Part_No.Replace("-",string.Empty)}'
                        AND F_Flag ='1')
                    ";

                int rowAffect = await _kbContext.Database.ExecuteSqlRawAsync(sqlQ);
                _log.WriteLogMsg("UPDATE (DELETE) TO T_TPCAP_Delivery_List_Declare => " + sqlQ);
            }
        }
        catch (Exception ex)
        {
            if (ex is CustomHttpException) throw;
            throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
        }
    }

}
