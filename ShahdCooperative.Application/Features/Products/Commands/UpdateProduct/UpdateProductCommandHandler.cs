using AutoMapper;
using MediatR;
using ShahdCooperative.Application.DTOs.Products;
using ShahdCooperative.Domain.Common;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var existingProduct = await _productRepository.GetByIdAsync(request.Product.Id, cancellationToken);
        if (existingProduct == null)
            return Result<ProductDto>.NotFound("Product not found");

        _mapper.Map(request.Product, existingProduct);
        await _productRepository.UpdateAsync(existingProduct, cancellationToken);

        var productDto = _mapper.Map<ProductDto>(existingProduct);
        return Result<ProductDto>.Success(productDto);
    }
}
