using FluentValidation;
using Ogani.ViewModels;

namespace Ogani.Validations
{
    public class ProductUpdateValidation : AbstractValidator<ProductUpdateViewModel>
    {
        public ProductUpdateValidation()
        {
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.Description).NotEmpty();
            RuleFor(p => p.Price).GreaterThan(0);
            RuleFor(p => p.Tags).NotNull();
            RuleFor(p => p.CategoryId).NotNull();
        }
    }
}
