using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APITestingRunner.Unit.Tests.SampleDatabase
{

    public record SampleTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class SampleDataFixture : IDisposable
    {
        public SampleDataContext SampleContext { get; private set; }

        public void Construct()
        {

            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<SampleDataContext>().UseSqlite(connection).Options;

            SampleContext = new SampleDataContext(options);
            SampleContext.Database.EnsureCreated();
            SampleContext.SampleTable.Add(new SampleTable { Id = 1, Name = "Movie 1", Description = "Sample Description 1" });
            SampleContext.SampleTable.Add(new SampleTable { Id = 2, Name = "Movie 2", Description = "Sample Description 2" });
            SampleContext.SampleTable.Add(new SampleTable { Id = 3, Name = "Movie 3", Description = "Sample Description 3" });
            SampleContext.SaveChanges();         

        }

        public void Dispose()
        {
            SampleContext.Dispose();
        }
    }
}
