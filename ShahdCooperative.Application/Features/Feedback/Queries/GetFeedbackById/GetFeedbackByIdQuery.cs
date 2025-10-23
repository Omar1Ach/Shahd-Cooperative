using MediatR;
using ShahdCooperative.Application.DTOs.Feedback;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Application.Features.Feedback.Queries.GetFeedbackById;

public record GetFeedbackByIdQuery(Guid Id) : IRequest<Result<FeedbackDto>>;
