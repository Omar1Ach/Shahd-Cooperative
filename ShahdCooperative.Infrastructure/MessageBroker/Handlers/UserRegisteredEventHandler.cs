using Microsoft.Extensions.Logging;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces;
using ShahdCooperative.Domain.Interfaces.Repositories;
using ShahdCooperative.Infrastructure.MessageBroker.Events;

namespace ShahdCooperative.Infrastructure.MessageBroker.Handlers;

public class UserRegisteredEventHandler : IEventHandler<UserRegisteredEvent>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        ILogger<UserRegisteredEventHandler> logger)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(UserRegisteredEvent @event)
    {
        try
        {
            _logger.LogInformation(
                "Processing UserRegisteredEvent for user {UserId} with email {Email}",
                @event.UserId,
                @event.Email);

            // Check if customer already exists
            var existingCustomer = await _customerRepository.GetByAuthIdAsync(@event.UserId.ToString());
            if (existingCustomer != null)
            {
                _logger.LogWarning(
                    "Customer with ExternalAuthId {ExternalAuthId} already exists",
                    @event.UserId);
                return;
            }

            // Extract name from email (before @)
            var name = @event.Email.Split('@')[0];

            // Create new customer
            var customer = Customer.Create(
                externalAuthId: @event.UserId.ToString(),
                name: name,
                email: @event.Email);

            await _customerRepository.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Successfully created customer {CustomerId} for user {UserId}",
                customer.Id,
                @event.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing UserRegisteredEvent for user {UserId}",
                @event.UserId);
            throw;
        }
    }
}
