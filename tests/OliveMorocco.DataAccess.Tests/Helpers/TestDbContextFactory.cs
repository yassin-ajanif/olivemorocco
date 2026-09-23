using Microsoft.EntityFrameworkCore;
using OliveMorocco.DataAccess;

namespace OliveMorocco.DataAccess.Tests.Helpers;

internal static class TestDbContextFactory
{
    public static AppDbContext Create(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
