using HINOSystem.Context;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;

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
                _BearerClass.Authentication();
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
                _BearerClass.Authentication();
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
                _BearerClass.Authentication();
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


        public async Task<IActionResult> Truck_Card_Report (string Route1, string Route2, string YM, string Rev, string incharge, string Dept)
        {

            try
            {
                if(_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                string sql = $"DELETE FROM dbo.KBNLC_180_TY  WHERE F_Update_By='{User.FindFirst(ClaimTypes.UserData).Value}' ";
                await _KB3Context.Database.ExecuteSqlRawAsync(sql);

                v_ShortNM = "";
                v_Dock_Code = "";

                sql = "SELECT Distinct F_Plant, F_YM, F_Rev, F_Truck_Card, F_short_Logistic " +
                    " FROM   TB_Import_Delivery " +
                    $"WHERE F_YM ='{YM}' AND F_Plant='{User.FindFirst(ClaimTypes.Locality).Value}' " +
                    $"AND F_Rev = {Rev} AND F_Truck_Card='{Route1}' " +
                    $"Order by F_Truck_Card,F_short_Logistic ";

                DataTable dtChk_NM = _FillDT.ExecuteSQL(sql);

                for (int i = 0; i < dtChk_NM.Rows.Count; i++)
                {
                    v_ShortNM = v_ShortNM.Trim() + (v_ShortNM.Trim() == "" ? "" : ", ") + dtChk_NM.Rows[i]["F_short_Logistic"].ToString().Trim();
                }

                sql = "SELECT Distinct F_Plant, F_YM, F_Rev, F_Truck_Card,  F_Dock_Cd " +
                    "FROM   TB_Import_Delivery " +
                    $"WHERE F_YM ='{YM}' " +
                    $"AND F_Plant='{User.FindFirst(ClaimTypes.Locality).Value}' " +
                    $"AND F_Rev= {Rev} " +
                    $"AND F_Truck_Card='{Route1}' " +
                    $"Order by F_Truck_Card,F_Dock_Cd ";

                dtChk_NM = _FillDT.ExecuteSQL(sql);

                for (int i = 0; i < dtChk_NM.Rows.Count; i++)
                {
                    v_Dock_Code = v_Dock_Code.Trim() + (v_Dock_Code.Trim() == "" ? "" : ", ") + dtChk_NM.Rows[i]["F_Dock_Cd"].ToString().Trim();
                }

                sql = "Select Count(Distinct F_Dock_Cd) As F_Dock_Cd " +
                    "FROM   TB_Import_Delivery " +
                    $"WHERE F_YM ='{YM}' AND F_Plant='{User.FindFirst(ClaimTypes.Locality).Value}' " +
                    $"AND F_Rev= {Rev} AND F_Truck_Card='{Route1}' " +
                    $"Group by F_Dock_Cd ";

                DataTable dtChk_Get = _FillDT.ExecuteSQL(sql);

                if (dtChk_Get.Rows.Count > 0)
                {
                    await CalShow(Route1, YM, Rev, incharge, Dept);
                }

                v_ShortNM = "";
                v_Dock_Code = "";

                sql = "SELECT Distinct F_Plant, F_YM, F_Rev, F_Truck_Card, F_short_Logistic " +
                    " FROM   TB_Import_Delivery " +
                    $"WHERE F_YM ='{YM}' AND F_Plant='{User.FindFirst(ClaimTypes.Locality).Value}' " +
                    $"AND F_Rev= {Rev} AND F_Truck_Card='{Route2}' " +
                    $"Order by F_Truck_Card,F_short_Logistic ";

                dtChk_NM = _FillDT.ExecuteSQL(sql);

                for (int i = 0; i < dtChk_NM.Rows.Count; i++)
                {
                    v_ShortNM = v_ShortNM.Trim() + (v_ShortNM.Trim() == "" ? "" : ", ") + dtChk_NM.Rows[i]["F_short_Logistic"].ToString().Trim();
                }

                sql = "SELECT Distinct F_Plant, F_YM, F_Rev, F_Truck_Card,  F_Dock_Cd " +
                    "FROM   TB_Import_Delivery " +
                    $"WHERE F_YM ='{YM}' " +
                    $"AND F_Plant='{User.FindFirst(ClaimTypes.Locality).Value}' " +
                    $"AND F_Rev= {Rev} " +
                    $"AND F_Truck_Card='{Route2}' " +
                    $"Order by F_Truck_Card,F_Dock_Cd ";

                dtChk_NM = _FillDT.ExecuteSQL(sql);

                for (int i = 0; i < dtChk_NM.Rows.Count; i++)
                {
                    v_Dock_Code = v_Dock_Code.Trim() + (v_Dock_Code.Trim() == "" ? "" : ", ") + dtChk_NM.Rows[i]["F_Dock_Cd"].ToString().Trim();
                }

                sql = "Select Count(Distinct F_Dock_Cd) As F_Dock_Cd " +
                    "FROM   TB_Import_Delivery " +
                    $"WHERE F_YM ='{YM}' AND F_Plant='{User.FindFirst(ClaimTypes.Locality).Value}' " +
                    $"AND F_Rev= {Rev} AND F_Truck_Card='{Route2}' " +
                    $"Group by F_Dock_Cd ";

                dtChk_Get = _FillDT.ExecuteSQL(sql);

                if (dtChk_Get.Rows.Count > 0)
                {
                    await CalShow(Route2, YM, Rev, incharge, Dept);
                }


                return StatusCode(200, new
                {
                    status = 200,
                    response = "OK",
                    message = "Open Report",
                });

            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    status = 500,
                    response = "Internal Server Error",
                    message = ex.Message
                });

            }

        }


        public async Task CalShow (string A_Route,string YM,string Rev,string incharge,string Dept)
        {
            int RowsData = 0;
            int k = 0;

            //using var _kbTrans = _KB3Context.Database.BeginTransaction();
            try
            {
                //_kbTrans.CreateSavepoint("Begin CalShow");

                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    throw new Exception("Unauthorized");
                }

                await _KB3Context.Database.ExecuteSqlRawAsync($"DELETE FROM dbo.KBNLC_180_Count  WHERE F_Update_By='{User.FindFirst(ClaimTypes.UserData).Value}' ");

                string sql = $@"Insert Into KBNLC_180_Count(F_Dock_Cd, F_Truck_Card, F_Count, F_Update_By, F_Flag) 
                            Select F_Dock_Cd, F_Truck_Card,Count(F_Dock_Cd)+1 As F_Count, '{User.FindFirst(ClaimTypes.UserData).Value}' As F_Update_By, '0' As F_Flag 
                            FROM  TB_Import_Delivery 
                            WHERE F_YM ='{YM}' AND F_Plant= '{User.FindFirst(ClaimTypes.Locality).Value}' AND F_Rev = {Rev} 
                            AND F_Truck_Card = '{A_Route}' Group by F_Truck_Card,F_Dock_Cd ";

                await _KB3Context.Database.ExecuteSqlRawAsync(sql);

                sql = $@"SELECT  F_Dock_Cd, F_Truck_Card, F_Count, F_Update_By, F_Flag 
                        FROM KBNLC_180_Count 
                        WHERE   F_Update_By =  '{User.FindFirst(ClaimTypes.UserData).Value}' 
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
                                Values('{k}' ,'{User.FindFirst(ClaimTypes.Locality).Value}' , '{YM}' ,'{Rev}', 'Round',
                                '{v_Dock_Code}' ,'{A_Route}' , '{v_ShortNM}', 
                                '{_dtChkList.Rows[i]["F_Dock_Cd"].ToString().Trim()} Arrival Time', 
                                '','','','','{incharge}','{Dept}','{User.FindFirst(ClaimTypes.UserData).Value}')";

                            await _KB3Context.Database.ExecuteSqlRawAsync(sql);

                            k = k + 1;

                            sql = $@"Select F_Plant, F_YM, F_Rev,F_Delivery_Trip, F_Dock_Cd, F_Truck_Card, F_Arrival_HMMT,F_Depart_HMMT 
                                FROM   TB_Import_Delivery 
                                WHERE F_YM ='{YM}' AND F_Plant='{User.FindFirst(ClaimTypes.Locality).Value}' AND F_Rev='{Rev}' 
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
                                    Values('{k}' ,'{User.FindFirst(ClaimTypes.Locality).Value}' , '{YM}' ,'{Rev}',
                                    '{_dtChkGet.Rows[j]["F_Delivery_Trip"].ToString().Trim()}',
                                    '{v_Dock_Code}' ,
                                    '{_dtChkGet.Rows[j]["F_Truck_Card"].ToString().Trim()}' , '{v_ShortNM}', 
                                    '{_dtChkGet.Rows[j]["F_Arrival_HMMT"].ToString().Trim()}', 
                                    '{_dtChkGet.Rows[j]["F_Depart_HMMT"].ToString().Trim()}',
                                    '','','','{incharge}','{Dept}','{User.FindFirst(ClaimTypes.UserData).Value}')";

                                    await _KB3Context.Database.ExecuteSqlRawAsync(sql);

                                    k = k + 1;
                                }

                                RowsData = RowsData - int.Parse(_dtChkList.Rows[i]["F_Count"].ToString().Trim());
                            }

                            sql = $@"UPDATE KBNLC_180_Count SET F_Flag='1' WHERE   F_Update_By =  '{User.FindFirst(ClaimTypes.UserData).Value}' 
                            AND F_Dock_Cd='{_dtChkList.Rows[i]["F_Dock_Cd"].ToString().Trim()}'";

                            await _KB3Context.Database.ExecuteSqlRawAsync(sql);
                        }


                        else if (_dtChkList.Rows[0]["F_Count"].ToString().CompareTo("6") >= 0 
                            && RowsData == 6 && i == 0)
                        {

                            sql = $@"INSERT INTO dbo.KBNLC_180_TY(F_ID, F_Plant, F_YM, F_Rev, F_Delivery_Trip, F_Dock_Cd, F_Truck_Card, F_short_Logistic, F_Arrival_HMMT, F_Depart_HMMT, F_Delivery_Trip2, 
                                F_Arrival_HMMT2, F_Depart_HMMT2, F_Incharge, F_Team, F_Update_By) 
                                Values('{k}' ,'{User.FindFirst(ClaimTypes.Locality).Value}' , '{YM}' ,'{Rev}', 'Round',
                                '{v_Dock_Code}' ,'{A_Route}' , '{v_ShortNM}', 
                                '{_dtChkList.Rows[i]["F_Dock_Cd"].ToString().Trim()} Arrival Time', 
                                '','','','',{incharge},'{Dept}','{User.FindFirst(ClaimTypes.UserData).Value}')  ";

                            await _KB3Context.Database.ExecuteSqlRawAsync(sql);

                            k = k + 1;

                            sql = "Select F_Plant, F_YM, F_Rev,F_Delivery_Trip, F_Dock_Cd, F_Truck_Card, F_Arrival_HMMT,F_Depart_HMMT" +
                                "FROM   TB_Import_Delivery " +
                                $"WHERE F_YM = '{YM}' " +
                                $"AND F_Plant='{User.FindFirst(ClaimTypes.Locality).Value}' " +
                                $"AND F_Rev= {Rev} " +
                                $"AND F_Truck_Card='{A_Route}' " +
                                $"AND F_Dock_Cd='{_dtChkList.Rows[i]["F_Dock_Cd"].ToString().Trim()}' " +
                                $"Group by F_Plant, F_YM, F_Rev,F_Delivery_Trip, F_Dock_Cd, F_Truck_Card, F_Arrival_HMMT,F_Depart_HMMT " +
                                $"Order by  F_Delivery_Trip, F_Arrival_HMMT, F_Depart_HMMT";

                            DataTable _dtChkGet = _FillDT.ExecuteSQL(sql);
                            
                            if(_dtChkGet.Rows.Count > 0)
                            {
                                for (int j = 0; j < _dtChkGet.Rows.Count; j++)
                                {
                                    if (j <= 4)
                                    {
                                        sql = $@"INSERT INTO dbo.KBNLC_180_TY(F_ID, F_Plant, F_YM, F_Rev, F_Delivery_Trip, F_Dock_Cd, F_Truck_Card,
                                            F_short_Logistic, F_Arrival_HMMT, F_Depart_HMMT, F_Delivery_Trip2, 
                                            F_Arrival_HMMT2, F_Depart_HMMT2, F_Incharge, F_Team, F_Update_By) 
                                            Values('{k}' ,'{User.FindFirst(ClaimTypes.Locality).Value}' , '{YM}' ,'{Rev}',
                                            '{_dtChkGet.Rows[j]["F_Delivery_Trip"].ToString().Trim()}',
                                            '{v_Dock_Code}' ,
                                            '{_dtChkGet.Rows[j]["F_Truck_Card"].ToString().Trim()}' , '{v_ShortNM}', 
                                            '{_dtChkGet.Rows[j]["F_Arrival_HMMT"].ToString().Trim()}', 
                                            '{_dtChkGet.Rows[j]["F_Depart_HMMT"].ToString().Trim()}',
                                            '','','',{incharge},'{Dept}','{User.FindFirst(ClaimTypes.UserData).Value}')";

                                        await _KB3Context.Database.ExecuteSqlRawAsync(sql);

                                        k = k + 1;
                                    }
                                    else
                                    {
                                        if (j == 5)
                                        {
                                            k = 1;

                                            sql = "UPDATE dbo.KBNLC_180_TY SET F_Delivery_Trip2='Round', " +
                                                $"F_Arrival_HMMT2='{_dtChkList.Rows[i]["F_Dock_Cd"]} Arrival Time', " +
                                                $"F_Depart_HMMT2='', " +
                                                $"WHERE F_Update_By='{User.FindFirst(ClaimTypes.UserData).Value}' " +
                                                $"AND F_Truck_Card='{A_Route}' " +
                                                $"AND F_ID={k}";

                                            await _KB3Context.Database.ExecuteSqlRawAsync(sql);

                                            k = k + 1;
                                        }

                                        sql = $"UPDATE dbo.KBNLC_180_TY SET F_Delivery_Trip2='{_dtChkGet.Rows[j]["F_Delivery_Trip"].ToString().Trim()}', " +
                                            $"F_Arrival_HMMT2='{_dtChkGet.Rows[j]["F_Arrival_HMMT"].ToString().Trim()}', " +
                                            $"F_Depart_HMMT2='{_dtChkGet.Rows[j]["F_Depart_HMMT"].ToString().Trim()}', " +
                                            $"WHERE F_Update_By='{User.FindFirst(ClaimTypes.UserData).Value}' " +
                                            $"AND F_Truck_Card='{A_Route}' " +
                                            $"AND F_ID={k}";

                                        await _KB3Context.Database.ExecuteSqlRawAsync(sql);

                                        k = k + 1;
                                    }
                                }
                            }

                            RowsData = RowsData - int.Parse(_dtChkList.Rows[i]["F_Count"].ToString().Trim());

                            sql = $"UPDATE KBNLC_180_Count SET F_Flag='1' " +
                                $"WHERE F_Update_By='{User.FindFirst(ClaimTypes.UserData).Value}' " +
                                $"AND F_Dock_Cd='{_dtChkList.Rows[i]["F_Dock_Cd"].ToString().Trim()}'";

                            await _KB3Context.Database.ExecuteSqlRawAsync(sql);

                        }

                    }

                    if (RowsData > 0)
                    {
                        for (int j = 1; j <= RowsData; j++)
                        {

                            sql = "INSERT INTO dbo.KBNLC_180_TY(F_ID, F_Plant, F_YM, F_Rev, F_Delivery_Trip, F_Dock_Cd, F_Truck_Card, F_short_Logistic, F_Arrival_HMMT, F_Depart_HMMT, F_Delivery_Trip2," +
                                $" F_Arrival_HMMT2, F_Depart_HMMT2, F_Incharge, F_Team, F_Update_By) " +
                                $"Values( '{k}' ,'{User.FindFirst(ClaimTypes.Locality).Value}' , '{YM}' ,'{Rev}', 'Round', '{v_Dock_Code}' ,'{A_Route}' , '{v_ShortNM}', " +
                                $"'','','','','','{incharge}','{Dept}','{User.FindFirst(ClaimTypes.UserData).Value}')";

                            await _KB3Context.Database.ExecuteSqlRawAsync(sql);

                            k = k + 1;
                        }
                    }

                }

                sql = $"SELECT F_Dock_Cd, F_Truck_Card, F_Count, F_Update_By, F_Flag " +
                    $" FROM KBNLC_180_Count " +
                    $" WHERE   F_Update_By = '{User.FindFirst(ClaimTypes.UserData).Value}' AND F_Flag='0' " +
                    $" Order by F_Truck_Card,F_Dock_Cd";

                DataTable _dtChkList2 = _FillDT.ExecuteSQL(sql);

                if (_dtChkList2.Rows.Count > 0)
                {
                    k = 1;
                    for (int i = 0; i < _dtChkList2.Rows.Count; i++)
                    {

                        sql = $"UPDATE dbo.KBNLC_180_TY SET F_Delivery_Trip2='Round', " +
                            $"F_Arrival_HMMT2='{_dtChkList2.Rows[i]["F_Dock_Cd"].ToString()}' Arrival Time, " +
                            $"F_Depart_HMMT2='' " +
                            $"WHERE F_Update_By='{User.FindFirst(ClaimTypes.UserData).Value}' " +
                            $"AND F_Truck_Card='{A_Route}' " +
                            $"AND F_ID= {k} ";

                        await _KB3Context.Database.ExecuteSqlRawAsync(sql);
                        k = k + 1;

                        sql = $"Select F_Plant, F_YM, F_Rev,F_Delivery_Trip, F_Dock_Cd, F_Truck_Card, F_Arrival_HMMT,F_Depart_HMMT " +
                            $"FROM   TB_Import_Delivery " +
                            $"WHERE F_YM ='{YM}' AND F_Plant='{User.FindFirst(ClaimTypes.Locality).Value}' " +
                            $"AND F_Rev={Rev} AND F_Truck_Card='{A_Route}' " +
                            $"AND F_Dock_Cd='{_dtChkList2.Rows[i]["F_Dock_Cd"].ToString()}' " +
                            $"Group by F_Plant, F_YM, F_Rev,F_Delivery_Trip, F_Dock_Cd, F_Truck_Card, F_Arrival_HMMT,F_Depart_HMMT " +
                            $"Order by  F_Delivery_Trip, F_Arrival_HMMT, F_Depart_HMMT";

                        DataTable _dtChkGet = _FillDT.ExecuteSQL(sql);

                        if (_dtChkGet.Rows.Count > 0)
                        {
                            for (int j = 0; j < _dtChkGet.Rows.Count; j++)
                            {
                                sql = $"UPDATE dbo.KBNLC_180_TY SET F_Delivery_Trip2='{_dtChkGet.Rows[j]["F_Delivery_Trip"].ToString().Trim()}', " +
                                    $"F_Arrival_HMMT2='{_dtChkGet.Rows[j]["F_Arrival_HMMT"].ToString().Trim()}', " +
                                    $"F_Depart_HMMT2='{_dtChkGet.Rows[j]["F_Depart_HMMT"].ToString().Trim()}' " +
                                    $"WHERE F_Update_By='{User.FindFirst(ClaimTypes.UserData).Value}' " +
                                    $"AND F_Truck_Card='{A_Route}' " +
                                    $"AND F_ID={k}";

                                await _KB3Context.Database.ExecuteSqlRawAsync(sql);

                                k = k + 1;
                            }
                        }

                        sql = $"UPDATE KBNLC_180_Count SET F_Flag='2' WHERE   F_Update_By =  '{User.FindFirst(ClaimTypes.UserData).Value}' " +
                            $"AND F_Dock_Cd='{_dtChkList2.Rows[i]["F_Dock_Cd"].ToString().Trim()}'";

                        await _KB3Context.Database.ExecuteSqlRawAsync(sql);
                    }
                }
                //_kbTrans.Commit();
            }
            catch (Exception ex)
            {
                //_kbTrans.RollbackToSavepoint("Begin CalShow");
                throw new Exception(ex.Message);
            }
        }

    }
}
