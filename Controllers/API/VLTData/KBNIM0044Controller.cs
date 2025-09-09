using HINOSystem.Libs;
using KANBAN.Models.KB3.Master;
using KANBAN.Models.KB3.VLT;
using KANBAN.Services;
using KANBAN.Services.Import.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class KBNIM0044Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly IImportService _importRepo;

        public KBNIM0044Controller(
            BearerClass bearerClass,
            IImportService importRepo
            )
        {
            _BearerClass = bearerClass;
            _importRepo = importRepo;
        }

        [HttpPost]
        public async Task<IActionResult> SaveImportData(List<VM_KBNIM0044> listData)
        {
            try
            {
                

                var RowAffected = await _importRepo.KBNIM0044.SaveImportData(listData);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been saved, " + RowAffected + " Records was Saved"
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListData(bool isAll = false)
        {
            try
            {
                

                var data = await _importRepo.KBNIM0044.GetDataList(isAll);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been retrieved",
                    data = data.Select(x => new
                    {
                        x.F_Date,
                        x.F_Line_ID,
                        x.F_Seq,
                        x.F_PartCode,
                        x.F_Parent_Part,
                        x.F_Deli_Date,
                        x.F_Deli_Shift,
                        x.F_Deli_Trip,
                        x.F_Flag,
                        x.F_Update_By,
                        x.F_Update_Date,
                        x.F_Customer,
                        x.F_Cust_Seq,
                    })
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFlag([FromBody] List<TB_Import_VHD> listObj, [FromQuery] string shift)
        {
            try
            {
                

                await _importRepo.KBNIM0044.UpdateFlag(listObj, shift);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been updated"
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Confirm([FromBody] List<TB_Import_VHD> listData, [FromQuery] string InchargeUser)
        {
            try
            {
                

                await _importRepo.KBNIM0044.Confirm(listData, InchargeUser);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been Confirmed"
                });
            }
            catch (CustomHttpException ex)
            {
                throw new CustomHttpException(ex.StatusCode, ex.Message);
            }
        }

    }
}
