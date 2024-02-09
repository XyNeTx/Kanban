using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HINOSystem.Models.KB3.Master;
using KANBAN.Models.KB3.Receive_Process;
using KANBAN.Models.KB3.ReportOrder;

namespace HINOSystem.Context
{
    public class KB3Context : DbContext
    {
        public KB3Context(DbContextOptions<KB3Context> options)
            : base(options)
        { }

        public DbSet<TB_MS_CodeOrder> TB_MS_CodeOrder { get; set; } = null!;
        public DbSet<TB_MS_Company> TB_MS_Company { get; set; } = null!;
        public DbSet<TB_MS_CTL> TB_MS_CTL { get; set; } = null!;
        public DbSet<TB_MS_DeliveryTime> TB_MS_DeliveryTime { get; set; } = null!;
        public DbSet<TB_MS_Dock_Code> TB_MS_Dock_Code { get; set; } = null!;
        public DbSet<TB_MS_Factory> TB_MS_Factory { get; set; } = null!;
        public DbSet<TB_MS_Heijunka> TB_MS_Heijunka { get; set; } = null!;
        public DbSet<TB_MS_Inform_News> TB_MS_Inform_News { get; set; } = null!;
        public DbSet<TB_MS_Kanban> TB_MS_Kanban { get; set; } = null!;
        public DbSet<TB_MS_Label> TB_MS_Label { get; set; } = null!;
        public DbSet<TB_MS_LinecodeMSP> TB_MS_LinecodeMSP { get; set; } = null!;
        public DbSet<TB_MS_LPSupplier> TB_MS_LPSupplier { get; set; } = null!;
        public DbSet<TB_MS_Matching_Supplier> TB_MS_Matching_Supplier { get; set; } = null!;
        public DbSet<TB_MS_MAXAREA> TB_MS_MAXAREA { get; set; } = null!;
        public DbSet<TB_MS_MaxArea_Stock> TB_MS_MaxArea_Stock { get; set; } = null!;
        public DbSet<TB_MS_NEW_PART> TB_MS_NEW_PART { get; set; } = null!;
        public DbSet<TB_MS_OldPart> TB_MS_OldPart { get; set; } = null!;
        public DbSet<TB_MS_Operator> TB_MS_Operator { get; set; } = null!;
        public DbSet<TB_MS_OrderType> TB_MS_OrderType { get; set; } = null!;
        public DbSet<TB_MS_Package> TB_MS_Package { get; set; } = null!;
        public DbSet<TB_MS_PairOrder> TB_MS_PairOrder { get; set; } = null!;
        public DbSet<TB_MS_Parameter> TB_MS_Parameter { get; set; } = null!;
        public DbSet<TB_MS_PartCode> TB_MS_PartCode { get; set; } = null!;
        public DbSet<TB_MS_PartCurrent> TB_MS_PartCurrent { get; set; } = null!;
        //public DbSet<Models.KB3.Master.TB_MS_PartOrder> TB_MS_PartOrder { get; set; } = null!;
        public DbSet<TB_MS_PartPackage> TB_MS_PartPackage { get; set; } = null!;
        public DbSet<TB_MS_PartSet> TB_MS_PartSet { get; set; } = null!;
        public DbSet<TB_MS_PartSpecial> TB_MS_PartSpecial { get; set; } = null!;
        public DbSet<TB_MS_Print_Replace_KB> TB_MS_Print_Replace_KB { get; set; } = null!;
        public DbSet<TB_MS_PrintKanban> TB_MS_PrintKanban { get; set; } = null!;
        public DbSet<TB_MS_RatioAddress> TB_MS_RatioAddress { get; set; } = null!;
        public DbSet<TB_MS_Remark_DocSheet> TB_MS_Remark_DocSheet { get; set; } = null!;
        public DbSet<TB_MS_Route> TB_MS_Route { get; set; } = null!;
        public DbSet<TB_MS_Route_Delivery> TB_MS_Route_Delivery { get; set; } = null!;
        public DbSet<TB_MS_SendMail> TB_MS_SendMail { get; set; } = null!;

        // public DbSet<TB_MS_SpcApprover> TB_MS_SpcApprover { get; set; } = null!;
        public DbSet<TB_MS_SupplierAttn> TB_MS_SupplierAttn { get; set; } = null!;
        public DbSet<TB_MS_TagColor> TB_MS_TagColor { get; set; } = null!;
        public DbSet<TB_MS_TruckType> TB_MS_TruckType { get; set; } = null!;
        public DbSet<TB_MS_VLT_Customer> TB_MS_VLT_Customer { get; set; } = null!;
        public DbSet<TB_MS_ZeroOrder> TB_MS_ZeroOrder { get; set; } = null!;
        public DbSet<TB_REC_HEADER> TB_REC_HEADER { get; set; }
        public DbSet<TB_REC_DETAIL> TB_REC_DETAIL { get; set; }
        public DbSet<VW_KBNRC_220_RPT> VW_KBNRC_220_RPT { get; set; }
        public DbSet<KANBAN.Models.KB3.Receive_Process.TB_MS_PartOrder> TB_MS_PartOrder { get; set; }
        public DbSet<TB_Import_Delivery> TB_Import_Delivery { get; set; }
        public DbSet<V_KBNRT_130> V_KBNRT_130_rpt { get; set; }
        public DbSet<RPT_KBNRT_110> RPT_KBNRT_110 { get; set; }
        public DbSet<V_KBNRT_110_rpt> V_KBNRT_110_rpt { get; set; }

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

