using HINOSystem.Models.KB3.Master;
using KANBAN.Models.KB3.Login;
using KANBAN.Models.KB3.LogisticCondition;
using KANBAN.Models.KB3.Master;
using KANBAN.Models.KB3.OrderingProcess;
using KANBAN.Models.KB3.OtherCondition.Model;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.KB3.ReportOrder;
using KANBAN.Models.KB3.SpecialOrdering;
using KANBAN.Models.KB3.UrgentOrder;
using KANBAN.Models.KB3.VLT;
using Microsoft.EntityFrameworkCore;
using TB_MS_PartOrder = HINOSystem.Models.KB3.Master.TB_MS_PartOrder;

namespace HINOSystem.Context
{
    public class KB3Context : DbContext
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private static string plantDev = "";

        public KB3Context(DbContextOptions<KB3Context> options, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _config = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured && _httpContextAccessor.HttpContext != null)
            {
                var plantCookie = _httpContextAccessor.HttpContext.Request.Cookies["plantCode"];
                var isDev = _httpContextAccessor.HttpContext.Request.Cookies["isDev"];
                string strPlant = "";
                string strIsDev = "";
                if (plantCookie != null)
                {
                    strPlant = plantCookie.ToString();
                }
                if (isDev != null)
                {
                    strIsDev = isDev.ToString() == "1" ? "Dev" : "";
                }
                plantDev = strPlant + strIsDev;
                string connectionString = plantDev switch
                {
                    "3" => _config.GetConnectionString("DefaultConnection"),
                    "2" => _config.GetConnectionString("KB2Connection"),
                    "1" => _config.GetConnectionString("KB1Connection"),
                    "3Dev" => _config.GetConnectionString("DevDefaultConnection"),
                    "2Dev" => _config.GetConnectionString("DevKB2Connection"),
                    "1Dev" => _config.GetConnectionString("DevKB1Connection"),
                    "Dev" => _config.GetConnectionString("DevDefaultConnection"),
                    _ => _config.GetConnectionString("DefaultConnection")
                };

                optionsBuilder.UseSqlServer(connectionString, option =>
                    option.CommandTimeout(600)
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserErp>().ToTable("User", "erp");
            modelBuilder.Entity<UserAuthorize>().ToTable("UserAuthorize", "erp");
            modelBuilder.Entity<Menu>().ToTable("Menu", "erp");

            // Add additional configuration here if needed
        }

        public DbSet<TB_MS_CodeOrder> TB_MS_CodeOrder { get; set; }
        public DbSet<UserErp> User { get; set; }
        public DbSet<TB_MS_Company> TB_MS_Company { get; set; }
        public DbSet<TB_MS_CTL> TB_MS_CTL { get; set; }
        public DbSet<TB_MS_DeliveryTime> TB_MS_DeliveryTime { get; set; }
        public DbSet<TB_MS_Dock_Code> TB_MS_Dock_Code { get; set; }
        public DbSet<TB_MS_Factory> TB_MS_Factory { get; set; }
        public DbSet<TB_MS_Heijunka> TB_MS_Heijunka { get; set; }
        //public DbSet<TB_MS_Inform_News> TB_MS_Inform_News { get; set; }
        public DbSet<TB_MS_Kanban> TB_MS_Kanban { get; set; }
        public DbSet<TB_MS_Label> TB_MS_Label { get; set; }
        public DbSet<TB_MS_LinecodeMSP> TB_MS_LinecodeMSP { get; set; }
        public DbSet<TB_MS_LPSupplier> TB_MS_LPSupplier { get; set; }
        public DbSet<TB_MS_Matching_Supplier> TB_MS_Matching_Supplier { get; set; }
        public DbSet<TB_MS_MAXAREA> TB_MS_MAXAREA { get; set; }
        public DbSet<TB_MS_MaxArea_Stock> TB_MS_MaxArea_Stock { get; set; }
        public DbSet<TB_MS_NEW_PART> TB_MS_NEW_PART { get; set; }
        public DbSet<TB_MS_OldPart> TB_MS_OldPart { get; set; }
        //public DbSet<Models.KB3.Master.TB_MS_Operator> TB_MS_Operator { get; set; }
        public DbSet<TB_MS_OrderType> TB_MS_OrderType { get; set; }
        public DbSet<TB_MS_Package> TB_MS_Package { get; set; }
        public DbSet<TB_MS_PairOrder> TB_MS_PairOrder { get; set; }
        public DbSet<TB_MS_Parameter> TB_MS_Parameter { get; set; }
        public DbSet<TB_MS_PartCode> TB_MS_PartCode { get; set; }
        public DbSet<TB_MS_PartCurrent> TB_MS_PartCurrent { get; set; }
        //public DbSet<Models.KB3.Master.TB_MS_PartOrder> TB_MS_PartOrder { get; set; } = null!;
        public DbSet<TB_MS_PartPackage> TB_MS_PartPackage { get; set; }
        public DbSet<TB_MS_PartSet> TB_MS_PartSet { get; set; }
        public DbSet<TB_MS_PartSpecial> TB_MS_PartSpecial { get; set; }
        public DbSet<TB_MS_Print_Replace_KB> TB_MS_Print_Replace_KB { get; set; }
        public DbSet<TB_MS_PrintKanban> TB_MS_PrintKanban { get; set; }
        public DbSet<TB_MS_RatioAddress> TB_MS_RatioAddress { get; set; }
        public DbSet<TB_MS_Remark_DocSheet> TB_MS_Remark_DocSheet { get; set; }
        public DbSet<TB_MS_Route> TB_MS_Route { get; set; }
        public DbSet<TB_MS_Route_Delivery> TB_MS_Route_Delivery { get; set; }
        public DbSet<TB_MS_SendMail> TB_MS_SendMail { get; set; }

        // public DbSet<TB_MS_SpcApprover> TB_MS_SpcApprover { get; set; } = null!;
        public DbSet<TB_MS_SupplierAttn> TB_MS_SupplierAttn { get; set; }
        public DbSet<TB_MS_TagColor> TB_MS_TagColor { get; set; }
        public DbSet<TB_MS_TruckType> TB_MS_TruckType { get; set; }
        //public DbSet<TB_MS_VLT_Customer> TB_MS_VLT_Customer { get; set; } = null!;
        public DbSet<TB_MS_ZeroOrder> TB_MS_ZeroOrder { get; set; }
        public DbSet<TB_REC_HEADER> TB_REC_HEADER { get; set; }
        public DbSet<TB_REC_DETAIL> TB_REC_DETAIL { get; set; }
        public DbSet<VW_KBNRC_220_RPT> VW_KBNRC_220_RPT { get; set; }
        public DbSet<TB_MS_PartOrder> TB_MS_PartOrder { get; set; }
        public DbSet<TB_Import_Delivery> TB_Import_Delivery { get; set; }
        public DbSet<V_KBNRT_130> V_KBNRT_130_rpt { get; set; }
        public DbSet<V_KBNRT_140> V_KBNRT_140_rpt { get; set; }
        public DbSet<RPT_KBNRT_110> RPT_KBNRT_110 { get; set; }
        public DbSet<V_KBNRT_110_rpt> V_KBNRT_110_rpt { get; set; }
        public DbSet<TB_Transaction> TB_Transaction { get; set; }
        public DbSet<V_KBNRT_180_rpt> V_KBNRT_180_Rpt { get; set; }
        public DbSet<V_KBNRT_190_rpt> V_KBNRT_190_Rpt { get; set; }
        public DbSet<V_KBNRT_210_rpt_Dev> V_KBNRT_210_rpt_Dev { get; set; }
        public DbSet<TB_Inquriy_KB_rpt_TMP> TB_Inquriy_KB_rpt_TMP { get; set; }
        public DbSet<TB_Import_Forecast> TB_Import_Forecast { get; set; }
        public DbSet<TB_Late_Deli_Rpt_TMP> TB_Late_Deli_Rpt_TMP { get; set; }
        public DbSet<V_KBNRT_220_rpt> V_KBNRT_220_rpt { get; set; }
        public DbSet<TB_BL> TB_BL { get; set; }

        public DbSet<KANBAN.Models.KB3.ReportOrder.TB_MS_VLT_Customer> TB_MS_VLT_Customer { get; set; }
        public DbSet<TB_VLT_INTERFACE> TB_VLT_INTERFACE { get; set; }

        public DbSet<TB_Import_Service> TB_Import_Service { get; set; }
        public DbSet<TB_Import_Service_Excel> TB_Import_Service_Excel { get; set; }

        public DbSet<TB_Import_EKanban_Pack> TB_Import_EKanban_Pack { get; set; }

        public DbSet<TB_Import_Urgent> TB_Import_Urgent { get; set; }

        public DbSet<TB_Transaction_TMP> TB_Transaction_TMP { get; set; }
        public DbSet<TB_Import_VLT> TB_Import_VLT { get; set; }
        public DbSet<TB_Import_UpdMRP_FG> TB_Import_UpdMRP_FG { get; set; }

        public DbSet<TB_Calendar> TB_Calendar { get; set; }
        public DbSet<TB_BL_SET> TB_BL_SET { get; set; }
        public DbSet<TB_BL_SET_HISTORY_DELETE> TB_BL_SET_HISTORY_DELETE { get; set; }
        public DbSet<TB_Calculate_D> TB_Calculate_D { get; set; }
        public DbSet<KANBAN.Models.KB3.OrderingProcess.TB_MS_Inform_News> TB_MS_Inform_News { get; set; }
        public DbSet<TB_Kanban_Chg_Qty> TB_Kanban_Chg_Qty { get; set; }
        public DbSet<TB_Kanban_Stop> TB_Kanban_Stop { get; set; }
        public DbSet<TB_Kanban_Cut> TB_Kanban_Cut { get; set; }
        public DbSet<TB_Kanban_Add> TB_Kanban_Add { get; set; }
        public DbSet<TMP_Planning_Order> TMP_Planning_Order { get; set; }
        public DbSet<TB_Kanban_Planning> TB_Kanban_Planning { get; set; }
        public DbSet<KBNLC_150> KBNLC_150 { get; set; }
        public DbSet<TB_Transaction_Spc> TB_Transaction_Spc { get; set; }
        public DbSet<UserAuthorize> UserAuthorize { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<TB_STOCK_KB_SPC_PART_REMAIN> TB_STOCK_KB_SPC_PART_REMAIN { get; set; }
        public DbSet<KANBAN.Models.KB3.SpecialOrdering.TB_MS_Operator> TB_MS_Operator { get; set; }
        public DbSet<TB_Survey_Header> TB_Survey_Header { get; set; }
        public DbSet<TB_Survey_Detail> TB_Survey_Detail { get; set; }
        public DbSet<TB_PDS_Header> TB_PDS_Header { get; set; }
        public DbSet<TB_PDS_Detail> TB_PDS_Detail { get; set; }
        public DbSet<TB_Slide_Order> TB_Slide_Order { get; set; }
        public DbSet<TB_Slide_Order_Part> TB_Slide_Order_Part { get; set; }
        public DbSet<TB_MS_Print_Replace_KB_TMP> TB_MS_Print_Replace_KB_TMP { get; set; }
        public DbSet<TB_MS_LineControl> TB_MS_LineControl { get; set; }
        public virtual DbSet<TB_Import_VHD> TB_Import_VHD { get; set; }
    }
}

