using Resanehlab.Plugin.Products.ProductComments.Domain;
using Nop.Data.Mapping;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Resanehlab.Plugin.Products.ProductComments.Data
{
    public partial class ProductCommentHelpfulnessMap : NopEntityTypeConfiguration<ProductCommentHelpfulness>
    {
        public override void Configure(EntityTypeBuilder<ProductCommentHelpfulness> builder)
        {
            builder.ToTable("RL_ProductCommentHelpfulness");

            builder.HasKey(pr => pr.Id);

            builder.HasOne(prh => prh.ProductComment)
                .WithMany(pr => pr.ProductCommentHelpfulnessEntries)
                .HasForeignKey(prh => prh.ProductCommentId).OnDelete(DeleteBehavior.Cascade);
        }

    }
}