using Infrastructure.Services;
using NotificationService.BackgroundServices;
using NotificationService.Hubs;
using NotificationService.Interfaces;
using NotificationService.Jobs;
using NotificationService.Services;
using Quartz;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<ISmsService, TwilioSmsService>();
builder.Services.AddSingleton<ISlackService, SlackService>();
builder.Services.AddSingleton<ITelegramService, TelegramService>();
builder.Services.AddSingleton<IPushNotificationService, FirebasePushNotificationService>();

builder.Services.AddHostedService<AdminNotificationConsumer>();
builder.Services.AddHttpClient();
builder.Services.AddSignalR();

// Register MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("AnalyticsReportJob");
    q.AddJob<AnalyticsReportJob>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("AnalyticsReportTrigger")
        .WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(DayOfWeek.Sunday, 8, 0)) // Every Sunday at 8 AM
    );
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
builder.Services.AddScoped<AnalyticsReportJob>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<AdminNotificationHub>("notification-admin");
app.MapHub<NotificationHub>("/notificationHub");

app.UseHttpsRedirection();

app.Run();
