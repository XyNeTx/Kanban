using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Master;
using KANBAN.Models.KB3.VLT;
using KANBAN.Services.Import.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Services.Import.Repository
{
    public class KBNIM0044 : IKBNIM0044
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;


        public KBNIM0044
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
        }

        public async Task<int> SaveImportData(List<VM_KBNIM0044> listData)
        {
            try
            {
                using var kbTrans = await _kbContext.Database.BeginTransactionAsync();
                var isInLineControl = await _kbContext.TB_MS_LineControl
                        .AsNoTracking()
                        .Select(x => x.F_Line_ID)
                        .ToListAsync();

                if (isInLineControl.Count == 0)
                {
                    throw new CustomHttpException(404, "Line Control is empty");
                }

                listData = listData.Where(x => isInLineControl.Any(y => x.F_LineCode.StartsWith(y))).ToList();

                if (listData.Count == 0)
                {
                    throw new CustomHttpException(404, "Please check Line Code, 0 record to Import");
                }

                int intRecords = 0;

                foreach (var data in listData)
                {
                    var getParentPart = await _kbContext.TB_MS_PartCode
                        .FirstOrDefaultAsync(x => x.F_Code == data.F_PartCode);

                    if (getParentPart == null) throw new CustomHttpException(404, "Cant get parent part");

                    var vltData = new TB_Import_VHD
                    {
                        F_Cust_Seq = data.F_Customer + "_" + data.F_Seq,
                        F_Customer = data.F_Customer,
                        F_Date = data.F_Date,
                        F_Line_ID = data.F_LineCode.Trim(),
                        F_Seq = data.F_Seq,
                        F_PartCode = data.F_PartCode,
                        F_Parent_Part = getParentPart!.F_Part_No + "-" + getParentPart.F_Ruibetsu,
                        F_Update_By = _BearerClass.UserCode,
                        F_Update_Date = DateTime.Now
                    };

                    var isExist = await _kbContext.TB_Import_VHD
                        .FirstOrDefaultAsync(x => x.F_Cust_Seq == vltData.F_Cust_Seq);

                    if (isExist != null) throw new CustomHttpException(400, $"Data already exist at Seq {vltData.F_Seq} | Customer {vltData.F_Customer} | Part Code {vltData.F_PartCode}");

                    var test = await _kbContext.TB_Import_VHD.AddAsync(vltData);
                    _log.WriteLogMsg("INSERT TB_Import_VHD => " + JsonConvert.SerializeObject(vltData));
                }
                intRecords = await _kbContext.SaveChangesAsync();
                await kbTrans.CommitAsync();

                return intRecords;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<TB_Import_VHD>> GetDataList(bool isAll = false)
        {
            try
            {
                var data = await _kbContext.TB_Import_VHD
                    .AsNoTracking()
                    .Where(x => string.IsNullOrWhiteSpace(x.F_Flag) || x.F_Flag == "S")
                    .ToListAsync();

                if (!isAll)
                {
                    data = await _kbContext.TB_Import_VHD
                        .AsNoTracking()
                        .Where(x => x.F_Flag == "P")
                        .ToListAsync();
                }

                if (data.Count == 0) throw new CustomHttpException(404, "Data not found");


                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task UpdateFlag(List<TB_Import_VHD> listData, string shift)
        {
            try
            {
                var isConfirmed = await _kbContext.TB_Import_VHD
                    .AsNoTracking()
                    .AnyAsync(x => x.F_Flag == "C"
                    && x.F_Deli_Shift == listData[0].F_Deli_Shift
                    && x.F_Deli_Date == listData[0].F_Deli_Date);

                int sumDeliveryTrip = 0;

                if (isConfirmed) throw new CustomHttpException(400, "This Date and Shift already confirmed");

                foreach (var data in listData)
                {
                    var vltData = await _kbContext.TB_Import_VHD
                        .FirstOrDefaultAsync(x => x.F_Cust_Seq == data.F_Cust_Seq);

                    _log.WriteLogMsg("UPDATE TB_Import_VHD BEFORE UPDATE => " + JsonConvert.SerializeObject(vltData));
                    int maxTrip = 0;

                    if (shift.ToLower() == "night")
                    {
                        var dbDeliveryTime = await _kbContext.TB_MS_DeliveryTime.AsNoTracking()
                            .Where(x => (x.F_Delivery_Time.CompareTo("07:29") <= 0 || x.F_Delivery_Time.CompareTo("19:30") >= 0)
                            && x.F_Supplier_Code == "2937"
                            && x.F_Supplier_Plant == "Z")
                            .OrderBy(x => x.F_Delivery_Trip)
                            .FirstOrDefaultAsync();

                        if (dbDeliveryTime != null)
                        {
                            sumDeliveryTrip = dbDeliveryTime.F_Delivery_Trip - 1;
                            maxTrip = int.Parse(dbDeliveryTime.F_Cycle.Substring(2, 2));
                        }

                    }

                    if (vltData == null) throw new CustomHttpException(404, "Data not found");

                    vltData.F_Flag = data.F_Flag;
                    vltData.F_Deli_Date = data.F_Deli_Date;
                    vltData.F_Deli_Shift = data.F_Deli_Shift;
                    vltData.F_Deli_Trip = data.F_Deli_Trip + sumDeliveryTrip;
                    vltData.F_Update_By = _BearerClass.UserCode;
                    vltData.F_Update_Date = DateTime.Now;


                    if (vltData.F_Deli_Trip > maxTrip)
                    {
                        throw new CustomHttpException(400, "Trip > cycle time Error!!");
                    }


                }
                await _kbContext.SaveChangesAsync();
                _log.WriteLogMsg("UPDATE TB_Import_VHD AFTER UPDATE => " + JsonConvert.SerializeObject(listData, Formatting.Indented));
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Confirm(List<TB_Import_VHD> listData)
        {
            try
            {
                await _kbContext.Database.ExecuteSqlRawAsync($@"exec [exec].[spKBNIM044_Confirm] @sPlant,@User",
                    new SqlParameter("@sPlant", _BearerClass.Plant),
                    new SqlParameter("@User", _BearerClass.UserCode));

                _log.WriteLogMsg($@"exec [exec].[spKBNIM044_Confirm] '{_BearerClass.Plant}' , '{_BearerClass.UserCode}' ");

                //var simDataList = await _kbContext.TB_Import_VHD
                //    .Where(x => x.F_Flag == "S" && x.F_Update_By == _BearerClass.UserCode)
                //    .ToListAsync();

                string LogMsg = "";

                foreach (var sim in listData)
                {
                    sim.F_Flag = "C";
                    sim.F_Update_By = _BearerClass.UserCode;
                    sim.F_Update_Date = DateTime.Now;

                    //_kbContext.TB_Import_VHD.Update(sim);

                    //LogMsg += Environment.NewLine + "Confirming IMPORT VHD Data => " + JsonConvert.SerializeObject(sim);
                }

                _kbContext.TB_Import_VHD.UpdateRange(listData);
                await _kbContext.SaveChangesAsync();
                LogMsg = "Confirm IMPORT VHD Data => " + JsonConvert.SerializeObject(listData, Formatting.Indented);

                _log.WriteLogMsg(LogMsg);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}
