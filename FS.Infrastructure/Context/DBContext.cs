using FS.Core;
using Microsoft.EntityFrameworkCore;

namespace FS.Infrastructure.Context
{
    public class DBContext : DbContext
    {
        public DbSet<Exibitor> Exibitors { get; set; }

        public DBContext(DbContextOptions options) : base(options)
        {
        }
    }
}