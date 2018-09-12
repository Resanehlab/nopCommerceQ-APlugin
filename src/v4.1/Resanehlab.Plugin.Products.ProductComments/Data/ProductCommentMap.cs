using Resanehlab.Plugin.Products.ProductComments.Domain;
using Nop.Data.Mapping;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Resanehlab.Plugin.Products.ProductComments.Data
{
    public partial class ProductCommentMap : NopEntityTypeConfiguration<ProductComment>
    {
        public override void Configure(EntityTypeBuilder<ProductComment> builder)
        {
            builder.ToTable("RL_ProductComment");

            builder.HasKey(pr => pr.Id);
        }
    }
}
