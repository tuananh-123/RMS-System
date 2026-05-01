using FluentValidation;
using RMS.Dtos.Auths;

namespace RMS.CustomDtoValidators.Auths;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("Email is required");
        RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("Password is required");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Email is not valid");
    }
}