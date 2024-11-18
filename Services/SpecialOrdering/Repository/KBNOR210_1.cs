using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Services.SpecialOrdering.Repository
{
    public class KBNOR210_1 : IKBNOR210_1
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;


        public KBNOR210_1
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

        public async Task<List<TB_Transaction_Spc>> LoadDataChangeDelivery(string? PDSNo, string? SuppCd, string? PartNo, bool? chkDeli, string? DeliFrom, string? DeliTo)
        {
            try
            {
                var data = _kbContext.TB_Transaction_Spc.Where(x => x.F_Qty != 0
                && x.F_Delivery_Date_New == "" && x.F_Survey_Flg == "0"
                && x.F_PDS_No != "").AsQueryable();

                if (!string.IsNullOrWhiteSpace(PDSNo))
                {
                    data = data.Where(x => x.F_PDS_No.Trim() == PDSNo);
                }
                if (!string.IsNullOrWhiteSpace(SuppCd))
                {
                    data = data.Where(x => x.F_Supplier_CD.Trim() + "-" + x.F_Supplier_Plant.Trim() == SuppCd);
                }
                if (!string.IsNullOrWhiteSpace(PartNo))
                {
                    data = data.Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == PartNo);
                }
                if (chkDeli == true)
                {
                    if (!string.IsNullOrWhiteSpace(DeliFrom) && !string.IsNullOrWhiteSpace(DeliTo))
                    {
                        data = data.Where(x => x.F_Delivery_Date_New.CompareTo(DeliFrom) >= 0 && x.F_Delivery_Date_New.CompareTo(DeliTo) <= 0);
                    }
                }

                if (data.Count() == 0)
                {
                    throw new Exception("Data not found");
                }

                return await data.ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GetSupplierName(string SuppCd)
        {
            try
            {
                string SupplierName = await _kbContext.Database.SqlQueryRaw<string>
                    ("Select Top 1 Rtrim(F_name) As Value From V_Supplier_ms Where F_supplier_cd + '-' + F_Plant_cd = {0}", SuppCd)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrWhiteSpace(SupplierName))
                {
                    throw new Exception("Supplier Name not found");
                }

                return SupplierName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GetPartName(string PartNo)
        {
            try
            {

                string PartName = await _kbContext.TB_Transaction_Spc
                    .Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == PartNo)
                    .Select(x => x.F_Part_Name).FirstOrDefaultAsync();

                if (string.IsNullOrWhiteSpace(PartName))
                {
                    throw new Exception("Part Name not found");
                }

                return PartName;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataTable> GetPOMergeData(string? PDSNo, string? SuppCd, string? PartNo, bool? chkDeli, string? DeliFrom, string? DeliTo)
        {
            using var transaction = _kbContext.Database.BeginTransaction();
            try
            {
                transaction.CreateSavepoint("Start GetPOMergeData");

                await _kbContext.Database.ExecuteSqlRawAsync("Delete From TB_PO_Merge_Tmp Where F_Rec_user = @p0",
                    new SqlParameter("@p0", _BearerClass.UserCode));

                var data = await _kbContext.TB_Transaction_Spc
                    .Where(x => x.F_PDS_No != "" && x.F_Survey_Flg == "0"
                    && x.F_Qty != 0).ToListAsync();
                if (!string.IsNullOrWhiteSpace(PDSNo))
                {
                    data = data.Where(x => x.F_PDS_No.Trim() == PDSNo).ToList();
                }
                if (!string.IsNullOrWhiteSpace(SuppCd))
                {
                    data = data.Where(x => x.F_Supplier_CD.Trim() + "-" + x.F_Supplier_Plant.Trim() == SuppCd).ToList();
                }
                if (!string.IsNullOrWhiteSpace(PartNo))
                {
                    data = data.Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu.Trim() == PartNo).ToList();
                }
                if (chkDeli == true)
                {
                    if (!string.IsNullOrWhiteSpace(DeliFrom) && !string.IsNullOrWhiteSpace(DeliTo))
                    {
                        data = data.Where(x => x.F_Delivery_Date_New.Trim().CompareTo(DeliFrom) >= 0 && x.F_Delivery_Date_New.Trim().CompareTo(DeliTo) <= 0).ToList();
                    }
                }
                data = data.DistinctBy(x => new
                {
                    x.F_PDS_No,
                    x.F_Supplier_CD,
                    x.F_Supplier_Plant,
                    x.F_Part_No,
                    x.F_Ruibetsu,
                    x.F_Delivery_Date
                }).ToList();

                if (data.Count == 0)
                {
                    throw new Exception("Data not found");
                }

                int j = 0;
                foreach (var item in data)
                {
                    j++;

                    await _kbContext.Database.ExecuteSqlRawAsync("Insert into TB_PO_Merge_Tmp (F_Id,F_PO_Number,F_Part_No,F_Ruibetsu,F_Supplier_CD,F_Rec_User) Values " +
                        "(@j,@F_PDS_No,@F_Part_No,@F_Ruibetsu,@F_Supp_CD,@UserCode)",
                        new SqlParameter("@j", j),
                        new SqlParameter("@F_PDS_No", item.F_PDS_No),
                        new SqlParameter("@F_Part_No", item.F_Part_No),
                        new SqlParameter("@F_Ruibetsu", item.F_Ruibetsu),
                        new SqlParameter("@F_Supp_CD", item.F_Supplier_CD.Trim() + "-" + item.F_Supplier_Plant.Trim()),
                        new SqlParameter("@UserCode", _BearerClass.UserCode)
                        );

                    string _sql = $"Select F_PDS_No,F_PDS_NO_New,F_Part_no,F_Ruibetsu,Rtrim(F_Supplier_CD)+'-'+F_Supplier_Plant AS F_Supp_CD, " +
                        $"Case When F_Delivery_Date_New = '' Then '0' Else '1' End As F_Flag , " +
                        $"Case When F_Delivery_Date_New = '' Then Convert(varchar(11),CAST(F_Delivery_Date AS datetime),103) " +
                        $"Else Convert(varchar(11),CAST(F_Delivery_Date_New AS datetime),103) End As F_Delivery_Date,F_Qty " +
                        $"FROM TB_Transaction_Spc Where F_Qty <> 0 and F_PDS_No <> '' and F_Survey_Flg = '0' " +
                        $"and  F_PDS_No  = '{item.F_PDS_No}' and  Rtrim(F_Supplier_CD)+'-'+LTrim(F_Supplier_Plant)  = '{item.F_Supplier_CD.Trim() + "-" + item.F_Supplier_Plant.Trim()}' " +
                        $"and F_Part_No = '{item.F_Part_No}' and Rtrim(F_Ruibetsu) = '{item.F_Ruibetsu}' " +
                        $"and F_Delivery_Date = '{item.F_Delivery_Date}' Order by F_Part_no,F_Ruibetsu,F_Delivery_Date ";

                    var _dt = _FillDT.ExecuteSQL(_sql);

                    if (_dt.Rows.Count == 0)
                    {
                        throw new Exception("Data not found (_dt) ");
                    }

                    int x = 0;
                    string color = "";

                    for (int k = 0; k < _dt.Rows.Count; k++)
                    {

                        if (k == 6)
                        {
                            j++;

                            await _kbContext.Database.ExecuteSqlRawAsync("Insert into TB_PO_Merge_Tmp (F_Id,F_PO_Number,F_Part_No,F_Ruibetsu,F_Supplier_CD,F_Rec_User) Values " +
                                "(@j,@F_PDS_No,@F_Part_No,@F_Ruibetsu,@F_Supp_CD,@UserCode)",
                                new SqlParameter("@j", j),
                                new SqlParameter("@F_PDS_No", item.F_PDS_No),
                                new SqlParameter("@F_Part_No", item.F_Part_No),
                                new SqlParameter("@F_Ruibetsu", item.F_Ruibetsu),
                                new SqlParameter("@F_Supp_CD", item.F_Supplier_CD.Trim() + "-" + item.F_Supplier_Plant.Trim()),
                                new SqlParameter("@UserCode", _BearerClass.UserCode)
                                );

                            await _kbContext.Database.ExecuteSqlRawAsync("Update TB_PO_Merge_Tmp Set F_Color_1 = '@color' " +
                                ", F_Qty_1 = @F_Qty , F_PDS_1 = @F_PDS_No_New ,F_Delivery_DT_1 = @F_Delivery_Date " +
                                "Where F_Rec_user = @UserCode and F_Id = @j and F_Part_no = @F_Part_No " +
                                "and F_Ruibetsu = @F_Ruibetsu ",
                                new SqlParameter("@color", color),
                                new SqlParameter("@F_Qty", _dt.Rows[k]["F_Qty"]),
                                new SqlParameter("@F_PDS_No_New", _dt.Rows[k]["F_PDS_No_New"]),
                                new SqlParameter("@F_Delivery_Date", _dt.Rows[k]["F_Delivery_Date"]),
                                new SqlParameter("@UserCode", _BearerClass.UserCode),
                                new SqlParameter("@j", j),
                                new SqlParameter("@F_Part_No", item.F_Part_No),
                                new SqlParameter("@F_Ruibetsu", item.F_Ruibetsu)
                                );

                            x = 1;

                        }
                        else
                        {
                            x++;
                            if (_dt.Rows[k]["F_Flag"].ToString() == "1")
                            {
                                color = "SandyBrown";
                            }
                            else
                            {
                                color = "White";
                            }

                            await _kbContext.Database.ExecuteSqlRawAsync($"Update TB_PO_Merge_Tmp Set F_Color_{x} = '@color' " +
                                $", F_Qty_{x} = @F_Qty , F_PDS_{x} = @F_PDS_No_New ,F_Delivery_DT_{x} = @F_Delivery_Date " +
                                "Where F_Rec_user = @UserCode and F_Id = @j and F_Part_no = @F_Part_No " +
                                "and F_Ruibetsu = @F_Ruibetsu ",
                                new SqlParameter("@color", color),
                                new SqlParameter("@F_Qty", _dt.Rows[k]["F_Qty"]),
                                new SqlParameter("@F_PDS_No_New", _dt.Rows[k]["F_PDS_No_New"]),
                                new SqlParameter("@F_Delivery_Date", _dt.Rows[k]["F_Delivery_Date"]),
                                new SqlParameter("@UserCode", _BearerClass.UserCode),
                                new SqlParameter("@j", j),
                                new SqlParameter("@F_Part_No", item.F_Part_No),
                                new SqlParameter("@F_Ruibetsu", item.F_Ruibetsu)
                                );
                        }
                    }
                }

                transaction.Commit();

                //transaction.CreateSavepoint("Start GetPOMergeData");

                string sql = "SELECT  CAST (F_ID As integer) as F_ID " +
                    $"FROM  TB_PO_Merge_Tmp WHERE (F_Rec_User = '{_BearerClass.UserCode}' ) " +
                    $" Order by  CAST (F_ID As integer) ";

                var dt = _FillDT.ExecuteSQL(sql);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sql = "Insert into TB_PO_Merge_Tmp (F_ID,F_Part_no,F_Delivery_DT_1,F_Delivery_DT_2,F_Delivery_DT_3,F_Delivery_DT_4,F_Delivery_DT_5,F_Delivery_DT_6,F_Rec_Date,F_Rec_User) " +
                            $"SELECT  CAST(F_ID As Integer)-0.5 , 'PDS',  F_PDS_1, F_PDS_2, F_PDS_3, F_PDS_4, F_PDS_5, F_PDS_6,getDate(), {_BearerClass.UserCode} " +
                            $"FROM  TB_PO_Merge_Tmp WHERE (F_Rec_User = '{_BearerClass.UserCode}' ) " +
                            $"and F_ID = '{dt.Rows[i]["F_ID"]}'";

                        await _kbContext.Database.ExecuteSqlRawAsync(sql);
                    }
                }

                sql = "Select  F_Id, F_PO_Number, CASE When Rtrim(F_Part_No) = 'PDS' Then '' Else RTRIM(F_Part_No) + '-' + F_Ruibetsu End AS F_part_no, F_Ruibetsu, F_Supplier_CD, F_Flag_1, F_Qty_1, F_Delivery_DT_1, F_Color_1, " +
                    "F_Flag_2, F_Qty_2, F_Delivery_DT_2, F_Color_2, " +
                    "F_Flag_3, F_Qty_3, F_Delivery_DT_3, F_Color_3, " +
                    "F_Flag_4, F_Qty_4, F_Delivery_DT_4, F_Color_4, " +
                    "F_Flag_5, F_Qty_5, F_Delivery_DT_5, F_Color_5, " +
                    "F_Flag_6, F_Qty_6, F_Delivery_DT_6, F_Color_6, F_Rec_Date, F_Rec_User " +
                    $"From TB_PO_Merge_Tmp Where F_Rec_User = '{_BearerClass.UserCode}' " +
                    $"Order by CAST(F_ID  AS float)  ";

                var _dtz = _FillDT.ExecuteSQL(sql);

                if (_dtz.Rows.Count == 0)
                {
                    throw new Exception("Data not found (_dtz) ");
                }

                return _dtz;

            }
            catch (Exception ex)
            {
                transaction.RollbackToSavepoint("Start GetPOMergeData");
                throw new Exception(ex.Message);
            }
        }

        public async Task Save(VM_Save_KBNOR210_1 obj)
        {
            try
            {

                string Delivery_Date_New = await CheckPODateStatus(obj);

                var existData = _kbContext.TB_Transaction_Spc
                    .Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu.Trim() == obj.F_Part_No
                     && x.F_PDS_No.Trim() == obj.F_PDS_No && x.F_PDS_No_New.Trim() == obj.F_PDS_No_New
                     && x.F_Qty == obj.F_Qty
                     && x.F_Supplier_CD.Trim() + "-" + x.F_Supplier_Plant.Trim() == obj.F_Supplier_CD)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(Delivery_Date_New))
                {
                    existData.Where(x => x.F_Delivery_Date_New == obj.F_Delivery_Date);
                }
                else
                {
                    existData.Where(x => x.F_Delivery_Date == obj.F_Delivery_Date);
                }

                var data = await existData.FirstOrDefaultAsync();

                if (data == null)
                {
                    throw new Exception("Data not found");
                }

                data.F_Delivery_Date_New = obj.F_Delivery_Date_New;

                _kbContext.TB_Transaction_Spc.Update(data);

                await _kbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> CheckPODateStatus(VM_Save_KBNOR210_1 obj)
        {
            try
            {

                var Delivery_Date_New = await _kbContext.TB_Transaction_Spc
                    .Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == obj.F_Part_No
                    && x.F_PDS_No == obj.F_PDS_No && x.F_PDS_No_New == obj.F_PDS_No_New
                    && x.F_Qty == obj.F_Qty && x.F_Delivery_Date == obj.F_Delivery_Date).FirstOrDefaultAsync();

                if (Delivery_Date_New == null)
                {
                    return "";
                }

                return Delivery_Date_New.F_Delivery_Date_New;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        // Modal KBNOR210_1_STC_3 : Use Stock Part
        public DataTable LoadGridData(string OrderNo)
        {
            try
            {

                string _sql = "Select F_Supplier, Rtrim(F_Part_no) AS F_Part_No ,F_Store_Cd " +
                    ",Sum(F_Actual_Qty) AS F_Actual_Qty, '' as Flag, F_OrderNo " +
                    ",Sum(F_Qty) AS F_Qty , Sum(F_Remain) AS F_Remain, Isnull(Sum(F_Use_StockQty),0) As  F_Use_StockQty " +
                    $"From dbo.FN_getUseOrderSPCData('{OrderNo}') " +
                    $"Group by F_Supplier, Rtrim(F_Part_no),F_OrderNo,F_Store_CD " +
                    $"Order by F_Supplier ";

                var _dt = _FillDT.ExecuteSQL(_sql);

                if (_dt.Rows.Count == 0)
                {
                    throw new Exception("Data not found");
                }

                return _dt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SaveKBNOR210_1_STC_3(List<VM_Save_KBNOR210_1_STC_3> listObj)
        {
            try
            {

                foreach (var obj in listObj)
                {
                    await _kbContext.Database.ExecuteSqlRawAsync("EXEC sp_updateTransactionSpcRemain @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7",
                            new SqlParameter("@p0", obj.F_Supplier.Split("-")[0]),
                            new SqlParameter("@p1", obj.F_Supplier.Split("-")[1]),
                            new SqlParameter("@p2", obj.F_Part_No.Split("-")[0]),
                            new SqlParameter("@p3", obj.F_Part_No.Split("-")[1]),
                            new SqlParameter("@p4", obj.F_Store_Cd),
                            new SqlParameter("@p5", obj.F_OrderNo),
                            new SqlParameter("@p6", obj.F_Use_StockQty),
                            new SqlParameter("@p7", _BearerClass.UserCode)
                        );

                }
                _log.WriteLogMsg("SaveKBNOR210_1_STC_3 | " + JsonConvert.SerializeObject(listObj));

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Modal KBNOR210_1_STC_3_1 : 
        public async Task<DataTable> GetDataKBNOR210_1_STC_3_1(string F_OrderNo, string F_Part_No, string F_Store_Cd, string F_Supplier, string? Delivery, int F_Use_StockQty)
        {
            using var transaction = _kbContext.Database.BeginTransaction();
            try
            {
                Delivery = "";
                F_Part_No = F_Part_No.Replace("-", "");
                F_Supplier = F_Supplier.Replace("-", "");

                transaction.CreateSavepoint("Start GetDataKBNOR210_1_STC_3_1");

                string _sql = $@"SELECT    F_PDS_No, F_PO_Customer, F_Delivery_Date, F_Part_No, 
                                F_Store_Cd, F_Order_Qty, F_Use_Qty, (F_Order_Qty - F_Use_Qty) As F_Remain_Qty  
                                FROM         TB_STOCK_KB_SPC_PART_TEMP 
                                Where F_PDS_No = '{F_OrderNo}' and Replace(F_Part_No,'-','') = '{F_Part_No}' 
                                Order by  F_PO_Customer,F_Delivery_Date,F_Part_No,F_Store_Cd ";

                var _dt = _FillDT.ExecuteSQL(_sql);

                if (_dt.Rows.Count == 0)
                {
                    await _kbContext.Database.ExecuteSqlRawAsync("EXEC sp_generateorderuseFIFO @p0,@p1,@p2,@p3,@p4,@p5,@p6"
                        , new SqlParameter("@p0", F_OrderNo)
                        , new SqlParameter("@p1", F_Part_No)
                        , new SqlParameter("@p2", F_Store_Cd)
                        , new SqlParameter("@p3", F_Supplier)
                        , new SqlParameter("@p4", Delivery)
                        , new SqlParameter("@p5", F_Use_StockQty)
                        , new SqlParameter("@p6", _BearerClass.UserCode)
                        );
                    transaction.Commit();
                }


                _sql = $@"SELECT F_PDS_No, F_PO_Customer, F_Delivery_Date, F_Part_No, 
                        F_Store_Cd, F_Order_Qty, F_Use_Qty, (F_Order_Qty - F_Use_Qty) As F_Remain_Qty  
                        FROM TB_STOCK_KB_SPC_PART_TEMP 
                        Where F_PDS_No = '{F_OrderNo}' and Replace(F_Part_No,'-','') = '{F_Part_No}' 
                        Order by  F_PO_Customer,F_Delivery_Date,F_Part_No,F_Store_Cd ";

                _dt = _FillDT.ExecuteSQL(_sql);

                if (_dt.Rows.Count == 0)
                {
                    throw new Exception("Data not found");
                }


                return _dt;
            }
            catch (Exception ex)
            {
                transaction.RollbackToSavepoint("Start GetDataKBNOR210_1_STC_3_1");
                throw new Exception(ex.Message);
            }

        }

        public async Task SaveKBNOR210_1_STC_3_1(List<VM_Save_KBNOR210_1_STC_3_1> listObj)
        {
            using var transaction = _kbContext.Database.BeginTransaction();
            try
            {

                transaction.CreateSavepoint("Start SaveKBNOR210_1_STC_3_1");

                await _kbContext.Database.ExecuteSqlRawAsync
                    ($"Delete From TB_STOCK_KB_SPC_PART_TEMP Where F_PDS_No = '{listObj[0].F_PDS_No}' " +
                    $"and F_Part_no = '{listObj[0].F_Part_No}'");

                foreach (var obj in listObj)
                {

                    await _kbContext.Database.ExecuteSqlRawAsync
                    ($"Insert into TB_STOCK_KB_SPC_PART_TEMP " +
                    $"(F_PDS_No, F_PO_Customer, F_Delivery_Date, F_Part_No, F_Store_Cd, " +
                    $"F_Order_Qty, F_Use_Qty, F_Update_By, F_Update_Date) " +
                    $"VALUES ('{obj.F_PDS_No}','{obj.F_PO_Customer}','{obj.F_Delivery_Date}','{obj.F_Part_No}', " +
                    $"'{obj.F_Store_Cd}',{obj.F_Order_Qty},{obj.F_Use_Qty},'{_BearerClass.UserCode}',getDate()) ");

                }

                _log.WriteLogMsg("SaveKBNOR210_1_STC_3_1 | " + JsonConvert.SerializeObject(listObj));

                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.RollbackToSavepoint("Start SaveKBNOR210_1_STC_3_1");
                throw new Exception(ex.Message);
            }

        }

        public string ListDatatogrid(string? SupplierCD = "", string? PartNo = "", string? StoreCode = "", string? StockDate = "", string? UpdateBy = "", string? UpdateDate = "")
        {

            try
            {

                string sql = $@"Select F_Stock_Date ,F_Supplier_code ,F_Part_No ,F_Kanban_No 
                            ,F_Store_CD ,F_Qty_Pack,F_Actual_KB ,F_Actual_PCS ,F_Check_By 
                            ,F_Update_By ,F_Update_Date from FN_getStockRemainSPCData() 
                            Where Isnull(F_Stock_Date,'') <> '' ";

                if (!string.IsNullOrWhiteSpace(SupplierCD))
                {
                    sql += $" and F_Supplier_code = '{SupplierCD}' ";
                }
                if (!string.IsNullOrWhiteSpace(PartNo))
                {
                    sql += $" and F_Part_No = '{PartNo}' ";
                }
                if (!string.IsNullOrWhiteSpace(StoreCode))
                {
                    sql += $" and F_Store_CD = '{StoreCode}' ";
                }
                if (!string.IsNullOrWhiteSpace(StockDate))
                {
                    sql += $" and F_Stock_Date = '{StockDate}' ";
                }
                if (!string.IsNullOrWhiteSpace(UpdateBy))
                {
                    sql += $" and F_Update_By = '{UpdateBy}' ";
                }
                if (!string.IsNullOrWhiteSpace(UpdateDate))
                {
                    sql += $" and Convert(varchar(8),F_Update_Date,112) = '{UpdateDate}' ";
                }

                sql += "Order by F_Stock_Date,F_Part_No,F_Supplier_Code ";


                var dt = _FillDT.ExecuteSQL(sql);

                if (dt.Rows.Count == 0)
                {
                    throw new Exception("Data not found");
                }

                return JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public string STC_1_GetSupplierCode(bool isNew, string StockDate)
        {

            try
            {

                if (isNew)
                {
                    string sql = $@"SELECT  F_Supplier_Cd + '-' + F_Plant AS F_Supplier 
                                FROM VW_PPMDB_BOM Where  F_Supplier_CD <> '' and (F_CKD_Str <= '{StockDate}'
                                and F_CKD_End >= '{StockDate}' ) GROUP BY F_Supplier_Cd, F_Plant ORDER BY F_Supplier_Cd, F_Plant ";

                    var dt = _FillDT.ExecuteSQL(sql);

                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("Data not found");
                    }

                    return JsonConvert.SerializeObject(dt);

                }
                else
                {
                    string sql = $@"SELECT  F_Supplier_Code As F_Supplier 
                                FROM FN_getStockRemainSPCData() 
                                Where  F_STock_Date = '{StockDate}'
                                GROUP BY  F_Supplier_Code 
                                ORDER BY F_Supplier_Code ";

                    var dt = _FillDT.ExecuteSQL(sql);

                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("Data not found");
                    }

                    return JsonConvert.SerializeObject(dt);

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public string STC_1_GetPartNo(bool isNew, string StockDate, string? Supplier_Code)
        {

            try
            {

                if (isNew)
                {
                    string sql = $@"SELECT RTRIM(F_Part_no) + '-' + F_Ruibetsu AS F_Part_no  
                                FROM  VW_PPMDB_BOM 
                                Where  (F_CKD_str <= '{StockDate}' and F_CKD_End >= '{StockDate}' )";

                    if (!string.IsNullOrWhiteSpace(Supplier_Code))
                    {
                        sql += $" and F_Supplier_cd = '{Supplier_Code.Split('-')[0]}' and F_Plant = '{Supplier_Code.Split('-')[1]}' ";
                    }

                    sql += "Group BY F_Part_no,F_Ruibetsu ORDER BY F_Part_no,F_Ruibetsu ";

                    var dt = _FillDT.ExecuteSQL(sql);

                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("Data not found");
                    }

                    return JsonConvert.SerializeObject(dt);

                }
                else
                {
                    string sql = $@"SELECT     RTRIM(F_Part_no)  AS F_Part_no  
                                FROM FN_getStockRemainSPCData() 
                                Where  F_STock_Date = '{StockDate}'";

                    if (!string.IsNullOrWhiteSpace(Supplier_Code))
                    {
                        sql += $" and F_Supplier_Code = '{Supplier_Code}' ";
                    }

                    sql += @" GROUP BY  F_Part_no 
                                ORDER BY F_Part_no ";

                    var dt = _FillDT.ExecuteSQL(sql);

                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("Data not found");
                    }

                    return JsonConvert.SerializeObject(dt);

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public string STC_1_GetStore(bool isNew, string StockDate, string? Supplier_Code, string? Part_No)
        {

            try
            {

                if (isNew)
                {
                    string sql = $@"SELECT F_Store_CD  
                                FROM  VW_PPMDB_BOM 
                                Where (F_CKD_str <= '{StockDate}' and F_CKD_End >= '{StockDate}' ) ";

                    if (!string.IsNullOrWhiteSpace(Supplier_Code))
                    {
                        sql += $" and F_Supplier_cd = '{Supplier_Code.Split('-')[0]}' and F_Plant = '{Supplier_Code.Split('-')[1]}' ";
                    }

                    if (!string.IsNullOrWhiteSpace(Part_No))
                    {
                        sql += $" and Rtrim(F_Part_no) = '{Part_No.Split('-')[0]}' and F_Ruibetsu = '{Part_No.Split('-')[1]}' ";
                    }

                    sql += "Group BY F_Store_CD ORDER BY F_Store_CD ";

                    var dt = _FillDT.ExecuteSQL(sql);

                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("Data not found");
                    }

                    return JsonConvert.SerializeObject(dt);

                }
                else
                {
                    string sql = $@"SELECT  F_Store_CD
                                FROM FN_getStockRemainSPCData() 
                                Where  F_STock_Date = '{StockDate}'";

                    if (!string.IsNullOrWhiteSpace(Supplier_Code))
                    {
                        sql += $" and F_Supplier_Code = '{Supplier_Code}' ";
                    }
                    if (!string.IsNullOrWhiteSpace(Part_No))
                    {
                        sql += $" and Rtrim(F_Part_no) = '{Part_No.Split('-')[0]}' and F_Ruibetsu = '{Part_No.Split('-')[1]}' ";
                    }

                    sql += @"  GROUP BY  F_Store_CD 
                                ORDER BY F_Store_CD ";

                    var dt = _FillDT.ExecuteSQL(sql);

                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("Data not found");
                    }

                    return JsonConvert.SerializeObject(dt);

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public string STC_1_GetSupplierName(string Supplier_Code)
        {
            try
            {
                var Supplier_Name = _kbContext.Database.SqlQueryRaw<string>
                    ("Select Top 1 Rtrim(F_name) As Value From VW_PPMDB_BOM Where F_supplier_cd + '-' + F_plant = {0}", Supplier_Code)
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(Supplier_Name))
                {
                    throw new Exception("Supplier Name not found");
                }

                return Supplier_Name;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public string STC_1_GetPartName(string Supplier_Code, string Part_No)
        {
            try
            {

                var Part_Name = _kbContext.Database.SqlQueryRaw<string>
                    ("Select F_Part_NM AS VALUE From VW_PPMDB_BOM Where  F_supplier_cd+'-'+F_plant = {0} " +
                    " and Rtrim(F_Part_No)+'-'+Rtrim(F_Ruibetsu) = {1} ",
                    Supplier_Code, Part_No).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(Part_Name))
                {
                    throw new Exception("Part Name not found");
                }

                return Part_Name;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string STC_1_GetKB_Qty(string Supplier_Code, string Part_No, string StockDate, string Store_CD)
        {
            try
            {
                string sql = $@"SELECT   F_Qty_Box, F_Kanban_No 
                            FROM  VW_PPMDB_BOM 
                            Where  F_Supplier_cd + '-' + F_Plant = '{Supplier_Code}' 
                            and (F_CKD_str <= '{StockDate}' and F_CKD_End >= '{StockDate}') 
                            and Rtrim(F_Part_no)+'-'+F_Ruibetsu = '{Part_No}' 
                            and F_Store_Cd = '{Store_CD}' 
                            Group BY F_Qty_Box, F_Kanban_No 
                            ORDER BY F_Qty_Box, F_Kanban_No ";


                var dt = _FillDT.ExecuteSQL(sql);

                if (dt.Rows.Count == 0)
                {
                    throw new Exception("Data not found");
                }

                return JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task STC_1_Save(VM_Post_KBNOR210_STC_1 obj)
        {

            try
            {

                if (obj.Action.ToLower() == "new")
                {

                    var existData = await _kbContext.TB_STOCK_KB_SPC_PART_REMAIN
                        .FirstOrDefaultAsync(x => x.F_Supplier_CD.Trim()
                        + "-" + x.F_Supplier_Plant.Trim() == obj.Supplier_Code
                        && x.F_Part_no.Trim() + "-" + x.F_Ruibetsu.Trim() == obj.Part_No
                        && x.F_Store_Code == obj.Store_CD);

                    if (existData != null)
                    {
                        var prevStock = existData.F_Prev_Stock_Qty;
                        existData.F_Stock_Qty = obj.Actual_PCS == 0 ? 0 : obj.Actual_PCS;
                        existData.F_Stock_Date = obj.Stock_Date;
                        existData.F_Prev_Stock_Qty = prevStock;
                        existData.F_Update_By = _BearerClass.UserCode;
                        existData.F_Update_Date = DateTime.Now;
                        existData.F_Kanban_no = obj.Kanban_No;
                        existData.F_Package = obj.Qty_Pack;
                        existData.F_Check_By = obj.Check_Stock_By;
                        existData.F_Remark = obj.Remark;

                        _kbContext.TB_STOCK_KB_SPC_PART_REMAIN.Update(existData);
                    }

                    else
                    {
                        var data = new TB_STOCK_KB_SPC_PART_REMAIN
                        {
                            F_Supplier_CD = obj.Supplier_Code.Split("-")[0],
                            F_Supplier_Plant = obj.Supplier_Code.Split("-")[1],
                            F_Part_no = obj.Part_No.Split("-")[0],
                            F_Ruibetsu = obj.Part_No.Split("-")[1],
                            F_Store_Code = obj.Store_CD,
                            F_Stock_Qty = obj.Actual_PCS == 0 ? 0 : obj.Actual_PCS,
                            F_Stock_Date = obj.Stock_Date,
                            F_Prev_Stock_Qty = 0,
                            F_Update_By = _BearerClass.UserCode,
                            F_Update_Date = DateTime.Now,
                            F_Kanban_no = obj.Kanban_No,
                            F_Package = obj.Qty_Pack,
                            F_Check_By = obj.Check_Stock_By,
                            F_Remark = obj.Remark
                        };

                        _kbContext.TB_STOCK_KB_SPC_PART_REMAIN.Add(data);
                    }

                }

                else if (obj.Action.ToLower() == "upd")
                {

                    var existData = await _kbContext.TB_STOCK_KB_SPC_PART_REMAIN
                        .FirstOrDefaultAsync(x => x.F_Supplier_CD.Trim()
                        + "-" + x.F_Supplier_Plant.Trim() == obj.Supplier_Code
                        && x.F_Part_no.Trim() + "-" + x.F_Ruibetsu.Trim() == obj.Part_No
                        && x.F_Store_Code == obj.Store_CD);

                    if (existData != null)
                    {

                        var prevStock = existData.F_Prev_Stock_Qty;
                        existData.F_Stock_Qty = obj.Actual_PCS == 0 ? 0 : obj.Actual_PCS;
                        existData.F_Stock_Date = obj.Stock_Date;
                        existData.F_Prev_Stock_Qty = prevStock;
                        existData.F_Update_By = _BearerClass.UserCode;
                        existData.F_Update_Date = DateTime.Now;
                        existData.F_Kanban_no = obj.Kanban_No;
                        existData.F_Package = obj.Qty_Pack;
                        existData.F_Check_By = obj.Check_Stock_By;
                        existData.F_Remark = obj.Remark;

                        _kbContext.TB_STOCK_KB_SPC_PART_REMAIN.Update(existData);

                    }
                    else
                    {
                        throw new Exception("Data not found");
                    }

                }

                else
                {

                    var existData = await _kbContext.TB_STOCK_KB_SPC_PART_REMAIN
                        .FirstOrDefaultAsync(x => x.F_Supplier_CD.Trim()
                        + "-" + x.F_Supplier_Plant.Trim() == obj.Supplier_Code
                        && x.F_Part_no.Trim() + "-" + x.F_Ruibetsu.Trim() == obj.Part_No
                        && x.F_Store_Code == obj.Store_CD);

                    if (existData != null)
                    {
                        _kbContext.TB_STOCK_KB_SPC_PART_REMAIN.Remove(existData);
                    }
                    else
                    {
                        throw new Exception("Data not found");
                    }
                }

                await _kbContext.SaveChangesAsync();

                _log.WriteLogMsg("STC_1_Save | " + JsonConvert.SerializeObject(obj));

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task STC_1_Import(List<VM_Import_KBNOR210_STC_1> listObj)
        {
            using var transaction = _kbContext.Database.BeginTransaction();

            try
            {

                foreach (var obj in listObj)
                {
                    VM_Post_KBNOR210_STC_1 _obj = new VM_Post_KBNOR210_STC_1
                    {
                        Action = "New",
                        Stock_Date = obj.StockDate,
                        Part_No = obj.PartNo + "-" + obj.Ruibetsu,
                        Supplier_Code = obj.Supp_CD + "-" + obj.Supp_Plant,
                        Store_CD = obj.StoreCd,
                        Actual_PCS = obj.StockQty,
                        Remark = null,
                        Check_Stock_By = null,
                    };

                    string sql = $@"SELECT (TOP 1)  F_Qty_Box, F_Kanban_No 
                            FROM  VW_PPMDB_BOM 
                            Where  F_Supplier_cd + '-' + F_Plant = '{_obj.Supplier_Code}' 
                            and (F_CKD_str <= '{_obj.Stock_Date}' and F_CKD_End >= '{_obj.Stock_Date}') 
                            and Rtrim(F_Part_no)+'-'+F_Ruibetsu = '{_obj.Part_No}' 
                            and F_Store_Cd = '{_obj.Store_CD}' 
                            Group BY F_Qty_Box, F_Kanban_No 
                            ORDER BY F_Qty_Box, F_Kanban_No ";

                    var getData = _FillDT.ExecuteSQL(sql);

                    if (getData.Rows.Count == 0)
                    {
                        throw new Exception("Data not found");
                    }

                    _obj.Kanban_No = getData.Rows[0]["F_Kanban_No"].ToString();
                    _obj.Qty_Pack = Convert.ToInt32(getData.Rows[0]["F_Qty_Box"]);

                    await STC_1_Save(_obj);

                }

                transaction.Commit();
                _log.WriteLogMsg("STC_1_Import | " + JsonConvert.SerializeObject(listObj));

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _log.WriteLogMsg("STC_1_Import ERROR!! | " + JsonConvert.SerializeObject(listObj));
                throw new Exception(ex.Message);
            }

        }

        public string STC_2_LoadSupplier(string Type, bool chkFlg, bool chkFlgDT, string DateFrom, string DateTo)
        {
            try
            {
                string sql = "";

                if (Type == "History")
                {
                    sql = $@"Select  Rtrim(F_Supplier_CD)+'-'+Rtrim(F_Supplier_Plant) AS F_Supplier 
                                From  TB_STOCK_KB_SPC_PART_HISTORY 
                                Where F_Type = 'TRN' ";
                    if (chkFlg)
                    {
                        sql += $" and F_Delivery_Date between '{DateFrom}' and '{DateTo}' ";
                    }

                    sql += "Group by  F_Supplier_CD, F_Supplier_Plant " +
                        "Order by F_Supplier_CD,F_Supplier_Plant ";
                }

                else
                {

                    sql = $@"Select  F_Supplier_Code  AS F_Supplier 
                                From  FN_getStockRemainSPCData() 
                                Where F_Supplier_Code <> ''";

                    if (chkFlgDT)
                    {
                        sql += $@" and F_Stock_Date between '{DateFrom}' and '{DateTo}' ";
                    }

                    sql += "Group by  F_Supplier_Code " +
                        "Order by F_Supplier_Code ";

                }

                var dt = _FillDT.ExecuteSQL(sql);

                if (dt.Rows.Count == 0)
                {
                    throw new Exception("Data not found");
                }

                return JsonConvert.SerializeObject(dt);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string STC_2_LoadPartNo(string Type, bool chkFlg, bool chkFlgDT, string DateFrom, string DateTo, string SuppF, string SuppT)
        {

            try
            {
                string sql = "";

                if (Type == "History")
                {
                    sql = $@"Select  Rtrim(F_Part_No)+'-'+Rtrim(F_Ruibetsu) AS F_Part_No 
                                From  TB_STOCK_KB_SPC_PART_HISTORY 
                                Where F_Type = 'TRN' 
                                and Rtrim(F_Supplier_CD)+'-'+Rtrim(F_Supplier_Plant) between '{SuppF}' and '{SuppT}' ";

                    if (chkFlg)
                    {
                        sql += $" and F_Delivery_Date between '{DateFrom}' and '{DateTo}' ";
                    }

                    sql += "Group by  F_Part_No, F_Ruibetsu " +
                        "Order by F_Part_No, F_Ruibetsu ";
                }

                else
                {

                    sql = $@"Select  F_Part_No 
                                From  FN_getStockRemainSPCData() 
                                Where F_Part_no <> '' 
                                and F_Supplier_Code between '{SuppF}' and '{SuppT}' ";

                    if (chkFlgDT)
                    {
                        sql += $@" and F_Stock_Date between '{DateFrom}' and '{DateTo}' ";
                    }

                    sql += "Group by  F_Part_No " +
                        "Order by F_Part_No ";

                }

                var dt = _FillDT.ExecuteSQL(sql);

                if (dt.Rows.Count == 0)
                {
                    throw new Exception("Data not found");
                }

                return JsonConvert.SerializeObject(dt);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string Del_LoadSupplier(string DeliYM)
        {
            try
            {

                string _sql = $@"Select RTrim(F_Supplier_CD)+'-'+Rtrim(F_Supplier_Plant) As F_Supplier 
                                From  TB_PO_Merge_Delete 
                                Where F_Delivery_Date like '{DeliYM}%' 
                                Group by F_Supplier_CD,F_Supplier_Plant 
                                Order by F_Supplier_CD,F_Supplier_Plant ";

                var _dt = _FillDT.ExecuteSQL(_sql);

                if (_dt.Rows.Count == 0)
                {
                    throw new Exception("Data not found");
                }

                return JsonConvert.SerializeObject(_dt);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string Del_LoadPartNo(string DeliYM, string? Supplier)
        {
            try
            {

                string _sql = $@"Select RTrim(F_Part_No) As F_Part_No 
                                From  TB_PO_Merge_Delete 
                                Where F_Delivery_Date like '{DeliYM}%' 
                                AND F_Customer_OrderNo <> '' ";

                if (!string.IsNullOrWhiteSpace(Supplier))
                {
                    _sql += $" and ( F_Supplier_CD = '{Supplier.Split("-")[0]}' and F_Supplier_Plant = '{Supplier.Split("-")[1]}' ) ";
                }

                _sql += "Group by F_Part_No " +
                    "Order by F_Part_No ";

                var _dt = _FillDT.ExecuteSQL(_sql);

                if (_dt.Rows.Count == 0)
                {
                    throw new Exception("Data not found");
                }

                return JsonConvert.SerializeObject(_dt);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}