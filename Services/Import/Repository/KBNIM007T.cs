using ClosedXML.Excel;
using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.SpecialData.ViewModel;
using KANBAN.Models.KB3.SpecialOrdering;
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

        public string SetCalendar(string YM, string? StoreCD,string TypeSpc)
        {
            try
            {
                string queryStoreCD = _FillDT.QueryStoreCode();

                string sql = $@"Select C.*,isnull(V.F_Date,'') as F_Date From TB_Calendar C 
                    CROSS JOIN (select F_YM + F_Day collate THAI_CI_AS as F_Date,F_YM,row_number() Over(Order by F_YM,F_Day) as F_NO from VW_Calendar 
                    Where F_WorkCd ='1' and F_YM + F_Day collate THAI_CI_AS >= convert(Char(8),getdate(),112))V  
                    Where C.F_Store_Cd='{queryStoreCD}' and C.F_YM='{YM}'";

                if (!string.IsNullOrWhiteSpace(StoreCD) && TypeSpc.ToUpper() == "NORMAL")
                {
                    sql += $@"And V.F_NO = 3 ";
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
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<TB_Transaction_TMP>> GetPO(string YM,string TypeSpc)
        {
            try
            {
                var data = await _kbContext.TB_Transaction_TMP
                    .Where(x=>x.F_Type == "Trial"
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
                    //string ppm = _FillDT.ppmConnect();
                    string ppm = "[HMMTA-PPM].[PPMDB]";
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
                    && x.F_Type == "Trial"
                    && x.F_Delivery_Date.Substring(0, 6) == YM)
                   .Select(x => new
                   {
                       F_Store_Cd = x.F_Store_Order
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
                    //string ppm = _FillDT.ppmConnect();
                    string ppm = "[HMMTA-PPM].[PPMDB]";
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
                    && x.F_Type == "Trial")
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
                    //string ppm = _FillDT.ppmConnect();
                    string ppm = "[HMMTA-PPM].[PPMDB]";
                    string query = $@"Select distinct  C.F_Parent_Part as F_Part_No,C.F_Name as F_Part_NM,C.F_Store_Cd as F_Store_Order,'' as F_Supplier,'' as F_Sup_name,'' as F_Remark,'' as F_Sebango,'' as F_PDS_Issued_Date 
                        From {ppm}.dbo.T_Parent_part C
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
                        .Where(x=> x.F_Type == "Trial"
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
                    //string ppm = _FillDT.ppmConnect();
                    string ppm = "[HMMTA-PPM].[PPMDB]";
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
                    && x.F_Type == "Trial"
                    && x.F_Delivery_Date.Substring(0, 6) == YM)
                    .ToListAsync();

                if(!string.IsNullOrWhiteSpace(ParentPartNo))
                {
                    data = data.Where(x => x.F_Part_Order.Trim() + "-" + x.F_Ruibetsu_Order.Trim() == ParentPartNo).ToList();
                }

                return JsonConvert.SerializeObject(data.Select(x => new { F_Store_Cd = x.F_Store_CD }).Distinct().ToList());

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
                    //string ppm = _FillDT.ppmConnect();
                    string ppm = "[HMMTA-PPM].[PPMDB]";
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
                        .Where(x => x.F_Type == "Trial")
                        .ToListAsync();

                    if(!string.IsNullOrWhiteSpace(ParentPartNo))
                    {
                        data = data.Where(x => x.F_Part_Order.Trim() + "-" + x.F_Ruibetsu.Trim() == ParentPartNo).ToList();
                    }
                    if(!string.IsNullOrWhiteSpace(CompStoreCD))
                    {
                        data = data.Where(x => x.F_Store_CD == CompStoreCD).ToList();
                    }
                    if(!string.IsNullOrWhiteSpace(PO))
                    {
                        data = data.Where(x => x.F_PDS_No == PO).ToList();
                    }

                    return JsonConvert.SerializeObject(data.Select(x => new
                    {
                        F_Part_No = x.F_Part_No.Trim() + "-" + x.F_Ruibetsu.Trim()
                    }).Distinct().OrderBy(x=>x.F_Part_No).ToList());
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<string>> ComponentStoreSelected(string YM, string? PO, string? CompStoreCD, string? CompPartNo,string IssuedDate, bool isNew)
        {
            try
            {
                string query = "";
                List<string> result = new List<string>();
                if (isNew)
                {
                    query = $@"Select distinct F_PDS_NO From TB_Transaction_TMP Where F_Type ='Trial' 
                        and F_PDS_NO ='{PO}' and F_Part_No+'-'+F_Ruibetsu ='{CompPartNo}' 
                        and substring(F_DeliverY_Date,1,6) ='{YM}' and F_Store_Cd='{CompStoreCD}' ";

                    DataTable dt = _FillDT.ExecuteSQL(query);

                    result.Add(JsonConvert.SerializeObject(dt));

                    query = $@"select F_Supplier_Cd+'-'+F_Plant as F_Supplier_Cd 
                        from dbo.T_Construction Where F_Part_no='{CompPartNo.Split("-")[0]}' 
                        and F_Ruibetsu ='{CompPartNo.Split("-")[1]}' 
                        and F_Store_Cd='{CompStoreCD}' 
                        and F_CKD_STR <='{IssuedDate}' 
                        and F_CKD_END >='{IssuedDate}' ";

                    dt = _FillDT.ExecuteSQLPPMDB(query);

                    result.Add(JsonConvert.SerializeObject(dt));

                }
                else
                {
                    query = $@"Select F_Supplier_Cd+'-'+F_Supplier_Plant as F_Supplier_Cd
                    from dbo.TB_Transaction_TMP
                    Where F_Part_no='{CompPartNo.Split("-")[0]}' and F_Ruibetsu ='{CompPartNo.Split("-")[1]}'
                    and F_Store_Cd='{CompStoreCD}' and F_PDS_NO='{PO}' 
                    and F_PDS_Issued_Date='{IssuedDate}' ";

                    DataTable dt2 = _FillDT.ExecuteSQL(query);

                    result.Add(JsonConvert.SerializeObject(dt2));
                }

                return result;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<string> ComponentPartSelected(string YM, string? CompStoreCD,string? CompPartNo,string? ParentPartNo,bool isNew)
        {
            try
            {
                string sql = "";
                //string ppmCon = _FillDT.ppmConnect();
                string ppmCon = "[HMMTA-PPM].[PPMDB]";
                if (isNew)
                {
                    sql = $@"Select distinct  C.F_Part_No,C.F_Part_NM,C.F_Store_Cd,C.F_Supplier_Cd+'-'+C.F_Plant as F_Supplier,S.F_Name as F_Sup_name,'' as F_Remark,'0' + rtrim(F_Sebango) as F_Sebango,'' as F_PDS_Issued_Date,C.F_QTY_BOX 
                        from T_Construction C INNER JOIN T_Supplier_MS S ON 
                        c.F_Supplier_CD = S.F_Supplier_CD And c.F_Plant = S.F_Plant_CD And c.F_Store_CD = S.F_Store_CD
                        Where (substring(C.F_CKD_Str,1,6) <='{YM}' and substring(C.F_CKD_End,1,6) >='{YM}' )
                        and (substring(S.F_TC_Str,1,6) <='{YM}' and substring(S.F_TC_End,1,6) >='{YM}' )
                        and C.F_Store_Cd = '{CompStoreCD}' and rtrim(F_Part_No)+'-'+rtrim(F_RUibetsu)  ='{CompPartNo}' 
                        order by C.F_PART_NO,C.F_Store_Cd ";

                    DataTable dt = _FillDT.ExecuteSQLPPMDB(sql);

                    return JsonConvert.SerializeObject(dt);

                }
                else
                {
                    sql = $@"Select distinct T.F_Part_No,T.F_Part_name as F_Part_NM,T.F_Store_CD,T.F_Supplier_Cd +'-'+ F_Supplier_Plant as F_Supplier,S.F_Name as F_Sup_name,T.F_Remark,T.F_Kanban_No as F_Sebango,F_PDS_Issued_Date,F_QTY_Pack as F_QTY_BOX
                        From dbo.TB_Transaction_Tmp  T INNER JOIN {ppmCon}.dbo.T_Supplier_MS S ON T.F_Supplier_CD = s.F_SUpplier_CD collate Thai_CI_AS
                        and T.F_Supplier_Plant = S.F_Plant_CD collate Thai_CI_AS and T.F_Store_Cd = S.f_Store_Cd collate Thai_CI_AS
                        Where F_TYpe='Trial' and substring(F_Delivery_Date,1,6) ='{YM}' 
                        and F_Part_NO +'-'+F_Ruibetsu ='{CompPartNo}' ";

                    if(!string.IsNullOrWhiteSpace(CompStoreCD))
                    {
                        sql += $" and T.F_Store_Cd = '{CompStoreCD}' ";
                    }
                    if(!string.IsNullOrWhiteSpace(ParentPartNo))
                    {
                        sql += $" and rtrim(F_Part_Order)+'-'+rtrim(F_Ruibetsu_order) ='{ParentPartNo}' ";
                    }

                    sql += $"and (substring(S.F_TC_Str,1,6) <='{YM}' and substring(S.F_TC_End,1,6) >='{YM}' ) ";

                    DataTable dt = _FillDT.ExecuteSQL(sql);

                    return JsonConvert.SerializeObject(dt);
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<string> ListCalendar(string YM,string? PO,string? ParentPartNo,string? ParentStoreCD,string? CompPartNo,string? CompStoreCD)
        {
            try
            {
                var data = await _kbContext.TB_Transaction_TMP
                    .Where(x=>x.F_Type == "Trial"
                    && x.F_Delivery_Date.Substring(0, 6) == YM)
                    .ToListAsync();

                if(!string.IsNullOrWhiteSpace(PO))
                {
                    data = data.Where(x => x.F_PDS_No == PO).ToList();
                }
                if (!string.IsNullOrWhiteSpace(ParentPartNo))
                {
                    data = data.Where(x => x.F_Part_Order.Trim() + "-" + x.F_Ruibetsu_Order.Trim() == ParentPartNo).ToList();
                }
                if (!string.IsNullOrWhiteSpace(ParentStoreCD))
                {
                    data = data.Where(x => x.F_Store_Order == ParentStoreCD).ToList();
                }
                if(!string.IsNullOrWhiteSpace(CompPartNo))
                {
                    data = data.Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu.Trim() == CompPartNo).ToList();
                }
                if (!string.IsNullOrWhiteSpace(CompStoreCD))
                {
                    data = data.Where(x => x.F_Store_CD == CompStoreCD).ToList();
                }

                var data2 = data.Select(x => new
                {
                    x.F_PDS_No,
                    F_Part_No = x.F_Part_Order + "-" + x.F_Ruibetsu_Order,
                    F_Delivery_Date = x.F_Delivery_Date.Substring(6, 2) + "/" + x.F_Delivery_Date.Substring(4, 2) + "/" + x.F_Delivery_Date.Substring(0, 4),
                    x.F_Qty_Level1,
                    x.F_Qty,
                    x.F_Remark,
                    x.F_Store_Order,
                    F_PDS_Issued_Date = x.F_PDS_Issued_Date.Substring(6, 2) + "/" + x.F_PDS_Issued_Date.Substring(4, 2) + "/" + x.F_PDS_Issued_Date.Substring(0, 4),
                }).Distinct().ToList();

                return JsonConvert.SerializeObject(data2);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<string> ListDatatable (string YM,string? PO,string? ParentPartNo, string? CompPartNo)
        {
            try
            {
                var data = await _kbContext.TB_Transaction_TMP
                    .Where(x => x.F_Type == "Trial"
                    && x.F_Delivery_Date.StartsWith(YM)).ToListAsync();

                if (!string.IsNullOrWhiteSpace(PO))
                {
                    data = data.Where(x => x.F_PDS_No == PO).ToList();
                }
                if (!string.IsNullOrWhiteSpace(ParentPartNo))
                {
                    data = data.Where(x => x.F_Part_Order.Trim() + "-" + x.F_Ruibetsu_Order.Trim() == ParentPartNo).ToList();
                }
                if (!string.IsNullOrWhiteSpace(CompPartNo))
                {
                    data = data.Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu.Trim() == CompPartNo).ToList();
                }

                var data2 = data.Select(x => new
                {
                    x.F_PDS_No,
                    F_Part_No = x.F_Part_Order + "-" + x.F_Ruibetsu_Order,
                    F_Child_Part = x.F_Part_No + "-" + x.F_Ruibetsu,
                    F_Delivery_Date = x.F_Delivery_Date.Substring(6, 2) + "/" + x.F_Delivery_Date.Substring(4, 2) + "/" + x.F_Delivery_Date.Substring(0, 4),
                    x.F_Qty_Level1,
                    x.F_Qty,
                    x.F_Remark,
                    F_Store_CD = x.F_Store_Order,
                    F_Store_Child = x.F_Store_CD,
                    F_PDS_Issued_Date = x.F_PDS_Issued_Date.Substring(6, 2) + "/" + x.F_PDS_Issued_Date.Substring(4, 2) + "/" + x.F_PDS_Issued_Date.Substring(0, 4),
                    F_Supplier = x.F_Supplier_CD + "-" + x.F_Supplier_Plant,
                    F_OrderType = x.F_OrderType == 'C' ? "CKD" : x.F_OrderType == 'S' ? "SPECIAL" : "NORMAL",
                    x.F_Round,
                    x.F_Part_Name
                }).Distinct().ToList();

                return JsonConvert.SerializeObject(data2);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Save(List<VM_Save_KBNIM007T> listObj,string _command)
        {
            try
            {
                List<TB_Transaction_TMP> data = new List<TB_Transaction_TMP>();

                if (_command.ToUpper() == "NEW")
                {
                    if (!string.IsNullOrWhiteSpace(listObj[0].ParentPartNo) && !string.IsNullOrWhiteSpace(listObj[0].CompPartNo))
                    {
                        throw new CustomHttpException(400, "Please Select Parent Part Or Component Part Only!");
                    }

                    if (string.IsNullOrWhiteSpace(listObj[0].ParentPartNo) && string.IsNullOrWhiteSpace(listObj[0].CompPartNo))
                    {
                        throw new CustomHttpException(400, "Please Select Parent Part Or Component Part!");
                    }

                    if (string.IsNullOrWhiteSpace(listObj[0].ParentStore) && string.IsNullOrWhiteSpace(listObj[0].CompStoreCD))
                    {
                        throw new CustomHttpException(400, "Please Select Parent Store Or Component Store!");
                    }

                    data = await _kbContext.TB_Transaction_TMP
                        .Where(x => x.F_Type == "Trial"
                        && x.F_Delivery_Date.Substring(0,6) == listObj[0].IssuedDate.Substring(0, 6)
                        && x.F_PDS_No == listObj[0].PO
                        && x.F_Part_Order.Trim() + "-" + x.F_Ruibetsu_Order.Trim() == listObj[0].ParentPartNo
                        && x.F_Store_Order == listObj[0].ParentStore).ToListAsync();

                    if (data.Count > 0)
                    {
                        throw new CustomHttpException(400, "Can not Insert Data because found data in database!");
                    }

                    foreach(var obj in listObj)
                    {
                        if (!string.IsNullOrWhiteSpace(obj.CompPartNo))
                        {
                            TB_Transaction_TMP compOrderInsert = new TB_Transaction_TMP
                            {
                                F_Type = "Trial",
                                F_Type_Spc = obj.TypeSpc,
                                F_Plant = _BearerClass.Plant[0],
                                F_PDS_No = obj.PO,
                                F_PDS_Issued_Date = obj.IssuedDate,
                                F_Store_CD = obj.CompStoreCD,
                                F_Part_No = obj.CompPartNo.Split("-")[0],
                                F_Ruibetsu = obj.CompPartNo.Split("-")[1],
                                F_Kanban_No = obj.CompSebango,
                                F_Part_Name = obj.CompPartName,
                                F_Qty_Pack = (short)obj.QtyBox,
                                F_Part_Code = string.Empty,
                                F_Part_Order = obj.CompPartNo.Split("-")[0],
                                F_Ruibetsu_Order = obj.CompPartNo.Split("-")[1],
                                F_Store_Order = obj.CompStoreCD,
                                F_Name_Order = obj.CompPartName,
                                F_Qty = obj.Qty,
                                F_Qty_Level1 = obj.Qty,
                                F_Seq_No = string.Empty,
                                F_Seq_Type = string.Empty,
                                F_Cut_Flag = ' ',
                                F_Delivery_Date = obj.DeliveryDate,
                                F_Adv_Deli_Date = string.Empty,
                                F_OrderType = 'S',
                                F_Country = string.Empty,
                                F_Reg_Flg = '0',
                                F_Inventory_Flg = '0',
                                F_Supplier_CD = obj.SupplierCD.Split("-")[0],
                                F_Supplier_Plant = obj.SupplierCD.Split("-")[1][0],
                                F_Cycle_Time = string.Empty,
                                F_Safty_Stock = 0,
                                F_Part_Refer = string.Empty,
                                F_Ruibetsu_Refer = string.Empty,
                                F_Update_By = _BearerClass.UserCode,
                                F_Update_Date = DateTime.Now,
                                F_Remark = string.Empty,
                                F_Parent_Level2 = string.Empty,
                                F_Qty_Level2 = 0,
                                F_Parent_Level3 = string.Empty,
                                F_Qty_Level3 = 0,
                                F_Parent_Level4 = string.Empty,
                                F_Qty_Level4 = 0,
                                F_Round = (short)obj.Trip,
                                F_Org_Store_CD = string.Empty,
                                F_Ratio = "100",
                                F_Customer_OrderType = ' ',
                                F_Survey_DOC = string.Empty,
                            };

                            await _kbContext.TB_Transaction_TMP.AddAsync(compOrderInsert);
                        }
                        else
                        {
                            await _kbContext.Database.ExecuteSqlRawAsync("Exec dbo.SP_IM007_FilterData {0}", _BearerClass.UserCode);

                            await _kbContext.Database.ExecuteSqlRawAsync($@"Exec dbo.SP_IM005_TRIAL 
                                '{obj.PO}','TRIAL','{obj.IssuedDate}','{obj.ParentPartNo}','{obj.ParentStore}',
                                '{obj.DeliveryDate}','{obj.Qty}','{obj.Trip}','{obj.TypeSpc[0]}','{_BearerClass.UserCode}' ");

                            var checkData = await _kbContext.TB_Transaction_TMP
                                .Where(x => x.F_Type == "Trial"
                                && x.F_Delivery_Date.Substring(0, 6) == obj.IssuedDate.Substring(0, 6)
                                && x.F_PDS_No == obj.PO
                                && x.F_Part_Order.Trim() + "-" + x.F_Ruibetsu_Order.Trim() == obj.ParentPartNo
                                && x.F_Store_Order == obj.ParentStore).ToListAsync();

                            if (checkData.Count == 0)
                            {
                                throw new CustomHttpException(400, "Can not Insert Data Please Check Data then Try Again");
                            }
                        }
                    }
                    await _kbContext.SaveChangesAsync();
                }
                else if (_command.ToUpper() == "UPD")
                {
                    if (string.IsNullOrWhiteSpace(listObj[0].ParentPartNo) && string.IsNullOrWhiteSpace(listObj[0].CompPartNo))
                    {
                        throw new CustomHttpException(400, "Please Select Parent Part Or Component Part!");
                    }

                    if (string.IsNullOrWhiteSpace(listObj[0].ParentStore) && string.IsNullOrWhiteSpace(listObj[0].CompStoreCD))
                    {
                        throw new CustomHttpException(400, "Please Select Parent Store Or Component Store!");
                    }

                    data = await _kbContext.TB_Transaction_TMP
                        .Where(x => x.F_Type == "Trial"
                        && x.F_Delivery_Date.Substring(0, 6) == listObj[0].IssuedDate.Substring(0, 6)
                        && x.F_PDS_No == listObj[0].PO)
                        .ToListAsync();

                    if (!string.IsNullOrWhiteSpace(listObj[0].ParentStore))
                    {
                        data = data.Where(x => x.F_Part_Order.Trim() + "-" + x.F_Ruibetsu_Order.Trim() == listObj[0].ParentPartNo
                        && x.F_Store_Order == listObj[0].ParentStore).ToList();
                    }
                    else
                    {
                        data = data.Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu.Trim() == listObj[0].CompPartNo
                        && x.F_Store_CD == listObj[0].CompStoreCD).ToList();
                    }

                    if (data.Count == 0)
                    {
                        throw new CustomHttpException(400, "Can not Update Data because not found data in database!");
                    }

                    _kbContext.TB_Transaction_TMP.RemoveRange(data);

                    await _kbContext.SaveChangesAsync();

                    foreach (var obj in listObj)
                    {
                        if (!string.IsNullOrWhiteSpace(obj.CompPartNo))
                        {
                            TB_Transaction_TMP compOrderInsert = new TB_Transaction_TMP
                            {
                                F_Type = "Trial",
                                F_Type_Spc = obj.TypeSpc,
                                F_Plant = _BearerClass.Plant[0],
                                F_PDS_No = obj.PO,
                                F_PDS_Issued_Date = obj.IssuedDate,
                                F_Store_CD = obj.CompStoreCD,
                                F_Part_No = obj.CompPartNo.Split("-")[0],
                                F_Ruibetsu = obj.CompPartNo.Split("-")[1],
                                F_Kanban_No = obj.CompSebango,
                                F_Part_Name = obj.CompPartName,
                                F_Qty_Pack = (short)obj.QtyBox,
                                F_Part_Code = string.Empty,
                                F_Part_Order = obj.CompPartNo.Split("-")[0],
                                F_Ruibetsu_Order = obj.CompPartNo.Split("-")[1],
                                F_Store_Order = obj.CompStoreCD,
                                F_Name_Order = obj.CompPartName,
                                F_Qty = obj.Qty,
                                F_Qty_Level1 = obj.Qty,
                                F_Seq_No = string.Empty,
                                F_Seq_Type = string.Empty,
                                F_Cut_Flag = ' ',
                                F_Delivery_Date = obj.DeliveryDate,
                                F_Adv_Deli_Date = string.Empty,
                                F_OrderType = 'S',
                                F_Country = string.Empty,
                                F_Reg_Flg = '0',
                                F_Inventory_Flg = '0',
                                F_Supplier_CD = obj.SupplierCD.Split("-")[0],
                                F_Supplier_Plant = obj.SupplierCD.Split("-")[1][0],
                                F_Cycle_Time = string.Empty,
                                F_Safty_Stock = 0,
                                F_Part_Refer = string.Empty,
                                F_Ruibetsu_Refer = string.Empty,
                                F_Update_By = _BearerClass.UserCode,
                                F_Update_Date = DateTime.Now,
                                F_Remark = string.Empty,
                                F_Parent_Level2 = string.Empty,
                                F_Qty_Level2 = 0,
                                F_Parent_Level3 = string.Empty,
                                F_Qty_Level3 = 0,
                                F_Parent_Level4 = string.Empty,
                                F_Qty_Level4 = 0,
                                F_Round = (short)obj.Trip,
                                F_Org_Store_CD = string.Empty,
                                F_Ratio = "100",
                                F_Customer_OrderType = ' ',
                                F_Survey_DOC = string.Empty,
                            };

                            await _kbContext.TB_Transaction_TMP.AddAsync(compOrderInsert);

                        }
                        else
                        {
                            await _kbContext.Database.ExecuteSqlRawAsync("Exec dbo.SP_IM007_FilterData {0}", _BearerClass.UserCode);

                            await _kbContext.Database.ExecuteSqlRawAsync($@"Exec dbo.SP_IM005_TRIAL 
                                '{obj.PO}','TRIAL','{obj.IssuedDate}','{obj.ParentPartNo}','{obj.ParentStore}',
                                '{obj.DeliveryDate}','{obj.Qty}','{obj.Trip}','{obj.TypeSpc[0]}','{_BearerClass.UserCode}' ");

                            var checkData = await _kbContext.TB_Transaction_TMP
                                .Where(x => x.F_Type == "Trial"
                                && x.F_Delivery_Date.Substring(0, 6) == obj.IssuedDate.Substring(0, 6)
                                && x.F_PDS_No == obj.PO
                                && x.F_Part_Order.Trim() + "-" + x.F_Ruibetsu_Order.Trim() == obj.ParentPartNo
                                && x.F_Store_Order == obj.ParentStore).ToListAsync();
                        }
                    }

                    await _kbContext.SaveChangesAsync();

                }
                else
                {
                    data = await _kbContext.TB_Transaction_TMP
                        .Where(x => x.F_Type == "Trial"
                        && x.F_Delivery_Date.Substring(0, 6) == listObj[0].IssuedDate.Substring(0, 6)
                        && x.F_PDS_No == listObj[0].PO)
                        .ToListAsync();

                    if (!string.IsNullOrWhiteSpace(listObj[0].ParentStore))
                    {
                        data = data.Where(x => x.F_Part_Order.Trim() + "-" + x.F_Ruibetsu_Order.Trim() == listObj[0].ParentPartNo
                            && x.F_Store_Order == listObj[0].ParentStore).ToList();
                    }
                    else
                    {
                        data = data.Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu.Trim() == listObj[0].CompPartNo
                            && x.F_Store_CD == listObj[0].CompStoreCD).ToList();
                    }

                    if(data.Count == 0)
                    {
                        throw new CustomHttpException(400, "Can not Delete Data because not found data in database!");
                    }

                    _kbContext.TB_Transaction_TMP.RemoveRange(data);

                    await _kbContext.SaveChangesAsync();

                }

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Import(VM_PostFile obj, string TypeSpc)
        {
            try
            {
                string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file_temp");
                if(!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                string path = Path.Combine(directory, "IMPORT_TRIAL.xlsx");

                if(File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var stream = new FileStream(path,FileMode.CreateNew))
                {
                    await obj.File.CopyToAsync(stream);
                    stream.Close();
                }
                string newFilePath = path.Replace(".xlsx", ".txt");

                using (var workbook = new XLWorkbook(path))
                {
                    var worksheet = workbook.Worksheets.First(); // Get the first worksheet

                    using (var writer = new StreamWriter(newFilePath))
                    {
                        // Iterate through rows and columns
                        foreach (var row in worksheet.RowsUsed())
                        {
                            foreach (var cell in row.CellsUsed())
                            {
                                writer.Write(cell.GetValue<string>() + "\t"); // Write cell value with a tab separator
                            }
                            writer.WriteLine(); // New line after each row
                        }
                    }
                }

                await _kbContext.Database.ExecuteSqlRawAsync($"Delete From TB_Import_Trial Where F_Update_BY='{_BearerClass.UserCode}'");

                await _kbContext.Database.ExecuteSqlRawAsync($"Delete From TB_Import_Error Where F_Update_BY='{_BearerClass.UserCode}' and F_Type='TRIAL'");

                await _kbContext.Database.ExecuteSqlRawAsync($"EXEC dbo.SP_IM005_IMPORT '{_BearerClass.UserCode}' , '{TypeSpc}'");

                DataTable dt = _FillDT.ExecuteSQL($"EXEC dbo.SP_IM005_IMPORT '{_BearerClass.UserCode}' , '{TypeSpc}'");

                if(dt.Rows.Count > 0)
                {
                    if (dt.Rows[0].ItemArray[0].ToString() == "CAL ERROR")
                    {
                        throw new CustomHttpException(400, $"การทำงานผิดพลาด กรุณานำเข้าไฟล์ใหม่อีกครั้ง. Error Because : {dt.Rows[0].ItemArray[2].ToString()}");
                    }
                    else
                    {
                        throw new CustomHttpException(400, $"พบข้อมูลบางอย่างผิดปรกติ กรุณาตรวจสอบข้อมูลที่รายงานค่ะ");
                    }
                }

                await _kbContext.Database.ExecuteSqlRawAsync($"exec dbo.SP_IM007_FilterData '{_BearerClass.UserCode}'");

                await _kbContext.Database.ExecuteSqlRawAsync($"Exec dbo.SP_IM005_IMPORT_TRIAL '{_BearerClass.UserCode}'");

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }
    }
}
