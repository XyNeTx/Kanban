using KANBAN.Models.KB3.Master;
using Microsoft.EntityFrameworkCore;

namespace KANBAN.Context;

public partial class KB3SFContext : DbContext
{
    public KB3SFContext()
    {
    }

    public KB3SFContext(DbContextOptions<KB3SFContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TB_Import_VHD> TbImportVhds { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=HMMTA-PPM;Initial Catalog=New_Kanban_F3;User ID=sa;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
