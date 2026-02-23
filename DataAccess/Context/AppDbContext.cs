using Entities.Concrete;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Barn>()
                .HasOne(b => b.OwnerUser)
                .WithOne(u => u.Barn)
                .HasForeignKey<Barn>(b => b.OwnerUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BarnInventory>()
                .HasOne(bi => bi.Barn)
                .WithMany(b => b.Inventory)
                .HasForeignKey(bi => bi.BarnId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BarnInventory>()
                .HasIndex(bi => new { bi.BarnId, bi.ProductId })
                .IsUnique();

            builder.Entity<Barn>().Property(b => b.BarnBalance).HasColumnType("decimal(18,2)");
            builder.Entity<Sale>().Property(s => s.SaleAmount).HasColumnType("decimal(18,2)");
            builder.Entity<Sale>().Property(s => s.UnitPriceAtSale).HasColumnType("decimal(18,2)");
            builder.Entity<Purchase>().Property(p => p.UnitPrice).HasColumnType("decimal(18,2)");
            builder.Entity<Purchase>().Property(p => p.TotalCost).HasColumnType("decimal(18,2)");
            builder.Entity<Product>().Property(p => p.ProductPrice).HasColumnType("decimal(18,2)");
            builder.Entity<AnimalSpecies>().Property(s => s.AnimalSpeciesPurchasePrice).HasColumnType("decimal(18,2)");
        }

        public DbSet<Barn> Barns { get; set; } = null!;
        public DbSet<Animal> Animals { get; set; } = null!;
        public DbSet<AnimalSpecies> AnimalSpecies { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<BarnInventory> BarnInventories { get; set; } = null!;
        public DbSet<Purchase> Purchases { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;
        public DbSet<BarnWorker> BarnWorkers { get; set; } = null!;
    }
}
