using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Models.KB3.Master;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class KBNMS010Controller : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly KB3Context _KB3Context;
        private readonly SerilogLibs _Log;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNMS010Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            KB3Context kB3Context,
            PPM3Context pPM3Context,
            SerilogLibs serilogLibs
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;
            _PPM3Context = pPM3Context;
            _Log = serilogLibs;
        }


        [HttpPost]
        public IActionResult initial([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                _BearerClass.Authentication(Request);
                if (_BearerClass.Status == 401) return Content(JsonConvert.SerializeObject(_BearerClass.Result), "application/json");

                

                _SQL = @" EXEC [exec].[spTB_MS_FACTORY] ";
                string _jsTB_MS_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName : ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

                string _result = @"{
                    ""status"":""200"",
                    ""response"":""OK"",
                    ""message"": ""Data Found"",
                    ""data"":
                            {
                                ""TB_MS_Factory"" : " + _jsTB_MS_Factory + @"
                            }
                }";
                return Content(_result, "application/json");
            }
            catch (Exception e)
            {
                return Content(e.Message.ToString(), "application/json");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get_StoreCode(string? F_YM = null, bool? IsInquiry = false)
        {
            
            _BearerClass.Authentication(Request);

            if (_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });

            try
            {
                if ((bool)IsInquiry!)
                {
                    var data = await _KB3Context.TB_Calendar.Where(x => x.F_YM == F_YM).Select(x=>new { F_Store_cd = x.F_Store_cd }).ToListAsync();
                    if (data.Count == 0) return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Not Found",
                        message = "Data Not Found"
                    });
                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        title = "Success",
                        message = "Data Found",
                        data = data
                    });
                }

                else
                {
                    var data = await _PPM3Context.T_Construction.Where(x => x.F_Store_cd.StartsWith(_BearerClass.Plant))
                        .Select(x => new { F_Store_cd = x.F_Store_cd.Trim() }).ToListAsync();

                    data = data.DistinctBy(x => x.F_Store_cd).OrderBy(x => x.F_Store_cd).ToList();

                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        title = "Success",
                        message = "Data Found",
                        data = data
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected error",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCalendarData(string F_YM, string F_Store_cd)
        {
            
            _BearerClass.Authentication(Request);

            if (_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });

            try
            {
                var data = await _KB3Context.TB_Calendar.Where(x => x.F_YM == F_YM && x.F_Store_cd == F_Store_cd).FirstOrDefaultAsync();
                if (data == null) return NotFound(new
                {
                    status = "404",
                    response = "Not Found",
                    title = "Not Found",
                    message = "Data Not Found"
                });
                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Found",
                    data = data
                });
            }

            catch ( Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected error",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Copy([FromQuery] string F_YM)
        {
            try
            {

                _BearerClass.Authentication(Request);
                if(_BearerClass.Status == 401)
                {
                    return StatusCode(401, new
                    {
                        status = "401",
                        response = "Unauthorized",
                        message = "Unauthorized Access!"
                    });
                }

                var storeList = await _PPM3Context.T_Construction.Where(x => x.F_Store_cd.StartsWith(_BearerClass.Plant))
                        .Select(x => new { F_Store_cd = x.F_Store_cd.Trim() }).ToListAsync();

                storeList = storeList.DistinctBy(x => x.F_Store_cd).OrderBy(x => x.F_Store_cd).ToList();
                
                string defaultStore = _BearerClass.Plant switch
                {
                    "1" => "1A",
                    "2" => "2B",
                    "3" => "3C",
                    _ => "3C"
                };

                var data = await _KB3Context.TB_Calendar.Where(x => x.F_YM == F_YM && x.F_Store_cd == defaultStore).FirstOrDefaultAsync();
                
                if (data == null)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Not Found",
                        message = "Not Found Data From Calendar " + F_YM + " Store " + defaultStore
                    });
                }
                List<TB_Calendar> addList = new List<TB_Calendar>();
                foreach (var store in storeList)
                {

                    var IsExisted = await _KB3Context.TB_Calendar.AnyAsync(x => x.F_YM == F_YM && x.F_Store_cd == store.F_Store_cd);
                    if (IsExisted) continue;
                    else
                    {
                        var addData = new TB_Calendar();
                        addData = JsonConvert.DeserializeObject<TB_Calendar>(JsonConvert.SerializeObject(data)); // <--- Copy Data from Default Store --
                        addData.F_Store_cd = store.F_Store_cd;
                        addData.F_Create_By = _BearerClass.UserCode;
                        addData.F_Create_Date = DateTime.Now;
                        addData.F_Update_By = _BearerClass.UserCode;
                        addData.F_Update_Date = DateTime.Now;
                        addList.Add(addData);
                    }
                }
                await _KB3Context.TB_Calendar.AddRangeAsync(addList);
                await _KB3Context.SaveChangesAsync();

                _Log.WriteLogMsg($"Copy : {JsonConvert.SerializeObject(addList)}");

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Calendar Copied To All Store !"
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected error",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(TB_Calendar obj)
        {
            

            _BearerClass.Authentication(Request);

            if (_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });

            try
            {
                var IsExisted = await _KB3Context.TB_Calendar.AnyAsync(x => x.Equals(obj));
                if(IsExisted)
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        title = "Bad Request",
                        message = "Data Already Exist!"
                    });
                }

                obj.F_Create_By = _BearerClass.UserCode;
                obj.F_Create_Date = DateTime.Now;
                obj.F_Update_By = _BearerClass.UserCode;
                obj.F_Update_Date = DateTime.Now;

                _KB3Context.TB_Calendar.Add(obj);
                await _KB3Context.SaveChangesAsync();
                _Log.WriteLog($"Action : Save | Database : TB_Calendar | F_YM : {obj.F_YM} | F_Store_cd : {obj.F_Store_cd}",_BearerClass.UserCode,_BearerClass.Device);

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Saved!"
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error!",
                    err = ex.Message.ToString()
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(TB_Calendar obj)
        {
            
            _BearerClass.Authentication(Request);

            if (_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });

            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
            try
            {
                _KB3Transaction.CreateSavepoint("StartUpdate");

                var IsExisted = await _KB3Context.TB_Calendar.AnyAsync(x => x.Equals(obj));
                if(!IsExisted)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Not Found",
                        message = "Not found Data to Update!"
                    });
                }
                
                obj.F_Update_By = _BearerClass.UserCode;
                obj.F_Update_Date = DateTime.Now;

                _KB3Context.TB_Calendar.Update(obj);
                await _KB3Context.SaveChangesAsync();
                _KB3Transaction.Commit();

                _Log.WriteLog($"Action : Update | Database : TB_Calendar | F_YM : {obj.F_YM} | F_Store_cd : {obj.F_Store_cd}", _BearerClass.UserCode, _BearerClass.Device);

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Updated!"
                });
            }
            catch (Exception ex)
            {
                _KB3Transaction.RollbackToSavepoint("StartUpdate");
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error!",
                    err = ex.Message.ToString()
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(TB_Calendar obj)
        {
            
            _BearerClass.Authentication(Request);

            if (_BearerClass.Status == 401) return Unauthorized(new
            {
                status = "401",
                response = "Unauthorized",
                title = "Unauthorized",
                message = "Please Login First"
            });

            using var _KB3Transaction = _KB3Context.Database.BeginTransaction();
            try
            {
                _KB3Transaction.CreateSavepoint("StartRemove");

                var IsExisted = await _KB3Context.TB_Calendar.AnyAsync(x => x.Equals(obj));
                if (!IsExisted)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        title = "Not Found",
                        message = "Not found Data to Update!"
                    });
                }

                _KB3Context.TB_Calendar.Remove(obj);
                await _KB3Context.SaveChangesAsync();
                _KB3Transaction.Commit();

                _Log.WriteLog($"Action : Delete | Database : TB_Calendar | F_YM : {obj.F_YM} | F_Store_cd : {obj.F_Store_cd}", _BearerClass.UserCode, _BearerClass.Device);

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    title = "Success",
                    message = "Data Removed!"
                });
            }
            catch (Exception ex)
            {
                _KB3Transaction.RollbackToSavepoint("StartRemove");
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    title = "Error",
                    message = "Unexpected Error!",
                    err = ex.Message.ToString()
                });
            }
        }
    }
}
