
using Resanehlab.Plugin.Products.ProductComments.Domain;
using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Resanehlab.Plugin.Products.ProductComments.Services
{
    public partial interface IProductCommentService
    {
        /// <summary>
        /// Gets all product Comments
        /// </summary>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item item creation to; null to load all records</param>
        /// <param name="message">Search title or review text; null to load all records</param>
        /// <returns>Comments</returns>
        IPagedList<ProductComment> GetAllProductComments(int customerId = 0, int productId = 0, int storeId = 0,
            string commentText = null, bool? isApproved = null, bool? visited = null, string productName = null, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets product comment
        /// </summary>
        /// <param name="productCommentId">Product comment identifier</param>
        /// <returns>Product comment</returns>
        ProductComment GetProductCommentById(int productCommentId);

        /// <summary>
        /// Deletes a product comment
        /// </summary>
        /// <param name="productComment">Product comment</param>
        void DeleteProductComment(ProductComment productComment);

        /// Inserts a product comment
        /// </summary>
        /// <param name="productComment">Product comment</param>
        void InsertProductComment(ProductComment productComment);

        void UpdateProductComment(ProductComment productComment);
    }
}