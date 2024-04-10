using Contract;
using Contracts;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service.Contract;
using Service;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Entities.ConfigurationModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Utilities.Constants;
using Entities.SystemModel;
using Service.BackgroundServices;

namespace PLT_ANP_API.Extentions
{
    public static class ServiceExtentions
    {
        /// <summary>
        /// Configures Cross-Origin Resource Sharing (CORS) policy.
        /// </summary>
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins("https://localhost:5173")
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials()
                           .WithExposedHeaders("X-Pagination");
                   
                });
            });
        }

        /// <summary>
        /// Configures integration with Internet Information Services (IIS).
        /// </summary>
        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
                // Configure IIS options here, if needed
            });
        }

        /// <summary>
        /// Registers the logger service for logging application events.
        /// </summary>
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        public static void ConfigureRepositoryManager(this IServiceCollection service) =>
             service.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(opts =>
                opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));
        }

        public static void ConfigureEmailServices(this IServiceCollection services)
        {
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<INotificationService, NotificationService>();
        }

        public static void ConfigureServiceManager(this IServiceCollection services) =>
            services.AddScoped<IServiceManager, ServiceManager>();

        /// <summary>
        /// Configures the identity system for user authentication and authorization.
        /// </summary>
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<UserModel, IdentityRole>(o =>
            {
                // Configure password requirements
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequiredLength = 5;
                o.User.RequireUniqueEmail = true;
                o.SignIn.RequireConfirmedEmail = true;
                o.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
            })
            .AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();
        }

        /// <summary>
        /// Configures JWT authentication for securing API endpoints.
        /// </summary>
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfiguration = new JwtConfiguration();
            configuration.Bind(jwtConfiguration.Section, jwtConfiguration);

            var secretKey = Environment.GetEnvironmentVariable(Constants.SecretKey);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Validate the issuer of the token
                    ValidateIssuer = true,
                    // Validate the audience of the token
                    ValidateAudience = true,
                    // Validate the expiration time of the token
                    ValidateLifetime = true,
                    // Validate the signing key of the token
                    ValidateIssuerSigningKey = true,
                    // Set the valid issuer and audience
                    ValidIssuer = jwtConfiguration.ValidIssuer,
                    ValidAudience = jwtConfiguration.ValidAudience,
                    // Set the signing key
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero,
                   
                };
            });
        }
        public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtConfiguration>(configuration.GetSection("JwtSettings"));
        }
        public static void AddSMTPConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SMTPSettings>(configuration.GetSection("SMTPSettings"));

            services.AddOptions<SMTPSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("SMTPSettings").Bind(settings);
                    settings.FromEmail = Environment.GetEnvironmentVariable("FromEmail");
                    settings.FromEmailPassword = Environment.GetEnvironmentVariable("FromEmailPassword");
                });
        }
        public static void ConfigureHosting(this IServiceCollection services)
        {
            services.AddHostedService<PLTBackGroundService>();
        }
    }
}
