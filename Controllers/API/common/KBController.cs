using Microsoft.AspNetCore.Mvc;
using HINOSystem.Libs;
using Newtonsoft.Json;


namespace HINOSystem.Controllers.API.common
{
    public class KBController : Controller
    {
        private readonly ILogger _logger;
        private readonly WarrantyClaimConnect _wrtConnect;
        private readonly BearerClass _BearerClass;
        
        public KBController(
            WarrantyClaimConnect wrtConnect,
            BearerClass bearerClass
            )
        {
            _wrtConnect = wrtConnect;
            _BearerClass = bearerClass;
        }


        [HttpPost]
        public IActionResult CN([FromBody] string pData = null)
        {
            string _sql = "", _result = "";
            dynamic _json = JsonConvert.DeserializeObject(pData);
            try
            {
                string _CN = _BearerClass.Decrypt(_json.CN.ToString());

                string[] _arr = _CN.Split("&");

                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"": {
                            ""UN"":""" + _arr[1] + @""",
                            ""CN"":""" + _arr[0] + @""",
                            ""IP"":""" + _arr[2] + @""",
                            ""KB"":""" + _BearerClass.Encrypt(_CN).ToString() + @"""
                            }
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



//        [HttpGet]
//        public IActionResult MENU()
//        {
//            string _sql = "", _result = "";
//            //dynamic _json = JsonConvert.DeserializeObject(pData);
//            try
//            {
//                _sql = @"

//SELECT m.* 
//	, mp._ID AS MPID
//	, mp.Parent_ID
//	, mp.Controller
//	, mp.Action
//	, mp.ViewType
//FROM erp.MenuParent mp 
//	LEFT JOIN erp.Menu m ON m._ID=Menu_ID
//WHERE mp.Parent_ID IS NULL 
//AND mp.isDelete = 0
//ORDER BY Seq


//";
//                DataTable _dtMenu = _wrtConnect.ExecuteSQL(_sql, skipLog: true);
//                string _s = @"";
//                for (int i = 0; i < _dtMenu.Rows.Count; i++)
//                {
//                    string _id = _dtMenu.Rows[i]["_id"].ToString();

//                    string _child = _getChildren(_id);
//                    _s += (_s == "" ? "" : ",") + @"
//{
//    ""id"": " + _dtMenu.Rows[i]["_ID"].ToString() + @",
//    ""text"": """ + _dtMenu.Rows[i]["Name"].ToString() + @""",
//    ""population"": null,
//    ""flagUrl"": null,
//    ""checked"": false,
//    ""hasChildren"": true,
//    ""children"": [" + _child + @"]
//}";

//                }




//                _result = @"[" + _s + @"]";
//                return Content(_result, "application/json");
//            }
//            catch (Exception e)
//            {
//                return Content(e.Message.ToString(), "application/json");
//            }
//        }




//        public string _getChildren(string _id)
//        {
//            string _s = @"";
//            string _SQL = @"
//SELECT m.* 
//	, mp._ID AS MPID
//	, mp.Parent_ID
//	, mp.Controller
//	, mp.Action
//	, mp.ViewType
//FROM erp.MenuParent mp 
//	LEFT JOIN erp.Menu m ON m._ID=Menu_ID
//WHERE mp.Parent_ID=" + _id + @"
//AND mp.isDelete = 0
//ORDER BY Seq
//";
//            DataTable _dtMenu = _wrtConnect.ExecuteSQL(_SQL, skipLog: true);
//            if (_dtMenu.Rows.Count > 0)
//            {
//                for (int i = 0; i < _dtMenu.Rows.Count; i++)
//                {
//                    string _child = _getChildren(_dtMenu.Rows[0]["_ID"].ToString());
//                    if (_dtMenu.Rows[i]["Code"].ToString() == "")
//                    {
//                        _s += (_s == "" ? "" : ",") + @"
//{
//    ""id"": " + _dtMenu.Rows[i]["_ID"].ToString() + @",
//    ""text"": """ + _dtMenu.Rows[i]["Name"].ToString() + @""",
//    ""population"": null,
//    ""flagUrl"": null,
//    ""checked"": false,
//    ""hasChildren"": false,
//    ""children"": [" + _child + @"]
//}";
//                    }
//                    else
//                    {
                        
//                    _s += (_s == "" ? "" : ",") + @"
//{
//    ""id"": " + _dtMenu.Rows[i]["_ID"].ToString() + @",
//    ""text"": """ + _dtMenu.Rows[i]["Name"].ToString() + @""",
//    ""population"": null,
//    ""flagUrl"": null,
//    ""checked"": false,
//    ""hasChildren"": false,
//    ""url"": ""~/" + _dtMenu.Rows[i]["Controller"].ToString() + "/" + _dtMenu.Rows[i]["Code"].ToString() + @""",
//    ""children"": [" + _child + @"]
//}";
//                    }
//                }

//                return _s;
//            }
//            return "";
//        }





    }
}
