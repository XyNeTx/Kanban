using Microsoft.AspNetCore.Http;
using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data;
using HINOSystem.Libs;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;

namespace HINOSystem.Controllers.API.lov
{
    public class LOVKB3Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMCN;
        private readonly PPM3Connection _PPM3CN;        
        private readonly BearerClass _BearerClass;

        private readonly DBConnection _pseConnect;
        private readonly DBConnection _issConnect;
        private readonly DBConnection _ppmConnect;

        public LOVKB3Controller(
            IConfiguration configuration,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            PPM3Connection ppm3Connect,
            BearerClass bearerClass
            )
        {
            _configuration = configuration;
            _KBCN = kanbanConnection;
            _PPMCN = ppmConnect;
            _PPM3CN = ppm3Connect;
            _BearerClass = bearerClass;

            _pseConnect = new DBConnection(_configuration, "pse");
            _issConnect = new DBConnection(_configuration, "iss");
            _ppmConnect = new DBConnection(_configuration, "ppm");
        }



        [HttpPost]
        public IActionResult SupplierList(string _id = null)
        {
            string _SQL = "";
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                _SQL = @"
                    SELECT F_Supplier_Code
						, F_Supplier_Code+'-'+F_Supplier_Plant AS Supplier_Code
						, F_Short_Name AS Supplier_Name 
                    FROM [dbo].[TB_MS_SupplierAttn] 
                    WHERE 1=1
                ";

                if (Request.Form["text"].ToString() != "") _SQL = _SQL + " AND F_Supplier_Code+'-'+F_Supplier_Plant = '" + Request.Form["text"].ToString() + "' ";

                if (_BearerClass.LOV != "") _SQL = _SQL + _BearerClass.LOV;

                string _json = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, skipLog: true);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": """ + (_json == "[]" ? "No data found in the list of values." : "Data Found") + @""",
                    ""data"":" + _json + @"
                }";

                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        [HttpPost]
        public IActionResult KanbanNoList(string _id = null)
        {
            string _SQL = "";
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                _SQL = @"
                    SELECT DISTINCT RIGHT('0000'+ CONVERT(VARCHAR,F_Sebango),4) AS Kanban_No 
                    FROM [PPMDB].[dbo].T_Construction  
                    WHERE F_Local_Str <= convert(char(8),getdate(),112) 
                    AND F_Local_End >= convert(char(8),getdate(),112) 
                ";

                if (Request.Form["text"].ToString() != "") _SQL = _SQL + " AND RIGHT('0000'+ CONVERT(VARCHAR,F_Sebango),4) = '" + Request.Form["text"].ToString() + "' ";

                if (_BearerClass.LOV != "") _SQL = _SQL + _BearerClass.LOV;

                _SQL = _SQL + "                    ORDER BY Kanban_No";

                string _json = _PPM3CN.ExecuteJSON(_SQL, pUser: _BearerClass, skipLog: true);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": """ + (_json == "[]" ? "No data found in the list of values." : "Data Found") + @""",
                    ""data"":" + _json + @"
                }";

                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        [HttpPost]
        public IActionResult StoreCodeList(string _id = null)
        {
            string _SQL = "";
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                _SQL = @"
                    SELECT DISTINCT RTRIM(F_Store_cd) As Store_Code 
                    FROM T_Construction 
                    WHERE F_Local_Str <= convert(char(8),getdate(),112) 
                    AND F_Local_End >= convert(char(8),getdate(),112)
                ";

                if (Request.Form["text"].ToString() != "") _SQL = _SQL + " AND RTRIM(F_Store_cd) = '" + Request.Form["text"].ToString() + "' ";

                if (_BearerClass.LOV != "") _SQL = _SQL + _BearerClass.LOV;

                _SQL = _SQL + "                    ORDER BY Store_Code";

                string _json = _PPM3CN.ExecuteJSON(_SQL, pUser: _BearerClass, skipLog: true);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": """ + (_json == "[]" ? "No data found in the list of values." : "Data Found") + @""",
                    ""data"":" + _json + @"
                }";

                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        [HttpPost]
        public IActionResult PartList(string _id = null)
        {
            string _SQL = "";
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                _SQL = @"
                    SELECT DISTINCT RTRIM(F_Part_no)+'-'+RTRIM(F_Ruibetsu) AS Part_No
						, F_Part_nm AS Part_Name
                    FROM T_Construction
                    WHERE F_Local_Str <= convert(char(8),getdate(),112)
                    AND F_Local_End >= convert(char(8),getdate(),112)
                ";

                if (Request.Form["text"].ToString() != "") _SQL = _SQL + " AND RTRIM(F_Part_no)+'-'+RTRIM(F_Ruibetsu) = '" + Request.Form["text"].ToString() + "' ";

                if (_BearerClass.LOV != "") _SQL = _SQL + _BearerClass.LOV;

                _SQL = _SQL + "                    ORDER BY Part_No";

                string _json = _PPM3CN.ExecuteJSON(_SQL, pUser: _BearerClass, skipLog: true);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": """ + (_json == "[]" ? "No data found in the list of values." : "Data Found") + @""",
                    ""data"":" + _json + @"
                }";

                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }




    }
}
