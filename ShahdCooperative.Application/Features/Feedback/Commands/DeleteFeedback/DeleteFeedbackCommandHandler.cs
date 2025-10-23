using MediatR;
using ShahdCooperative.Domain.Common;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Features.Feedback.Commands.DeleteFeedback;

public class DeleteFeedbackCommandHandler : IRequestHandler<DeleteFeedbackCommand, Result<bool>>
{
    private readonly IFeedbackRepository _feedbackRepository;

    public DeleteFeedbackCommandHandler(IFeedbackRepository feedbackRepository)
    {
        _feedbackRepository = feedbackRepository;
    }

    public async Task<Result<bool>> Handle(DeleteFeedbackCommand request, CancellationToken cancellationToken)
    {
        var feedback = await _feedbackRepository.GetByIdAsync(request.Id, cancellationToken);

        if (feedback == null)
            return Result<bool>.Failure("Feedback not found", "NOT_FOUND");

        await _feedbackRepository.DeleteAsync(feedback, cancellationToken);
        return Result<bool>.Success(true);
    }
}
