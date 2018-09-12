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
using Nop.Services.Seo;
using Nop.Web.Framework.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;

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


        public ProductCommentsController(IPermissionService permissionService,
            IProductService productService,
            IProductCommentService productCommentService,
            IWorkContext workContext,
            ProductCommentsSetting productCommentsSetting,
            ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper,
            IStoreContext storeContext,
            CustomerSettings customerSettings,
            ICustomerService customerService)
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
        }

        #region Utilities

        [NonAction]
        protected virtual void PrepareProductCommentsModel(ProductCommentsModel model, Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (model == null)
                throw new ArgumentNullException("model");

            model.ProductId = product.Id;
            model.ProductName = product.GetLocalized(x => x.Name);
            model.ProductSeName = product.GetSeName();

            var productComments = _productCommentService.GetAllProductComments(isApproved:true, productId: product.Id, storeId: _storeContext.CurrentStore.Id);
            foreach (var pr in productComments)
            {
                var customer = pr.CustomerId.HasValue ? _customerService.GetCustomerById(pr.CustomerId.Value) : null;
                model.Items.Add(new ProductCommentModel
                {
                    Id = pr.Id,
                    CustomerId = pr.CustomerId,
                    CustomerName = customer !=null ? customer.FormatUserName() : _localizationService.GetResource("Customer.Guest"),
                    AllowViewingProfiles = customer != null && !customer.IsGuest() && _customerSettings.AllowViewingProfiles,
                    CommentText = pr.CommentText,
                    ReplyText = pr.ReplyText,
                    Helpfulness = new ProductCommentHelpfulnessModel
                    {
                        ProductCommentId = pr.Id,
                        HelpfulYesTotal = pr.HelpfulYesTotal,
                        HelpfulNoTotal = pr.HelpfulNoTotal,
                    },
                    CreatedOnStr = _dateTimeHelper.ConvertToUserTime(pr.CreatedOnUtc, DateTimeKind.Utc).ToString("g"),

                });
            }

            model.AddProductComment.CanCurrentCustomerLeaveComment = _productCommentsSetting.AllowAnonymousUsersToCommentProduct || !_workContext.CurrentCustomer.IsGuest();
        }

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

            PrepareProductCommentsModel(productCommentsModel, product);
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
            if(additionalData == null)
                return Content("");

            var product = _productService.GetProductById((int)additionalData);
            if (product == null || product.Deleted || !product.Published)
                return Content("");

            if (!_productCommentsSetting.EnablePlugin)
                return Content("");

            var model = new ProductCommentsModel();
            PrepareProductCommentsModel(model, product);
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

            PrepareProductCommentsModel(model, productById);
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
