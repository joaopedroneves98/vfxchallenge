namespace VFXChallenge.Application.Tests.Services
{
    using System;
    using System.Threading.Tasks;

    using Abstractions;

    using AutoFixture;

    using Domain.ForeignExchanges;

    using DTO;

    using FluentAssertions;

    using FluentValidation;
    using FluentValidation.Results;

    using Infrastructure.AlphaVantageApi;
    using Infrastructure.Exceptions;
    using Infrastructure.Messaging;
    using Infrastructure.Repositories;

    using Moq;

    using VFXChallenge.Application.Services;
    using Xunit;

    using ValidationException = Infrastructure.Exceptions.ValidationException;
    public class CurrencyExchangeRateServiceTest
    {
        private readonly Mock<IAlphaVantageGateway> _alphaVantageGatewayMock;
        private readonly Mock<IForeignExchangeRatesRepository> _foreignExchangeRatesRepository;
        private readonly Mock<IProduceForeignExchangeRateCreatedMessage> _produceMessageMock;
        private readonly Mock<IValidator<CreateForeignExchangeRate>> _createValidatorMock;
        private readonly Mock<IValidator<UpdateForeignExchangeRate>> _updateValidatorMock;

        private readonly ICurrencyExchangeRateService _currencyExchangeRateService;
        
        private readonly IFixture _fixture;
        
        public CurrencyExchangeRateServiceTest()
        {
            this._fixture = new Fixture();
            this._alphaVantageGatewayMock = new Mock<IAlphaVantageGateway>(MockBehavior.Strict);
            this._foreignExchangeRatesRepository = new Mock<IForeignExchangeRatesRepository>(MockBehavior.Strict);
            this._produceMessageMock = new Mock<IProduceForeignExchangeRateCreatedMessage>(MockBehavior.Strict);
            this._createValidatorMock = new Mock<IValidator<CreateForeignExchangeRate>>(MockBehavior.Strict);
            this._updateValidatorMock = new Mock<IValidator<UpdateForeignExchangeRate>>(MockBehavior.Strict);
            this._currencyExchangeRateService = new CurrencyExchangeRateService(
                this._alphaVantageGatewayMock.Object,
                this._foreignExchangeRatesRepository.Object,
                this._produceMessageMock.Object,
                this._createValidatorMock.Object,
                this._updateValidatorMock.Object);
        }

        [Fact]
        public async Task GetAsync_NullParameters_ShouldThrowException()
        {
            // Arrange
            
            // Act 
            Func<Task> result = async () => await this._currencyExchangeRateService.GetAsync(null, null);

            // Assert
            await result.Should().ThrowAsync<ValidationException>().Where(x => x.Message.Equals("From Currency and To Currency are required"));
        }

        [Fact]
        public async Task GetAsync_DataNotAvailableInRepository_ShouldRetrieveFromAPI()
        {
            // Arrange
            var fromCurrency = "EUR";
            var toCurrency = "USD";

            var apiResponse = this._fixture.Create<ForeignExchangeRate>();
            
            this._foreignExchangeRatesRepository
                .Setup(x => x.GetAsync(fromCurrency, toCurrency))
                .ReturnsAsync((ForeignExchangeRate)null);

            this._alphaVantageGatewayMock
                .Setup(x => x.GetExchangeRateForCurrencyPairAsync(fromCurrency, toCurrency))
                .ReturnsAsync(apiResponse);
            
            this._foreignExchangeRatesRepository
                .Setup(x => x.CreateAsync(apiResponse))
                .ReturnsAsync(this._fixture.Create<ForeignExchangeRate>());

            this._produceMessageMock
                .Setup(x => x.ProduceAsync(apiResponse))
                .Returns(Task.CompletedTask);

            // Act
            var result = await this._currencyExchangeRateService.GetAsync(fromCurrency, toCurrency);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(apiResponse, options => options.ExcludingMissingMembers());
            
            this._foreignExchangeRatesRepository.VerifyAll();
            this._alphaVantageGatewayMock.VerifyAll();
        }
        
        [Fact]
        public async Task GetAsync_DataNotAvailableInRepositoryAndInAPI_ShouldThrowException()
        {
            // Arrange
            var fromCurrency = "EUR";
            var toCurrency = "USD";
            
            this._foreignExchangeRatesRepository
                .Setup(x => x.GetAsync(fromCurrency, toCurrency))
                .ReturnsAsync((ForeignExchangeRate)null);

            this._alphaVantageGatewayMock
                .Setup(x => x.GetExchangeRateForCurrencyPairAsync(fromCurrency, toCurrency))
                .ReturnsAsync((ForeignExchangeRate)null);
            
            // Act
            Func<Task> result = async () => await this._currencyExchangeRateService.GetAsync(fromCurrency, toCurrency);

            // Assert
            await result.Should().ThrowAsync<NotFoundException>().Where(x => x.Message.Equals("Rate not found."));
            
            this._foreignExchangeRatesRepository.VerifyAll();
            this._alphaVantageGatewayMock.VerifyAll();
        }
        
        
        [Fact]
        public async Task GetAsync_DataInRepository_ShouldReturnRate()
        {
            // Arrange
            var fromCurrency = "EUR";
            var toCurrency = "USD";

            var repoResponse = this._fixture.Create<ForeignExchangeRate>();
            
            this._foreignExchangeRatesRepository
                .Setup(x => x.GetAsync(fromCurrency, toCurrency))
                .ReturnsAsync(repoResponse);

            // Act
            var result = await this._currencyExchangeRateService.GetAsync(fromCurrency, toCurrency);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(repoResponse, options => options.ExcludingMissingMembers());
            
            this._foreignExchangeRatesRepository.VerifyAll();
        }
        
        [Fact]
        public async Task CreateAsync_InvalidRequest_ShouldThrowException()
        {
            // Arrange

            this._createValidatorMock
                .Setup(x => x.Validate(It.IsAny<CreateForeignExchangeRate>()))
                .Returns(new ValidationResult(this._fixture.CreateMany<ValidationFailure>()));

            // Act
            Func<Task> result = async () => await this._currencyExchangeRateService.CreateAsync(null);

            // Assert
            await result.Should().ThrowAsync<ValidationException>();
            
            this._createValidatorMock.VerifyAll();
        }
        
        [Fact]
        public async Task CreateAsync_ExistingCurrencyPair_ShouldThrowException()
        {
            // Arrange
            var request = this._fixture.Create<CreateForeignExchangeRate>();
            
            var existingPair = this._fixture.Create<ForeignExchangeRate>();

            this._createValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult());
            
            this._foreignExchangeRatesRepository
                .Setup(x => x.GetAsync(request.FromCurrencyCode, request.ToCurrencyCode))
                .ReturnsAsync(existingPair);
            
            // Act
            Func<Task> result = async () => await this._currencyExchangeRateService.CreateAsync(request);

            // Assert
            await result.Should().ThrowAsync<ApplicationErrorException>()
                .Where(x => x.Message.Equals("The provided Foreign Exchange Rate Pair already exists."));
            
            this._foreignExchangeRatesRepository.VerifyAll();
            this._createValidatorMock.VerifyAll();
        }
        
        [Fact]
        public async Task CreateAsync_ValidRequest_ShouldCreateForeignExchangeRate()
        {
            // Arrange
            var request = this._fixture.Create<CreateForeignExchangeRate>();

            var repoResponse = this._fixture.Create<ForeignExchangeRate>();
            
            this._createValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult());
            
            this._foreignExchangeRatesRepository
                .Setup(x => x.GetAsync(request.FromCurrencyCode, request.ToCurrencyCode))
                .ReturnsAsync((ForeignExchangeRate)null);

            this._foreignExchangeRatesRepository
                .Setup(x => x.CreateAsync(It.Is<ForeignExchangeRate>(
                    y => y.FromCurrencyCode == request.FromCurrencyCode &&
                         y.ToCurrencyCode == request.ToCurrencyCode)))
                .ReturnsAsync(repoResponse);
            
            this._produceMessageMock
                .Setup(x => x.ProduceAsync(repoResponse))
                .Returns(Task.CompletedTask);
            
            // Act
            var result = await this._currencyExchangeRateService.CreateAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(repoResponse, options => options.ExcludingMissingMembers());
            
            this._foreignExchangeRatesRepository.VerifyAll();
            this._createValidatorMock.VerifyAll();
        }
        
        [Fact]
        public async Task UpdateAsync_ValidRequest_ShouldNotThrowException()
        {
            // Arrange
            var request = this._fixture.Create<UpdateForeignExchangeRate>();
            var id = Guid.NewGuid();
            
            this._updateValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult());
            
            this._foreignExchangeRatesRepository
                .Setup(x => x.ExistsAsync(id))
                .ReturnsAsync(true);

            this._foreignExchangeRatesRepository
                .Setup(x => x.UpdateAsync(id, It.Is<ForeignExchangeRate>(
                    y => y.FromCurrencyCode == request.FromCurrencyCode &&
                         y.ToCurrencyCode == request.ToCurrencyCode)))
                .Returns(Task.CompletedTask);
            
            this._produceMessageMock
                .Setup(x => x.ProduceAsync(It.Is<ForeignExchangeRate>(
                    y => y.FromCurrencyCode == request.FromCurrencyCode &&
                         y.ToCurrencyCode == request.ToCurrencyCode)))
                .Returns(Task.CompletedTask);
            
            // Act
            var result = async () => await this._currencyExchangeRateService.UpdateAsync(request, id);

            // Assert
            await result.Should().NotThrowAsync();
            
            this._foreignExchangeRatesRepository.VerifyAll();
            this._updateValidatorMock.VerifyAll();
        }
        
        [Fact]
        public async Task UpdateAsync_RateNotFound_ShouldThrowException()
        {
            // Arrange
            var request = this._fixture.Create<UpdateForeignExchangeRate>();
            var id = Guid.NewGuid();
            
            this._updateValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult());
            
            this._foreignExchangeRatesRepository
                .Setup(x => x.ExistsAsync(id))
                .ReturnsAsync(false);
            
            // Act
            var result = async () => await this._currencyExchangeRateService.UpdateAsync(request, id);

            // Assert
            await result.Should().ThrowAsync<NotFoundException>()
                .Where(x => x.Message.Equals("No rate found for the provided Id."));;
            
            this._foreignExchangeRatesRepository.VerifyAll();
            this._updateValidatorMock.VerifyAll();
        }
        
        [Fact]
        public async Task DeleteAsync_RateNotFound_ShouldThrowException()
        {
            // Arrange
            var id = Guid.NewGuid();
            
            this._foreignExchangeRatesRepository
                .Setup(x => x.GetAsync(id))
                .ReturnsAsync((ForeignExchangeRate)null);
            
            // Act
            var result = async () => await this._currencyExchangeRateService.DeleteAsync(id);

            // Assert
            await result.Should().ThrowAsync<NotFoundException>()
                .Where(x => x.Message.Equals("No rate found for the provided Id."));;
            
            this._foreignExchangeRatesRepository.VerifyAll();
        }
        
        [Fact]
        public async Task DeleteAsync_RateExists_ShouldNotThrowException()
        {
            // Arrange
            var id = Guid.NewGuid();

            var repoResponse = this._fixture.Create<ForeignExchangeRate>();
            
            this._foreignExchangeRatesRepository
                .Setup(x => x.GetAsync(id))
                .ReturnsAsync(repoResponse);

            this._foreignExchangeRatesRepository
                .Setup(x => x.DeleteAsync(repoResponse))
                .Returns(Task.CompletedTask);
            
            // Act
            var result = async () => await this._currencyExchangeRateService.DeleteAsync(id);

            // Assert
            await result.Should().NotThrowAsync();
            
            this._foreignExchangeRatesRepository.VerifyAll();
        }
    }
}