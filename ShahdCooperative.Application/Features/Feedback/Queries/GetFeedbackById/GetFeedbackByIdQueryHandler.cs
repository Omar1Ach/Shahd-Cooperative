using AutoMapper;
using MediatR;
using ShahdCooperative.Application.DTOs.Feedback;
using ShahdCooperative.Domain.Common;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Features.Feedback.Queries.GetFeedbackById;

public class GetFeedbackByIdQueryHandler : IRequestHandler<GetFeedbackByIdQuery, Result<FeedbackDto>>
{
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IMapper _mapper;

    public GetFeedbackByIdQueryHandler(IFeedbackRepository feedbackRepository, IMapper mapper)
    {
        _feedbackRepository = feedbackRepository;
        _mapper = mapper;
    }

    public async Task<Result<FeedbackDto>> Handle(GetFeedbackByIdQuery request, CancellationToken cancellationToken)
    {
        var feedback = await _feedbackRepository.GetByIdAsync(request.Id, cancellationToken);

        if (feedback == null)
            return Result<FeedbackDto>.Failure("Feedback not found", "NOT_FOUND");

        var feedbackDto = _mapper.Map<FeedbackDto>(feedback);
        return Result<FeedbackDto>.Success(feedbackDto);
    }
}
