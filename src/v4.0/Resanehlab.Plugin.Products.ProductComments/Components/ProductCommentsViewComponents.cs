using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using Resanehlab.Plugin.Products.ProductComments.Factories;
using Resanehlab.Plugin.Products.ProductComments.Models;
using Resanehlab.Plugin.Products.ProductComments.Services;
using Resanehlab.Plugin.Products.ProductComments.Settings;

namespace Resanehlab.Plugin.Products.ProductComments.Components
{
    [ViewComponent(Name = "ProductComments")]
    public class ProductCommentsViewComponents : NopViewComponent
    {
        private readonly IProductService _productService;
        private readonly IProductCommentService _productCommentService;
        private readonly IWorkContext _workContext;
        private ProductCommentsSetting _productCommentsSetting;
        private readonly IProductCommentModelFactory _productCommentModelFactory;
        private readonly ILocalizationService _localizationService;

        public ProductCommentsViewComponents(IProductService productService,
            IProductCommentService productCommentService,
            IWorkContext workContext,
            ProductCommentsSetting productCommentsSetting,
            IProductCommentModelFactory productCommentModelFactory,
            ILocalizationService localizationService)
        {
            this._productService = productService;
            this._productCommentService = productCommentService;
            this._workContext = workContext;
            this._productCommentsSetting = productCommentsSetting;
            this._productCommentModelFactory = productCommentModelFactory;
            this._localizationService = localizationService;
        }
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (additionalData == null)
                return Content("");

            var product = _productService.GetProductById((int)additionalData);
            if (product == null || product.Deleted || !product.Published)
                return Content("");

            if (!_productCommentsSetting.EnablePlugin)
                return Content("");

            var model = new ProductCommentsModel();
            _productCommentModelFactory.PrepareProductCommentsModel(model, product);
            //only registered users can leave comments
            if (_workContext.CurrentCustomer.IsGuest() && !_productCommentsSetting.AllowAnonymousUsersToCommentProduct)
                ModelState.AddModelError("", _localizationService.GetResource("Comments.OnlyRegisteredUsersCanWriteComments"));

            return View("~/Plugins/Resanehlab.ProductComments/Views/_ProductComments.cshtml", model);
        }
    }
}
