namespace VFXChallenge.Infrastructure
{
    using Domain.ForeignExchanges;

    using Mappings;

    using Microsoft.EntityFrameworkCore;
    public class ExchangeRatesDbContext : DbContext
    {
        public ExchangeRatesDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ForeignExchangeRatesConfiguration());
        
            base.OnModelCreating(modelBuilder);
        }
        
        public DbSet<ForeignExchangeRate> ForeignExchangeRates { get; set; }
    }
}