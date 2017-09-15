var comments = {
    ScrollToCommentBox: function (parentId, followId, beReplyedUserId) {
        $("#h_replyTo").val(parentId);
        $("#h_appendToFollowId").val(followId);
        $("#h_beReplyedUserId").val(beReplyedUserId);
        $('html,body').animate({ scrollTop: $("#div_comment_reply").offset().top }, 500);
        $("#txt_comment_content").focus();
    },
    AddComment: function () {
       
        var content = $("#txt_comment_content").val();
        var replyTo = $("#h_replyTo").val();
        var followAppendTo = $("#h_appendToFollowId").val();
        var beUserId = parseInt($("#h_beReplyedUserId").val());
        var goodsId = $("#h_goodsId").val();
        var pageNo = $("#h_pageNo").val();

        if (isEmpty(content)) {
            MsgDialog.Alert(false, "请输入品论内容");
            return;
        }
        $("#div_comment_reply").showLoading();
        $.post("/home/_AddComment?t=" + Math.random(), { content: content, replyTo: replyTo, goodsId: goodsId, beReplyedUserId: beUserId, page: pageNo }, function (data) {
            if (beUserId == 0) {
                $("#div_detail_comments_container").append(data);
            } else {
                $("#comments_reply_" + followAppendTo).appendTo(data);
            }
            if (data.length > 0) {
                MsgDialog.Alert(true, "评论成功", function () {
                    $("#txt_comment_content").val("");
                    $("#h_replyTo").val("0");
                    $("#h_beReplyedUserId").val("0");
                    $("#div_comment_reply").hideLoading();
                });
            }
        });
    }
}

$(function () {
    //数量增减
    $('[data-trigger="spinner"]').spinner('delay', 100).spinner('changed', function (e, newVal, oldVal) {
        //trigger lazed, depend on delay option.
    });

    $("#btn_buy_immediately").bind("click", function () {
        var goodsId = $("#h_detail_goodsId").val();
        var quantity = parseInt($("#txt_detail_buyCount").val());
        updateCart(goodsId, quantity, function () {
            window.location.href = "/home/checkout?p=" + encodeURIComponent(goodsId + "$" + quantity);
        });
    });
})