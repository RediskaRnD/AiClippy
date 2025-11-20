using Persistence.Extensions;

namespace Api;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.ConfigureKestrel(f => { f.AddServerHeader = false; });
        builder.Services.AddHttpContextAccessor();

        builder.Services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.AllowInputFormatterExceptionMessages = false;
                options.JsonSerializerOptions.SetOptions();
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddDatabase();

        var app = builder.Build();

        app.MapMethods("/", [HttpMethods.Get, HttpMethods.Head], async context => await context.Response.WriteAsync("ok"));
        app.MapGet("/api", () => "Hello API!");
        app.MapControllers();
        app.Run();
    }
}