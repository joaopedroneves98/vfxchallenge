namespace VFXChallenge.Api;

using Application;

using Infrastructure;
using Infrastructure.Configurations;

using KafkaFlow;
using KafkaFlow.Serializer;

using Middleware;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.Configure<AlphaVantageApi>(builder.Configuration.GetSection("AlphaVantageApi"));
        builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("KafkaSettings"));

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddExceptionHandler<ExceptionHandlingMiddleware>();
        builder.Services.AddProblemDetails();

        // Kafka
        var kafkaSettings = builder.Configuration.GetSection("KafkaSettings").Get<KafkaSettings>();
        builder.Services.AddKafka(kafka => kafka
            .AddCluster(cluster => cluster
                .WithBrokers(new[]
                {
                    "localhost:9092"
                })
                .CreateTopicIfNotExists(kafkaSettings.ForeignExchangeRateTopicName, 1, 1)
                .AddProducer(kafkaSettings.ForeignExchangeRateCreatedProducer,
                    producer => producer
                        .DefaultTopic(kafkaSettings.ForeignExchangeRateTopicName)
                        .AddMiddlewares(m =>
                            m.AddSerializer<JsonCoreSerializer>())
                )));

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

        var kafkaBus = app.Services.CreateKafkaBus();
        await kafkaBus.StartAsync();
        
        app.Run();
    }
}