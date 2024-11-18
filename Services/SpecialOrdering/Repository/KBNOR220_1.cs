using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public class KBNOR220_1 : IKBNOR220_1
    {

        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly ISpecialLibs _specialLibs;
        private readonly ISpecialOrderingServices _services;

        public KBNOR220_1
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            ISpecialLibs specialLibs,
            ISpecialOrderingServices services
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _specialLibs = specialLibs;
            _services = services;
        }

        public string LoadSurveyDoc(string? surveyDoc, string? mode)
        {
            try
            {
                var _dt = _specialLibs.GetSurveyHeader(_BearerClass.Plant, surveyDoc, "0", mode);

                return JsonConvert.SerializeObject(_dt);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Save(bool isDel, List<VM_Post_KBNOR220_1> listModel)
        {
            try
            {
                if (!isDel)
                {
                    if (_services.IKBNOR220.ChkDebitCode(listModel[0].F_Acc_Dr, DateTime.Now.ToString("yyyyMMdd")))
                    {
                        throw new CustomHttpException(400, "Not match debit account code in master!");
                    }
                    if (_services.IKBNOR220.ChkDeptCode(listModel[0].F_Dept_Code, DateTime.Now.ToString("yyyyMMdd")))
                    {
                        throw new CustomHttpException(400, "Not match dept code in master!");
                    }
                    if (chkTagColor(listModel[0].F_Remark_KB))
                    {
                        throw new CustomHttpException(400, "Not match color of tag in master!");
                    }
                    string sWKCode = "";
                    if (listModel[0].F_WK_Code.Trim().ToUpper() == "-  -")
                    {
                        sWKCode = "";
                    }
                    else
                    {
                        sWKCode = listModel[0].F_WK_Code.Trim().ToUpper();
                    }

                    foreach (var model in listModel)
                    {
                        var exModel = await _kbContext.TB_Survey_Header
                            .FirstOrDefaultAsync(x => x.F_Survey_Doc == model.F_Survey_Doc
                            && x.F_Status == "N");

                        if (exModel == null)
                        {
                            throw new CustomHttpException(400, "Survey Header not found!");
                        }

                        exModel.F_Dept_Code = model.F_Dept_Code;
                        exModel.F_Acc_Cr = model.F_Acc_Cr;
                        exModel.F_Acc_Dr = model.F_Acc_Dr;
                        exModel.F_WK_Code = sWKCode;
                        exModel.F_Remark = model.F_Remark;
                        exModel.F_Remark2 = model.F_Remark2;
                        exModel.F_Remark3 = model.F_Remark3;
                        exModel.F_Remark_KB = model.F_Remark_KB;
                        exModel.F_CustomerOrder_Type = model.F_CustomerOrder_Type;
                        exModel.F_Update_By = _BearerClass.UserCode;
                        exModel.F_Update_Date = DateTime.Now;

                        _kbContext.Update(exModel);
                        _log.WriteLogMsg("Update Survey Header" + JsonConvert.SerializeObject(exModel));
                    }

                    await _kbContext.SaveChangesAsync();
                }
                else
                {
                    foreach (var model in listModel)
                    {
                        var exModel = await _kbContext.TB_Survey_Header
                            .FirstOrDefaultAsync(x => x.F_Survey_Doc == model.F_Survey_Doc
                                                       && x.F_PO_Customer == model.F_PO_Customer);

                        if (exModel == null)
                        {
                            throw new CustomHttpException(400, "Survey Header not found!");
                        }

                        exModel.F_Status_D = "D";
                        exModel.F_Update_By = _BearerClass.UserCode;
                        exModel.F_Update_Date = DateTime.Now;

                        _kbContext.Update(exModel);
                        _log.WriteLogMsg("Delete Survey Header" + JsonConvert.SerializeObject(exModel));
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public bool chkTagColor(string tagColor)
        {
            try
            {
                string _sql = $@"Select * FROM  TB_MS_TagColor Where F_Color_Tag  = '{tagColor}'";

                var _dt = _FillDT.ExecuteSQL(_sql);

                if (_dt.Rows.Count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

    }

}
