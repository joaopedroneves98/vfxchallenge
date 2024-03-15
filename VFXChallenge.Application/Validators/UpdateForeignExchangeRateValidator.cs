namespace VFXChallenge.Application.Validators
{
    using DTO;

    using FluentValidation;
    public class UpdateForeignExchangeRateValidator: AbstractValidator<UpdateForeignExchangeRate>
    {
        public UpdateForeignExchangeRateValidator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.FromCurrencyCode).NotNull().NotEmpty().Length(3);
            RuleFor(x => x.ToCurrencyCode).NotNull().NotEmpty().Length(3);
            RuleFor(x => x.FromCurrencyName).NotNull().NotEmpty().MaximumLength(100);
            RuleFor(x => x.ToCurrencyName).NotNull().NotEmpty().MaximumLength(100);
            RuleFor(x => x.Bid).NotEqual(0);
            RuleFor(x => x.Ask).NotEqual(0);
        }
    }
}