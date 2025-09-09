using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services;
using KANBAN.Services.Automapper.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/[controller]/[action]")]
    public class KBNMS017Controller : ControllerBase
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _autoMap;


        public KBNMS017Controller
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

        public string strDateNow = DateTime.Now.ToString("yyyyMMdd");


        [HttpGet]
        public async Task<IActionResult> ListData(string? F_Part_No, string? F_Ruibetsu, string? F_Store_Cd)
        {
            try
            {
                

                var data = await _kbContext.TB_MS_RatioAddress.AsNoTracking()
                    .Where(x => x.F_Plant == User.FindFirst(ClaimTypes.Locality).Value).ToListAsync();

                if (!string.IsNullOrWhiteSpace(F_Part_No))
                {
                    data = data.Where(x => x.F_Part_No.Trim() == F_Part_No
                    && x.F_Ruibetsu == F_Ruibetsu).ToList();
                }
                if (!string.IsNullOrWhiteSpace(F_Store_Cd))
                {
                    data = data.Where(x => x.F_Store_Cd == F_Store_Cd).ToList();
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data
                });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(List<TB_MS_RatioAddress> listObj, string action)
        {
            try
            {
                string logMsg = "";
                

                var obj = listObj.FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(obj.F_Address2))
                {
                    if (obj.F_Ratio2 == 0)
                    {
                        throw new CustomHttpException(400, "Please Input Ratio2 Before Process Data");
                    }
                }

                if (!string.IsNullOrWhiteSpace(obj.F_Address3))
                {
                    if (obj.F_Ratio3 == 0)
                    {
                        throw new CustomHttpException(400, "Please Input Ratio3 Before Process Data");
                    }
                }

                var T_Con = await _PPM3Context.T_Construction.AsNoTracking()
                    .Where(x => x.F_Local_Str.CompareTo(strDateNow) <= 0
                    && x.F_Local_End.CompareTo(strDateNow) >= 0
                    && x.F_Store_cd.StartsWith(User.FindFirst(ClaimTypes.Locality).Value)).ToListAsync();

                if (!string.IsNullOrWhiteSpace(obj.F_Part_No))
                {
                    T_Con = T_Con.Where(x => x.F_Part_no.Trim() == obj.F_Part_No
                        && x.F_Ruibetsu == obj.F_Ruibetsu
                        ).ToList();
                }
                if (!string.IsNullOrWhiteSpace(obj.F_Store_Cd))
                {
                    T_Con = T_Con.Where(x => x.F_Store_cd == obj.F_Store_Cd).ToList();
                }

                if (T_Con.Count == 0)
                {
                    throw new CustomHttpException(400, "Data not exist in System!!");
                }

                if (action.ToLower() == "new")
                {
                    var listExObj = await _kbContext.TB_MS_RatioAddress.AsNoTracking()
                    .Where(x => x.F_Plant == User.FindFirst(ClaimTypes.Locality).Value).ToListAsync();

                    if (!string.IsNullOrWhiteSpace(obj.F_Part_No))
                    {
                        listExObj = listExObj.Where(x => x.F_Part_No.Trim() == obj.F_Part_No
                            && x.F_Ruibetsu == obj.F_Ruibetsu).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(obj.F_Store_Cd))
                    {
                        listExObj = listExObj.Where(x => x.F_Store_Cd == obj.F_Store_Cd).ToList();
                    }

                    if (listExObj.Count == 0)
                    {
                        obj.F_Create_By = User.FindFirst(ClaimTypes.UserData).Value;
                        obj.F_Update_By = User.FindFirst(ClaimTypes.UserData).Value;
                        obj.F_Create_Date = DateTime.Now;
                        obj.F_Update_Date = DateTime.Now;
                        obj.F_Ratio2 = obj.F_Ratio2 == 0 ? null : obj.F_Ratio2;
                        obj.F_Ratio3 = obj.F_Ratio3 == 0 ? null : obj.F_Ratio3;
                        await _kbContext.TB_MS_RatioAddress.AddAsync(obj);
                        logMsg = "INSERT INTO TB_MS_RatioAddress => " + JsonConvert.SerializeObject(obj);
                    }
                    else
                    {
                        throw new CustomHttpException(400, "Data have exist in System!!");
                    }
                }
                else if (action.ToLower() == "upd")
                {
                    var listExObj = await _kbContext.TB_MS_RatioAddress.AsNoTracking()
                    .Where(x => x.F_Plant == User.FindFirst(ClaimTypes.Locality).Value).ToListAsync();

                    if (!string.IsNullOrWhiteSpace(obj.F_Part_No))
                    {
                        listExObj = listExObj.Where(x => x.F_Part_No.Trim() == obj.F_Part_No
                            && x.F_Ruibetsu == obj.F_Ruibetsu).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(obj.F_Store_Cd))
                    {
                        listExObj = listExObj.Where(x => x.F_Store_Cd == obj.F_Store_Cd).ToList();
                    }

                    if (listExObj.Count > 0)
                    {
                        logMsg = "UPDATE TB_MS_RatioAddress Before => " + JsonConvert.SerializeObject(listExObj.FirstOrDefault());

                        obj.F_Create_By = listExObj[0].F_Create_By;
                        obj.F_Create_Date = listExObj[0].F_Create_Date;
                        obj.F_Update_By = User.FindFirst(ClaimTypes.UserData).Value;
                        obj.F_Update_Date = DateTime.Now;
                        obj.F_Ratio2 = obj.F_Ratio2 == 0 ? null : obj.F_Ratio2;
                        obj.F_Ratio3 = obj.F_Ratio3 == 0 ? null : obj.F_Ratio3;
                        _kbContext.TB_MS_RatioAddress.Attach(obj);
                        _kbContext.Entry(obj).State = EntityState.Modified;

                        logMsg += Environment.NewLine + "UPDATE TB_MS_RatioAddress After => " + JsonConvert.SerializeObject(obj);
                    }
                    else
                    {
                        throw new CustomHttpException(400, "Data have not exist in System!!");
                    }
                }
                else if (action.ToLower() == "del")
                {
                    foreach (var item in listObj)
                    {
                        var delObj = await _kbContext.TB_MS_RatioAddress
                            .FirstOrDefaultAsync(x => x.F_Plant == User.FindFirst(ClaimTypes.Locality).Value
                            && x.F_Part_No == obj.F_Part_No
                            && x.F_Ruibetsu == obj.F_Ruibetsu
                            && x.F_Store_Cd == obj.F_Store_Cd);

                        if (delObj != null)
                        {
                            _kbContext.TB_MS_RatioAddress.Remove(delObj);
                            logMsg = "DELETE TB_MS_RatioAddress => " + JsonConvert.SerializeObject(delObj);
                        }
                    }
                }
                else
                {
                    throw new CustomHttpException(400, "Please select action to process");
                }
                await _kbContext.SaveChangesAsync();
                _log.WriteLogMsg(logMsg);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Saved",
                });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDropDownList(string? F_Part_No, string? F_Ruibetsu, string? F_Store_Cd, string action)
        {
            try
            {
                
                if (action.ToLower() == "new")
                {
                    var data = await _PPM3Context.T_Construction.AsNoTracking()
                        .Where(x => x.F_Plant_CD == User.FindFirst(ClaimTypes.Locality).Value[0]
                        && x.F_Local_Str.CompareTo(strDateNow) <= 0
                        && x.F_Local_End.CompareTo(strDateNow) >= 0)
                        .ToListAsync();

                    if (!string.IsNullOrWhiteSpace(F_Part_No))
                    {
                        data = data.Where(x => x.F_Part_no.Trim() == F_Part_No
                            && x.F_Ruibetsu == F_Ruibetsu).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(F_Store_Cd))
                    {
                        data = data.Where(x => x.F_Store_cd.Trim() == F_Store_Cd).ToList();
                    }

                    return Ok(new
                    {
                        status = "200",
                        resposne = "Success",
                        message = "Data Found",
                        partNo = data.Select(x => new
                        {
                            F_Part_No = x.F_Part_no.Trim() + "-" + x.F_Ruibetsu
                        }).DistinctBy(x => x.F_Part_No).OrderBy(x => x.F_Part_No).ToList(),
                        storeCode = data.Select(x => new
                        {
                            F_Store_Cd = x.F_Store_cd
                        }).DistinctBy(x => x.F_Store_Cd).OrderBy(x => x.F_Store_Cd).ToList()
                    });
                }
                else
                {

                    var data = await _kbContext.TB_MS_RatioAddress.AsNoTracking()
                        .Where(x => x.F_Plant == User.FindFirst(ClaimTypes.Locality).Value).ToListAsync();

                    if (!string.IsNullOrWhiteSpace(F_Part_No))
                    {
                        data = data.Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == x.F_Part_No + "-" + x.F_Ruibetsu).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(F_Store_Cd))
                    {
                        data = data.Where(x => x.F_Store_Cd.Trim() == x.F_Store_Cd).ToList();
                    }

                    return Ok(new
                    {
                        status = "200",
                        resposne = "Success",
                        message = "Data Found",
                        partNo = data.Select(x => new
                        {
                            F_Part_No = x.F_Part_No.Trim() + "-" + x.F_Ruibetsu
                        }).DistinctBy(x => x.F_Part_No).OrderBy(x => x.F_Part_No).ToList(),
                        storeCode = data.Select(x => new
                        {
                            F_Store_Cd = x.F_Store_Cd
                        }).DistinctBy(x => x.F_Store_Cd).OrderBy(x => x.F_Store_Cd).ToList()
                    });
                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPartName(string? F_Part_No, string? F_Ruibetsu)
        {
            try
            {
                

                var PartName = await _PPM3Context.T_Construction.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.F_Plant_CD == User.FindFirst(ClaimTypes.Locality).Value[0]
                    && x.F_Local_Str.CompareTo(strDateNow) <= 0
                    && x.F_Local_End.CompareTo(strDateNow) >= 0
                    && x.F_Part_no.Trim() == F_Part_No
                    && x.F_Ruibetsu == F_Ruibetsu);



                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = PartName.F_Part_nm.Trim()
                });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                else throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}
