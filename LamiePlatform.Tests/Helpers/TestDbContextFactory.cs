using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using BadeePlatform.Data;

namespace LamiePlatform.Tests.Helpers
{
    public static class TestDbContextFactory
    {
        public static BadeedbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<BadeedbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new BadeedbContext(options);
        }
    }
}