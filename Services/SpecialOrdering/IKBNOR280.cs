using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Services.Automapper.Interface;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;

namespace KANBAN.Services.SpecialOrdering
{
    public interface IKBNOR280
    {
        string GetPDSData(string FacCD, string DeliYM);
        Task Register(List<VM_Register_KBNOR280> listObj);
    }
    public class KBNOR280 : IKBNOR280
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _BearerClass;
        private readonly PPM3Context _PPM3Context;
        private readonly FillDataTable _FillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;
        private readonly ISpecialLibs _specialLibs;
        private readonly IAutoMapService _automapService;


        public KBNOR280
            (
            KB3Context kbContext,
            BearerClass BearerClass,
            PPM3Context PPM3Context,
            FillDataTable FillDT,
            SerilogLibs log,
            IEmailService emailService,
            ISpecialLibs specialLibs,
            IAutoMapService autoMapService
            )
        {
            _kbContext = kbContext;
            _BearerClass = BearerClass;
            _PPM3Context = PPM3Context;
            _FillDT = FillDT;
            _log = log;
            _emailService = emailService;
            _specialLibs = specialLibs;
            _automapService = autoMapService;
        }

        

        public string GetPDSData(string FacCD, string DeliYM)
        {
            try
            {
                string _sql = $@"Select RTrim(P.F_Supplier_Code) collate THAI_CI_AS +'-'+LTrim(P.F_Supplier_Plant) As F_Supp_CD, P.F_OrderNo, 
                    Convert(varchar(11),Cast(P.F_Delivery_Date As datetime),103) As F_Delivery_Date,
                    P.F_Delivery_Cycle As F_Delivery_Cycle ,P.F_Delivery_Trip 
                    From  TB_PDS_Header P inner join TB_Survey_Detail D 
                    On P.F_OrderNO collate THAI_CI_AS = D.F_PDS_No 
                    Where P.F_OrderNo like '{FacCD}%' 
                    and P.F_Delivery_Date like '{DeliYM}%' 
                    Group by P.F_Supplier_Code,P.F_Supplier_Plant,P.F_OrderNo, 
                    P.F_Delivery_Date,P.F_Delivery_Cycle,P.F_Delivery_Trip 
                    Order by P.F_OrderNO ";

                DataTable dt = _FillDT.ExecuteSQL(_sql);

                return JsonConvert.SerializeObject(dt);

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private void ChkSurveyDoc()
        {
            try
            {
                string _sql = $@"Select F_Status From TB_Survey_Header 
                    Where F_Factory_code  in ('{_BearerClass.Plant}') 
                    and F_Status not in ('N','D') and F_PDS_Flg = '0' ";

                DataTable dt = _FillDT.ExecuteSQL(_sql);

                if (dt.Rows.Count <= 0)
                {
                    throw new CustomHttpException(StatusCodes.Status404NotFound, "Not found survey data to generate pds.");
                }

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public async Task Register(List<VM_Register_KBNOR280> listObj)
        {
            try
            {
                foreach (var obj in listObj)
                {
                    string PDSNO = obj.F_OrderNo;
                    string Supplier = obj.F_Supp_CD;
                    string DeliDate = obj.F_Delivery_Date;
                    string trip = obj.F_Delivery_Trip.ToString();
                    string Supp_CD = Supplier.Split("-")[0];
                    string Supp_Plant = Supplier.Split("-")[1];

                    var listIns = _kbContext.TB_PDS_Header
                        .Where(x => x.F_OrderNo == PDSNO
                        && x.F_Delivery_Date == DateTime.ParseExact(DeliDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd")
                        && x.F_Supplier_Code == Supp_CD && x.F_Supplier_Plant == Supp_Plant
                        && x.F_Delivery_Trip == int.Parse(trip)).ToList();

                    if (listIns.Count <= 0)
                    {
                        throw new CustomHttpException(StatusCodes.Status404NotFound, "Not found data to register.");
                    }

                    foreach(var ins in listIns)
                    {
                        //var objRecHead = JsonConvert.DeserializeObject<TB_REC_HEADER>(JsonConvert.SerializeObject(ins));
                        var objRecHead = _automapService._IPDS_Header_Map_Rec_Header.MapTo(ins);
                        if (objRecHead.F_Transportor == null)
                        {
                            objRecHead.F_Transportor = "";
                        }
                        objRecHead.F_Plant = _BearerClass.Plant[0];
                        objRecHead.F_Flg_Epro = '9';
                        objRecHead.F_Status = 'C';

                        _kbContext.TB_REC_HEADER.Add(objRecHead);
                    }

                    var li

                }
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
