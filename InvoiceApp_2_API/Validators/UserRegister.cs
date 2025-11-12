using FluentValidation;
using MyInvoiceApp_Shared.Model;

namespace MyInvoiceApp_API.Validators
{
    public class UserRegisterValidator : AbstractValidator<Register>
    {
        public UserRegisterValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name is required")
                .MaximumLength(200);

            RuleFor(x => x.SSM_No)
               .NotEmpty().WithMessage("SSM registration number is required")
               .Must(CheckSSMNoLength).WithMessage("SSM number must be exactly 12 digits");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match");

        }

        private bool CheckSSMNoLength(long ssmNo)
        {
            return ssmNo.ToString().Length == 12;
        }
    }
}
