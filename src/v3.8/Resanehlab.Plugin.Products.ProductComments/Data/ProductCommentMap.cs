
using Resanehlab.Plugin.Products.ProductComments.Domain;
using Nop.Data.Mapping;

namespace Resanehlab.Plugin.Products.ProductComments.Data
{
    public partial class ProductCommentMap : NopEntityTypeConfiguration<ProductComment>
    {
        public ProductCommentMap()
        {
            this.ToTable("RL_ProductComment");
            this.HasKey(pr => pr.Id);
        }
    }
}
