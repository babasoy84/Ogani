using Ogani.ViewModels;
using FluentValidation;

namespace Ogani.Validations
{
    public class CategoryValidation : AbstractValidator<CategoryAddViewModel>
    {
        public CategoryValidation()
        {
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.Name).MaximumLength(20);
        }
    }
}
