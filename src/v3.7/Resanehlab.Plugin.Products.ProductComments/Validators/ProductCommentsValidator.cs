using FluentValidation;
using Resanehlab.Plugin.Products.ProductComments.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Resanehlab.Plugin.Products.ProductComments.Validators
{
    public partial class ProductCommentsValidator : BaseNopValidator<ProductCommentsModel>
    {
        public ProductCommentsValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AddProductComment.CommentText).NotEmpty().WithMessage(localizationService.GetResource("Comments.Fields.CommentText.Required")).When(x => x.AddProductComment != null);
        }
    }
}