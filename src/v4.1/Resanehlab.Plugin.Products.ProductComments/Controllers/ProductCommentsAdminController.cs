using Resanehlab.Plugin.Products.ProductComments.Settings;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tasks;
using Nop.Web.Framework.Controllers;
using Resanehlab.Plugin.Products.ProductComments.Models;
using Nop.Web.Framework.Kendoui;
using Resanehlab.Plugin.Products.ProductComments.Services;
using System.Linq;
using System;
using Nop.Services.Helpers;
using Nop.Services.Catalog;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Services.Seo;
using Nop.Web.Framework;
using Nop.Core.Infrastructure;

namespace Resanehlab.Plugin.Products.ProductComments.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class ProductCommentsAdminController : BasePluginController
    {
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ICustomerService _customerService;
        private readonly IPictureService _pictureService;
        private readonly IProductCommentService _productCommentService;
        private readonly IStoreContext _storeContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;

        public ProductCommentsAdminController(IPermissionService permissionService,
            IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            ILocalizationService localizationService,
            IScheduleTaskService scheduleTaskService,
            ICustomerService customerService,
            IPictureService pictureService,
            IProductCommentService productCommentService,
            IStoreContext storeContext,
            IDateTimeHelper dateTimeHelper,
            IProductService productService,
            IUrlRecordService urlRecordService)
        {
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._scheduleTaskService = scheduleTaskService;
            this._customerService = customerService;
            this._pictureService = pictureService;
            this._productCommentService = productCommentService;
            this._storeContext = storeContext;
            this._dateTimeHelper = dateTimeHelper;
            this._productService = productService;
            this._urlRecordService = urlRecordService;
        }

        public ActionResult ConfigureWidget()
        {
            return PartialView("~/Plugins/Resanehlab.ProductComments/Views/Admin/ConfigureWidget.cshtml");
        }

        public ActionResult Configure()
        {
            if (!this._permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
            {
                return RedirectToAction("AccessDenied", "Security", new { pageUrl = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request) });
            }

            int activeStoreScopeConfiguration = _storeContext.ActiveStoreScopeConfiguration;
            ProductCommentsSetting settings =
                this._settingService.LoadSetting<ProductCommentsSetting>(activeStoreScopeConfiguration);
            ConfigurationModel model = new ConfigurationModel
            {
                EnablePlugin = settings.EnablePlugin,
                ProductCommentsMustBeApproved = settings.ProductCommentsMustBeApproved,
                AllowAnonymousUsersToCommentProduct = settings.AllowAnonymousUsersToCommentProduct
            };
            if (activeStoreScopeConfiguration > 0)
            {
                model.EnablePlugin_OverrideForStore = this._settingService.SettingExists(settings, t => t.EnablePlugin,
                    activeStoreScopeConfiguration);
                model.ProductCommentsMustBeApproved_OverrideForStore = this._settingService.SettingExists(settings,
                    t => t.ProductCommentsMustBeApproved, activeStoreScopeConfiguration);
                model.AllowAnonymousUsersToCommentProduct_OverrideForStore = this._settingService.SettingExists(settings,
                    t => t.AllowAnonymousUsersToCommentProduct, activeStoreScopeConfiguration);
            }
            return base.View("~/Plugins/Resanehlab.ProductComments/Views/Admin/Configure.cshtml", model);
        }

        [HttpPost, AuthorizeAdmin]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!this._permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
            {
                return RedirectToAction("AccessDenied", "Security", new { pageUrl = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request) });
            }
            int activeStoreScopeConfiguration = _storeContext.ActiveStoreScopeConfiguration;

            ProductCommentsSetting settings =
                this._settingService.LoadSetting<ProductCommentsSetting>(activeStoreScopeConfiguration);
            settings.EnablePlugin = model.EnablePlugin;
            settings.ProductCommentsMustBeApproved = model.ProductCommentsMustBeApproved;
            settings.AllowAnonymousUsersToCommentProduct = model.AllowAnonymousUsersToCommentProduct;

            this._settingService.SaveSettingOverridablePerStore(settings, t => t.EnablePlugin,
                model.EnablePlugin_OverrideForStore, activeStoreScopeConfiguration, false);
            this._settingService.SaveSettingOverridablePerStore(settings, t => t.ProductCommentsMustBeApproved,
                model.ProductCommentsMustBeApproved_OverrideForStore, activeStoreScopeConfiguration, false);
            this._settingService.SaveSettingOverridablePerStore(settings, t => t.AllowAnonymousUsersToCommentProduct,
                model.AllowAnonymousUsersToCommentProduct_OverrideForStore, activeStoreScopeConfiguration, false);

            this._settingService.ClearCache();
            this.SuccessNotification(this._localizationService.GetResource("Admin.Configuration.Updated"), true);
            return base.RedirectToAction("Configure");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return RedirectToAction("AccessDenied", "Security", new { pageUrl = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request) });

            var model = new ProductCommentListModel();

            model.AvailableApprovedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApproved.All"), Value = "0" });
            model.AvailableApprovedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApproved.ApprovedOnly"), Value = "1" });
            model.AvailableApprovedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApproved.NotApprovedOnly"), Value = "2" });

            model.AvailableVisitedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisited.All"), Value = "0" });
            model.AvailableVisitedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisited.VisitedOnly"), Value = "1" });
            model.AvailableVisitedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisited.NotVisitedOnly"), Value = "2" });

            return View("~/Plugins/Resanehlab.ProductComments/Views/Admin/List.cshtml", model);
        }

        [HttpPost]
        public ActionResult ProductCommentList(DataSourceRequest command, ProductCommentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return RedirectToAction("AccessDenied", "Security", new { pageUrl = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request) });

            bool? overrideIsApproved = null;
            if (model.SearchIsApprovedId == 1)
                overrideIsApproved = true;
            else if (model.SearchIsApprovedId == 2)
                overrideIsApproved = false;

            bool? overrideVisited = null;
            if (model.SearchVisitedId == 1)
                overrideVisited = true;
            else if (model.SearchVisitedId == 2)
                overrideVisited = false;

            var comments = _productCommentService.GetAllProductComments(isApproved: overrideIsApproved,
                visited: overrideVisited,
                storeId: _storeContext.CurrentStore.Id,
                commentText: model.SearchCommentText,
                productName: model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);

            var gridModel = new DataSourceResult();
            gridModel.Data = comments.Select(x =>
            {
                var p = _productService.GetProductById(x.ProductId);
                var productCommentModel = new ProductCommentModel()
                {
                    Id = x.Id,
                    CommentText = x.CommentText,
                    CreatedOnStr = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc).ToString("g"),
                    IsApproved = x.IsApproved,
                    ReplyText = x.ReplyText,
                    Visited = x.Visited,
                    ProductName = _localizationService.GetLocalized(p, t => t.Name),
                    ProductSeName = _urlRecordService.GetSeName(p)
                };
                return productCommentModel;
            });
            gridModel.Total = comments.TotalCount;

            return Json(gridModel);
        }

        //edit comment
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return RedirectToAction("AccessDenied", "Security", new { pageUrl = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request) });

            var comment = _productCommentService.GetProductCommentById(id);
            if (comment == null || comment.Deleted)
                //No comment found with the specified id
                return RedirectToAction("List");

            var product = _productService.GetProductById(comment.ProductId);

            var model = new ProductCommentModel()
            {
                Id = comment.Id,
                CommentText = comment.CommentText,
                IsApproved = comment.IsApproved,
                ReplyText = comment.ReplyText,
                ProductName = _localizationService.GetLocalized(product, t => t.Name),
                ProductSeName = _urlRecordService.GetSeName(product)
            };

            return View("~/Plugins/Resanehlab.ProductComments/Views/Admin/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(ProductCommentModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return RedirectToAction("AccessDenied", "Security", new { pageUrl = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request) });

            var comment = _productCommentService.GetProductCommentById(model.Id);

            if (comment == null || comment.Deleted)
                //No comment found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                comment.ReplyText = model.ReplyText;
                comment.IsApproved = model.IsApproved;
                comment.Visited = true;

                _productCommentService.UpdateProductComment(comment);

                SuccessNotification(_localizationService.GetResource("Admin.Plugin.ResanehlabProductComments.Comments.Updated"));

                if (continueEditing)
                {
                    return RedirectToAction("Edit", new { id = comment.Id });
                }
                return RedirectToAction("List");
            }

            return View("~/Plugins/Resanehlab.ProductComments/Views/Admin/Edit.cshtml", model);
        }

        //delete comment
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return RedirectToAction("AccessDenied", "Security", new { pageUrl = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request) });

            var comment = _productCommentService.GetProductCommentById(id);
            if (comment == null)
                //No comment found with the specified id
                return RedirectToAction("List");

            _productCommentService.DeleteProductComment(comment);

            SuccessNotification(_localizationService.GetResource("Admin.Plugin.ResanehlabProductComments.Comments.Deleted"));

            return RedirectToAction("List");
        }
    }
}
