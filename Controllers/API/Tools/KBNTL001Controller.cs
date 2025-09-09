using HINOSystem.Context;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HINOSystem.Controllers.API.Master
{
    public class KBNTL001Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;
        private readonly ProcWebConnection _PCWCN;
        
        private readonly KB3Context _KB3Context;

        private readonly DBConnection _kanbanConnect;
        private readonly DBConnection _procwebConnect;
        private readonly DBConnection _ppmConnect;

        public KBNTL001Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            KanbanConnection kanbanConnection,
            ProcWebConnection procWebConnection,
            KB3Context kB3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KBCN = kanbanConnection;
            _PCWCN = procWebConnection;
            _KB3Context = kB3Context;


            _kanbanConnect = new DBConnection(_configuration, "kanban");
            _procwebConnect = new DBConnection(_configuration, "procweb");
        }



        [HttpPost]
        public IActionResult Search([FromBody] string pData = null)
        {
            string _SQL = "";

            try
            {
                
                _BearerClass.Authentication();
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                dynamic _json = JsonConvert.DeserializeObject(pData);


                _SQL = @"
                    SELECT '' AS RunningNo
                        , F_OrderNo, F_ISSUED_DATE
                        , Substring(F_Delivery_Date,1,4) + '-' + Substring(F_Delivery_Date,5,2) + '-' + Substring(F_Delivery_Date,7,2) AS F_Delivery_Date 
                        , Substring(F_Delivery_Date,1,4) + '-' + Substring(F_Delivery_Date,5,2) + '-' + Substring(F_Delivery_Date,7,2) AS NewDate 
                    FROM TB_REC_HEADER 
                    WHERE 1=1
                    AND F_OrderNo = '" + _json.pdsno + @"' 
                ";
                
                string _jsonReceiveSpecial9Y = _KBCN.ExecuteJSON(_SQL, skipLog: true);

                _KBCN.writeLog(_SQL
                    , pAction: "LOAD"
                    , pResult: "OK"
                    , pMessage: "ExecuteJSON"
                    , pUser: _BearerClass
                    , pControllerName: ControllerContext.ActionDescriptor.ControllerName
                    , pActionName: ControllerContext.ActionDescriptor.ActionName
                    , pSystem: "KB3");



                _SQL = @"
                    SELECT '' AS RunningNo
                         , d.F_Part_No, d.F_Part_Name, d.F_Box_Qty, d.F_Unit_price, d.F_Unit_Amount, d.F_Receive_amount
                         , FORMAT(F_Receive_Date,'yyyy-MM-dd') AS F_Receive_Date
                    FROM TB_REC_DETAIL d
                    WHERE 1=1
                    AND F_OrderNo = '" + _json.pdsno + @"'
                    ORDER BY d.F_No 
                ";
                
                string _jsonReceiveDate = _KBCN.ExecuteJSON(_SQL, skipLog: true);

                _SQL = @"
                    SELECT *
                    FROM [Proc_Web].[dbo].[T_Receive_Header]
                    WHERE 1=1
                    AND F_pds_cd = '" + _json.pdsno + @"';
                ";
                string _jsonReceivePCW = _PCWCN.ExecuteJSON(_SQL, skipLog: true);

                _SQL = @"
                    SELECT '' AS RunningNo
                        , a.F_rec_cd, a.F_pds_cd, a.F_prt_no 
                        , a.F_amount AS F_amount_proc, b.F_amount AS F_amount_PDS 
                        , a.F_unit_price AS F_unit_price_proc, b.F_unit_price AS F_unit_price_PDS 
                    FROM ( 
                        SELECT   RH.F_rec_cd, RH.F_pds_cd, RD.F_prt_no, RD.F_unit_price, SUM(RD.F_amount) AS F_amount 
                        FROM T_Receive_header RH 
                            INNER JOIN T_Receive_Detail RD ON RH.F_rec_cd = RD.F_rec_cd 
                        WHERE 1=1 
                        AND RH.F_pds_cd = '" + _json.pdsno + @"'
                        GROUP BY RH.F_rec_cd, RH.F_pds_cd, RD.F_prt_no, RD.F_unit_price 
                    ) AS a 
                        LEFT JOIN T_PDS692_Detail b ON b.OrderNo=a.F_pds_cd AND b.PartsNo=a.F_prt_no
                    WHERE 1=1 
                    ORDER BY b.F_No;
                ";
                string _jsonReceivePrice = _PCWCN.ExecuteJSON(_SQL, skipLog:true);


                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""ReceiveSpecial9Y"" : " + _jsonReceiveSpecial9Y + @",
                                ""ReceiveDate"" : " + _jsonReceiveDate + @",
                                ""ReceiveDatePCW"" : " + _jsonReceivePCW + @",
                                ""ReceivePrice"" : " + _jsonReceivePrice + @"
                            }
                }";


                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                string _result = @"{
                        ""status"":""200"",
                        ""response"":""ERROR"",
                        ""message"": " + e.Message.ToString() + @"
                    }";
                return Content(_result, "application/json");

            }
        }


        [HttpPost]
        public IActionResult execSpecial9Y([FromBody] string pData = null)
        {
            string _SQL = "", _result = "";

            try
            {
                
                _BearerClass.Authentication();
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                dynamic _json = JsonConvert.DeserializeObject(pData);


                _SQL = "EXEC [QQ].[Chg_IssuedDate] '" + _json.pdsno + "' ";
                
                _KBCN.Execute(_SQL, skipLog: true);

                _KBCN.writeLog(_SQL
                    , pAction: "EXECUTE"
                    , pResult:"OK"
                    , pMessage:"ExecuteNonQuery"
                    , pUser: _BearerClass
                    , pControllerName: ControllerContext.ActionDescriptor.ControllerName
                    , pActionName: ControllerContext.ActionDescriptor.ActionName
                    , pSystem: "KB3");

                _PCWCN.Execute(_SQL, skipLog:true);
                

                _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""ปลด Receive Special 9Y เรียบร้อย [Issued Date].""
                    }";
                return Content(_result, "application/json");

            }
            catch (Exception e)
            {
                _result = @"{
                        ""status"":""200"",
                        ""response"":""ERROR"",
                        ""message"": " + e.Message.ToString() + @"
                    }";
                return Content(_result, "application/json");

            }
        }


        [HttpPost]
        public IActionResult execReceiveDate([FromBody] string pData = null)
        {
            string _SQL = "", _result = "";

            try
            {
                
                _BearerClass.Authentication();
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                dynamic _json = JsonConvert.DeserializeObject(pData);

                _SQL = "EXEC [QQ].[Chg_ReceiveDate] '" + _json.pdsno + "', '" + _json.receivedate + "' ";
                
                _KBCN.Execute(_SQL, skipLog: true);

                _KBCN.writeLog(_SQL
                    , pAction: "EXECUTE"
                    , pResult: "OK"
                    , pMessage: "ExecuteNonQuery"
                    , pUser: _BearerClass
                    , pControllerName: ControllerContext.ActionDescriptor.ControllerName
                    , pActionName: ControllerContext.ActionDescriptor.ActionName
                    , pSystem: "KB3");

                _PCWCN.Execute(_SQL, skipLog: true);


                _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""เปลี่ยนวันที่ช่อง Receive เรียบร้อย [Receive Date].""
                    }";
                return Content(_result, "application/json");

            }
            catch (Exception e)
            {
                _result = @"{
                        ""status"":""200"",
                        ""response"":""ERROR"",
                        ""message"": " + e.Message.ToString() + @"
                    }";
                return Content(_result, "application/json");

            }
        }


        [HttpPost]
        public IActionResult execReceivePriceAmt([FromBody] string pData = null)
        {
            string _SQL = "", _result = "";
            try
            {
                _BearerClass.Authentication();
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                dynamic _json = JsonConvert.DeserializeObject(pData);

                _SQL = @"
                    UPDATE RD SET F_Amount = D.F_Amount 
                    FROM T_Receive_header RH 
                        INNER JOIN T_Receive_Detail RD ON RH.F_rec_cd = RD.F_rec_cd 
                        INNER JOIN T_PDS692_Detail D ON RH.F_pds_cd = D.Orderno AND RD.F_prt_no = D.PartsNo 
                    WHERE 1=1 
                    AND RD.F_amount <> D.F_amount 
                    AND RH.F_pds_cd = '" + _json.pdsno + @"';
                ";
                _procwebConnect.Execute(_SQL);

                _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""ปรับจำนวน Receive เรียบร้อย.""
                    }";
                return Content(_result, "application/json");

            }
            catch (Exception e)
            {
                _result = @"{
                        ""status"":""200"",
                        ""response"":""ERROR"",
                        ""message"": " + e.Message.ToString() + @"
                    }";
                return Content(_result, "application/json");

            }
        }


        [HttpPost]
        public IActionResult execReceivePricePrc([FromBody] string pData = null)
        {
            string _SQL = "", _result = "";
            try
            {
                _BearerClass.Authentication();
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                dynamic _json = JsonConvert.DeserializeObject(pData);

                _SQL = @"
                    UPDATE RD SET F_unit_price = D.F_unit_price 
                    FROM T_Receive_header RH 
                        INNER JOIN T_Receive_Detail RD ON RH.F_rec_cd = RD.F_rec_cd 
                        INNER JOIN T_PDS692_Detail D ON RH.F_pds_cd = D.Orderno AND RD.F_prt_no = D.PartsNo 
                    WHERE 1=1 
                    AND RD.F_amount <> D.F_amount 
                    AND RH.F_pds_cd = '" + _json.pdsno + @"';
                ";
                _procwebConnect.Execute(_SQL);

                _result = @"{
                        ""status"":""200"",
                        ""response"":""OK"",
                        ""message"": ""ปรับราคา Receive เรียบร้อย.""
                    }";
                return Content(_result, "application/json");

            }
            catch (Exception e)
            {
                _result = @"{
                        ""status"":""200"",
                        ""response"":""ERROR"",
                        ""message"": " + e.Message.ToString() + @"
                    }";
                return Content(_result, "application/json");

            }
        }
    }
}
