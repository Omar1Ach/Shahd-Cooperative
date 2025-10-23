using MediatR;
using ShahdCooperative.Application.DTOs.Feedback;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Application.Features.Feedback.Queries.GetFeedback;

public record GetFeedbackQuery : IRequest<Result<IEnumerable<FeedbackDto>>>;
