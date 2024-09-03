using ensek_test.Models;
using Microsoft.EntityFrameworkCore;

namespace ensek_test.DataAccess.DbContexts
{

    public class UnitOfWork : DbContext
    {
        public UnitOfWork(DbContextOptions<UnitOfWork> options) : base(options) {  }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlite(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MeterReading>().HasKey(x => x.Id);
            modelBuilder.Entity<Account>().HasKey(x => x.AccountId);
        }

        public DbSet<MeterReading> meter_readings { get; set; }
        public DbSet<Account> accounts { get; set; }
    }
}
