using Microsoft.EntityFrameworkCore;
using MushroomApi.Models;

namespace MushroomApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Inquiry> Inquiries { get; set; }
    }
}