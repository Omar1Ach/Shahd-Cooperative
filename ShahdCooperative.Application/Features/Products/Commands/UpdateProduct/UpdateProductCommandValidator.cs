using FluentValidation;

namespace ShahdCooperative.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Product.Id)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.Product.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Product.Category)
            .NotEmpty().WithMessage("Category is required")
            .MaximumLength(100).WithMessage("Category cannot exceed 100 characters");

        RuleFor(x => x.Product.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.Product.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be a 3-letter ISO code");

        RuleFor(x => x.Product.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative");

        RuleFor(x => x.Product.ThresholdLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Threshold level cannot be negative");
    }
}
