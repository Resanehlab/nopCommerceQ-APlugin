using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;
using System;
using System.Collections.Generic;

namespace Resanehlab.Plugin.Products.ProductComments.Domain
{
    public partial class ProductComment : BaseEntity
    {
        private ICollection<ProductCommentHelpfulness> _productCommentHelpfulnessEntries;

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int? CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the comment text
        /// </summary>
        public string CommentText { get; set; }        

        /// <summary>
        /// Gets the replt text
        /// </summary>
        /// 
        public string ReplyText { get; set; }

        /// <summary>
        /// comment helpful votes total
        /// </summary>
        public int HelpfulYesTotal { get; set; }

        /// <summary>
        /// comment not helpful votes total
        /// </summary>
        public int HelpfulNoTotal { get; set; }

        /// <summary>
        /// Set comment visiting status by admin
        /// </summary>
        public bool Visited { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content is approved
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the instance is deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets the entries of product comment helpfulness
        /// </summary>
        public virtual ICollection<ProductCommentHelpfulness> ProductCommentHelpfulnessEntries
        {
            get => _productCommentHelpfulnessEntries ?? (_productCommentHelpfulnessEntries = new List<ProductCommentHelpfulness>());
            set => _productCommentHelpfulnessEntries = value;
        }
    }
}
