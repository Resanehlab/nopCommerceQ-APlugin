using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Resanehlab.Plugin.Products.ProductComments.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routes)
        {
            routes.MapRoute("Resanehlab.Plugin.Products.ProductComments.Admin.List",
                "Admin/Plugin/ResanehlabProductComments/List", new
                {
                    controller = "ProductCommentsAdmin",
                    action = "List",
                    area = "Admin"
                });

            routes.MapRoute("Resanehlab.Plugin.Products.ProductComments.Admin.ProductCommentList",
                "Admin/Plugin/ResanehlabProductComments/ProductCommentList", new
                {
                    controller = "ProductCommentsAdmin",
                    action = "ProductCommentList",
                    area = "Admin"
                });

            routes.MapRoute("Resanehlab.Plugin.Products.ProductComments.Admin.Edit",
                "Admin/Plugin/ResanehlabProductComments/Edit/{id}", new
                {
                    controller = "ProductCommentsAdmin",
                    action = "Edit",
                    area = "Admin"
                });

            routes.MapRoute("Resanehlab.Plugin.Products.ProductComments.Admin.Configure",
                "Admin/Plugin/ResanehlabProductComments/Configure", new
                {
                    controller = "ProductCommentsAdmin",
                    action = "Configure",
                    area = "Admin"
                });

            routes.MapRoute("Resanehlab.Plugin.Products.ProductComments.Admin.Delete",
                "Admin/Plugin/ResanehlabProductComments/Delete/{id}", new
                {
                    controller = "ProductCommentsAdmin",
                    action = "Delete",
                    area = "Admin"
                });

            routes.MapRoute("Resanehlab.Plugin.Products.ProductComments.ProductComments",
                "Plugin/ResanehlabProductComments/ProductComments", new
                {
                    controller = "ProductComments",
                    action = "ProductComments"
                });

            routes.MapRoute("Resanehlab.Plugin.Products.ProductComments.ProductCommentsAddNew",
                "Plugin/ResanehlabProductComments/ProductCommentsAddNew", new
                {
                    controller = "ProductCommentsAddNew",
                    action = "ProductComments"
                });

            routes.MapRoute("Resanehlab.Plugin.Products.ProductComments.SetProductCommentHelpfulness",
                "Plugin/ResanehlabProductComments/SetProductCommentHelpfulness", new
                {
                    controller = "SetProductCommentHelpfulness",
                    action = "ProductComments"
                });
        }

        public virtual int Priority
        {
            get { return 10; }
        }
    }
}
