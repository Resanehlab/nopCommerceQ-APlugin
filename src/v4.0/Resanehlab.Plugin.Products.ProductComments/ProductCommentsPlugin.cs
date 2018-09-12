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
            return new List<string> { "productdetails_before_collateral" };
        }
        public void GetPublicViewComponent(string widgetZone, out string viewComponentName)
        {
            viewComponentName = "ProductComments";
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

                this.AddOrUpdatePluginLocaleResource("Comments.SeeAfterApproving", "Your question will be shown after admin approval.", null);
                this.AddOrUpdatePluginLocaleResource("Comments.SuccessfullyAdded", "Your question has been saved successfully.", null);
                this.AddOrUpdatePluginLocaleResource(
                    "Comments.OnlyRegisteredUsersCanWriteComments",
                    "Only registered customers can insert questions.", null);
                this.AddOrUpdatePluginLocaleResource("Comments.Helpfulness.OnlyRegistered", "Only registered customers can rate questions.", null);
                this.AddOrUpdatePluginLocaleResource("Comments.Helpfulness.YourOwnComment", "You can't rate your own questions.", null);
                this.AddOrUpdatePluginLocaleResource("Comments.Helpfulness.SuccessfullyVoted", "Your rate has been submited.",
                    null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.Enable.Plugin",
                    "Active", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.ProductCommentsMustBeApproved", "Questions must be approved by the admin.", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.AllowAnonymousUsersToCommentProduct", "Guest customers can ask question.", null);
                this.AddOrUpdatePluginLocaleResource("Comments.Fields.CommentText", "Question", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ProductComments", "Product Q&A Plugin", null);
                this.AddOrUpdatePluginLocaleResource("Resanehlab.Plugin.Products.ProductComments.Configuration", "Settings", null);
                this.AddOrUpdatePluginLocaleResource("Comments.Write", "Ask your questions", null);
                this.AddOrUpdatePluginLocaleResource("Comments.Helpfulness.WasHelpful?", "Is this helpful?", null);
                this.AddOrUpdatePluginLocaleResource("Comments.SubmitButton", "Asking question", null);
                this.AddOrUpdatePluginLocaleResource("Comments.ExistingComments", "Asked questions", null);
                this.AddOrUpdatePluginLocaleResource("Comments.From", "From", null);
                this.AddOrUpdatePluginLocaleResource("Comments.Date", "Date", null);
                this.AddOrUpdatePluginLocaleResource("Comments.Fields.CommentText.Required", "Text is required", null);
                this.AddOrUpdatePluginLocaleResource("Resanehlab.Plugin.Products.ProductComments", "Product Q&A Plugin", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchCommentText", "Question text", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApprovedId", "Approved", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisitedId", "Reviewed", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ProductComments.Comments", "Questions", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApproved.All", "All", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApproved.ApprovedOnly", "Only approved", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApproved.NotApprovedOnly", "Only not approved", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisited.All", "All", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisited.VisitedOnly", "Only reviewed", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisited.NotVisitedOnly", "Only not reviewed", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.CommentText", "Question text", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.ReplyText", "Reply", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.IsApproved", "Approved", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.Visited", "Reviewed", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.CreatedOn", "Creation date", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Comments.Updated", "The question has been updated successfully.", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Comments.Deleted", "The question has been deleted successfully.", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ProductComments.EditCommentDetails", "Edit question", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ProductComments.BackToList", "Back to question list", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ProductComments.Info", "Info", null);
                this.AddOrUpdatePluginLocaleResource("Resanehlab.Plugin.Products.ProductComments.ManageComments", "Manage questions", null);
                this.AddOrUpdatePluginLocaleResource("Comment.Reply", "Reply", null);
                this.AddOrUpdatePluginLocaleResource("Resanehlab.Plugin.Products.ProductComments.Admin.ClickToConfigure", "In order to configure this plugin click here or go to Plugins => Product Q&A plugin => Settings.", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchProductName", "Product name", null);
                this.AddOrUpdatePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.ProductName", "Product name", null);

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

            this.DeletePluginLocaleResource("Comments.SeeAfterApproving");
            this.DeletePluginLocaleResource("Comments.SuccessfullyAdded");
            this.DeletePluginLocaleResource("Comments.OnlyRegisteredUsersCanWriteComments");
            this.DeletePluginLocaleResource("Comments.Helpfulness.OnlyRegistered");
            this.DeletePluginLocaleResource("Comments.Helpfulness.YourOwnComment");
            this.DeletePluginLocaleResource("Comments.Helpfulness.SuccessfullyVoted");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.Enable.Plugin");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.ProductCommentsMustBeApproved");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.AllowAnonymousUsersToCommentProduct");
            this.DeletePluginLocaleResource("Comments.Fields.CommentText");
            this.DeletePluginLocaleResource("Admin.Plugin.ProductComments");
            this.DeletePluginLocaleResource("Resanehlab.Plugin.Products.ProductComments.Configuration");
            this.DeletePluginLocaleResource("Comments.Write");
            this.DeletePluginLocaleResource("Comments.Helpfulness.WasHelpful?");
            this.DeletePluginLocaleResource("Comments.SubmitButton");
            this.DeletePluginLocaleResource("Comments.ExistingComments");
            this.DeletePluginLocaleResource("Comments.From");
            this.DeletePluginLocaleResource("Comments.Date");
            this.DeletePluginLocaleResource("Comments.Fields.CommentText.Required");
            this.DeletePluginLocaleResource("Resanehlab.Plugin.Products.ProductComments");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchCommentText");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApprovedId");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisitedId");
            this.DeletePluginLocaleResource("Admin.Plugin.ProductComments.Comments");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApproved.All");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApproved.ApprovedOnly");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchIsApproved.NotApprovedOnly");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisited.All");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisited.VisitedOnly");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchVisited.NotVisitedOnly");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.CommentText");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.ReplyText");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.IsApproved");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.Visited");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.CreatedOn");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Comments.Updated");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Comments.Deleted");
            this.DeletePluginLocaleResource("Admin.Plugin.ProductComments.EditCommentDetails");
            this.DeletePluginLocaleResource("Admin.Plugin.ProductComments.BackToList");
            this.DeletePluginLocaleResource("Admin.Plugin.ProductComments.Info");
            this.DeletePluginLocaleResource("Resanehlab.Plugin.Products.ProductComments.ManageComments");
            this.DeletePluginLocaleResource("Comment.Reply");
            this.DeletePluginLocaleResource("Resanehlab.Plugin.Products.ProductComments.Admin.ClickToConfigure");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.List.SearchProductName");
            this.DeletePluginLocaleResource("Admin.Plugin.ResanehlabProductComments.Fields.ProductName");

            #endregion

            this._objectContext.Uninstall();

            base.Uninstall();
        }
    }
}