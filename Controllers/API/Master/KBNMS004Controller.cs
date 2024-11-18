using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Services.SpecialOrdering.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KBNMS004Controller : ControllerBase
    {
        private readonly ISpecialOrderingServices _services;
        private readonly BearerClass _bearerClass;

        public KBNMS004Controller(ISpecialOrderingServices services, BearerClass bearerClass)
        {
            _services = services;
            _bearerClass = bearerClass;
        }

        [HttpGet]
        public IActionResult ListData(string? Supplier)
        {
            try
            {
                if(_bearerClass.CheckAuthen() == 401 || _bearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_bearerClass.Status,
                        new
                        {
                            status = _bearerClass.Status,
                            response = _bearerClass.Response,
                            message = _bearerClass.Message
                        });
                }

                var data = _services.IKBNMS004.List_Data(Supplier);

                return Ok(new { status = "200", response = "Success", message = "Data Found", data = data });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("No data found"))
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        message = ex.Message
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        status = "500",
                        response = "Internal Server Error",
                        message = ex.Message
                    });
                }
            }
        }

        [HttpGet]
        public IActionResult SupplierDropDown(bool isNew)
        {
            try
            {
                if (_bearerClass.CheckAuthen() == 401 || _bearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_bearerClass.Status,
                                               new
                                               {
                            status = _bearerClass.Status,
                            response = _bearerClass.Response,
                            message = _bearerClass.Message
                        });
                }

                var data = _services.IKBNMS004.SupplierDropDown(isNew);

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
                if (ex.Message.Contains("No data found"))
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        message = ex.Message
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        status = "500",
                        response = "Internal Server Error",
                        message = ex.Message
                    });
                }
            }
        }

        [HttpGet]
        public IActionResult SupplierChanged(string Supplier)
        {
            try
            {
                if (_bearerClass.CheckAuthen() == 401 || _bearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_bearerClass.Status,
                                               new
                                               {
                                                   status = _bearerClass.Status,
                                                   response = _bearerClass.Response,
                                                   message = _bearerClass.Message
                                               });
                }

                var data = _services.IKBNMS004.SupplierChanged(Supplier);

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
                if (ex.Message.Contains("No data found"))
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        message = ex.Message
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        status = "500",
                        response = "Internal Server Error",
                        message = ex.Message
                    });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromQuery] string Action,[FromBody] List<TB_MS_SupplierAttn> listModel)
        {
            try
            {
                if (_bearerClass.CheckAuthen() == 401 || _bearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_bearerClass.Status,
                        new
                        {
                            status = _bearerClass.Status,
                            response = _bearerClass.Response,
                            message = _bearerClass.Message
                        });
                }

                await _services.IKBNMS004.Save(Action, listModel);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Saved"
                });

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Data already exists"))
                {
                    return BadRequest(new
                    {
                        status = "400",
                        response = "Bad Request",
                        message = ex.Message
                    });
                }
                else if (ex.Message.Contains("Data not found"))
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        message = ex.Message
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        status = "500",
                        response = "Internal Server Error",
                        message = ex.Message
                    });
                }
            }
        }

    }
}
