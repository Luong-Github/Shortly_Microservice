using AnalyticsService.Data;
using AnalyticsService.Events;
using AnalyticsService.Hubs;
using AnalyticsService.Middlewares;
using AnalyticsService.Repositories;
using AnalyticsService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Reflection;
using System.Text;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using AnalyticsService.Jobs;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<AppAnalyticsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppAnalyticsDb"));
});

// Confiugre REdis
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "http://localhost:5001",
            ValidAudience = "urlshortent_api",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("tQkM8cZXgXP1GK90841hBaoHIDoEwtud"))
        };
    });
// Register MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

builder.Services.AddSingleton<RedisService>();

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var syncJobKey = new JobKey("SyncRedisAnalyticsJob");
    q.AddJob<SyncRedisAnalyticsJob>(opts => opts.WithIdentity(syncJobKey));
    q.AddTrigger(opts => opts
        .ForJob(syncJobKey)
        .WithIdentity("SyncRedisAnalyticsTrigger")
        .WithCronSchedule("0 */10 * * * ?")); // Runs every 10 minutes
});

builder.Services.AddHostedService<RabbitMqConsumer>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowedCors", policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyHeader()
                                                  .AllowAnyMethod(); 
        policy.AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials()
               .SetIsOriginAllowed(_ => true);
});
});

builder.Services.AddSignalR();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var analyticsService = app.Services.GetRequiredService<AnalyticsService.Services.AnalyticsService>();
analyticsService.SubscribeToAnalyticsUpdates();

var clickEventConsumer = app.Services.GetRequiredService<ClickEventConsumer>();
Task.Run(() => clickEventConsumer.StartListening());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<AuditLoggingMiddleware>(); // Add Audit Logging Middleware


app.UseCors("AllowedCors");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHub<LoginAnalyticsHub>("login-hub");

app.Run();
