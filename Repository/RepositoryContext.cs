using Entities.Models;
using Entities.SytstemModel;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class RepositoryContext: DbContext
    {
        public RepositoryContext(DbContextOptions options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.ApplyConfiguration(new RoleConfiguration());
            //modelBuilder.Entity<UserModel>()
            //    .Has
        }
        public DbSet<DealsModel> Deals { get; set; }
        public DbSet<EmailModel> EmailLogs { get; set; }
    }
}