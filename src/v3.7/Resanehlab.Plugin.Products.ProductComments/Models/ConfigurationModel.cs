using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Resanehlab.Plugin.Products.ProductComments.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.Plugin.ResanehlabProductComments.Fields.Enable.Plugin")]
        public bool EnablePlugin { get; set; }
        public bool EnablePlugin_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Plugin.ResanehlabProductComments.Fields.ProductCommentsMustBeApproved")]
        public bool ProductCommentsMustBeApproved { get; set; }

        public bool ProductCommentsMustBeApproved_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Plugin.ResanehlabProductComments.Fields.AllowAnonymousUsersToCommentProduct")]
        public bool AllowAnonymousUsersToCommentProduct { get; set; }

        public bool AllowAnonymousUsersToCommentProduct_OverrideForStore { get; set; }
    }
}

