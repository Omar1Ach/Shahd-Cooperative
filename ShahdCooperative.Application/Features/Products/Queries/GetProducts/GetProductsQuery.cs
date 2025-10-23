using MediatR;
using ShahdCooperative.Application.DTOs.Products;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Application.Features.Products.Queries.GetProducts;

public record GetProductsQuery : IRequest<Result<IEnumerable<ProductDto>>>;
