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
    public class KBNLC150Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly DefaultConnection _KB3Connect;

        private readonly KB3Context _KB3Context;

        public KBNLC150Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            DefaultConnection defaultConnection,
            KB3Context kB3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KB3Connect = defaultConnection;
            _KB3Context = kB3Context;

        }



        [HttpPost]
        public IActionResult initial([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                JObject _JBearer = _BearerClass.Authorization(Request.Headers.Authorization);
                if (_JBearer.GetValue("status").ToString() == "401") return Content(JsonConvert.SerializeObject(_JBearer), "application/json");


                _SQL = @" EXEC [exec].[spTB_MS_FACTORY] ";
                string _jsTB_MS_Factory = _KB3Connect.executeSQLJSON(_SQL, pUser: _JBearer, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""TB_MS_Factory"" : " + _jsTB_MS_Factory + @"
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
        public IActionResult Import([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                JObject _JBearer = _BearerClass.Authorization(Request.Headers.Authorization);
                if (_JBearer.GetValue("status").ToString() == "401") return Content(JsonConvert.SerializeObject(_JBearer), "application/json");

                _json = JsonConvert.DeserializeObject(pData);                

                _SQL = @"
                    SELECT Distinct  F_Plant, F_YM, F_Rev
                    FROM  TB_Import_Delivery 
                    WHERE  F_YM='"+ _json.Period + @"'  
                    AND  F_Plant='"+ _json.Plant + @"'  
                    Order by F_Rev desc";
                _KB3Connect.Plant = _json.F_Plant;
                DataTable dt = _KB3Connect.executeSQL(_SQL);

                int _rev = (dt.Rows.Count > 0 ? int.Parse(dt.Rows[0]["F_Rev"].ToString()) + 1 : 0);


                _SQL = @"
                    Delete From KBNLC_150 Where F_Import_By='" + _JBearer.GetValue("user")["Code"].ToString() + @"';
                    Delete From TB_Import_Error Where F_Update_BY='" + _JBearer.GetValue("user")["Code"].ToString() + @"' AND F_Type='IDT';
                    ";
                _KB3Connect.executeNonQuery(_SQL);



                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data has been save""
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
