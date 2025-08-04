using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.UrgentOrder;
using KANBAN.Models.PPM3;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.UrgentOrder.IRepository;
using Microsoft.EntityFrameworkCore;

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


        public KBNIM017R
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

        public async Task<List<TB_Transaction_TMP>> GetUrgentOrders(List<VM_KBNIM017R_ImportData> listObj)
        {
            try
            {
                var addList = new List<TB_Transaction_TMP>();
                string PDSNo = "TR"+ DateTime.Now.ToString("yyyyMMddHHmmssffff");
                foreach (var obj in listObj) {

                    var T_Convert_FG_THPS_PEFF = await _InvenContext.T_Convert_FG_THPS_PEFF.AsNoTracking()
                        .Where(x => x.F_Fg_Part_no == obj.PartNo).FirstOrDefaultAsync();

                    var T_Parent_Part = await _PPM3Context.T_Parent_part.AsNoTracking()
                        //.FirstOrDefaultAsync();
                        .Where(x => x.F_Parent_part.Trim() == T_Convert_FG_THPS_PEFF.F_Part_no_PEFF
                        && x.F_Ruibetsu == T_Convert_FG_THPS_PEFF.F_Ruibetsu_PEFF
                        && x.F_store_cd.StartsWith("0")).FirstOrDefaultAsync();

                    if (T_Parent_Part == null)
                    {
                        throw new CustomHttpException(404, $"Parent part not found for {T_Convert_FG_THPS_PEFF.F_Fg_Part_no}");
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
                            && x.F_Store_cd == T_Child_Part.F_ch_store_cd).FirstOrDefaultAsync();

                        var addObj = new TB_Transaction_TMP
                        {
                            F_Type = "Urgent",
                            F_Type_Spc = "Truck",
                            F_Plant = _BearerClass.Plant[0],
                            F_PDS_No = PDSNo,
                            F_PDS_Issued_Date = DateTime.Now.ToString("yyyyMMdd"),
                            F_Store_CD = T_Constuction.F_Store_cd,
                            F_Part_No = T_Constuction.F_Part_no,
                            F_Ruibetsu = T_Constuction.F_Ruibetsu,
                            F_Kanban_No = "0" + T_Constuction.F_Sebango,
                            F_Part_Name = T_Constuction.F_Part_nm,
                            F_Qty_Pack = short.Parse(obj.Packs.ToString()),
                            F_Part_Code = " ",
                            F_Part_Order = T_Parent_Part.F_Parent_part,
                            F_Ruibetsu_Order = T_Parent_Part.F_Ruibetsu,
                            F_Store_Order = T_Parent_Part.F_store_cd,
                            F_Name_Order = T_Parent_Part.F_name,
                            F_Qty = obj.DeliveryQty * T_Child_Part.F_Use_pieces.Value,
                            F_Qty_Level1 = T_Child_Part.F_Use_pieces.Value,
                            F_Seq_No = " ",
                            F_Seq_Type = " ",
                            F_Cut_Flag = ' ',
                            F_Delivery_Date = DateTime.Now.AddDays(7).ToString("yyyyMMdd"),
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
                            F_Update_By = _BearerClass.UserCode,
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

                    //List<T_Parents_child> T_Child_Parent_List = new List<T_Parents_child>();
                    T_Parents_child T_Child_Parent_List = new T_Parents_child();
                    do
                    {
                        T_Child_Parent_List = await _PPM3Context.T_Parents_child
                            .AsNoTracking()
                            .Where(x => x.F_parent_part == T_Parent_Part.F_Parent_part
                            && x.F_Ruibetsu == T_Parent_Part.F_Ruibetsu
                            && x.F_store_cd == T_Parent_Part.F_store_cd
                            && x.F_ch_store_cd.StartsWith("0")
                            ).FirstOrDefaultAsync();

                        if (T_Child_Parent_List != null)
                        {
                            var childList = await GetChildPartBOM(T_Child_Parent_List, T_Parent_Part, PDSNo, obj.DeliveryQty, BOM_Level);
                            addList.AddRange(childList);
                        }
                        BOM_Level++;
                    }
                    while (T_Child_Parent_List == null && BOM_Level <= 4);
                    //while (T_Child_Parent_List.Count > 0 && BOM_Level <= 4);

                    //while (T_Child_Parent_List == null && BOM_Level <= 4)
                    //{
                    //    T_Child_Parent_List = await _PPM3Context.T_Parents_child
                    //        .AsNoTracking()
                    //        .Where(x => x.F_parent_part == T_Parent_Part.F_Parent_part
                    //        && x.F_Ruibetsu == T_Parent_Part.F_Ruibetsu
                    //        && x.F_store_cd == T_Parent_Part.F_store_cd
                    //        && x.F_ch_store_cd.StartsWith("0")
                    //        ).FirstOrDefaultAsync();

                    //    if (T_Child_Parent_List != null)
                    //    {
                    //        var childList = await GetChildPartBOM(T_Child_Parent_List, T_Parent_Part, PDSNo, obj.DeliveryQty, BOM_Level);
                    //        addList.AddRange(childList);
                    //    }
                    //    BOM_Level++;
                    //}

                }
                return addList;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }
        private async Task<List<TB_Transaction_TMP>> GetChildPartBOM(T_Parents_child T_Parents_Child, T_Parent_part T_Parent_Part, string PDSNo,int DeliveryQty, int level)
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

                    var T_Child_Name = await _PPM3Context.T_Construction.AsNoTracking()
                        .Where(x => x.F_Part_no.Trim() == T_Parents_Child.F_Child_part.Trim()
                        && x.F_Ruibetsu == T_Parents_Child.F_Ch_ruibetsu
                        && x.F_Store_cd == T_Parents_Child.F_ch_store_cd).FirstOrDefaultAsync();

                    if(T_Child_Name == null || T_Constuction == null)
                    {
                        throw new CustomHttpException(500, "Construction Not Found");
                    }

                    var addObj = new TB_Transaction_TMP
                    {
                        F_Type = "Urgent",
                        F_Type_Spc = "Truck",
                        F_Plant = _BearerClass.Plant[0],
                        F_PDS_No = PDSNo,
                        F_PDS_Issued_Date = DateTime.Now.ToString("yyyyMMdd"),
                        F_Store_CD = T_Constuction.F_Store_cd,
                        F_Part_No = T_Constuction.F_Part_no,
                        F_Ruibetsu = T_Constuction.F_Ruibetsu,
                        F_Kanban_No = "0" + T_Constuction.F_Sebango,
                        F_Part_Name = T_Constuction.F_Part_nm,
                        F_Qty_Pack = 1,
                        F_Part_Code = " ",
                        F_Part_Order = T_Child_Name.F_Part_no,
                        F_Ruibetsu_Order = T_Child_Name.F_Ruibetsu,
                        F_Store_Order = T_Child_Name.F_Store_cd,
                        F_Name_Order = T_Child_Name.F_Part_nm,
                        F_Qty = DeliveryQty * T_Child_Part.F_Use_pieces.Value,
                        F_Qty_Level1 = T_Child_Part.F_Use_pieces.Value,
                        F_Seq_No = " ",
                        F_Seq_Type = " ",
                        F_Cut_Flag = ' ',
                        F_Delivery_Date = DateTime.Now.AddDays(7).ToString("yyyyMMdd"),
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
                        F_Update_By = _BearerClass.UserCode,
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
