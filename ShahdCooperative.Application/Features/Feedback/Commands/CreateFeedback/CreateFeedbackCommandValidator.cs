using FluentValidation;

namespace ShahdCooperative.Application.Features.Feedback.Commands.CreateFeedback;

public class CreateFeedbackCommandValidator : AbstractValidator<CreateFeedbackCommand>
{
    public CreateFeedbackCommandValidator()
    {
        RuleFor(x => x.Feedback.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.Feedback.Content)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(2000).WithMessage("Content cannot exceed 2000 characters");

        RuleFor(x => x.Feedback.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");
    }
}
