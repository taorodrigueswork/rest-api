using Business;
using Business.Interfaces;
using Entities.Context;
using Entities.DTO.Request.Person;
using Entities.Entity;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web host");

    var builder = WebApplication.CreateBuilder(args);

    // Using this as reference to split the configuration in multiple functions
    // https://andrewlock.net/exploring-dotnet-6-part-12-upgrading-a-dotnet-5-startup-based-app-to-dotnet-6/
    ConfigureConfiguration(builder.Configuration);
    ConfigureServices(builder.Services);

    var app = builder.Build();

    ConfigureMiddleware(app);
    ConfigureEndpoints(app);

    app.Run();


    void ConfigureConfiguration(ConfigurationManager configuration)
    {
        // Add Serilog to the application
        // https://www.youtube.com/watch?v=0acSdHJfk64
        // https://dotnetintellect.com/2020/09/06/logging-with-elasticsearch-kibana-serilog-using-asp-net-core-docker/
        // Seq: 
        // Serilog configuration
        // _logger.LogInformation("Log message generated with INFORMATION severity level.");
        // _logger.LogWarning("Log message generated with WARNING severity level.");
        // _logger.LogError("Log message generated with ERROR severity level.");
        // _logger.LogCritical("Log message log generated with CRITICAL severity level.");

        builder.Host.UseSerilog(new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console()
            .WriteTo.Debug()
            .WriteTo.Seq("http://localhost:5341")
            //.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(builder.Configuration["ElasticConfiguration:Uri"]))
            //{
            //    IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name!.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
            //    AutoRegisterTemplate = true
            //})
            .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger());
    }

    // Register your services/dependencies 
    void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.
        services.AddScoped<IBusiness<PersonDTO, PersonEntity>, PersonBusiness>();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Connect to the database
        var connectionString = builder.Configuration.GetConnectionString("SqlServer");

        builder.Services.AddDbContext<ApiContext>(options =>
        {
            options.UseSqlServer(connectionString)
                   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
    }

    void ConfigureMiddleware(WebApplication app)
    {
        // Migrate latest database changes during startup
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider
                .GetRequiredService<ApiContext>();

            // Here is the migration executed
            dbContext.Database.Migrate();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseSerilogRequestLogging();

        app.MapControllers();
    }

    void ConfigureEndpoints(IEndpointRouteBuilder app) { }

}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}