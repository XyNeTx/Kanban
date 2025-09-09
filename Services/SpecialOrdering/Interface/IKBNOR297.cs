using System.Data;

namespace KANBAN.Services.SpecialOrdering.Interface
{
    public interface IKBNOR297
    {
        string GenReportExcel(string CustOrderNo);
        string GetCustomerPO(string? DeliYM);
        DataTable GetListAllPDSDetail(string? CustOrderNo);
    }

}
