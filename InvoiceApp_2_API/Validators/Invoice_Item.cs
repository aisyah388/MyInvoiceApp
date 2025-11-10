using FluentValidation;
using MyInvoiceApp.Shared.Model;

namespace MyInvoiceApp_API.Validators
{
    public class Invoice_ItemValidator : AbstractValidator<Invoice_Item>
    {
        public Invoice_ItemValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Item description is required.")
                .MaximumLength(500)
                .WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero.");

            RuleFor(x => x.Unit_Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Unit price cannot be negative.")
                .PrecisionScale(16, 2, false)
                .WithMessage("Unit price can have maximum 2 decimal places.");
        }
    }
}