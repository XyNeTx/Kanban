using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HINOSystem.Controllers.API.Master
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    public class KBNMS022Controller : ControllerBase
    {
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMInvenContext _PPMInvenContext;
        private readonly KB3Context _KB3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _IEmail;

        public KBNMS022Controller(
            BearerClass bearerClass,
            PPM3Context ppm3Context,
            PPMInvenContext ppmInvenContext,
            KB3Context kb3Context,
            FillDataTable fillDataTable,
            SerilogLibs log
        )
        {
            _BearerClass = bearerClass;
            _PPM3Context = ppm3Context;
            _PPMInvenContext = ppmInvenContext;
            _KB3Context = kb3Context;
            _FillDT = fillDataTable;
            _log = log;
        }


        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                string plant = Request.Cookies["plantCode"].ToString();
                string dev = Request.Cookies["isDev"].ToString() == "1" ? "Dev" : "";
                string plantDev = plant + dev;

                string connectToPPM = plantDev switch
                {
                    "1" => "[HMMT-PPM].[PPMDB]",
                    "2" => "[HMMT-PPM].[PPMDB]",
                    "3" => "[HMMTA-PPM].[PPMDB]",
                    "1Dev" => "[HMMT-PPM].[PPMDB]",
                    "2Dev" => "[HMMT-PPM].[PPMDB]",
                    "3Dev" => "[PPMDB]",
                    _ => "[PPMDB]",
                };

                string sql = "SELECT RTRIM(D.F_Supplier_Code)+'-'+RTRIM(D.F_Supplier_Plant) AS F_Supplier_Code " +
                    ",CASE WHEN D.F_Cycle = '' THEN '' " +
                    "ELSE SUBSTRING(D.F_Cycle,1,2)+'-'+SUBSTRING(D.F_Cycle,3,2)+'-'+SUBSTRING(D.F_Cycle,5,2) " +
                    "END AS F_Cycle ,CASE WHEN F_Start_Date = '' THEN '' " +
                    "ELSE SUBSTRING(D.F_Start_Date,7,2)+'/'+SUBSTRING(D.F_Start_Date,5,2)+'/'+SUBSTRING(D.F_Start_Date,1,4) " +
                    "END AS F_Start_Date ,CASE WHEN D.F_End_Date = '' THEN '' " +
                    "ELSE SUBSTRING(D.F_End_Date,7,2)+'/'+SUBSTRING(D.F_End_Date,5,2)+'/'+SUBSTRING(D.F_End_Date,1,4) " +
                    "END AS F_End_Date ,CASE WHEN D.F_Start_Order_Date = '' THEN '' " +
                    "ELSE SUBSTRING(D.F_Start_Order_Date,7,2)+'/'+SUBSTRING(D.F_Start_Order_Date,5,2)+'/'+SUBSTRING(D.F_Start_Order_Date,1,4) " +
                    "END AS F_Start_Order_Date ,CASE WHEN D.F_End_Order_Date = '' THEN '' " +
                    "ELSE SUBSTRING(D.F_End_Order_Date,7,2)+'/'+SUBSTRING(D.F_End_Order_Date,5,2)+'/'+SUBSTRING(D.F_End_Order_Date,1,4) " +
                    "END AS F_End_Order_Date ,D.F_Delivery_Trip, D.F_Delivery_Time , '('+RTRIM(S.F_short_name)+')  '+S.F_name AS F_name " +
                    $"FROM TB_MS_DeliveryTime D INNER JOIN {connectToPPM}.[dbo].[T_Supplier_ms] S " +
                    $"ON D.F_Supplier_Code = S.F_supplier_cd collate Thai_CI_AS " +
                    $"AND D.F_Supplier_Plant = S.F_Plant_cd collate Thai_CI_AS " +
                    $"WHERE F_Plant = '{plant}' " +
                    $"AND RTRIM(S.F_Name)+RTRIM(F_Store_cd) IN(Select Top 1 RTRIM(F_Name)+RTRIM(F_Store_cd) From {connectToPPM}.[dbo].[T_Supplier_ms] SM " +
                    $"Where RTRIM(D.F_Supplier_Code)+'-'+RTRIM(D.F_Supplier_Plant)=RTRIM(SM.F_supplier_cd)+'-'+RTRIM(SM.F_Plant_cd) collate Thai_CI_AS " +
                    $"AND RTRIM(SM.F_Store_cd) LIKE '{plant}%' " +
                    $"AND SM.F_TC_Str <= convert(char(8),getdate(),112) " +
                    $"AND SM.F_TC_End >= convert(char(8),getdate(),112)) " +
                    $"AND LEFT(D.F_Start_Date,6) <= '{DateTime.Now.ToString("yyyyMM")}' " +
                    $"AND LEFT(D.F_End_Date,6) >= '{DateTime.Now.ToString("yyyyMM")}' " +
                    $"GROUP BY D.F_Supplier_Code,D.F_Supplier_Plant " +
                    $",D.F_Cycle,D.F_Start_Date, D.F_End_Date,D.F_Delivery_Trip " +
                    $",D.F_Start_Order_Date,D.F_End_Order_Date, D.F_Delivery_Time ,S.F_short_name,S.F_name " +
                    $"ORDER BY F_Supplier_Code,F_Start_Date,F_End_Date,F_Delivery_Trip,F_Delivery_Time ";

                var dt = _FillDT.ExecuteSQL(sql);

                if (dt.Rows.Count == 0)
                {
                    return NotFound(new
                    {
                        status = "404",
                        response = "Not Found",
                        message = "No data found."
                    });
                }

                return Ok(new
                {
                    status = "200",
                    response = "Success",
                    message = "Data found.",
                    data = JsonConvert.SerializeObject(dt)
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500,new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = "Unexpected error occurred.",
                    error = ex.Message
                });
            }
        }

    }
}
