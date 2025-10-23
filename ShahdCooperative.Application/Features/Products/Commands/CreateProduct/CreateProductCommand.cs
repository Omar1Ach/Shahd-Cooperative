using MediatR;
using ShahdCooperative.Application.DTOs.Products;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(CreateProductDto Product) : IRequest<Result<ProductDto>>;
