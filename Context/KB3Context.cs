using HINOSystem.Models.KB3.Master;
using KANBAN.Models.KB3.Master;
using KANBAN.Models.KB3.OrderingProcess;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.KB3.ReportOrder;
using KANBAN.Models.KB3.UrgentOrder;
using KANBAN.Models.KB3.VLT;
using Microsoft.EntityFrameworkCore;

namespace HINOSystem.Context
{
    public class KB3Context : DbContext
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;

        public KB3Context(DbContextOptions<KB3Context> options, IHttpContextAccessor httpContextAccessor , IConfiguration configuration)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _config = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured && _httpContextAccessor.HttpContext != null)
            {
                var plantHeader = _httpContextAccessor.HttpContext.Request.Headers["Plant"].ToString();
                string connectionString = plantHeader switch
                {
                    "3" => _config.GetConnectionString("DefaultConnection"),
                    "2" => _config.GetConnectionString("KB2Connection"),
                    "1" => _config.GetConnectionString("KB1Connection"),
                    _ => _config.GetConnectionString("DefaultConnection")
                };

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public DbSet<TB_MS_CodeOrder> TB_MS_CodeOrder { get; set; }
        public DbSet<TB_MS_Company> TB_MS_Company { get; set; }
        public DbSet<TB_MS_CTL> TB_MS_CTL { get; set; }
        public DbSet<TB_MS_DeliveryTime> TB_MS_DeliveryTime { get; set; }
        public DbSet<TB_MS_Dock_Code> TB_MS_Dock_Code { get; set; }
        public DbSet<TB_MS_Factory> TB_MS_Factory { get; set; }
        public DbSet<TB_MS_Heijunka> TB_MS_Heijunka { get; set; }
        public DbSet<TB_MS_Inform_News> TB_MS_Inform_News { get; set; }
        public DbSet<TB_MS_Kanban> TB_MS_Kanban { get; set; }
        public DbSet<TB_MS_Label> TB_MS_Label { get; set; }
        public DbSet<TB_MS_LinecodeMSP> TB_MS_LinecodeMSP { get; set; }
        public DbSet<TB_MS_LPSupplier> TB_MS_LPSupplier { get; set; }
        public DbSet<TB_MS_Matching_Supplier> TB_MS_Matching_Supplier { get; set; }
        public DbSet<TB_MS_MAXAREA> TB_MS_MAXAREA { get; set; }
        public DbSet<TB_MS_MaxArea_Stock> TB_MS_MaxArea_Stock { get; set; }
        public DbSet<TB_MS_NEW_PART> TB_MS_NEW_PART { get; set; }
        public DbSet<TB_MS_OldPart> TB_MS_OldPart { get; set; }
        public DbSet<TB_MS_Operator> TB_MS_Operator { get; set; }
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
        public DbSet<KANBAN.Models.KB3.Receive_Process.TB_MS_PartOrder> TB_MS_PartOrder { get; set; }
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

        public DbSet<TB_Import_EKanban_Pack> TB_Import_EKanban_Pack { get; set; }

        public DbSet<TB_Import_Urgent> TB_Import_Urgent { get; set; }

        public DbSet<TB_Transaction_TMP> TB_Transaction_TMP { get; set; }
        public DbSet<TB_Import_VLT> TB_Import_VLT { get; set; }
        public DbSet<TB_Import_UpdMRP_FG> TB_Import_UpdMRP_FG { get; set; }

        public DbSet<TB_Calendar> TB_Calendar { get; set; }
        public DbSet<TB_BL_SET> TB_BL_SET { get; set; }
        public DbSet<TB_BL_SET_HISTORY_DELETE> TB_BL_SET_HISTORY_DELETE { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<PO>().ToTable("PO", schema: "iss");

        //    // Define primary key
        //    modelBuilder.Entity<PO>().HasKey(c => c._ID);

        //    // Configure other properties, relationships, etc.

        //    base.OnModelCreating(modelBuilder);



        //    ////// configures one-to-many relationship
        //    //modelBuilder.Entity<issPODetail>()
        //    //    .HasRequired<issPO>(s => s.CurrentGrade)
        //    //    .WithMany(g => g.issPODetail)
        //    //    .HasForeignKey<int>(s => s.CurrentGradeId);
        //}
    }
}

