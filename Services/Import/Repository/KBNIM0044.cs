using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Master;
using KANBAN.Models.KB3.VLT;
using KANBAN.Services.Import.Interface;
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

        public async Task SaveImportData(List<VM_KBNIM0044> listData)
        {
            try
            {

                var isInLineControl = await _kbContext.TB_MS_LineControl
                        .AsNoTracking()
                        .Select(x => x.F_Line_ID)
                        .ToListAsync();

                if (isInLineControl.Count == 0)
                {
                    throw new CustomHttpException(404, "Line Control is empty");
                }

                listData = listData.Where(x => isInLineControl.Any(y => x.F_LineCode.StartsWith(y))).ToList();

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

                    await _kbContext.TB_Import_VHD.AddAsync(vltData);
                    _log.WriteLogMsg("INSERT TB_Import_VHD => " + JsonConvert.SerializeObject(vltData));
                    await _kbContext.SaveChangesAsync();
                }
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
                    .Where(x => x.F_Update_By == _BearerClass.UserCode
                    && string.IsNullOrWhiteSpace(x.F_Flag) || x.F_Flag == "S")
                    .ToListAsync();

                if (data.Count == 0) throw new CustomHttpException(404, "Data not found");

                if (!isAll)
                {
                    data = await _kbContext.TB_Import_VHD
                        .AsNoTracking()
                        .Where(x => x.F_Update_By == _BearerClass.UserCode
                        && x.F_Flag == "P")
                        .ToListAsync();
                }

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task UpdateFlag(List<TB_Import_VHD> listData)
        {
            try
            {
                var isConfirmed = await _kbContext.TB_Import_VHD
                    .AsNoTracking()
                    .AnyAsync(x => x.F_Update_By == _BearerClass.UserCode
                    && x.F_Flag == "C"
                    && x.F_Deli_Shift == listData[0].F_Deli_Shift
                    && x.F_Deli_Date == listData[0].F_Deli_Date);

                if (isConfirmed) throw new CustomHttpException(400, "This Date and Shift already confirmed");

                foreach (var data in listData)
                {
                    var vltData = await _kbContext.TB_Import_VHD
                        .FirstOrDefaultAsync(x => x.F_Cust_Seq == data.F_Cust_Seq);

                    if (vltData == null) throw new CustomHttpException(404, "Data not found");

                    vltData.F_Flag = data.F_Flag;
                    vltData.F_Deli_Date = data.F_Deli_Date;
                    vltData.F_Deli_Shift = data.F_Deli_Shift;
                    vltData.F_Deli_Trip = data.F_Deli_Trip;
                    vltData.F_Update_By = _BearerClass.UserCode;
                    vltData.F_Update_Date = DateTime.Now;

                    _log.WriteLogMsg("UPDATE TB_Import_VHD => " + JsonConvert.SerializeObject(vltData));
                    await _kbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                else throw new CustomHttpException(500, ex.Message);
            }
        }

    }
}
