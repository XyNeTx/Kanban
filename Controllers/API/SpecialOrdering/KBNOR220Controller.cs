using HINOSystem.Libs;
using KANBAN.Services;
using KANBAN.Services.SpecialOrdering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers.API.SpecialOrdering
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNOR220Controller : ControllerBase
    {

        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _bearerClass;

        public KBNOR220Controller(ISpecialOrderingServices services, BearerClass bearerClass)
        {
            _services = services;
            _bearerClass = bearerClass;
        }

        [HttpGet]
        public IActionResult LoadColorofTag()
        {
            try
            {
                if (_bearerClass.CheckAuthen() == 401)
                {
                    throw new CustomHttpException("Unauthorized");
                }
                else if (_bearerClass.CheckAuthen() == 403)
                {
                    throw new CustomHttpException("Forbidden");
                }

                var data = _services.IKBNOR220.LoadColorofTag();

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (Exception ex)
            {
                throw new CustomHttpException(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult LoadListView()
        {
            try
            {
                if (_bearerClass.CheckAuthen() == 401)
                {
                    throw new CustomHttpException("Unauthorized");
                }
                else if (_bearerClass.CheckAuthen() == 403)
                {
                    throw new CustomHttpException("Forbidden");
                }

                var data = _services.IKBNOR220.LoadListView();

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (Exception ex)
            {
                throw new CustomHttpException(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult InitialScreenCmb(string ProcessDT)
        {
            try
            {

                if(_bearerClass.CheckAuthen() == 401)
                {
                    throw new CustomHttpException("Unauthorized");
                }
                else if (_bearerClass.CheckAuthen() == 403)
                {
                    throw new CustomHttpException("Forbidden");
                }

                var data = _services.IKBNOR220.InitialScreenCmb(ProcessDT);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });

            }
            catch (Exception ex)
            {
                throw new CustomHttpException(ex.Message);
            }
        }

    }
}
