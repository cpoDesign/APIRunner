using Microsoft.EntityFrameworkCore;

namespace APITestingRunner.Unit.Tests.SampleDatabase
{
    public class SampleDataContext : DbContext
    {
        public SampleDataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<SampleTable> SampleTable { get; set; }
    }
}