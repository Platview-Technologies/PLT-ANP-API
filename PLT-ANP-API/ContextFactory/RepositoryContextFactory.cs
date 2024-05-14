using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repository;

namespace PLT_ANP_API.ContextFactory
{
    public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
    {
        public RepositoryContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("sqlConnection");
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            connectionString ??= configuration.GetConnectionString("sqlConnection");
            var builder = new DbContextOptionsBuilder<RepositoryContext>()
        .UseSqlServer(connectionString,
        b => b.MigrationsAssembly("PLT-ANP-API"));
            return new RepositoryContext(builder.Options);
        }
    }
}
