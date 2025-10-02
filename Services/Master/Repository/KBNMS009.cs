using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Master;
using KANBAN.Models.PPM3;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Master.IRepository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Claims;

namespace KANBAN.Services.Master.Repository
{
    public class KBNMS009 : IKBNMS009
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public KBNMS009
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            IAutoMapService autoMapService
            , IHttpContextAccessor httpContextAccessor
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _automapService = autoMapService;
            _httpContextAccessor = httpContextAccessor;
        }

        //public string strDate = DateTime.Now.ToString("yyyyMMdd");
        public string strDate = "20241220";

        public async Task<List<T_Construction>> GetSupplier()
        {
            try
            {
                var data = await _PPM3Context.T_Construction
                    .Where(x => x.F_Store_cd.StartsWith(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value)
                    //.Where(x=>x.F_Store_cd.StartsWith(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value)
                    && x.F_Local_Str.CompareTo(strDate) <= 0 && x.F_Local_End.CompareTo(strDate) >= 0)
                    .OrderBy(x => x.F_supplier_cd).ThenBy(x => x.F_plant)
                    .AsNoTracking().ToListAsync();

                if (data.Count == 0)
                {
                    throw new CustomHttpException(404, "Supplier Not Found");
                }

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<TB_MS_Print_Replace_KB>> SupplierClicked(string Supplier)
        {
            try
            {
                var datenow = DateTime.Now.ToString("yyyyMMdd");
                await _kbContext.Database.ExecuteSqlRawAsync($"DELETE FROM TB_MS_Print_Replace_KB WHERE F_Update_By = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}'");

                // Step 1: Retrieve T_Construction data from _PPM3Context
                var Tcon = await _PPM3Context.T_Construction
                    .Where(x => x.F_Store_cd.StartsWith(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value) // Adjust this filter as necessary
                        && (x.F_supplier_cd.Trim() + "-" + x.F_plant.ToString()) == Supplier)
                    .AsNoTracking()
                    .ToListAsync();

                // Step 2: Retrieve all TB_MS_Kanban data from _kbContext for filtering
                var kanbanData = await _kbContext.TB_MS_Kanban.AsNoTracking().ToListAsync();

                // Step 3: Perform in-memory filtering and projection
                var data = Tcon.SelectMany(x => kanbanData
                    .Where(y => x.F_supplier_cd.Trim() == y.F_Supplier_Code.Trim()
                        && x.F_plant.ToString() == y.F_Supplier_Plant.Trim()
                        && x.F_Store_cd.Trim() == y.F_Store_Code.Trim()
                        && x.F_Sebango.Trim() == y.F_Kanban_No.Trim().Substring(1, 3)
                        && x.F_Part_no.Trim() == y.F_Part_No.Trim()
                        && x.F_Ruibetsu.Trim() == y.F_Ruibetsu.Trim()),
                    (x, y) => new TB_MS_Print_Replace_KB
                    {
                        F_Supplier_Code = x.F_supplier_cd!.Trim(),
                        F_Supplier_Plant = x.F_plant.ToString()!.Trim(),
                        F_Store_Code = x.F_Store_cd.Trim(),
                        F_Kanban_No = y.F_Kanban_No.Trim(),
                        F_Part_No = y.F_Part_No.Trim(),
                        F_Ruibetsu = y.F_Ruibetsu.Trim(),
                        F_Supply_Code = y.F_Supply_Code.Trim(),
                        F_Number = 0,
                        F_Update_Date = DateTime.Now,
                        F_Update_By = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value // Adjust as necessary
                    }).OrderBy(x => x.F_Supplier_Code).ThenBy(x => x.F_Supplier_Plant).ThenBy(x => x.F_Kanban_No).ToList();

                data = data.DistinctBy(x=> new
                {
                    x.F_Supplier_Code,
                    x.F_Supplier_Plant,
                    x.F_Store_Code,
                    x.F_Kanban_No,
                    x.F_Part_No,
                    x.F_Ruibetsu,
                    x.F_Supply_Code,
                    x.F_Update_By
                }).ToList();

                // Step 4: Add the filtered data to _kbContext and save changes
                _kbContext.TB_MS_Print_Replace_KB.AddRange(data);

                _log.WriteLogMsg($"Insert TB_MS_Print_Replace_KB {JsonConvert.SerializeObject(data)}");

                await _kbContext.SaveChangesAsync();

                return data;


            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Save(List<TB_MS_Print_Replace_KB> listObj)
        {
            try
            {
                foreach (var obj in listObj)
                {
                    var updateObj = await _kbContext.TB_MS_Print_Replace_KB
                        .Where(x => x.F_Supplier_Code == obj.F_Supplier_Code
                        && x.F_Supplier_Plant == obj.F_Supplier_Plant
                        && x.F_Store_Code == obj.F_Store_Code
                        && x.F_Kanban_No == obj.F_Kanban_No
                        && x.F_Supply_Code == obj.F_Supply_Code
                        && x.F_Update_By == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value)
                        .FirstOrDefaultAsync();

                    if (updateObj != null)
                    {
                        updateObj.F_Number = obj.F_Number;

                        _kbContext.TB_MS_Print_Replace_KB.Update(updateObj);
                        _log.WriteLogMsg($"Update TB_MS_Print_Replace_KB {JsonConvert.SerializeObject(updateObj)}");
                    }
                }

                await _kbContext.SaveChangesAsync();
                await Process_Data();
                await Process_Barcode();
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Process_Data()
        {
            try
            {
                await _kbContext.Database.ExecuteSqlRawAsync($"DELETE FROM TB_MS_Print_Replace_KB_TMP WHERE F_Update_By = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}'");
                var selectData = await _kbContext.TB_MS_Print_Replace_KB
                    .Where(x => x.F_Update_By == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value
                    && x.F_Number > 0)
                    .ToListAsync();

                string sql = "";
                string ppm = _FillDT.ppmConnect();

                if (selectData.Count == 0)
                {
                    throw new CustomHttpException(404, "Data Not Found");
                }

                foreach (var obj in selectData)
                {
                    int intNum1 = obj.F_Number!.Value;

                    var selectMax = await _kbContext.TB_MS_Print_Replace_KB_TMP
                        .Where(x => x.F_Supplier_Code == obj.F_Supplier_Code
                        && x.F_Supplier_Plant == obj.F_Supplier_Plant
                        && x.F_Store_Code == obj.F_Store_Code
                        && x.F_Kanban_No == obj.F_Kanban_No
                        && x.F_Supply_Code == obj.F_Supply_Code
                        && x.F_Part_No == obj.F_Part_No
                        && x.F_Ruibetsu == obj.F_Ruibetsu)
                        .OrderByDescending(x => x.F_Running)
                        .Select(x => x.F_Running).FirstOrDefaultAsync();

                    int intNum2 = selectMax == null ? 0 : selectMax == 0 ? 0 : selectMax;

                    if (intNum1 > intNum2)
                    {
                        sql = $@"SELECT C.F_supplier_cd, C.F_plant,S.F_name, S.F_short_name, C.F_Store_cd 
                            , RIGHT('0000'+ CONVERT(VARCHAR,RTRIM(C.F_Sebango)),4) AS F_Kanban_No 
                            , C.F_Part_no, C.F_Ruibetsu, C.F_Part_nm, C.F_qty_box 
                            , D.F_Supply_Code AS F_Dock_Cd, D.F_Address, K.F_Text 
                            FROM {ppm}.dbo.T_Construction C INNER JOIN TB_MS_Kanban D 
                            ON C.F_supplier_cd = D.F_Supplier_Code COLLATE Thai_CI_AS 
                            AND C.F_plant = D.F_Supplier_Plant COLLATE Thai_CI_AS 
                            AND C.F_Store_cd = D.F_Store_Code COLLATE Thai_CI_AS 
                            AND C.F_Sebango = RIGHT(D.F_Kanban_No,3) COLLATE Thai_CI_AS 
                            AND C.F_Part_no = D.F_Part_No COLLATE Thai_CI_AS 
                            AND C.F_Ruibetsu = D.F_Ruibetsu COLLATE Thai_CI_AS 
                            INNER JOIN {ppm}.[dbo].[T_Supplier_ms] S 
                            ON C.F_supplier_cd = S.F_supplier_cd COLLATE Thai_CI_AS 
                            AND C.F_plant = S.F_Plant_cd COLLATE Thai_CI_AS 
                            AND C.F_Store_cd = S.F_Store_cd COLLATE Thai_CI_AS 
                            LEFT JOIN  TB_MS_Label K 
                            ON C.F_supplier_cd = K.F_Supplier_Cd COLLATE Thai_CI_AS 
                            AND C.F_plant = K.F_Supplier_Plant COLLATE Thai_CI_AS 
                            AND C.F_Store_cd = K.F_Store_Code COLLATE Thai_CI_AS 
                            AND RIGHT('0000'+ CONVERT(VARCHAR,RTRIM(C.F_Sebango)),4) = K.F_Kanban_No COLLATE Thai_CI_AS 
                            AND C.F_Part_no = K.F_Part_No COLLATE Thai_CI_AS 
                            AND C.F_Ruibetsu = K.F_Ruibetsu COLLATE Thai_CI_AS 
                            WHERE C.F_Part_no = '{obj.F_Part_No}' 
                            AND C.F_Ruibetsu = '{obj.F_Ruibetsu}' 
                            AND RIGHT('0000'+ CONVERT(VARCHAR,RTRIM(C.F_Sebango)),4) = '{obj.F_Kanban_No}' 
                            AND C.F_supplier_cd = '{obj.F_Supplier_Code}' 
                            AND C.F_plant = '{obj.F_Supplier_Plant}' 
                            AND C.F_Store_cd = '{obj.F_Store_Code}' 
                            AND S.F_name = (SELECT Top 1 F_name From {ppm}.[dbo].[T_Supplier_ms] SS 
                            Where S.F_supplier_cd = SS.F_supplier_cd And S.F_Plant_cd = SS.F_Plant_cd And S.F_Store_cd = SS.F_Store_cd ) ";

                        var dt = _FillDT.ExecuteSQL(sql);

                        if (dt.Rows.Count > 0)
                        {
                            for (int i = (intNum1 - (intNum1 - intNum2)) + 1; i <= intNum1; i++)
                            {
                                var objTmp = new TB_MS_Print_Replace_KB_TMP
                                {
                                    F_Running = i,
                                    F_Plant = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value,
                                    F_Supplier_Code = dt.Rows[0]["F_supplier_cd"].ToString(),
                                    F_Supplier_Plant = dt.Rows[0]["F_plant"].ToString(),
                                    F_Supplier_Name = dt.Rows[0]["F_name"].ToString(),
                                    F_Short_Name = dt.Rows[0]["F_short_name"].ToString(),
                                    F_Store_Code = dt.Rows[0]["F_Store_cd"].ToString(),
                                    F_Kanban_No = dt.Rows[0]["F_Kanban_No"].ToString(),
                                    F_Part_No = dt.Rows[0]["F_Part_no"].ToString(),
                                    F_Ruibetsu = dt.Rows[0]["F_Ruibetsu"].ToString(),
                                    F_Part_Name = dt.Rows[0]["F_Part_nm"].ToString(),
                                    F_Box_Qty = int.Parse(dt.Rows[0]["F_qty_box"].ToString()),
                                    F_Supply_Code = dt.Rows[0]["F_Dock_Cd"].ToString(),
                                    F_Address = dt.Rows[0]["F_Address"].ToString(),
                                    F_Description = dt.Rows[0]["F_Text"].ToString(),
                                    F_Page = i,
                                    F_Page_Total = intNum1,
                                    F_Update_date = DateTime.Now,
                                    F_Update_By = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value
                                };


                                _kbContext.TB_MS_Print_Replace_KB_TMP.Add(objTmp);
                                await _kbContext.SaveChangesAsync();

                                await _kbContext.TB_MS_Print_Replace_KB_TMP
                                    .Where(x => x.F_Update_By == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value
                                    && x.F_Running <= intNum1
                                    && x.F_Supplier_Code == obj.F_Supplier_Code
                                    && x.F_Supplier_Plant == obj.F_Supplier_Plant
                                    && x.F_Store_Code == obj.F_Store_Code
                                    && x.F_Kanban_No == obj.F_Kanban_No
                                    && x.F_Part_No == obj.F_Part_No
                                    && x.F_Ruibetsu == obj.F_Ruibetsu
                                    && x.F_Supply_Code == obj.F_Supply_Code)
                                    .ExecuteUpdateAsync(x => x.SetProperty(y => y.F_Page_Total, intNum1));
                            }
                        }
                    }
                    else if (intNum1 < intNum2)
                    {
                        await _kbContext.TB_MS_Print_Replace_KB_TMP
                            .Where(x => x.F_Update_By == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value
                            && x.F_Running > intNum1
                            && x.F_Supplier_Code == obj.F_Supplier_Code
                            && x.F_Supplier_Plant == obj.F_Supplier_Plant
                            && x.F_Store_Code == obj.F_Store_Code
                            && x.F_Kanban_No == obj.F_Kanban_No
                            && x.F_Part_No == obj.F_Part_No
                            && x.F_Ruibetsu == obj.F_Ruibetsu
                            && x.F_Supply_Code == obj.F_Supply_Code)
                            .ExecuteDeleteAsync();

                        _log.WriteLogMsg($@"Delete TB_MS_Print_Replace_KB_TMP 
                            _kbContext.TB_MS_Print_Replace_KB_TMP
                            .Where(x => x.F_Update_By == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value
                            && x.F_Running > intNum1
                            && x.F_Supplier_Code == obj.F_Supplier_Code
                            && x.F_Supplier_Plant == obj.F_Supplier_Plant
                            && x.F_Store_Code == obj.F_Store_Code
                            && x.F_Kanban_No == obj.F_Kanban_No
                            && x.F_Part_No == obj.F_Part_No
                            && x.F_Ruibetsu == obj.F_Ruibetsu
                            && x.F_Supply_Code == obj.F_Supply_Code");

                        await _kbContext.TB_MS_Print_Replace_KB_TMP
                            .Where(x => x.F_Update_By == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value
                            && x.F_Running <= intNum1
                            && x.F_Supplier_Code == obj.F_Supplier_Code
                            && x.F_Supplier_Plant == obj.F_Supplier_Plant
                            && x.F_Store_Code == obj.F_Store_Code
                            && x.F_Kanban_No == obj.F_Kanban_No
                            && x.F_Part_No == obj.F_Part_No
                            && x.F_Ruibetsu == obj.F_Ruibetsu
                            && x.F_Supply_Code == obj.F_Supply_Code)
                            .ExecuteUpdateAsync(x => x.SetProperty(y => y.F_Page_Total, intNum1));

                        _log.WriteLogMsg($@"Update TB_MS_Print_Replace_KB_TMP
                            _kbContext.TB_MS_Print_Replace_KB_TMP
                            .Where(x => x.F_Update_By == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value
                            && x.F_Running <= intNum1
                            && x.F_Supplier_Code == obj.F_Supplier_Code
                            && x.F_Supplier_Plant == obj.F_Supplier_Plant
                            && x.F_Store_Code == obj.F_Store_Code
                            && x.F_Kanban_No == obj.F_Kanban_No
                            && x.F_Part_No == obj.F_Part_No
                            && x.F_Ruibetsu == obj.F_Ruibetsu
                            && x.F_Supply_Code == obj.F_Supply_Code");
                    }
                    else
                    {
                        sql = $@"SELECT C.F_supplier_cd, C.F_plant,S.F_name, S.F_short_name, C.F_Store_cd 
                            , RIGHT('0000'+ CONVERT(VARCHAR,RTRIM(C.F_Sebango)),4) AS F_Kanban_No 
                            , C.F_Part_no, C.F_Ruibetsu, C.F_Part_nm, C.F_qty_box 
                            , D.F_Supply_Code AS F_Dock_Cd, D.F_Address, K.F_Text 
                            FROM {ppm}.dbo.T_Construction C INNER JOIN TB_MS_Kanban D 
                            ON C.F_supplier_cd = D.F_Supplier_Code COLLATE Thai_CI_AS 
                            AND C.F_plant = D.F_Supplier_Plant COLLATE Thai_CI_AS 
                            AND C.F_Store_cd = D.F_Store_Code COLLATE Thai_CI_AS 
                            AND C.F_Sebango = RIGHT(D.F_Kanban_No,3) COLLATE Thai_CI_AS 
                            AND C.F_Part_no = D.F_Part_No COLLATE Thai_CI_AS 
                            AND C.F_Ruibetsu = D.F_Ruibetsu COLLATE Thai_CI_AS 
                            INNER JOIN {ppm}.[dbo].[T_Supplier_ms] S 
                            ON C.F_supplier_cd = S.F_supplier_cd COLLATE Thai_CI_AS 
                            AND C.F_plant = S.F_Plant_cd COLLATE Thai_CI_AS 
                            AND C.F_Store_cd = S.F_Store_cd COLLATE Thai_CI_AS 
                            LEFT JOIN  TB_MS_Label K 
                            ON C.F_supplier_cd = K.F_Supplier_Cd COLLATE Thai_CI_AS 
                            AND C.F_plant = K.F_Supplier_Plant COLLATE Thai_CI_AS 
                            AND C.F_Store_cd = K.F_Store_Code COLLATE Thai_CI_AS 
                            AND RIGHT('0000'+ CONVERT(VARCHAR,RTRIM(C.F_Sebango)),4) = K.F_Kanban_No COLLATE Thai_CI_AS 
                            AND C.F_Part_no = K.F_Part_No COLLATE Thai_CI_AS 
                            AND C.F_Ruibetsu = K.F_Ruibetsu COLLATE Thai_CI_AS 
                            WHERE C.F_Part_no = '{obj.F_Part_No}' 
                            AND C.F_Ruibetsu = '{obj.F_Ruibetsu}' 
                            AND RIGHT('0000'+ CONVERT(VARCHAR,RTRIM(C.F_Sebango)),4) = '{obj.F_Kanban_No} 
                            AND C.F_supplier_cd = '{obj.F_Supplier_Code}'
                            AND C.F_plant = '{obj.F_Supplier_Plant}'
                            AND C.F_Store_cd = '{obj.F_Store_Code}'
                            AND S.F_name = (SELECT Top 1 F_name From {ppm}.[dbo].[T_Supplier_ms] SS 
                            Where S.F_supplier_cd = SS.F_supplier_cd And S.F_Plant_cd = SS.F_Plant_cd And S.F_Store_cd = SS.F_Store_cd ) ";


                        var dt = _FillDT.ExecuteSQL(sql);

                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 1; i <= intNum1; i++)
                            {
                                var objTmp = new TB_MS_Print_Replace_KB_TMP
                                {
                                    F_Running = i,
                                    F_Plant = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Locality).Value,
                                    F_Supplier_Code = dt.Rows[0]["F_supplier_cd"].ToString(),
                                    F_Supplier_Plant = dt.Rows[0]["F_plant"].ToString(),
                                    F_Supplier_Name = dt.Rows[0]["F_name"].ToString(),
                                    F_Short_Name = dt.Rows[0]["F_short_name"].ToString(),
                                    F_Store_Code = dt.Rows[0]["F_Store_cd"].ToString(),
                                    F_Kanban_No = dt.Rows[0]["F_Kanban_No"].ToString(),
                                    F_Part_No = dt.Rows[0]["F_Part_no"].ToString(),
                                    F_Ruibetsu = dt.Rows[0]["F_Ruibetsu"].ToString(),
                                    F_Part_Name = dt.Rows[0]["F_Part_nm"].ToString(),
                                    F_Box_Qty = int.Parse(dt.Rows[0]["F_qty_box"].ToString()),
                                    F_Supply_Code = dt.Rows[0]["F_Dock_Cd"].ToString(),
                                    F_Address = dt.Rows[0]["F_Address"].ToString(),
                                    F_Description = dt.Rows[0]["F_Text"].ToString(),
                                    F_Page = i,
                                    F_Page_Total = intNum1,
                                    F_Update_date = DateTime.Now,
                                    F_Update_By = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value
                                };
                                _kbContext.TB_MS_Print_Replace_KB_TMP.Add(objTmp);
                                _log.WriteLogMsg($"Insert TB_MS_Print_Replace_KB_TMP {JsonConvert.SerializeObject(objTmp)}");
                            }

                            await _kbContext.SaveChangesAsync();

                        }

                    }
                }

                sql = $@"DELETE TB_MS_Print_Replace_KB_TMP WHERE F_Update_By = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}'
                    AND RTRIM(F_Supplier_Code) + RTRIM(F_Supplier_Plant) + RTRIM(F_Store_Code) COLLATE DATABASE_DEFAULT + 
                    RTRIM(F_Kanban_No) + RTRIM(F_Part_No) + RTRIM(F_Ruibetsu) + RTRIM(F_Supply_Code) 
                    IN (Select RTRIM(F_Supplier_Code) + RTRIM(F_Supplier_Plant) + RTRIM(F_Store_Code) COLLATE DATABASE_DEFAULT + 
                    RTRIM(F_Kanban_No) + RTRIM(F_Part_No) + RTRIM(F_Ruibetsu) + RTRIM(F_Supply_Code) 
                    From TB_MS_Print_Replace_KB WHERE F_Update_By = '{_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value}' AND F_Number = 0 ) ";

                await _kbContext.Database.ExecuteSqlRawAsync(sql);
                _log.WriteLogMsg($"Delete TB_MS_Print_Replace_KB_TMP {sql}");

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Process_Barcode()
        {
            try
            {
                var dataList = await _kbContext.TB_MS_Print_Replace_KB_TMP
                    .Where(x => x.F_Update_By == _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.UserData).Value)
                    .ToListAsync();

                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "QRCode");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                else
                {
                    if (Directory.GetFiles(directoryPath).Length > 0)
                    {
                        foreach (var file in Directory.GetFiles(directoryPath))
                        {
                            if (File.GetLastWriteTime(file) < DateTime.Now.AddDays(-1))
                            {
                                File.Delete(file);
                            }
                        }
                    }
                }

                if (dataList.Count == 0)
                {
                    throw new CustomHttpException(404, "Data Not Found");
                }

                foreach (var obj in dataList)
                {
                    obj.F_Order_No = "T" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    string barcodeData = "00" + obj.F_Part_No.Trim() + obj.F_Ruibetsu.Trim() + "0" + "|" +
                        obj.F_Short_Name.Trim() + "|" + obj.F_Supplier_Code.Trim() + "-" + obj.F_Supplier_Plant.Trim() +
                        "||||" + obj.F_Store_Code.Trim() + "|" + obj.F_Supply_Code.Trim() + "|" + (obj.F_Plant == "1" ? "SAMRONG PLANT" : "OTHER") +
                        "|" + obj.F_Kanban_No.Trim() + "|" + obj.F_Part_Name.Trim() + "|" + obj.F_Order_No.Trim() + "|" + "Normal" +
                        "|" + obj.F_Box_Qty.ToString() + "|" + obj.F_Page.ToString() + "/" + obj.F_Page_Total.ToString() + "|" + obj.F_Box_Qty.ToString() + "|" + obj.F_Description.Trim();

                    QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(barcodeData, QRCodeGenerator.ECCLevel.Q);
                    PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                    byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20);
                    using (var ms = new MemoryStream(qrCodeAsPngByteArr))
                    {
                        var qrCodeImage = new Bitmap(ms);
                        qrCodeImage.Save(Path.Combine(directoryPath, obj.F_Order_No + ".png"), ImageFormat.Png);
                    }


                    obj.F_Barcode = File.ReadAllBytes(Path.Combine(directoryPath, obj.F_Order_No + ".png"));

                    _kbContext.TB_MS_Print_Replace_KB_TMP.Update(obj);
                    await _kbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

    }
}
