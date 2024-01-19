using Ogani.ViewModels;
using FluentValidation;

namespace Ogani.Validations
{
    public class ProductValidation : AbstractValidator<ProductAddViewModel>
    {
        public ProductValidation()
        {
            RuleFor(p => p.ImageUrl).NotEmpty();
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.Description).NotEmpty();
            RuleFor(p => p.Price).GreaterThan(0);
            RuleFor(p => p.Tags).NotNull();
            RuleFor(p => p.CategoryId).NotNull();
        }
    }
}
