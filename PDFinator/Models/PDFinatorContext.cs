using Microsoft.EntityFrameworkCore;

namespace PDFinator.Models;

public class PDFinatorContext:DbContext
{
    public virtual DbSet<PDF> PDFs { get; set; }
    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<PDFTags> PDFTags { get;set; }

    public PDFinatorContext(DbContextOptions<PDFinatorContext> options): base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
