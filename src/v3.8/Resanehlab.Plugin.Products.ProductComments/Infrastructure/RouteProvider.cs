using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Resanehlab.Plugin.Products.ProductComments.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            string[] namespaces = new string[] { "Resanehlab.Plugin.Products.ProductComments.Controllers" };

            var route1 = routes.MapRoute("Resanehlab.Plugin.Products.ProductComments.Admin.List",
                "Admin/Plugin/ResanehlabProductComments/List", new
                {
                    controller = "ProductCommentsAdmin",
                    action = "List"
                }, namespaces);
            route1.DataTokens.Add("area", "admin");
            routes.Remove(route1);
            routes.Insert(0, route1);

            var route2 = routes.MapRoute("Resanehlab.Plugin.Products.ProductComments.Admin.ProductCommentList",
                "Admin/Plugin/ResanehlabProductComments/ProductCommentList", new
                {
                    controller = "ProductCommentsAdmin",
                    action = "ProductCommentList"
                }, namespaces);
            route2.DataTokens.Add("area", "admin");
            routes.Remove(route2);
            routes.Insert(1, route2);

            var route3 = routes.MapRoute("Resanehlab.Plugin.Products.ProductComments.Admin.Edit",
                "Admin/Plugin/ResanehlabProductComments/Edit/{id}", new
                {
                    controller = "ProductCommentsAdmin",
                    action = "Edit"
                }, namespaces);
            route3.DataTokens.Add("area", "admin");
            routes.Remove(route3);
            routes.Insert(2, route3);

            var route4 = routes.MapRoute("Resanehlab.Plugin.Products.ProductComments.Admin.Configure",
                "Admin/Plugin/ResanehlabProductComments/Configure", new
                {
                    controller = "ProductCommentsAdmin",
                    action = "Configure"
                }, namespaces);
            route4.DataTokens.Add("area", "admin");
            routes.Remove(route4);
            routes.Insert(3, route4);

            var route5 = routes.MapRoute("Resanehlab.Plugin.Products.ProductComments.Admin.Delete",
                "Admin/Plugin/ResanehlabProductComments/Delete/{id}", new
                {
                    controller = "ProductCommentsAdmin",
                    action = "Delete"
                }, namespaces);
            route5.DataTokens.Add("area", "admin");
            routes.Remove(route5);
            routes.Insert(4, route5);

            routes.MapRoute("Resanehlab.Plugin.Products.ProductComments.ProductComments",
                "Plugin/ResanehlabProductComments/ProductComments", new
                {
                    controller = "ProductComments",
                    action = "ProductComments"
                }, namespaces); 

            routes.MapRoute("Resanehlab.Plugin.Products.ProductComments.ProductCommentsAddNew",
                "Plugin/ResanehlabProductComments/ProductCommentsAddNew", new
                {
                    controller = "ProductCommentsAddNew",
                    action = "ProductComments"
                }, namespaces);

            routes.MapRoute("Resanehlab.Plugin.Products.ProductComments.SetProductCommentHelpfulness",
                "Plugin/ResanehlabProductComments/SetProductCommentHelpfulness", new
                {
                    controller = "SetProductCommentHelpfulness",
                    action = "ProductComments"
                }, namespaces);
        }

        public virtual int Priority
        {
            get { return 10; }
        }
    }
}
