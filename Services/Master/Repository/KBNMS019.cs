using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.PPM3;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Master.IRepository;
using Microsoft.EntityFrameworkCore;

namespace KANBAN.Services.Master.Repository
{
    public class KBNMS019 : IKBNMS019
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _autoMap;


        public KBNMS019
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            IAutoMapService autoMap
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _autoMap = autoMap;
        }

        private readonly string strDateNow = DateTime.Now.ToString("yyyyMMdd");

        public async Task<List<T_Supplier_MS>> GetSupplierNew()
        {
            try
            {
                var data = await _PPM3Context.T_Supplier_MS
                    .AsNoTracking()
                    .ToListAsync();

                return data.DistinctBy(x => new
                {
                    F_Supplier_Code = x.F_supplier_cd + "-" + x.F_Plant_cd
                }).OrderBy(x => x.F_supplier_cd)
                    .ThenBy(x => x.F_Plant_cd)
                    .ToList();
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task<List<TB_MS_MAXAREA>> GetSupplierInq()
        {
            try
            {
                var data = await _kbContext.TB_MS_MAXAREA
                    .AsNoTracking()
                    .Where(x => x.F_Plant == _BearerClass.Plant)
                    .ToListAsync();

                return data.DistinctBy(x => new
                {
                    F_Supplier_Code = x.F_Supplier_Code + "-" + x.F_Supplier_Plant
                }).OrderBy(x => x.F_Supplier_Code)
                    .ThenBy(x => x.F_Supplier_Plant)
                    .ToList();

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task<TB_MS_MAXAREA> GetSupplierDetail(string SupplierCode)
        {
            try
            {
                var data = await _kbContext.TB_MS_MAXAREA
                    .AsNoTracking().FirstOrDefaultAsync(
                    x => x.F_Supplier_Code + "-" + x.F_Supplier_Plant == SupplierCode
                    && x.F_Part_No == "" && x.F_Ruibetsu == ""
                    && x.F_Kanban_No == "" && x.F_Store_CD == "");

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task<List<T_Construction>> GetPartNew()
        {
            try
            {
                var data = await _PPM3Context.T_Construction
                    .AsNoTracking()
                    .Where(x => x.F_Local_Str.CompareTo(strDateNow) <= 0
                    && x.F_Local_End.CompareTo(strDateNow) >= 0
                    && x.F_Store_cd.StartsWith(_BearerClass.Plant)).ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task<List<TB_MS_MAXAREA>> GetPartInq(string SupplierCode)
        {
            try
            {
                var data = await _kbContext.TB_MS_MAXAREA
                    .AsNoTracking()
                    .Where(x => x.F_Plant == _BearerClass.Plant
                    && x.F_Supplier_Code + "-" + x.F_Supplier_Plant == SupplierCode)
                    .ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task<T_Construction> GetPartName(string PartNo)
        {
            try
            {
                var data = await _PPM3Context.T_Construction
                    .AsNoTracking().FirstOrDefaultAsync(
                    x => x.F_Part_no.Trim() + "-" + x.F_Ruibetsu == PartNo);

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

        public async Task<TB_MS_MAXAREA> GetMaxTrip(string SupplierCode, string PartNo, string StoreCode, string KanbanNo)
        {
            try
            {
                var data = await _kbContext.TB_MS_MAXAREA.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.F_Plant == _BearerClass.Plant
                    && x.F_Supplier_Code + "-" + x.F_Supplier_Plant == SupplierCode
                    && x.F_Part_No + "-" + x.F_Ruibetsu == PartNo
                    && x.F_Kanban_No == KanbanNo && x.F_Store_CD == StoreCode);

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, null, ex);
            }
        }

    }
}
