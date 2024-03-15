namespace VFXChallenge.Infrastructure.Messaging
{
    using Domain.ForeignExchanges;
    
    public class ProduceForeignExchangeRateCreatedMessage : IProduceForeignExchangeRateCreatedMessage
    {
        public Task ProduceAsync(ForeignExchangeRate rate)
        {
            // To be implemented using https://github.com/Farfetch/kafkaflow
            return Task.CompletedTask;
        }
    }
}