using MagniseTestTask.Database;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddControllers();


Log.Logger = new LoggerConfiguration().MinimumLevel
    .Override("Microsoft", LogEventLevel.Information).
    WriteTo.File("MgnTestTask-.txt", rollingInterval:RollingInterval.Day).CreateLogger();

using (ServiceProvider serviceProvider = builder.Services.BuildServiceProvider())
{
    try
    {
        var ctx = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await DbInitializer.Seed(ctx, builder.Configuration);
    }
    catch (Exception e)
    {
        Log.Fatal(e, $"Error: {e.Message}");
    }
}

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();