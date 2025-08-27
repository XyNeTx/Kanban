using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.CKD_Ordering.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Spire.Barcode;
using System.Data;
using System.Text;

namespace KANBAN.Services.CKD_Ordering.Repository
{
    public class KBNOR370 : IKBNOR370
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;
        private readonly CKDWH_Context _CKDContext;
        private readonly CKDUSA_Context _CKDUSAContext;


        public KBNOR370
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            IAutoMapService autoMapService,
            CKDWH_Context CKDContext,
            CKDUSA_Context CKDUSAContext
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _automapService = autoMapService;
            _CKDContext = CKDContext;
            _CKDUSAContext = CKDUSAContext;
        }

        private readonly string StoragePath = @"wwwroot\Storage\Image\Barcode";

        public async Task<List<string>> Preview(List<VM_Post_KBNOR261> listobj)
        {
            try
            {
                List<string> resultList = new List<string>();
                await _kbContext.Database.ExecuteSqlRawAsync($"DELETE FROM [dbo].[KBNOR_450] WHERE F_Update_By='{_BearerClass.UserCode}'");

                DataTable _dt = new DataTable();

                foreach (var obj in listobj)
                {
                    string OrderNO = obj.F_OrderNO.Split("_")[0];
                    //string OrderNO_To = obj.F_OrderNO_To.Split("_")[0];
                    string OrderType = obj.F_OrderNO.Split("_")[1];


                    var sqlParams = new List<SqlParameter>
                    {
                        new SqlParameter("@pUserCode", _BearerClass.UserCode),
                        new SqlParameter("@pPlant", _BearerClass.Plant),
                        new SqlParameter("@pDeliveryDate", ""),
                        new SqlParameter("@F_orderType", OrderType),
                        new SqlParameter("@F_OrderNo", OrderNO),
                        new SqlParameter("@F_OrderNoTo", OrderNO),
                        new SqlParameter("@F_Supplier_Code", ""),
                        new SqlParameter("@F_Supplier_CodeTo", ""),
                        new SqlParameter("@F_Delivery_Date", ""),
                        new SqlParameter("@F_Delivery_DateTo", ""),
                        new SqlParameter("@ErrorMessage", "")
                    };

                    var _dt2 = await _FillDT.ExecuteStoreSQLAsync("[exec].[spKBNOR700_PDS]",sqlParams.ToArray());

                    if (_dt2.Rows.Count > 0)
                    {
                        if (_dt.Columns.Count == 0)
                        {
                            foreach (DataColumn dc in _dt2.Columns)
                            {
                                _dt.Columns.Add(dc.ColumnName, dc.DataType);
                            }
                        }

                        foreach (DataRow _dr in _dt2.Rows)
                        {
                            if( !_dt.AsEnumerable().Any(r => r.Field<string>("F_Barcode") == _dr["F_Barcode"].ToString()))
                            {
                                _dt.ImportRow(_dr);
                            }
                        }
                    }
                }

                foreach(DataRow dr in _dt.Rows)
                {
                    resultList.Add(dr["F_Barcode"].ToString());
                }

                return resultList;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task PreviewKB()
        {
            try
            {
                await _kbContext.Database.ExecuteSqlRawAsync($"DELETE FROM [dbo].[KBNOR_140_KB] WHERE F_Update_By='{_BearerClass.UserCode}'");
                
                await _kbContext.Database.ExecuteSqlRawAsync("EXEC [exec].[spKBNOR700_KANBAN] " +
                    "@pUserCode,@pPlant,@pDeliveryDate,@F_orderType",
                    new SqlParameter("@pUserCode", _BearerClass.UserCode),
                    new SqlParameter("@pPlant", _BearerClass.Plant),
                    new SqlParameter("@pDeliveryDate", DateTime.Now.AddMonths(-3).ToString("yyyyMMdd")),
                    new SqlParameter("@F_orderType", "U")
                    );
                await _kbContext.Database.ExecuteSqlRawAsync("EXEC [exec].[spKBNOR700_KANBAN] " +
                    "@pUserCode,@pPlant,@pDeliveryDate,@F_orderType",
                    new SqlParameter("@pUserCode", _BearerClass.UserCode),
                    new SqlParameter("@pPlant", _BearerClass.Plant),
                    new SqlParameter("@pDeliveryDate", DateTime.Now.AddMonths(-3).ToString("yyyyMMdd")),
                    new SqlParameter("@F_orderType", "N")
                    );
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<TB_REC_HEADER>> GetPDS()
        {
            try
            {
                //string FacCD = _BearerClass.Plant switch
                //{
                //    "1" => "9Z",
                //    "2" => "8Z",
                //    "3" => "7Z",
                //    _ => "9Z"
                //};

                string dateMonthN_3 = DateTime.Now.AddMonths(-3).ToString("yyyyMMdd");

                var data = await _kbContext.TB_REC_HEADER
                    .Where(x => x.F_Plant == _BearerClass.Plant[0]
                    //&& x.F_OrderNo.StartsWith(FacCD))
                    && (x.F_Supplier_Code == "9999" || x.F_Supplier_Code == "9995")
                    && (x.F_OrderType == 'N' || x.F_OrderType == 'U')
                    && x.F_Delivery_Date.CompareTo(dateMonthN_3) >= 0
                    )
                    .OrderBy(x => x.F_Delivery_Date)
                    .ToListAsync();

                if (data == null || data.Count == 0)
                {
                    throw new CustomHttpException(StatusCodes.Status404NotFound, "PDS Not found.");
                }

                return data.DistinctBy(x => new
                {
                    x.F_OrderNo
                }).ToList();

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task PDS_GENBARCODE(List<VM_Post_KBNOR261> listObj)
        {
            dynamic _data = null;
            string _SQL, _resData, fileName = "";
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""data"": null
                }";
            try
            {
                foreach(var obj in listObj)
                {
                    string _f = obj.F_OrderNO.Trim();
                    string _f2 = _f.Substring(0, _f.Length - 1);

                    string _path = Path.Combine(StoragePath, DateTime.Now.ToString("yyyyMM"));
                    if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);
                    string _saveFile = Path.Combine(StoragePath, DateTime.Now.ToString("yyyyMM"), _f2+ ".png");
                    BarcodeSettings _setting = new BarcodeSettings();
                    _setting.Type = BarCodeType.Code128;
                    _setting.Data = _f;
                    BarCodeGenerator _barcode = new BarCodeGenerator(_setting);
                    _barcode.GenerateImage().Save(_saveFile);
                }

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


    }
}
