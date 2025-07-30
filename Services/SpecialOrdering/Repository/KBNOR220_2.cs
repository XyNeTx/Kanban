using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Master;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Services.SpecialOrdering.Repository
{
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
                string storeCD = _BearerClass.Plant switch
                {
                    "1" => "1F",
                    "2" => "2B",
                    "3" => "3C",
                    _ => "3C",
                };

                var data = await _kbContext.TB_Calendar.Where(x => x.F_Store_cd == storeCD
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
                    .Join(_kbContext.TB_Survey_Detail.Where(d => d.F_PDS_No == ""),
                    h => new { h.F_Survey_Doc, h.F_Revise_Rev },
                    d => new { d.F_Survey_Doc, d.F_Revise_Rev },
                    (h, d) => new { h, d })
                    .Select(x => new TB_Survey_Detail
                    {
                        F_PO_Customer = x.d.F_PO_Customer.Trim()
                    }).OrderBy(x => x.F_PO_Customer).ToListAsync();

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

        public string GetSuppCD(string PO, string? YM)
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

        public async Task<List<TB_Survey_Detail>> GetPartNo(string PO, string SuppCD)
        {
            try
            {
                var data = await _kbContext.TB_Survey_Header.AsNoTracking()
                    .Where(h => h.F_Supplier_CD.Trim() + "-" + h.F_Supplier_Plant.Trim() == SuppCD)
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

        public async Task<int> PartNoSelected(string surveyDoc, string suppCD, string partNo)
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

        public async Task<string> GetCalendarQty(string SurveyDoc, string suppCD, string YM, string partNo)
        {
            try
            {
                var data = await _kbContext.TB_Survey_Header.AsNoTracking()
                    .Where(h => h.F_Survey_Doc.Trim() == SurveyDoc
                    && h.F_Supplier_CD.Trim() + "-" + h.F_Supplier_Plant.Trim() == suppCD)
                    .Join(_kbContext.TB_Survey_Detail.AsNoTracking()
                    .Where(d => d.F_Delivery_Date.Trim().StartsWith(YM)
                    && d.F_Part_No.Trim() + "-" + d.F_Ruibetsu.Trim() == partNo),
                    h => new { h.F_Survey_Doc, h.F_Revise_Rev },
                    d => new { d.F_Survey_Doc, d.F_Revise_Rev },
                    (h, d) => new { h, d })
                    .AsNoTracking().OrderBy(x => x.d.F_Delivery_Date.Trim().Substring(6, 2)).ToListAsync();

                if (data.Count == 0)
                {
                    throw new CustomHttpException(StatusCodes.Status404NotFound, "Data Not Found");
                }

                data = data.DistinctBy(x => new
                {
                    x.d.F_Delivery_Date,
                    x.d.F_Qty,
                    x.d.F_Part_No,
                    x.d.F_Ruibetsu

                }).ToList();

                return JsonConvert.SerializeObject(data);

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

        public async Task Save(List<VM_Post_KBNOR220_2> listObj)
        {
            try
            {
                string SurveyDoc = "";

                foreach (var obj in listObj)
                {
                    if (obj.Qty != 0 && !string.IsNullOrWhiteSpace(obj.Qty.ToString()))
                    {

                        var DTM = await _kbContext.TB_Survey_Detail
                            .Where(x => x.F_Survey_Doc.Trim() == obj.Survey
                            && !string.IsNullOrWhiteSpace(x.F_Survey_Doc)
                            && x.F_Delivery_Date.Trim().StartsWith(obj.Delivery_Date)
                            && x.F_Part_No.Trim() + "-" + x.F_Ruibetsu.Trim() == obj.PartNo).FirstOrDefaultAsync();

                        if (DTM != null)
                        {
                            _kbContext.TB_Survey_Detail.Remove(DTM);
                            await _kbContext.SaveChangesAsync();

                            DTM.F_Adjust_Qty = obj.POQty;
                            DTM.F_Qty = obj.Qty.Value;
                            DTM.F_Delivery_Date = obj.Delivery_Date;
                            DTM.F_PDS_No = "";
                            DTM.F_PDS_Flg = 0;

                            await _kbContext.TB_Survey_Detail.AddAsync(DTM);

                        }
                        else
                        {
                            var DTY = await _kbContext.TB_Survey_Header.AsNoTracking()
                                .Where(h => h.F_Supplier_CD.Trim() + "-" + h.F_Supplier_Plant.Trim() == obj.SuppCD)
                                .Join(_kbContext.TB_Survey_Detail.AsNoTracking()
                                .Where(d => d.F_PO_Customer.Trim() == obj.Survey.Substring(0, obj.Survey.Length - 4)
                                && d.F_Delivery_Date.StartsWith(obj.Delivery_Date.Substring(0, 6))),
                                h => new { h.F_Survey_Doc, h.F_Revise_Rev },
                                d => new { d.F_Survey_Doc, d.F_Revise_Rev },
                                (h, d) => new { h, d }).ToListAsync();

                            if (DTY.Count <= 0)
                            {
                                SurveyDoc = getMSurveyID(obj.Survey);
                                var DTIssue = _specialLibs.GetIssueBy();

                                var insHead = await _kbContext.TB_Survey_Header
                                    .Where(x => x.F_Survey_Doc.Trim() == obj.Survey
                                    && x.F_Supplier_CD.Trim() + "-" + x.F_Supplier_Plant.Trim() == obj.SuppCD)
                                    .OrderByDescending(x => x.F_Survey_Doc).FirstOrDefaultAsync();

                                insHead.F_Survey_Doc = SurveyDoc;
                                insHead.F_Supplier_CD = obj.SuppCD.Split('-')[0];
                                insHead.F_Supplier_Plant = obj.SuppCD.Split('-')[1];
                                insHead.F_Issued_Date = DateTime.Now.ToString("yyyyMMdd");
                                insHead.F_Issue_By = DTIssue.Rows[0]["F_User_name"].ToString().Trim();
                                insHead.F_Issue_Tel = DTIssue.Rows[0]["F_Telephone"].ToString().Trim();
                                insHead.F_Issue_Fax = DTIssue.Rows[0]["F_Fax"].ToString().Trim();
                                insHead.F_Issue_Mail = DTIssue.Rows[0]["F_Email"].ToString().Trim();
                                insHead.F_Delay_Date = _specialLibs.GetDelayDate(DateTime.Now.ToString("yyyyMMdd"));
                                insHead.F_Confirm_Date = null;
                                insHead.F_Status = "N";
                                insHead.F_Upload_Flg = "0";
                                insHead.F_PDS_No = "";
                                insHead.F_PDS_Flg = "0";
                                insHead.F_Approve_Date = null;
                                insHead.F_Approve_By = "";
                                insHead.F_Create_Date = DateTime.Now;
                                insHead.F_Update_Date = null;
                                insHead.F_Update_By = "";

                                _kbContext.TB_Survey_Header.Add(insHead);

                                var insList = await _kbContext.TB_Survey_Detail
                                    .Where(x => x.F_Survey_Doc.Trim() == obj.Survey
                                    && x.F_Part_No.Trim() + "-" + x.F_Ruibetsu.Trim() == obj.PartNo)
                                    .ToListAsync();

                                insList = insList.DistinctBy(x => new
                                {
                                    x.F_Survey_Doc,
                                    x.F_PO_Customer,
                                    x.F_Part_No,
                                    x.F_Part_Name,
                                    x.F_Ruibetsu,
                                    x.F_Kanban_No,
                                    x.F_Store_Code,
                                    x.F_Package,
                                    x.F_Adjust_Qty,
                                }).ToList();

                                var insDetail = insList[0];

                                insDetail.F_Survey_Doc = SurveyDoc;
                                insDetail.F_No = 1;
                                insDetail.F_Qty = obj.Qty.Value;
                                insDetail.F_Status = "N";
                                insDetail.F_Delivery_Date = obj.Delivery_Date;

                                _kbContext.TB_Survey_Detail.Add(insDetail);

                                _log.WriteLogMsg("Insert TB_Survey_Header Condtion => add header and detail | Obj => " + JsonConvert.SerializeObject(insHead));
                                _log.WriteLogMsg("Insert TB_Survey_Detail Condtion => add header and detail | Obj => " + JsonConvert.SerializeObject(insDetail));
                            }
                            else
                            {
                                var insDetailList = await _kbContext.TB_Survey_Detail
                                    .Where(x => x.F_Survey_Doc.Trim() == obj.Survey
                                    && x.F_Part_No.Trim() + "-" + x.F_Ruibetsu.Trim() == obj.PartNo)
                                    .ToListAsync();

                                insDetailList = insDetailList.DistinctBy(x => new
                                {
                                    x.F_Survey_Doc,
                                    x.F_PO_Customer,
                                    x.F_Part_No,
                                    x.F_Part_Name,
                                    x.F_Ruibetsu,
                                    x.F_Kanban_No,
                                    x.F_Status,
                                    x.F_Store_Code,
                                    x.F_Package,
                                    x.F_Adjust_Qty,
                                }).ToList();

                                var maxNo = insDetailList.GroupBy(x => x.F_Survey_Doc)
                                    .Select(x => new
                                    {
                                        x.Key,
                                        MaxNo = x.Max(y => y.F_No)
                                    }).FirstOrDefault();

                                var insDetail = insDetailList[0];

                                insDetail.F_Survey_Doc = DTY[0].h.F_Survey_Doc;
                                insDetail.F_Revise_Rev = DTY[0].d.F_Revise_Rev;
                                insDetail.F_No = insDetailList.Count + 1;
                                insDetail.F_Qty = obj.Qty.Value;
                                insDetail.F_Delivery_Date = obj.Delivery_Date;

                                _kbContext.TB_Survey_Detail.Add(insDetail);

                                _log.WriteLogMsg("Insert TB_Survey_Detail Condtion => add new Qty | Obj => " + JsonConvert.SerializeObject(insDetail));
                            }
                        }
                    }
                    else
                    {

                        //EF Core cant update primary key
                        string _sql = $@"Update TB_Survey_Detail Set F_Qty = 0 
                            Where F_Survey_Doc = '{obj.Survey}' 
                            and F_Part_No = '{obj.PartNo.Split('-')[0]}' 
                            and F_Ruibetsu = '{obj.PartNo.Split('-')[1]}' 
                            and F_Delivery_Date = '{obj.Delivery_Date}' ";

                        await _kbContext.Database.ExecuteSqlRawAsync(_sql);
                    }
                    await _kbContext.SaveChangesAsync();
                }

                var delList = await _kbContext.TB_Survey_Detail.Where(x => x.F_Survey_Doc.Trim() == listObj[0].Survey
                        && x.F_Qty == 0).ToListAsync();

                _kbContext.TB_Survey_Detail.RemoveRange(delList);

                _log.WriteLogMsg("DELETE TB_Survey_Detail => " + JsonConvert.SerializeObject(delList, Formatting.Indented));

                var DTZ = await _kbContext.TB_Survey_Header
                    .Where(x => x.F_Survey_Doc.Trim() == listObj[0].Survey)
                    .Join(_kbContext.TB_Survey_Detail,
                    h => new { h.F_Survey_Doc },
                    d => new { d.F_Survey_Doc },
                    (h, d) => new
                    {
                        h.F_Upload_Flg
                    }).FirstOrDefaultAsync();

                if (DTZ.F_Upload_Flg == "1")
                {
                    var updHead = await _kbContext.TB_Survey_Header
                        .Where(x => x.F_Survey_Doc.Trim() == listObj[0].Survey).ToListAsync();

                    foreach (var item in updHead)
                    {
                        item.F_Issued_Date = DateTime.Now.ToString("yyyyMMdd");
                        item.F_Confirm_Date = "";
                        item.F_Delay_Date = _specialLibs.GetDelayDate(DateTime.Now.ToString("yyyyMMdd"));
                        item.F_Status = "R";
                        item.F_Upload_Flg = "0";
                        item.F_Remark_Delivery = "";
                        item.F_Approve_Name = "";
                        item.F_Approve_Mobile = "";
                        item.F_Approve_Position = "";
                        item.F_Approve_Dept = "";
                        item.F_Update_Date = DateTime.Now;
                        item.F_Update_By = _BearerClass.UserCode;

                        _kbContext.TB_Survey_Header.Update(item);

                    }
                }

                string sql = $@"Select F_Survey_Doc,F_Part_no,F_Ruibetsu,F_Status,F_Delivery_Date 
                    ,ROW_NUMBER() OVER(PARTITION BY F_Survey_Doc Order by F_Delivery_Date,F_Part_No,F_Ruibetsu) As ROWID 
                    From TB_Survey_Detail 
                    Where F_Survey_Doc = '{listObj[0].Survey}' 
                    and F_Qty > 0 
                    Group by F_Survey_Doc,F_Delivery_Date,F_Part_no,F_Ruibetsu,F_Status 
                    Order by F_Delivery_Date,F_Part_no,F_Ruibetsu,F_Status ";

                var DTS = _FillDT.ExecuteSQL(sql);

                if (DTS.Rows.Count > 0)
                {
                    foreach (DataRow row in DTS.Rows)
                    {
                        var _updDetail = await _kbContext.TB_Survey_Detail
                            .Where(x => x.F_Survey_Doc.Trim() == row["F_Survey_Doc"].ToString().Trim()
                            && x.F_Part_No.Trim() == row["F_Part_no"].ToString().Trim()
                            && x.F_Ruibetsu.Trim() == row["F_Ruibetsu"].ToString().Trim()
                            && x.F_Delivery_Date.Trim() == row["F_Delivery_Date"].ToString().Trim()).FirstOrDefaultAsync();

                        if (_updDetail != null)
                        {
                            _updDetail.F_No = int.Parse(row["ROWID"].ToString());
                            _updDetail.F_Status = "N";
                            _kbContext.TB_Survey_Detail.Update(_updDetail);
                        }
                    }
                }
                else
                {
                    var updHead = await _kbContext.TB_Survey_Header
                        .Where(x => x.F_Survey_Doc.Trim() == listObj[0].Survey).ToListAsync();

                    _log.WriteLogMsg("DELETE TB_Survey_Header => " + JsonConvert.SerializeObject(updHead, Formatting.Indented));
                    await _kbContext.Database.ExecuteSqlRawAsync($"Delete From TB_Survey_Header Where F_Survey_Doc = '{listObj[0].Survey}'");
                }

                await _kbContext.SaveChangesAsync();
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

        public string getMSurveyID(string SurveyDoc)
        {
            try
            {
                string _sql = "Select Rtrim(H.F_PO_Customer) As F_PO_Customer, " +
                    "(Select CAST(Max(Right(Rtrim(F_Survey_Doc),3)) AS integer)+1 AS Cnt from TB_Survey_Header Where F_PO_Customer = H.F_PO_Customer)  AS F_Survey_Running " +
                    "From TB_Survey_Header  H " +
                    "Where Isnull(H.F_Survey_Doc,'') <> '' " +
                    $"and H.F_PO_Customer  = '{SurveyDoc.Substring(0, SurveyDoc.Length - 4)}' " +
                    $"Group by H.F_PO_Customer ";

                var data = _FillDT.ExecuteSQL(_sql);

                if (data.Rows.Count == 0)
                {
                    throw new CustomHttpException(StatusCodes.Status404NotFound, "Data Not Found");
                }

                return data.Rows[0]["F_PO_Customer"].ToString() + "/" + data.Rows[0]["F_Survey_Running"].ToString().PadLeft(3, '0');
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public async Task<string> GetSupplierName(string SuppCD, string SuppPlant)
        {
            try
            {
                return await _specialLibs.GetSupplierName(SuppCD, SuppPlant);
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
