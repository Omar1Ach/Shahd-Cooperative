using MediatR;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Application.Features.Feedback.Commands.DeleteFeedback;

public record DeleteFeedbackCommand(Guid Id) : IRequest<Result<bool>>;
