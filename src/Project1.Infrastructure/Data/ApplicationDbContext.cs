using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project1.Core.Products.Entities;
using Project1.Infrastructure.UserManagement.Entities;

namespace Project1.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long,
    IdentityUserClaim<long>, IdentityUserRole<long>, IdentityUserLogin<long>,
    IdentityRoleClaim<long>, IdentityUserToken<long>>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    #region user related entities

    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }

    #endregion

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Permission configuration
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("AspNetPermissions");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(256);

            entity.HasMany(p => p.UserPermissions)
                  .WithOne(up => up.Permission)
                  .HasForeignKey(up => up.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.RolePermissions)
                  .WithOne(rp => rp.Permission)
                  .HasForeignKey(rp => rp.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // UserPermission configuration
        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.ToTable("AspNetUserPermissions");

            entity.HasKey(up => new { up.UserId, up.PermissionId });

            entity.Property(up => up.IsGranted)
                  .IsRequired();

            entity.HasOne(up => up.User)
                  .WithMany(u => u.UserPermissions)
                  .HasForeignKey(up => up.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(up => up.Permission)
                  .WithMany(p => p.UserPermissions)
                  .HasForeignKey(up => up.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // RolePermission configuration
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("AspNetRolePermissions");

            entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            entity.HasOne(rp => rp.Role)
                  .WithMany(r => r.RolePermissions)
                  .HasForeignKey(rp => rp.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rp => rp.Permission)
                  .WithMany(p => p.RolePermissions)
                  .HasForeignKey(rp => rp.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
