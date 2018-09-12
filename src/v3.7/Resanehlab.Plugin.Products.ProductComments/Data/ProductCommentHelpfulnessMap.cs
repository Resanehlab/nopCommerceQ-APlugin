using Resanehlab.Plugin.Products.ProductComments.Domain;
using Nop.Data.Mapping;

namespace Resanehlab.Plugin.Products.ProductComments.Data
{
    public partial class ProductCommentHelpfulnessMap : NopEntityTypeConfiguration<ProductCommentHelpfulness>
    {
        public ProductCommentHelpfulnessMap()
        {
            this.ToTable("RL_ProductCommentHelpfulness");
            this.HasKey(pr => pr.Id);

            this.HasRequired(prh => prh.ProductComment)
                .WithMany(pr => pr.ProductCommentHelpfulnessEntries)
                .HasForeignKey(prh => prh.ProductCommentId).WillCascadeOnDelete(true);
        }
    }
}