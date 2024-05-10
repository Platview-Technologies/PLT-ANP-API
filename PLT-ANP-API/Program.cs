using Microsoft.AspNetCore.HttpOverrides;
using PLT_ANP_API.Extentions;
using NLog;
using PLT_ANP_API.Presentation.ActionFilters;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using PLT_ANP_API.Presentation;
using Utilities;
using Utilities.Constants;

var builder = WebApplication.CreateBuilder(args);


// passing the enlog config file nto the logmanager load configuration method
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "./nlog.config"));

// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
// Configure SQL Context and pass it the IConfiguration class to get the connection string.
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
//configure EMailServices
builder.Services.ConfigureEmailServices();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<ValidationFilterAttribute>();
// configure identity user
builder.Services.AddAuthentication();
// identity
builder.Services.ConfigureIdentity();
//jwt
builder.Services.ConfigureJWT(builder.Configuration);
// IOptions
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddSMTPConfigurations(builder.Configuration);
builder.Services.ConfigureHosting();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddControllers()
    .AddApplicationPart(typeof(AssemblyReference).Assembly);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.ConfigureSwagger();
var app = builder.Build();
AppDomain.CurrentDomain.SetData(Constants.WebRootPath, app.Environment.WebRootPath);
app.UseCors("CorsPolicy");
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(s => s.SwaggerEndpoint("/swagger/v1/swagger.json", "PLT-ANP-API By Platview"));
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
   
}
else
    app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
