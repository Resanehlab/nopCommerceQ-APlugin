using Nop.Core.Configuration;

namespace Resanehlab.Plugin.Products.ProductComments.Settings
{
    public class ProductCommentsSetting : ISettings
    {
        public bool EnablePlugin { get; set; }
        public bool ProductCommentsMustBeApproved { get; set; }
        public bool AllowAnonymousUsersToCommentProduct { get; set; }
    }
}

