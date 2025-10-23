using MediatR;
using Microsoft.Extensions.Logging;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that wraps command execution in a database transaction.
/// Automatically commits on success and rolls back on failure.
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Skip transaction for queries (convention: queries don't modify data)
        if (IsQuery(requestName))
        {
            _logger.LogDebug("Skipping transaction for query: {RequestName}", requestName);
            return await next();
        }

        _logger.LogInformation("Beginning transaction for {RequestName}", requestName);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var response = await next();

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Transaction committed successfully for {RequestName}", requestName);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed for {RequestName}. Rolling back...", requestName);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private static bool IsQuery(string requestName)
    {
        // Convention: Queries typically have "Query" in their name
        return requestName.Contains("Query", StringComparison.OrdinalIgnoreCase) ||
               requestName.Contains("Get", StringComparison.OrdinalIgnoreCase);
    }
}
