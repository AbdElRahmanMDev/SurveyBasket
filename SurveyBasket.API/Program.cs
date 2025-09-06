
using FluentValidation;
using Hangfire;
using Serilog;
using SurveyBasket.API;
using SurveyBasket.API.Entities;
using SurveyBasket.API.Middlewares;
using SurveyBasket.API.Persistence;
using SurveyBasket.API.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


////Identity
//builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
//    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.

builder.Services.AddService(builder,builder.Configuration);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddScoped<ICacheService, CacheService>();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);    
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseHangfireDashboard("/jobs");

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var notificatinService = scope.ServiceProvider.GetRequiredService<INotificationService>();

RecurringJob.AddOrUpdate("sendNewPollNotification",() => notificatinService.SendNewPollNotification(null), Cron.Daily);

app.UseCors("MyPolicy");

app.UseAuthorization();

//app.MapIdentityApi<ApplicationUser>();

app.MapControllers();

app.UseExceptionHandler();

app.Run();
