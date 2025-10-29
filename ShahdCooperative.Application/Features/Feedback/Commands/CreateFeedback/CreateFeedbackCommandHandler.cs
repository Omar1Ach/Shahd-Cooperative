using AutoMapper;
using MediatR;
using ShahdCooperative.Application.DTOs.Feedback;
using ShahdCooperative.Application.Events;
using ShahdCooperative.Domain.Common;
using ShahdCooperative.Domain.Interfaces;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Features.Feedback.Commands.CreateFeedback;

public class CreateFeedbackCommandHandler : IRequestHandler<CreateFeedbackCommand, Result<FeedbackDto>>
{
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;

    public CreateFeedbackCommandHandler(
        IFeedbackRepository feedbackRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        IMapper mapper,
        IEventPublisher eventPublisher)
    {
        _feedbackRepository = feedbackRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }

    public async Task<Result<FeedbackDto>> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
    {
        // Validate customer exists
        var customer = await _customerRepository.GetByIdAsync(request.Feedback.CustomerId, cancellationToken);
        if (customer == null)
            return Result<FeedbackDto>.Failure("Customer not found", "CUSTOMER_NOT_FOUND");

        try
        {
            var feedback = Domain.Entities.Feedback.Create(
                request.Feedback.CustomerId,
                request.Feedback.Content,
                request.Feedback.Rating,
                request.Feedback.ProductId,
                request.Feedback.OrderId);

            var createdFeedback = await _feedbackRepository.AddAsync(feedback, cancellationToken);
            var feedbackDto = _mapper.Map<FeedbackDto>(createdFeedback);

            // Get product name if product feedback
            string? productName = null;
            if (request.Feedback.ProductId.HasValue)
            {
                var product = await _productRepository.GetByIdAsync(request.Feedback.ProductId.Value, cancellationToken);
                productName = product?.Name;
            }

            // Publish FeedbackReceivedEvent to RabbitMQ for NotificationService
            var feedbackReceivedEvent = new FeedbackReceivedEvent
            {
                FeedbackId = createdFeedback.Id,
                CustomerId = customer.Id,
                CustomerEmail = customer.Email,
                CustomerName = customer.Name,
                ProductId = request.Feedback.ProductId,
                ProductName = productName,
                OrderId = request.Feedback.OrderId,
                Content = request.Feedback.Content,
                Rating = request.Feedback.Rating,
                SubmittedAt = DateTime.UtcNow
            };

            await _eventPublisher.PublishAsync("feedback.received", feedbackReceivedEvent, cancellationToken);

            return Result<FeedbackDto>.Success(feedbackDto);
        }
        catch (ArgumentException ex)
        {
            return Result<FeedbackDto>.Failure(ex.Message, "VALIDATION_ERROR");
        }
    }
}
