using ClosedXML.Excel;
using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.ImportData.Model;
using KANBAN.Models.KB3.ImportData.ViewModel;
using KANBAN.Models.KB3.Master;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.Proc_DB;
using KANBAN.Services.Import.Interface;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;

namespace KANBAN.Services.Import.Repository
{
    public class KBNIM012M : IKBNIM012M
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly ProcDBContext _procDBContext;


        public KBNIM012M
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            ProcDBContext procContext
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _procDBContext = procContext;
        }

        public async Task<LoadMonthlyForecast> FormLoadKBNIM012M(string database)
        {
            try
            {
                var loadMonthly = new LoadMonthlyForecast();

                if (database == "ProcDb")
                {
                    var result = await _procDBContext.VW_MaxVersionForecast
                                                     .OrderByDescending(x => x.F_PO)
                                                     .FirstOrDefaultAsync();

                    DateTime prodYm = DateTime.ParseExact(result.F_PO.Substring(0, 6), "yyyyMM", null);

                    loadMonthly.Version = result.F_Version;
                    loadMonthly.Revision = result.F_PO.Substring(6);
                    loadMonthly.ProdYM = prodYm.ToString("MM/yyyy");

                    return loadMonthly;
                }
                else
                {
                    var result = await _kbContext.TB_Import_Forecast
                                .OrderByDescending(x => x.F_Production_date)
                                .ThenBy(x => x.F_Version)
                                .ThenByDescending(x => x.F_revision_no)
                                .FirstOrDefaultAsync();

                    DateTime prodYm = DateTime.ParseExact(result.F_Production_date.Trim(), "yyyyMM", null);

                    loadMonthly.Version = result.F_Version.ToString() == "C" ? "CONFIRM" : "TENTATIVE";
                    loadMonthly.Revision = result.F_revision_no;
                    loadMonthly.ProdYM = prodYm.ToString("MM/yyyy");




                    return loadMonthly;

                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<string>> Supplier_DropDown()
        {
            try
            {
                var result = await _kbContext.TB_Import_Forecast
                    .Select(x => x.F_Supplier_code.Substring(1, 4) + "-" + x.F_supplier_plant.ToString().TrimEnd())
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<string>> KanBan_DropDown(string supplierCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(supplierCode))
                {
                    var result = await _kbContext.TB_Import_Forecast
                                                .Select(x => x.F_sebango)
                                                .Distinct()
                                                .OrderBy(x => x)
                                                .ToListAsync();

                    return result;
                }
                else
                {
                    var result = await _kbContext.TB_Import_Forecast
                                                .Where(x => x.F_Supplier_code.Substring(1, 4) + "-" + x.F_supplier_plant.ToString().TrimEnd() == supplierCode)
                                                .Select(x => x.F_sebango)
                                                .Distinct()
                                                .OrderBy(x => x)
                                                .ToListAsync();

                    return result;
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<T_Supplier_MS> SupplierName(string supplierCode)
        {
            try
            {
                var result = await _PPM3Context.T_Supplier_MS
                                    .Where(x => x.F_supplier_cd.TrimEnd() + "-" + x.F_Plant_cd.ToString().TrimEnd() == supplierCode)
                                    .FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<string>> StoreCode_DropDown(string? supplierCode, string? kanbanNo)
        {
            try
            {
                var query = _kbContext.TB_Import_Forecast.AsQueryable();

                if (!string.IsNullOrWhiteSpace(supplierCode))

                    query = query.Where(x => x.F_Supplier_code.Substring(1, 4) + "-" + x.F_supplier_plant.ToString().TrimEnd() == supplierCode);


                if (!string.IsNullOrWhiteSpace(kanbanNo))

                    query = query.Where(x => x.F_sebango == kanbanNo);


                var result = await query
                    .Select(x => x.F_Store_cd)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<string>> PartNoList(string supplierCode, string storeCode, string kanbanNo)
        {
            try
            {
                var query = _kbContext.TB_Import_Forecast.AsQueryable();

                if (!string.IsNullOrWhiteSpace(supplierCode))
                    query = query.Where(x => x.F_Supplier_code.Substring(1, 4) + "-" + x.F_supplier_plant.ToString().TrimEnd() == supplierCode);


                if (!string.IsNullOrWhiteSpace(storeCode))
                    query = query.Where(x => x.F_Store_cd == storeCode);


                if (!string.IsNullOrWhiteSpace(kanbanNo))
                    query = query.Where(x => x.F_sebango == kanbanNo);

                var result = await query
                                    .Select(x => x.F_part_no.TrimEnd() + "-" + x.F_ruibetsu.TrimEnd())
                                    .Distinct()
                                    .OrderBy(x => x)
                                    .ToListAsync();

                return result;

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<string> PartName(string partNo)
        {
            try
            {
                var result = await _PPM3Context.T_Construction
                                    .Where(x => x.F_Part_no.Trim() + "-" + x.F_Ruibetsu.Trim() == partNo)
                                    .Select(x => x.F_Part_nm)
                                    .Distinct()
                                    .OrderBy(x => x)
                                    .FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TB_IMPORT_FORECAST_TEMP>> Search(VM_KBNIM012M model)
        {
            using var transaction = await _kbContext.Database.BeginTransactionAsync();
            try
            {

                var userID = _BearerClass.UserCode;
                var sPlant = _BearerClass.Plant;

                await _kbContext.Database.ExecuteSqlRawAsync($"DELETE FROM TB_IMPORT_FORECAST_TEMP WHERE F_Update_By = '{userID}'");

                //----Select Data----

                // ดึงค่า Server Name และ Database Name
                var connection = _PPM3Context.Database.GetDbConnection();
                string server = connection.DataSource;
                string database = connection.Database;

                string sql = $@"
                                INSERT INTO TB_IMPORT_FORECAST_TEMP(
                                    F_production_date, F_revision_no, F_Supplier_Code, F_Short_Name, F_Part_No, F_Part_Name, 
                                    F_Sebango, F_Delivery_qty, F_cycle_supply, F_Amount_MD1, F_Amount_MD2, F_Amount_MD3, F_Amount_MD4,
                                    F_Amount_MD5, F_Amount_MD6, F_Amount_MD7, F_Amount_MD8, F_Amount_MD9, F_Amount_MD10, F_Amount_MD11, 
                                    F_Amount_MD12, F_Amount_MD13, F_Amount_MD14, F_Amount_MD15, F_Amount_MD16, F_Amount_MD17, F_Amount_MD18,
                                    F_Amount_MD19, F_Amount_MD20, F_Amount_MD21, F_Amount_MD22, F_Amount_MD23, F_Amount_MD24, F_Amount_MD25, 
                                    F_Amount_MD26, F_Amount_MD27, F_Amount_MD28, F_Amount_MD29, F_Amount_MD30, F_Amount_MD31, F_Store_cd, 
                                    F_Amount_M, F_Amount_M1, F_Amount_M2, F_Amount_M3, F_Lock, F_Update_By, F_Update_Date)
                                SELECT  F_production_date
                                       ,F_revision_no,substring(F_Supplier_code,2,4)+'-'+ F_supplier_plant AS F_Supplier_Code
                                       ,S.F_Short_Name,rtrim(F.F_part_no)+'-'+ rtrim(F.F_ruibetsu) AS F_Part_No,rtrim(C.F_PART_NM) AS F_Part_Name
                                       ,F_sebango AS F_Sebango,F_Delivery_qty ,'0'+F_Cycle_Supply AS F_cycle_supply,F_Amount_MD1,F_Amount_MD2
                                       ,F_Amount_MD3,F_Amount_MD4,F_Amount_MD5,F_Amount_MD6,F_Amount_MD7,F_Amount_MD8,F_Amount_MD9,F_Amount_MD10
                                       ,F_Amount_MD11,F_Amount_MD12,F_Amount_MD13,F_Amount_MD14,F_Amount_MD15,F_Amount_MD16,F_Amount_MD17,F_Amount_MD18
                                       ,F_Amount_MD19,F_Amount_MD20,F_Amount_MD21,F_Amount_MD22,F_Amount_MD23,F_Amount_MD24,F_Amount_MD25,F_Amount_MD26
                                       ,F_Amount_MD27,F_Amount_MD28,F_Amount_MD29,F_Amount_MD30,F_Amount_MD31,F.F_Store_cd,F_Amount_M,F_Amount_M1
                                       ,F_Amount_M2,F_Amount_M3,0 AS F_Lock
                                       ,'{userID}' AS F_Update_BY
                                       ,getdate() AS F_Update_Date
                                FROM TB_IMPORT_FORECAST F
                                INNER JOIN
                                (
	                                SELECT  F_PART_NO
	                                       ,F_Ruibetsu
	                                       ,F_Store_Cd
	                                       ,F_PART_NM
	                                       ,F_Supplier_CD
	                                FROM [{server}].[{database}].dbo.T_Construction
	                                WHERE F_Local_Str <= convert(Char(8), Getdate(), 112)
	                                AND F_Local_End >= convert(Char(8), Getdate(), 112)
	                                AND F_Supplier_Cd <> '9997'
                                )C
                                ON F.F_PART_NO = C.F_PART_NO AND F.F_Ruibetsu = C.F_Ruibetsu AND F.F_Store_Cd = C.F_Store_Cd AND Substring(F.F_Supplier_Code, 2, 4) = C.F_SUpplier_Cd
                                INNER JOIN
                                (
	                                SELECT  F_Supplier_Cd
	                                       ,F_Plant_CD
	                                       ,F_Store_CD
	                                       ,F_Short_name
	                                FROM [{server}].[{database}].dbo.T_Supplier_Ms
	                                WHERE F_TC_Str <= convert(Char(8), Getdate(), 112)
	                                AND F_TC_End >= convert(Char(8), Getdate(), 112)
                                )S
                                ON substring(F_Supplier_code, 2, 4) = S.F_Supplier_Cd AND F.F_supplier_plant = S.F_Plant_CD AND F.F_Store_Cd = S.F_Store_Cd
                                WHERE F_production_date = '{model.ProdYM}'
                                AND F_revision_no = '{model.Revision}'
                                AND F.F_Store_Cd LIKE '{sPlant}%'

                                ";

                if (model.Condition == "ConditionDetail")
                {
                    if (!string.IsNullOrWhiteSpace(model.SupplierCode))
                        sql += $@"AND substring(F.F_Supplier_code, 2, 4)+'-'+ F.F_supplier_plant = '{model.SupplierCode}' ";

                    if (!string.IsNullOrWhiteSpace(model.KanbanNo))
                        sql += $@"AND F.F_Sebango = '{model.KanbanNo}' ";

                    if (!string.IsNullOrWhiteSpace(model.StoreCode))
                        sql += $@"AND F.F_Store_Cd = '{model.StoreCode}' ";

                    if (!string.IsNullOrWhiteSpace(model.PartNo))
                        sql += $@"AND rtrim(F.F_part_no)+'-'+ rtrim(F.F_ruibetsu) = '{model.PartNo}' ";
                }

                if (model.Condition == "ConditionSupplier")
                {
                    if (!string.IsNullOrWhiteSpace(model.SupplierCode))
                        sql += $@"AND substring(F.F_Supplier_code, 2, 4)+'-'+ F.F_supplier_plant = '{model.SupplierCode}' ";

                    if (!string.IsNullOrWhiteSpace(model.StoreCode))
                        sql += $@"AND F.F_Store_Cd = '{model.StoreCode}' ";
                }

                await _kbContext.Database.ExecuteSqlRawAsync(sql);
       

                var cycle_supply = await _kbContext.TB_IMPORT_FORECAST_TEMP
                                                   .Where(x => x.F_Update_By == userID)
                                                   .Distinct()
                                                   .Select(x => x.F_cycle_supply)
                                                   .ToListAsync();

                int nDate_Adv = 0;
                string nDate = "";
                string nLast = await GetLastOrder();

                foreach (var cycle in cycle_supply)
                {
                    nDate_Adv = CalculateDate(cycle);
                    nDate = await Get_AdvDelivery(nLast, nDate_Adv);

                    if (nDate.Substring(0, 6) == model.Txt_ProdYM)
                    {
                        sql = $@"
                                 UPDATE TB_IMPORT_FORECAST_TEMP 
                                 SET F_Lock = {int.Parse(nDate.Substring(6, 2))}  
                                 WHERE F_Update_By = '{userID}'
                                 AND F_Cycle_Supply = '{cycle}'
                                ";
                        await _kbContext.Database.ExecuteSqlRawAsync(sql);

                    }

                }

                await transaction.CommitAsync();

                var result = await _kbContext.TB_IMPORT_FORECAST_TEMP.Where(x => x.F_Update_By == userID).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public int CalculateDate(string sCycle)
        {
            int nDate = 0;

            if (sCycle.Substring(0, 2) == "01")
            {
                if (sCycle.Substring(2, 2) == "01") // Example: 01-01-03 = 3
                {
                    nDate = int.Parse(sCycle.Substring(4, 2));
                }
                else // Example: 01-04-08 = 2, 01-12-06 = 1
                {
                    double c = double.Parse(sCycle.Substring(4, 2));
                    double b = double.Parse(sCycle.Substring(2, 2));
                    nDate = (int)Math.Round(c / b, MidpointRounding.AwayFromZero);
                }
            }
            else // Example: 05-01-01 = 5
            {
                nDate = int.Parse(sCycle.Substring(0, 2));
            }

            nDate = nDate + 1; // Adjust according to business logic
            return nDate;
        }

        public async Task<string> GetLastOrder()
        {
            var result = await _kbContext.TB_MS_Parameter
                .Where(p => p.F_Code == "LO")
                .Select(p => p.F_Value3.Substring(0, 8))
                .FirstOrDefaultAsync();

            return result?.Trim() ?? "";
        }


        public async Task<string> Get_AdvDelivery(string sDeliveryDate, int sAdv)
        {
            try
            {
                var userID = _BearerClass.UserCode;
                var sPlant = _BearerClass.Plant;

                string nDeliAdv = "";
                int nCount = int.Parse(sDeliveryDate.Substring(6, 2));
                bool flagNext = false;

            Run_Again:
                var query = _kbContext.TB_Calendar.AsQueryable();

                if (flagNext == true)
                    query = query.Where(x => x.F_YM == DateTime.ParseExact(sDeliveryDate, "yyyyMMdd", null).AddMonths(1).ToString("yyyyMM"));

                if (flagNext == false)
                    query = query.Where(x => x.F_YM == sDeliveryDate.Substring(0, 6));

                var storeCd = await _kbContext.TB_MS_Parameter.Where(x => x.F_Code == "SC").FirstOrDefaultAsync();

                var Get_Adv = await query.Where(x => x.F_Store_cd == storeCd.F_Value3).FirstOrDefaultAsync();

                for (int i = 0; i <= sAdv; i++)
                {
                BB_AGAIN:
                    if (nCount > 31)
                    {
                        nCount = 1;
                        sAdv = sAdv - i;
                        flagNext = true;
                        goto Run_Again;
                    }

                    string propName = $"F_workCd_D{nCount}";
                    var prop = typeof(TB_Calendar).GetProperty(propName);
                    string value = prop?.GetValue(Get_Adv)?.ToString();

                    if (value != "0")
                    {
                        nDeliAdv = Get_Adv.F_YM + nCount.ToString("00");
                        nCount++;
                    }
                    else
                    {
                        nCount++;
                        goto BB_AGAIN;
                    }
                }

                if(nDeliAdv == "")
                {
                    sDeliveryDate = new DateTime(int.Parse(sDeliveryDate.Substring(0, 4)), int.Parse(sDeliveryDate.Substring(4, 2)), int.Parse(sDeliveryDate.Substring(6, 2))).AddMonths(-1).ToString("yyyyMM") + "32";
                    goto Run_Again;
                }

                return nDeliAdv;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

       
        public async Task Save(List<TB_IMPORT_FORECAST_TEMP> model)
        {
            using var transaction = await _kbContext.Database.BeginTransactionAsync();
            try
            {
                

                foreach (var temp in model)
                {
                    string sql;
                    for (int i = 1; i <= 31; i++)
                    {
                        var prop = typeof(TB_IMPORT_FORECAST_TEMP).GetProperty($"F_Amount_MD{i}");
                        string amount = prop?.GetValue(temp)?.ToString();

                        sql = $@"
                                 UPDATE TB_Import_Forecast
                                 SET F_Amount_MD{i} = {amount}
                                 WHERE F_Production_Date = '{temp.F_production_date}'
                                 AND F_Revision_No = '{temp.F_revision_no}'
                                 AND substring(F_Supplier_code, 2, 4)+'-'+ F_supplier_plant = '{temp.F_Supplier_Code}'
                                 AND rtrim(F_part_no)+'-'+ rtrim(F_ruibetsu) = '{temp.F_Part_No}'
                                 AND F_Sebango = '{temp.F_Sebango}'
                                ";

                        await _kbContext.Database.ExecuteSqlRawAsync(sql);
                    }

                    string nStart = temp.F_production_date + "01";

                    if (Int32.Parse(nStart) <= Int32.Parse(DateTime.Now.ToString("yyyyMMdd"))) 
                        nStart = DateTime.Now.ToString("yyyyMMdd");

                    string[] suplierCd = temp.F_Supplier_Code.Split("-");
                    string[] partNo = temp.F_Part_No.Split("-");

                    string nSupplier = suplierCd[0];
                    string nPlant = suplierCd[1];
                    string nStore = temp.F_Store_cd;
                    string nEnd = DateTime.ParseExact(temp.F_production_date + "01" , "yyyyMMdd",null).AddMonths(1).AddDays(-1).ToString("yyyyMMdd");
                    string nPart = partNo[0];
                    string nRuibetsu = partNo[1];
                    string nKanban = temp.F_Sebango;

                    sql = $"Exec dbo.SP_ReRun_SP_CalculateForecast_Update '{nStart}','{nEnd}','{nSupplier}','{nPart}','{nStore}','{nPart}','{nRuibetsu}','{nKanban}'";
                    _kbContext.Database.ExecuteSqlRawAsync(sql);

                    await transaction.CommitAsync();

                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task Transfer(VM_KBNIM012M model)
        {
            using var transaction = await _kbContext.Database.BeginTransactionAsync();
            try
            {
                var userID = _BearerClass.UserCode;
                var sPlant = _BearerClass.Plant;

                string nError = "";
                string nDate = "";
                string nLast = await GetLastOrder();
                int nDate_Adv = 0;

                var query = _kbContext.TB_Import_Forecast.AsQueryable();

                if (model.Condition == "ConditionDetail")
                {
                    if (!string.IsNullOrWhiteSpace(model.SupplierCode))
                        query = query.Where(x => x.F_Supplier_code.Substring(1, 4) + "-" + x.F_supplier_plant.ToString().TrimEnd() == model.SupplierCode);

                    if (!string.IsNullOrWhiteSpace(model.KanbanNo))
                        query = query.Where(x => x.F_sebango == model.KanbanNo);

                    if (!string.IsNullOrWhiteSpace(model.StoreCode))
                        query = query.Where(x => x.F_Store_cd == model.StoreCode);

                    if (!string.IsNullOrWhiteSpace(model.PartNo))
                        query = query.Where(x => x.F_part_no.Trim() + x.F_ruibetsu.Trim() == model.PartNo);
                }

                if (model.Condition == "ConditionSupplier")
                {
                    if (!string.IsNullOrWhiteSpace(model.SupplierCode))
                        query = query.Where(x => x.F_Supplier_code.Substring(1, 4) + "-" + x.F_supplier_plant.ToString().TrimEnd() == model.SupplierCode);

                    if (!string.IsNullOrWhiteSpace(model.StoreCode))
                        query = query.Where(x => x.F_Store_cd == model.StoreCode);
                }

                var cycleSupply = await query
                                    .Where(x => x.F_Production_date == model.ProdYM && 
                                                x.F_revision_no == model.Revision && 
                                                x.F_Store_cd.StartsWith(sPlant) )
                                    .Select(x => x.F_cycle_supply)
                                    .Distinct()
                                    .OrderBy(x => x)
                                    .ToListAsync();

                foreach (var cycle_supply in cycleSupply)
                {
                    nDate_Adv = CalculateDate(cycle_supply);
                    nDate = await Get_AdvDelivery(nLast, nDate_Adv);

                    if (Int32.Parse(nDate) >= Int32.Parse(model.Txt_DateF))
                        throw new Exception("Can not Transfer Data Because source Date order already.");
                }

                //------Case can Transfer-----

                string sql = $@"
                                Update TB_IMPORT_FORECAST 
                                SET F_Amount_MD{model.Txt_DateT.Substring(6, 2)} = F_Amount_MD{model.Txt_DateF.Substring(6, 2)} , 
                                    F_Amount_MD{model.Txt_DateF.Substring(6, 2)} = 0 
                                WHERE F_production_date ='{model.Txt_ProdYM}'  
                                AND F_revision_no = '{model.Txt_Rev}' 
                                AND F_Store_Cd like '{sPlant}%'
                                ";

                if (model.Condition == "ConditionDetail")
                {
                    if (!string.IsNullOrWhiteSpace(model.SupplierCode))
                        sql += $@"AND substring(F_Supplier_code, 2, 4)+'-'+ F_supplier_plant = '{model.SupplierCode}' ";

                    if (!string.IsNullOrWhiteSpace(model.KanbanNo))
                        sql += $@"AND F_Sebango = '{model.KanbanNo}' ";

                    if (!string.IsNullOrWhiteSpace(model.StoreCode))
                        sql += $@"AND F_Store_Cd = '{model.StoreCode}' ";

                    if (!string.IsNullOrWhiteSpace(model.PartNo))
                        sql += $@"AND rtrim(F_part_no)+'-'+ rtrim(F_ruibetsu) = '{model.PartNo}' ";
                }

                if (model.Condition == "ConditionSupplier")
                {
                    if (!string.IsNullOrWhiteSpace(model.SupplierCode))
                        sql += $@"AND substring(F_Supplier_code, 2, 4)+'-'+ F_supplier_plant = '{model.SupplierCode}' ";

                    if (!string.IsNullOrWhiteSpace(model.StoreCode))
                        sql += $@"AND F_Store_Cd = '{model.StoreCode}' ";
                }

                await _kbContext.Database.ExecuteSqlRawAsync(sql);
                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task Report(VM_KBNIM012M model)
        {
          
            try
            {
                var userID = _BearerClass.UserCode;
                var sPlant = _BearerClass.Plant;

                await _kbContext.Database.ExecuteSqlRawAsync($"DELETE FROM [HMMTA-PPM].[NEW_KANBAN_F3].[DBO].RPT_KBNIM_012M WHERE F_Update_By = '{userID}'");

                // ดึงค่า Server Name และ Database Name
                var connection = _kbContext.Database.GetDbConnection();
                string server = connection.DataSource;
                string database = connection.Database;

                string sql = $@"
                                INSERT INTO [HMMTA-PPM].[NEW_KANBAN_F3].[DBO].RPT_KBNIM_012M
                                SELECT  TB_Import_Forecast.F_Supplier_code
                                       ,TB_Import_Forecast.F_Store_cd
                                       ,TB_Import_Forecast.F_part_no
                                       ,TB_Import_Forecast.F_sebango
                                       ,TB_Import_Forecast.F_Delivery_qty
                                       ,TB_Import_Forecast.F_cycle_supply
                                       ,TB_Import_Forecast.F_Amount_MD1
                                       ,TB_Import_Forecast.F_Amount_MD2
                                       ,TB_Import_Forecast.F_Amount_MD3
                                       ,TB_Import_Forecast.F_Amount_MD4
                                       ,TB_Import_Forecast.F_Amount_MD5
                                       ,TB_Import_Forecast.F_Amount_MD6
                                       ,TB_Import_Forecast.F_Amount_MD7
                                       ,TB_Import_Forecast.F_Amount_MD8
                                       ,TB_Import_Forecast.F_Amount_MD9
                                       ,TB_Import_Forecast.F_Amount_MD10
                                       ,TB_Import_Forecast.F_Amount_MD11
                                       ,TB_Import_Forecast.F_Amount_MD12
                                       ,TB_Import_Forecast.F_Amount_MD13
                                       ,TB_Import_Forecast.F_Amount_MD14
                                       ,TB_Import_Forecast.F_Amount_MD15
                                       ,TB_Import_Forecast.F_Amount_MD16
                                       ,TB_Import_Forecast.F_Amount_MD17
                                       ,TB_Import_Forecast.F_Amount_MD18
                                       ,TB_Import_Forecast.F_Amount_MD19
                                       ,TB_Import_Forecast.F_Amount_MD20
                                       ,TB_Import_Forecast.F_Amount_MD21
                                       ,TB_Import_Forecast.F_Amount_MD22
                                       ,TB_Import_Forecast.F_Amount_MD23
                                       ,TB_Import_Forecast.F_Amount_MD24
                                       ,TB_Import_Forecast.F_Amount_MD25
                                       ,TB_Import_Forecast.F_Amount_MD26
                                       ,TB_Import_Forecast.F_supplier_plant
                                       ,TB_Import_Forecast.F_ruibetsu
                                       ,TB_Import_Forecast.F_Amount_MD27
                                       ,TB_Import_Forecast.F_Amount_MD28
                                       ,TB_Import_Forecast.F_Amount_MD29
                                       ,TB_Import_Forecast.F_Amount_MD30
                                       ,TB_Import_Forecast.F_Amount_MD31
                                       ,TB_Import_Forecast.F_Amount_M
                                       ,TB_Import_Forecast.F_Amount_M1
                                       ,TB_Import_Forecast.F_Amount_M2
                                       ,TB_Import_Forecast.F_Amount_M3
                                       ,TMP_Supplier_MS.F_short_name
                                       ,TB_Import_Forecast.F_production_date
                                       ,TB_Import_Forecast.F_revision_no
	                                   ,'{userID}' AS F_Update_By
                                FROM [{server}].[{database}].[dbo].TB_Import_Forecast TB_Import_Forecast 
                                INNER JOIN
                                (
	                                SELECT  distinct F_Supplier_cd
	                                       ,F_Plant_Cd
	                                       ,F_Store_Cd
	                                       ,F_Short_name
	                                FROM [{server}].[{database}].[dbo].TMP_Supplier_MS
                                ) TMP_Supplier_MS
                                ON substring(TB_Import_Forecast.F_Supplier_code, 2, 4) = TMP_Supplier_MS.F_supplier_cd 
                                AND TB_Import_Forecast.F_supplier_plant = TMP_Supplier_MS.F_Plant_cd 
                                AND TB_Import_Forecast.F_Store_cd = TMP_Supplier_MS.F_Store_cd
                                WHERE 0 = 0 
                                AND F_production_date = '{model.Txt_ProdYM}' 
                                --AND F_revision_no = '{model.Txt_Rev}'
                                ";

                if (model.Condition == "ConditionDetail")
                {
                    if (!string.IsNullOrWhiteSpace(model.SupplierCode))
                        sql += $@"AND substring(TB_Import_Forecast.F_Supplier_code, 2, 4)+'-'+ TB_Import_Forecast.F_supplier_plant = '{model.SupplierCode}' ";

                    if (!string.IsNullOrWhiteSpace(model.KanbanNo))
                        sql += $@"AND TB_Import_Forecast.F_Sebango = '{model.KanbanNo}' ";

                    if (!string.IsNullOrWhiteSpace(model.StoreCode))
                        sql += $@"AND TB_Import_Forecast.F_Store_Cd = '{model.StoreCode}' ";

                    if (!string.IsNullOrWhiteSpace(model.PartNo))
                        sql += $@"AND rtrim(TB_Import_Forecast.F_part_no)+'-'+ rtrim(TB_Import_Forecast.F_ruibetsu) = '{model.PartNo}' ";
                }

                if (model.Condition == "ConditionSupplier")
                {
                    if (!string.IsNullOrWhiteSpace(model.SupplierCode))
                        sql += $@"AND substring(TB_Import_Forecast.F_Supplier_code, 2, 4)+'-'+ TB_Import_Forecast.F_supplier_plant = '{model.SupplierCode}' ";

                    if (!string.IsNullOrWhiteSpace(model.StoreCode))
                        sql += $@"AND TB_Import_Forecast.F_Store_Cd = '{model.StoreCode}' ";
                }

                sql += $@"ORDER BY TB_Import_Forecast.F_Supplier_code, TB_Import_Forecast.F_Store_cd, TB_Import_Forecast.F_part_no";

                await _kbContext.Database.ExecuteSqlRawAsync(sql);
               

            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.Message);
            }
        }

        public async Task Import(VM_KBNIM012M model)
        {
            try
            {
                if (model.File == null ) throw new Exception("Please Choose file Excel .xlxs for Import Data");
                if (System.IO.Path.GetExtension(model.File.FileName) != ".xlsx") throw new Exception("Please Choose file Excel .xlxs for Import Data");

                string fileName = model.File.FileName;
                string nameOnly = System.IO.Path.GetFileNameWithoutExtension(fileName);
                string extension = System.IO.Path.GetExtension(fileName);

                var userID = _BearerClass.UserCode;
                var sPlant = _BearerClass.Plant;
                string type_Import = "FORECAST";

                await _kbContext.Database.ExecuteSqlRawAsync($"DELETE FROM TB_Import_Data_Forecast WHERE F_Update_BY = '{userID}'");
                await _kbContext.Database.ExecuteSqlRawAsync($"DELETE FROM [HMMT-E_KANBAN].[NEW_KANBAN].[DBO].TB_Import_Error WHERE F_Update_BY = '{userID}' AND F_Type = '{type_Import}'");


                // ดึงค่า Server Name และ Database Name
                var connection = _PPM3Context.Database.GetDbConnection();
                string source = connection.DataSource;
                string database = connection.Database;

                // Change Method for Import
                string locationFileIMP = (sPlant == "1" || sPlant == "2") ? @"\NEW_KANBAN\File_Temp\TEMP_FILE_FORECAST.TXT" : @"\ImportFile\Forecast\TEMP_FILE_FORECAST.TXT";
                string fileToDelete = @"\\" + source + locationFileIMP;


                if (System.IO.File.Exists(fileToDelete))
                {
                    System.IO.File.Delete(fileToDelete);
                }

                Console.WriteLine(_kbContext.Database.GetDbConnection().ConnectionString);
                ConvertExcelToText(model.File, fileToDelete);


                var error = await RunImportForecastAsync(userID);
                if (!string.IsNullOrEmpty(error)) throw new Exception($"Can't import data: {error}");

                Console.WriteLine(_kbContext.Database.GetDbConnection().ConnectionString);

                string sql = $@"
                                INSERT INTO [HMMT-E_KANBAN].[NEW_KANBAN].[DBO].TB_IMPORT_ERROR(F_PDS_CD, F_Row, F_Field, F_Remark, F_Update_By, F_Update_Date, F_Type)
                                SELECT  T.F_Production_Date
                                       ,0                         AS F_Row
                                       ,T.F_Production_Date
                                       ,'Production Date Mistake' AS F_Remark
                                       ,'{userID}'                AS F_Update_By
                                       ,getdate()                 AS F_Update_Date
                                       ,'{type_Import}'                AS F_TYpe
                                FROM TB_Import_Data_Forecast T
                                LEFT OUTER JOIN TB_IMPORT_Forecast F
                                ON T.F_Production_Date = F.F_Production_Date
                                WHERE T.F_Update_By = '{userID}'
                                AND F.F_Production_Date is null 
                                ";
                await _kbContext.Database.ExecuteSqlRawAsync(sql);

                sql = $@"
                         SELECT  T.F_Production_Date
                               ,0                               AS F_Row
                               ,T.F_SUpplier_Code
                               ,'Not Found Part in Supplier MS' AS F_Remark
                               ,'{userID}'                      AS F_Update_By
                               ,getdate()                       AS F_Update_Date
                               ,'FORECAST'                      AS F_TYpe
                        FROM TB_IMPORT_DATA_FORECAST T
                        LEFT OUTER JOIN
                        (
	                        SELECT  *
	                        FROM [{source}].[{database}].dbo.T_Supplier_MS
	                        WHERE F_TC_Str <= convert(cHAR(8), GETDATE(), 112)
	                        AND F_TC_End >= convert(cHAR(8), GETDATE(), 112)
                        )S
                        ON substring(T.F_Supplier_Code, 1, 4) = S.F_Supplier_Cd 
                        AND substring(T.F_Supplier_Code, 6, 1) = S.F_Plant_CD 
                        AND T.F_Store_CD = S.F_Store_Cd
                        WHERE T.F_Update_By = '{userID}'
                        AND S.F_Supplier_CD is null 
                        ";
                await _kbContext.Database.ExecuteSqlRawAsync(sql);

                var NMonth = await _kbContext.TB_Import_Data_Forecast
                                              .Where(f => f.F_Update_By == userID)
                                              .MaxAsync(f => f.F_production_date);

                sql = $@"
                        INSERT INTO [HMMT-E_KANBAN].[NEW_KANBAN].[DBO].TB_IMPORT_ERROR(F_PDS_CD, F_Row, F_Field, F_Remark, F_Update_By, F_Update_Date, F_Type)
                        SELECT  T.F_Production_Date
                               ,0                                AS F_Row
                               ,rtrim(T.F_Part_No)+'-'+T.F_Store_Cd
                               ,'Not Found Part in Construction' AS F_Remark
                               ,'{userID}'                       AS F_Update_By
                               ,getdate()                        AS F_Update_Date
                               ,'{type_Import}'                  AS F_TYpe
                        FROM TB_IMPORT_DATA_FORECAST T
                        LEFT OUTER JOIN
                        (
	                        SELECT  *
	                        FROM [{source}].[{database}].dbo.T_Construction
	                        WHERE SUbstring(F_Local_Str, 1, 6) <= '{NMonth}'
	                        AND Substring(F_Local_End, 1, 6) >= '{NMonth}'
                        )S
                        ON substring(T.F_Supplier_Code, 1, 4) = S.F_Supplier_Cd 
                        AND substring(T.F_Supplier_Code, 6, 1) = S.F_Plant 
                        AND T.F_Store_CD = S.F_Store_Cd 
                        AND substring(T.F_Part_NO, 1, 10) = rtrim(S.F_Part_No) 
                        AND substring(T.F_Part_No, 11, 2) = rtrim(S.F_Ruibetsu)
                        WHERE T.F_Update_By = '{userID}'
                        AND S.F_Supplier_CD is null 
                        ";
                await _kbContext.Database.ExecuteSqlRawAsync(sql);

                sql = $@"
                        INSERT INTO [HMMT-E_KANBAN].[NEW_KANBAN].[DBO].TB_IMPORT_ERROR(F_PDS_CD, F_Row, F_Field, F_Remark, F_Update_By, F_Update_Date, F_Type)
                        SELECT  F_Production_Date
                               ,0                           AS F_Row
                               ,F_Amount_M
                               ,'Data M1 not equal summary' AS F_Remark
                               ,'{userID}'                  AS F_Update_By
                               ,getdate()                   AS F_Update_Date
                               ,'{type_Import}'             AS F_TYpe
                        FROM TB_IMPORT_DATA_FORECAST
                        WHERE F_Amount_MD1+F_Amount_MD2+F_Amount_MD3+ F_Amount_MD4+F_Amount_MD5+
	                          F_Amount_MD6+F_Amount_MD7+F_Amount_MD8+F_Amount_MD9+F_Amount_MD10+
	                          F_Amount_MD11+ F_Amount_MD12+F_Amount_MD13+F_Amount_MD14+F_Amount_MD15+
	                          F_Amount_MD16+F_Amount_MD17+F_Amount_MD18+F_Amount_MD19+ F_Amount_MD20+
	                          F_Amount_MD21+F_Amount_MD22+F_Amount_MD23+F_Amount_MD24+F_Amount_MD25+
                              F_Amount_MD26+F_Amount_MD27+ F_Amount_MD28+F_Amount_MD29+F_Amount_MD30+
	                          F_Amount_MD31 <> F_Amount_M
                         ";
                await _kbContext.Database.ExecuteSqlRawAsync(sql);

                //var err = await _kbContext.TB_Import_Error.Where(x => x.F_Update_By == userID && x.F_Type == type_Import).ToListAsync();
                var err = await _kbContext.TB_Import_Error.FromSqlRaw($@"SELECT * FROM [HMMT-E_KANBAN].[NEW_KANBAN].[DBO].TB_Import_Error WHERE F_Update_By = '{userID}' AND F_Type = '{type_Import}'").ToListAsync();
                if (err.Count > 0) throw new Exception("Please Check Data Again");

                sql = $@"
                        INSERT INTO TB_Import_Forecast
                                (F_Version, F_production_date, F_revision_no, F_Supplier_code, F_supplier_plant, 
                                 F_part_no, F_ruibetsu, F_Store_cd, F_sebango, F_Delivery_qty, F_cycle_supply, 
                                 F_Amount_M, F_Amount_M1, F_Amount_M2, F_Amount_M3, F_Amount_M4, F_Amount_MD1, 
                                 F_Amount_MD2, F_Amount_MD3, F_Amount_MD4, F_Amount_MD5, F_Amount_MD6, F_Amount_MD7, 
                                 F_Amount_MD8, F_Amount_MD9, F_Amount_MD10, F_Amount_MD11, F_Amount_MD12, F_Amount_MD13, 
                                 F_Amount_MD14, F_Amount_MD15, F_Amount_MD16, F_Amount_MD17, F_Amount_MD18, F_Amount_MD19, 
                                 F_Amount_MD20, F_Amount_MD21, F_Amount_MD22, F_Amount_MD23, F_Amount_MD24, F_Amount_MD25, 
                                 F_Amount_MD26, F_Amount_MD27, F_Amount_MD28, F_Amount_MD29, F_Amount_MD30, F_Amount_MD31, 
                                 F_unit_price, F_amount, F_Fac, F_Import_By, F_Import_Date)
                        SELECT  'C'                                   AS F_Version
                               ,D.F_production_date
                               ,D.F_revision_no
                               ,'0'+ substring(D.F_Supplier_code,1,4) AS F_Supplier_Code
                               ,substring(D.F_Supplier_Code,6,1)      AS F_Supplier_Plant
                               ,Substring(D.F_part_no,1,10)           AS F_Part_No
                               ,Substring(D.F_part_no,11,2)           AS F_ruibetsu
                               ,D.F_Store_cd
                               ,D.F_sebango
                               ,C.F_Qty_BOX                           AS F_Delivery_qty
                               ,S.F_Cycle_A+S.F_Cycle_B+S.F_CYcle_C   AS F_cycle_supply
                               ,D.F_Amount_M,D.F_Amount_M1,D.F_Amount_M2 ,D.F_Amount_M3
                               ,0                                     AS F_Amount_M4
                               ,D.F_Amount_MD1 ,D.F_Amount_MD2 ,D.F_Amount_MD3
                               ,D.F_Amount_MD4 ,D.F_Amount_MD5 ,D.F_Amount_MD6
                               ,D.F_Amount_MD7 ,D.F_Amount_MD8 ,D.F_Amount_MD9
                               ,D.F_Amount_MD10,D.F_Amount_MD11,D.F_Amount_MD12
                               ,D.F_Amount_MD13,D.F_Amount_MD14,D.F_Amount_MD15
                               ,D.F_Amount_MD16,D.F_Amount_MD17,D.F_Amount_MD18
                               ,D.F_Amount_MD19,D.F_Amount_MD20,D.F_Amount_MD21
                               ,D.F_Amount_MD22,D.F_Amount_MD23,D.F_Amount_MD24
                               ,D.F_Amount_MD25,D.F_Amount_MD26,D.F_Amount_MD27
                               ,D.F_Amount_MD28,D.F_Amount_MD29,D.F_Amount_MD30
                               ,D.F_Amount_MD31
                               ,0                                     AS F_unit_price
                               ,0                                     AS F_amount
                               ,1                                     AS F_Fac
                               ,'{userID}'                            AS F_Import_By
                               ,getdate()                             AS F_Import_Date
                        FROM TB_Import_Data_Forecast AS D
                        INNER JOIN
                        (
	                        SELECT  *
	                        FROM [{source}].[{database}].dbo.T_Construction
	                        WHERE F_Local_Str <= convert(Char(8), getdate(), 112)
	                        AND F_Local_End >= convert(Char(8), getdate(), 112)
                        )C
                        ON Substring(D.F_part_no, 1, 10) = C.F_PART_NO 
                        AND Substring(D.F_Part_No, 11, 2) = C.F_Ruibetsu 
                        AND D.F_Store_Cd = C.F_Store_Cd 
                        AND Substring(D.F_Supplier_code, 1, 4) = C.F_Supplier_Cd 
                        AND substring(D.F_Supplier_Code, 6, 1) = C.F_Plant
                        INNER JOIN
                        (
	                        SELECT  *
	                        FROM [{source}].[{database}].dbo.T_SUPPLIER_MS
	                        WHERE F_TC_Str <= convert(Char(8), getdate(), 112)
	                        AND F_TC_End >= convert(Char(8), getdate(), 112)
                        )S
                        ON D.F_Store_Cd = S.F_Store_Cd 
                        AND Substring(D.F_Supplier_code, 1, 4) = S.F_Supplier_Cd 
                        AND substring(D.F_Supplier_Code, 6, 1) = S.F_PLant_Cd
                        LEFT OUTER JOIN TB_Import_Forecast AS F
                        ON D.F_production_date = F.F_production_date 
                        AND D.F_revision_no = F.F_revision_no 
                        AND Substring(D.F_Supplier_code, 1, 4) = substring(f.F_Supplier_code, 2, 4) 
                        AND substring(D.F_Supplier_Code, 6, 1) = f.F_supplier_plant 
                        AND Substring(D.F_part_no, 1, 10) = F.F_part_no 
                        AND Substring(D.F_Part_No, 11, 2) = F.F_Ruibetsu 
                        AND D.F_Store_cd = F.F_Store_cd 
                        AND D.F_sebango = F.F_sebango
                        WHERE D.F_Update_By = '{userID}'
                        AND F.F_Part_No is null 
                        ";
                await _kbContext.Database.ExecuteSqlRawAsync(sql);

                var importData = await _kbContext.TB_Import_Data_Forecast.Where(x => x.F_Update_By == userID)
                                                                         .OrderBy(x => x.F_production_date)
                                                                         .ThenBy(x => x.F_Supplier_Code)
                                                                         .ThenBy(x => x.F_Store_cd)
                                                                         .ThenBy(x => x.F_Part_No)
                                                                         .ToListAsync();
                int i = 1;
                foreach(var imp in importData)
                {
                    
                    sql = $@"EXEC dbo.SP_UpdateForecast_FILE '{userID}','{imp.F_production_date}','{imp.F_Supplier_Code.Substring(0, 4)}','{imp.F_Supplier_Code.Substring(5, 1)}','{imp.F_Store_cd}','{imp.F_Part_No.Substring(0, 10)}','{imp.F_Part_No.Substring(10,2)}','0{imp.F_Sebango}','{model.Txt_Ver.Substring(0, 1)}'";
                    await _kbContext.Database.ExecuteSqlRawAsync(sql);
                    _log.WriteLogMsg($@"Finished Interface Forecast : {model.Txt_ProdYM} : REV : {model.Txt_Rev} : Detail : {imp.F_Supplier_Code} : {imp.F_Store_cd} : 0{imp.F_Sebango} : {imp.F_Part_No.Substring(0, 10)}-{imp.F_Part_No.Substring(10,2)} : {i++}/{importData.Count}");
                }


            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task ConvertExcelToText(IFormFile file, string sNewFile)
        {
            try
            {
                var sb = new StringBuilder();

                using (var stream = file.OpenReadStream())
                using (var workbook = new XLWorkbook(stream)) // โหลด Excel จาก Stream
                {
                    var worksheet = workbook.Worksheet(1); // อ่าน Sheet แรก
                    var range = worksheet.RangeUsed();     // หาช่วงที่มีข้อมูลจริง

                    if (range != null)
                    {
                        foreach (var row in range.Rows())
                        {
                            var rowData = row.Cells().Select(cell => cell.GetValue<string>());
                            sb.AppendLine(string.Join("\t", rowData)); // คั่นแต่ละคอลัมน์ด้วย Tab
                        }
                    }
                }

                File.WriteAllText(sNewFile, sb.ToString(), Encoding.UTF8); // เขียนเป็น .txt ด้วย UTF-8
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<string?> RunImportForecastAsync(string userID)
        {
            var connection = _kbContext.Database.GetDbConnection(); // ❌ no 'using'

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(); // เปิดเฉย ๆ ถ้ายังไม่เปิด

            using var command = connection.CreateCommand();
            command.CommandText = $"EXEC dbo.SP_IMPORT_FORECAST '{userID}'";
            command.CommandType = CommandType.Text;

            using var reader = await command.ExecuteReaderAsync();

            // ถ้ามีผลลัพธ์แสดงว่าเกิด error
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    var errorMessage = reader["ErrorMessage"]?.ToString();
                    var errorLine = reader["ErrorLine"]?.ToString();

                    return $"Error at line {errorLine}: {errorMessage}";
                }
            }

            return null; // ไม่มี error = success
        }

    }
}
