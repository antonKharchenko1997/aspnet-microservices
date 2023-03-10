using System.Net;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

/// <summary>
///     Catalog controller. Endpoints for swagger.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ILogger<CatalogController> _logger;
    private readonly IProductRepository _productRepository;

    /// <summary>
    ///     .ctor
    /// </summary>
    /// <param name="productRepository">Repository for interaction with Mongo db.</param>
    /// <param name="logger">Logger.</param>
    public CatalogController(IProductRepository productRepository, ILogger<CatalogController> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    ///     Gets the existed list of products.
    /// </summary>
    /// <returns>The list of products.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsAsync()
    {
        return Ok(await _productRepository.GetProductsAsync().ConfigureAwait(false));
    }

    /// <summary>
    ///     Gets the product associated with the specified ID.
    /// </summary>
    /// <param name="id">ID of product.</param>
    /// <returns>The list of products.</returns>
    [HttpGet("{id:length(24)}", Name = "GetProduct")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Product>> GetProductByIdAsync(string id)
    {
        var product = await _productRepository.GetProductByIdAsync(id).ConfigureAwait(false);
        if (product is not null) return Ok(product);
        _logger.LogError($"Product with id: {id}, not found.");
        return NotFound();
    }

    /// <summary>
    ///     Gets the product associated with the category name.
    /// </summary>
    /// <param name="category">Category name of product.</param>
    /// <returns>The list of products.</returns>
    [HttpGet]
    [Route("[action]/{category}", Name = "GetProductByCategory")]
    [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategoryNameAsync(string category)
    {
        return Ok(await _productRepository.GetProductByCategoryNameAsync(category).ConfigureAwait(false));
    }

    /// <summary>
    ///     Create a new product.
    /// </summary>
    /// <param name="product">The body of a product.</param>
    /// <returns>A new product.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Product>> CreateProductAsync([FromBody] Product product)
    {
        await _productRepository.CreateProductAsync(product).ConfigureAwait(false);

        return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
    }

    /// <summary>
    ///     Update the existed product.
    /// </summary>
    /// <param name="product">Updated body of th product.</param>
    [HttpPut]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateProductAsync([FromBody] Product product)
    {
        return Ok(await _productRepository.UpdateProductAsync(product).ConfigureAwait(false));
    }

    /// <summary>
    ///     Delete the product associated with the specified ID.
    /// </summary>
    /// <param name="id">ID of product.</param>
    [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteProductByIdAsync(string id)
    {
        return Ok(await _productRepository.DeleteProductAsync(id).ConfigureAwait(false));
    }
}
