using Lodgr.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Lodgr.Api.Data;

public class LodgrDbContext(DbContextOptions<LodgrDbContext> options) : DbContext(options)
{
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<DepositTransaction> DepositTransactions => Set<DepositTransaction>();
    public DbSet<District> Districts => Set<District>();
    public DbSet<FeeConfig> FeeConfigs => Set<FeeConfig>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<OccupancyChange> OccupancyChanges => Set<OccupancyChange>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Province> Provinces => Set<Province>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UtilityReading> UtilityReadings => Set<UtilityReading>();
    public DbSet<Ward> Wards => Set<Ward>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().Property(x => x.Role).HasConversion<string>();

        modelBuilder.Entity<Contract>().Property(x => x.DepositStatus).HasConversion<string>();
        modelBuilder.Entity<Contract>().Property(x => x.Status).HasConversion<string>();

        modelBuilder.Entity<DepositTransaction>().Property(x => x.TxnType).HasConversion<string>();
        modelBuilder.Entity<DepositTransaction>().Property(x => x.PaymentMethod).HasConversion<string>();

        modelBuilder.Entity<FeeConfig>().Property(x => x.FeeType).HasConversion<string>();
        modelBuilder.Entity<FeeConfig>().Property(x => x.ChargeMode).HasConversion<string>();

        modelBuilder.Entity<Invoice>().Property(x => x.Status).HasConversion<string>();

        modelBuilder.Entity<InvoiceItem>().Property(x => x.ItemType).HasConversion<string>();

        modelBuilder.Entity<Notification>().Property(x => x.Type).HasConversion<string>();
        modelBuilder.Entity<Notification>().Property(x => x.Status).HasConversion<string>();

        modelBuilder.Entity<OccupancyChange>().Property(x => x.Reason).HasConversion<string>();

        modelBuilder.Entity<Payment>().Property(x => x.PaymentMethod).HasConversion<string>();

        modelBuilder.Entity<Room>().Property(x => x.RoomType).HasConversion<string>();
        modelBuilder.Entity<Room>().Property(x => x.OperationalStatus).HasConversion<string>();

        modelBuilder.Entity<UtilityReading>().Property(x => x.ReadingType).HasConversion<string>();

        modelBuilder.Entity<Room>()
            .HasIndex(x => new { x.BuildingId, x.RoomNumber })
            .IsUnique()
            .HasFilter("is_deleted = FALSE");

        modelBuilder.Entity<Invoice>()
            .HasIndex(x => new { x.ContractId, x.PeriodFrom, x.PeriodTo })
            .IsUnique();

        modelBuilder.Entity<UtilityReading>()
            .HasIndex(x => new { x.RoomId, x.PeriodFrom, x.PeriodTo, x.ReadingType })
            .IsUnique();

        modelBuilder.Entity<Contract>().ToTable(t =>
        {
            t.HasCheckConstraint("ck_contracts_billing_cycle_day", "billing_cycle_day BETWEEN 1 AND 28");
            t.HasCheckConstraint("ck_contracts_current_occupants", "current_occupants >= 1");
            t.HasCheckConstraint("ck_contracts_deposit_amount", "deposit_amount >= 0");
            t.HasCheckConstraint("ck_contracts_deposit_balance", "deposit_balance >= 0");
            t.HasCheckConstraint("ck_contracts_end_date", "end_date IS NULL OR end_date >= start_date");
            t.HasCheckConstraint("ck_contracts_actual_end_date", "actual_end_date IS NULL OR actual_end_date >= start_date");
        });

        modelBuilder.Entity<UtilityReading>().ToTable(t =>
        {
            t.HasCheckConstraint("ck_utility_readings_period", "period_to > period_from");
            t.HasCheckConstraint("ck_utility_readings_closing_date", "closing_date >= period_to");
            t.HasCheckConstraint("ck_utility_readings_electricity", "electricity_new IS NULL OR electricity_old IS NULL OR electricity_new >= electricity_old");
            t.HasCheckConstraint("ck_utility_readings_water", "water_new IS NULL OR water_old IS NULL OR water_new >= water_old");
        });

        modelBuilder.Entity<Invoice>().ToTable(t =>
        {
            t.HasCheckConstraint("ck_invoices_period", "period_to > period_from");
            t.HasCheckConstraint("ck_invoices_paid_amount", "paid_amount >= 0 AND paid_amount <= total_amount");
        });

        modelBuilder.Entity<Payment>()
            .ToTable(t => t.HasCheckConstraint("ck_payments_amount", "amount > 0"));

        modelBuilder.Entity<DepositTransaction>()
            .ToTable(t => t.HasCheckConstraint("ck_deposit_transactions_amount", "amount > 0"));
    }
}