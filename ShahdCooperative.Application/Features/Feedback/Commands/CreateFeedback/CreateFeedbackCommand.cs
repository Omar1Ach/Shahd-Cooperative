using MediatR;
using ShahdCooperative.Application.DTOs.Feedback;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Application.Features.Feedback.Commands.CreateFeedback;

public record CreateFeedbackCommand(CreateFeedbackDto Feedback) : IRequest<Result<FeedbackDto>>;
