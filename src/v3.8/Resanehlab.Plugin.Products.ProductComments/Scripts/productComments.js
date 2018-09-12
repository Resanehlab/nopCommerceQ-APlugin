!function ($) {
    /**
     * @param {string} src
     * @return {undefined}
     */
    function addProductComment(src) {
        var data = $("#AddProductComment_CommentText").val() || "";
        /** @type {number} */
        $("#updateProductCommentsTargetId")[0].style.opacity = 0.5;
        $.ajax({
            cache: false,
            type: "POST",
            url: src,
            data: "id=" + $(".productId-container").attr("data-productId") + "&AddProductComment.CommentText=" + data.toString()
        }).done(function (input) {
            /** @type {number} */
            $("#updateProductCommentsTargetId")[0].style.opacity = 1;
            $("#updateProductCommentsTargetId").replaceWith(input);
            $("#AddProductComment_CommentText").val("");
            $(".submitCommentResult").fadeIn("slow").delay(3000).fadeOut("slow");
        }).fail(function () {
            /** @type {number} */
            $("#updateProductCommentsTargetId")[0].style.opacity = 1;
            alert("Failed to add comment.");
        });
    }

    $(document).ready(function ($) {
        $(document).on("click", "#add-comment", function () {
            addProductComment($("#updateProductCommentsTargetId").attr("data-productCommentsAddNewUrl"));
        });

        $(document).on("click", ".product-comment-helpfulness .vote", function () {
            var nodes = $(this).closest(".product-comment-helpfulness");
            /** @type {number} */
            var errorClass = parseInt(nodes.attr("data-productCommentId")) || 0;
            var appFrontendUrl = nodes.attr("data-productCommentVoteUrl");
            $.ajax({
                cache: false,
                type: "POST",
                url: appFrontendUrl,
                data: {
                    productCommentId: errorClass,
                    washelpful: $(this).hasClass("vote-yes")
                }
            }).done(function (data) {
                $(".product-comment-helpfulness #helpfulness-vote-yes-" + errorClass).html(data.TotalYes);
                $(".product-comment-helpfulness #helpfulness-vote-no-" + errorClass).html(data.TotalNo);
                $(".product-comment-helpfulness #helpfulness-vote-result-" + errorClass).html(data.Result).fadeIn("slow").delay(3000).fadeOut("slow");
            }).fail(function () {
                alert("Failed to vote. Please refresh the page and try one more time.");
            });
        });

    });
}(jQuery);
