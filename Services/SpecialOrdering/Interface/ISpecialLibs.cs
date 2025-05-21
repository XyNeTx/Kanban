using System.Data;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface ISpecialLibs
    {
        DataTable GetSupCodeSPC(string PDSNo, string PDSDate, string StoreCD);
        DataTable GetIssueBy();
        int getMaxSurveyID(string PDSNo, string Fac = "");
        string FormatNumber(int Number, int Digits = 3);
        string GetDelayDate(string IssDate);
        DataTable GetTransactionSPCDetail(string PDSNo, string PDSDate, string SuppCD, string SuppPlant, string Fac, string? DeliDT = "", string? StoreCD = "");
        DataTable GetSurveyHeader(string Fac, string? SurveyDoc, string? UploadFlg, string? Mode);
        string GetPOSurvey(string? PO = "", string? Fac = "", string? Flag = "", string? DeliDate = "", string? select = "");
        Task<string> GetSupplierName(string SuppCD, string SuppPlant);
        DataTable GetSurveyHeaderUpload(string? Status = "", string? Fac = "", string? SurveyDoc = "", string? UploadFlg = "");
        string CheckPriceFlag(string SurveyDoc);
        DataTable GetStatusSurveyHeader(string? SurveyDoc = "");
        DataTable GetSurveyHeaderDownload();
        string GetPDSNo(string PlantType, string Fac, string DeliDate, string Running,
                                string DeliTrip, string StoreCD, string FormatPDS);
        int GetMaxRegRunning(string PlantType, string SuppCD, string SuppPlant,
            string Fac, string? DeliYM = "", string? DeliTrip = "", string? StoreCD = "",
            string? FormatPDS = "");

        bool ChkPDSData(string PlantType, string Fac, string DeliYM, string DeliTrip, string StoreCD, string? FormatPDS = "");

        int GetMaxPDSRunning(string PlantType, string SuppCD, string SuppPlant,
                            string Fac, string? DeliYM = "", string? DeliTrip = "", string? StoreCD = "",
                            string? FormatPDS = "");

        Task<string> getCycleTime(string Fac, string SuppCD, string SuppPlant, string DeliDT, string DeliTrip);
        string GetDockCode(string Supp_CD, string Supp_Plant, string DeliDate, string Trip);

        string GetDeliveryTime(string SuppCD, string SuppPlant, string Cycle, string DeliDT, string DeliTrip);
        string getVat(string Fac);
    }
}
