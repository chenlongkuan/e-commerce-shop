var MobileDetails = {
    //手机端加入购物车
    UpdateCart: function (goodsId, quantity, type, callback, obj) {
        if (goodsId <= 0) {
            alert("商品Id不正确");
            return;
        }
        $.get("/home/UpdateCart?t=" + Math.random(), { goodsId: goodsId, quantity: quantity, type: type }, function (data) {
            if (!data.status) {
                alert(data.msg);
                return;
            } else {
                $("#addCartOk").show();
                setTimeout(" $('#addCartOk').hide();window.location.href = '/mobile/home/cart'", 3000);

                //window.location.href = "/mobile/home/cart";
            }
        });
    },
    //手机端立即购买
    BuyImmediately: function () {
        var goodsId = $("#h_detail_goodsId").val();
        var quantity = 1;
        if (goodsId <= 0) {
            alert("商品Id不正确");
            return;
        }
        $.get("/home/UpdateCart?t=" + Math.random(), { goodsId: goodsId, quantity: quantity, type: "add" }, function (data) {
            if (!data.status) {
                alert(data.msg);
                return;
            } else {
                window.location.href = "/mobile/home/checkout?p=" + encodeURIComponent(goodsId + "$" + quantity);
            }
        });
    }
};