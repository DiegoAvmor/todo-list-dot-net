using FluentValidation;
using TodoListApi.Models.Data.DTO;

namespace TodoListApi.Models.Data.Validators
{
    public class TodoTaskValidator : AbstractValidator<TodoTaskRequestDto>
    {
        public TodoTaskValidator(){
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty")
                .NotNull().WithMessage("Title cannot be null");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description cannot be empty")
                .NotNull().WithMessage("Description cannot be null");
        }
    }
}