using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repository;

namespace PLT_ANP_API.ContextFactory
{
    public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
    {
        public RepositoryContext CreateDbContext(string[] args)
        {
           
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();


            var builder = new DbContextOptionsBuilder<RepositoryContext>()
        .UseSqlServer(configuration.GetConnectionString("sqlConnection"),
        b => b.MigrationsAssembly("PLT-ANP-API"));
            return new RepositoryContext(builder.Options);
        }
    }
}
