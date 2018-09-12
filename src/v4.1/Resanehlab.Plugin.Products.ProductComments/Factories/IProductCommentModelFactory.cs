using Nop.Core.Domain.Catalog;
using Resanehlab.Plugin.Products.ProductComments.Models;

namespace Resanehlab.Plugin.Products.ProductComments.Factories
{
    public partial interface IProductCommentModelFactory
    {
        void PrepareProductCommentsModel(ProductCommentsModel model, Product product);
    }
}
