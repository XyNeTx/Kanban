using Microsoft.EntityFrameworkCore;

namespace KANBAN.Models.PPM;

[PrimaryKey("F_Declare_No", "F_Pds_No", "F_Part_No","F_ID","F_Parent_Part_No")]
public class T_TPCAP_Delivery_List_Declare
{
    public string F_Declare_No { get; set; }
    public string F_Pds_No { get; set; }
    public string F_Part_No { get; set; }
    public string F_KB_No { get; set; }
    public string F_Name { get; set; }
    public int F_Qty { get; set; }
    public int F_Inhouse { get; set; }
    public int F_Part_Set { get; set; }
    public int F_Runout { get; set; }
    public string F_Delivery_Date { get; set; }
    public string F_Remark { get; set; }
    public int F_ID { get; set; }
    public string F_Flag { get; set; }
    public string F_Biz { get; set; }
    public string F_ReferDeclare_No { get; set; }
    public string F_Update_By { get; set; }
    public DateTime F_Update_Date { get; set; }
    public string F_PO_Date { get; set; }
    public string F_Flag_Declare { get; set; }
    public string F_Privilege { get; set; }
    public int F_Privilege_RemainQty { get; set; }
    public string F_Parent_Part_No { get; set; }
    public int F_PO_Qty { get; set; }
}
