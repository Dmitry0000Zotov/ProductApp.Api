using Microsoft.EntityFrameworkCore;
using TestTask.Application.Interfaces;
using TestTask.Domain;

namespace TestTask.Persistence;

public partial class TestDbContext : DbContext, ITestDbContext
{
    public TestDbContext()
    {
    }

    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EventLog> EventLogs { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductVersion> ProductVersions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventLog>(entity =>
        {
            entity.ToTable("EventLog");

            entity.HasIndex(e => e.EventDate, "IX_EventLog_EventDate");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.EventDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product", tb => tb.HasTrigger("Product_InsertUpdateDeleteTrigger"));

            entity.HasIndex(e => e.Name, "IX_Product_Name");

            entity.HasIndex(e => e.Name, "UQ_Product_Name").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<ProductVersion>(entity =>
        {
            entity.ToTable("ProductVersion", tb => tb.HasTrigger("ProductVersion_InsertUpdateDeleteTrigger"));

            entity.HasIndex(e => e.CreatingDate, "IX_ProductVersion_CreatingDate");

            entity.HasIndex(e => e.Height, "IX_ProductVersion_Height");

            entity.HasIndex(e => e.Length, "IX_ProductVersion_Length");

            entity.HasIndex(e => e.Name, "IX_ProductVersion_Name");

            entity.HasIndex(e => e.Width, "IX_ProductVersion_Width");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CreatingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Height).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Length).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Width).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductVersions)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductVersion_Product");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
