using AutoFixture;
using AutoMapper;
using Basket.API.Controllers;
using Basket.API.Entities;
using Basket.API.Repositories;
using Basket.API.Services;
using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Basket.API.Tests.Controllers;

public class BasketControllerTests
{
    private readonly BasketController _basketController;
    private readonly IFixture _fixture;
    private readonly Mock<IBasketRepository> _basketRepository;

    public BasketControllerTests()
    {
        _fixture = new Fixture();
        _basketRepository = _fixture.Freeze<Mock<IBasketRepository>>();
        var discountGrpcService = _fixture.Freeze<Mock<DiscountGrpcService>>();
        var endpoint = _fixture.Freeze<Mock<IPublishEndpoint>>();
        var mapper = _fixture.Freeze<Mock<IMapper>>();

        _basketController = new BasketController(_basketRepository.Object, mapper.Object, endpoint.Object, discountGrpcService.Object);
    }

    [Fact]
    public async Task GetBasketShouldReturnOkResponseWhenDataFound()
    {
        var basket = _fixture.Create<ShoppingCart>();
        var userName = _fixture.Create<string>();
        _basketRepository.Setup(i => i.GetBasketAsync(userName)).ReturnsAsync(basket);

        var result = await _basketController.GetBasketAsync(userName).ConfigureAwait(false);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ActionResult<ShoppingCart?>>();
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        result.Result.As<OkObjectResult>().Value
              .Should()
              .NotBeNull()
              .And.BeOfType(basket.GetType());
        _basketRepository.Verify(i => i.GetBasketAsync(userName), Times.Once);
    }


    [Fact]
    public async Task UpdateBasketShouldReturnOkResponseWhenDataValid()
    {
        var basket = _fixture.Create<ShoppingCart>();
        _basketRepository.Setup(i => i.UpdateBasketAsync(basket)).ReturnsAsync(basket);

        var result = await _basketController.UpdateBasketAsync(basket).ConfigureAwait(false);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ActionResult<ShoppingCart?>>();
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        result.Result.As<OkObjectResult>().Value
              .Should()
              .NotBeNull()
              .And.BeOfType(basket.GetType());
        _basketRepository.Verify(i => i.UpdateBasketAsync(basket), Times.Once);
    }


    [Fact]
    public async Task DeleteBasketShouldReturnOkWhenDeletedRecord()
    {
        var userName = _fixture.Create<string>();
        _basketRepository.Setup(x => x.DeleteBasketAsync(userName));

        var result = await _basketController.DeleteBasketAsync(userName).ConfigureAwait(false);

        result.Should().NotBeNull();
    }
}
