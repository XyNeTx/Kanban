//using System.Net.Mail;
//using EASendMail;
using HINOSystem.Libs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Globalization;

namespace HINOSystem.Controllers
{
    public class APIController : Controller
    {
        private readonly WarrantyClaimConnect _wrtConnect;
        private readonly EmailClass _email;


        private readonly DbConnect _dbConnect;

        public APIController(WarrantyClaimConnect wrtConnect, EmailClass email, DbConnect dbConnect)
        {
            _wrtConnect = wrtConnect;
            _email = email;

            _wrtConnect.setContext(HttpContext);

            _dbConnect = dbConnect;
        }



        [HttpPost]
        public IActionResult getLocation()
        {
            //return View();


            string _statement = @"
                SELECT * FROM [ISS].[dbo].[INLOCD] WHERE 1 = 1;
            ";
            //DataTable _dataTable = _dbConnect.ExecuteSQL(_statement);

            string _jsonData = _dbConnect.ExecuteJSON(_statement);

            string _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""Created"",
                ""data"":" + _jsonData + @"
            }";


            return Content(_result, "application/json");
        }

        [HttpPost]
        public IActionResult setLocation()
        {
            //return View();


            string _statement = @"
                UPDATE [ISS].[dbo].[INLOCD]
                SET [F_Dept_cd] ='" + Request.Form["toDept"].ToString() + @"'
                WHERE [LocationKey] = '" + Request.Form["location"].ToString() + @"' 
                AND [F_Dept_cd] = '" + Request.Form["fromDept"].ToString() + @"';


                SELECT * FROM [ISS].[dbo].[INLOCD] WHERE 1 = 1 AND [LocationKey] = '" + Request.Form["location"].ToString() + @"' ;
            ";

            string _jsonData = _dbConnect.ExecuteJSON(_statement);

            string _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""Created"",
                ""data"":" + _jsonData + @"
            }";


