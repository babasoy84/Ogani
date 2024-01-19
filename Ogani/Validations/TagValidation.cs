using Ogani.ViewModels;
using FluentValidation;

namespace Ogani.Validations
{
    public class TagValidation : AbstractValidator<TagAddViewModel>
    {
        public TagValidation()
        {
            RuleFor(t => t.Name).NotEmpty();
        }
    }
}
