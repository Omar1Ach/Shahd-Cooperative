using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShahdCooperative.Application.DTOs.Products;
using ShahdCooperative.Application.Features.Products.Commands.CreateProduct;
using ShahdCooperative.Application.Features.Products.Commands.DeleteProduct;
using ShahdCooperative.Application.Features.Products.Commands.UpdateProduct;
using ShahdCooperative.Application.Features.Products.Queries.GetProductById;
using ShahdCooperative.Application.Features.Products.Queries.GetProducts;

namespace ShahdCooperative.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all products");

        var query = new GetProductsQuery();
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error, errorCode = result.ErrorCode });

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProduct(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting product {ProductId}", id);

        var query = new GetProductByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorCode == "NOT_FOUND")
                return NotFound(new { error = result.Error, errorCode = result.ErrorCode });

            return BadRequest(new { error = result.Error, errorCode = result.ErrorCode });
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductDto dto,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new product: {ProductName}", dto.Name);

        var command = new CreateProductCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error, errorCode = result.ErrorCode });

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = result.Value!.Id },
            result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductDto dto,
        CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest(new { error = "Product ID mismatch", errorCode = "ID_MISMATCH" });

        _logger.LogInformation("Updating product {ProductId}", id);

        var command = new UpdateProductCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorCode == "NOT_FOUND")
                return NotFound(new { error = result.Error, errorCode = result.ErrorCode });

            return BadRequest(new { error = result.Error, errorCode = result.ErrorCode });
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting product {ProductId}", id);

        var command = new DeleteProductCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorCode == "NOT_FOUND")
                return NotFound(new { error = result.Error, errorCode = result.ErrorCode });

            return BadRequest(new { error = result.Error, errorCode = result.ErrorCode });
        }

        return NoContent();
    }
}
