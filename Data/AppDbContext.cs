using Microsoft.EntityFrameworkCore;
using WhereTo.Api.Models;

namespace WhereTo.Api.Data;

/// <summary>
/// Lentelės sukuriamos automatiškai (EnsureCreated, žr. Program.cs) – be EF migracijų.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Event> Events => Set<Event>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // created_at užpildo pati DB (DEFAULT CURRENT_TIMESTAMP)
        modelBuilder.Entity<Organization>()
            .Property(o => o.CreatedAt)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Event>()
            .Property(e => e.CreatedAt)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Trinant organizaciją, renginių organization_id tampa NULL (kaip schema.sql)
        modelBuilder.Entity<Event>()
            .HasOne(e => e.Organization)
            .WithMany(o => o.Events)
            .HasForeignKey(e => e.OrganizationId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
