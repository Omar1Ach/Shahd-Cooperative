using FluentValidation;

namespace ShahdCooperative.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Product.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Product.SKU)
            .NotEmpty().WithMessage("SKU is required")
            .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters");

        RuleFor(x => x.Product.Category)
            .NotEmpty().WithMessage("Category is required")
            .MaximumLength(100).WithMessage("Category cannot exceed 100 characters");

        RuleFor(x => x.Product.Type)
            .NotEmpty().WithMessage("Product type is required")
            .Must(type => new[] { "Honey", "BeeswaxProduct", "Equipment", "Other" }.Contains(type))
            .WithMessage("Invalid product type");

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
