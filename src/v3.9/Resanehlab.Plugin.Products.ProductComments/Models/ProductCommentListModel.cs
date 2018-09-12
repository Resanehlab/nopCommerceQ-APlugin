using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

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
        [AllowHtml]
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