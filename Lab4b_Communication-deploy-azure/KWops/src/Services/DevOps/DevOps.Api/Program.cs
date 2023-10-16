using DevOps.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Api.Filters;
using DevOps.AppLogic;
using AutoMapper;
using MassTransit;
using DevOps.AppLogic.Events;
using Api.MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<DevOpsDbInitializer>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IDeveloperRepository, DeveloperRepository>();

builder.Services.AddSingleton(provider => new ApplicationExceptionFilterAttribute(provider.GetRequiredService<ILogger<ApplicationExceptionFilterAttribute>>()));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ApplicationExceptionFilterAttribute>();
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<EmployeeHiredEventConsumer>();

    //Alternative to registering all consumers one by one: register all consumers in the AppLogic assembly
    //x.AddConsumers(typeof(EmployeeHiredEventConsumer).Assembly);

    IConfigurationSection eventBusSection = builder.Configuration.GetSection("EventBus");
    var eventBusSettings = new EventBusSettings();
    eventBusSection.Bind(eventBusSettings);
    x.UseEventBus(eventBusSettings);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ConfigurationManager configuration = builder.Configuration;
builder.Services.AddDbContext<DevOpsContext>(options =>
{
    string connectionString = configuration["ConnectionString"];
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
    });
#if DEBUG
    options.UseLoggerFactory(LoggerFactory.Create(loggingBuilder => loggingBuilder.AddDebug()));
    options.EnableSensitiveDataLogging();
#endif
});

var app = builder.Build();

IServiceScope startUpScope = app.Services.CreateScope();
var initializer = startUpScope.ServiceProvider.GetRequiredService<DevOpsDbInitializer>();
initializer.MigrateDatabase();
initializer.SeedData();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
