using MyInvoiceApp.Shared.Model;
using Microsoft.EntityFrameworkCore;
using MyInvoiceApp_Shared.Model;

namespace MyInvoiceApp_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Invoice_Item> Invoice_Items { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Status> Status { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Invoice>()
                .HasMany(i => i.Items)
                .WithOne(ii => ii.Invoice)
                .HasForeignKey(ii => ii.Invoice_Id);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Company)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.Company_Id)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Client)
                .WithMany()
                .HasForeignKey(i => i.Client_Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
