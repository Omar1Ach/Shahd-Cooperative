using MediatR;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Application.Features.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest<Result<bool>>;
