$(function () {

    $(".tabChange").bind("click", function () {
        $("#currentTabId").val($(this).attr("tabid"));
        loadCreditGoods(1);
    });


});

function doExchange(goodsId) {
    var toExchangeCount = parseInt($("#txt_detail_buyCount").val());
    if (toExchangeCount > quantity || toExchangeCount > maxExchangeTimes) {
        MsgDialog.Alert(false, "兑换数量超出可兑换次数，请修改要兑换的数量");
        return;
    }
    $.get("/Exchanges/_MyAddress?t=" + Math.random(), {}, function (html) {

        BootstrapDialog.show({
            title: '选择收货信息',
            message: $(html),
            closable: false,
            buttons: [{
                id: "btn_submit",
                icon: 'glyphicon glyphicon-send',
                label: '确认兑换',
                cssClass: 'btn-primary',
                autospin: true,
                action: function (dialogRef) {
                    dialogRef.enableButtons(false);
                    dialogRef.setClosable(false);
                    var $button = dialogRef.getButton('btn_submit');

                    var addressCount = parseInt($("#h_address_Count").val());
                    var addressId = parseInt($("input[name='rd_exchange_address']:checked").val());
                    if (isNaN(addressCount) || addressCount < 1) {
                        MsgDialog.Alert(false, "您还没有添加收货地址！前往添加？", function () {
                            window.location.href = "/Me/MyAddresses";
                        });

                    }

                    if (isNaN(addressId) || addressId < 1) {
                        MsgDialog.Alert(false, "请选择一个收货地址");
                        $button.stopSpin();
                        dialogRef.enableButtons(true);
                        dialogRef.setClosable(true);
                        return;
                    }

                    var url = "/Exchanges/ToExchange?t=" + Math.random();

                    $.get(url, { goodsId: goodsId, quantiy: toExchangeCount, addressId: addressId }, function (data) {
                        $button.stopSpin();
                        dialogRef.enableButtons(true);
                        dialogRef.setClosable(true);
                        MsgDialog.Alert(data.status, data.msg, function (data) {
                            if (data.status) {
                                dialogRef.close();
                            }
                        });
                    });
                }
            }, {
                label: '取消',
                action: function (dialogRef) {
                    dialogRef.close();
                }
            }]
        });
    });
}

function loadCreditGoods(page) {
    var tabId = $("#currentTabId").val();
    $(".msg_jf").showLoading();
    $.get("/Exchanges/_Index", { tabId: tabId, page: page }, function (data) {
        $(".list_main").html(data);
        $(".list_main").show();
        $(".msg_jf").hideLoading();
    });
}