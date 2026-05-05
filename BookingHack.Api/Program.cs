using System.Globalization;
using BookingHack.Core.Configuration;
using BookingHack.Infrastructure.PostgreSql.Seeding;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting BookingHack Application");
    
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Services.AddConfiguration(builder.Configuration);
    
    var app = builder.Build();

    await DatabaseSeeder.SeedAsync(app.Services);

    app.Configure();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}