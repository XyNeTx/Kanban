using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;

namespace KANBAN.Services.SpecialOrdering.Repo
{
    public class KBNOR280 : IKBNOR280
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly ISpecialLibs _specialLibs;
        private readonly IAutoMapService _automapService;


        public KBNOR280
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            ISpecialLibs specialLibs,
            IAutoMapService autoMapService
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _specialLibs = specialLibs;
            _automapService = autoMapService;
        }



        public string GetPDSData(string FacCD, string DeliYM)
        {
            try
            {
                string _sql = $@"Select RTrim(P.F_Supplier_Code) collate THAI_CI_AS +'-'+LTrim(P.F_Supplier_Plant) As F_Supp_CD, P.F_OrderNo, 
                    Convert(varchar(11),Cast(P.F_Delivery_Date As datetime),103) As F_Delivery_Date,
                    P.F_Delivery_Cycle As F_Delivery_Cycle ,P.F_Delivery_Trip 
                    From  TB_PDS_Header P inner join TB_Survey_Detail D 
                    On P.F_OrderNO collate THAI_CI_AS = D.F_PDS_No 
                    Where P.F_OrderNo like '{FacCD}%' 
                    and P.F_Delivery_Date like '{DeliYM}%' 
                    Group by P.F_Supplier_Code,P.F_Supplier_Plant,P.F_OrderNo, 
                    P.F_Delivery_Date,P.F_Delivery_Cycle,P.F_Delivery_Trip 
                    Order by P.F_OrderNO ";

                DataTable dt = _FillDT.ExecuteSQL(_sql);

                return JsonConvert.SerializeObject(dt);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private void ChkSurveyDoc()
        {
            try
            {
                string _sql = $@"Select F_Status From TB_Survey_Header 
                    Where F_Factory_code  in ('{_BearerClass.Plant}') 
                    and F_Status not in ('N','D') and F_PDS_Flg = '0' ";

                DataTable dt = _FillDT.ExecuteSQL(_sql);

                if (dt.Rows.Count <= 0)
                {
                    throw new CustomHttpException(StatusCodes.Status404NotFound, "Not found survey data to generate pds.");
                }

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public async Task Register(List<VM_Register_KBNOR280> listObj)
        {
            using var transaction = _kbContext.Database.BeginTransaction();
            try
            {
                await transaction.CreateSavepointAsync("Savepoint_Register");
                foreach (var obj in listObj)
                {
                    string PDSNO = obj.F_OrderNo;
                    string Supplier = obj.F_Supp_CD;
                    string DeliDate = obj.F_Delivery_Date;
                    string trip = obj.F_Delivery_Trip.ToString();
                    string Supp_CD = Supplier.Split("-")[0];
                    string Supp_Plant = Supplier.Split("-")[1];

                    var listIns = _kbContext.TB_PDS_Header
                        .Where(x => x.F_OrderNo == PDSNO
                        && x.F_Delivery_Date == DateTime.ParseExact(DeliDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd")
                        && x.F_Supplier_Code == Supp_CD && x.F_Supplier_Plant == Supp_Plant
                        && x.F_Delivery_Trip == int.Parse(trip)).ToList();

                    if (listIns.Count <= 0)
                    {
                        throw new CustomHttpException(StatusCodes.Status404NotFound, "Not found data to register.");
                    }

                    foreach (var ins in listIns)
                    {
                        //var objRecHead = JsonConvert.DeserializeObject<TB_REC_HEADER>(JsonConvert.SerializeObject(ins));
                        var mappingService = _automapService.GetAutoMapRepo<TB_PDS_Header, TB_REC_HEADER>();
                        var objRecHead = mappingService.MapTo(ins);
                        if (objRecHead.F_Transportor == null)
                        {
                            objRecHead.F_Transportor = "";
                        }
                        objRecHead.F_Plant = _BearerClass.Plant[0];
                        objRecHead.F_Flg_Epro = '9';
                        objRecHead.F_Status = 'C';
                        objRecHead.F_Approver = " ";
                        objRecHead.F_Cancel_By = " ";
                        objRecHead.F_Cancel_Date = DateTime.Now;
                        objRecHead.F_Delay_Invoice_Date = " ";
                        objRecHead.F_Type_Version = " ";
                        objRecHead.F_Flag_Gen_WDS = false;
                        objRecHead.F_Flag_Transfer = true;

                        _kbContext.TB_REC_HEADER.Add(objRecHead);
                        _log.WriteLogMsg("Insert to TB_REC_Header  : " + JsonConvert.SerializeObject(objRecHead));
                    }


                    var listInsDetail = _kbContext.TB_PDS_Detail
                        .Where(x => x.F_OrderNo == PDSNO).ToList();

                    foreach (var insDetail in listInsDetail)
                    {
                        var mappingService = _automapService.GetAutoMapRepo<TB_PDS_Detail, TB_REC_DETAIL>();
                        var objRecDetail = mappingService.MapTo(insDetail);
                        objRecDetail.F_Receive_Date = DateTime.Now;

                        _kbContext.TB_REC_DETAIL.Add(objRecDetail);
                        _log.WriteLogMsg("Insert to TB_REC_Detail  : " + JsonConvert.SerializeObject(objRecDetail));
                    }

                    await _kbContext.TB_Survey_Detail
                        .Where(x => x.F_PDS_No == PDSNO &&
                        x.F_Delivery_Date == DateTime.ParseExact(DeliDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd"))
                        .ExecuteUpdateAsync(setter => setter.SetProperty(x => x.F_PDS_Flg, 2));

                    await _kbContext.TB_PDS_Detail.Where(x => x.F_OrderNo == PDSNO).ExecuteDeleteAsync();
                    _log.WriteLogMsg("Delete from TB_Survey_Detail  : " + PDSNO);

                    await _kbContext.TB_PDS_Header.Where(x => x.F_OrderNo == PDSNO
                        && x.F_Delivery_Date == DateTime.ParseExact(DeliDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd")
                        && x.F_Supplier_Code == Supp_CD && x.F_Supplier_Plant == Supp_Plant
                        && x.F_Delivery_Trip == int.Parse(trip)).ExecuteDeleteAsync();

                    _log.WriteLogMsg("Delete from TB_Survey_Header  : " + PDSNO);

                }

                await _kbContext.SaveChangesAsync();
                //await _kbContext.Database.ExecuteSqlRawAsync($"Update TB_REC_HEADER Set F_Cancel_Date = '' Where F_Cancel_Date = '{DateTime.Today}'");
                //await _kbContext.Database.ExecuteSqlRawAsync($"Update TB_REC_DETAIL Set F_Receive_Date = '' Where F_Receive_Date = '{DateTime.Today}'");

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                await transaction.RollbackToSavepointAsync("Savepoint_Register");
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //-------------------------------- Modal ----------------------------------

        public async Task<List<TB_REC_HEADER>> GetSupplier()
        {
            try
            {
                string FacCD = _BearerClass.Plant switch
                {
                    "1" => "9Z",
                    "2" => "8Z",
                    "3" => "7Z",
                    _ => "9Z"
                };

                var data = await _kbContext.TB_REC_HEADER
                    .Where(x => x.F_OrderType == 'S'
                    && x.F_Plant == _BearerClass.Plant[0]
                    && x.F_OrderNo.StartsWith(FacCD))
                    .ToListAsync();

                if (data == null || data.Count == 0)
                {
                    throw new CustomHttpException(StatusCodes.Status404NotFound, "Supplier Code not found.");
                }

                return data.DistinctBy(x => new
                {
                    x.F_Supplier_Code,
                    x.F_Supplier_Plant
                }).ToList();
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public async Task<List<TB_REC_HEADER>> GetPO(string YM)
        {
            try
            {
                string FacCD = _BearerClass.Plant switch
                {
                    "1" => "9Z",
                    "2" => "8Z",
                    "3" => "7Z",
                    _ => "9Z"
                };

                var data = await _kbContext.TB_REC_HEADER
                    .Where(x => x.F_OrderType == 'S'
                    && x.F_Plant == _BearerClass.Plant[0]
                    && x.F_OrderNo.StartsWith(FacCD)
                    && x.F_Issued_Date!.Value.Year == int.Parse(YM.Substring(0, 4))
                    && x.F_Issued_Date!.Value.Month == int.Parse(YM.Substring(4, 2))
                    && !(x.F_Status == 'C' || x.F_Status == 'W'))
                    .OrderBy(x => x.F_PO_Customer)
                    .ToListAsync();

                if (data == null || data.Count == 0)
                {
                    throw new CustomHttpException(StatusCodes.Status404NotFound, "PO No. not found.");
                }

                return data.DistinctBy(x => new
                {
                    x.F_PO_Customer
                }).ToList();

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public async Task<List<TB_REC_HEADER>> GetPDS(string? POFrom, string? POTo)
        {
            try
            {
                string FacCD = _BearerClass.Plant switch
                {
                    "1" => "9Z",
                    "2" => "8Z",
                    "3" => "7Z",
                    _ => "9Z"
                };

                var data = await _kbContext.TB_REC_HEADER
                    .Where(x => x.F_OrderType == 'S'
                    && x.F_Plant == _BearerClass.Plant[0]
                    && x.F_OrderNo.StartsWith(FacCD))
                    .OrderBy(x => x.F_PO_Customer)
                    .ToListAsync();

                if (!string.IsNullOrEmpty(POFrom) && !string.IsNullOrEmpty(POTo))
                {
                    data = data.Where(x => x.F_PO_Customer.CompareTo(POFrom) >= 0 && x.F_PO_Customer.CompareTo(POTo) <= 0).ToList();
                }

                if (data == null || data.Count == 0)
                {
                    throw new CustomHttpException(StatusCodes.Status404NotFound, "PDS Not found.");
                }

                return data.DistinctBy(x => new
                {
                    x.F_OrderNo
                }).ToList();

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public string ExportData(string? PONoFrom, string? PONoTo, string? PDSNoFrom, string? PDSNoTo, string? SupplierFrom, string? SupplierTo,
                                    string? DeliveryFrom, string? DeliveryTo)
        {
            try
            {
                string _sql = $@"SELECT H.F_OrderNo, H.F_PO_Customer, H.F_Plant, H.F_OrderType, '0'+H.F_Supplier_Code As F_Supplier_Code , H.F_Supplier_Plant, Convert(varchar(11),CAST(H.F_Delivery_Date As Datetime),103) As F_Delivery_Date, H.F_Delivery_Trip, '('+H.F_Delivery_Time+')' As F_Delivery_Time, 
                    Convert(varchar(11),F_Issued_Date,103) As F_Issued_Date,H.F_Delivery_Cycle, H.F_Delivery_Dock, H.F_Dock_Code, D.F_No,D.F_Part_No, D.F_Ruibetsu, Replace(D.F_Part_Name,',',' ') As F_Part_Name, D.F_Kanban_No, 
                    D.F_Box_Qty, D.F_Unit_Amount, H.F_Remark, H.F_Remark_KB,H.F_Vat,S.F_short_name, replace(S.F_name,',',' ') As F_name,'{_BearerClass.UserCode}' As F_Update_By, 
                    H.F_Remark2,H.F_Remark3,D.F_Address,Left(F_Approver,8) As F_Approver,H.F_Transportor,H.F_Collect_Date,H.F_Collect_Time, 
                    H.F_Type_Version,H.F_OrderNo_Old,H.F_Dept, H.F_DR,H.F_WK_Code 
                    FROM  TB_REC_HEADER AS H INNER JOIN TB_REC_DETAIL AS D ON H.F_OrderNo = D.F_OrderNo 
                    INNER JOIN V_Supplier_ms AS S ON H.F_Supplier_Code COLLATE Thai_CS_AI = S.F_supplier_cd 
                    AND H.F_Supplier_Plant COLLATE Thai_CS_AI = S.F_Plant_cd AND H.F_Delivery_Dock COLLATE Thai_CS_AI = S.F_Store_cd 
                    WHERE  S.F_TC_Str COLLATE Thai_CS_AI <=H.F_Delivery_Date AND  S.F_TC_End  COLLATE Thai_CS_AI >=H.F_Delivery_Date 
                    AND H.F_OrderType = 'S' 
                    AND H.F_Plant in ('{_BearerClass.Plant}') ";

                if (!string.IsNullOrWhiteSpace(PONoFrom) && !string.IsNullOrWhiteSpace(PONoFrom))
                {
                    _sql += $"AND (H.F_PO_Customer between '{PONoFrom}' and '{PONoTo}' )";
                }
                if (!string.IsNullOrWhiteSpace(PDSNoFrom) && !string.IsNullOrWhiteSpace(PDSNoTo))
                {
                    _sql += $"AND (H.F_OrderNo between '{PDSNoFrom}' and '{PDSNoTo}' )";
                }
                if (!string.IsNullOrWhiteSpace(SupplierFrom) && !string.IsNullOrWhiteSpace(SupplierTo))
                {
                    _sql += $"AND (H.F_Supplier_Code + '-' + H.F_Supplier_Plant between '{SupplierFrom}' and '{SupplierTo}' )";
                }
                if (!string.IsNullOrWhiteSpace(DeliveryFrom) && !string.IsNullOrWhiteSpace(DeliveryTo))
                {
                    _sql += $"AND (H.F_Delivery_Date between '{DeliveryFrom}' and '{DeliveryTo}' )";
                }

                _sql += "Order by H.F_OrderNo,D.F_No ";

                DataTable dt = _FillDT.ExecuteSQL(_sql);

                return JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.InnerException.Message ?? ex.Message);
            }
        }

    }
}
