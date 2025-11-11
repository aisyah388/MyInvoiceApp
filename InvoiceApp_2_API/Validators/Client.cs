using FluentValidation;
using MyInvoiceApp.Shared.Model;

namespace MyInvoiceApp_API.Validators
{
    public class ClientValidator : AbstractValidator<Client>
    {
        public ClientValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Company name is required.")
                .MaximumLength(200)
                .WithMessage("Company name cannot exceed 200 characters.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.")
                .MaximumLength(100)
                .WithMessage("Email cannot exceed 100 characters.");

            RuleFor(x => x.Phone)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Matches(@"^[\d\s\-\+\(\)]+$")
                .WithMessage("Phone number can only contain digits, spaces, +, -, and parentheses.")
                .MinimumLength(7)
                .WithMessage("Phone number must be at least 7 characters.")
                .MaximumLength(20)
                .WithMessage("Phone number cannot exceed 20 characters.");

            RuleFor(x => x.Address)
                .MaximumLength(500)
                .WithMessage("Address cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Address));
        }
    }
}