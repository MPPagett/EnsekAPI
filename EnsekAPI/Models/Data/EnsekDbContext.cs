using Microsoft.EntityFrameworkCore;
using EnsekAPI.Data.Models;

namespace EnsekAPI.Data
{
    public class EnsekDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }


        public EnsekDbContext(DbContextOptions<EnsekDbContext> options) : base(options)
        {
        }
    }
}
