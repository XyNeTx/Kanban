using HINOSystem.Libs;
using KANBAN.Models.KB3.Master;
using KANBAN.Models.KB3.VLT;
using KANBAN.Services;
using KANBAN.Services.Import.Interface;
using Microsoft.AspNetCore.Mvc;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HINOSystem.Controllers.API.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
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
                await _BearerClass.CheckAuthorize();

                await _importRepo.KBNIM0044.SaveImportData(listData);

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data has been saved"
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
                await _BearerClass.CheckAuthorize();

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
        public async Task<IActionResult> UpdateFlag(List<TB_Import_VHD> listObj)
        {
            try
            {
                await _BearerClass.CheckAuthorize();

                await _importRepo.KBNIM0044.UpdateFlag(listObj);

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


    }
}
