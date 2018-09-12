
using Nop.Core;

namespace Resanehlab.Plugin.Products.ProductComments.Domain
{
    /// <summary>
    /// Represents a product comment helpfulness
    /// </summary>
    public partial class ProductCommentHelpfulness : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product comment identifier
        /// </summary>
        public int ProductCommentId { get; set; }

        /// <summary>
        /// A value indicating whether a comment a helpful
        /// </summary>
        public bool WasHelpful { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets the product
        /// </summary>
        public virtual ProductComment ProductComment { get; set; }
    }
}
