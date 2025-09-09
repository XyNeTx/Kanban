using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.UrgentOrder;
using KANBAN.Models.PPM3;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.UrgentOrder.IRepository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace KANBAN.Services.UrgentOrder.Repository
{
    public class KBNIM017R : IKBNIM017R
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _InvenContext;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public KBNIM017R
            (
                KB3Context kbContext,
                BearerClass BearerClass,
                PPM3Context PPM3Context,
                PPMInvenContext InvenContext,
                FillDataTable FillDT,
                SerilogLibs log,
                IEmailService emailService,
                IAutoMapService autoMapService,
                IHttpContextAccessor httpContextAccessor
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
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<TB_Transaction_TMP>> GetUrgentOrders(List<VM_KBNIM017R_ImportData> listObj)
        {
            try
            {
                var addList = new List<TB_Transaction_TMP>();
                string PDSNo = listObj[0].PDS_No;

                foreach (var obj in listObj) {

                    var Delivery_Date = DateTime.ParseExact(obj.DeliveryDate, "M/dd/yyyy", null).AddDays(-10);
                    var YM = Delivery_Date.ToString("yyyyMM");
                    bool isWorkDay = false;

                    do
                    {
                        var TB_MS_Calendar = await _kbContext.TB_Calendar
                            .FirstOrDefaultAsync(x => x.F_YM == YM
                            && x.F_Store_cd == "3C");

                        if (TB_MS_Calendar == null)
                        {
                            throw new CustomHttpException(500, "Please Set Master Working Calendar");
                        }

                        int date = int.Parse(Delivery_Date.ToString("dd"));
                        string accessWork = $"F_workCd_D{date}";
                        var propCalendar = TB_MS_Calendar.GetType().GetProperty(accessWork);

                        if (propCalendar != null && propCalendar.GetValue(TB_MS_Calendar).ToString() == "1")
                        {
                            isWorkDay = true;
                        }
                        else
                        {
                            Delivery_Date = Delivery_Date.AddDays(-1);
                        }
                    }
                    while (!isWorkDay);

                    var T_Convert_FG_THPS_PEFF = await _InvenContext.T_Convert_FG_THPS_PEFF.AsNoTracking()
                        .Where(x => x.F_Fg_Part_no.Trim() == obj.PartNo.Trim()).FirstOrDefaultAsync();

                    if(T_Convert_FG_THPS_PEFF == null)
                    {
                        throw new CustomHttpException(404, $"Convert FG THPS not found for {obj.PartNo}");
                    }

                    var T_Parent_Part = await _PPM3Context.T_Parent_part.AsNoTracking()
                        //.FirstOrDefaultAsync();
                        .Where(x => x.F_Parent_part.Trim() == T_Convert_FG_THPS_PEFF.F_Part_no_PEFF
                        && x.F_Ruibetsu == T_Convert_FG_THPS_PEFF.F_Ruibetsu_PEFF
                        && x.F_store_cd.StartsWith("0")).FirstOrDefaultAsync();

                    if (T_Parent_Part == null)
                    {
                        throw new CustomHttpException(404, $"Parent part not found for {T_Convert_FG_THPS_PEFF.F_Fg_Part_no}");
                    }

                    var isDuplicate = await _kbContext.TB_Transaction
                        .AnyAsync(x => x.F_PDS_No == PDSNo
                        && x.F_Part_Order.Trim() == T_Parent_Part.F_Parent_part.Trim()
                        && x.F_Ruibetsu_Order == T_Parent_Part.F_Ruibetsu
                        && x.F_Store_Order == T_Parent_Part.F_store_cd);

                    var isDuplicateTMP = await _kbContext.TB_Transaction_TMP
                        .AnyAsync(x => x.F_PDS_No == PDSNo
                        && x.F_Part_Order.Trim() == T_Parent_Part.F_Parent_part.Trim()
                        && x.F_Ruibetsu_Order == T_Parent_Part.F_Ruibetsu
                        && x.F_Store_Order == T_Parent_Part.F_store_cd);

                    if (isDuplicateTMP || isDuplicate)
                    {
                        throw new CustomHttpException(500, "Duplicate PO & Part No");
                    }

                    var T_Child_Part_List = await _PPM3Context.T_Parents_child
                        .AsNoTracking()
                        .Where(x => x.F_parent_part == T_Parent_Part.F_Parent_part
                        && x.F_Ruibetsu == T_Parent_Part.F_Ruibetsu
                        && x.F_store_cd == T_Parent_Part.F_store_cd
                        && !x.F_ch_store_cd.StartsWith("0")
                        ).ToListAsync();

                    foreach (var T_Child_Part in T_Child_Part_List)
                    {
                        var T_Constuction = await _PPM3Context.T_Construction.AsNoTracking()
                            .Where(x => x.F_Part_no == T_Child_Part.F_Child_part
                            && x.F_Ruibetsu == T_Child_Part.F_Ch_ruibetsu
                            && x.F_Store_cd == T_Child_Part.F_ch_store_cd
                            ).FirstOrDefaultAsync();

                        if (T_Constuction == null)
                        {
                            throw new CustomHttpException(500, "Construction Not Found");
                        }
                        if(T_Constuction.F_Store_cd == "RM")
                        {
                            continue;
                        }

                        //int Qty = int.Parse(Math.Ceiling(((obj.DeliveryQty * T_Child_Part.F_Use_pieces.Value) * 0.2)).ToString());
                        //Qty += (obj.DeliveryQty * T_Child_Part.F_Use_pieces.Value);

                        int Qty = (obj.DeliveryQty * T_Child_Part.F_Use_pieces.Value);

                        var addObj = new TB_Transaction_TMP
                        {
                            F_Type = "Urgent",
                            F_Type_Spc = "",
                            F_Plant = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value[0],
                            F_PDS_No = PDSNo,
                            F_PDS_Issued_Date = DateTime.Now.ToString("yyyyMMdd"),
                            F_Store_CD = T_Constuction.F_Store_cd,
                            F_Part_No = T_Constuction.F_Part_no,
                            F_Ruibetsu = T_Constuction.F_Ruibetsu,
                            F_Kanban_No = "0" + T_Constuction.F_Sebango,
                            F_Part_Name = T_Constuction.F_Part_nm,
                            F_Qty_Pack = T_Constuction.F_qty_box.Value,
                            F_Part_Code = " ",
                            F_Part_Order = T_Parent_Part.F_Parent_part,
                            F_Ruibetsu_Order = T_Parent_Part.F_Ruibetsu,
                            F_Store_Order = T_Parent_Part.F_store_cd,
                            F_Name_Order = T_Parent_Part.F_name,
                            F_Qty = Qty,
                            F_Qty_Level1 = T_Child_Part.F_Use_pieces.Value,
                            F_Seq_No = " ",
                            F_Seq_Type = " ",
                            F_Cut_Flag = ' ',
                            F_Delivery_Date = Delivery_Date.ToString("yyyyMMdd"),
                            F_Adv_Deli_Date = "",
                            F_OrderType = 'U',
                            F_Country = "",
                            F_Reg_Flg = '0',
                            F_Inventory_Flg = '0',
                            F_Supplier_CD = T_Constuction.F_supplier_cd,
                            F_Supplier_Plant = T_Constuction.F_plant.Value,
                            F_Cycle_Time = "",
                            F_Safty_Stock = T_Constuction.F_Safety_Stk.Value,
                            F_Part_Refer = "",
                            F_Ruibetsu_Refer = "",
                            F_Update_By = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value,
                            F_Update_Date = DateTime.Now,
                            F_Remark = "",
                            F_Parent_Level2 = "",
                            F_Qty_Level2 = 0,
                            F_Parent_Level3 = "",
                            F_Qty_Level3 = 0,
                            F_Parent_Level4 = "",
                            F_Qty_Level4 = 0,
                            F_Org_Store_CD = "",
                            F_Round = 1,
                            F_Ratio = T_Constuction.F_ratio.ToString(),
                        };

                        addList.Add(addObj);
                    }

                    int BOM_Level = 2;

                    T_Parents_child T_Child_Parent_List = new T_Parents_child();
                    do
                    {
                        T_Child_Parent_List = await _PPM3Context.T_Parents_child
                            .AsNoTracking()
                            .Where(x => x.F_parent_part.Trim() == T_Parent_Part.F_Parent_part.Trim()
                            && x.F_Ruibetsu == T_Parent_Part.F_Ruibetsu
                            && x.F_store_cd == T_Parent_Part.F_store_cd
                            && x.F_ch_store_cd.StartsWith("0")
                            ).FirstOrDefaultAsync();

                        if (T_Child_Parent_List != null)
                        {
                            var childList = await GetChildPartBOM(T_Child_Parent_List, T_Parent_Part, PDSNo, obj.DeliveryQty, BOM_Level, Delivery_Date.ToString("yyyyMMdd"));
                            addList.AddRange(childList);
                        }
                        BOM_Level++;
                    }
                    while (T_Child_Parent_List == null && BOM_Level <= 4);
                }
                await _kbContext.AddRangeAsync(addList);
                await _kbContext.SaveChangesAsync();
                _log.WriteLogMsg("INSERT INTO TB_Transaction_TMP => " + JsonConvert.SerializeObject(addList, Formatting.Indented));
                return addList;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }
        private async Task<List<TB_Transaction_TMP>> GetChildPartBOM(T_Parents_child T_Parents_Child, T_Parent_part T_Parent_Part, string PDSNo,int DeliveryQty, int level,string Delivery_Date)
        {
            try
            {
                var addList = new List<TB_Transaction_TMP>();

                var T_Child_Part_List = await _PPM3Context.T_Parents_child
                            .AsNoTracking()
                            .Where(x => x.F_parent_part == T_Parents_Child.F_Child_part
                            && x.F_Ruibetsu == T_Parents_Child.F_Ch_ruibetsu
                            && x.F_store_cd == T_Parents_Child.F_ch_store_cd
                            && !x.F_ch_store_cd.StartsWith("0")
                            ).ToListAsync();

                foreach (var T_Child_Part in T_Child_Part_List)
                {
                    var T_Constuction = await _PPM3Context.T_Construction.AsNoTracking()
                        .Where(x => x.F_Part_no == T_Child_Part.F_Child_part
                        && x.F_Ruibetsu == T_Child_Part.F_Ch_ruibetsu
                        && x.F_Store_cd == T_Child_Part.F_ch_store_cd).FirstOrDefaultAsync();

                    if(T_Constuction == null)
                    {
                        throw new CustomHttpException(500, "Construction Not Found");
                    }
                    if(T_Constuction.F_Store_cd == "RM")
                    {
                        continue;
                    }

                    //int Qty = int.Parse(Math.Ceiling(((DeliveryQty * T_Child_Part.F_Use_pieces.Value) * 0.2)).ToString());
                    //Qty += (DeliveryQty * T_Child_Part.F_Use_pieces.Value);

                    int Qty = (DeliveryQty * T_Child_Part.F_Use_pieces.Value);

                    var addObj = new TB_Transaction_TMP
                    {
                        F_Type = "Urgent",
                        F_Type_Spc = "",
                        F_Plant = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value[0],
                        F_PDS_No = PDSNo,
                        F_PDS_Issued_Date = DateTime.Now.ToString("yyyyMMdd"),
                        F_Store_CD = T_Constuction.F_Store_cd,
                        F_Part_No = T_Constuction.F_Part_no,
                        F_Ruibetsu = T_Constuction.F_Ruibetsu,
                        F_Kanban_No = "0" + T_Constuction.F_Sebango,
                        F_Part_Name = T_Constuction.F_Part_nm,
                        F_Qty_Pack = T_Constuction.F_qty_box.Value,
                        F_Part_Code = " ",
                        F_Part_Order = T_Parent_Part.F_Parent_part,
                        F_Ruibetsu_Order = T_Parent_Part.F_Ruibetsu,
                        F_Store_Order = T_Parent_Part.F_store_cd,
                        F_Name_Order = T_Parent_Part.F_name,
                        F_Qty = Qty,
                        F_Qty_Level1 = T_Child_Part.F_Use_pieces.Value,
                        F_Seq_No = " ",
                        F_Seq_Type = " ",
                        F_Cut_Flag = ' ',
                        F_Delivery_Date = Delivery_Date,
                        F_Adv_Deli_Date = "",
                        F_OrderType = 'U',
                        F_Country = "",
                        F_Reg_Flg = '0',
                        F_Inventory_Flg = '0',
                        F_Supplier_CD = T_Constuction.F_supplier_cd,
                        F_Supplier_Plant = T_Constuction.F_plant.Value,
                        F_Cycle_Time = "",
                        F_Safty_Stock = T_Constuction.F_Safety_Stk.Value,
                        F_Part_Refer = "",
                        F_Ruibetsu_Refer = "",
                        F_Update_By = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value,
                        F_Update_Date = DateTime.Now,
                        F_Remark = "",
                        F_Parent_Level2 = "",
                        F_Qty_Level2 = 0,
                        F_Parent_Level3 = "",
                        F_Qty_Level3 = 0,
                        F_Parent_Level4 = "",
                        F_Qty_Level4 = 0,
                        F_Org_Store_CD = "",
                        F_Round = 1,
                        F_Ratio = T_Constuction.F_ratio.ToString(),
                    };

                    if (level == 2)
                    {
                        addObj.F_Parent_Level2 = T_Parents_Child.F_parent_part;
                        addObj.F_Qty_Level2 = T_Parents_Child.F_Use_pieces.Value;
                    }
                    else if (level == 3)
                    {
                        addObj.F_Parent_Level3 = T_Parents_Child.F_parent_part;
                        addObj.F_Qty_Level3 = T_Parents_Child.F_Use_pieces.Value;
                    }
                    else if (level == 4)
                    {
                        addObj.F_Parent_Level4 = T_Parents_Child.F_parent_part;
                        addObj.F_Qty_Level4 = T_Parents_Child.F_Use_pieces.Value;
                    }

                    addList.Add(addObj);
                }
                return addList;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }
    }
}
