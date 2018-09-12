using Resanehlab.Plugin.Products.ProductComments.Domain;
using Resanehlab.Plugin.Products.ProductComments.Models;
using Resanehlab.Plugin.Products.ProductComments.Services;
using Resanehlab.Plugin.Products.ProductComments.Settings;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Seo;
using Resanehlab.Plugin.Products.ProductComments.Factories;
using Nop.Core.Infrastructure;

namespace Resanehlab.Plugin.Products.ProductComments.Controllers
{
    public class ProductCommentsController : BasePluginController
    {
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly IProductCommentService _productCommentService;
        private readonly IWorkContext _workContext;
        private ProductCommentsSetting _productCommentsSetting;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IStoreContext _storeContext;
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerService _customerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IProductCommentModelFactory _productCommentModelFactory;

        public ProductCommentsController(IPermissionService permissionService,
            IProductService productService,
            IProductCommentService productCommentService,
            IWorkContext workContext,
            ProductCommentsSetting productCommentsSetting,
            ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper,
            IStoreContext storeContext,
            CustomerSettings customerSettings,
            ICustomerService customerService,
            IUrlRecordService urlRecordService,
            IProductCommentModelFactory productCommentModelFactory)
        {
            this._permissionService = permissionService;
            this._productService = productService;
            this._productCommentService = productCommentService;
            this._workContext = workContext;
            this._productCommentsSetting = productCommentsSetting;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._storeContext = storeContext;
            this._customerSettings = customerSettings;
            this._customerService = customerService;
            this._urlRecordService = urlRecordService;
            this._productCommentModelFactory = productCommentModelFactory;
        }

        #region Utilities

        private void InsertProductComment(ProductCommentsModel productCommentsModel, Product product)
        {
            bool productCommentsMustBeApproved = !_productCommentsSetting.ProductCommentsMustBeApproved;
            ProductComment productComment = new ProductComment()
            {
                ProductId = product.Id,
                CustomerId = _workContext.CurrentCustomer.Id,
                CommentText = productCommentsModel.AddProductComment.CommentText,
                HelpfulYesTotal = 0,
                HelpfulNoTotal = 0,
                IsApproved = productCommentsMustBeApproved,
                CreatedOnUtc = DateTime.UtcNow,
                StoreId = _storeContext.CurrentStore.Id,
                Visited = false
            };

            _productCommentService.InsertProductComment(productComment);

            _productCommentModelFactory.PrepareProductCommentsModel(productCommentsModel, product);
            productCommentsModel.AddProductComment.CommentText = null;
            productCommentsModel.AddProductComment.SuccessfullyAdded = true;
            if (!productCommentsMustBeApproved)
            {
                productCommentsModel.AddProductComment.Result = _localizationService.GetResource("Comments.SeeAfterApproving");
                return;
            }
            productCommentsModel.AddProductComment.Result = _localizationService.GetResource("Comments.SuccessfullyAdded");
        }

        #endregion

        #region Methods

        public ActionResult ProductComments(string widgetZone, object additionalData = null)
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

        [HttpPost]
        public ActionResult ProductCommentsAddNew(int id, ProductCommentsModel model)
        {
            Product productById = _productService.GetProductById(id);
            if (productById == null || productById.Deleted || !productById.Published || !_productCommentsSetting.EnablePlugin)
            {
                return Content("");
            }
            if (base.ModelState.IsValid)
            {
                if (!_workContext.CurrentCustomer.IsGuest() || _productCommentsSetting.AllowAnonymousUsersToCommentProduct)
                {
                    InsertProductComment(model, productById);
                    return PartialView("~/Plugins/Resanehlab.ProductComments/Views/_ProductComments.cshtml", model);
                }
                base.ModelState.AddModelError("", _localizationService.GetResource("Comments.OnlyRegisteredUsersCanWriteComments"));
            }

            _productCommentModelFactory.PrepareProductCommentsModel(model, productById);
            return PartialView("~/Plugins/Resanehlab.ProductComments/Views/_ProductComments.cshtml", model);
        }

        [HttpPost]
        public ActionResult SetProductCommentHelpfulness(int productCommentId, bool washelpful)
        {
            var productComment = _productCommentService.GetProductCommentById(productCommentId);
            if (productComment == null)
                throw new ArgumentException("No product comment found with the specified id");

            if (!_productCommentsSetting.EnablePlugin)
                return Content("");

            if (_workContext.CurrentCustomer.IsGuest() && !_productCommentsSetting.AllowAnonymousUsersToCommentProduct)
            {
                return Json(new
                {
                    Result = _localizationService.GetResource("Comments.Helpfulness.OnlyRegistered"),
                    TotalYes = productComment.HelpfulYesTotal,
                    TotalNo = productComment.HelpfulNoTotal
                });
            }

            //customers aren't allowed to vote for their own comments
            if (productComment.CustomerId == _workContext.CurrentCustomer.Id)
            {
                return Json(new
                {
                    Result = _localizationService.GetResource("Comments.Helpfulness.YourOwnComment"),
                    TotalYes = productComment.HelpfulYesTotal,
                    TotalNo = productComment.HelpfulNoTotal
                });
            }

            //delete previous helpfulness
            var prh = productComment.ProductCommentHelpfulnessEntries
                .FirstOrDefault(x => x.CustomerId == _workContext.CurrentCustomer.Id);
            if (prh != null)
            {
                //existing one
                prh.WasHelpful = washelpful;
            }
            else
            {
                //insert new helpfulness
                prh = new ProductCommentHelpfulness
                {
                    ProductCommentId = productComment.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    WasHelpful = washelpful,
                };
                productComment.ProductCommentHelpfulnessEntries.Add(prh);
            }
            _productCommentService.UpdateProductComment(productComment);

            //new totals
            productComment.HelpfulYesTotal = productComment.ProductCommentHelpfulnessEntries.Count(x => x.WasHelpful);
            productComment.HelpfulNoTotal = productComment.ProductCommentHelpfulnessEntries.Count(x => !x.WasHelpful);
            _productCommentService.UpdateProductComment(productComment);

            return Json(new
            {
                Result = _localizationService.GetResource("Comments.Helpfulness.SuccessfullyVoted"),
                TotalYes = productComment.HelpfulYesTotal,
                TotalNo = productComment.HelpfulNoTotal
            });
        }

        #endregion
    }
}
