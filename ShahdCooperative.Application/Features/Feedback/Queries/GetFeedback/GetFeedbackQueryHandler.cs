using AutoMapper;
using MediatR;
using ShahdCooperative.Application.DTOs.Feedback;
using ShahdCooperative.Domain.Common;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Features.Feedback.Queries.GetFeedback;

public class GetFeedbackQueryHandler : IRequestHandler<GetFeedbackQuery, Result<IEnumerable<FeedbackDto>>>
{
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IMapper _mapper;

    public GetFeedbackQueryHandler(IFeedbackRepository feedbackRepository, IMapper mapper)
    {
        _feedbackRepository = feedbackRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<FeedbackDto>>> Handle(GetFeedbackQuery request, CancellationToken cancellationToken)
    {
        var feedbacks = await _feedbackRepository.GetAllAsync(cancellationToken);
        var feedbackDtos = _mapper.Map<IEnumerable<FeedbackDto>>(feedbacks);
        return Result<IEnumerable<FeedbackDto>>.Success(feedbackDtos);
    }
}
