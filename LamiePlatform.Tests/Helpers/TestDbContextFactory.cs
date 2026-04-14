using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using LamiePlatform.Data;

namespace LamiePlatform.Tests.Helpers
{
    public static class TestDbContextFactory
    {
        public static LamieDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<LamieDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new LamieDbContext(options);
        }
    }
}