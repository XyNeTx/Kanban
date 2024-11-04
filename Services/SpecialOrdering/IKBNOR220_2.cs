using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Master;
using KANBAN.Models.KB3.SpecialOrdering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Services.SpecialOrdering
{
    public interface IKBNOR220_2
    {
        Task<TB_Calendar> GetCalendar(string YM);
        Task<List<TB_Survey_Detail>> GetPOList();
        string GetSurvey(string YM);
        string GetSuppCD(string PO, string YM);
        Task<List<TB_Survey_Detail>> GetPartNo(string PO, string SuppCD);
        Task<int> PartNoSelected(string surveyDoc, string suppCD, string partNo);
    }

    public class KBNOR220_2 : IKBNOR220_2
    {

        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly ISpecialLibs _specialLibs;


        public KBNOR220_2
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

        public async Task<TB_Calendar> GetCalendar(string YM)
        {
            try
            {
                var data = await _kbContext.TB_Calendar.Where(x=> x.F_Store_cd == "1F"
                    && x.F_YM == YM).FirstOrDefaultAsync();

                if (data == null)
                {
                    throw new CustomHttpException(StatusCodes.Status404NotFound, "Data Not Found");
                }

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    throw new CustomHttpException((ex as CustomHttpException).StatusCode, ex.Message);
                }
                else
                {
                    throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
        }

        public async Task<List<TB_Survey_Detail>> GetPOList()
        {
            try
            {
                var data = await _kbContext.TB_Survey_Header
                    .Where(h => !string.IsNullOrWhiteSpace(h.F_Survey_Doc)
                    && (h.F_Status == "N" || h.F_Status == "M"))
                    .Join(_kbContext.TB_Survey_Detail.Where(d=>d.F_PDS_No == ""),
                    h => new { h.F_Survey_Doc, h.F_Revise_Rev },
                    d => new { d.F_Survey_Doc, d.F_Revise_Rev },
                    (h, d) => new { h, d })
                    .Select(x => new TB_Survey_Detail
                    {
                        F_PO_Customer = x.d.F_PO_Customer.Trim()
                    }).OrderBy(x=>x.F_PO_Customer).ToListAsync();

                if (data.Count == 0)
                {
                    throw new CustomHttpException(StatusCodes.Status404NotFound, "Data Not Found");
                }

                return data.DistinctBy(x => x.F_PO_Customer).ToList();

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException)
                {
                    throw new CustomHttpException((ex as CustomHttpException).StatusCode, ex.Message);
                }
                else
                {
                    throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
        }

        public string GetSurvey(string YM)
        {
            try
            {
                var data = _specialLibs.GetPOSurvey("", _BearerClass.Plant, "0", YM, "survey");

                if (data == null)
                {
                    throw new CustomHttpException(StatusCodes.Status404NotFound, "Data Not Found");
                }

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public string GetSuppCD(string PO,string YM)
        {
            try
            {
                var data = _specialLibs.GetPOSurvey(PO, _BearerClass.Plant, "0", YM, "SuppCD");

                if (data == null)
                {
                    throw new CustomHttpException(StatusCodes.Status404NotFound, "Data Not Found");
                }

                return data;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public async Task<List<TB_Survey_Detail>> GetPartNo(string PO,string SuppCD)
        {
            try
            {
                var data = await _kbContext.TB_Survey_Header.AsNoTracking()
                    .Where(h=>h.F_Supplier_CD.Trim() + "-" + h.F_Supplier_Plant.Trim() == SuppCD)
                    .Join(_kbContext.TB_Survey_Detail.AsNoTracking().Where(d => string.IsNullOrWhiteSpace(d.F_PDS_No)
                    && d.F_Survey_Doc.Trim() == PO),
                    h => new { h.F_Survey_Doc, h.F_Revise_Rev },
                    d => new { d.F_Survey_Doc, d.F_Revise_Rev },
                    (h, d) => new { h, d })
                    .Select(x => new TB_Survey_Detail
                    {
                        F_Part_No = x.d.F_Part_No.Trim() + "-" + x.d.F_Ruibetsu.Trim()
                    }).AsNoTracking().ToListAsync();

                return data.DistinctBy(x => x.F_Part_No).ToList();
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public async Task<int> PartNoSelected(string surveyDoc, string suppCD,string partNo)
        {
            try
            {
                var data = await _kbContext.TB_Survey_Header.AsNoTracking()
                    .Where(h => h.F_Survey_Doc.Trim() == surveyDoc
                    && h.F_Supplier_CD.Trim() + "-" + h.F_Supplier_Plant.Trim() == suppCD)
                    .Join(_kbContext.TB_Survey_Detail.AsNoTracking()
                    .Where(d => d.F_Part_No.Trim() + "-" + d.F_Ruibetsu.Trim() == partNo),
                    h => new { h.F_Survey_Doc, h.F_Revise_Rev },
                    d => new { d.F_Survey_Doc, d.F_Revise_Rev },
                    (h, d) => new { h, d })
                    .AsNoTracking().GroupBy(x => x.h.F_Survey_Doc)
                    .Select(x => new TB_Survey_Detail
                    {
                        F_Qty = x.Sum(y => y.d.F_Qty)
                    }).AsNoTracking().FirstOrDefaultAsync();

                if (data == null)
                {
                    throw new CustomHttpException(StatusCodes.Status404NotFound, "Data Not Found");
                }

                return data.F_Qty;
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //public async Task<string> GetCalendarQty(string SurveyDoc)
        //{
        //    try
        //    {
        //        var data = await _kbContext.TB_Calendar.Where(x => x.F_Store_cd == "1F"
        //                           && x.F_YM == YM).FirstOrDefaultAsync();

        //        if (data == null)
        //        {
        //            throw new CustomHttpException(StatusCodes.Status404NotFound, "Data Not Found");
        //        }

        //        return data.F_Qty;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex is CustomHttpException)
        //        {
        //            throw new CustomHttpException((ex as CustomHttpException).StatusCode, ex.Message);
        //        }
        //        else
        //        {
        //            throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
        //        }
        //    }
        //}
    }
}
