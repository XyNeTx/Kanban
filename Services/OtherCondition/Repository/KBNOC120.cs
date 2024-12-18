using HINOSystem.Context;
using HINOSystem.Libs;
using HINOSystem.Models.KB3.Master;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.OtherCondition.Model;
using KANBAN.Models.KB3.OtherCondition.ViewModel;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.OtherCondition.IRepository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TB_MS_PartOrder = HINOSystem.Models.KB3.Master.TB_MS_PartOrder;

namespace KANBAN.Services.OtherCondition.Repository
{
    public class KBNOC120 : IKBNOC120
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly IAutoMapService _automapService;


        public KBNOC120
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            IAutoMapService autoMapService
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _automapService = autoMapService;
        }

        public async Task<List<TB_MS_PartOrder>> GetSupplier(string? StoreCD)
        {
            try
            {
                string strDate = DateTime.Now.ToString("yyyyMMdd");

                var data = await _kbContext.TB_MS_PartOrder.Where(x=>x.F_Store_Code.StartsWith(_BearerClass.Plant)
                    && x.F_Start_Date.CompareTo(strDate) <= 0 && x.F_End_Date.CompareTo(strDate) >= 0)
                    .OrderBy(x=>x.F_Supplier_Cd.Trim() + "-" + x.F_Supplier_Plant.Trim()).ToListAsync();

                if(!string.IsNullOrEmpty(StoreCD))
                {
                    data = data.Where(x => x.F_Store_Code == StoreCD).ToList();
                }

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<T_Supplier_MS>> GetStore(string? SupplierCD)
        {
            try
            {
                string strDate = DateTime.Now.ToString("yyyyMMdd");

                var data = await _PPM3Context.T_Supplier_MS.Where(x => x.F_TC_Str.CompareTo(strDate) <= 0
                    && x.F_TC_End.CompareTo(strDate) >= 0)
                    .OrderBy(x => x.F_Store_cd).ToListAsync();

                if (!string.IsNullOrEmpty(SupplierCD))
                {
                    data = data.Where(x => x.F_supplier_cd + "-" + x.F_Plant_cd == SupplierCD).ToList();
                }

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task Save(List<VM_SAVE_KBNOC120> ListVMObj, string action)
        {
            try
            {

                var VMObj = ListVMObj.FirstOrDefault();

                TB_Slide_Order Obj = new TB_Slide_Order
                {
                    F_Delivery_Trip = byte.Parse(VMObj.Trip),
                    F_Delivery_Date = VMObj.DeliveryDate,
                    F_Supplier_CD = VMObj.Supplier.Split("-")[0],
                    F_Supplier_Plant = VMObj.Supplier.Split("-")[1],
                    F_Store_CD = VMObj.StoreCD,
                    F_Slide_Date = VMObj.SlideDateTo,
                    F_Slide_Trip = int.Parse(VMObj.TripNext),
                    F_Plant = _BearerClass.Plant,
                    F_Update_By = _BearerClass.UserCode,
                    F_Update_Date = DateTime.Now,
                    F_Keep_Order = VMObj.IsSlideOrder ? "1" : "0",
                };

                string strDate = DateTime.Now.ToString("yyyyMMdd");

                var DeliveryTimeObj = await _kbContext.TB_MS_DeliveryTime
                    .FirstOrDefaultAsync(x => x.F_Supplier_Code + "-" + x.F_Supplier_Plant == VMObj.Supplier
                    && x.F_Start_Date.CompareTo(strDate) <= 0 && x.F_End_Date.CompareTo(strDate) >= 0);

                if(DeliveryTimeObj == null)
                {
                    throw new CustomHttpException(404, "Supplier not found in Delivery Time Master");
                }

                int CycleB = int.Parse(DeliveryTimeObj.F_Cycle.Substring(2,2));

                if(int.Parse(VMObj.Trip) > CycleB)
                {
                    throw new CustomHttpException(400, "Trip > Cycle Time in Supplier MS");
                }

                if (VMObj.IsSlideOrder)
                {
                    var supMsObj = _PPM3Context.T_Supplier_MS.FirstOrDefault(x => x.F_supplier_cd + "-" + x.F_Plant_cd == VMObj.Supplier
                        && x.F_Store_cd == VMObj.StoreCD && x.F_TC_Str.CompareTo(strDate) <= 0 && x.F_TC_End.CompareTo(strDate) >= 0);

                    if(supMsObj == null)
                    {
                        throw new CustomHttpException(404, "Supplier not found in Supplier MS");
                    }

                    CycleB = int.Parse(supMsObj.F_Cycle_B);

                    if (int.Parse(VMObj.TripNext) > CycleB)
                    {
                        throw new CustomHttpException(400, "Trip Next > Cycle Time in Supplier MS");
                    }
                }

                if (strDate.CompareTo(VMObj.DeliveryDate) > 0)
                {
                    throw new CustomHttpException(400, "Delivery Date Not Less than Current Date!!!");
                }

                if (strDate.CompareTo(VMObj.SlideDateTo) > 0)
                {
                    throw new CustomHttpException(400, "Slide Date Not Less than Current Date!!!");
                }

                if (VMObj.DeliveryDate.CompareTo(VMObj.SlideDateTo) > 0)
                {
                    throw new CustomHttpException(400, "Please Check Delivery Date More than Slide Date");
                }

                if (action.ToLower() == "new")
                {
                    var IsExisted = await _kbContext.TB_Slide_Order.AnyAsync(x => x.F_Plant == _BearerClass.Plant
                        && x.F_Delivery_Date == VMObj.DeliveryDate
                        && x.F_Supplier_CD.Trim() + "-" + x.F_Supplier_Plant.Trim() == VMObj.Supplier
                        && x.F_Store_CD == VMObj.StoreCD
                        && x.F_Delivery_Trip == int.Parse(VMObj.Trip)
                        );

                    if(IsExisted)
                    {
                        throw new CustomHttpException(400, "Data Already Exist!!!");
                    }

                    await _kbContext.TB_Slide_Order.AddAsync(Obj);
                    _log.WriteLogMsg("Insert TB_Slice_Order | Obj : " + JsonConvert.SerializeObject(Obj));

                }

                else if (action.ToLower() == "upd")
                {
                    var dbObj = await _kbContext.TB_Slide_Order.FirstOrDefaultAsync(x => x.F_Plant == _BearerClass.Plant
                        && x.F_Delivery_Date == VMObj.DeliveryDate
                        && x.F_Supplier_CD.Trim() + "-" + x.F_Supplier_Plant.Trim() == VMObj.Supplier
                        && x.F_Store_CD == VMObj.StoreCD
                        && x.F_Delivery_Trip == int.Parse(VMObj.Trip));

                    if (dbObj == null)
                    {
                        throw new CustomHttpException(404, "Data to Update Not Found!!!");
                    }

                    dbObj.F_Keep_Order = VMObj.IsSlideOrder ? "1" : "0";
                    dbObj.F_Slide_Date = VMObj.IsSlideOrder ? VMObj.SlideDateTo : "";
                    dbObj.F_Slide_Trip = VMObj.IsSlideOrder ? int.Parse(VMObj.TripNext) : 0;
                    dbObj.F_Update_By = _BearerClass.UserCode;
                    dbObj.F_Update_Date = DateTime.Now;

                    _kbContext.TB_Slide_Order.Update(dbObj);

                    _log.WriteLogMsg("Update TB_Slice_Order | Obj : " + JsonConvert.SerializeObject(dbObj));


                }

                else
                {
                    foreach(var each in ListVMObj)
                    {
                        var DelData = await _kbContext.TB_Slide_Order.FirstOrDefaultAsync(x => x.F_Plant == _BearerClass.Plant
                            && x.F_Delivery_Date == each.DeliveryDate
                            && x.F_Supplier_CD.Trim() + "-" + x.F_Supplier_Plant.Trim() == each.Supplier
                            && x.F_Store_CD == each.StoreCD
                            && x.F_Delivery_Trip == int.Parse(each.Trip));

                        if (DelData == null)
                        {
                            throw new CustomHttpException(404, "Data to Delete Not Found!!!");
                        }

                        _kbContext.TB_Slide_Order.Remove(DelData);
                        //await _kbContext.SaveChangesAsync();
                        _log.WriteLogMsg("Delete TB_Slice_Order | Obj : " + JsonConvert.SerializeObject(DelData));

                    }

                }

                await _kbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                if (ex.InnerException != null) throw new CustomHttpException(500, ex.InnerException.Message);
                throw new CustomHttpException(500, ex.Message);
            }
        }

        public async Task<List<TB_Slide_Order>> GetListData(string? SupplierCD,string? StoreCD)
        {
            try
            {
                var data = await _kbContext.TB_Slide_Order.Where(x=>x.F_Plant == _BearerClass.Plant)
                    .OrderBy(x=>x.F_Plant)
                    .ThenBy(x=>x.F_Supplier_CD)
                    .ThenBy(x=>x.F_Supplier_Plant)
                    .ThenBy(x=>x.F_Store_CD)
                    .ThenBy(x=>x.F_Delivery_Date)
                    .ThenBy(x=>x.F_Delivery_Trip)
                    .ThenBy(x=>x.F_Keep_Order)
                    .ToListAsync();

                if(!string.IsNullOrWhiteSpace(SupplierCD))
                {
                    data = data.Where(x => x.F_Supplier_CD.Trim() + "-" + x.F_Supplier_Plant.Trim() == SupplierCD).ToList();
                }
                if (!string.IsNullOrWhiteSpace(StoreCD))
                {
                    data = data.Where(x => x.F_Store_CD == StoreCD).ToList();
                }

                return data;

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.Message);
            }
        }

    }
}
