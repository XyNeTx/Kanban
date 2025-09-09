using HINOSystem.Libs;
using KANBAN.Models.KB3.UrgentOrder;
using KANBAN.Services.UrgentOrder.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KANBAN.Controllers.API.UrgentOrder
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class KBNIM017RController : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly IUrgentRepo _UrgentRepo;
        public KBNIM017RController
            (
                BearerClass bearerClass,
                IUrgentRepo UrgentRepo
            )
        {
            _BearerClass = bearerClass;
            _UrgentRepo = UrgentRepo;
        }


        [HttpPost]
        public async Task<IActionResult> ImportToOrder(List<VM_KBNIM017R_ImportData> listObj)
        {
            

            var result = await _UrgentRepo.IKBNIM017R_Repo.GetUrgentOrders(listObj);

            return Ok(new
            {
                status = "200",
                response = "Success",
                message = "Urgent orders retrieved successfully",
                data = result.Select(x => new
                {
                    F_Supplier_CD = x.F_Supplier_CD + "-" +x.F_Supplier_Plant,
                    F_Part_No = x.F_Part_No.Trim() + "-" +x.F_Ruibetsu,
                    x.F_Kanban_No,
                    x.F_Part_Name,
                    F_Delivery_Date = x.F_Delivery_Date.Substring(6,2) + "/" + x.F_Delivery_Date.Substring(4, 2) + "/" + x.F_Delivery_Date.Substring(0, 4),
                    F_Part_Order = x.F_Part_Order.Trim() + "-" + x.F_Ruibetsu_Order,
                    x.F_Store_CD,
                    x.F_Store_Order,
                    x.F_Name_Order,
                    x.F_Qty,
                    x.F_PDS_No,
                }).OrderBy(x=>x.F_PDS_No)
                .ThenBy(x=>x.F_Supplier_CD)
                .ThenBy(x=>x.F_Part_No)
                .ThenBy(x=>x.F_Kanban_No)
                .ThenBy(x=>x.F_Store_CD)
                .ThenBy(x=>x.F_Delivery_Date)
                .ThenBy(x=>x.F_Part_Order)
                .ThenBy(x=>x.F_Store_Order)
            });

        }
    }
}
