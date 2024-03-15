using VFXChallenge.Api.Controllers;
namespace VFXChallenge.Api.Tests.Controllers
{
    using Application.Abstractions;
    using Application.DTO;

    using AutoFixture;

    using FluentAssertions;

    using Microsoft.AspNetCore.Mvc;

    using Moq;
    public class ForeignExchangeRatesControllerTest
    {
        private readonly Mock<ICurrencyExchangeRateService> _currencyExchangeRateServiceMock;

        private readonly IFixture _fixture;

        private readonly ForeignExchangeRatesController _controller;

        public ForeignExchangeRatesControllerTest()
        {
            this._fixture = new Fixture();
            this._currencyExchangeRateServiceMock = new Mock<ICurrencyExchangeRateService>(MockBehavior.Strict);
            this._controller = new ForeignExchangeRatesController(this._currencyExchangeRateServiceMock.Object);
        }
        
        [Fact]
        public async Task GetAsync_ValidRequest_ShouldReturnOkResult()
        {
            // Arrange
            var fromCurrency = this._fixture.Create<string>();
            var toCurrency = this._fixture.Create<string>();

            var serviceResponse = this._fixture.Create<ForeignExchangeRateResponse>();

            this._currencyExchangeRateServiceMock
                .Setup(x => x.GetAsync(fromCurrency, toCurrency))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await this._controller.GetAsync(fromCurrency, toCurrency);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(serviceResponse);
            
            this._currencyExchangeRateServiceMock.VerifyAll();
        }
        
        [Fact]
        public async Task PostAsync_ValidRequest_ShouldReturnCreated()
        {
            // Arrange
            var request = this._fixture.Create<CreateForeignExchangeRate>();

            var serviceResponse = this._fixture.Create<ForeignExchangeRateResponse>();

            this._currencyExchangeRateServiceMock
                .Setup(x => x.CreateAsync(request))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await this._controller.PostAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<CreatedAtRouteResult>().Which.Value.Should().BeEquivalentTo(serviceResponse);
            
            this._currencyExchangeRateServiceMock.VerifyAll();
        }
        
        [Fact]
        public async Task PutAsync_ValidRequest_ShouldReturnNoContent()
        {
            // Arrange
            var request = this._fixture.Create<UpdateForeignExchangeRate>();
            var id = Guid.NewGuid();

            this._currencyExchangeRateServiceMock
                .Setup(x => x.UpdateAsync(request, id))
                .Returns(Task.CompletedTask);

            // Act
            var result = await this._controller.PutAsync(id, request);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
            
            this._currencyExchangeRateServiceMock.VerifyAll();
        }
        
        [Fact]
        public async Task DeleteAsync_ValidRequest_ShouldReturnNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();

            this._currencyExchangeRateServiceMock
                .Setup(x => x.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            // Act
            var result = await this._controller.DeleteAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
            
            this._currencyExchangeRateServiceMock.VerifyAll();
        }
    }
}