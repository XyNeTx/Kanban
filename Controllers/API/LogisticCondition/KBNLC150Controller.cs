using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Models.KB3.LogisticCondition;
using KANBAN.Services.Logistical;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace HINOSystem.Controllers.API.Master
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class KBNLC150Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly KanbanConnection _KBCN;
        private readonly ILogisticService _Service;

        public KBNLC150Controller(
            BearerClass bearerClass,
            KanbanConnection kanbanConnection,
            ILogisticService service
            )
        {
            _BearerClass = bearerClass;
            _KBCN = kanbanConnection;
            _Service = service;
        }



        [HttpPost]
        public IActionResult initial([FromBody] string pData = null)
        {
            dynamic _json = null;
            string _SQL = "";
            try
            {
                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                _SQL = @" EXEC [exec].[spTB_MS_FACTORY] ";
                string _jsTB_MS_Factory = _KBCN.ExecuteJSON(_SQL, pUser: _BearerClass, pControllerName: ControllerContext.ActionDescriptor.ControllerName, pActionName: ControllerContext.ActionDescriptor.ActionName);

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
        public IActionResult ShowRevision(string YM)
        {
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
                string _result = _Service.IKBNLC150.ShowRevision(YM);

                if (string.IsNullOrWhiteSpace(_result))
                {
                    throw new Exception("No Data");
                }

                return Ok(new
                {
                    status = "200",
                    response = "OK",
                    message = "Success",
                    data = _result
                });

            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = e.Message,
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Import(List<VM_TB_Import_Delivery> listObj)
        {
            try
            {

                if (_BearerClass.CheckAuthen() == 401 || _BearerClass.CheckAuthen() == 403)
                {
                    return StatusCode(_BearerClass.Status, new
                    {
                        status = _BearerClass.Status,
                        response = _BearerClass.Response,
                        message = _BearerClass.Message
                    });
                }

                if (await _Service.IKBNLC150.Import(listObj) == "Success")
                {
                    return Ok(new
                    {
                        status = "200",
                        response = "OK",
                        message = "Import Data Success"
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        status = "500",
                        response = "Internal Server Error",
                        message = "Import Failed"
                    });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = ex.Message
                });
            }
        }


        [HttpGet]
        public IActionResult GetProcessBar()
        {
            return Ok(new
            {
                status = "200",
                response = "OK",
                message = "Success",
                data = _Service.IKBNLC150.GetProcessBar()
            });
        }
    }
}
