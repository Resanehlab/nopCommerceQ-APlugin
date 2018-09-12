using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Resanehlab.Plugin.Products.ProductComments.Models
{
    public partial class ProductCommentListModel : BaseNopModel
    {
        public ProductCommentListModel()
        {
            AvailableApprovedOptions = new List<SelectListItem>();
            AvailableVisitedOptions = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Plugin.ResanehlabProductComments.List.SearchCommentText")]
        public string SearchCommentText { get; set; }

        [NopResourceDisplayName("Admin.Plugin.ResanehlabProductComments.List.SearchIsApprovedId")]
        public int SearchIsApprovedId { get; set; }

        [NopResourceDisplayName("Admin.Plugin.ResanehlabProductComments.List.SearchVisitedId")]
        public int SearchVisitedId { get; set; }

        [NopResourceDisplayName("Admin.Plugin.ResanehlabProductComments.List.SearchProductName")]
        public string SearchProductName { get; set; }

        public IList<SelectListItem> AvailableApprovedOptions { get; set; }
        public IList<SelectListItem> AvailableVisitedOptions { get; set; }
    }
}