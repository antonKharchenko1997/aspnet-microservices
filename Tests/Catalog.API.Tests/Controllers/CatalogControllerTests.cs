using AutoFixture;
using Catalog.API.Controllers;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.API.Tests.Controllers;

public class CatalogControllerTests
{
    private readonly CatalogController _catalogController;
    private readonly IFixture _fixture;
    private readonly Mock<IProductRepository> _productRepository;

    public CatalogControllerTests()
    {
        _fixture = new Fixture();
        _productRepository = _fixture.Freeze<Mock<IProductRepository>>();
        var logger = _fixture.Freeze<Mock<ILogger<CatalogController>>>();
        _catalogController = new CatalogController(_productRepository.Object, logger.Object);
    }

    [Fact]
    public async Task GetProductsShouldReturnOkResponseWhenDataFound()
    {
        var products = _fixture.Create<IEnumerable<Product>>();
        _productRepository.Setup(x => x.GetProductsAsync()).ReturnsAsync(products);

        var result = await _catalogController.GetProductsAsync().ConfigureAwait(false);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ActionResult<IEnumerable<Product>>>();
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        result.Result.As<OkObjectResult>().Value
            .Should()
            .NotBeNull()
            .And.BeOfType(products.GetType());
        _productRepository.Verify(x => x.GetProductsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetProductShouldReturnOkResponseWhenDataFound()
    {
        var product = _fixture.Create<Product>();
        var id = _fixture.Create<string>();
        _productRepository.Setup(x => x.GetProductByIdAsync(id)).ReturnsAsync(product);

        var result = await _catalogController.GetProductByIdAsync(id).ConfigureAwait(false);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ActionResult<Product?>>();
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        result.Result.As<OkObjectResult>().Value
            .Should()
            .NotBeNull()
            .And.BeOfType(product.GetType());
        _productRepository.Verify(x => x.GetProductByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetProductByIdShouldReturnNotFoundWhenDataNotFound()
    {
        Product? product = null;
        var id = _fixture.Create<string>();
        _productRepository.Setup(x => x.GetProductByIdAsync(id)).ReturnsAsync(product);

        var result = await _catalogController.GetProductByIdAsync(id).ConfigureAwait(false);

        result.Should().NotBeNull();
        result.Result.Should().BeAssignableTo<NotFoundResult>();
        _productRepository.Verify(x => x.GetProductByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetProductsByCategoryNameShouldReturnOkResponseWhenDataFound()
    {
        var products = _fixture.Create<IEnumerable<Product>>();
        var categoryName = _fixture.Create<string>();
        _productRepository.Setup(x => x.GetProductByCategoryNameAsync(categoryName)).ReturnsAsync(products);

        var result = await _catalogController.GetProductByCategoryNameAsync(categoryName).ConfigureAwait(false);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ActionResult<IEnumerable<Product>>>();
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        result.Result.As<OkObjectResult>().Value
            .Should()
            .NotBeNull()
            .And.BeOfType(products.GetType());
        _productRepository.Verify(x => x.GetProductByCategoryNameAsync(categoryName), Times.Once);
    }

    [Fact]
    public async Task CreateProductShouldReturnOkWhenValidRequest()
    {
        var request = _fixture.Create<Product>();
        _productRepository.Setup(x => x.CreateProductAsync(request));

        var result = await _catalogController.CreateProductAsync(request).ConfigureAwait(false);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ActionResult<Product>>();
        result.Result.Should().BeAssignableTo<CreatedAtRouteResult>();
        _productRepository.Verify(x => x.CreateProductAsync(request), Times.Once);
    }

    [Fact]
    public async Task UpdateProductShouldReturnOkWhenValidRequest()
    {
        var request = _fixture.Create<Product>();
        _productRepository.Setup(x => x.UpdateProductAsync(request));

        var result = await _catalogController.UpdateProductAsync(request).ConfigureAwait(false);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ActionResult>();
        _productRepository.Verify(x => x.UpdateProductAsync(request), Times.Once);
    }

    [Fact]
    public async Task DeleteProductShouldReturnTrueWhenDeletedRecord()
    {
        var id = _fixture.Create<string>();
        _productRepository.Setup(x => x.DeleteProductAsync(id)).ReturnsAsync(true);

        var result = await _catalogController.DeleteProductByIdAsync(id).ConfigureAwait(false);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ActionResult>();
    }
}
