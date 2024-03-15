namespace VFXChallenge.Infrastructure.Mappings
{
    using Domain.ForeignExchanges;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class ForeignExchangeRatesConfiguration: IEntityTypeConfiguration<ForeignExchangeRate>
    {

        public void Configure(EntityTypeBuilder<ForeignExchangeRate> builder)
        {
            builder.ToTable("ForeignExchangeRates");
            
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ToCurrencyCode)
                .HasColumnType("varchar(3)");
            builder.Property(x => x.FromCurrencyCode)
                .HasColumnType("varchar(3)");
            
            builder.Property(x => x.ToCurrencyName)
                .HasColumnType("nvarchar(100)");
            builder.Property(x => x.FromCurrencyName)
                .HasColumnType("nvarchar(100)");
            
            builder.Property(x => x.Bid)
                .HasColumnType("money");
            builder.Property(x => x.Ask)
                .HasColumnType("money");
            
            builder.Property(x => x.Created)
                .HasColumnType("DateTime");
            builder.Property(x => x.Updated)
                .HasColumnType("DateTime");
        }
    }
}