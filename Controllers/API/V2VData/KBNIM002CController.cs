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
using NPOI.HPSF;
using Humanizer;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.Eval;
using PdfSharp.Pdf.Filters;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;
using NPOI.POIFS.Properties;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.Master
{
    public class KBNIM002CController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;        
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;

        private readonly KB3Context _KB3Context;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNIM002CController(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            KB3Context kB3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;

        }



        [HttpPost]
        public IActionResult initial([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                if (pData != null) _json = JsonConvert.DeserializeObject(pData);

                _SQL = @" EXEC [exec].[spTB_MS_FACTORY] ";
                string _jsTB_MS_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

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
        //public IActionResult Import([FromBody] string pData = null)
        public async Task<IActionResult> Upload(IFormFile file)
        {
            dynamic _json = null;
            string _SQL = "";
            VBController _VB = new VBController();
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                //_json = JsonConvert.DeserializeObject(pData);


                string fileName = null;

                string orgFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Value;

                //fileName = userid + "_" + usercode;
                fileName += Path.GetFileNameWithoutExtension(orgFileName).ToString().ToUpper().Replace(" ", "") + Path.GetExtension(orgFileName);

                string fullPath = this.StoragePath + @"\" + DateTime.Now.ToString("yyyyMM") + @"\" + DateTime.Now.ToString("dd") + @"\" + fileName;

                // Create the directory.
                Directory.CreateDirectory(Directory.GetParent(fullPath).FullName);

                // Save the file to the server.
                await using FileStream output = System.IO.File.Create(fullPath);
                await file.CopyToAsync(output);
                output.Close();


                int _lineCount = 0; // Initialize a counter for the lines
                using (var _stream = new StreamReader(fullPath))
                {
                    while (!_stream.EndOfStream)
                    {
                        string _line = _stream.ReadLine(); // Read a line from the file
                        if (!string.IsNullOrEmpty(_line)) // Check if the line is not empty
                        {
                            _lineCount++; // Increment the line count
                        }
                    }
                }

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""File is uploaded"",
                    ""data"":
                            {
                                ""count"" : """ + _lineCount + @""",
                                ""file"" : """ + fileName + @"""
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
        public IActionResult Import(string fileName = null)
        {
            dynamic _json = null;
            string _SQL = "", _result;
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                //_json = JsonConvert.DeserializeObject(pData);

                string fullPath = this.StoragePath + @"\" + DateTime.Now.ToString("yyyyMM") + @"\" + DateTime.Now.ToString("dd") + @"\" + fileName;



                _SQL = @" 
    Delete From TB_Import_EKanban Where F_Update_BY='" + _BearerClass.UserCode.ToString() + @"' and F_TYpe ='EKanban'; 

    Delete From TB_Import_Error Where F_Update_BY='" + _BearerClass.UserCode.ToString() + @"' and F_TYpe ='D_EKanban'; 
    
    Delete From TB_Import_Error Where F_Update_BY='" + _BearerClass.UserCode.ToString() + @"' and F_TYpe ='EKanban'; 

    ";
                _KBCN.Execute(_SQL);


                int _current = 1, _typeP = 0, _lineCount = 0;
                string _PO = "", _Item = "", _Delivery = "", _PartNo = "", _Ruibetsu = "";
                
                using (var _stream = new StreamReader(fullPath))
                {
                    while (!_stream.EndOfStream)
                    {
                        _lineCount++;
                        string _OneLine = _stream.ReadLine();
                        var _line = _OneLine.Split(@"|");

                        _SQL = "INSERT INTO TB_Import_EKanban (F_Type,F_ITEM_NO, F_Supplier, F_Supplier_Plant, F_Supplier_Name, F_Plant, F_Plant_Name, F_Receive_Place, F_Order_Type, F_PDS_No, F_EKBPDS_No, ";
                        _SQL = _SQL + "F_Collect_Date, F_Collect_Time, F_Arrival_Date, F_Arrival_Time, F_Main_route_Grp_Code, F_Main_route_Order_Seq, F_Sub_route_Grp_Code1, ";
                        _SQL = _SQL + "F_Sub_route_Order_Seq1, F_Crs1_route, F_Crs1_dock, F_Crs1_arv_Date, F_Crs1_arv_Time, F_Crs1_dpt_Date, F_Crs1_dpt_Time, F_Crs2_route, ";
                        _SQL = _SQL + "F_Crs2_dock, F_Crs2_arv_Date, F_Crs2_arv_Time, F_Crs2_dpt_Date, F_Crs2_dpt_Time, F_Crs3_route, F_Crs3_dock, F_Crs3_arv_Date, ";
                        _SQL = _SQL + "F_Crs3_arv_Time, F_Crs3_dpt_Date, F_Crs3_dpt_Time, F_Supplier_Type, F_No, F_Part_No, F_Part_Name, F_Kanban_No, F_Line_Addr, F_Pack_Qty, ";
                        _SQL = _SQL + "F_Qty, F_Pack, F_Zero_Order, F_Sort_Lane, F_Shipping_Date, F_Shipping_Time, F_Kb_print_Date_p, F_Kb_print_Time_p, F_Kb_print_Date_i, ";
                        _SQL = _SQL + "F_Kb_print_Time_i, F_Remark, F_Order_Release_Date, F_Order_Release_Time, F_Main_route_Date, F_Bill_Out_Flag, F_Shipping_Dock, F_Plant_CD, ";
                        _SQL = _SQL + "F_Update_By, F_Update_Date) VALUES ('EKanban',";

                        for (var i = 0; i < _line.Length; i++)
                        {

                            if (i == 42 || i == 43 || i == 44)
                            {
                                _SQL = _SQL + "" + (_line[i] == "" ? 0 : _line[i]) + ",";
                            }
                            else
                            {
                                _SQL = _SQL + "'" + _line[i] + "',";
                            }

                            if (i == 8) _PO = _line[i];
                            if (i == 0) _Item = _line[i];

                            if (i == 10)
                            {
                                _Delivery = _line[i];
                                _Delivery = _Delivery.Substring(6, 4) + _Delivery.Substring(3, 2) + _Delivery.Substring(0, 2);
                            }
                            if (i == 38)
                            {
                                _PartNo = _line[i];
                                _Ruibetsu = _PartNo.Substring(12, 2);
                                _PartNo = _PartNo.Substring(0, 5) + _PartNo.Substring(6, 5);
                            }

                        }
                        _SQL = _SQL + "'" + _BearerClass.UserCode.ToString() + "', GETDATE())";

                        bool _res = _KBCN.Execute(_SQL);
                        if (!_res)
                        {
                            _result = _ActionResult.Failed(@"Line " + _lineCount + @" : " + _OneLine);
                            return Content(_result, "application/json");

                        }

                        if (!Check_Parent(_PartNo, _Ruibetsu, _Delivery))
                            if (!Check_Construction(_PartNo, _Ruibetsu, _Delivery))
                            {
                                string _SQLError = @"
    INSERT INTO TB_Import_Error (F_TYpe,F_PDS_CD, F_Row, F_Field, F_Remark, F_Update_By, F_Update_Date)
    VALUES('EKanban','" + _PO + "','" + _Item + "','" + _PartNo + "','PART NOT FOUND IN MASTER','" + _BearerClass.UserCode.ToString() + "', GETDATE()) ";
                                _KBCN.Execute(_SQL);

                            }

                    }
                }


                _SQL = @"
    DELETE E
    FROM TB_Import_EKanban E 
    INNER JOIN TB_TRANSACTION T ON e.F_PDS_No = T.F_PDS_No 
							    AND Substring(e.F_Collect_Date, 7, 4) + Substring(e.F_Collect_Date, 4, 2) + Substring(e.F_Collect_Date, 1, 2) = T.F_Delivery_Date
							    AND E.F_Type = T.F_Type
    WHERE 1=1
    AND E.F_TYpe ='EKanban' 
    AND E.F_Update_By='" + _BearerClass.UserCode.ToString() + @"' 
";  
                _KBCN.Execute(_SQL);


                _SQL = @"
    DELETE E
    FROM TB_Import_EKanban E 
    INNER JOIN TB_TRANSACTION_TMP T ON e.F_PDS_No = T.F_PDS_No 
								    AND Substring(e.F_Collect_Date, 7, 4) + Substring(e.F_Collect_Date, 4, 2) + Substring(e.F_Collect_Date, 1, 2) = T.F_Delivery_Date
								    AND E.F_Type = T.F_Type
    WHERE 1=1
    AND E.F_TYpe ='EKanban' 
    AND E.F_Update_By='" + _BearerClass.UserCode.ToString() + @"' 
";
                _KBCN.Execute(_SQL);



                //''Change to Data Filter : Store Procedure
                _SQL = "EXEC dbo.SP_IM003_FilterData '" + _BearerClass.UserCode.ToString() + "' ";
                _KBCN.Execute(_SQL);



                //''---- - Addition Check Data Duplicate After Filter Data : 26 / 02 / 2019 : Chirawan Add
                string _NPart = "";
                //'1. Check Construction 
                _SQL = @"
    SELECT Distinct F_Part_No,F_Ruibetsu,F_Store_Cd,F_Sebango 
    FROM TMP_Construction 
    WHERE 1=1
    AND F_update_BY='" + _BearerClass.UserCode.ToString() + @"' 
    AND substring(F_TC_END,1,4) = '2999'
    GROUP BY F_Part_No, F_Ruibetsu, F_Store_Cd, F_Sebango
    HAVING COUNT(*) > 1";
                DataTable _dtCon = _KBCN.ExecuteSQL(_SQL);
                if (_dtCon.Rows.Count > 0)
                {
                    for (int i = 0; i < _dtCon.Rows.Count; i++)
                    {
                        _NPart = _NPart + "[" + _dtCon.Rows[i][0].ToString().Trim() + "-" +
                            _dtCon.Rows[i][1].ToString().Trim() + "-" +
                            _dtCon.Rows[i][2].ToString().Trim() + "],<br> ";
                    }

                    _result = _ActionResult.Failed(@"Please Check Data in Construction.<br>" + _NPart + @" have duplicate time control");
                    return Content(_result, "application/json");
                }


                //'2. Check parent Child
                _SQL = @"
    SELECT Distinct F_Parent_part,F_Ruibetsu,F_Store_Cd,F_Child_Part,F_Ch_Ruibetsu,F_ch_store_cd 
    FROM TMP_Parents_Child 
    WHERE 1=1
    AND F_update_BY='" + _BearerClass.UserCode.ToString() + @"' 
    AND substring(F_TC_END,1,4) = '2999'
    AND F_Store_Cd in ('00','01','05') and F_ch_store_cd <>'RM' 
    GROUP BY F_Parent_part,F_Ruibetsu,F_Store_Cd,F_Child_Part,F_Ch_Ruibetsu,F_ch_store_cd
    HAVING COUNT(*) > 1";
                DataTable _dtPC = _KBCN.ExecuteSQL(_SQL);


                if(_dtPC.Rows.Count > 0)
                {
                    for (int i = 0; i < _dtPC.Rows.Count; i++)
                    {
                        _NPart = _NPart + "[" + _dtPC.Rows[i][0].ToString().Trim() + "-" +
                            _dtPC.Rows[i][1].ToString().Trim() + "-" +
                            _dtPC.Rows[i][2].ToString().Trim() + ":" +
                            _dtPC.Rows[i][3].ToString().Trim() + "-" +
                            _dtPC.Rows[i][4].ToString().Trim() + "-" +
                            _dtPC.Rows[i][5].ToString().Trim() + "],<br> ";
                    }

                    _result = _ActionResult.Failed(@"Please Check Data in Parents Child.<br>" + _NPart + @" have duplicate time control");
                    return Content(_result, "application/json");
                }




                //'' === Check Have data in Parent Part
                _SQL = @"
    INSERT INTO TB_IMPORT_ERROR(F_TYpe,F_PDS_CD, F_Row, F_Field, F_Remark, F_Update_By, F_Update_Date)
    SELECT 'EKanban',S.F_PDS_NO,S.F_ITEM_NO,S.F_PART_NO AS F_Part_NO,'Not Found Data IN Parent Child','" + _BearerClass.UserCode.ToString() + @"',GETDATE()
    FROM TB_IMPORT_EKANBAN S 
        LEFT OUTER JOIN(
                SELECT * 
                FROM TMP_Parents_Child C 
                WHERE C.F_Update_By  = '" + _BearerClass.UserCode.ToString() + @"' 
                AND F_Store_Cd IN ('00','01') 
			    AND F_TC_Str COLLATE Thai_CI_AS <= (
												    SELECT MIN(SUBSTRING(F_Collect_Date, 7, 4) + SUBSTRING(F_Collect_Date, 4, 2) + SUBSTRING(F_Collect_Date, 1, 2))
												    FROM TB_IMPORT_EKANBAN 
												    WHERE F_Update_By = '" + _BearerClass.UserCode.ToString() + @"' 
												    AND F_TYpe = 'EKanban'
												    AND SUBSTRING(F_PART_NO,1,5)+SUBSTRING(F_PART_NO, 7, 5) = C.F_PARENT_PART COLLATE Thai_CI_AS
												    AND SUBSTRING(F_PART_NO,13,2) = C.F_Ruibetsu COLLATE Thai_CI_AS
												    )
			    AND F_TC_End COLLATE Thai_CI_AS >= (
												    SELECT MAX(SUBSTRING(F_Collect_Date, 7, 4) + SUBSTRING(F_Collect_Date, 4, 2) + SUBSTRING(F_Collect_Date, 1, 2))
												    FROM TB_IMPORT_EKANBAN 
												    WHERE F_Update_By = '" + _BearerClass.UserCode.ToString() + @"' 
												    AND F_TYpe = 'EKanban'
												    AND SUBSTRING(F_PART_NO,1,5)+SUBSTRING(F_PART_NO, 7, 5) = C.F_PARENT_PART COLLATE Thai_CI_AS
												    AND SUBSTRING(F_PART_NO,13,2) = C.F_Ruibetsu COLLATE Thai_CI_AS
												    )
	    ) P ON SUBSTRING(S.F_PART_NO, 1, 5) + SUBSTRING(S.F_PART_NO, 7, 5) = P.F_PArent_Part COLLATE Thai_CI_AS
	    AND SUBSTRING(S.F_PART_NO,13,2) = P.F_Ruibetsu COLLATE Thai_CI_AS
    WHERE S.F_Update_By = '" + _BearerClass.UserCode.ToString() + @"' 
    AND F_TYpe = 'EKanban' 
    AND RTRIM(P.F_Parent_Part) IS NULL
";
                _KBCN.Execute(_SQL);

                //sql_Chk_Error = "Select * from TB_IMPORT_ERROR Where F_Update_BY='" & User_Logon & "' and F_Type ='D_" & Type_Import & "' "
                //frmKBNIM_003_ERR.rpt
                //RPT_SQL = "{TB_IMPORT_ERROR.F_Update_By}='" & User_Logon & "' and {TB_IMPORT_ERROR.F_Type}='D_" & Type_Import & "' "


                //sql_Chk_Error = "Select * from TB_IMPORT_ERROR Where F_Update_BY='" & User_Logon & "' and F_Type ='" & Type_Import & "' "
                //frmKBNIM_003_ERR.rpt
                //RPT_SQL = "{TB_IMPORT_ERROR.F_Update_By}='" & User_Logon & "' and {TB_IMPORT_ERROR.F_Type}='" & Type_Import & "' "



                _SQL = @"
    INSERT INTO  TB_EKanban_History(F_Supplier, F_Supplier_Plant, F_Supplier_Dock, F_Dock, F_PDS_NO, F_Date, F_OrderNo, F_Shipping_Date, F_Shipping_Time, F_Arrival_Date, 
    F_Arrival_Time, F_Import_By, F_Import_Date)
    SELECT E.* 
	FROM (
		SELECT DISTINCT F_Supplier, F_Supplier_Plant, '' AS F_SUpplier_Dock, F_Receive_Place, F_PDS_No, SUBSTRING(F_EKBPDS_No, 1, 8) AS F_Order_Date
        , SUBSTRING(F_EKBPDS_No, 9, 2) AS F_order_No
        , SUBSTRING(F_Collect_Date,7,4) + SUBSTRING(F_Collect_Date,4,2) + SUBSTRING(F_Collect_Date,1,2) as F_Collect_Date
        , SUBSTRING(F_Collect_Time,1,2) + SUBSTRING(F_Collect_Time,4,2) as F_Collect_Time
        , SUBSTRING(F_Arrival_Date,7,4) + SUBSTRING(F_Arrival_Date,4,2) + SUBSTRING(F_Arrival_Date,1,2) as F_Arrival_Date
        , SUBSTRING(F_Arrival_Time,1,2) + SUBSTRING(F_Arrival_Time,4,2) as F_Arrival_Time
        , '" + _BearerClass.UserCode.ToString() + @"' as F_Update_By
        , GETDATE() as F_Update_Date
		FROM TB_Import_EKanban 
		WHERE 1=1
		AND F_Update_BY='" + _BearerClass.UserCode.ToString() + @"' 
		AND F_TYpe='EKanban'
		)E 
		LEFT OUTER join (
			SELECT * 
            FROM TB_EKanban_History 
            WHERE F_CHG_Shipping_Date=''
		)H ON E.F_PDS_No = H.F_PDS_No 
            AND E.F_Order_Date = H.F_Date 
            AND E.F_order_No = H.F_orderNo
    WHERE H.F_PDS_NO is null 
    ";
                _KBCN.Execute(_SQL);



                //''Change Delivery Date From Slide Order Data
                _SQL = @"
    UPDATE TB_EKanban_History Set F_PDS_NO = I.F_PDS_NO 
    FROM TB_EKANBAN_History E 
        INNER JOIN TB_Import_EKANBAN I ON  E.F_PDS_NO = I.F_PDS_NO 
                                        AND e.F_Supplier = I.F_SUpplier 
                                        AND e.F_Supplier_Plant = I.F_Supplier_Plant 
                                        AND e.F_Dock = I.F_Receive_Place
                                        AND rtrim(E.F_Date) + rtrim(E.F_OrderNO) = rtrim(I.F_EKBPDS_NO)
                                        AND E.F_Shipping_Date = Substring(I.F_Collect_Date,7,4)+Substring(I.F_Collect_Date,4,2)+Substring(I.F_Collect_Date,1,2) 
                                        AND E.F_Shipping_Time = substring(I.F_Collect_Time,1,2)+substring(I.F_Collect_Time,4,2)
    WHERE I.F_Update_By='" + _BearerClass.UserCode.ToString() + @"' 
    AND I.F_TYPE='EKanban' ";
                _KBCN.Execute(_SQL);


                _SQL = @"
    Update TB_IMPORT_EKANBAN set F_Collect_Date = substring(E.F_Chg_SHipping_Date ,7,2) +'/'+ substring(E.F_Chg_SHipping_Date,5,2) +'/'+substring(E.F_Chg_SHipping_Date,1,4),
        F_Collect_Time = substring(E.F_CHG_Shipping_Time,1,2)+':'+substring(E.F_CHG_Shipping_Time,3,2)
    from TB_EKANBAN_History E 
        INNER JOIN TB_Import_EKANBAN I ON e.F_Supplier = I.F_SUpplier 
                                        And e.F_Supplier_Plant = I.F_Supplier_Plant 
                                        And e.F_Dock = I.F_Receive_Place
                                        and rtrim(E.F_Date) + rtrim(E.F_OrderNO) = rtrim(I.F_EKBPDS_NO)
                                        and E.F_Shipping_Date = Substring(I.F_Collect_Date,7,4)+Substring(I.F_Collect_Date,4,2)+Substring(I.F_Collect_Date,1,2) 
                                        and E.F_Shipping_Time = substring(I.F_Collect_Time,1,2)+substring(I.F_Collect_Time,4,2)
    WHERE I.F_Update_By='" + _BearerClass.UserCode.ToString() + @"' 
    and I.F_TYPE='EKanban' 
    and E.F_CHG_SHipping_Date <> '' 
    and E.F_CHG_SHIPPING_DATE + E.F_CHG_SHIPPING_TIME in (
                                                        Select max(F_Chg_SHipping_Date+F_Chg_Shipping_TIME) 
                                                        From TB_EKANBAN_History
                                                        Where F_Supplier = e.F_Supplier 
                                                        And F_Supplier_Plant = e.F_Supplier_Plant 
                                                        And F_Dock = e.F_Dock
                                                        and F_Date = E.F_Date 
                                                        and F_OrderNO = E.F_OrderNO 
                                                        and F_Shipping_Date =  E.F_Shipping_Date 
                                                        and F_Shipping_Time = E.F_Shipping_Time
                                                        ) ";
                _KBCN.Execute(_SQL);



                bool _BreakDown = BreakDown(_BearerClass);
















                _result = @"{
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



        private bool BreakDown(BearerClass _BearerClass)
        {
            string _SQL = "";
            try
            {

                //''====== Break Down Part into TB_TMP
                //''Insert to Data Temp in case Other : Maru G : 05
                _SQL = @"

    Insert into TB_Transaction_TMP(F_Type, F_Plant, F_PDS_No, F_PDS_Issued_Date, F_Store_CD, F_Part_No, F_Ruibetsu, F_Kanban_No, F_Part_Name, F_Qty_Pack, F_Part_Order,
        F_Ruibetsu_Order, F_Store_Order, F_Name_Order, F_Qty, F_Qty_Level1, F_Seq_No, F_Seq_Type, F_Delivery_Date, F_Adv_Deli_Date, F_OrderType, F_Country, F_Reg_Flg,
        F_Inventory_Flg, F_Supplier_CD, F_Supplier_Plant, F_Cycle_Time, F_Safty_Stock, F_Part_Refer, F_Ruibetsu_Refer, F_Update_By, F_Update_Date, F_Remark, F_Ratio)
    select 'EKanban' as F_Type,' " + @"' as F_Plant, S.F_PDS_No,substring(F_Collect_Date, 7, 4) + substring(F_Collect_Date, 4, 2) + substring(F_Collect_Date, 1, 2) as F_PDS_Issued_Date,
    P.F_CH_Store_Cd as F_Store_CD,P.F_Child_Part as F_Part_No,P.F_CH_Ruibetsu as F_ruibetsu,case when substring(P.F_CH_Store_Cd, 1, 1) = '0' then '' else '0' + rtrim(CON.F_Sebango) end F_Kanban_No,case when substring(P.F_CH_Store_Cd, 1, 1) = '0' then '' else Con.F_Part_Nm end F_Part_Name,
    case when substring(P.F_CH_Store_Cd, 1, 1) = '0' then 0 else Con.F_Qty_Box end F_Qty_Pack, substring(S.F_PART_NO, 1, 5) + substring(S.F_PART_NO, 7, 5) as F_Part_Order,substring(S.F_PART_NO, 13, 2) as F_Ruibetsu_Order,P.F_Store_Cd as F_Store_Order, PP.F_Name as F_Name_order,
    S.F_Qty* P.F_Use_Pieces as F_Qty,
    S.F_Qty as F_Qty_Level1,'' as F_Seq_No,
    '' as F_Seq_Type, substring(S.F_Collect_Date, 7, 4) + substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) as F_Delivery_Date, 
    Case when S.F_Collect_Time >= '00:01' and
    S.F_Collect_Time <= (SELECT F_Start_Time FROM TB_MS_CTL WHERE(F_Plant = '1') AND(F_Shift = 'A')) 
    then convert(Char(8),dateadd(day, -1, substring(S.F_Collect_Date, 4, 3) + substring(S.F_Collect_Date, 1, 3) + substring(S.F_Collect_Date, 7, 4)),112)
    else substring(S.F_Collect_Date, 7, 4) + substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) end as F_Adv_Deli_Date,'N' as F_OrderType,S.F_Collect_Time as F_Country,'0' as F_Reg_Flg,
    '0' as F_Inventory_Flg, case when substring(P.F_CH_Store_Cd, 1, 1) = '0' then '' else Con.F_Supplier_Cd end F_Supplier_CD,case when substring(P.F_CH_Store_Cd, 1, 1) = '0' then '' else Con.F_Plant end F_Supplier_Plant, 
    '' as F_CycleTime,0 as F_Safety_Stk,
    '' as F_Part_Refer,'' as F_Ruibetsu_Refer,'" + _BearerClass.UserCode.ToString() + @"' as F_Update_By,getdate() as F_Update_Date,'' as F_Remark
    ,case when not(Con.F_Ratio_N) is null then cast(Con.F_Ratio_N as nchar(3)) else case when rtrim(P.F_Sel_Part) = '' then '' else cast(Con.F_Ratio as nchar(3)) end end F_ratio
    from(Select F_PDS_NO, F_part_no, F_Collect_Date, F_Collect_Time, Sum(F_Qty) as F_QTY, substring(F_PDS_NO, 1, 2) as F_PDS_SUB From TB_Import_EKanban Where F_Update_BY = '" + _BearerClass.UserCode.ToString() + @"' and F_TYpe = 'EKanban'
    Sql = Sql & Group by F_PDS_NO, F_part_no, F_Collect_Date, F_Collect_Time, substring(F_PDS_NO, 1, 2)
    Sql = Sql & )S INNER JOIN(Select * from dbo.TMP_PARENTS_CHILD Where F_Update_BY = '" + _BearerClass.UserCode.ToString() + @"')P ON
    substring(S.F_PART_NO, 1, 5) + substring(S.F_PART_NO, 7, 5) = rtrim(P.F_PARENT_PART) collate Thai_CI_AS
    AND substring(S.F_PART_NO,13,2) = P.F_ruibetsu collate Thai_CI_AS
    and P.F_Store_Cd in ('05'),'01'
    and substring(S.F_Collect_Date,7,4)+substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) >= P.F_TC_Str collate Thai_CI_AS
    and substring(S.F_Collect_Date,7,4)+substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) <= P.F_TC_End collate Thai_CI_AS
    LEFT OUTER JOIN(select C.F_Part_No, C.F_Ruibetsu, C.F_Store_CD, C.F_TC_Str, C.F_TC_End, C.F_Supplier_Cd, C.F_Plant, C.F_Sebango, C.F_QTY_BOX, C.F_Ratio, C.F_Part_NM, P.F_Ratio_N from
    (Select F_Part_NO, F_Ruibetsu, F_Store_Cd, F_TC_Str, F_TC_End, F_Supplier_Cd, F_Plant, F_Sebango, F_Ratio, F_Part_Nm, F_QTY_BOX from TMP_CONSTRUCTION Where F_Update_BY = '" + _BearerClass.UserCode.ToString() + @"' and  F_Supplier_CD<>'9997')C
    LEFT OUTER JOIN(Select F_part_no, F_RUibetsu, F_Store_CD, F_Ratio_N from dbo.TMP_PRG006_parameter Where F_Update_BY = '" + _BearerClass.UserCode.ToString() + @"') P
    ON C.F_Part_no = P.F_Part_no and C.F_Ruibetsu = P.F_Ruibetsu and C.F_Store_Cd = P.F_Store_Cd)CON
    ON P.F_Child_Part = CON.F_Part_NO collate Thai_CI_AS
    and P.F_Ch_Ruibetsu = Con.F_ruibetsu collate Thai_CI_AS
    and P.F_Ch_Store_Cd = Con.F_Store_CD collate Thai_CI_AS
    and substring(S.F_Collect_Date,7,4)+substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) >= Con.F_TC_Str collate Thai_CI_AS
    and substring(S.F_Collect_Date,7,4)+substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) <= Con.F_TC_End  collate Thai_CI_AS
    LEFT OUTER JOIN(Select * from dbo.TMP_PARENT_PART Where F_Update_BY = ' & User_Logon & ')PP ON
    P.F_Parent_Part = PP.F_Parent_Part And P.F_Ruibetsu = PP.F_Ruibetsu And P.F_Store_CD = PP.F_Store_CD
    and substring(S.F_Collect_Date,7,4)+substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) >= PP.F_TC_Str collate Thai_CI_AS
    and substring(S.F_Collect_Date,7,4)+substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) <= PP.F_TC_End collate Thai_CI_AS
    Order by S.F_PDS_NO";
                _KBCN.Execute(_SQL);


                //''Insert to Data Temp in case Other : 00 : assy
                _SQL = @"
    Insert into TB_Transaction_TMP (F_Type, F_Plant, F_PDS_No, F_PDS_Issued_Date, F_Store_CD, F_Part_No, F_Ruibetsu, F_Kanban_No, F_Part_Name, F_Qty_Pack, F_Part_Order, 
        F_Ruibetsu_Order, F_Store_Order, F_Name_Order,F_Qty,F_Qty_Level1, F_Seq_No, F_Seq_Type, F_Delivery_Date, F_Adv_Deli_Date, F_OrderType, F_Country, F_Reg_Flg, 
        F_Inventory_Flg, F_Supplier_CD, F_Supplier_Plant, F_Cycle_Time, F_Safty_Stock, F_Part_Refer, F_Ruibetsu_Refer, F_Update_By, F_Update_Date,F_Remark,F_Ratio)
    select 'EKanban' as F_Type,'" + @"' as F_Plant, S.F_PDS_No,substring(F_Collect_Date, 7, 4) + substring(F_Collect_Date, 4, 2) + substring(F_Collect_Date, 1, 2) as F_PDS_Issued_Date,
    P.F_CH_Store_Cd as F_Store_CD,P.F_Child_Part as F_Part_No,P.F_CH_Ruibetsu as F_ruibetsu,case when P.F_CH_Store_Cd = '00' then '' else '0' + rtrim(CON.F_Sebango) end F_Kanban_No,case when substring(P.F_CH_Store_Cd, 1, 1) = '0' then '' else Con.F_Part_Nm end F_Part_Name,
    case when substring(P.F_CH_Store_Cd, 1, 1) = '0' then 0 else Con.F_Qty_Box end F_Qty_Pack, substring(S.F_PART_NO, 1, 5) + substring(S.F_PART_NO, 7, 5) as F_Part_Order,substring(S.F_PART_NO, 13, 2) as F_Ruibetsu_Order,P.F_Store_Cd as F_Store_Order, PP.F_Name as F_Name_order,
    S.F_Qty* P.F_Use_Pieces as F_Qty,
    S.F_Qty as F_Qty_Level1,'' as F_Seq_No,
    '' as F_Seq_Type, substring(S.F_Collect_Date, 7, 4) + substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) as F_Delivery_Date, 
    Case when S.F_Collect_Time >= '00:01' and
    S.F_Collect_Time <= (SELECT F_Start_Time FROM TB_MS_CTL WHERE(F_Plant = '1') AND(F_Shift = 'A')) 
    then convert(Char(8),dateadd(day, -1, substring(S.F_Collect_Date, 4, 3) + substring(S.F_Collect_Date, 1, 3) + substring(S.F_Collect_Date, 7, 4)),112)
    else substring(S.F_Collect_Date, 7, 4) + substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) end as F_Adv_Deli_Date,'N' as F_OrderType,S.F_Collect_Time as F_Country,'0' as F_Reg_Flg,
    '0' as F_Inventory_Flg, case when P.F_CH_Store_Cd = '00' then '' else Con.F_Supplier_Cd end F_Supplier_CD,case when P.F_CH_Store_Cd = '00' then '' else Con.F_Plant end F_Supplier_Plant, 
    '' as F_CycleTime,0 as F_Safety_Stk,
    '' as F_Part_Refer,'' as F_Ruibetsu_Refer,'" + _BearerClass.UserCode.ToString() + @"' as F_Update_By,getdate() as F_Update_Date,'' as F_Remark
    ,case when not(Con.F_Ratio_N) is null then cast(Con.F_Ratio_N as nchar(3)) else case when rtrim(P.F_Sel_Part) = '' then '' else cast(Con.F_Ratio as nchar(3)) end end F_ratio
    from(Select F_PDS_NO, F_part_no, F_Collect_Date, F_Collect_Time, Sum(F_Qty) as F_QTY, substring(F_PDS_NO, 1, 2) as F_PDS_SUB From TB_Import_EKanban Where F_Update_BY = '" + _BearerClass.UserCode.ToString() + @"' and F_TYpe = 'EKanban'
    Group by F_PDS_NO, F_part_no, F_Collect_Date, F_Collect_Time, substring(F_PDS_NO, 1, 2)
    )S INNER JOIN(Select * from dbo.TMP_PARENTS_CHILD Where F_Update_BY = '" + _BearerClass.UserCode.ToString() + @"')P ON
    substring(S.F_PART_NO, 1, 5) + substring(S.F_PART_NO, 7, 5) = rtrim(P.F_PARENT_PART) collate Thai_CI_AS
    AND substring(S.F_PART_NO,13,2) = P.F_ruibetsu collate Thai_CI_AS
    and P.F_Store_Cd in ('00'),'01'
    and substring(S.F_Collect_Date,7,4)+substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) >= P.F_TC_Str collate Thai_CI_AS
    and substring(S.F_Collect_Date,7,4)+substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) <= P.F_TC_End collate Thai_CI_AS
    LEFT OUTER JOIN(select C.F_Part_No, C.F_Ruibetsu, C.F_Store_CD, C.F_TC_Str, C.F_TC_End, C.F_Supplier_Cd, C.F_Plant, C.F_Sebango, C.F_QTY_BOX, C.F_Ratio, C.F_Part_NM, P.F_Ratio_N from
    (Select F_Part_NO, F_Ruibetsu, F_Store_Cd, F_TC_Str, F_TC_End, F_Supplier_Cd, F_Plant, F_Sebango, F_Ratio, F_Part_Nm, F_QTY_BOX from TMP_CONSTRUCTION Where F_Update_BY = '" + _BearerClass.UserCode.ToString() + @"' and F_Supplier_CD<>'9997')C
    LEFT OUTER JOIN(Select F_part_no, F_RUibetsu, F_Store_CD, F_Ratio_N from TMP_PRG006_parameter WHERE F_Update_BY = '" + _BearerClass.UserCode.ToString() + @"') P
    ON C.F_Part_no = P.F_Part_no and C.F_Ruibetsu = P.F_Ruibetsu and C.F_Store_Cd = P.F_Store_Cd)CON
    ON P.F_Child_Part = CON.F_Part_NO collate Thai_CI_AS
    and P.F_Ch_Ruibetsu = Con.F_ruibetsu collate Thai_CI_AS
    and P.F_Ch_Store_Cd = Con.F_Store_CD collate Thai_CI_AS
    and substring(S.F_Collect_Date,7,4)+substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) >= Con.F_TC_Str collate Thai_CI_AS
    and substring(S.F_Collect_Date,7,4)+substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) <= Con.F_TC_End  collate Thai_CI_AS
    LEFT OUTER JOIN(Select * from dbo.TMP_PARENT_PART Where F_Update_BY = '" + _BearerClass.UserCode.ToString() + @"')PP ON
    P.F_Parent_Part = PP.F_Parent_Part And P.F_Ruibetsu = PP.F_Ruibetsu And P.F_Store_CD = PP.F_Store_CD
    and substring(S.F_Collect_Date,7,4)+substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) >= PP.F_TC_Str collate Thai_CI_AS
    and substring(S.F_Collect_Date,7,4)+substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) <= PP.F_TC_End collate Thai_CI_AS
    LEFT OUTER JOIN(Select * from TB_Transaction_TMP Where F_Update_By = '" + _BearerClass.UserCode.ToString() + @"' and F_Type = 'EKanban' and F_Plant = '" + @"')TMP
    ON substring(S.F_PART_NO, 1, 5) + substring(S.F_PART_NO, 7, 5) = TMP.F_PART_ORDER and S.F_PDS_No = TMP.F_PDS_NO and substring(F_Collect_Date,7,4)+substring(F_Collect_Date, 4, 2) + substring(F_Collect_Date, 1, 2) = TMP.F_PDS_ISSUED_DATE
    Where TMP.F_PART_ORDER IS NULL and substring(S.F_PDS_NO, 1, 2) = S.F_PDS_SUB Order by S.F_PDS_NO
    ";
                _KBCN.Execute(_SQL);


                //'' for 01 Only
                _SQL = @"
    Insert into TB_Transaction_TMP(F_Type, F_Plant, F_PDS_No, F_PDS_Issued_Date, F_Store_CD, F_Part_No, F_Ruibetsu, F_Kanban_No, F_Part_Name, F_Qty_Pack, F_Part_Order,
        F_Ruibetsu_Order, F_Store_Order, F_Name_Order, F_Qty, F_Qty_Level1, F_Seq_No, F_Seq_Type, F_Delivery_Date, F_Adv_Deli_Date, F_OrderType, F_Country, F_Reg_Flg,
        F_Inventory_Flg, F_Supplier_CD, F_Supplier_Plant, F_Cycle_Time, F_Safty_Stock, F_Part_Refer, F_Ruibetsu_Refer, F_Update_By, F_Update_Date, F_Remark, F_Ratio)
    select 'EKanban' as F_Type,' + ' as F_Plant, S.F_PDS_No,substring(F_Collect_Date, 7, 4) + substring(F_Collect_Date, 4, 2) + substring(F_Collect_Date, 1, 2) as F_PDS_Issued_Date,
    P.F_CH_Store_Cd as F_Store_CD,P.F_Child_Part as F_Part_No,P.F_CH_Ruibetsu as F_ruibetsu,case when P.F_CH_Store_Cd = '00' then '' else '0' + rtrim(CON.F_Sebango) end F_Kanban_No,case when P.F_CH_Store_Cd = '00' then '' else Con.F_Part_Nm end F_Part_Name,
    case when P.F_CH_Store_Cd = '00' then 0 else Con.F_Qty_Box end F_Qty_Pack, substring(S.F_PART_NO, 1, 5) + substring(S.F_PART_NO, 7, 5) as F_Part_Order,substring(S.F_PART_NO, 13, 2) as F_Ruibetsu_Order,P.F_Store_Cd as F_Store_Order, PP.F_Name as F_Name_order,
    S.F_Qty* P.F_Use_Pieces as F_Qty,
    S.F_Qty,'' as F_Seq_No,
    '' as F_Seq_Type, substring(S.F_Collect_Date, 7, 4) + substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) as F_Delivery_Date, 
    Case when S.F_Collect_Time >= '00:01' and
    S.F_Collect_Time <= (SELECT F_Start_Time FROM TB_MS_CTL WHERE(F_Plant = '1') AND(F_Shift = 'A')) 
    then convert(Char(8),dateadd(day, -1, substring(S.F_Collect_Date, 4, 3) + substring(S.F_Collect_Date, 1, 3) + substring(S.F_Collect_Date, 7, 4)),112)
    else substring(S.F_Collect_Date, 7, 4) + substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) end as F_Adv_Deli_Date,'N' as F_OrderType,S.F_Collect_Time as F_Country,'0' as F_Reg_Flg,
    '0' as F_Inventory_Flg, case when P.F_CH_Store_Cd = '00' then '' else Con.F_Supplier_Cd end F_Supplier_CD,case when P.F_CH_Store_Cd = '00' then '' else Con.F_Plant end F_Supplier_Plant, 
    '' as F_CycleTime,0 as F_Safety_Stk,
    '' as F_Part_Refer,'' as F_Ruibetsu_Refer,'" + _BearerClass.UserCode.ToString() + @"' as F_Update_By,getdate() as F_Update_Date,'' as F_Remark
    ,case when not(Con.F_Ratio_N) is null then cast(Con.F_Ratio_N as nchar(3)) else case when rtrim(P.F_Sel_Part) = '' then '' else cast(Con.F_Ratio as nchar(3)) end end F_ratio
    from(Select F_PDS_NO, F_part_no, F_Collect_Date, F_Collect_Time, Sum(F_Qty) as F_QTY, substring(F_PDS_NO, 1, 2) as F_PDS_SUB From TB_Import_EKanban Where F_Update_BY = '" + _BearerClass.UserCode.ToString() + @"' and F_TYpe = 'EKanban'
    Sql = Sql & Group by F_PDS_NO, F_part_no, F_Collect_Date, F_Collect_Time, substring(F_PDS_NO, 1, 2)
    Sql = Sql & )S INNER JOIN(Select * from dbo.TMP_PARENTS_CHILD Where F_Update_BY = '" + _BearerClass.UserCode.ToString() + @"')P ON
    substring(S.F_PART_NO, 1, 5) + substring(S.F_PART_NO, 7, 5) = rtrim(P.F_PARENT_PART) collate Thai_CI_AS
    AND substring(S.F_PART_NO,13,2) = P.F_ruibetsu collate Thai_CI_AS
    and P.F_Store_Cd in ('01') and substring(P.F_Ch_Store_Cd,1,1) in ('0', '1')
    and substring(S.F_Collect_Date,7,4)+substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) >= P.F_TC_Str collate Thai_CI_AS
    and substring(S.F_Collect_Date,7,4)+substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) <= P.F_TC_End collate Thai_CI_AS
    LEFT OUTER JOIN(select C.F_Part_No, C.F_Ruibetsu, C.F_Store_CD, C.F_TC_Str, C.F_TC_End, C.F_Supplier_Cd, C.F_Plant, C.F_Sebango, C.F_QTY_BOX, C.F_Ratio, C.F_Part_NM, P.F_Ratio_N from
    (Select F_Part_NO, F_Ruibetsu, F_Store_Cd, F_TC_Str, F_TC_End, F_Supplier_Cd, F_Plant, F_Sebango, F_Ratio, F_Part_Nm, F_QTY_BOX from TMP_CONSTRUCTION Where F_Update_BY = '" + _BearerClass.UserCode.ToString() + @"' and F_Supplier_CD<>'9997')C
    LEFT OUTER JOIN(Select F_part_no, F_RUibetsu, F_Store_CD, F_Ratio_N from TMP_PRG006_parameter WHERE F_Update_BY = '" + _BearerClass.UserCode.ToString() + @"') P
    ON C.F_Part_no = P.F_Part_no and C.F_Ruibetsu = P.F_Ruibetsu and C.F_Store_Cd = P.F_Store_Cd)CON
    ON P.F_Child_Part = CON.F_Part_NO collate Thai_CI_AS
    and P.F_Ch_Ruibetsu = Con.F_ruibetsu collate Thai_CI_AS
    and P.F_Ch_Store_Cd = Con.F_Store_CD collate Thai_CI_AS
    and substring(S.F_Collect_Date,7,4)+substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) >= Con.F_TC_Str collate Thai_CI_AS
    and substring(S.F_Collect_Date,7,4)+substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) <= Con.F_TC_End  collate Thai_CI_AS
    LEFT OUTER JOIN(Select * from dbo.TMP_PARENT_PART Where F_Update_BY = '" + _BearerClass.UserCode.ToString() + @"')PP ON
    P.F_Parent_Part = PP.F_Parent_Part And P.F_Ruibetsu = PP.F_Ruibetsu And P.F_Store_CD = PP.F_Store_CD
    and substring(S.F_Collect_Date,7,4)+substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) >= PP.F_TC_Str collate Thai_CI_AS
    and substring(S.F_Collect_Date,7,4)+substring(S.F_Collect_Date, 4, 2) + substring(S.F_Collect_Date, 1, 2) <= PP.F_TC_End collate Thai_CI_AS and PP.F_Plant_Cd = ' + '
    LEFT OUTER JOIN(Select * from TB_Transaction_TMP Where F_Update_By = '" + _BearerClass.UserCode.ToString() + @"' and F_Type = 'EKanban' and F_Plant = ' + ')TMP
    ON substring(S.F_PART_NO, 1, 5) + substring(S.F_PART_NO, 7, 5) = TMP.F_PART_ORDER and S.F_PDS_No = TMP.F_PDS_NO and substring(F_Collect_Date,7,4)+substring(F_Collect_Date, 4, 2) + substring(F_Collect_Date, 1, 2) = TMP.F_PDS_ISSUED_DATE
    Where TMP.F_PART_ORDER IS NULL and substring(S.F_PDS_NO, 1, 2) = S.F_PDS_SUB Order by S.F_PDS_NO";
                _KBCN.Execute(_SQL);







                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }



        //    [HttpPost]
        //    //public IActionResult Import([FromBody] string pData = null)
        //    public async Task<IActionResult> Import(IFormFile file)
        //    {
        //        dynamic _json = null;
        //        string _SQL = "";
        //        VBController _VB = new VBController();
        //        try
        //        {
        //            _BearerClass.Authentication(Request);
        //            if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

        //            //_json = JsonConvert.DeserializeObject(pData);


        //            string fileName = null;

        //            string orgFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Value;

        //            //fileName = userid + "_" + usercode;
        //            fileName += Path.GetFileNameWithoutExtension(orgFileName).ToString().ToUpper().Replace(" ", "") + Path.GetExtension(orgFileName);

        //            string fullPath = this.StoragePath + @"\" + DateTime.Now.ToString("yyyyMMdd") + @"\" + fileName;

        //            // Create the directory.
        //            Directory.CreateDirectory(Directory.GetParent(fullPath).FullName);

        //            // Save the file to the server.
        //            await using FileStream output = System.IO.File.Create(fullPath);
        //            await file.CopyToAsync(output);
        //            output.Close();


        //            //_CSVPath = Directory.GetParent(_CSVPath).FullName + "\\" + filecsv;





        //            _SQL = @" 
        //Delete From TB_Import_EKanban Where F_Update_BY='" + _BearerClass.UserCode.ToString() + @"' and F_TYpe ='EKanban'; 

        //Delete From TB_Import_Error Where F_Update_BY='" + _BearerClass.UserCode.ToString() + @"' and F_TYpe ='D_EKanban'; 

        //Delete From TB_Import_Error Where F_Update_BY='" + _BearerClass.UserCode.ToString() + @"' and F_TYpe ='EKanban'; 

        //";
        //            _KBCN.Execute(_SQL);


        //            int _current = 1, _typeP = 0;
        //            string _PO = "", _Item = "", _Delivery = "", _PartNo = "", _Ruibetsu = "";

        //            using (var _stream = new StreamReader(fullPath))
        //            {
        //                while (!_stream.EndOfStream)
        //                {
        //                    var _line = _stream.ReadLine().Split(@"|");
        //                    //string _OneLine = _stream.ReadLine();

        //                    _SQL = "INSERT INTO TB_Import_EKanban (F_Type,F_ITEM_NO, F_Supplier, F_Supplier_Plant, F_Supplier_Name, F_Plant, F_Plant_Name, F_Receive_Place, F_Order_Type, F_PDS_No, F_EKBPDS_No, ";
        //                    _SQL = _SQL + "F_Collect_Date, F_Collect_Time, F_Arrival_Date, F_Arrival_Time, F_Main_route_Grp_Code, F_Main_route_Order_Seq, F_Sub_route_Grp_Code1, ";
        //                    _SQL = _SQL + "F_Sub_route_Order_Seq1, F_Crs1_route, F_Crs1_dock, F_Crs1_arv_Date, F_Crs1_arv_Time, F_Crs1_dpt_Date, F_Crs1_dpt_Time, F_Crs2_route, ";
        //                    _SQL = _SQL + "F_Crs2_dock, F_Crs2_arv_Date, F_Crs2_arv_Time, F_Crs2_dpt_Date, F_Crs2_dpt_Time, F_Crs3_route, F_Crs3_dock, F_Crs3_arv_Date, ";
        //                    _SQL = _SQL + "F_Crs3_arv_Time, F_Crs3_dpt_Date, F_Crs3_dpt_Time, F_Supplier_Type, F_No, F_Part_No, F_Part_Name, F_Kanban_No, F_Line_Addr, F_Pack_Qty, ";
        //                    _SQL = _SQL + "F_Qty, F_Pack, F_Zero_Order, F_Sort_Lane, F_Shipping_Date, F_Shipping_Time, F_Kb_print_Date_p, F_Kb_print_Time_p, F_Kb_print_Date_i, ";
        //                    _SQL = _SQL + "F_Kb_print_Time_i, F_Remark, F_Order_Release_Date, F_Order_Release_Time, F_Main_route_Date, F_Bill_Out_Flag, F_Shipping_Dock, F_Plant_CD, ";
        //                    _SQL = _SQL + "F_Update_By, F_Update_Date) VALUES ('EKanban',";

        //                    for (var i=0; i<_line.Length; i++) {

        //                        if (i == 42 || i == 43 || i == 44)
        //                        {
        //                            _SQL = _SQL + "" + (_line[i] == "" ? 0 : _line[i]) + ",";
        //                        }
        //                        else
        //                        {
        //                            _SQL = _SQL + "'" + _line[i] + "',";
        //                        }

        //                        if (i == 8) _PO = _line[i];
        //                        if (i == 0) _Item = _line[i];

        //                        if (i == 10) {
        //                            _Delivery = _line[i];
        //                            _Delivery = _Delivery.Substring(6, 4) + _Delivery.Substring(3, 2) + _Delivery.Substring(0, 2);
        //                        }
        //                        if (i == 38) {
        //                            _PartNo = _line[i];
        //                            _Ruibetsu = _PartNo.Substring(12, 2);
        //                            _PartNo = _PartNo.Substring(0, 5) + _PartNo.Substring(6, 5);
        //                        }

        //                    }
        //                    _SQL = _SQL + "'" + _BearerClass.UserCode.ToString() + "', GETDATE())";

        //                    _KBCN.Execute(_SQL);


        //                    if(!Check_Parent(_PartNo, _Ruibetsu, _Delivery))
        //                        if(!Check_Construction(_PartNo, _Ruibetsu, _Delivery))
        //                        {
        //                            string _SQLError = @"
        //INSERT INTO TB_Import_Error (F_TYpe,F_PDS_CD, F_Row, F_Field, F_Remark, F_Update_By, F_Update_Date)
        //VALUES('EKanban','" + _PO + "','" + _Item + "','" + _PartNo + "','PART NOT FOUND IN MASTER','" + _BearerClass.UserCode.ToString() + "', GETDATE()) ";
        //                            _KBCN.Execute(_SQL);

        //                        }

        //                }
        //            }



        //            string _result = @"{
        //                ""status"":""200"",
        //                ""response"":""OK"",
        //                ""message"": ""Data has been save""
        //            }";
        //            return Content(_result, "application/json");
        //        }
        //        catch (Exception e)
        //        {
        //            return Content(e.Message.ToString(), "application/json");
        //        }
        //    }



        [HttpPost]
        public IActionResult checkProgress()
        {
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                string _SQL = @"
    Select isnull(count(*),0) as cnt 
    from TB_Import_EKanban 
    Where F_Update_BY='" + _BearerClass.UserCode.ToString() + @"' 
    and F_TYpe ='EKanban' ";
                string _js = _KBCN.ExecuteJSON(_SQL, skipLog: true);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Progress status"",
                    ""data"": " + _js + @"
                }";
                return Content(_result, "application/json");

            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }




        private Boolean Check_Construction(String pPartNO = "", String pRuibetsu = "", String pDelivery = "")
        {
            Boolean _return = false;
            string _SQL = @"
    SELECT * 
    FROM T_Construction 
    WHERE F_Part_No='" + pPartNO + @"' 
    AND F_Ruibetsu ='" + pRuibetsu + @"' ";
            _SQL = _SQL + " AND F_Local_Str <='" + pDelivery + "' ";
            _SQL = _SQL + " AND F_Local_End >='" + pDelivery + "' ";
            DataTable _dt = _PPMConnect.ExecuteSQL(_SQL, skipLog: true);

            if (_dt.Rows.Count > 0) _return = true;


            return _return;
        }
        private Boolean Check_Parent(String pPartNO = "", String pRuibetsu = "", String pDelivery = "")
        {
            Boolean _return = false;
            string _SQL = @"
    SELECT * 
    FROM T_Parent_Part 
    WHERE F_Parent_Part='" + pPartNO + @"' 
    AND F_Ruibetsu ='" + pRuibetsu + @"' 
    AND F_Store_Cd in ('00')";
            _SQL = _SQL + " AND F_TC_Str <='" + pDelivery + "' ";
            _SQL = _SQL + " AND F_TC_End >='" + pDelivery + "' ";
            DataTable _dt = _PPMConnect.ExecuteSQL(_SQL, skipLog: true);

            if (_dt.Rows.Count > 0) _return = true;


            return _return;
        }


    }
}
