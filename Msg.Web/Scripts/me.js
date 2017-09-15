
//首页
var index = {
    //加载未付款订单
    loadUnPayOrders: function (page) {
        $("#div_me_unpay_container").showLoading();
        $.get("/me/_UnPayOrders?t=" + Math.random(), { page: page }, function (html) {
            $("#div_me_unpay_container").html(html);
            $("#div_me_unpay_container").hideLoading();
        });
    },

    //加载未收货订单
    loadUnReceivedOrders: function (page) {
        $("#div_me_unpay_container").showLoading();
        $.get("/me/_UnReceivedOrders?t=" + Math.random(), { page: page }, function (html) {
            $("#div_me_unpay_container").html(html);
            $("#div_me_unpay_container").hideLoading();
        });
    }
}
//我的订单
var myOrders = {

    filterClick: function (status, obj) {
        $("#h_status").val(status);
        $(obj).siblings().removeClass("over");
        $(obj).addClass("over");
        myOrders.loadOrders(1);
    },

    loadOrders: function (page) {
        $(".order_d").showLoading();
        var status = parseInt($("#h_status").val());
        if (isNaN(status)) {
            $(".order_d").hideLoading();
            return;
        }
        $.get("/me/_MyOrders?t=" + Math.random(), { page: page, orderStatus: status }, function (html) {
            $(".order_d").html(html);
            $(".order_d").hideLoading();
        });
    },
    //取消订单
    cancelOrder: function (id) {
        MsgDialog.Confirm("确定要取消此订单吗?", function () {
            $.get("/Me/CancelOrder?t=" + Math.random(), { id: id }, function (data) {
                MsgDialog.Alert(data.status, data.msg, function() {
                    window.location.href = window.location.href;
                });
            });
        });
    },
    //确认收货
    confirmReceipt: function (id) {
        MsgDialog.Confirm("确定要取消此订单吗?", function () {
            $.get("/Me/ConfirmReceipt?t=" + Math.random(), { id: id }, function (data) {
                MsgDialog.Alert(data.status, data.msg, function () {
                    window.location.href = window.location.href;
                });
            });
        });
    }
}
//我的创业集市
var mymarket = {
    //申请成为创业者
    tobeSupplier: function () {
        var linkMan = $("#txt_supplier_linkman").val();
        var linkTel = $("#txt_supplier_linktel").val();
        var reason = $("#txt_supplier_reason").val();
        if (linkMan.length < 2 || linkMan.length > 10) {
            MsgDialog.Alert(false, "申请人姓名长度须在2-10个字符内");
            return;
        }
        if (!ISMobile(linkTel) || !IsTelephone(linkTel)) {
            MsgDialog.Alert(false, "联系电话格式不正确，必须是中国大陆手机号码或座机号码");
            return;
        }
        if (isEmpty(reason)) {
            MsgDialog.Alert(false, "申请理由不能为空");
            return;
        }
        $.post("/me/ToBeSupplier?t=" + Math.random(), { linkMan: linkMan, linkTel: linkTel, reason: reason }, function (data) {
            MsgDialog.Alert(data.status, data.msg, function () {
                if (data.status) {
                    window.location.href = window.location.href;
                }
            });
        });
    },
    //删除申请记录
    delete: function (applyId) {
        MsgDialog.Confirm("确认删除此产品吗？", function () {
            $.get("/me/DeleteMarketProduct?t=" + Math.random(), { id: applyId }, function (data) {
                MsgDialog.Alert(data.status, data.msg, function () {
                    if (data.status) {
                        window.location.href = window.location.href;
                    }
                });
            });
        });
    },
    //商品下架
    goodsDown: function (id) {
        var html = '<div class="form-group"><input type="text" class="form-control" id="txt_content"  maxlength="200" placeholder="请输入下架理由,200字以内" /></div>';

        BootstrapDialog.show({
            title: '商品下架',
            message: $(html),
            closable: false,
            buttons: [{
                id: "btn_submit",
                icon: 'glyphicon glyphicon-send',
                label: 'Save',
                cssClass: 'btn-primary',
                autospin: true,
                action: function (dialogRef) {
                    dialogRef.enableButtons(false);
                    dialogRef.setClosable(false);
                    var $button = dialogRef.getButton('btn_submit');
                    var remark = $("#txt_content").val();

                    var url = "/Me/GoodsDown?t=" + Math.random();

                    $.get(url, { goodsId: id, remark: remark }, function (data) {

                        MsgDialog.Alert(data.status, data.msg, function () {
                            if (data.status) {
                                window.location.href = window.location.href;
                            }
                        });
                        $button.stopSpin();
                        dialogRef.enableButtons(true);
                        dialogRef.setClosable(true);
                    });
                }
            }, {
                label: 'Close',
                action: function (dialogRef) {
                    dialogRef.close();
                }
            }]
        });
    }
}
//我的评论
var myComments = {
    deleteComment: function (id) {
        MsgDialog.Confirm("确定要删除此评论吗?", function () {
            $.get("/Me/DeleteComment?t=" + Math.random(), { id: id }, function (data) {
                MsgDialog.Alert(data.status, data.msg, function () {
                    if (data.status) {
                        window.location.href = window.location.href;
                    }
                });
            });
        });
    }
}
//我的站内信
var myNotifies = {
    //删除站内信
    deleteNotify: function (id) {
        MsgDialog.Confirm("确定要删除此站内信吗?", function () {
            $.get("/Me/DeleteMyNotify?t=" + Math.random(), { id: id }, function (data) {
                MsgDialog.Alert(data.status, data.msg, function () {
                    if (data.status) {
                        window.location.href = window.location.href;
                    }
                });
            });
        });
    },
    //改变站内信状态为已读
    readNotify: function (id, isReaded) {
        $(".st_hide").hide();
        $(".st_hide").eq($(this).attr("st_show")).show();
        if (!isReaded) {
            $.get("/Me/ToBeRead?t=" + Math.random(), { id: id }, function (data) {
                if (data.status) {
                    $("#sp_notify_state").html('已读');
                }
            });
        }
    }
}


