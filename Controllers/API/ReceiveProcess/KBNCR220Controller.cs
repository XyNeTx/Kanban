using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NPOI.SS.Formula.Functions;
using System.Globalization;

namespace KANBAN.Controllers.API.ReceiveProcess
{
    public class KBNCR220Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BearerClass _BearerClass;
        private readonly ActionResultClass _ActionResult;
        private readonly KanbanConnection _KBCN;
        private readonly PPMConnect _PPMConnect;
        private readonly PPM3Context _PPM3Context;
        private readonly PPMContext _PPMContext;
        private readonly KB3Context _KB3Context;


        private readonly string StoragePath = @"wwwroot\Storage\Uploads";

        public KBNCR220Controller(
            IConfiguration configuration,
            BearerClass bearerClass,
            ActionResultClass actionResultClass,
            KanbanConnection kanbanConnection,
            PPMConnect ppmConnect,
            PPMContext pPMContext,
            PPM3Context pPM3Context,
            KB3Context kB3Context
            )
        {
            _configuration = configuration;
            _BearerClass = bearerClass;
            _ActionResult = actionResultClass;
            _KB3Context = kB3Context;
            _KBCN = kanbanConnection;
            _PPMConnect = ppmConnect;
            _PPM3Context = pPM3Context;
            _PPMContext = pPMContext;
        }

        public async Task<IActionResult> Initial([FromBody] string data)
        {
            try
            {
                Int32 now = Int32.Parse(DateTime.Now.ToString("yyyyMMdd"));
                if (data == null)
                {
                    var _supplierHead = await _KB3Context.TB_REC_HEADER.Select(x => new
                    {
                        x.F_OrderNo,
                        x.F_Supplier_Code,
                        x.F_Supplier_Plant
                    }).Where(x => !x.F_OrderNo.StartsWith("7Y") || !x.F_OrderNo.StartsWith("7Z"))
                    .OrderBy(x => x.F_Supplier_Code).ToListAsync();

                    var supplierHead = _supplierHead.DistinctBy(x => new { x.F_Supplier_Code, x.F_Supplier_Plant });

                    List<string> supplierList = new();

                    foreach (var sup in supplierHead)
                    {
                        var T_Supplier = await _PPM3Context.T_Supplier_MS.Select(x => new
                        {
                            x.F_supplier_cd,
                            x.F_Plant_cd,
                            x.F_short_name,
                            x.F_TC_Str,
                            x.F_TC_End
                        }).Where(x => x.F_supplier_cd == sup.F_Supplier_Code && x.F_Plant_cd == sup.F_Supplier_Plant)
                        .OrderBy(x => x.F_supplier_cd).ToListAsync(); //Int32.Parse(x.F_TC_Str) <= now && now >= Int32.Parse(x.F_TC_End) &&
                        
                        var distincList = T_Supplier.DistinctBy(x => new
                        {
                            x.F_supplier_cd,
                            x.F_Plant_cd,
                        }).Where(x => Int32.Parse(x.F_TC_Str) <= now && now >= Int32.Parse(x.F_TC_End));
                        
                        if (T_Supplier.Count > 0)
                        {
                            foreach (var t in distincList)
                            {
                                var _supplier = t.F_supplier_cd + '-' + t.F_Plant_cd + " : " + t.F_short_name;
                                supplierList.Add(_supplier);
                            }
                        }
                    }

                    return Ok(supplierList);
                }
                else if (data.ToString() == "7Z")
                {
                    var _supplierHead = await _KB3Context.TB_REC_HEADER.Select(x => new
                    {
                        x.F_OrderNo,
                        x.F_Supplier_Code,
                        x.F_Supplier_Plant
                    }).Where(x => x.F_OrderNo.StartsWith("7Z"))
                    .OrderBy(x => x.F_Supplier_Code).ToListAsync();

                    var supplierHead = _supplierHead.DistinctBy(x => new { x.F_Supplier_Code, x.F_Supplier_Plant });

                    List<string> supplierList = new();

                    foreach (var sup in supplierHead)
                    {
                        var T_Supplier = await _PPM3Context.T_Supplier_MS.Select(x => new
                        {
                            x.F_supplier_cd,
                            x.F_Plant_cd,
                            x.F_short_name,
                            x.F_TC_Str,
                            x.F_TC_End
                        }).Where(x => x.F_supplier_cd == sup.F_Supplier_Code && x.F_Plant_cd == sup.F_Supplier_Plant)
                        .OrderBy(x => x.F_supplier_cd).ToListAsync(); //Int32.Parse(x.F_TC_Str) <= now && now >= Int32.Parse(x.F_TC_End) &&
                        
                        var distincList = T_Supplier.DistinctBy(x => new
                        {
                            x.F_supplier_cd,
                            x.F_Plant_cd,
                        }).Where(x => Int32.Parse(x.F_TC_Str) <= now && now >= Int32.Parse(x.F_TC_End));
                        
                        if (T_Supplier.Count > 0)
                        {
                            foreach (var t in distincList)
                            {
                                var _supplier = t.F_supplier_cd + '-' + t.F_Plant_cd + " : " + t.F_short_name;
                                supplierList.Add(_supplier);
                            }
                        }
                    }

                    return Ok(supplierList);
                }
                else
                {
                    var _supplierHead = await _KB3Context.TB_REC_HEADER.Select(x => new
                    {
                        x.F_OrderNo,
                        x.F_Supplier_Code,
                        x.F_Supplier_Plant
                    }).Where(x => x.F_OrderNo.StartsWith("7Y"))
                    .OrderBy(x => x.F_Supplier_Code).ToListAsync();

                    var supplierHead = _supplierHead.DistinctBy(x => new { x.F_Supplier_Code, x.F_Supplier_Plant });

                    List<string> supplierList = new();

                    foreach (var sup in supplierHead)
                    {
                        var T_Supplier = await _PPM3Context.T_Supplier_MS.Select(x => new
                        {
                            x.F_supplier_cd,
                            x.F_Plant_cd,
                            x.F_short_name,
                            x.F_TC_Str,
                            x.F_TC_End
                        }).Where(x => x.F_supplier_cd == sup.F_Supplier_Code && x.F_Plant_cd == sup.F_Supplier_Plant)
                        .OrderBy(x => x.F_supplier_cd).ToListAsync();
                        var distincList = T_Supplier.DistinctBy(x => new
                        {
                            x.F_supplier_cd,
                            x.F_Plant_cd,
                        }).Where(x => Int32.Parse(x.F_TC_Str) <= now && now >= Int32.Parse(x.F_TC_End));
                        if (T_Supplier.Count > 0)
                        {
                            foreach (var t in distincList)
                            {
                                var _supplier = t.F_supplier_cd + '-' + t.F_Plant_cd + " : " + t.F_short_name;
                                supplierList.Add(_supplier);
                            }
                        }
                    }

                    return Ok(supplierList);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

    }
}