            return Content(_result, "application/json");
        }



        [HttpGet, HttpPost]
        public IActionResult getAOBList()
        {
            string _statement = @"
				SELECT *
				FROM [ISS].[dbo].[INTXCH_2022]
				WHERE 1=1
				AND Locationkey = '40002'
				AND TransactionType = 'A'
				AND TransactionSubType = 'OB'
                AND original = 1
                AND Split=0
				ORDER BY InEventID                
            ";

            string _jsonData = _dbConnect.ExecuteJSON(_statement);

            string _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""Get List"",
                ""data"":" + _jsonData + @"
            }";


            return Content(_result, "application/json");
        }


        [HttpPost]
        public IActionResult onSpliteRow(string pData)
        {

            string _sql = "";
            JObject _JSON = JObject.Parse(pData);
            string _ItemKey = _JSON.GetValue("ItemKey").ToString().Trim();
            if (_ItemKey == "EFERE12071")
            {
                string _debug = "";
            }


            string _statement = @"
				SELECT *
				FROM [ISS].[dbo].[INTXCH_2022]
				WHERE 1=1
				AND Locationkey = '40002'
				AND TransactionType = 'A'
				AND TransactionSubType = 'OB'
                AND original = 1
                AND Split=0
				AND ItemKey = '" + _ItemKey + @"'
				ORDER BY InEventID
                
            ";
            // AND ItemKey IN ('EACFZTAK01','EACHNDE001','EACIPID002','EACIPID007','EACISSM001','EACISSM002','EACKEFU009','EACKEFU010','EACLMCM049','EACLMKI001','EACLMMC007','EACLMMC008','EACLMMC009','EACLMNA001','EACLMOM001')

            DataTable _dtAOB = _dbConnect.ExecuteSQL(_statement);

            string _jsonData = "";

            for (int i = 0; i < _dtAOB.Rows.Count; i++)
            {
                float _Qty_AOB = float.Parse(_dtAOB.Rows[i]["StockQty"].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                float _Qty_P = 0;
                DataTable _dtNew = new DataTable();

                for (int c = 0; c < _dtAOB.Columns.Count; c++)
                {
                    _dtNew.Columns.Add(_dtAOB.Columns[c].ToString());
                }


                string _SQL = @"
				        SELECT *
				        FROM [ISS].[dbo].[INTXCH_2022]
				        WHERE 1=1
				        AND Locationkey = '30002'
                        AND (TransactionType = 'P' OR (TransactionType = 'A' AND TransactionSubType = 'OB'))
				        AND ItemKey = '" + _dtAOB.Rows[i]["ItemKey"].ToString() + @"'
                        ORDER BY InEventID DESC
                    ";
                DataTable _dtP = _dbConnect.ExecuteSQL(_SQL);

                for (int p = 0; p < _dtP.Rows.Count; p++)
                {
                    _Qty_P += float.Parse(_dtP.Rows[p]["StockQty"].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    string _InEventID = _dtP.Rows[p]["InEventID"].ToString();
                    _dtNew.Rows.Add(new object[] { });

                    if (_Qty_P <= _Qty_AOB)
                    {
                        //INSERT FULL
                        //_Qty_New.Rows.Add(new Object[] { p, _InEventID, _Qty_P });

                        for (int c = 0; c < _dtNew.Columns.Count; c++)
                        {
                            //_dtNew.Rows[p][c] = _dtP.Rows[p][c];
                            switch (c)
                            {
                                case 0: _dtNew.Rows[p][c] = _ItemKey; break;
                                case 1: _dtNew.Rows[p][c] = "40002"; break;
                                case 4: _dtNew.Rows[p][c] = "A"; break;
                                case 5: _dtNew.Rows[p][c] = "OB"; break;
                                case 7: _dtNew.Rows[p][c] = _Qty_P; break;
                                case 9: _dtNew.Rows[p][c] = _Qty_P; break;
                                case 10: _dtNew.Rows[p][c] = _Qty_P * float.Parse(_dtAOB.Rows[i][c].ToString()); break;
                                //case 11: _dtNew.Rows[p][c] = "REF-" + _dtP.Rows[p][11]; break;
                                case 13: _dtNew.Rows[p][c] = _dtP.Rows[p][13]; break;
                                case 14: _dtNew.Rows[p][c] = _dtP.Rows[p][14]; break;
                                case 23: _dtNew.Rows[p][c] = "REF-" + _dtP.Rows[p][11].ToString().Trim() + "-" + _dtP.Rows[p][2].ToString().Trim(); break;
                                case 43: _dtNew.Rows[p][c] = _dtAOB.Rows[i][2]; break;
                                default: _dtNew.Rows[p][c] = _dtAOB.Rows[i][c]; break;
                            }

                        }

                    }
                    else
                    {
                        //INSERT 
                        //_Qty_New.Rows.Add(new Object[] { p, _InEventID, _Qty_AOB });

                        for (int c = 0; c < _dtNew.Columns.Count; c++)
                        {

                            switch (c)
                            {
                                case 0: _dtNew.Rows[p][c] = _ItemKey; break;
                                case 1: _dtNew.Rows[p][c] = "40002"; break;
                                case 4: _dtNew.Rows[p][c] = "A"; break;
                                case 5: _dtNew.Rows[p][c] = "OB"; break;
                                case 7: _dtNew.Rows[p][c] = _Qty_AOB; break;
                                case 9: _dtNew.Rows[p][c] = _Qty_AOB; break;
                                case 10: _dtNew.Rows[p][c] = _Qty_AOB * float.Parse(_dtAOB.Rows[i][c].ToString()); break;
                                //case 11: _dtNew.Rows[p][c] = "REF-"+_dtP.Rows[p][11]; break;
                                case 13: _dtNew.Rows[p][c] = _dtP.Rows[p][13]; break;
                                case 14: _dtNew.Rows[p][c] = _dtP.Rows[p][14]; break;
                                case 23: _dtNew.Rows[p][c] = "REF-" + _dtP.Rows[p][11].ToString().Trim() + "-" + _dtP.Rows[p][2].ToString().Trim(); break;
                                case 43: _dtNew.Rows[p][c] = _dtAOB.Rows[i][2]; break;
                                default: _dtNew.Rows[p][c] = _dtAOB.Rows[i][c]; break;
                            }

                        }
                    }
                    _Qty_AOB = _Qty_AOB - _Qty_P;


                    string _SQLINS = @"
INSERT INTO INTXCH_2022 
(ItemKey ,Locationkey ,CostType ,TransactionType ,TransactionSubType ,ReasonCode ,StockQty ,StockUnitCost ,StockQtyRemain ,StockAmt ,DocumentNumber ,SysLinSq ,DocumentDate ,ApplyDate ,StockUOM ,BinNumber ,ProductKey ,CommodityKey ,NLAcctKey ,INAcctKey ,IncDecFlag ,ApplyToEventID ,Reference ,Factory ,Section ,DeptCode ,MechineCode ,PeriodCode ,ProjectCode ,WorkCode ,ApplyTo ,PODocID ,ToLocation ,PeriodStatus ,SpareTxt1 ,SpareTxt2 ,SpareNum1 ,SpareNum2 ,SpareDate ,RecUserID ,RecDate ,RecTime ,original)
VALUES
('" + _dtNew.Rows[p][0] + @"',
'" + _dtNew.Rows[p][1] + @"',
'" + _dtNew.Rows[p][3] + @"',
'" + _dtNew.Rows[p][4] + @"',
'" + _dtNew.Rows[p][5] + @"',
'" + _dtNew.Rows[p][6] + @"',
'" + _dtNew.Rows[p][7] + @"',
'" + _dtNew.Rows[p][8] + @"',
'" + _dtNew.Rows[p][9] + @"',
'" + _dtNew.Rows[p][10] + @"',
'" + _dtNew.Rows[p][11] + @"',
'" + _dtNew.Rows[p][12] + @"',
'" + _dtNew.Rows[p][13] + @"',
'" + _dtNew.Rows[p][14] + @"',
'" + _dtNew.Rows[p][15] + @"',
'" + _dtNew.Rows[p][16] + @"',
'" + _dtNew.Rows[p][17] + @"',
'" + _dtNew.Rows[p][18] + @"',
'" + _dtNew.Rows[p][19] + @"',
'" + _dtNew.Rows[p][20] + @"',
'" + _dtNew.Rows[p][21] + @"',
'" + _dtNew.Rows[p][22] + @"',
'" + _dtNew.Rows[p][23] + @"',
'" + _dtNew.Rows[p][24] + @"',
'" + _dtNew.Rows[p][25] + @"',
'" + _dtNew.Rows[p][26] + @"',
'" + _dtNew.Rows[p][27] + @"',
'" + _dtNew.Rows[p][28] + @"',
'" + _dtNew.Rows[p][29] + @"',
'" + _dtNew.Rows[p][30] + @"',
'" + _dtNew.Rows[p][31] + @"',
'" + _dtNew.Rows[p][32] + @"',
'" + _dtNew.Rows[p][33] + @"',
'" + _dtNew.Rows[p][34] + @"',
'" + _dtNew.Rows[p][35] + @"',
'" + _dtNew.Rows[p][36] + @"',
'" + _dtNew.Rows[p][37] + @"',
'" + _dtNew.Rows[p][38] + @"',
'" + _dtNew.Rows[p][39] + @"',
'" + _dtNew.Rows[p][40] + @"',
'" + _dtNew.Rows[p][41] + @"',
'" + _dtNew.Rows[p][42] + @"',
'" + _dtNew.Rows[p][43] + @"'
);
";
                    _dbConnect.Execute(_SQLINS);



                    if (_Qty_AOB <= 0) p = _dtP.Rows.Count;


                }


                _jsonData += JsonConvert.SerializeObject(_dtNew);


                string _SQLUPDATE = @"
				UPDATE [ISS].[dbo].[INTXCH_2022]
                SET Split=1
				WHERE 1=1
				AND Locationkey = '40002'
				AND TransactionType = 'A'
				AND TransactionSubType = 'OB'
                AND original = 1
                AND Split=0
				AND ItemKey = '" + _ItemKey + @"'                
            ";
                _dbConnect.Execute(_SQLUPDATE);


            }



            //string _jsonData = _dbConnect.ExecuteJSON(_statement);

            string _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""Created"",
                ""data"":" + _jsonData + @"
            }";


            return Content(_result, "application/json");
        }







        [HttpPost]
        public IActionResult agingStock()
        {
            string _statement = @"
				SELECT TOP (20) *
				FROM [ISS].[dbo].[INTXCH_2022]
				WHERE 1=1
				AND Locationkey = '40002'
				AND TransactionType = 'A'
				AND TransactionSubType = 'OB'
                
            ";
            // AND ItemKey IN ('EACFZTAK01','EACHNDE001','EACIPID002','EACIPID007','EACISSM001','EACISSM002','EACKEFU009','EACKEFU010','EACLMCM049','EACLMKI001','EACLMMC007','EACLMMC008','EACLMMC009','EACLMNA001','EACLMOM001')

            DataTable _dtAOB = _dbConnect.ExecuteSQL(_statement);

            string _jsonData = "";

            for (int i = 0; i < _dtAOB.Rows.Count; i++)
            {
                float _Qty_AOB = float.Parse(_dtAOB.Rows[i]["StockQty"].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                float _Qty_P = 0;
                DataTable _dtNew = new DataTable();

                for (int c = 0; c < _dtAOB.Columns.Count; c++)
                {
                    _dtNew.Columns.Add(_dtAOB.Columns[c].ToString());
                    //_Qty_New.Columns.Add("row_number");
                    //_Qty_New.Columns.Add("InEventID");
                    //_Qty_New.Columns.Add("QTY");
                }


                string _SQL = @"
				        SELECT *
				        FROM [ISS].[dbo].[INTXCH_2022]
				        WHERE 1=1
				        AND Locationkey = '30002'
				        AND TransactionType = 'P'
				        AND ItemKey = '" + _dtAOB.Rows[i]["ItemKey"].ToString() + @"'
                        ORDER BY InEventID DESC
                    ";
                DataTable _dtP = _dbConnect.ExecuteSQL(_SQL);

                for (int p = 0; p < _dtP.Rows.Count; p++)
                {
                    _Qty_P += float.Parse(_dtP.Rows[p]["StockQty"].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    string _InEventID = _dtP.Rows[p]["InEventID"].ToString();
                    if (_Qty_P <= _Qty_AOB)
                    {
                        //INSERT FULL
                        //_Qty_New.Rows.Add(new Object[] { p, _InEventID, _Qty_P });

                        _dtNew.Rows.Add(new object[] { });
                        for (int c = 0; c < _dtNew.Columns.Count; c++)
                        {
                            //_dtNew.Rows[p][c] = _dtP.Rows[p][c];
                            switch (c)
                            {
                                case 1: _dtNew.Rows[p][c] = "40002"; break;
                                case 4: _dtNew.Rows[p][c] = "A"; break;
                                case 5: _dtNew.Rows[p][c] = "OB"; break;
                                case 7: _dtNew.Rows[p][c] = _Qty_P; break;
                                case 9: _dtNew.Rows[p][c] = _Qty_P; break;
                                case 10: _dtNew.Rows[p][c] = _Qty_P * float.Parse(_dtAOB.Rows[i][c].ToString()); break;
                                //case 11: _dtNew.Rows[p][c] = "REF-" + _dtP.Rows[p][11]; break;
                                case 13: _dtNew.Rows[p][c] = _dtP.Rows[p][13]; break;
                                case 14: _dtNew.Rows[p][c] = _dtP.Rows[p][14]; break;
                                case 23: _dtNew.Rows[p][c] = "REF-" + _dtP.Rows[p][11]; break;
                                case 43: _dtNew.Rows[p][c] = _dtAOB.Rows[i][2]; break;
                                default: _dtNew.Rows[p][c] = _dtAOB.Rows[i][c]; break;
                            }

                        }

                    }
                    else
                    {
                        //INSERT 
                        //_Qty_New.Rows.Add(new Object[] { p, _InEventID, _Qty_AOB });

                        _dtNew.Rows.Add(new object[] { });
                        for (int c = 0; c < _dtNew.Columns.Count; c++)
                        {

                            switch (c)
                            {
                                case 1: _dtNew.Rows[p][c] = "40002"; break;
                                case 4: _dtNew.Rows[p][c] = "A"; break;
                                case 5: _dtNew.Rows[p][c] = "OB"; break;
                                case 7: _dtNew.Rows[p][c] = _Qty_AOB; break;
                                case 9: _dtNew.Rows[p][c] = _Qty_AOB; break;
                                case 10: _dtNew.Rows[p][c] = _Qty_AOB * float.Parse(_dtAOB.Rows[i][c].ToString()); break;
                                //case 11: _dtNew.Rows[p][c] = "REF-"+_dtP.Rows[p][11]; break;
                                case 13: _dtNew.Rows[p][c] = _dtP.Rows[p][13]; break;
                                case 14: _dtNew.Rows[p][c] = _dtP.Rows[p][14]; break;
                                case 23: _dtNew.Rows[p][c] = "REF-" + _dtP.Rows[p][11]; break;
                                case 43: _dtNew.Rows[p][c] = _dtAOB.Rows[i][2]; break;
                                default: _dtNew.Rows[p][c] = _dtAOB.Rows[i][c]; break;
                            }

                        }
                    }
                    _Qty_AOB = _Qty_AOB - _Qty_P;

                    if (_Qty_AOB <= 0) p = _dtP.Rows.Count;

                }


                _jsonData += JsonConvert.SerializeObject(_dtNew);

            }



            //string _jsonData = _dbConnect.ExecuteJSON(_statement);

            string _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""Created"",
                ""data"":" + _jsonData + @"
            }";


            return Content(_result, "application/json");
        }







        [HttpPost]
        public IActionResult api230202()
        {
            string _statement = @"
                SELECT *
                FROM INTXCH
                WHERE 1=1
                AND TransactionType = 'A'
                AND TransactionSubType = 'OB'
                AND original IS NULL
                AND Locationkey = '40002'
                AND DocumentNumber LIKE 'ADJ%22-12-19-IN'
                AND original1 = 1
                ORDER BY ItemKey, InEventID
            ";
            // AND ItemKey IN ('EACFZTAK01','EACHNDE001','EACIPID002','EACIPID007','EACISSM001','EACISSM002','EACKEFU009','EACKEFU010','EACLMCM049','EACLMKI001','EACLMMC007','EACLMMC008','EACLMMC009','EACLMNA001','EACLMOM001')

            DataTable _dtOld = _dbConnect.ExecuteSQL(_statement);

            //string _jsonData = "";

            for (int i = 0; i < _dtOld.Rows.Count; i++)
            {
                string _InEventID = _dtOld.Rows[i]["InEventID"].ToString();
                string _StockQty = _dtOld.Rows[i]["StockQty"].ToString();
                string _StockUnitCost = _dtOld.Rows[i]["InEventID"].ToString();
                string _StockAmt = _dtOld.Rows[i]["StockAmt"].ToString();
                string _StockQtyRemain = _dtOld.Rows[i]["StockQtyRemain"].ToString();
                string _DocumentDate = _dtOld.Rows[i]["DocumentDate"].ToString();


                string _sql = @"
                    UPDATE INTXCH
                    SET StockQty = '"+ _StockQty + @"'
                        ,StockUnitCost = '" + _StockUnitCost + @"'
                        ,StockAmt = '" + _StockAmt + @"'
                        ,DocumentDate = '" + _DocumentDate + @"'
                        ,StockQtyRemain = '" + _StockQtyRemain + @"'
                    WHERE 1=1
                    AND original = '" + _InEventID + @"'
                ";
                _dbConnect.Execute(_sql);


                _statement = @"
                    SELECT *
                    FROM INTXCH
                    WHERE 1=1
                    AND original = '" + _InEventID + @"'
                ";
                DataTable _dtNew = _dbConnect.ExecuteSQL(_statement);
                if(_dtNew.Rows.Count > 0)
                {
                    string _InEventID_new = _dtNew.Rows[0]["InEventID"].ToString();

                    _sql = @"
                    UPDATE INLAYER
                    SET E_Apply = '" + _InEventID_new + @"'
                    WHERE 1=1
                    AND E_Apply = '" + _InEventID + @"'
                ";
                    _dbConnect.Execute(_sql);
                }


                _sql = @"
                    UPDATE INTXCH
                    SET original1 = '2'
                    WHERE 1=1
                    AND InEventID = '" + _InEventID + @"'
                ";
                _dbConnect.Execute(_sql);



                //    _jsonData += JsonConvert.SerializeObject(_dtNew);

            }



            string _jsonData = _dbConnect.ExecuteJSON(_statement);

            string _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""api230202"",
                ""data"":" + _jsonData + @"
            }";


            return Content(_result, "application/json");
        }











    }
}
