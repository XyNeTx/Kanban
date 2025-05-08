using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Services;
using KANBAN.Services.Master.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class KBNMS021Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly IMasterRepo _masterRepo;

        public KBNMS021Controller(
            BearerClass bearerClass,
            IMasterRepo masterRepo
            )
        {
            _BearerClass = bearerClass;
            _masterRepo = masterRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetListDataTables(string? Line, string? PartCode, string? PartNo)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _masterRepo.IKBNMS021.GetListDataTables(Line, PartCode, PartNo);

                if (data.Count == 0)
                {
                    throw new CustomHttpException(404, "Data Not Found");
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data.Select(x => new
                    {
                        F_Line = x.F_Line.Trim() == "F" ? "FRAME" : x.F_Line.Trim() == "R" ? "Rear Axle" : x.F_Line.Trim() == "S" ? "Side Panel" : x.F_Line.Trim() == "T" ? "Tail Gate" : x.F_Line.Trim() == "D" ? "De Dion" : " ",
                        F_Code = x.F_Code.Trim(),
                        F_Part_No = x.F_Part_No.Trim() + "-" + x.F_Ruibetsu.Trim(),
                        F_name = x.F_name.Trim(),
                        F_Bridge = x.F_Bridge.Trim() == "Y" ? "TRUE" : "FALSE",
                        F_Detail = x.F_Detail.Trim(),
                        F_Bridges = x.F_Bridge.Trim() == "Y" ? "TRUE" : "FALSE",
                        F_Details = x.F_Detail.Trim(),
                    }).ToList()
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLine(string? PartCode, string? PartNo)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _masterRepo.IKBNMS021.GetLine(PartCode, PartNo);

                if (data.Count == 0)
                {
                    throw new CustomHttpException(404, "Data Not Found");
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data.Select(x => new
                    {
                        F_Line = x.F_Line.Trim() == "F" ? "FRAME" : x.F_Line.Trim() == "R" ? "Rear Axle" : x.F_Line.Trim() == "S" ? "Side Panel" : x.F_Line.Trim() == "T" ? "Tail Gate" : x.F_Line.Trim() == "D" ? "De Dion" : " ",
                    }).Distinct().ToList()
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPartCode(string? Line, string? PartNo)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _masterRepo.IKBNMS021.GetPartCode(Line, PartNo);

                if (data.Count == 0)
                {
                    throw new CustomHttpException(404, "Data Not Found");
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data.Select(x => new
                    {
                        F_Code = x.F_Code.Trim(),
                    }).Distinct().ToList()
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPartNo(string? Line, string? PartCode)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                var data = await _masterRepo.IKBNMS021.GetPartNo(Line, PartCode);

                if (data.Count == 0)
                {
                    throw new CustomHttpException(404, "Data Not Found");
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Found",
                    data = data.Select(x => new
                    {
                        F_Part_No = x.F_Part_No.Trim() + "-" + x.F_Ruibetsu.Trim(),
                    }).Distinct().ToList()
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(List<TB_MS_PartCode> listObj, [FromQuery] string action)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                await _masterRepo.IKBNMS021.Save(listObj, action);

                var data = await _masterRepo.IKBNMS021.CheckPairPart();

                var groupData = data.GroupBy(x => new
                {
                    x.F_Line,
                    x.F_Code,
                    x.F_Bridge,
                }).Where(x => x.Count() < 2).ToList();

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data Saved",
                    data = groupData.Select(x => new
                    {
                        F_Line = x.Key.F_Line.Trim(),
                        F_Code = x.Key.F_Code.Trim(),
                        F_Bridge = x.Count(),
                    }).ToList()
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

    }
}
