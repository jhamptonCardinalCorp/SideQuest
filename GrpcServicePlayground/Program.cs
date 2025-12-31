
//      This is going to be shelved until we start working with Clusters and remote execution.

using GrpcServicePlayground.Services;

namespace GrpcServicePlayground
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddGrpc();

            var app = builder.Build();

            app.MapGrpcService<HelloServiceImpl>();
            app.MapGet("/", () => "Use a gRPC client to communicate.");

            // Configure the HTTP request pipeline.
            app.MapGrpcService<GreeterService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}