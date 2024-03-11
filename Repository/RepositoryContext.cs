using Entities.Models;
using Entities.SystemModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repository.Configuration;

namespace Repository
{
    public class RepositoryContext: IdentityDbContext<UserModel>
    {
        public RepositoryContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EmailModel>()
                .HasOne(c => c.Owner)
                .WithMany(p => p.Emails)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<TempUserModel>()
                .HasOne(c => c.UserModel)
                .WithOne(p => p.TempUser)
                .HasForeignKey<TempUserModel>(c => c.UserId)
                .IsRequired(false);




            //modelBuilder.ApplyConfiguration(new RoleConfiguration());

            //modelBuilder.Entity<UserModel>()
            //    .Has
        }
        public DbSet<DealsModel> Deals { get; set; }
        public DbSet<EmailModel> EmailLogs { get; set; }
        public DbSet<EmailTemplateModel> EmailTemplates { get; set; }
        public DbSet<TempUserModel> TempUser { get; set; }
    }
}   