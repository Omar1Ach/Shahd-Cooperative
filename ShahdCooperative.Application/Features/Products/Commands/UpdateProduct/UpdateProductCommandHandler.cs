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

        _mapper.Map(request.Product, existingProduct);
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
