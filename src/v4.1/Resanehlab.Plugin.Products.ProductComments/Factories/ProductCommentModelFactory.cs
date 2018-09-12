using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Resanehlab.Plugin.Products.ProductComments.Models;
using Resanehlab.Plugin.Products.ProductComments.Services;
using Resanehlab.Plugin.Products.ProductComments.Settings;
using System;

namespace Resanehlab.Plugin.Products.ProductComments.Factories
{
    public partial class ProductCommentModelFactory : IProductCommentModelFactory
    {
        #region Fields

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

        #endregion

        #region Ctor

        public ProductCommentModelFactory(IProductService productService,
            IProductCommentService productCommentService,
            IWorkContext workContext,
            ProductCommentsSetting productCommentsSetting,
            ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper,
            IStoreContext storeContext,
            CustomerSettings customerSettings,
            ICustomerService customerService,
            IUrlRecordService urlRecordService)
        {
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
        }

        #endregion

        #region Methods

        public virtual void PrepareProductCommentsModel(ProductCommentsModel model, Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (model == null)
                throw new ArgumentNullException("model");

            model.ProductId = product.Id;
            model.ProductName = _localizationService.GetLocalized(product, t => t.Name);
            model.ProductSeName = _urlRecordService.GetSeName(product);

            var productComments = _productCommentService.GetAllProductComments(isApproved: true, productId: product.Id, storeId: _storeContext.CurrentStore.Id);
            foreach (var pr in productComments)
            {
                var customer = pr.CustomerId.HasValue ? _customerService.GetCustomerById(pr.CustomerId.Value) : null;
                model.Items.Add(new ProductCommentModel
                {
                    Id = pr.Id,
                    CustomerId = pr.CustomerId,
                    CustomerName = customer != null ? _customerService.FormatUserName(customer) : _localizationService.GetResource("Customer.Guest"),
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

        #endregion
    }
}
