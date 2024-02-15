using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Math.EC.Multiplier;
using System.Globalization;

namespace KANBAN.Controllers.API.ReceiveProcess
{
    public class KBNCR220Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNCR220Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            PPMInvenContext pPMInvenContext,
            PPM3Context pPM3Context,
            KB3Context kB3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;
            _PPM3Context = pPM3Context;
            _PPMInvenContext = pPMInvenContext;
        }

        public void setConString()
        {
            try
            {
                if (_KBCN.Plant.ToString() == "3")
                {
                    var KBConnectString = _configuration.GetConnectionString("KB3Connection");
                    var PPMConnectString = _configuration.GetConnectionString("PPM3Connection");
                    _KB3Context.Database.SetConnectionString(KBConnectString);
                    _PPM3Context.Database.SetConnectionString(PPMConnectString);
                }
                else if (_KBCN.Plant.ToString() == "2")
                {
                    var KBConnectString = _configuration.GetConnectionString("KB2Connection");
                    var PPMConnectString = _configuration.GetConnectionString("PPMConnection");
                    _KB3Context.Database.SetConnectionString(KBConnectString);
                    _PPM3Context.Database.SetConnectionString(PPMConnectString);
                }
                else if (_KBCN.Plant.ToString() == "1")
                {
                    var KBConnectString = _configuration.GetConnectionString("KB1Connection");
                    var PPMConnectString = _configuration.GetConnectionString("PPMConnection");
                    _KB3Context.Database.SetConnectionString(KBConnectString);
                    _PPM3Context.Database.SetConnectionString(PPMConnectString);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task<IActionResult> Initial([FromBody] string data)
        {
            try
            {
                setConString();
                //first load page
                if (data == null)
                {
                    Int32 now = Int32.Parse(DateTime.Now.ToString("yyyyMMdd"));
                    return await Supplier(now, now, "7Y", "7Z");
                }
                else
                {
                    dynamic _json = JsonConvert.DeserializeObject(data);
                    string type = _json["type"];
                    string strDevDate = _json["devDate"];
                    string devDateRep = strDevDate.Replace("-", string.Empty);
                    string strToDate = _json["toDate"];
                    string toDateRep = strToDate.Replace("-", string.Empty);
                    int fromDate = Int32.Parse(devDateRep);
                    int toDate = Int32.Parse(toDateRep);
                    string queryOrder1 = "";
                    string queryOrder2 = "";

                    if (type == "All")
                    {
                        queryOrder1 = "7Z";
                        queryOrder2 = "7Y";
                    }
                    else if (type == "7Z")
                    {
                        queryOrder1 = "7Z";
                        queryOrder2 = "7Z";
                    }
                    else
                    {
                        queryOrder1 = "7Y";
                        queryOrder2 = "7Y";
                    }
                    return await Supplier(fromDate, toDate, queryOrder1, queryOrder2);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<IActionResult> Supplier(int fromDate, int toDate, string queryOrder1, string queryOrder2)
        {
            try
            {
                setConString();
                string _result = "";
                Int32 now = Int32.Parse(DateTime.Now.ToString("yyyyMMdd"));

                var _supplierHead = await _KB3Context.TB_REC_HEADER.Select(x => new
                {
                    x.F_OrderNo,
                    x.F_Supplier_Code,
                    x.F_Supplier_Plant,
                    x.F_Delivery_Date
                }).Where(x => x.F_OrderNo.StartsWith(queryOrder1) || x.F_OrderNo.StartsWith(queryOrder2))
                            .OrderBy(x => x.F_Supplier_Code).ToListAsync();

                var supplierHead = _supplierHead.Where(x => Int32.Parse(x.F_Delivery_Date) >= fromDate && Int32.Parse(x.F_Delivery_Date) <= toDate)
                    .DistinctBy(x => new { x.F_Supplier_Code, x.F_Supplier_Plant });

                List<string> supplierList = new();

                foreach (var sup in supplierHead)
                {
                    var T_Supplier = await _PPM3Context.T_Supplier_MS.Select(x => new
                    {
                        x.F_supplier_cd,
                        x.F_Plant_cd,
                        x.F_short_name,
                        x.F_TC_Str,
                        x.F_TC_End
                    }).Where(x => x.F_supplier_cd == sup.F_Supplier_Code && x.F_Plant_cd == sup.F_Supplier_Plant)
                    .OrderBy(x => x.F_supplier_cd).ToListAsync(); //Int32.Parse(x.F_TC_Str) <= now && now >= Int32.Parse(x.F_TC_End) &&

                    var singleSupplier = T_Supplier.OrderByDescending(x => x.F_TC_Str).DistinctBy(x => new
                    {
                        x.F_supplier_cd,
                        x.F_Plant_cd,
                        x.F_short_name
                    }).SingleOrDefault();

                    if (Int32.Parse(singleSupplier.F_TC_Str) <= now && Int32.Parse(singleSupplier.F_TC_End) >= now)
                    {
                        var _supplier = singleSupplier.F_supplier_cd + '-' + singleSupplier.F_Plant_cd + " : " + singleSupplier.F_short_name;
                        supplierList.Add(_supplier);
                    }
                }

                string _jsonData = JsonConvert.SerializeObject(supplierList);

                _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsonData + @"}";

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<IActionResult> Search([FromBody] string data)
        {
            try
            {
                setConString();
                dynamic _json = JsonConvert.DeserializeObject(data);
                string strDateFrom = _json["devDate"];
                string strDateTo = _json["toDate"];
                string strSupFrom = _json["supFrom"];
                string strSupTo = _json["supTo"];
                if (strSupFrom == "" || strSupFrom == null)
                {
                    strSupFrom = "0000";
                }
                if (strSupTo == "" || strSupTo == null)
                {
                    strSupTo = "9999";
                }
                string type = _json["type"];
                int dateFrom = int.Parse(strDateFrom);
                int dateTo = int.Parse(strDateTo);
                int supFrom = int.Parse(strSupFrom);
                int supTo = int.Parse(strSupTo);
                string queryOrder1 = "";
                string queryOrder2 = "";
                string _result = "";

                if (type == "All")
                {
                    queryOrder1 = "7Z";
                    queryOrder2 = "7Y";
                }
                else if (type == "7Z")
                {
                    queryOrder1 = "7Z";
                    queryOrder2 = "7Z";
                }
                else
                {
                    queryOrder1 = "7Y";
                    queryOrder2 = "7Y";
                }
                var datalist = await _KB3Context.VW_KBNRC_220_RPT
                        .Where(x => x.F_OrderNo.StartsWith(queryOrder1) || x.F_OrderNo.StartsWith(queryOrder2))
                        .ToListAsync();

                var dataListWhere = datalist.Where(x => int.Parse(x.F_Delivery_Date) >= dateFrom && int.Parse(x.F_Delivery_Date) <= dateTo)
                .Where(x => int.Parse(x.F_Supplier[..4]) >= supFrom && int.Parse(x.F_Supplier[..4]) <= supTo)
                .OrderBy(x => x.F_OrderNo).ThenBy(x => x.F_Delivery_Date).ThenBy(x => x.F_Supplier);

                if (!dataListWhere.Any())
                {
                    _result = @"{
                                ""status"":""400"",
                                ""response"":""OK"",
                                ""title"":""Search Special Report Error"",
                                ""message"": ""Data Not Found"" }";
                    return Ok(_result);
                }
                else
                {
                    var dataWithNo = dataListWhere.Select((x, index) => new
                    {
                        No = index + 1,
                        x.F_OrderNo,
                        x.F_Supplier,
                        x.F_Delivery_Date,
                        x.F_Receive_Date,
                        x.F_PDS_Status,
                        x.F_Receive_Status,
                        x.F_OrderType
                    }).ToList();

                    string _jsonData = JsonConvert.SerializeObject(dataWithNo);
                    _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _jsonData + @"}";
                }
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }
    }
}