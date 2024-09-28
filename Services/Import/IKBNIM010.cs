using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace KANBAN.Services.Import
{
    public interface IKBNIM010
    {
        bool Check_Holiday(string date, string shift);
        List<string> ListData(string date, string shift);
        Task<bool> Confirm(string date, string shift);
    }

    public class KBNIM010 : IKBNIM010
    {
        private readonly KB3Context _kbContext;
        private readonly BearerClass _bearerClass;
        private readonly PPM3Context _ppm3Context;
        private readonly FillDataTable _fillDT;
        private readonly SerilogLibs _log;
        private readonly IEmailService _emailService;


        public KBNIM010
            (
            KB3Context kbContext,
            BearerClass bearerClass,
            PPM3Context ppm3Context,
            FillDataTable fillDT,
            SerilogLibs log,
            IEmailService emailService
            )
        {
            _kbContext = kbContext;
            _bearerClass = bearerClass;
            _ppm3Context = ppm3Context;
            _fillDT = fillDT;
            _log = log;
            _emailService = emailService;
        }

        public bool Check_Holiday(string date, string shfit)
        { 

            string sql = $"Select F_WorkCD_{(shfit[0])}" +
                $"{int.Parse(date.Substring(0, 2))} from TB_Calendar Where F_YM='{date.Substring(6, 4) + date.Substring(3, 2)}' " +
                $"and F_Store_Cd='{_bearerClass.StoreAccess()}'";

            var dt = _fillDT.ExecuteSQL(sql);

            if (dt.Rows[0].ItemArray[0].ToString() == "1")
            {
                return false;
            }

            return true;
        }

        public List<string> ListData(string date,string shift)
        {
            List<string> returnList = new List<string>();

            string _sql = "Select * from dbo.FN_GETDATA_BF_CONFIRM" +
                $"({date.Substring(6, 4) + date.Substring(3, 2) + date.Substring(0, 2)} " +
                $",'{(shift.Substring(0, 1))}') Order by F_NO,F_TYPE";

            var dt = _fillDT.ExecuteSQL(_sql);

            if(dt.Rows.Count == 0)
            {
                throw new Exception("No Data");
            }

            string obj = JsonConvert.SerializeObject(dt);

            returnList.Add(obj);

            _sql = "Select * from dbo.FN_GETDATA_CONFIRM" +
                $"({date.Substring(6, 4) + date.Substring(3, 2) + date.Substring(0, 2)} " +
                $",'{(shift.Substring(0, 1))}') Order by F_NO,F_TYPE";

            var dt2 = _fillDT.ExecuteSQL(_sql);
            if (dt2.Rows.Count == 0)
            {
                throw new Exception("No Data");
            }

            string obj2 = JsonConvert.SerializeObject(dt2);

            returnList.Add(obj2);

            return returnList;
        }

        public async Task<bool> Confirm(string date,string shift)
        {
            try
            {

                await _kbContext.Database.BeginTransactionAsync();

                int rowAff = await _kbContext.Database.ExecuteSqlRawAsync("SELECT F_Code, F_Value2  FROM TB_MS_Parameter  WHERE  F_Code = N'CI' AND F_Value2>'0' ");
                if (rowAff > 0)
                {
                    throw new Exception("ระบบไม่สามารถยืนยันการนำเข้าข้อมูลทั้งหมดได้ เนื่องจากมีการยืนยันการนำเข้าข้อมูลทั้งหมดแล้ว!!");
                }

                _log.WriteLogMsg("KBNIM010 | Confirm Import Data | Start Confirm All Import Data");

                await _kbContext.Database.ExecuteSqlRawAsync($"Update TB_MS_Parameter " +
                    $"Set F_Value2='1',F_Update_Date=getdate(), F_Update_By='{_bearerClass.UserCode}' " +
                    $",F_Value3= '{date.Substring(6, 4) + date.Substring(3, 2) + date.Substring(0, 2) + shift}' " +
                    $" Where F_Code = N'CI' AND F_Value2='0' ");

                await _kbContext.Database.ExecuteSqlRawAsync($"Update TB_MS_Parameter " +
                    $"Set F_Value2='1',F_Update_Date=getdate(), F_Update_By='{_bearerClass.UserCode}' " +
                    $",F_Value3= '{date.Substring(6, 4) + date.Substring(3, 2) + date.Substring(0, 2) + shift}' " +
                    $" Where F_Code = N'CI_CKD' AND F_Value2='0' ");

                _log.WriteLogMsg("KBNIM010 | Confirm All Update from TB_MS_Parameter Set F_Value2='1' | " +
                    "Update TB_MS_Parameter " +
                    $"Set F_Value2='1',F_Update_Date=getdate(), F_Update_By='{_bearerClass.UserCode}' " +
                    $",F_Value3= '{date.Substring(6, 4) + date.Substring(3, 2) + date.Substring(0, 2) + shift}' " +
                    $" Where F_Code = N'CI' AND F_Value2='0' ");

                await _kbContext.Database.CommitTransactionAsync();

                return true;
            }
            catch (Exception)
            {
                await _kbContext.Database.RollbackTransactionAsync();
                throw new Exception("Confirm Import Data Error");
            }
        }
    }
}