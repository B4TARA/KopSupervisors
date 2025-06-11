using FluentValidation;
using KOP.DAL.Entities;

namespace KOP.Import.Utils
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(e => e.ServiceNumber)
                .NotEmpty().WithMessage("ServiceNumber поле является обязательным");
            //.MustAsync(IsServiceNumberUniqueAsync).WithMessage("ServiceNumber поле является уникальным");

            RuleFor(e => e.FullName)
                .NotEmpty().WithMessage("FullName поле является обязательным");

            RuleFor(e => e.Position)
                .NotEmpty().WithMessage("Position поле является обязательным");

            RuleFor(e => e.SubdivisionFromFile)
                .NotEmpty().WithMessage("Subdivision поле является обязательным");

            RuleFor(e => e.GradeGroup)
                .NotEmpty().WithMessage("GradeGroup поле является обязательным");

            RuleFor(e => e.HireDate)
                .NotEmpty().WithMessage("WorkPeriod поле является обязательным");

            RuleFor(e => e.ContractStartDate)
                .NotEmpty().WithMessage("ContractStartDate поле является обязательным");

            RuleFor(e => e.ContractEndDate)
                .NotEmpty().WithMessage("ContractEndDate поле является обязательным");

            RuleFor(e => e.Login)
                .NotEmpty().WithMessage("Login поле является обязательным");

            RuleFor(e => e.Password)
                .NotEmpty().WithMessage("Password поле является обязательным");

            RuleFor(e => e.Email)
                .NotEmpty().WithMessage("Email поле является обязательным");
        }
    }
}
