using Nop.Core;
using Nop.Core.Plugins;
using System;
using System.Linq;
using Resanehlab.Plugin.Products.ProductComments.Data;
using Resanehlab.Plugin.Products.ProductComments.Settings;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;
using Nop.Services.Cms;
using System.Collections.Generic;
using Nop.Core.Domain.Cms;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Infrastructure;

namespace Resanehlab.Plugin.Products.ProductComments
{
    public class ProductCommentsPlugin : BasePlugin, IPlugin, IAdminMenuPlugin, IWidgetPlugin
    {
        private readonly ProductCommentsObjectContext _objectContext;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly WidgetSettings _widgetSettings;

        public ProductCommentsPlugin(ProductCommentsObjectContext objectContext,
            ILocalizationService localizationService,
            ISettingService settingService,
            WidgetSettings widgetSettings)
        {
            this._objectContext = objectContext;
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._widgetSettings = widgetSettings;
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            SiteMapNode pluginNode =
                rootNode.ChildNodes.FirstOrDefault<SiteMapNode>(x => x.SystemName == "Third party plugins");
            if (pluginNode == null)
            {
                pluginNode =
                    new SiteMapNode()
                {
                    SystemName = "Third party plugins",
                    Title = this._localizationService.GetResource("Admin.Plugins"),
                    Visible = true
                };
                rootNode.ChildNodes.Add(pluginNode);
            }
            SiteMapNode menu = new SiteMapNode()
            {
                Title = this._localizationService.GetResource("Resanehlab.Plugin.Products.ProductComments"),
                Visible = true,
                IconClass = "fa fa-dot-circle-o"
            };
            menu.ChildNodes.Add(new SiteMapNode()
            {
                Title = this._localizationService.GetResource("Resanehlab.Plugin.Products.ProductComments.Configuration"),
                Url = "/Admin/Plugin/ResanehlabProductComments/Configure",
                Visible = true,
                IconClass = "fa fa fa-genderless",
                SystemName = "Resanehlab.Plugin.Products.ProductComments.Configuration"
            });
            menu.ChildNodes.Add(new SiteMapNode()
            {
                Title = this._localizationService.GetResource("Resanehlab.Plugin.Products.ProductComments.ManageComments"),
                Url = "/Admin/Plugin/ResanehlabProductComments/List",
                Visible = true,
                IconClass = "fa fa fa-genderless",
                SystemName = "Resanehlab.Plugin.Products.ProductComments.ManageComments"
            });
            pluginNode.ChildNodes.Add(menu);
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "ConfigureWidget";
            controllerName = "ProductCommentsAdmin";
            routeValues = new RouteValueDictionary { { "Namespaces", "Resanehlab.Plugin.Products.ProductComments.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { PublicWidgetZones.ProductDetailsBeforeCollateral };
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "ProductComments";
        }

        public override void Install()
        {
            try
            {
                #region Settings

                ProductCommentsSetting productCommentsSetting = new ProductCommentsSetting()
                {
                    EnablePlugin = true,
                    ProductCommentsMustBeApproved = true,
                    AllowAnonymousUsersToCommentProduct = true
                };
                this._settingService.SaveSetting(productCommentsSetting);

                _widgetSettings.ActiveWidgetSystemNames.Add("Resanehlab.ProductComments");
                _settingService.SaveSetting(_widgetSettings);

                #endregion

                #region Localization Resources

                _localizationService.AddOrUpdatePluginLocaleResource("Comments.SeeAfterApproving", "Your question will be shown after admin approval.", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Comments.SuccessfullyAdded", "Your question has been saved successfully.", null);
                _localizationService.AddOrUpdatePluginLocaleResource(
                    "Comments.OnlyRegisteredUsersCanWriteComments",
                    "Only registered customers can insert questions.", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Comments.Helpfulness.OnlyRegistered", "Only registered customers can rate questions.", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Comments.Helpfulness.YourOwnComment", "You can't rate your own questions.", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Comments.Helpfulness.SuccessfullyVoted", "Your rate has been submited.",
                    null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.Enable.Plugin",
                    "Active", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.ProductCommentsMustBeApproved", "Questions must be approved by the admin.", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.AllowAnonymousUsersToCommentProduct", "Guest customers can ask question.", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Comments.Fields.CommentText", "Question", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ProductComments", "Product Q&A Plugin", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Resanehlab.Plugin.Products.ProductComments.Configuration", "Settings", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Comments.Write", "Ask your questions", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Comments.Helpfulness.WasHelpful?", "Is this helpful?", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Comments.SubmitButton", "Asking question", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Comments.ExistingComments", "Asked questions", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Comments.From", "From", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Comments.Date", "Date", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Comments.Fields.CommentText.Required", "Text is required", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Resanehlab.Plugin.Products.ProductComments", "Product Q&A Plugin", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchCommentText", "Question text", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApprovedId", "Approved", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisitedId", "Reviewed", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ProductComments.Comments", "Questions", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApproved.All", "All", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApproved.ApprovedOnly", "Only approved", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApproved.NotApprovedOnly", "Only not approved", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisited.All", "All", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisited.VisitedOnly", "Only reviewed", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisited.NotVisitedOnly", "Only not reviewed", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.CommentText", "Question text", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.ReplyText", "Reply", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.IsApproved", "Approved", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.Visited", "Reviewed", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.CreatedOn", "Creation date", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Comments.Updated", "The question has been updated successfully.", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Comments.Deleted", "The question has been deleted successfully.", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ProductComments.EditCommentDetails", "Edit question", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ProductComments.BackToList", "Back to question list", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ProductComments.Info", "Info", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Resanehlab.Plugin.Products.ProductComments.ManageComments", "Manage questions", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Comment.Reply", "Reply", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Resanehlab.Plugin.Products.ProductComments.Admin.ClickToConfigure", "In order to configure this plugin click here or go to Plugins => Product Q&A plugin => Settings.", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchProductName", "Product name", null);
                _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.ProductName", "Product name", null);

                #endregion

                this._objectContext.Install();


                base.Install();
            }
            catch (Exception exception)
            {
                throw new NopException(exception.Message);
            }
        }

        public override void Uninstall()
        {
            #region Settings

            this._settingService.DeleteSetting<ProductCommentsSetting>();

            _widgetSettings.ActiveWidgetSystemNames.Remove("Resanehlab.ProductComments");
            _settingService.SaveSetting(_widgetSettings);

            #endregion

            #region Localization Resources

            _localizationService.DeletePluginLocaleResource("Comments.SeeAfterApproving");
            _localizationService.DeletePluginLocaleResource("Comments.SuccessfullyAdded");
            _localizationService.DeletePluginLocaleResource("Comments.OnlyRegisteredUsersCanWriteComments");
            _localizationService.DeletePluginLocaleResource("Comments.Helpfulness.OnlyRegistered");
            _localizationService.DeletePluginLocaleResource("Comments.Helpfulness.YourOwnComment");
            _localizationService.DeletePluginLocaleResource("Comments.Helpfulness.SuccessfullyVoted");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.Enable.Plugin");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.ProductCommentsMustBeApproved");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.AllowAnonymousUsersToCommentProduct");
            _localizationService.DeletePluginLocaleResource("Comments.Fields.CommentText");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ProductComments");
            _localizationService.DeletePluginLocaleResource("Resanehlab.Plugin.Products.ProductComments.Configuration");
            _localizationService.DeletePluginLocaleResource("Comments.Write");
            _localizationService.DeletePluginLocaleResource("Comments.Helpfulness.WasHelpful?");
            _localizationService.DeletePluginLocaleResource("Comments.SubmitButton");
            _localizationService.DeletePluginLocaleResource("Comments.ExistingComments");
            _localizationService.DeletePluginLocaleResource("Comments.From");
            _localizationService.DeletePluginLocaleResource("Comments.Date");
            _localizationService.DeletePluginLocaleResource("Comments.Fields.CommentText.Required");
            _localizationService.DeletePluginLocaleResource("Resanehlab.Plugin.Products.ProductComments");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchCommentText");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApprovedId");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisitedId");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ProductComments.Comments");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApproved.All");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApproved.ApprovedOnly");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApproved.NotApprovedOnly");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisited.All");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisited.VisitedOnly");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisited.NotVisitedOnly");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.CommentText");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.ReplyText");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.IsApproved");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.Visited");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.CreatedOn");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Comments.Updated");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Comments.Deleted");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ProductComments.EditCommentDetails");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ProductComments.BackToList");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ProductComments.Info");
            _localizationService.DeletePluginLocaleResource("Resanehlab.Plugin.Products.ProductComments.ManageComments");
            _localizationService.DeletePluginLocaleResource("Comment.Reply");
            _localizationService.DeletePluginLocaleResource("Resanehlab.Plugin.Products.ProductComments.Admin.ClickToConfigure");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchProductName");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.ProductName");

            #endregion

            this._objectContext.Uninstall();

            base.Uninstall();
        }

    }
}