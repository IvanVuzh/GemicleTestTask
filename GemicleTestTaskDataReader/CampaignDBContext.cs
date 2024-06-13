using GemicleTestTaskModels.DBModels;
using Microsoft.EntityFrameworkCore;

namespace GemicleTestTaskData
{
    public class CampaignDbContext : DbContext
    {
        public CampaignDbContext(DbContextOptions<CampaignDbContext> options)
            : base(options)
        {
        }

        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerCampaignSchedule> CustomerCampaignSchedule { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Campaign>().HasKey(ccl => ccl.Id);

            modelBuilder.Entity<Campaign>().Property(ccl => ccl.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Customer>().HasKey(ccl => ccl.Id);

            modelBuilder.Entity<CustomerCampaignSchedule>().HasKey(ccl => ccl.Id);

            modelBuilder.Entity<CustomerCampaignSchedule>().Property(ccl => ccl.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<CustomerCampaignSchedule>()
                .HasOne(ccl => ccl.Campaign)
                .WithMany(c => c.CustomerLogs)
                .HasForeignKey(ccl => ccl.CampaignId);

            modelBuilder.Entity<CustomerCampaignSchedule>()
                .HasOne(ccl => ccl.Customer)
                .WithMany(c => c.CustomerLogs)
                .HasForeignKey(ccl => ccl.CustomerId);
        }
    }
}
