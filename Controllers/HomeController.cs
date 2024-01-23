using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HINOSystem.Libs;

namespace HINOSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly AuthenGuard _authenGuard;

        private readonly ILogger<HomeController> _logger;
        private readonly DbConnect _dbConnect;
        private readonly string _conn;


        private readonly WarrantyClaimConnect _wrtConnect;


        public HomeController(ILogger<HomeController> logger, DbConnect dbConnect, AuthenGuard authenGuard, WarrantyClaimConnect wrtConnect)
        {
            _authenGuard = authenGuard;

            _authenGuard.ComponentItem = "";
            _authenGuard.ComponentToolbar = false;

            _logger = logger;
            this._dbConnect = dbConnect;
            this._conn = _dbConnect.GetConncetionString();
            _wrtConnect = wrtConnect;
        }

        public IActionResult Index()
        {
            return _authenGuard.guard(ControllerContext);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Home";
            return View();
        }

        /********** Callback API **********/
        public IActionResult MyAction(string Parameter1, string Parameter2)
        {
            return Content("Test");
        }


        [HttpPost]
        public IActionResult onSelectPPM(string pYear)
        {
            string _result;

            if (pYear == "2022")
            {

                string _jsonData = @"[""Avg 2021"", ""Jan-22"", ""Feb"", ""Mar"", ""Apr"", ""May"", ""Jun"", ""Jul"", ""Aug"", ""Sep"", ""Oct"", ""Nov"", ""Dec""]";
                string _row1 = @"{ ""label"":""PPM Monthly (PPM ประจำเดือน)"", ""data"" : [5.44, 1.71, 7.31, 3.72, 3.37, 0, 0, 0, 0, 0, 0, 0, 0]}";
                string _row2 = @"{ ""label"":""PPM Target (PPM เป้าหมาย)"", ""data"" :[13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13]}";

                _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""Created"",
                ""data"": { ""label"" : " + _jsonData + @",
                           ""data"" : [" + _row1 + @", " + _row2 + @"]
                            }
                }";

            }
            else
            {


                string _jsonData = @"[""Avg 2020"", ""Jan-21"", ""Feb"", ""Mar"", ""Apr"", ""May"", ""Jun"", ""Jul"", ""Aug"", ""Sep"", ""Oct"", ""Nov"", ""Dec""]";
                string _row1 = @"{ ""label"":""PPM Monthly (PPM ประจำเดือน)"", ""data"" : [11.5, 14.2, 10.9, 13.72, 9.84, 10.26, 10.92, 11.49, 9.94, 11.87, 10.58, 12.10, 11.03]}";
                string _row2 = @"{ ""label"":""PPM Target (PPM เป้าหมาย)"", ""data"" :[12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12]}";

                _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""Created"",
                ""data"": { ""label"" : " + _jsonData + @",
                           ""data"" : [" + _row1 + @", " + _row2 + @"]
                            }
                }";

            }

            //string SQL = "SELECT 'Avg 2021', 'Jan-22', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'";
            //string _jsonData = _wrtConnect.ExecuteJSON(SQL);

            //string _result = @"{
            //    ""status"":""200"",
            //    ""response"":""OK"",
            //    ""message"": ""Created"",
            //    ""data"": " + _jsonData + @",
            //}";

            return Content(_result, "application/json");
        }



        [HttpPost]
        public IActionResult onSelectClaimCost(string pYear)
        {
            string _result;

            if (pYear == "2022")
            {

                string _jsonData = @"[""Avg 2021"", ""Jan-22"", ""Feb"", ""Mar"", ""Apr"", ""May"", ""Jun"", ""Jul"", ""Aug"", ""Sep"", ""Oct"", ""Nov"", ""Dec""]";
                string _row1 = @"{ ""label"":""Supplier"", ""data"" :[565.47, 430.55, 657.15, 496.73, 392.12, 0, 0, 0, 0, 0, 0, 0, 0 ]}";
                string _row2 = @"{ ""label"":""HMMT"", ""data"" :[641.57, 51.76, 928.13, 524.00, 451.60, 0, 0, 0, 0, 0, 0, 0, 0]}";
                string _row3 = @"{ ""label"":""CKD"", ""data"" : [99.96, 8.71, 33.72, 8.27, 0.28, 0, 0, 0, 0, 0, 0, 0, 0]}";
                string _row4 = @"{ ""label"":""Total Claim Cost"", ""data"" : [1307, 491, 1619, 1029, 844, 0, 0, 0, 0, 0, 0, 0, 0]}";

                _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""Created"",
                ""data"": { ""label"" : " + _jsonData + @",
                           ""data"" : [" + _row1 + @", " + _row2 + @", " + _row3 + @", " + _row4 + @"]
                            }
                }";

            }
            else
            {


                string _jsonData = @"[""Avg 2020"", ""Jan-21"", ""Feb"", ""Mar"", ""Apr"", ""May"", ""Jun"", ""Jul"", ""Aug"", ""Sep"", ""Oct"", ""Nov"", ""Dec""]";
                string _row1 = @"{ ""label"":""Supplier"", ""data"" :[565.47, 430.55, 657.15, 496.73, 392.12, 356.2, 488.18, 601.85, 772.74, 519.74, 466.67, 422.66, 391.18 ]}";
                string _row2 = @"{ ""label"":""HMMT"", ""data"" :[641.57, 51.76, 928.13, 524.00, 451.60, 517.34, 571.46, 892.32, 649.37, 356.2, 727.74, 597.49, 426.62]}";
                string _row3 = @"{ ""label"":""CKD"", ""data"" : [99.96, 8.71, 33.72, 8.27, 0.28, 92.02, 37.69, 16.40, 32.89, 36.52, 47.77, 54.97, 42.66]}";
                string _row4 = @"{ ""label"":""Total Claim Cost"", ""data"" : [1307, 491, 1619, 1029, 844, 965.56, 1097.33, 1510.57, 1455, 912.46, 1242.18, 1075.12, 860.46]}";

                _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""Created"",
                ""data"": { ""label"" : " + _jsonData + @",
                           ""data"" : [" + _row1 + @", " + _row2 + @", " + _row3 + @", " + _row4 + @"]
                            }
                }";

            }

            //string SQL = "SELECT 'Avg 2021', 'Jan-22', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'";
            //string _jsonData = _wrtConnect.ExecuteJSON(SQL);

            //string _result = @"{
            //    ""status"":""200"",
            //    ""response"":""OK"",
            //    ""message"": ""Created"",
            //    ""data"": " + _jsonData + @",
            //}";

            return Content(_result, "application/json");
        }

        public IActionResult Logout()
        {
            try
            {
                //this.fncActionLog("LOGOUT", "OK");

                //string _url = "~/" + this.Systems;
                //if (HttpContext.Session.GetString("HINO").ToString() == "YES") _url = "~/" + this.Systems + "/Hino";


                HttpContext.Session.Clear();
                return Redirect("~/");
            }
            catch (Exception ex)
            {
                HttpContext.Session.Clear();
                return Redirect("~/");
            }

        }
    }

}