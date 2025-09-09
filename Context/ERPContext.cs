using Microsoft.EntityFrameworkCore;

using HINOSystem.Models.ERP;



namespace HINOSystem.Context
{    public class ERPContext : DbContext
    {
        public ERPContext(DbContextOptions<ERPContext> options)
            : base(options)
        {}
        public DbSet<erpGroup> erpGroup { get; set; } = null!;
        public DbSet<erpGroupMenu> erpGroupMenu { get; set; } = null!;
        public DbSet<erpGroupUser> erpGroupUser { get; set; } = null!;
        public DbSet<erpMenu> erpMenu { get; set; } = null!;
        public DbSet<erpMenuParent> erpMenuParent { get; set; } = null!;
        public DbSet<erpSystem> erpSystem { get; set; } = null!;
        public DbSet<erpSystemUser> erpSystemUser { get; set; } = null!;
        public DbSet<erpUser> erpUser { get; set; } = default!;


        //public DbSet<erpCountry> erpCountry { get; set; } = null!;
        //public DbSet<erpCountryThailand> erpCountryThailand { get; set; } = null!;
        //public DbSet<mstDistributor> mstDistributor { get; set; } = default!;
        //public DbSet<mstProblem> mstProblem { get; set; } = default!;
        //public DbSet<PO> PO { get; set; } = null!;
        //public DbSet<PODetail> PODetail { get; set; } = null!;



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

