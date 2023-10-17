using DevOps.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Api.Filters;
using DevOps.AppLogic;
using AutoMapper;
using MassTransit;
using DevOps.AppLogic.Events;
using Api.MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Api.Swagger;
using Microsoft.OpenApi.Models;

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



string identityUrlExternal = builder.Configuration.GetValue<string>("Urls:IdentityUrlExternal");
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DevOps.Api", Version = "v1" });
    string securityScheme = "OpenID";
    var scopes = new Dictionary<string, string>
    {
        { "devops.read", "DevOps API - Read access" },
        { "manage", "Write access" }
    };
    c.AddSecurityDefinition(securityScheme, new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{identityUrlExternal}/connect/authorize"),
                TokenUrl = new Uri($"{identityUrlExternal}/connect/token"),
                Scopes = scopes
            }
        }
    });
    c.OperationFilter<AlwaysAuthorizeOperationFilter>(securityScheme, scopes.Keys.ToArray());
});

var readPolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .RequireClaim("scope", "devops.read")
    .Build();

builder.Services.AddSingleton(provider => new ApplicationExceptionFilterAttribute(provider.GetRequiredService<ILogger<ApplicationExceptionFilterAttribute>>()));
builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ApplicationExceptionFilterAttribute>();
    options.Filters.Add(new AuthorizeFilter(readPolicy));
});

var writePolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .RequireClaim("scope", "manage")
    .Build();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("write", writePolicy);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        string identityUrl = builder.Configuration.GetValue<string>("Urls:IdentityUrl");
        options.Authority = identityUrl;
        options.Audience = "devops";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false
        };

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
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DevOps.Api v1");
        c.OAuthClientId("swagger.devops");
        c.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
