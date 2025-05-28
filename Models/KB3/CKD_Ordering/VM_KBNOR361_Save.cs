namespace KANBAN.Models.KB3.CKD_Ordering
{
    public class VM_KBNOR361_Save
    {
        public bool F_Flg_ClearModule { get; set; } = false;
        public string F_Supplier_Code { get; set; }
        public string F_Part_No { get; set; }
        public string F_Kanban_No { get; set; }
        public string F_Store_Code { get; set; }
        public string F_Plant { get; set; }
    }
}
