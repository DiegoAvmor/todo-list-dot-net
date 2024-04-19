using FluentValidation;
using TodoListApi.Models.Data.DTO;

namespace TodoListApi.Models.Data.Validators
{
    public class UserValidator: AbstractValidator<RegistrationRequestDto>
    {
        public UserValidator(){
            RuleFor(x=>x.Email)
            .EmailAddress().WithMessage("Must follow an email format")
            .NotEmpty().WithMessage("Email cannot be empty");
            RuleFor(x=>x.Password)
            .MaximumLength(20).WithMessage("Password cannot be greater than 20")
            .NotEmpty().WithMessage("Password cannot be empty");
            RuleFor(x=>x.UserName)
            .MaximumLength(20).WithMessage("Username cannot be greater than 20")
            .NotEmpty().WithMessage("Username cannot be empty");
        }
    }
}