using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Master;
using KANBAN.Models.KB3.SpecialOrdering;
using Microsoft.EntityFrameworkCore;

namespace KANBAN.Services.SpecialOrdering
{
    public interface IKBNOR220_2
    {
        Task<TB_Calendar> GetCalendar(string YM);
    }

    public class KBNOR220_2 : IKBNOR220_2
    {

        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;


        public KBNOR220_2
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

        public async Task<TB_Survey_Header> GetPOList()
        {
            try
            {

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
