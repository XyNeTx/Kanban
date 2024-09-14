using HINOSystem.Context;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Spire.Barcode;
using System.ComponentModel.DataAnnotations;


namespace HINOSystem.Controllers.API.Master
{
    public class KBNDL001Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;

        private readonly KB3Context _KB3Context;


        private readonly string StoragePath = @"wwwroot\Storage\Image\Barcode";
        private readonly string StoragePathQRCode = @"wwwroot\Storage\Image\QRCode";

        

        private Dictionary<string, string[]> SumDigit = new Dictionary<string, string[]>();


        private int[] _value = new int[43];
        private string[] _data = new string[43];

        public KBNDL001Controller(
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

            try
            {

                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }


                _SQL = @" EXEC [exec].[spKBNDL001_INI_PDS] '1'";
                string _jsPDSNo = _KBCN.ExecuteJSONKB1(_SQL, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                _SQL = @" EXEC [exec].[spKBNDL001_INI_SUPPLIER] '1'";
                string _jsSupplier = _KBCN.ExecuteJSONKB1(_SQL, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""PDSNo"" : " + _jsPDSNo + @",
                                ""Supplier"" : " + _jsSupplier + @"
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
        public IActionResult PDS_GENBARCODE([FromBody] string pPostData = null)
        {
            dynamic _data = null;
            string _SQL, _resData, fileName = "";
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""data"": null
                }";

            try
            {

                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                string _pdsno = Convert.ToString(_data.PDSNO);
                string[] _arr = _pdsno.Substring(0, _pdsno.Length-1).Split(',');

                for (int i=0; i<_arr.Length; i++)
                {
                    string _p = _arr[i];

                    string _path = Path.Combine(StoragePath, DateTime.Now.ToString("yyyyMM"));
                    if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);
                    BarcodeSettings _setting = new BarcodeSettings();
                    _setting.Type = BarCodeType.Code128;
                    _setting.Data = _p;
                    BarCodeGenerator _barcode = new BarCodeGenerator(_setting);
                    _barcode.GenerateImage().Save(_path + @"/" + _p + @".png");


                    //GeneratedBarcode barcode = IronBarCode.BarcodeWriter.CreateBarcode(_p, BarcodeWriterEncoding.Code128);
                    //barcode.ResizeTo(1000, 100);
                    //barcode.AddBarcodeValueTextBelowBarcode();
                    //barcode.ChangeBarCodeColor(Color.Black);
                    //barcode.SetMargins(0);
                    //string path = Path.Combine(StoragePath, "202404");
                    //if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    //string filePath = Path.Combine(path, _p + @".png");
                    //barcode.SaveAsPng(filePath);
                    //fileName = Path.GetFileName(filePath);
                    ////string imageUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/202404/" + fileName;
                    ////ViewBag.QrCodeUri = imageUrl;

                }

                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"": """ + _pdsno + @"""
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                _result = @"{
                    ""status"":""200"",
                    ""response"":""NO"",
                    ""message"": ""ERROR : " + e.Message + @"""
                }";
                return Content(_result, "application/json");
            }

        }




        [HttpPost]
        public IActionResult PDS_GENQRCODE([FromBody] string pPostData = null)
        {
            dynamic _data = null;
            string _SQL, _resData, fileName = "";
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""data"": null
                }";
            try
            {

                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                string _pdsno = Convert.ToString(_data.PDSNO);
                string[] _arr = _pdsno.Substring(0, _pdsno.Length - 1).Split(',');

                for (int i = 0; i < _arr.Length; i++)
                {
                    string _p = _arr[i];

                    string[] _f = _arr[i].Split('|');
                    string _fileName = _f[11] + _f[0] + _f[9] + _f[14].Replace("/","_");

                    string _path = Path.Combine(StoragePathQRCode, DateTime.Now.ToString("yyyyMM"));
                    if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);
                    BarcodeSettings _setting = new BarcodeSettings();
                    _setting.Type = BarCodeType.QRCode;
                    _setting.Data = _p;
                    _setting.Data2D = _p;
                    _setting.ShowText = false;
                    _setting.ImageWidth = 200;
                    _setting.ImageHeight = 200;
                    _setting.BottomMargin = 1;
                    _setting.TopMargin = 1;
                    _setting.RightMargin = 1;
                    _setting.LeftMargin = 1;
                    _setting.QRCodeDataMode = QRCodeDataMode.AlphaNumber;
                    _setting.X = 1.0f;
                    _setting.QRCodeECL = QRCodeECL.H;


                    BarCodeGenerator _qrcode = new BarCodeGenerator(_setting);
                    _qrcode.GenerateImage().Save(_path + @"/" + _fileName + @".png");

                }

                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"": """ + _pdsno + @"""
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                _result = @"{
                    ""status"":""200"",
                    ""response"":""NO"",
                    ""message"": ""ERROR : " + e.Message + @"""
                }";
                return Content(_result, "application/json");
            }

        }









        [HttpPost]
        public IActionResult PDS_INSERTDATA([FromBody] string pPostData = null)
        {
            dynamic _data = null;
            string _SQL, _resData;
            string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""No data found"",
                    ""data"": null
                }";

            try
            {

                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                if (pPostData != null) _data = JsonConvert.DeserializeObject(pPostData);

                //Clear  TB_PDS_DETAIL
                _SQL = @" EXEC [exec].[spKBNDL001RPT_PDS] 
                    '" + _BearerClass.UserCode + @"',
                    '1',
                    '20240101',
                    '1A24032201U04',
                    '1A24032501U03',
                    '20240101',
                    '20240401',
                    '' ";
                //DataTable _dt = _KBCN.ExecuteSQL(_SQL, pUser: _BearerClass,
                //    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                //    pActionName: ControllerContext.ActionDescriptor.ActionName
                //    );

                //if (_dtChk.Rows.Count > 0)
                //{
                //    for (int i = 0; i < _dtChk.Rows.Count; i++)
                //    {
                //        String _Remark = "";

                //        _SQL = @" EXEC [exec].[spKBNOR440_01_D] 
                //            '" + _data.OrderType.ToString() + @"',
                //            '" + _BearerClass.Plant + @"',
                //            '" + _BearerClass.UserCode + @"',
                //            '" + _dtChk.Rows[i]["F_OrderNo"].ToString() + @"',
                //            '' ";
                //        _KBCN.Execute(_SQL, pUser: _BearerClass,
                //                    pControllerName: ControllerContext.ActionDescriptor.ControllerName,
                //                    pActionName: ControllerContext.ActionDescriptor.ActionName
                //                    );
                //    }
                //}

                string _dt = _KBCN.ExecuteJSONKB1(_SQL, pUser: _BearerClass, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);


                _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"": " + _dt + @"
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


    }







    public class GenerateBarcodeModel
    {
        [Display(Name = "Enter Barcode Text")]
        public string BarcodeText
        {
            get;
            set;
        }
    }
















}