var accounts = {
    modifyPwd: function () {
        var token = $("input[name='__RequestVerificationToken']").val();
        var oldPwd = $("#txt_pwd_old").val();
        var newPwd = $("#txt_pwd_new").val();
        var confirmPwd = $("#txt_pwd_new_confirm").val();
        if (oldPwd.length < 8 || oldPwd.length > 16) {
            MsgDialog.Alert(false, "请输入8-16位的原密码");
            return;
        }
        if (newPwd.length < 8 || newPwd.length > 16) {
            MsgDialog.Alert(false, "请输入8-16位的新密码");
            return;
        }

        if (confirmPwd != newPwd) {
            MsgDialog.Alert(false, "两次密码输入不一致");
            return;
        }
        $(".entre_f").showLoading();
        $.post("/me/ModifyPassword?t=" + Math.random(), { oldPwd: oldPwd, newPwd: newPwd, __RequestVerificationToken: token }, function (data) {
            MsgDialog.Alert(data.status, data.msg, function () {
                if (data.status) {
                    window.location.href = window.location.href;
                } else {
                    $(".entre_f").hideLoading();
                }
            });
        });
    },
    setDefault: function (id) {
        $.get("/me/SetDefaultAddress?t=" + Math.random(), { id: id }, function (data) {
            $(".entre_f").showLoading();
            MsgDialog.Alert(data.status, data.msg, function () {
                if (data.status) {
                    window.location.href = window.location.href;
                } else {
                    $(".entre_f").hideLoading();
                }
            });
        });
    },
    deleteAddress: function (id) {
        MsgDialog.Confirm("确定要删除此收货地址吗?", function () {
            $(".entre_f").showLoading();
            $.get("/Me/DeleteAddress?t=" + Math.random(), { id: id }, function (data) {
                MsgDialog.Alert(data.status, data.msg, function () {
                    if (data.status) {
                        window.location.href = window.location.href;
                    } else {
                        $(".entre_f").hideLoading();
                    }
                });
            });
        });
    },
    ToEditAddressModel: function (id) {
        var receiverName = $("#sp_txt_ReciverName_"+id).html();
        var tel = $("#sp_txt_ReciverTel_" + id).html();
        var detail = $("#sp_txt_DetailAddress_" + id).html();
        var schoolId = $("#h_schoolId_" + id).val();
        var regionId = $("#h_regionId_" + id).val();
        $("#h_address_id").val(id);
        $("#txt_address_receiverName").val(receiverName);
        $("#txt_address_tel").val(tel);
        $("#sel_address_school").val(schoolId);
        $("#sel_address_region").val(regionId);
        $("#txt_address_detail").val(detail);
        $("#btn_address_save").html("编辑地址");
        $("#btn_quitEditModel").show();
    },
    quitEditModel: function () {
        $("#h_address_id").val('');
        $("#txt_address_receiverName").val('');
        $("#txt_address_tel").val('');
        $("#sel_address_school").val('-1');
        $("#txt_address_detail").val('');
        $("#btn_address_save").html("添加新收货地址");
        $("#btn_quitEditModel").hide();
    },
    saveAddress: function () {
        var receiverName = $("#txt_address_receiverName").val();
        var tel = $("#txt_address_tel").val();
        var regionId = parseInt($("#sel_address_region").val());
        var regionName = $("#sel_address_region").find("option:selected").text();
        var schoolId = parseInt($("#sel_address_school").val());
        var school = $("#sel_address_school").find("option:selected").text();
        var detail = $("#txt_address_detail").val();
        var id = $("#h_address_id").val();
        if (isEmpty(receiverName)) {
            MsgDialog.Alert(false, "请输入收货人姓名");
            return;
        }

        if (!ISMobile(tel)) {
            MsgDialog.Alert(false, "联系电话格式不正确");
            return;
        }
        if (schoolId <= 0) {
            MsgDialog.Alert(false, "请选择所在学校");
            return;
        }
        $(".entre_f").showLoading();
        var cityName = $("#sel_address_ctiy").find("option:selected").text();
        $.post("/me/SaveAddress?t=" + Math.random(), {
            Id: id, ReciverName: receiverName, ReciverTel: tel, SchoolId: schoolId, SchoolName: school,
            CityName: cityName, RegionId: regionId, RegionName: regionName, DetailAddress: detail
        }, function (data) {
            MsgDialog.Alert(data.status, data.msg, function () {
                if (data.status) {
                    window.location.href = window.location.href;
                } else {
                    $(".entre_f").hideLoading();
                }
            });
        });
    }
}
