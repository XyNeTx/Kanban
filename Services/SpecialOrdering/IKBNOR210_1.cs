using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.SpecialOrdering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace KANBAN.Services.SpecialOrdering
{
    public interface IKBNOR210_1
    {
        Task<List<TB_Transaction_Spc>> LoadDataChangeDelivery(string? PDSNo, string? SuppCd, string? PartNo, bool? chkDeli, string? DeliFrom, string? DeliTo);
        Task<string> GetSupplierName(string SuppCd);
        Task<string> GetPartName(string PartNo);


    }

    public class KBNOR210_1 : IKBNOR210_1
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;


        public KBNOR210_1
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
        }

        public async Task<List<TB_Transaction_Spc>> LoadDataChangeDelivery(string? PDSNo,string? SuppCd,string? PartNo,bool? chkDeli,string? DeliFrom,string? DeliTo)
        {
            try
            {
                var data = _kbContext.TB_Transaction_Spc.Where(x=>x.F_Qty != 0
                && x.F_Delivery_Date_New == "" && x.F_Survey_Flg == "0"
                && x.F_PDS_No != "").AsQueryable();

                if(!string.IsNullOrWhiteSpace(PDSNo))
                {
                    data = data.Where(x=>x.F_PDS_No == PDSNo);
                }
                if(!string.IsNullOrWhiteSpace(SuppCd))
                {
                    data = data.Where(x=>x.F_Supplier_CD.Trim() + "-" + x.F_Supplier_Plant.Trim() == SuppCd);
                }
                if (!string.IsNullOrWhiteSpace(PartNo))
                {
                    data = data.Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == PartNo);
                }
                if(chkDeli == true)
                {
                    if(!string.IsNullOrWhiteSpace(DeliFrom) && !string.IsNullOrWhiteSpace(DeliTo))
                    {
                        data = data.Where(x=>x.F_Delivery_Date_New.CompareTo(DeliFrom) >= 0 && x.F_Delivery_Date_New.CompareTo(DeliTo) <= 0);
                    }
                }

                if(data.Count() == 0)
                {
                    throw new Exception("Data not found");
                }

                return await data.ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GetSupplierName(string SuppCd)
        {
            try
            {
                string SupplierName = await _kbContext.Database.SqlQueryRaw<string>
                    ("Select Top 1 Rtrim(F_name) As Value From V_Supplier_ms Where F_supplier_cd + '-' + F_Plant_cd = {0}", SuppCd)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrWhiteSpace(SupplierName))
                {
                    throw new Exception("Supplier Name not found");
                }

                return SupplierName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GetPartName(string PartNo)
        {
            try
            {

                string PartName = await _kbContext.TB_Transaction_Spc
                    .Where(x => x.F_Part_No.Trim() + "-" + x.F_Ruibetsu == PartNo)
                    .Select(x => x.F_Part_Name).FirstOrDefaultAsync();

                if (string.IsNullOrWhiteSpace(PartName))
                {
                    throw new Exception("Part Name not found");
                }

                return PartName;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task GetPOMergeData(string? PDSNo, string? SuppCd, string? PartNo, bool? chkDeli, string? DeliFrom, string? DeliTo)
        {
            try
            {
                await _kbContext.Database.ExecuteSqlRawAsync("Delete From TB_PO_Merge_Tmp Where F_Rec_user = @p0",
                    new SqlParameter("@p0", _BearerClass.UserCode));



            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
