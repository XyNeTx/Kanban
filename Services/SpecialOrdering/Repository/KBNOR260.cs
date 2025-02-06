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

namespace KANBAN.Services.SpecialOrdering.Repository
{
    public class KBNOR260 : IKBNOR260
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly ISpecialLibs _specialLibs;
        private readonly IAutoMapService _automapService;


        public KBNOR260
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

        public string GetApproverList()
        {
            try
            {
                string _sql = $@"Select Rtrim(F_User_ID)+':'+F_Name AS F_Name 
                    From TB_MS_SpcApprover Order by F_User_ID,F_Name ";

                var dt = _FillDT.ExecuteSQL(_sql);

                return JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<TB_REC_HEADER>> GetPDSDataNoApprove(string? fac)
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
                    .Where(x => !(x.F_Status == 'N' || x.F_Status == 'D' || x.F_Status == 'W' || x.F_Status == 'P')
                    && !string.IsNullOrWhiteSpace(x.F_OrderNo) && x.F_OrderNo.StartsWith(FacCD)).ToListAsync();

                if (!string.IsNullOrWhiteSpace(fac))
                {
                    data = data.Where(x => x.F_Plant == fac[0]).ToList();
                }

                data = data.DistinctBy(x => new
                {
                    x.F_OrderNo,
                    x.F_PO_Customer,
                    x.F_Delivery_Date,
                    x.F_Supplier_Code,
                    x.F_Supplier_Plant,
                    x.F_Status
                }).OrderBy(x => x.F_OrderNo).ToList();

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task SendApprove(List<VM_TB_Rec_Header> listObj)
        {
            try
            {
                string sPDS = "";
                string sql = "";
                string FacCD = _BearerClass.Plant switch
                {
                    "1" => "9Z",
                    "2" => "8Z",
                    "3" => "7Z",
                    _ => "9Z"
                };

                List<string> listOrder = new List<string>();

                foreach (var obj in listObj)
                {
                    await _kbContext.TB_REC_HEADER
                        .Where(x => x.F_OrderNo == obj.F_OrderNo
                        && (x.F_Status == 'C' || x.F_Status == 'W')
                        && x.F_Flg_Epro == '9'
                        && x.F_Supplier_Code.Trim() + "-" + x.F_Supplier_Plant == obj.F_Supp_CD)
                        .ExecuteUpdateAsync(x => x.SetProperty(y => y.F_Status, 'W')
                        .SetProperty(y => y.F_Approver, obj.F_Approver)
                        .SetProperty(y => y.F_Flg_Epro, '9'));

                    _log.WriteLogMsg("Update to TB_REC_Header Set F_Status = 'W' Where F_OrderNo = " + obj.F_OrderNo + " and F_Supplier_Code = " + obj.F_Supp_CD);

                    await _kbContext.Database.ExecuteSqlRawAsync($"Delete From TB_PDS_Approve Where F_OrderNo = '{obj.F_OrderNo}'");

                    sql = $@"Insert into TB_PDS_Approve (F_OrderNo, F_Approver, F_Send_Date, F_RecUser) 
                        Values ('{obj.F_OrderNo}', '{obj.F_Approver}', GetDate(), '{_BearerClass.UserCode}')";

                    await _kbContext.Database.ExecuteSqlRawAsync(sql);

                    if (!listOrder.Contains(obj.F_OrderNo))
                    {
                        listOrder.Add(obj.F_OrderNo);
                        sPDS = sPDS + obj.F_OrderNo + "<br>";
                    }

                    await _kbContext.SaveChangesAsync();
                }

                await _emailService.SendEmailApprover(sPDS, listObj[0].F_Approver.Split(":")[0]);
                await _kbContext.Database.ExecuteSqlRawAsync($"Update TB_PDS_Approve Set F_Send_Mail_Flag = '1' " +
                    $"  Where F_Send_Mail_Flag = '0' and F_Approver like '{_BearerClass.UserCode}%'");

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<TB_REC_HEADER>> GetPDSWaitApprove(string? fac)
        {
            try
            {
                var data = await _kbContext.TB_REC_HEADER
                    .Where(x => x.F_Status == 'W').ToListAsync();

                if (!string.IsNullOrWhiteSpace(fac))
                {
                    data = data.Where(x => x.F_Plant == fac[0]).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }
    }
}