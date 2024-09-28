using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System;
using System.Web;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

using System.Reflection.PortableExecutable;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Microsoft.Net.Http.Headers;
using System.Collections.Specialized;
using System.Net;
using System.DirectoryServices.ActiveDirectory;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;
using Org.BouncyCastle.Asn1.Ocsp;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Threading.Tasks;

using HINOSystem.Libs;
using HINOSystem.Context;
using HINOSystem.Models.KB3;

namespace HINOSystem.Controllers.API.Master
{
    public class KBNLC180Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;

        public KBNLC180Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            KanbanConnection kanbanConnection,
            KB3Context kB3Context,
            FillDataTable FillDT,
            SerilogLibs log
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KBCN = kanbanConnection;
            _KB3Context = kB3Context;
            _FillDT = FillDT;
            _log = log;
        }

        public string v_Dock_Code = "";
        public string v_ShortNM = "";

        [HttpPost]
        public IActionResult initial([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                _SQL = @" EXEC [exec].[spTB_MS_FACTORY] ";
                string _jsMS_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""TB_MS_Factory"" : " + _jsMS_Factory + @"
                            }
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        [HttpPost]
        public IActionResult Period([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                _json = JsonConvert.DeserializeObject(pData);

                _SQL = @"
                    SELECT Distinct  F_Plant, F_YM, F_Rev
                    FROM  TB_Import_Delivery 
                    WHERE  F_YM='" + _json.Period + @"'  
                    AND  F_Plant='" + _json.Plant + @"'  
                    Order by F_Rev";
                
                string _jsImport_Delivery = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""TB_Import_Delivery"" : " + _jsImport_Delivery + @"
                            }
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        [HttpPost]
        public IActionResult Rev([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                _json = JsonConvert.DeserializeObject(pData);

                _SQL = @"
                    SELECT Distinct  F_Plant, F_YM, F_Rev, F_Dock_CD
                    FROM  TB_Import_Delivery 
                    WHERE  F_YM='" + _json.Period + @"'  
                    AND  F_Plant='" + _json.Plant + @"'  
                    AND  F_Rev='" + _json.Rev + @"'  
                    Order by F_Dock_CD";
                string _jsDock = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                _SQL = @"
                    SELECT Distinct  F_Plant, F_YM, F_Rev, F_Dock_CD, F_Truck_Card As F_Truck_Card
                    FROM  TB_Import_Delivery 
                    WHERE  F_YM='" + _json.Period + @"'  
                    AND  F_Plant='" + _json.Plant + @"'  
                    AND  F_Rev='" + _json.Rev + @"'  
                    Order by F_Truck_Card";
                string _jsRoute = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""TB_Dock"" : " + _jsDock + @",
                                ""TB_Route"" : " + _jsRoute + @"
                            }
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        public async Task CalShow (string A_Route,string YM,string Rev,string incharge,string Dept)
        {
            int RowsData = 0;
            int k = 0;

            try
            {

                if(_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    throw new Exception("Unauthorized");
                }

                await _KB3Context.Database.ExecuteSqlRawAsync($"DELETE FROM dbo.KBNLC_180_Count  WHERE F_Update_By='{_BearerClass.UserCode}' ");

                string sql = $@"Insert Into KBNLC_180_Count(F_Dock_Cd, F_Truck_Card, F_Count, F_Update_By, F_Flag) 
                            Select F_Dock_Cd, F_Truck_Card,Count(F_Dock_Cd)+1 As F_Count, '{_BearerClass.UserCode}' As F_Update_By, '0' As F_Flag 
                            FROM  TB_Import_Delivery 
                            WHERE F_YM ='{YM}' AND F_Plant= '{_BearerClass.Plant}' AND F_Rev = {Rev} 
                            AND F_Truck_Card = '{A_Route}' Group by F_Truck_Card,F_Dock_Cd ";

                await _KB3Context.Database.ExecuteSqlRawAsync(sql);

                sql = $@"SELECT  F_Dock_Cd, F_Truck_Card, F_Count, F_Update_By, F_Flag 
                        FROM KBNLC_180_Count 
                        WHERE   F_Update_By =  '{_BearerClass.UserCode}' 
                        Order by F_Truck_Card,F_Dock_Cd ";

                DataTable _dtChkList = _FillDT.ExecuteSQL(sql);

                if (_dtChkList.Rows.Count > 0)
                {

                    RowsData = 6;
                    k = 1;

                    for (int i = 0; i < _dtChkList.Rows.Count; i++)
                    {

                        if (RowsData >= int.Parse(_dtChkList.Rows[i]["F_Count"].ToString()))
                        {
                            sql = $@"INSERT INTO dbo.KBNLC_180_TY(F_ID, F_Plant, F_YM, F_Rev, F_Delivery_Trip, F_Dock_Cd, F_Truck_Card, F_short_Logistic, F_Arrival_HMMT, F_Depart_HMMT, F_Delivery_Trip2, 
                                F_Arrival_HMMT2, F_Depart_HMMT2, F_Incharge, F_Team, F_Update_By) 
                                Values('{k}' ,'{_BearerClass.Plant}' , '{YM}' ,'{Rev}', 'Round',
                                '{v_Dock_Code}' ,'{A_Route}' , '{v_ShortNM}', 
                                '{_dtChkList.Rows[i]["F_Dock_Cd"].ToString().Trim()} Arrival Time', 
                                '','','','',{incharge},'{Dept}','{_BearerClass.UserCode}')";

                            await _KB3Context.Database.ExecuteSqlRawAsync(sql);

                            k = k + 1;

                            sql = $@"Select F_Plant, F_YM, F_Rev,F_Delivery_Trip, F_Dock_Cd, F_Truck_Card, F_Arrival_HMMT,F_Depart_HMMT 
                                FROM   TB_Import_Delivery 
                                WHERE F_YM ='{YM}' AND F_Plant='{_BearerClass.Plant}' AND F_Rev='{Rev}' 
                                AND F_Truck_Card='{A_Route}' AND F_Dock_Cd='{_dtChkList.Rows[i]["F_Dock_Cd"].ToString().Trim()}' 
                                Group by F_Plant, F_YM, F_Rev,F_Delivery_Trip, F_Dock_Cd, F_Truck_Card, F_Arrival_HMMT,F_Depart_HMMT 
                                Order by  F_Delivery_Trip, F_Arrival_HMMT, F_Depart_HMMT ";

                            DataTable _dtChkGet = _FillDT.ExecuteSQL(sql);

                            if (_dtChkGet.Rows.Count > 0)
                            {

                                for (int j = 0; j < _dtChkGet.Rows.Count; j++)
                                {
                                    sql = $@"INSERT INTO dbo.KBNLC_180_TY(F_ID, F_Plant, F_YM, F_Rev, F_Delivery_Trip, F_Dock_Cd, F_Truck_Card, F_short_Logistic, F_Arrival_HMMT, F_Depart_HMMT, F_Delivery_Trip2, 
                                    F_Arrival_HMMT2, F_Depart_HMMT2, F_Incharge, F_Team, F_Update_By) 
                                    Values('{k}' ,'{_BearerClass.Plant}' , '{YM}' ,'{Rev}',
                                    '{_dtChkGet.Rows[j]["F_Delivery_Trip"].ToString().Trim()}',
                                    '{v_Dock_Code}' ,
                                    '{_dtChkGet.Rows[j]["F_Truck_Card"].ToString().Trim()}' , '{v_ShortNM}', 
                                    '{_dtChkGet.Rows[j]["F_Arrival_HMMT"].ToString().Trim()}', 
                                    '{_dtChkGet.Rows[j]["F_Depart_HMMT"].ToString().Trim()}',
                                    '','','',{incharge},'{Dept}','{_BearerClass.UserCode}')";

                                    await _KB3Context.Database.ExecuteSqlRawAsync(sql);

                                    k = k + 1;
                                }

                                RowsData = RowsData - int.Parse(_dtChkList.Rows[i]["F_Count"].ToString().Trim());
                            }

                            sql = $@"UPDATE KBNLC_180_Count SET F_Flag='1' WHERE   F_Update_By =  '{_BearerClass.UserCode}' 
                            AND F_Dock_Cd='{_dtChkList.Rows[i]["F_Dock_Cd"].ToString().Trim()}'";

                            await _KB3Context.Database.ExecuteSqlRawAsync(sql);
                        }
                        //else if (_dtChkList.Rows[0]

                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
