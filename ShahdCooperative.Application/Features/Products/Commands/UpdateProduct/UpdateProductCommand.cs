using MediatR;
using ShahdCooperative.Application.DTOs.Products;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(UpdateProductDto Product) : IRequest<Result<ProductDto>>;
