using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Login;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace KANBAN.Controllers.API.DevelopMaintain
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNMT110Controller : ControllerBase
    {

        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;


        public KBNMT110Controller
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

        public List<Int64?> parentList = new List<Int64?>();


        [HttpGet]
        public async Task<IActionResult> GetMenu()
        {

            if(_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
            {
                return StatusCode(_BearerClass.Status, new
                {
                    status = _BearerClass.Status,
                    response = _BearerClass.Response,
                    message = _BearerClass.Message
                });
            }

            try
            {

                var data = await _kbContext.Menu.OrderBy(x=>x.Code)
                    .Select(x => new
                    {
                        remark = x.Code + " : " + x.Name,
                        menu_ID = x._ID,
                    }).ToListAsync();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data found",
                    data = data.Distinct().ToList()
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    response = "Internal Server Error",
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {

            if(_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
            {
                return StatusCode(_BearerClass.Status, new
                {
                    status = _BearerClass.Status,
                    response = _BearerClass.Response,
                    message = _BearerClass.Message
                });
            }

            try
            {

                var data = await _kbContext.User.Select(x => new
                {
                    x.Code
                }).ToListAsync();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data found",
                    data = data.Distinct().ToList().OrderBy(x=>x.Code)
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    response = "Internal Server Error",
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserDetailAndAuth(string User_Code)
        {

            if(_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
            {
                return StatusCode(_BearerClass.Status, new
                {
                    status = _BearerClass.Status,
                    response = _BearerClass.Response,
                    message = _BearerClass.Message
                });
            }

            try
            {

                var data = await _kbContext.User.Where(x=>x.Code == User_Code)
                    .SelectMany(user => _kbContext.UserAuthorize
                    .Where(auth => auth.User_ID == user._ID),
                    (user, auth) => new
                    {
                        user._ID,
                        user.Code,
                        user.Name,
                        user.Surname,
                        auth.Menu_ID,
                        auth.Remark,
                    }).ToListAsync();

                if(data.Count == 0)
                {
                    var user = await _kbContext.User.Where(x => x.Code.Trim() == User_Code).ToListAsync();
                    if (user == null)
                    {
                        return NotFound(new
                        {
                            status = 404,
                            response = "Not Found",
                            message = "User not found"
                        });
                    }

                    return Ok(new
                    {
                        status = "200",
                        response = "Success",
                        message = "Data found",
                        data = user
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data found",
                    data = data.Distinct().ToList()
                });

            }

            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    response = "Internal Server Error",
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetUserAuthorize(List<UserAuthorize> listObj)
        {
            using var transaction = _kbContext.Database.BeginTransaction();

            transaction.CreateSavepoint("Start SetUserAuthorize");

            if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
            {
                return StatusCode(_BearerClass.Status, new
                {
                    status = _BearerClass.Status,
                    response = _BearerClass.Response,
                    message = _BearerClass.Message
                });
            }

            try
            {

                var exist = await _kbContext.UserAuthorize.Where(x => listObj[0].User_ID == x.User_ID).ToListAsync();

                if (exist.Count > 0)
                {
                    _kbContext.RemoveRange(exist);
                    await _kbContext.SaveChangesAsync();
                }

                foreach (var item in listObj)
                {
                    if (parentList.Contains(item.Menu_ID))
                    {
                        continue;
                    }

                    var data = new UserAuthorize
                    {
                        User_ID = item.User_ID,
                        Menu_ID = item.Menu_ID,
                        Remark = item.Remark,
                        CreateBy = _BearerClass.UserCode,
                        CreateAt = DateTime.Now,
                        UpdateBy = _BearerClass.UserCode,
                        UpdateAt = DateTime.Now
                    };

                    await _kbContext.UserAuthorize.AddAsync(data);
                    parentList.Add(item.Menu_ID);

                    Int64? parent = item.Menu_ID;

                    do 
                    {
                        parent = await addParentAuth(parent,item.User_ID);
                    } 

                    while (parent != null);

                }

                await _kbContext.SaveChangesAsync();
                transaction.Commit();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data saved"
                });

            }

            catch (Exception ex)
            {
                transaction.RollbackToSavepoint("Start SetUserAuthorize");
                return StatusCode(500, new
                {
                    status = 500,
                    response = "Internal Server Error",
                    message = ex.Message
                });
            }   
        }

        public async Task<Int64?> addParentAuth(Int64? Menu_ID, Int64? User_ID)
        {
            var parent = await _kbContext.Database.SqlQueryRaw<Int64?>("SELECT Parent_ID AS VALUE FROM erp.MenuParent " +
                    $"WHERE Menu_ID = '{Menu_ID}' ").Select(x => x).FirstOrDefaultAsync();
            
            if (parent == null)
            {
                return null;
            }

            if (parentList.Contains(parent))
            {
                return null;
            }

            var menu = await _kbContext.Menu.Where(x => x._ID == parent).FirstOrDefaultAsync();
            if (menu != null)
            {
                var dataParent = new UserAuthorize
                {
                    User_ID = (long)User_ID!,
                    Menu_ID = (long)parent,
                    Remark = menu.Name,
                    CreateBy = _BearerClass.UserCode,
                    CreateAt = DateTime.Now,
                    UpdateBy = _BearerClass.UserCode,
                    UpdateAt = DateTime.Now
                };

                await _kbContext.UserAuthorize.AddAsync(dataParent);
                parentList.Add(parent);
            }
            return parent;
        }

    }
}
