using System.Collections.Generic;
using FluentValidation.Attributes;
using Resanehlab.Plugin.Products.ProductComments.Validators;
using System;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Resanehlab.Plugin.Products.ProductComments.Models
{
    public partial class ProductCommentOverviewModel : BaseNopModel
    {
        public int ProductId { get; set; }
    }

    [Validator(typeof(ProductCommentsValidator))]
    public partial class ProductCommentsModel : BaseNopModel
    {
        public ProductCommentsModel()
        {
            Items = new List<ProductCommentModel>();
            AddProductComment = new AddProductCommentModel();
        }
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        public IList<ProductCommentModel> Items { get; set; }
        public AddProductCommentModel AddProductComment { get; set; }
    }

    public partial class ProductCommentModel : BaseNopEntityModel
    {
        public int? CustomerId { get; set; }

        public string CustomerName { get; set; }

        public int ProductId { get; set; }

        [NopResourceDisplayName("Admin.Plugin.ResanehlabProductComments.Fields.ProductName")]
        public string ProductName { get; set; }

        public bool AllowViewingProfiles { get; set; }

        [NopResourceDisplayName("Admin.Plugin.ResanehlabProductComments.Fields.CommentText")]
        public string CommentText { get; set; }

        [NopResourceDisplayName("Admin.Plugin.ResanehlabProductComments.Fields.ReplyText")]
        public string ReplyText { get; set; }
        
        public bool Visited { get; set; }

        [NopResourceDisplayName("Admin.Plugin.ResanehlabProductComments.Fields.IsApproved")]
        public bool IsApproved { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public ProductCommentHelpfulnessModel Helpfulness { get; set; }

        public string CreatedOnStr { get; set; }

        public string ProductSeName { get; set; }
    }


    public partial class ProductCommentHelpfulnessModel : BaseNopModel
    {
        public int ProductCommentId { get; set; }

        public int HelpfulYesTotal { get; set; }

        public int HelpfulNoTotal { get; set; }
    }

    public partial class AddProductCommentModel : BaseNopModel
    {
        [NopResourceDisplayName("Comments.Fields.CommentText")]
        public string CommentText { get; set; }

        public bool CanCurrentCustomerLeaveComment { get; set; }
        public bool SuccessfullyAdded { get; set; }
        public string Result { get; set; }
    }
}