using API;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Business;
using Business.Interfaces;
using Entities.DTO.Request.Day;
using Entities.DTO.Request.Person;
using Entities.DTO.Request.Schedule;
using Entities.Entity;
using ExceptionHandling.CustomMiddleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Interfaces;
using Persistence.Interfaces.GenericRepository;
using Persistence.Repository;
using Persistence.Repository.GenericRepository;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    ConfigureServices(builder.Services);

    Log.Information("Starting web host");

    var app = builder.Build();

    ConfigureMiddleware(app);

    app.Run();


    // Register your services/dependencies 
    void ConfigureServices(IServiceCollection services)
    {
        builder.Services.AddControllers().AddJsonOptions(options =>
        {   // avoid circular references when returning JSON in the API
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        // Add API versioning to the application
        builder.Services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());
        });

        builder.Services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // Add services to the container.
        services.AddScoped<DbContext, ApiContext>();

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IDayRepository, DayRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IDayPersonRepository, DayPersonRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();

        services.AddScoped<IBusiness<PersonDto, PersonEntity>, PersonBusiness>();
        services.AddScoped<IBusiness<DayDto, DayEntity>, DayBusiness>();
        services.AddScoped<IBusiness<ScheduleDto, ScheduleEntity>, ScheduleBusiness>();

        // Invoking action filters to validate the model state for all entities received in POST and PUT requests
        // https://code-maze.com/aspnetcore-modelstate-validation-web-api/
        services.AddScoped<ValidationFilterAttribute>();
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        var serilogUrl = builder.Configuration.GetSection("Seq")["Url"];
        // Connect to the database using Azure Key Vault and Azure Managed Identity to retrieve the connection string
        if (builder.Environment.IsProduction())
        {
            // Connect to Azure Key Vault using Azure Managed Identity
            var keyVaultURL = builder.Configuration["AzureKeyVault:Url"]!;

            builder.Configuration.AddAzureKeyVault(new Uri(keyVaultURL), new DefaultAzureCredential());

            var keyVaultClient = new SecretClient(new Uri(keyVaultURL), new DefaultAzureCredential());

            builder.Services.AddDbContext<ApiContext>(options =>
            {
                options.UseSqlServer(keyVaultClient.GetSecret("ConnectionStrings--SqlServer").Value.ToString())
                       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            serilogUrl = keyVaultClient.GetSecret("Seq--Url").Value.ToString();
        }

        // Access the database using the local connection string is running locally
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddDbContext<ApiContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"))
                       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
        }

        // Add Serilog to the application
        // https://www.youtube.com/watch?v=0acSdHJfk64
        // https://dotnetintellect.com/2020/09/06/logging-with-elasticsearch-kibana-serilog-using-asp-net-core-docker/
        builder.Host.UseSerilog(new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console()
            .WriteTo.Debug()
            .WriteTo.Seq(serverUrl: serilogUrl!)
            //.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(builder.Configuration["ElasticConfiguration:Uri"]))
            //{
            //    IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name!.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
            //    AutoRegisterTemplate = true
            //})
            .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger());
    }

    void ConfigureMiddleware(WebApplication app)
    {
        // Migrate latest database changes during startup
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApiContext>();

            dbContext.Database.Migrate();
        }

        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });
            app.UseDeveloperExceptionPage();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseSerilogRequestLogging();

        app.MapControllers();
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Created to support this class to be used for integration tests purpose
[ExcludeFromCodeCoverage]
public partial class Program { }