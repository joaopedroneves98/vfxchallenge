namespace VFXChallenge.Infrastructure.Messaging
{
    using Domain.ForeignExchanges;
    
    public interface IProduceForeignExchangeRateCreatedMessage
    {
        Task ProduceAsync(ForeignExchangeRate rate);
    }
}