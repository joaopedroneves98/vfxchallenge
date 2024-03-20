namespace VFXChallenge.Infrastructure.Messaging
{
    using Configurations;

    using Domain.ForeignExchanges;

    using KafkaFlow;
    using KafkaFlow.Producers;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class ProduceForeignExchangeRateCreatedMessage : IProduceForeignExchangeRateCreatedMessage
    {
        private readonly IMessageProducer _producer;
        private readonly KafkaSettings _kafkaSettings;
        private readonly ILogger<ProduceForeignExchangeRateCreatedMessage> _logger;
        
        public ProduceForeignExchangeRateCreatedMessage(IProducerAccessor producer,
            IOptions<KafkaSettings> kafkaSettings, 
            ILogger<ProduceForeignExchangeRateCreatedMessage> logger)
        {
            this._logger = logger;
            this._kafkaSettings = kafkaSettings.Value;
            this._producer = producer.GetProducer(kafkaSettings.Value.ForeignExchangeRateCreatedProducer);

        }
        
        public async Task ProduceAsync(ForeignExchangeRate rate)
        {
            await this._producer.ProduceAsync(this._kafkaSettings.ForeignExchangeRateTopicName, Guid.NewGuid().ToString(), rate);
            
            this._logger.LogInformation("Message produced!");
        }
    }
}