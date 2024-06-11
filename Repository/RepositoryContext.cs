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
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TempUserModel>()
                .HasOne(c => c.UserModel)
                .WithOne(p => p.TempUser)
                .HasForeignKey<TempUserModel>(c => c.UserId)
                .IsRequired(false);

            modelBuilder.Entity<NotificationModel>()
                .HasOne(c => c.Deal)
                .WithMany(p => p.Notifications)
                .HasForeignKey(c => c.DealId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LoginSessions>()
                .HasOne(c => c.User)
                .WithMany(k => k.Sessions)
                .HasForeignKey(c => c.UserId)
                .IsRequired(true);

            modelBuilder.Entity<RenewalsModel>()
                .Property(c => c.Value)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<DealsModel>()
                .Property(c => c.Value)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<RenewalsModel>()
                .HasOne(c => c.Deal)
                .WithMany(l => l.Renewals)
                .HasForeignKey(c => c.DealId)
                .IsRequired(true);
            //modelBuilder.ApplyConfiguration(new RoleConfiguration());

            //modelBuilder.Entity<UserModel>()
            //    .Has

        }
        public DbSet<DealsModel> Deals { get; set; }
        public DbSet<EmailModel> EmailLogs { get; set; }
        public DbSet<EmailTemplateModel> EmailTemplates { get; set; }
        public DbSet<NotificationModel> Notifications { get; set; }
        public DbSet<TempUserModel> TempUser { get; set; }
        public DbSet<LoginSessions> LoginSessions { get; set; }
        public DbSet<RenewalsModel> Renewals { get; set; }
    }
}   