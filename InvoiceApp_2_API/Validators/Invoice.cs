using FluentValidation;
using MyInvoiceApp.Shared.Model;

namespace MyInvoiceApp_API.Validators
{
    public class InvoiceValidator : AbstractValidator<Invoice>
    {
        public InvoiceValidator()
        {
            // Client validation
            RuleFor(x => x.Client_Id)
                .NotEmpty()
                .WithMessage("Client is required.");

            // Status validation
            RuleFor(x => x.Status_Id)
                .NotEmpty()
                .WithMessage("Payment status is required.");

            // Issue Date validation
            RuleFor(x => x.Issue_Date)
                .NotEmpty()
                .WithMessage("Issue date is required.")
                .Must(BeAValidDate)
                .WithMessage("Issue date must be a valid date.");

            // Due Date validation
            RuleFor(x => x.Due_Date)
                .NotEmpty()
                .WithMessage("Due date is required.")
                .Must(BeAValidDate)
                .WithMessage("Due date must be a valid date.");

            // Due date must be after issue date
            RuleFor(x => x)
                .Must(HaveDueDateAfterIssueDate)
                .WithMessage("Due date must be after or equal to issue date.")
                .When(x => x.Issue_Date.HasValue && x.Due_Date.HasValue);

            // Items validation
            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("Invoice must contain at least one item.");

            // Validate each item in the collection
            RuleForEach(x => x.Items)
                .SetValidator(new Invoice_ItemValidator())
                .When(x => x.Items != null && x.Items.Any());

            // At least one item with quantity > 0
            RuleFor(x => x.Items)
                .Must(items => items != null && items.Any(i => i.Quantity > 0))
                .WithMessage("Invoice must have at least one item with quantity greater than zero.")
                .When(x => x.Items != null);
        }

        private bool BeAValidDate(DateTime? date)
        {
            if (!date.HasValue)
                return false;

            return date.Value.Year >= 2000 && date.Value.Year <= DateTime.UtcNow.Year + 10;
        }

        private bool HaveDueDateAfterIssueDate(Invoice invoice)
        {
            if (!invoice.Issue_Date.HasValue || !invoice.Due_Date.HasValue)
                return true;

            return invoice.Due_Date >= invoice.Issue_Date;
        }
    }
}