using AutoMapper;
using MediatR;
using ShahdCooperative.Application.DTOs.Feedback;
using ShahdCooperative.Domain.Common;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Features.Feedback.Commands.CreateFeedback;

public class CreateFeedbackCommandHandler : IRequestHandler<CreateFeedbackCommand, Result<FeedbackDto>>
{
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CreateFeedbackCommandHandler(
        IFeedbackRepository feedbackRepository,
        ICustomerRepository customerRepository,
        IMapper mapper)
    {
        _feedbackRepository = feedbackRepository;
        _customerRepository = customerRepository;
        _mapper = mapper;
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

            return Result<FeedbackDto>.Success(feedbackDto);
        }
        catch (ArgumentException ex)
        {
            return Result<FeedbackDto>.Failure(ex.Message, "VALIDATION_ERROR");
        }
    }
}
