using GrpcSessionManager;
using Microsoft.Extensions.Caching.Memory;
using rss_base.InfraStructure;
using rss_base.Services;

namespace rss_base
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;

            // Add services to the container.
            ConfigureServices(services);

            //create & configure hoster
            var app = builder.Build();
            var webHostEnvironment = app.Environment;
            Configure(app, webHostEnvironment);

            app.MapGrpcService<SessionManagerGrpcServer>();
            if (webHostEnvironment.IsDevelopment())
            {
                app.MapGrpcReflectionService();
            }
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            //Do it
            app.Run();
        }

        static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            serviceCollection.AddEndpointsApiExplorer();
            serviceCollection.AddSwaggerGen();
            serviceCollection.AddGrpc();
            serviceCollection.AddGrpcReflection();
            serviceCollection.AddApiVersioning();
            serviceCollection.AddScoped<ISessionManager, SessionManager>();
            serviceCollection.AddScoped<ISessionCache, SessionCache>();
            serviceCollection.AddSingleton<IMemoryCache, MemoryCache>();
        }

        static void Configure(WebApplication webApplication, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                webApplication.UseSwagger();
                webApplication.UseSwaggerUI();
                using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            }
            webApplication.UseHttpsRedirection();
            webApplication.UseAuthorization();
            webApplication.MapControllers();
        }
    }
}
