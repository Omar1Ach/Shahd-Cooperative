using AutoMapper;
using MediatR;
using ShahdCooperative.Application.DTOs.Products;
using ShahdCooperative.Domain.Common;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Enums;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Parse ProductType enum from string
        if (!Enum.TryParse<ProductType>(request.Product.Type, out var productType))
        {
            return Result<ProductDto>.Failure($"Invalid product type: {request.Product.Type}");
        }

        // Use factory method to create product with proper domain logic
        var product = Product.Create(
            name: request.Product.Name,
            sku: request.Product.SKU,
            category: request.Product.Category,
            type: productType,
            price: request.Product.Price,
            currency: request.Product.Currency,
            stockQuantity: request.Product.StockQuantity,
            thresholdLevel: request.Product.ThresholdLevel,
            description: request.Product.Description,
            imageUrl: request.Product.ImageUrl
        );

        var createdProduct = await _productRepository.AddAsync(product, cancellationToken);
        var productDto = _mapper.Map<ProductDto>(createdProduct);

        return Result<ProductDto>.Success(productDto);
    }
}
