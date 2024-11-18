using ClosedXML.Excel;
using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public class KBNOR297 : IKBNOR297
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly ISpecialLibs _specialLibs;


        public KBNOR297
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            ISpecialLibs specialLibs
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _specialLibs = specialLibs;
        }

        public string GenReportExcel(string CustOrderNo)
        {
            try
            {

                using var workbook = new XLWorkbook("\\\\hmmt-app03\\Event_log\\Template_Outreport_SPC_KB\\Template_Out_Special_Part_Del_Hide.xlsx");
                var worksheet = workbook.Worksheet("Page1");
                var workRow = worksheet.Row(9);
                workRow.Cell(27).Value = CustOrderNo;

                var dt_row = GetRowCount(CustOrderNo);
                int rowCount = int.Parse(dt_row.Rows[0]["F_Row_Count"].ToString().Trim());
                var dtTop25 = GetListPDSDetail(CustOrderNo);

                for (int i = 0; i < dtTop25.Rows.Count; i++)
                {
                    workRow = worksheet.Row(13 + i);
                    string partNo = dtTop25.Rows[i]["F_Part_No"].ToString().Trim().Substring(0, 5)
                        + "-" + dtTop25.Rows[i]["F_Part_No"].ToString().Trim().Substring(5, 5)
                        + "-" + dtTop25.Rows[i]["F_Ruibetsu"].ToString().Trim();

                    workRow.Cell(5).Value = partNo;
                    workRow.Cell(9).Value = dtTop25.Rows[i]["F_Kanban_No"].ToString().Trim();
                    workRow.Cell(10).Value = dtTop25.Rows[i]["F_Part_Name"].ToString().Trim();
                    workRow.Cell(24).Value = dtTop25.Rows[i]["F_Unit_Amount"].ToString().Trim();
                    workRow.Cell(27).Value = dtTop25.Rows[i]["F_Supplier_Code"].ToString().Trim();
                    workRow.Cell(28).Value = dtTop25.Rows[i]["F_Short_Name"].ToString().Trim();
                    workRow.Cell(30).Value = dtTop25.Rows[i]["F_OrderNo"].ToString().Trim();
                }

                if (rowCount > 25)
                {
                    worksheet = workbook.Worksheet("Page2");
                    workRow = worksheet.Row(9);
                    workRow.Cell(27).Value = CustOrderNo;

                    var dtOver25 = GetListPDSDetailOver25(CustOrderNo);

                    for (int i = 0; i < dtOver25.Rows.Count; i++)
                    {
                        workRow = worksheet.Row(13 + i);
                        string partNo = dtOver25.Rows[i]["F_Part_No"].ToString().Trim().Substring(0, 5)
                            + "-" + dtOver25.Rows[i]["F_Part_No"].ToString().Trim().Substring(5, 5)
                            + "-" + dtOver25.Rows[i]["F_Ruibetsu"].ToString().Trim();

                        workRow.Cell(5).Value = partNo;
                        workRow.Cell(9).Value = dtOver25.Rows[i]["F_Kanban_No"].ToString().Trim();
                        workRow.Cell(10).Value = dtOver25.Rows[i]["F_Part_Name"].ToString().Trim();
                        workRow.Cell(24).Value = dtOver25.Rows[i]["F_Unit_Amount"].ToString().Trim();
                        workRow.Cell(27).Value = dtOver25.Rows[i]["F_Supplier_Code"].ToString().Trim();
                        workRow.Cell(28).Value = dtOver25.Rows[i]["F_Short_Name"].ToString().Trim();
                        workRow.Cell(30).Value = dtOver25.Rows[i]["F_OrderNo"].ToString().Trim();
                    }
                }

                string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Report", "SpecialOrdering", "KBNOR297");
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                string fileName = CustOrderNo.Replace(":", "-") + ".xlsx";
                string fullFilePath = Path.Combine(savePath, fileName);

                // Check if the file already exists and delete it
                if (File.Exists(fullFilePath))
                {
                    File.Delete(fullFilePath);
                }

                // Save the workbook
                workbook.SaveAs(fullFilePath);

                // Return the relative path to the file for access in the web application
                return Path.Combine("/Report/SpecialOrdering/KBNOR297/", fileName).Replace("\\", "/");

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    throw;
                }
                else
                {
                    throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
        }

        private DataTable GetRowCount(string? CustOrderNo)
        {
            try
            {
                string _sql = $@"SELECT  Count(*) As F_Row_Count 
                    FROM   VW_SPC_KBNOR297_OutboundRPT 
                    Where F_Part_No <>  '' ";

                if (string.IsNullOrWhiteSpace(CustOrderNo))
                {
                    _sql += $" and F_PO_Customer like '%{CustOrderNo}%' ";
                }

                var dt = _FillDT.ExecuteSQL(_sql);

                return dt;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    throw;
                }
                else
                {
                    throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
        }

        private DataTable GetListPDSDetail(string? CustOrderNo)
        {
            try
            {
                string _sql = $@"SELECT  Top 25 F_OrderNo, F_Part_No, F_Ruibetsu, F_Part_Name, F_Kanban_No,  
                    F_Unit_Amount, F_PO_Customer, F_Supplier_Code, F_Short_Name, F_Delivery_Date 
                    FROM   VW_SPC_KBNOR297_OutboundRPT 
                    Where F_Part_No <>  '' ";

                if (!string.IsNullOrWhiteSpace(CustOrderNo))
                {
                    _sql += $" and F_PO_Customer like '%{CustOrderNo}%' ";
                }

                _sql = _sql + " Order by F_OrderNo,F_Kanban_no ";

                var dt = _FillDT.ExecuteSQL(_sql);

                return dt;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    throw;
                }
                else
                {
                    throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
        }

        public DataTable GetListAllPDSDetail(string? CustOrderNo)
        {
            try
            {
                string _sql = $@"SELECT F_OrderNo, F_Part_No, F_Ruibetsu, F_Part_Name, F_Kanban_No,  
                    F_Unit_Amount, F_PO_Customer, F_Supplier_Code, F_Short_Name, F_Delivery_Date 
                    FROM   VW_SPC_KBNOR297_OutboundRPT
                    Where F_Part_No <>  '' ";

                if (!string.IsNullOrWhiteSpace(CustOrderNo))
                {
                    _sql += $" and F_PO_Customer like '%{CustOrderNo}%' ";
                }

                _sql = _sql + " Order by F_OrderNo,F_Kanban_no ";

                var dt = _FillDT.ExecuteSQL(_sql);

                return dt;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    throw;
                }
                else
                {
                    throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
        }

        private DataTable GetListPDSDetailOver25(string? CustOrderNo)
        {
            try
            {
                string _sql = $@"SELECT F_OrderNo, F_Part_No, F_Ruibetsu, F_Part_Name, F_Kanban_No,  
                    F_Unit_Amount, F_PO_Customer, F_Supplier_Code, F_Short_Name, F_Delivery_Date 
                    FROM   VW_SPC_KBNOR297_OutboundRPT
                    Where F_Part_No <>  '' ";

                if (!string.IsNullOrWhiteSpace(CustOrderNo))
                {
                    _sql += $" and F_PO_Customer like '%{CustOrderNo}%' ";
                    _sql += $" and F_OrderNo not in (Select Top 25 F_orderNo from VW_SPC_KBNOR297_OutboundRPT ";
                    _sql += $" Where  F_PO_Customer like '%{CustOrderNo}%' Order by F_OrderNo,F_Kanban_no ) ";
                }

                _sql = _sql + " Order by F_OrderNo,F_Kanban_no ";

                var dt = _FillDT.ExecuteSQL(_sql);

                return dt;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    throw;
                }
                else
                {
                    throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
        }

        public string GetCustomerPO(string? DeliYM)
        {
            try
            {
                string _sql = $@"SELECT  Rtrim(F_PO_Customer) As F_PO_customer 
                    FROM   VW_SPC_KBNOR297_OutboundRPT 
                    Where F_Delivery_Date <>  '' ";

                if (!string.IsNullOrWhiteSpace(DeliYM))
                {
                    _sql += $" and F_Delivery_Date like '{DeliYM}%' ";
                }
                _sql += " Group by F_PO_Customer Order by F_PO_Customer ";

                var _dt = _FillDT.ExecuteSQL(_sql);
                if (_dt.Rows.Count == 0)
                {
                    throw new CustomHttpException(StatusCodes.Status404NotFound, "Data not found");
                }

                return JsonConvert.SerializeObject(_dt);


            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    throw;
                }
                else
                {
                    throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
        }
    }

}
