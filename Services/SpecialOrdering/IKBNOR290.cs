using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using Microsoft.EntityFrameworkCore;

namespace KANBAN.Services.SpecialOrdering
{
    public interface IKBNOR290
    {
        Task<List<TB_Survey_Header>> ProdYMChanged(string ProdYM);
        Task<List<TB_Survey_Header>> GetSuppCD(string? PO);
    }
    public class KBNOR290 : IKBNOR290
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly ISpecialLibs _specialLibs;


        public KBNOR290
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

        public async Task<List<TB_Survey_Header>> ProdYMChanged(string ProdYM)
        {
            try
            {
                var data = await _kbContext.TB_Survey_Header
                    .Where(x => !string.IsNullOrWhiteSpace(x.F_Survey_Doc)
                    && x.F_Issued_Date.StartsWith(ProdYM))
                    .Select(x => new TB_Survey_Header
                    {
                        F_PO_Customer = x.F_PO_Customer
                    })
                    .ToListAsync();

                data = data.DistinctBy(x=>x.F_PO_Customer).OrderBy(x => x.F_PO_Customer).ToList();

                if (data.Count == 0) throw new CustomHttpException(404, "Data Not Found");


                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<TB_Survey_Header>> GetSuppCD(string? PO)
        {
            try
            {
                var data = await _kbContext.TB_Survey_Header
                    .Where(h => !string.IsNullOrWhiteSpace(h.F_Survey_Doc))
                    .Join(_kbContext.TB_Survey_Detail
                    .Select(d => new
                    {
                        F_survey_Doc = d.F_Survey_Doc,
                        CntSurvey = d.F_Survey_Doc.Count()
                    }),
                    h => h.F_Survey_Doc, d => d.F_survey_Doc, (h, d) => h)
                    .Select(h => new TB_Survey_Header
                    {
                        F_PO_Customer = h.F_PO_Customer,
                        F_Supplier_CD = h.F_Supplier_CD.Trim() + "-" + h.F_Supplier_Plant.Trim()
                    })
                    .ToListAsync();

                if(!string.IsNullOrWhiteSpace(PO))
                {
                    data = data.Where(x => x.F_PO_Customer.Trim() == PO).ToList();
                }

                if (data.Count == 0) throw new CustomHttpException(404, "Data Not Found");

                return data.DistinctBy(x => x.F_Supplier_CD).OrderBy(x => x.F_Supplier_CD).ToList();

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

    }
}
