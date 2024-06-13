using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Controllers.API.OrderReport
{
    public class KBNRT250Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _Serilog;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNRT250Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            KanbanConnection kanbanConnection,
            PPMInvenContext pPMInvenContext,
            PPM3Context pPM3Context,
            KB3Context kB3Context,
            FillDataTable fillDataTable,
            SerilogLibs serilog
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPM3Context = pPM3Context;
            _PPMInvenContext = pPMInvenContext;
            _Serilog = serilog;
            _FillDT = fillDataTable;
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

        public async Task<IActionResult> F_System_Flag()
        {
            try
            {
                setConString();
                string _result = "";
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");

                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}{Request.QueryString.Value.ToString()}");
                }
                var FlagList = await _KB3Context.TB_VLT_INTERFACE.Select(x => x.F_System_Flag).Distinct().ToListAsync();

                if (FlagList.Count == 0)
                {
                    _result = @"{
                                    ""status"":""400"",
                                    ""response"":""OK"",
                                    ""title"":""Initial Data not Found"",
                                    ""message"": ""Data Error"",
                                    }";

                    return Ok(_result);
                }
                else
                {
                    string _JsonData = JsonConvert.SerializeObject(FlagList);

                    _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _JsonData + @"
                                    }";

                    return Ok(_result);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        public async Task<IActionResult> OnClickReport([FromBody] string data)
        {
            try
            {
                setConString();
                string _result = "";
                string UserName = HttpContext.Session.GetString("USER_ID");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");
                dynamic _json = JsonConvert.DeserializeObject(data);
                string flagFrom = _json["flagFrom"];
                string flagTo = _json["flagTo"];
                string dateFrom = _json["dateFrom"];
                string dateTo = _json["dateTo"];
                string timeFrom = _json["timeFrom"];
                string timeTo = _json["timeTo"];

                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(HostName))
                {
                    return Redirect($"{Request.Path.ToString()}{Request.QueryString.Value.ToString()}");
                }
                DateTime now = DateTime.Now;
                _Serilog.WriteLog($"Start IMPORT to TB_VLT_INTERFACE_TEMP", UserName, HostName);
                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM TB_VLT_INTERFACE_TEMP WHERE F_UPDATE_BY = {0}", UserName);
                await _KB3Context.Database.ExecuteSqlRawAsync($"EXEC [dbo].[SP_KBNRT250_INSERT_TB_VLT_INTERFACE_TEMP] '{UserName}', '{flagFrom}', '{flagTo}', '{dateFrom}', '{dateTo}', '{timeFrom}', '{timeTo}' ");
                _Serilog.WriteLog("Insert TB_VLT_INTERFACE_TEMP From E-Kanban", UserName, HostName);
                DataTable dt = new();
                dt = _FillDT.ExecuteSQL($"SELECT * FROM TB_VLT_INTERFACE_TEMP WHERE F_Update_By = '{UserName}' ");
                if (dt.Rows.Count == 0)
                {
                    _result = @"{
                                    ""status"":""404"",
                                    ""response"":""OK"",
                                    ""title"":""Report Data Not Found"",
                                    ""message"": ""Please Try Other Option!""
                                    }";

                    return Ok(_result);
                }
                else
                {
                    _Serilog.WriteLog("Generate VLT DATA Report", UserName, HostName);
                    string _JsonData = JsonConvert.SerializeObject(UserName);
                    string _JsonData2 = JsonConvert.SerializeObject(_KBCN.Plant);
                    _result = @"{
                                    ""status"":""200"",
                                    ""response"":""OK"",
                                    ""message"": ""Data Found"",
                                    ""data"": " + _JsonData + @",
                                    ""data2"": " + _JsonData2 + @"
                                    }";

                    return Ok(_result);
                }
            }
            catch (Exception ex)
            {
                string UserName = HttpContext.Session.GetString("USER_NAME");
                string HostName = HttpContext.Session.GetString("USER_DEVICENAME");
                _Serilog.WriteLog($"ERROR IMPORT to TB_VLT_INTERFACE_TEMP : {ex.Message}", UserName, HostName);
                await _KB3Context.Database.ExecuteSqlRawAsync("DELETE FROM TB_VLT_INTERFACE_TEMP WHERE F_UPDATE_BY = {0}", UserName);
                return Content(ex.Message);
            }
        }
    }
}
