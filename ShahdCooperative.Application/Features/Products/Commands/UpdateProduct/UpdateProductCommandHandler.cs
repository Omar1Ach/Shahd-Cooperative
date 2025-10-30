using AutoMapper;
using MediatR;
using ShahdCooperative.Application.DTOs.Products;
using ShahdCooperative.Application.Events;
using ShahdCooperative.Domain.Common;
using ShahdCooperative.Domain.Interfaces;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        IMapper mapper,
        IEventPublisher eventPublisher)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }

    public async Task<Result<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var existingProduct = await _productRepository.GetByIdAsync(request.Product.Id, cancellationToken);
        if (existingProduct == null)
            return Result<ProductDto>.NotFound("Product not found");

        var oldStockQuantity = existingProduct.StockQuantity;

        // Use domain business methods instead of AutoMapper
        existingProduct.UpdateDetails(
            name: request.Product.Name,
            category: request.Product.Category,
            price: request.Product.Price,
            description: request.Product.Description,
            imageUrl: request.Product.ImageUrl
        );

        // Update stock if changed
        if (existingProduct.StockQuantity != request.Product.StockQuantity)
        {
            existingProduct.UpdateStock(request.Product.StockQuantity, "Stock updated via API");
        }

        // Update threshold level if changed
        if (existingProduct.ThresholdLevel != request.Product.ThresholdLevel)
        {
            existingProduct.UpdateThresholdLevel(request.Product.ThresholdLevel);
        }

        // Handle active status
        if (request.Product.IsActive && !existingProduct.IsActive)
        {
            existingProduct.Activate();
        }
        else if (!request.Product.IsActive && existingProduct.IsActive)
        {
            existingProduct.Deactivate();
        }

        await _productRepository.UpdateAsync(existingProduct, cancellationToken);

        // Check if stock fell below threshold and publish event
        if (existingProduct.StockQuantity <= existingProduct.ThresholdLevel &&
            oldStockQuantity > existingProduct.ThresholdLevel)
        {
            var productOutOfStockEvent = new ProductOutOfStockEvent
            {
                ProductId = existingProduct.Id,
                ProductName = existingProduct.Name,
                SKU = existingProduct.SKU,
                CurrentStock = existingProduct.StockQuantity,
                ThresholdLevel = existingProduct.ThresholdLevel,
                DetectedAt = DateTime.UtcNow
            };

            await _eventPublisher.PublishAsync("product.out-of-stock", productOutOfStockEvent, cancellationToken);
        }

        var productDto = _mapper.Map<ProductDto>(existingProduct);
        return Result<ProductDto>.Success(productDto);
    }
}
