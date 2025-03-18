using MeterReadingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterReadingApp.Repositories
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }
    }
}
