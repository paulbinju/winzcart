using Microsoft.EntityFrameworkCore;
using Winzcart.Domain.Entities;

namespace Winzcart.Infrastructure.Data;

public class WinzcartDbContext : DbContext
{
    public WinzcartDbContext(DbContextOptions<WinzcartDbContext> options) : base(options) { }

    public DbSet<AppUser> Users { get; set; } = null!;
    public DbSet<Merchant> Merchants { get; set; } = null!;
    public DbSet<Bill> Bills { get; set; } = null!;
    public DbSet<Coupon> Coupons { get; set; } = null!;
    public DbSet<UserCoupon> UserCoupons { get; set; } = null!;
    public DbSet<PointTransaction> PointTransactions { get; set; } = null!;
    public DbSet<WeeklyWinner> WeeklyWinners { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // AppUser -> Merchant (1:1)
        modelBuilder.Entity<AppUser>()
            .HasOne(u => u.Merchant)
            .WithOne(m => m.User)
            .HasForeignKey<Merchant>(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Coupon -> UserCoupon (1:N)
        modelBuilder.Entity<Coupon>()
            .HasMany(c => c.UserCoupons)
            .WithOne(uc => uc.Coupon)
            .HasForeignKey(uc => uc.CouponId)
            .OnDelete(DeleteBehavior.Restrict);

        // UserCoupon Code Unique
        modelBuilder.Entity<UserCoupon>()
            .HasIndex(uc => uc.Code)
            .IsUnique();
    }
}
