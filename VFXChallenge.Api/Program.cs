namespace VFXChallenge.Api;

using System.Configuration;

using Application;

using Infrastructure;
using Infrastructure.Configurations;

using Middleware;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        
        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.Configure<AlphaVantageApi>(builder.Configuration.GetSection("AlphaVantageApi"));

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        
        builder.Services.AddExceptionHandler<ExceptionHandlingMiddleware>();
        builder.Services.AddProblemDetails();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseExceptionHandler();
        
        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}