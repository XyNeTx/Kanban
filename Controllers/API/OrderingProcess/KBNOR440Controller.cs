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
using HINOSystem.Models.KB3.Master;
using NPOI.SS.Formula.Functions;
using System.Dynamic;
using Microsoft.SqlServer.Server;
using Microsoft.VisualBasic;
using System.Globalization;

namespace HINOSystem.Controllers.API.Master
{
    public class KBNOR440Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly KB3Context _KB3Context;

        private Dictionary<string, string[]> SumDigit = new Dictionary<string, string[]>();


        private int[] _value = new int[43];
        private string[] _data = new string[43];

        public KBNOR440Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            KanbanConnection kanbanConnection,
            KB3Context kB3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _KBCN = kanbanConnection;
            _KB3Context = kB3Context;

            // Initialize the value
            this.SumDigit["value"] = new string[43];
            this.SumDigit["data"] = new string[43];
        }



        [HttpPost]
        public IActionResult initial([FromBody] string pData = null)
        {
            dynamic _data = null;
            string _SQL, _resData;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""data"": null
                }";

            _BearerClass.Authentication(Request);
            if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

            try
            {
                

                //_SQL = @" ";
                //string _jsTB_MS_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"": null
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }



        [HttpPost]
        public IActionResult KBNOR440_01([FromBody] string pPostData = null)
        {
            dynamic _data = null;
            string _SQL, _resData;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""data"": null
                }";

            _BearerClass.Authentication(Request);
            if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

            try
            {
                

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                //Clear  TB_PDS_DETAIL
                _SQL = @" EXEC [exec].[spKBNOR440_01] 
                    '" + _data.OrderType.ToString() + @"',
                    '" + _BearerClass.Plant + @"',
                    '" + _BearerClass.UserCode + @"',
                    '' ";
                DataTable _dtChk = _KBCN.ExecuteSQL(_SQL, pUser: _BearerClass,
                    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                    pActionName: ControllerContext.ActionDescriptor.ActionName
                    );

                if (_dtChk.Rows.Count > 0)
                {
                    for (int i = 0; i < _dtChk.Rows.Count; i++)
                    {
                        String _Remark = "";

                        _SQL = @" EXEC [exec].[spKBNOR440_01_D] 
                            '" + _data.OrderType.ToString() + @"',
                            '" + _BearerClass.Plant + @"',
                            '" + _BearerClass.UserCode + @"',
                            '" + _dtChk.Rows[i]["F_OrderNo"].ToString() + @"',
                            '' ";
                        _KBCN.Execute(_SQL, pUser: _BearerClass,
                                    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                                    pActionName: ControllerContext.ActionDescriptor.ActionName
                                    );
                    }
                }


                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Process Complete""
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                _SQL = @" EXEC [exec].[spKBNOR440_EXCEPTION]
                    '" + _data.OrderType.ToString() + @"',
                    '" + _BearerClass.Plant + @"',
                    '" + _BearerClass.UserCode + @"' ";
                _KBCN.Execute(_SQL, pUser: _BearerClass,
                            pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                            pActionName: ControllerContext.ActionDescriptor.ActionName
                            );


                _result = @"{
                    ""status"":""200"",
                    ""response"":""NO"",
                    ""message"": ""Process Not Complete!!!""
                }";
                return Content(_result, "application/json");
            }
        }



        [HttpPost]
        public IActionResult KBNOR440_03([FromBody] string pPostData = null)
        {
            dynamic _data = null;
            string _SQL, _resData;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""data"": null
                }";

            _BearerClass.Authentication(Request);
            if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

            try
            {
                

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                this.GetCheckSum();
                string _IssuedDate = "";

                _SQL = @" EXEC [exec].[spKBNOR440_03] 
                    '" + _data.OrderType.ToString() + @"',
                    '" + _BearerClass.Plant + @"',
                    '" + _BearerClass.UserCode + @"',
                    '' ";
                DataTable _dtChk = _KBCN.ExecuteSQL(_SQL, pUser: _BearerClass,
                    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                    pActionName: ControllerContext.ActionDescriptor.ActionName
                    );

                String _PDS_No = "";
                if (_dtChk.Rows.Count > 0)
                {
                    for (int i = 0; i < _dtChk.Rows.Count; i++)
                    {
                        if (i == 0 || _dtChk.Rows[i]["F_Delivery_Date"].ToString().Substring(3, 6) + _dtChk.Rows[i]["F_Delivery_Date"].ToString().PadLeft(2, '0') != _PDS_No)
                        {

                            _SQL = @" EXEC [exec].[spKBNOR440_03_S] 
                            '" + _data.OrderType.ToString() + @"',
                            '" + _BearerClass.Plant + @"',
                            '" + _BearerClass.UserCode + @"',
                            '" + _dtChk.Rows[i]["F_Delivery_Date"].ToString().Trim() + @"',
                            '" + _dtChk.Rows[i]["F_Delivery_Trip"].ToString().Trim() + @"',
                            '' ";
                            DataTable _dtChkPOM = _KBCN.ExecuteSQL(_SQL, pUser: _BearerClass,
                                        pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                                        pActionName: ControllerContext.ActionDescriptor.ActionName
                                        );

                            if (_dtChkPOM.Rows.Count == 0) _PDS_No = "9Y"
                                    + _dtChk.Rows[i]["F_Delivery_Date"].ToString().Substring(3, 6)
                                    + _dtChk.Rows[i]["F_Delivery_Trip"].ToString().PadLeft(2, '0')
                                    + "001";
                            int _F_OrderNo;
                            if (_dtChkPOM.Rows.Count != 0) _PDS_No = "9Y"
                                    + _dtChk.Rows[i]["F_Delivery_Date"].ToString().Substring(3, 6)
                                    + _dtChk.Rows[i]["F_Delivery_Trip"].ToString().PadLeft(2, '0')
                                    + (Int32.Parse(_dtChk.Rows[i]["F_OrderNo"].ToString()) + 1).ToString().PadLeft(3, '0');

                        }
                        else
                        {
                            _PDS_No = "9Y"
                                    + _dtChk.Rows[i]["F_Delivery_Date"].ToString().Substring(3, 6)
                                    + _dtChk.Rows[i]["F_Delivery_Trip"].ToString().PadLeft(2, '0')
                                    + (Int32.Parse(_PDS_No) + 1).ToString().PadLeft(3, '0');
                        }

                        string _Barcode = CheckSum(_PDS_No);
                        _SQL = @" EXEC [exec].[spKBNOR440_03_U]
                            '" + _data.OrderType.ToString() + @"',
                            '" + _BearerClass.Plant + @"',
                            '" + _BearerClass.UserCode + @"',
                            '" + _PDS_No + @"',
                            '" + _dtChk.Rows[i]["F_Delivery_Date"].ToString().Trim() + @"',
                            '" + _dtChk.Rows[i]["F_Delivery_Time"].ToString().Trim() + @"',
                            '" + _dtChk.Rows[i]["F_Supplier_Cd"].ToString().Trim() + @"',
                            '" + _dtChk.Rows[i]["F_Supplier_Plant"].ToString().Trim() + @"',
                            '" + _dtChk.Rows[i]["F_Store_CD"].ToString().Trim() + @"',
                            '" + _dtChk.Rows[i]["F_Collect_Date"].ToString().Trim() + @"',
                            '' ";
                        _KBCN.Execute(_SQL, pUser: _BearerClass,
                                    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                                    pActionName: ControllerContext.ActionDescriptor.ActionName
                                    );

                        _SQL = @" EXEC [exec].[spKBNOR440_04]
                            '" + _data.OrderType.ToString() + @"',
                            '" + _BearerClass.Plant + @"',
                            '" + _BearerClass.UserCode + @"',
                            '' ";
                        DataTable _dtParameter = _KBCN.ExecuteSQL(_SQL, pUser: _BearerClass,
                                    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                                    pActionName: ControllerContext.ActionDescriptor.ActionName
                                    );

                        if (_dtParameter.Rows.Count > 0)
                        {
                            _SQL = @" EXEC [exec].[spKBNOR440_04_I]
                            '" + _data.OrderType.ToString() + @"',
                            '" + _BearerClass.Plant + @"',
                            '" + _BearerClass.UserCode + @"',
                            '" + _PDS_No + @"',
                            '" + _dtChk.Rows[i]["F_Delivery_Date"].ToString().Trim() + @"',
                            '" + _dtChk.Rows[i]["F_Delivery_Time"].ToString().Trim() + @"',
                            '" + _dtChk.Rows[i]["F_Supplier_Cd"].ToString().Trim() + @"',
                            '" + _dtChk.Rows[i]["F_Supplier_Plant"].ToString().Trim() + @"',
                            '" + _dtChk.Rows[i]["F_Store_CD"].ToString().Trim() + @"',
                            '" + _dtChk.Rows[i]["F_Collect_Date"].ToString().Trim() + @"',
                            '" + _dtParameter.Rows[0]["F_Dept_Code"].ToString().Trim() + @"',
                            '" + _dtParameter.Rows[0]["F_Cr"].ToString().Trim() + @"',
                            '" + _dtParameter.Rows[0]["F_DR"].ToString().Trim() + @"',
                            '" + _dtParameter.Rows[0]["F_Value2"].ToString().Trim() + @"',
                            '" + _IssuedDate + @"',
                            '" + _Barcode + @"',
                            '' ";
                            _KBCN.Execute(_SQL, pUser: _BearerClass,
                                        pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                                        pActionName: ControllerContext.ActionDescriptor.ActionName
                                        );
                        }

                        //1.  หาค่า Collect Time ก่อนเลย พอดีเค้าหา Dock Code อยู่แล้วเลยแอบใส่ตัีวนี้ด้วยเลย (ของพี่ทิฝากนิดหนึง กรณี 9Y ไม่ต้องคำนวณตัว Collect Date กะ Time ใส่เป็นว่าง ๆ เลย) 
                        _SQL = @" EXEC [exec].[spKBNOR440_04_U]
                            '" + _data.OrderType.ToString() + @"',
                            '" + _BearerClass.Plant + @"',
                            '" + _BearerClass.UserCode + @"',
                            '' ";
                        _KBCN.Execute(_SQL, pUser: _BearerClass,
                                    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                                    pActionName: ControllerContext.ActionDescriptor.ActionName
                                    );


                        //'2.  ตัวนี้คือกรณีที่ Collect Time = "00:00" แปลว่าเป็นรถที่ขนส่งโดย Milk Run ไม่จำเป็นต้องคำนวณเวลา เลยให้ Collect Date = Delivery Date ส่วน Collect Time ก้อ 00:00 เหมือนเดิม 
                        _SQL = @" EXEC [exec].[spKBNOR440_04_U]
                            '" + _data.OrderType.ToString() + @"',
                            '" + _BearerClass.Plant + @"',
                            '" + _BearerClass.UserCode + @"',
                            '' ";
                        _KBCN.Execute(_SQL, pUser: _BearerClass,
                                    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                                    pActionName: ControllerContext.ActionDescriptor.ActionName
                                    );

                        //'5.  ตัวนี้คือกรณีที่ออกจาก Supplier กลางคืนถึงเราเช้าหลัง 07:30 สรุปว่าจะถอยวัน
                        //'' ตัวเรียกนะ
                        _SQL = @" EXEC [exec].[spKBNOR440_05]
                            '" + _data.OrderType.ToString() + @"',
                            '" + _BearerClass.Plant + @"',
                            '" + _BearerClass.UserCode + @"',
                            '' ";
                        DataTable _dtChkGet = _KBCN.ExecuteSQL(_SQL, pUser: _BearerClass,
                                    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                                    pActionName: ControllerContext.ActionDescriptor.ActionName
                                    );
                        if (_dtChkGet.Rows.Count > 0)
                        {
                            for (int j = 0; j < _dtChkGet.Rows.Count; j++)
                            {
                                String pDelivery = GetDate(_dtChkGet.Rows[j][0].ToString().Trim());
                                _SQL = @" EXEC [exec].[spKBNOR440_05_U]
                                    '" + _data.OrderType.ToString() + @"',
                                    '" + _BearerClass.Plant + @"',
                                    '" + _BearerClass.UserCode + @"',
                                    '" + _dtChkGet.Rows[j]["sDelivery"] + @"',
                                    '" + _dtChkGet.Rows[j][0].ToString() + @"',
                                    '' ";
                                _KBCN.Execute(_SQL, pUser: _BearerClass,
                                            pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                                            pActionName: ControllerContext.ActionDescriptor.ActionName
                                            );

                            }
                        }

                        //'Insert to Detail
                        _SQL = @" EXEC [exec].[spKBNOR440_05_I]
                            '" + _data.OrderType.ToString() + @"',
                            '" + _BearerClass.Plant + @"',
                            '" + _BearerClass.UserCode + @"',
                            '" + _PDS_No + @"',
                            '" + _dtChk.Rows[i]["F_Delivery_Date"].ToString().Trim() + @"',
                            '" + _dtChk.Rows[i]["F_Delivery_Time"].ToString().Trim() + @"',
                            '" + _dtChk.Rows[i]["F_Supplier_Cd"].ToString().Trim() + @"',
                            '" + _dtChk.Rows[i]["F_Supplier_Plant"].ToString().Trim() + @"',
                            '" + _dtChk.Rows[i]["F_Store_CD"].ToString().Trim() + @"',
                            '" + _dtChk.Rows[i]["F_Collect_Date"].ToString().Trim() + @"',
                            '' ";
                        _KBCN.Execute(_SQL, pUser: _BearerClass,
                                    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                                    pActionName: ControllerContext.ActionDescriptor.ActionName
                                    );



                    }
                }


                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Process Complete""
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                _SQL = @" EXEC [exec].[spKBNOR440_EXCEPTION]
                    '" + _data.OrderType.ToString() + @"',
                    '" + _BearerClass.Plant + @"',
                    '" + _BearerClass.UserCode + @"' ";
                _KBCN.Execute(_SQL, pUser: _BearerClass,
                            pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                            pActionName: ControllerContext.ActionDescriptor.ActionName
                            );


                _result = @"{
                    ""status"":""200"",
                    ""response"":""NO"",
                    ""message"": ""Process Not Complete!!!""
                }";
                return Content(_result, "application/json");
            }
        }

        private void GetCheckSum()
        {
            for (int i = 0; i <= 9; i++)
            {
                this._value[i] = i;
                this._data[i] = i.ToString();
            }
            for (int J = 10; J <= 35; J++)
            {
                this._value[J] = J;
                this._data[J] = ((char)(55 + J)).ToString();
            }
            this._value[36] = 36; this._data[36] = "-";
            this._value[37] = 37; this._data[37] = ".";
            this._value[38] = 38; this._data[38] = " ";
            this._value[39] = 39; this._data[39] = "$";
            this._value[40] = 40; this._data[40] = "/";
            this._value[41] = 41; this._data[41] = "+";
            this._value[42] = 42; this._data[42] = "%";
        }


        private string CheckSum(string PDS)
        {
            string Check_Sum = "";
            int nNo = 0;
            for (int i = 0; i < PDS.Length; i++)
            {
                string nCh = PDS.Substring(i, 1);
                int nValue = 0;
                if (nCh.CompareTo("9") <= 0)
                {
                    nValue = this._value[int.Parse(nCh)];
                }
                else
                {
                    nValue = this._value[Convert.ToInt32(nCh[0]) - 55];
                }
                nNo += nValue;
            }
            nNo %= 43;
            Check_Sum = PDS + this._data[nNo];
            return Check_Sum;
        }


        private string GetDate(string pDelivery)
        {
            string _result = "";
            string _SQL = @" EXEC [exec].[spKBNOR440_GetDate]
                            '" + pDelivery.Substring(1, 6) + @"',
                            '' ";
            DataTable _dtChk = _KBCN.ExecuteSQL(_SQL, skipLog: true);
            if (_dtChk.Rows.Count > 0)
            {
                if (Int32.Parse(pDelivery.Substring(7, 2)) > 1)
                {
                    for (int i = Int32.Parse(pDelivery.Substring(7, 2)) - 1; i >= 1; i--)
                    {
                        if (Int32.Parse(_dtChk.Rows[0][i * 2].ToString()) + Int32.Parse(_dtChk.Rows[0][i * 2 + 1].ToString()) == 2)
                        {
                            _result = GetDate(pDelivery.Substring(1, 6) + i.ToString().PadLeft(2, '0'));
                            break;
                        }

                    }
                }
            }

            if (_result == "")
            {
                //'' Calculate last Month
                string sYMLast = DateTime.ParseExact(pDelivery.Substring(4, 2) + "/"
                    + pDelivery.Substring(6, 2) + "/"
                    + pDelivery.Substring(0, 4)
                    , "MMddyyyy", CultureInfo.InvariantCulture).AddMonths(-1).ToString("yyyyMMdd");

                _SQL = @" EXEC [exec].[spKBNOR440_GetDate]
                            '" + pDelivery.Substring(1, 6) + @"',
                            '' ";
                if (_dtChk.Rows.Count > 0)
                {
                    if (Int32.Parse(pDelivery.Substring(7, 2)) > 1)
                    {
                        for (int i = Int32.Parse(pDelivery.Substring(7, 2)) - 1; i >= 1; i--)
                        {
                            if (Int32.Parse(_dtChk.Rows[0][i * 2].ToString()) + Int32.Parse(_dtChk.Rows[0][i * 2 + 1].ToString()) == 2)
                            {
                                _result = GetDate(pDelivery.Substring(1, 6) + i.ToString().PadLeft(2, '0'));
                                break;
                            }

                        }
                    }
                }

            }



            return "";
        }


        //private void GetCheck_Sum()
        //{
        //    for (int i = 0; i <= 9; i++)
        //    {
        //        //this.SumDigit["value"][i] = i.ToString();
        //        //this.SumDigit["data"][i] = i.ToString();
        //        this._value[i] = i;
        //        this._data[i] = i.ToString();
        //    }
        //    for (int i = 10; i <= 35; i++)
        //    {
        //        //this.SumDigit["value"][i] = i.ToString();
        //        //this.SumDigit["data"][i] = ((char)(55 + i)).ToString();
        //        this._value[i] = i;
        //        this._data[i] = i.ToString();
        //    }


        //    //this.SumDigit["value"][36] = "36"; this.SumDigit["data"][36] = "-";
        //    //this.SumDigit["value"][37] = "37"; this.SumDigit["data"][37] = ".";
        //    //this.SumDigit["value"][38] = "38"; this.SumDigit["data"][38] = " ";
        //    //this.SumDigit["value"][39] = "39"; this.SumDigit["data"][39] = "$";
        //    //this.SumDigit["value"][40] = "40"; this.SumDigit["data"][40] = "/";
        //    //this.SumDigit["value"][41] = "41"; this.SumDigit["data"][41] = "+";
        //    //this.SumDigit["value"][42] = "42"; this.SumDigit["data"][42] = "%";

        //    this._value[36] = 36; this._data[36] = "-";
        //    this._value[37] = 37; this._data[37] = ".";
        //    this._value[38] = 38; this._data[38] = " ";
        //    this._value[39] = 39; this._data[39] = "$";
        //    this._value[40] = 40; this._data[40] = "/";
        //    this._value[41] = 41; this._data[41] = "+";
        //    this._value[42] = 42; this._data[42] = "%";
        //}

        //private string Check_Sum(string PDS)
        //{
        //    string _Check_Sum = "";
        //    for (int i = 0; i < PDS.Length; i++)
        //    {
        //        string _ch = PDS.Substring(i + 1, 1).Trim();
        //        int _val = 0;
        //        if (Int32.Parse(_ch) <= 9)
        //        {
        //            _val = this._value[Int32.Parse(_ch)];
        //        }
        //        else
        //        {
        //            _val = this._value[Char.ToUpper(Char.Parse(_ch)) - 55];
        //        }
        //    }


        //    return _Check_Sum;
        //}



    }
}
