using Resanehlab.Plugin.Products.ProductComments.Domain;
using Nop.Core.Data;
using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Catalog;
using Resanehlab.Plugin.Products.ProductComments.Data;

namespace Resanehlab.Plugin.Products.ProductComments.Services
{
    public partial class ProductCommentService : IProductCommentService
    {

        #region Fields

        private readonly IRepository<ProductComment> _productCommentRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IWorkContext _workContext;
        private readonly ProductCommentsObjectContext _productCommentsObjectContext;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="productCommentRepository">Product comment repository</param>
        /// <param name="localizedPropertyRepository">Localized property repository</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="workContext">Work context</param>
        public ProductCommentService(IRepository<ProductComment> productCommentRepository,
            IRepository<LocalizedProperty> localizedPropertyRepository,
            IRepository<Product> productRepository,
            IWorkContext workContext,
            ProductCommentsObjectContext productCommentsObjectContext)
        {
            this._productCommentRepository = productCommentRepository;
            this._localizedPropertyRepository = localizedPropertyRepository;
            this._productRepository = productRepository;
            this._workContext = workContext;
            this._productCommentsObjectContext = productCommentsObjectContext;
        }

        #endregion

        /// <summary>
        /// Gets all product Comments
        /// </summary>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item item creation to; null to load all records</param>
        /// <param name="message">Search title or review text; null to load all records</param>
        /// <returns>Comments</returns>
        public virtual IPagedList<ProductComment> GetAllProductComments(int customerId = 0, int productId = 0, int storeId = 0,
            string commentText = null, bool? isApproved = null, bool? visited = null, string productName = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = "select distinct PC.[Id], PC.[CustomerId],PC.[ProductId],PC.[StoreId],PC.[CommentText],PC.[ReplyText]," +
                "PC.[HelpfulYesTotal],PC.[HelpfulNoTotal],PC.[Visited],PC.[IsApproved],PC.[Deleted],PC.[CreatedOnUtc] from [RL_ProductComment] PC ";

            if (!string.IsNullOrWhiteSpace(productName))
            {
                query += string.Format("inner join Product on PC.ProductId=Product.Id left outer join LocalizedProperty on Product.Id = LocalizedProperty.EntityId");
            }

            if (customerId > 0)
                query += string.Format(" where customerId={0}", customerId);
            else
                query += string.Format(" where customerId>0");

            if (!string.IsNullOrWhiteSpace(productName))
            {
                var languageId = _workContext.WorkingLanguage.Id;
                query += string.Format(@" and (Product.Name like N'%{0}%' or LocalizedProperty.LanguageId={1} and 
LocalizedProperty.LocaleKeyGroup='Product' and LocalizedProperty.LocaleKey='Name' and LocalizedProperty.LocaleValue like N'%{0}%')",
productName, languageId);
            }
            if (productId > 0)
                query += string.Format(" and productId={0}", productId);
            if (storeId > 0)
                query += string.Format(" and storeId={0}", storeId);
            if (!String.IsNullOrEmpty(commentText))
                query += string.Format(" and commentText like N'%{0}%'", commentText);
            if (isApproved.HasValue)
                query += string.Format(" and isApproved={0}", isApproved.Value ? 1 : 0);
            if (visited.HasValue)
                query += string.Format(" and visited={0}", visited.Value ? 1 : 0);

            query += " and PC.[Deleted]=0";

            var data = _productCommentsObjectContext.EntityFromSql<ProductComment>(query).AsQueryable().OrderBy(t=> t.CreatedOnUtc).AsQueryable();

            var result = new PagedList<ProductComment>(data, pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// Gets product comment
        /// </summary>
        /// <param name="productCommentId">Product comment identifier</param>
        /// <returns>Product comment</returns>
        public virtual ProductComment GetProductCommentById(int productCommentId)
        {
            if (productCommentId == 0)
                return null;

            return _productCommentRepository.GetById(productCommentId);
        }

        /// <summary>
        /// Deletes a product comment
        /// </summary>
        /// <param name="productComment">Product comment</param>
        public virtual void DeleteProductComment(ProductComment productComment)
        {
            if (productComment == null)
                throw new ArgumentNullException("productComment");

            productComment.Deleted = true;

            _productCommentRepository.Update(productComment);
        }

        /// <summary>
        /// Inserts a product comment
        /// </summary>
        /// <param name="productComment">ProductComment</param>
        public virtual void InsertProductComment(ProductComment productComment)
        {
            if (productComment == null)
                throw new ArgumentNullException("productComment");

            _productCommentRepository.Insert(productComment);

        }

        public virtual void UpdateProductComment(ProductComment productComment)
        {
            if (productComment == null)
                throw new ArgumentNullException("productComment");

            _productCommentRepository.Update(productComment);
        }
    }
}