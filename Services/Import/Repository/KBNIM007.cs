using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.SpecialData.ViewModel;
using KANBAN.Models.KB3.UrgentOrder;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Import.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using KANBAN.Models.KB3.SpecialOrdering;
using ClosedXML.Excel;

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
                string queryStoreCD = _FillDT.QueryStoreCode();

                string sql = $@"Select C.*,isnull(V.F_Date,'') as F_Date From TB_Calendar C 
                    CROSS JOIN (select F_YM + F_Day collate THAI_CI_AS as F_Date,F_YM,row_number() Over(Order by F_YM,F_Day) as F_NO from VW_Calendar 
                    Where F_WorkCd ='1' and F_YM + F_Day collate THAI_CI_AS >= convert(Char(8),getdate(),112))V  
                    Where C.F_Store_Cd='{queryStoreCD}' and C.F_YM='{YM}'";

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

        public string ListDataTable(string? PO, string? PartNo)
        {
            try
            {
                string _sql = $@"Select Distinct F_PDS_No,F_Part_order+'-'+F_Ruibetsu_Order as F_Part_No,F_Part_No+'-'+F_Ruibetsu as F_Child_Part,substring(F_Delivery_Date,7,2)+'/'+substring(F_Delivery_Date,5,2)+'/'+substring(F_Delivery_Date,1,4) as F_Delivery_Date,F_Qty as F_Qty,F_ROund,F_Remark,F_Store_Order as F_Store_CD,F_Store_CD as F_Store_Child, 
                    substring(F_PDS_Issued_Date,5,2)+'/'+substring(F_PDS_Issued_Date,1,4) as F_PDS_Issued_Date,F_Part_name as f_Child_Name, 
                    F_Supplier_CD +'-'+F_Supplier_Plant as F_SUpplier,F_Supplier_CD,F_Supplier_Plant 
                    ,case when F_OrderType='C' then 'CKD' else case when F_orderTYpe='S' then 'SPECIAL' else 'NORMAL' end end F_OrderType,F_Update_Date 
                    From TB_TRANSACTION_TMP WHERE F_Type ='Special' ";

                if (!string.IsNullOrWhiteSpace(PO))
                {
                    _sql += $@"and F_PDS_No ='{PO}' ";
                }
                if (!string.IsNullOrWhiteSpace(PartNo))
                {
                    _sql += $@"and F_Part_order+'-'+F_Ruibetsu_Order ='{PartNo}' ";
                }
                _sql += "Order by F_Update_Date,F_ORderTYPE desc,F_Delivery_Date,F_PDS_NO,F_Supplier_CD,F_Supplier_Plant,F_Part_order+'-'+F_Ruibetsu_Order,F_Part_No+'-'+F_Ruibetsu";

                DataTable dt = _FillDT.ExecuteSQL(_sql);

                return JsonConvert.SerializeObject(dt);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<TB_Transaction_TMP>> ListCalendar(string YM, string PO, string StoreCD, string PartNo)
        {
            try
            {
                var data = await _kbContext.TB_Transaction_TMP
                    .Where(x=> x.F_Type == "Special" && x.F_Type_Spc == "Special" && x.F_Delivery_Date.Substring(0, 6) == YM
                    && x.F_PDS_No == PO && x.F_Part_Order + "-" + x.F_Ruibetsu_Order == PartNo && x.F_Store_Order == StoreCD)
                    .OrderBy(x => x.F_Delivery_Date)
                    .ThenBy(x=>x.F_PDS_No)
                    .ThenBy(x=>x.F_Part_Order + "-" + x.F_Ruibetsu_Order)
                    .ToListAsync();

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Save(List<VM_Save_IM007> listObj, string action)
        {
            try
            {
                string sql = "";

                await _kbContext.Database.ExecuteSqlRawAsync("EXEC dbo.SP_IM007_FilterData {0}", _BearerClass.UserCode);

                if (action.ToUpper() == "NEW")
                {
                    if (string.IsNullOrWhiteSpace(listObj[0].PDS))
                    {
                        throw new CustomHttpException(400, "Please input Customer Order No.");
                    }
                    if (string.IsNullOrWhiteSpace(listObj[0].PartNo))
                    {
                        throw new CustomHttpException(400, "Please input Part No.");
                    }
                    if (string.IsNullOrWhiteSpace(listObj[0].StoreCD))
                    {
                        throw new CustomHttpException(400, "Please input Store Code.");
                    }


                    sql = $@"Select distinct F_PDS_NO From TB_TRANSACTION_TMP  
                        Where F_Type='Special' and F_TYpe_Spc ='Special' 
                        and F_PDS_NO ='{listObj[0].PDS}' 
                        and F_Part_Order+'-'+F_RUibetsu_Order='{listObj[0].PartNo}' 
                        and F_Store_Order='{listObj[0].StoreCD}' 
                        and Substring(F_Delivery_Date,1,6) ='{listObj[0].DeliveryDate.Substring(0,6)}' ";
                    
                    int check = await _kbContext.TB_Transaction_TMP.FromSqlRaw(sql).CountAsync();
                    if (check > 0)
                    {
                        throw new CustomHttpException(400, "Can not Insert Data because found data in database!");
                    }
                    else
                    {
                        foreach(var obj in listObj)
                        {
                            await _kbContext.Database.ExecuteSqlRawAsync($@"Exec dbo.SP_IMPORT_SPECIAL 
                                '{obj.PDS}','Special','{obj.IssuedDate}','{obj.PartNo}','{obj.StoreCD}',
                                '{obj.DeliveryDate}','{obj.Qty}','{obj.Trip}','{_BearerClass.UserCode}' ");

                            _log.WriteLogMsg("Insert to TMP : Special Order | Obj : "+ JsonConvert.SerializeObject(obj));
                        }
                    }
                }

                else if (action.ToUpper() == "DEL")
                {
                    var data = _kbContext.TB_Transaction_TMP.Where(x => x.F_Type == "Special" && x.F_Type_Spc == "Special"
                        && x.F_PDS_No == listObj[0].PDS
                        && x.F_Delivery_Date.Substring(0, 6) == listObj[0].DeliveryDate.Substring(0, 6))
                        .ToList();

                    if (!string.IsNullOrWhiteSpace(listObj[0].PartNo))
                    {
                        data = data.Where(x => x.F_Part_Order + "-" + x.F_Ruibetsu_Order == listObj[0].PartNo).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(listObj[0].StoreCD))
                    {
                        data = data.Where(x => x.F_Store_Order == listObj[0].StoreCD).ToList();
                    }

                    if(data.Count == 0)
                    {
                        throw new CustomHttpException(400, "Can not Delete Data because not found data in database!");
                    }

                    _kbContext.TB_Transaction_TMP.RemoveRange(data);
                    _log.WriteLogMsg("Delete to TMP : Special Order | Obj : " + JsonConvert.SerializeObject(listObj));

                    await _kbContext.SaveChangesAsync();

                }

                else //Update action
                {
                    if (string.IsNullOrWhiteSpace(listObj[0].PDS))
                    {
                        throw new CustomHttpException(400, "Please input Customer Order No.");
                    }
                    if (string.IsNullOrWhiteSpace(listObj[0].PartNo))
                    {
                        throw new CustomHttpException(400, "Please input Part No.");
                    }
                    if (string.IsNullOrWhiteSpace(listObj[0].StoreCD))
                    {
                        throw new CustomHttpException(400, "Please input Store Code.");
                    }

                    var data = _kbContext.TB_Transaction_TMP.Where(x => x.F_Type == "Special" && x.F_Type_Spc == "Special"
                        && x.F_PDS_No == listObj[0].PDS
                        && x.F_Delivery_Date.Substring(0, 6) == listObj[0].DeliveryDate.Substring(0, 6)
                        && x.F_Part_Order + "-" + x.F_Ruibetsu_Order == listObj[0].PartNo
                        && x.F_Store_Order == listObj[0].StoreCD)
                        .ToList();

                    if (data.Count == 0)
                    {
                        throw new CustomHttpException(400, "Can not Update Data because not found data in database!");
                    }

                    _kbContext.TB_Transaction_TMP.RemoveRange(data);
                    await _kbContext.SaveChangesAsync();

                    foreach (var obj in listObj)
                    {
                        if(obj.Qty == 0)
                        {
                            continue;
                        }
                        await _kbContext.Database.ExecuteSqlRawAsync($@"Exec dbo.SP_IMPORT_SPECIAL 
                                '{obj.PDS}','Special','{obj.IssuedDate}','{obj.PartNo}','{obj.StoreCD}',
                                '{obj.DeliveryDate}','{obj.Qty}','{obj.Trip}','{_BearerClass.UserCode}' ");

                        _log.WriteLogMsg("Update to TMP : Special Order | Obj : " + JsonConvert.SerializeObject(obj));
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Import(VM_PostFile obj)
        {
            try
            {
                string path = Directory.GetCurrentDirectory() + "\\wwwroot\\file_temp\\IMPORT_SPECIAL.xlsx";

                if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\wwwroot\\file_temp"))
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\wwwroot\\file_temp");
                }
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await obj.File.CopyToAsync(stream);
                    stream.Close();
                }

                ConvertExcelToText();

                //await _kbContext.Database.ExecuteSqlRawAsync("EXEC dbo.SP_IM007_IMPORT {0}", _BearerClass.UserCode);

                DataTable dt = _FillDT.ExecuteSQL($"EXEC dbo.SP_IM007_IMPORT '{_BearerClass.UserCode}'");

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0].ItemArray[0].ToString() == "CAL ERROR")
                    {
                        throw new CustomHttpException(400, $"การทำงานผิดพลาด กรุณานำเข้าไฟล์ใหม่อีกครั้ง. Error Because : {dt.Rows[0].ItemArray[2].ToString()}");
                    }
                    else
                    {
                        string stringError = "";
                        for(int i = 0; i < dt.Rows.Count; i++)
                        {
                            stringError += dt.Rows[i].ItemArray[0].ToString() + " : " + dt.Rows[i].ItemArray[1].ToString() + " : " + dt.Rows[i].ItemArray[2].ToString() + Environment.NewLine;
                        }

                        if(stringError != "")
                        {
                            throw new CustomHttpException(400, stringError);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task ImportSCP(VM_PostFile obj,string BackDate)
        {
            try
            {
                string path = Directory.GetCurrentDirectory() + "\\wwwroot\\file_temp\\IMPORT_SPECIAL_SPC.txt";

                if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\wwwroot\\file_temp"))
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\wwwroot\\file_temp");
                }
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await obj.File.CopyToAsync(stream);
                    stream.Close();
                }

                await _kbContext.Database.ExecuteSqlRawAsync("EXEC dbo.SP_IM007_IMPORT_SCP {0}, {1} ", _BearerClass.UserCode, int.Parse(BackDate));

                DataTable dt = _FillDT.ExecuteSQL($"EXEC dbo.SP_IM007_IMPORT_SCP '{_BearerClass.UserCode}', '{BackDate}'");

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0].ItemArray[0].ToString() == "CAL ERROR")
                    {
                        throw new CustomHttpException(400, $"การทำงานผิดพลาด กรุณานำเข้าไฟล์ใหม่อีกครั้ง. Error Because : {dt.Rows[0].ItemArray[2].ToString()}");
                    }
                    else
                    {
                        string stringError = "";
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            stringError += dt.Rows[i].ItemArray[0].ToString() + " : " + dt.Rows[i].ItemArray[1].ToString() + " : " + dt.Rows[i].ItemArray[2].ToString() + Environment.NewLine;
                        }

                        if (stringError != "")
                        {
                            throw new CustomHttpException(400, stringError);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task ImportIPMS(VM_PostFile obj, string BackDate)
        {
            try
            {
                string path = Directory.GetCurrentDirectory() + "\\wwwroot\\file_temp\\IMPORT_SPECIAL_IPMS.CSV";

                if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\wwwroot\\file_temp"))
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\wwwroot\\file_temp");
                }
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await obj.File.CopyToAsync(stream);
                    stream.Close();
                }

                await _kbContext.Database.ExecuteSqlRawAsync("EXEC dbo.SP_IM007_IMPORT_IPMS {0}, {1} ", _BearerClass.UserCode, int.Parse(BackDate));

                DataTable dt = _FillDT.ExecuteSQL($"EXEC dbo.SP_IM007_IMPORT_IPMS '{_BearerClass.UserCode}', '{BackDate}'");

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0].ItemArray[0].ToString() == "CAL ERROR")
                    {
                        throw new CustomHttpException(400, $"การทำงานผิดพลาด กรุณานำเข้าไฟล์ใหม่อีกครั้ง. Error Because : {dt.Rows[0].ItemArray[2].ToString()}");
                    }
                    else
                    {
                        string stringError = "";
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            stringError += dt.Rows[i].ItemArray[0].ToString() + " : " + dt.Rows[i].ItemArray[1].ToString() + " : " + dt.Rows[i].ItemArray[2].ToString() + Environment.NewLine;
                        }

                        if (stringError != "")
                        {
                            throw new CustomHttpException(400, stringError);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public void ConvertExcelToText()
        {
            try
            {
                // Construct file paths
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file_temp", "IMPORT_SPECIAL.xlsx");
                if (!File.Exists(path))
                {
                    throw new CustomHttpException(400, "File not found!");
                }
                string nNewFile = path.Replace(".xlsx", ".txt");
                _log.WriteLogMsg("path : " + path);
                using (var workbook = new XLWorkbook(path))
                {
                    var worksheet = workbook.Worksheets.First(); // Get the first worksheet

                    using (var writer = new StreamWriter(nNewFile))
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
            }
            catch (Exception ex)
            {
                // Re-throw custom exceptions
                if (ex is CustomHttpException)
                    throw;

                // Wrap other exceptions in a custom exception
                throw new CustomHttpException(500, ex.Message);
            }
        }
    }
}
