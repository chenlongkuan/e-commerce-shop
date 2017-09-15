
//浮点数加法运算  
function FloatAdd(arg1, arg2) {
    var r1, r2, m;
    try { r1 = arg1.toString().split(".")[1].length } catch (e) { r1 = 0 }
    try { r2 = arg2.toString().split(".")[1].length } catch (e) { r2 = 0 }
    m = Math.pow(10, Math.max(r1, r2))
    return (arg1 * m + arg2 * m) / m
}

//浮点数减法运算  
function FloatSub(arg1, arg2) {
    var r1, r2, m, n;
    try { r1 = arg1.toString().split(".")[1].length } catch (e) { r1 = 0 }
    try { r2 = arg2.toString().split(".")[1].length } catch (e) { r2 = 0 }
    m = Math.pow(10, Math.max(r1, r2));
    //动态控制精度长度  
    n = (r1 >= r2) ? r1 : r2;
    return ((arg1 * m - arg2 * m) / m).toFixed(n);
}

//浮点数乘法运算  
function FloatMul(arg1, arg2) {
    var m = 0, s1 = arg1.toString(), s2 = arg2.toString();
    try { m += s1.split(".")[1].length } catch (e) { }
    try { m += s2.split(".")[1].length } catch (e) { }
    return Number(s1.replace(".", "")) * Number(s2.replace(".", "")) / Math.pow(10, m)
}


//浮点数除法运算  
function FloatDiv(arg1, arg2) {
    var t1 = 0, t2 = 0, r1, r2;
    try { t1 = arg1.toString().split(".")[1].length } catch (e) { }
    try { t2 = arg2.toString().split(".")[1].length } catch (e) { }
    with (Math) {
        r1 = Number(arg1.toString().replace(".", ""))
        r2 = Number(arg2.toString().replace(".", ""))
        return (r1 / r2) * pow(10, t2 - t1);
    }
}


$(function () {
    //数量增减
    $('[data-trigger="spinner"]').spinner('delay', 100).spinner('changed', function (e, newVal, oldVal) {
        //trigger lazed, depend on delay option.
        var goodsId = $(this).attr("goodsid");
        if ($(".not_enough").length == 0) {
            updateCartGoods(goodsId);
        } else {
            $("#txt_cart_buyCount_goods_" + goodsId).val(oldVal);
        }

    });
    //全选
    $("input[name='cbx_cart_all']").bind("change", function () {
        var $checkAllBtn = $("input[name='cbx_cart_all']");
        MsgFunc.CboxCheckAll($checkAllBtn, "cbx_cart_item");
        $(".sp_cbx_cart_all_desc").html($checkAllBtn.attr("title"));
        if ($checkAllBtn.is(":checked")) {
            $checkAllBtn.removeAttr("checked");
        } else {
            $checkAllBtn.each(function () {
                $(this).prop("checked", "checked");
            });
        }

    });
    //删除商品
    $(".btn_cart_delete_goods").bind("click", function () {
        var goodsId = $(this).attr("goodsId");
        MsgDialog.Confirm("确定要从购物车删除此商品吗？", function () {
            $.get("/home/RemoveGoodsFromCart?t=" + Math.random(), { ids: goodsId }, function (data) {
                MsgDialog.Alert(data.status, data.msg, function () {
                    if (data.status) {
                        $("#tr_item_" + goodsId).fadeOut();
                        $("#tr_item_" + goodsId).remove();
                        $("#div_cart_head_quantity").html(data.quantity);
                        $("#sp_cart_right_quantity").html(data.quantity);
                        ReCalculatePrice();
                    }
                });
            });
        });
    });
    //删除所选商品
    $("#btn_cart_delete_all_goods").bind("click", function () {
        var ids = MsgFunc.CboxCheckedValues("cbx_cart_item");
        MsgDialog.Confirm("确定要从购物车删除所选商品吗？", function () {
            $.get("/home/RemoveGoodsFromCart?t=" + Math.random(), { ids: ids }, function (data) {
                MsgDialog.Alert(data.status, data.msg, function () {
                    if (data.status) {
                        var idsArr = ids.split(',');
                        for (var id in idsArr) {
                            if (idsArr.hasOwnProperty(id)) {
                                $("#tr_item_" + idsArr[id]).fadeOut();
                                $("#tr_item_" + idsArr[id]).remove();
                            }
                        }
                        ReCalculatePrice();
                        $("#div_cart_head_quantity").html(data.quantity);
                        $("#sp_cart_right_quantity").html(data.quantity);
                    }
                });
            });
        });
    });
    ////更新购物车总价格和购物车cookie
    //$(".calculate").bind("click", function () {
    //    var goodsId = $(this).attr("goodsId");
    //    updateCartGoods(goodsId);
    //});

    //var stepperInputEventType = "propertychange";
    //if (!$.support.leadingWhitespace) {
    //    stepperInputEventType = "keyup";
    //}
    //////当数量文本框发生值改变则修改购物车对应商品的数量
    //$(".stepper-input").bind(stepperInputEventType, function () {
    //    var goodsId = $(this).attr("goodsid");
    //    updateCartGoods(goodsId);
    //});

    preOrder.changeToAddressEditModel();
    preOrder.regionChange();
    preOrder.saveAddress();
    preOrder.deleteAddress();
    preOrder.changeCoupon();
    preOrder.submitOrder();
});

function updateCartGoods(goodsId) {
    var quantity = parseInt($("#txt_cart_buyCount_goods_" + goodsId).val());
    //var singlePrice = parseFloat($("#sp_single_price_goods_" + goodsId).html());
    $.get("/home/UpdateCart?t=" + Math.random(), { goodsId: goodsId, quantity: quantity, type: "update" }, function (data) {
        if (!data.status) {
            MsgDialog.Alert(data.status, data.msg);
            return;
        } else {
            $("#div_cart_head_quantity").html(data.quantity);
            $("#sp_cart_right_quantity").html(data.quantity);

            ReCalculatePrice();
        }
        if (data.update.LeftQuantity - parseInt(quantity) <= 1) {
            $("#txt_cart_buyCount_goods_" + goodsId).attr("data-max", data.update.LeftQuantity);

            $("#txt_cart_buyCount_goods_" + goodsId).parent().parent().append("<span style='color: red' class='not_enough'>库存不足</span>");
        } else {
            $(".not_enough").remove();
        }
    });

}

function ReCalculatePrice() {
    //重新计算总价格
    var sumPrice = 0.00;
    $(".singlePrice").each(function () {
        var otherId = $(this).attr("goodsid");
        var otherQuantity = parseInt($("#txt_cart_buyCount_goods_" + otherId).val());
        var preSum = FloatMul(parseFloat($(this).html()), otherQuantity);
        sumPrice = FloatAdd(preSum, sumPrice);
    });

    $("#sp_cart_sumprice").html(sumPrice);
}

var preOrder = {
    //地址选择切换到编辑模式
    changeToAddressEditModel: function () {
        $("#btn_address_edit").bind("click", function () {
            $("#div_view_model").hide();
            $("#div_edit_model").show();
        });
    },
    //区域切换对应学校联动
    regionChange: function () {
        $("#sel_cart_address_new_region").bind("change", function () {
            var regionId = $(this).val();
            var html = "<option value='-1'>--请选择--</option>";
            if (regionId == '-1') {
                $("#sel_cart_address_new_school").html(html);
                return;
            } else {
                $.get("/common/GetSchoolByRegion?t=" + Math.random(), { regionId: regionId }, function (data) {
                    if (data != '') {
                        $(data).each(function () {
                            html += "<option value='" + this.Id + "'>" + this.Name + "</option>";
                        });

                    }
                    $("#sel_cart_address_new_school").html(html);
                });
            }
        });

    },

    saveAddress: function () {
        $("#btn_cart_address_save").bind("click", function () {
            var isEdit = $("#h_isEditAddress").val();
            if (isEdit) { //编辑
                var id = $("#h_currentEditAddress_Id").val();
                preOrder.saveAddressRefact(id);
            }
                //使用新地址
            else if ($("#cbx_cart_address_new").is(":checked")) {
                preOrder.saveAddressRefact(0);

            } else { //仅选择其他可用的地址
                var addId = $("input[name='cbx_cart_address_item']:checked").val();
                if (addId == 'new') {
                    preOrder.saveAddressRefact(0);
                } else {
                    $("#h_cart_address_id").val(addId);
                    $("#div_edit_model").hide();
                    $("#div_view_model").show();
                    var addHtml = $("#div_cart_edit_model_address_" + addId).html();
                    $("#div_view_model_address_container").html(addHtml);
                }
            }

        });
    },
    //保存新地址验证
    saveAddressRefact: function (id) {
        var receiverName = $("#txt_cart_address_new_receiver").val();
        var tel = $("#txt_cart_address_new_mobile").val();
        var regionId = parseInt($("#sel_cart_address_new_region").val());
        var regionName = $("#sel_cart_address_new_region").find("option:selected").text();
        var schoolId = parseInt($("#sel_cart_address_new_school").val());
        var school = $("#sel_cart_address_new_school").find("option:selected").text();
        var detail = $("#txt_cart_address_new_detail").val();
        if (isEmpty(receiverName)) {
            MsgDialog.Alert(false, "请输入收货人姓名");
            return;
        }
        if (!ISMobile(tel)) {
            MsgDialog.Alert(false, "联系电话格式不正确");
            return;
        }
        if (regionId <= 0) {
            MsgDialog.Alert(false, "请选择所在区域");
            return;
        }
        if (schoolId <= 0) {
            MsgDialog.Alert(false, "请选择所在学校");
            return;
        }
        $("#div_edit_model").showLoading();
        var cityName = $("#sel_cart_address_new_ctiy").find("option:selected").text();
        $.post("/me/SaveAddress?t=" + Math.random(), {
            Id: id,
            ReciverName: receiverName,
            ReciverTel: tel,
            SchoolId: schoolId,
            SchoolName: school,
            CityName: cityName,
            RegionId: regionId,
            RegionName: regionName,
            DetailAddress: detail
        }, function (data) {
            if (data.status) {
                $("#txt_cart_address_new_receiver").val("");
                $("#txt_cart_address_new_mobile").val("");
                $("#sel_cart_address_new_region").val("");
                $("#sel_cart_address_new_school").val("");
                $("#txt_cart_address_new_detail").val("");
                $("#h_cart_address_id").val(data.obj.Id);
                var addHtml = '<div class="div_edit" id="div_edit_model_existsAddress_item_' + data.obj.Id + '"> <input name="cbx_cart_address_item" type="radio" value="' + data.obj.Id + '" checked="checked">';
                addHtml += ' <span id="div_cart_edit_model_address_' + data.obj.Id + '">';
                addHtml += '<span>&nbsp;</span>' + data.obj.ReciverName;
                addHtml += ' <span>&nbsp;&nbsp;&nbsp;</span>';
                addHtml += data.obj.CityName + '&nbsp;&nbsp;' + data.obj.RegionName + '&nbsp;&nbsp;' + data.obj.SchoolName + '&nbsp;&nbsp;' + data.obj.DetailAddress;
                addHtml += ' <span>&nbsp;&nbsp;&nbsp;</span>';
                addHtml += data.obj.ReciverTel;
                addHtml += ' <span>&nbsp;&nbsp;</span></span>';
                addHtml += '<a href="javascript:;"  onclick="' + preOrder.editAddress(data.obj.Id, data.obj.ReciverName, data.obj.DetailAddress, data.obj.ReciverTel, data.obj.RegionId, data.obj.SchoolId) + ';return false;">编辑</a>';
                addHtml += ' <span>&nbsp;&nbsp;</span>';
                addHtml += '<a href="javascript:;" class="btn_cart_edit_model_address_delete" onclick="preOrder.deleteAddress();return false;" addId="' + data.obj.Id + '">删除</a></div>';
                $("#div_edit_model_existsAddress").append(addHtml);
            } else {
                MsgDialog.Alert(data.status, data.msg);
            }

            $("#div_edit_model").hideLoading();
        });
    },
    editAddress: function (id, receiver, detail, mobile, regionId, schoolId) {
        $("#h_currentEditAddress_Id").val(id);
        $("#txt_cart_address_new_receiver").val(receiver);
        $("#txt_cart_address_new_mobile").val(mobile);
        $("#sel_cart_address_new_region").val(regionId);
        $("#sel_cart_address_new_school").val(schoolId);
        $("#txt_cart_address_new_detail").val(detail);
    },
    deleteAddress: function () {
        $(".btn_cart_edit_model_address_delete").bind("click", function () {
            var id = $(this).attr("addId");
            MsgDialog.Confirm("确定要删除此收货地址吗?", function () {
                $("#div_edit_model").showLoading();
                $.get("/Me/DeleteAddress?t=" + Math.random(), { id: id }, function (data) {
                    MsgDialog.Alert(data.status, data.msg, function () {
                        if (data.status) {
                            $("#div_edit_model_existsAddress_item_" + id).fadeOut();
                        }
                        $("#div_edit_model").hideLoading();
                    });
                });
            });
        });
    },
    //切换优惠券
    changeCoupon: function () {
        //被选中的优惠券价值
        var couponValue = parseFloat($("input[name='cbx_cart_coupons_item']:checked").attr("couponValue"));
        //当前显示的优惠券价值
        var showCouponValue = parseFloat($("#sp_submitOrder_coupon_value").html());
        //应付价格
        var shouldPay = parseFloat($("#sp_submitOrder_shouldpay").html());
        if (isNaN(couponValue) || couponValue == 0) {
            $("#sp_submitOrder_coupon_value").html(0);
            //$("#sp_submitOrder_shouldpay").html(shouldPay);
            return;
        }
        $("#sp_submitOrder_coupon_value").html(couponValue);
        //计算应该计入总价格的优惠券价值
        var fillInCouponPay = 0;
        if (couponValue > showCouponValue) {
            fillInCouponPay = couponValue - showCouponValue;
        } else {
            fillInCouponPay = showCouponValue - couponValue;
        }
        //计算总价
        shouldPay = shouldPay - fillInCouponPay;
        ////计算邮费
        //var defaultExpressCost = parseFloat($("#h_cart_default_express_cost").val());
        //var defaultFreeExpressCost = parseFloat($("#h_cart_default_free_express_cost").val());
        //if (shouldPay < defaultFreeExpressCost) {
        //    shouldPay += defaultExpressCost;
        //    $("#sp_submit_order_expresscost").val(defaultExpressCost);
        //}
        $("#sp_submitOrder_shouldpay").html(shouldPay);
    },

    submitOrder: function () {
        $("#btn_submit_order").bind("click", function () {
            var addressId = parseInt($("#h_cart_address_id").val());
            var payway = $("input[name='payway']:checked").val();
            var expressWay = $("input[name='expressway']:checked").val();
            var sendTimeBuckets = $("input[name='SendTimeBuckets']:checked").val();
            var couponCount = parseInt($("#h_cart_coupons_count").val());
            var sendDate = $("#sel_send_date").val();
            var couponId = 0;
            if (couponCount > 0) {
                couponId = parseInt($("input[name='cbx_cart_coupons_item']:checked").val());
            }
            if (isNaN(addressId) || addressId == 0) {
                MsgDialog.Alert(false, "收货地址参数错误，请刷新页面重试", function () {
                    window.location.href = window.location.href;
                });
                return;
            }
            if (isNaN(couponId) && couponCount > 0) {
                MsgDialog.Alert(false, "优惠券参数错误，请刷新页面重试", function () {
                    window.location.href = window.location.href;
                });
                return;
            }

            window.location.href = "/home/CreateOrder?addressId=" + addressId + "&payway=" + payway + "&expressWay=" + expressWay + "&sendTimeDate=" + sendDate + "&sendTimeBuckets=" + sendTimeBuckets + "&couponId=" + couponId;
        });
    },
    //手机端保存收货地址
    SaveMobileAddress: function () {
        var id = $("#h_currentEditAddress_Id").val();
        var receiverName = $("#txt_cart_address_new_receiver").val();
        var tel = $("#txt_cart_address_new_mobile").val();
        var regionId = parseInt($("#sel_cart_address_new_region").val());
        var regionName = $("#sel_cart_address_new_region").find("option:selected").text();
        var schoolId = parseInt($("#sel_cart_address_new_school").val());
        var school = $("#sel_cart_address_new_school").find("option:selected").text();
        var detail = $("#txt_cart_address_new_detail").val();
        if (isEmpty(receiverName)) {
            $("#errorMsg").text("请输入收货人姓名");
            return;
        }
        if (!ISMobile(tel)) {
            $("#errorMsg").text("联系电话格式不正确");
            return;
        }
        if (regionId <= 0) {
            $("#errorMsg").text("请选择所在区域");
            return;
        }
        if (schoolId <= 0) {
            $("#errorMsg").text("请选择所在学校");
            return;
        }
        var cityName = $("#sel_cart_address_new_ctiy").find("option:selected").text();
        $.post("/me/SaveAddress?t=" + Math.random(), {
            Id: id,
            ReciverName: receiverName,
            ReciverTel: tel,
            SchoolId: schoolId,
            SchoolName: school,
            CityName: cityName,
            RegionId: regionId,
            RegionName: regionName,
            DetailAddress: detail
        }, function (data) {
            if (data.status) {
                var parameter = $.trim($("#p").val());
                window.location.href = "/Mobile/home/CheckOut?p=" + encodeURIComponent(parameter);
            } else {
                $("#errorMsg").text(data.msg);
            }
        });
    },
    //手机端提交订单
    submitMobileOrder: function () {
        var addressId = parseInt($("#h_cart_address_id").val());
        var payway = $.trim($("#h_cart_payway").val());
        var expressWay = $.trim($("#expressway").val());
        var sendTimeBuckets = $.trim($("#SendTimeBuckets").val());
        var couponCount = parseInt($("#h_cart_coupons_count").val());
        var sendDate = $("#sel_send_date").val();
        var couponId = 0;
        if (couponCount > 0) {
            couponId = parseInt($("#h_currentCouponId").val());
        }
        if (isNaN(addressId) || addressId == 0) {
            $("#errorMsg").text("收货地址参数错误，请刷新页面重试");
            window.location.href = window.location.href;
            return;
        }
        if (isNaN(couponId) && couponCount > 0) {
            $("#errorMsg").text("优惠券参数错误，请刷新页面重试");
            window.location.href = window.location.href;
            return;
        }
        window.location.href = "/home/CreateOrder?addressId=" + addressId + "&payway=" + payway + "&expressWay=" + expressWay + "&sendTimeDate=" + sendDate + "&sendTimeBuckets=" + sendTimeBuckets + "&couponId=" + couponId;
    },
    //移动端选择收货时间
    selTime: function(time,obj) {
        $("#sendTimeType").val(time);
        $(obj).parent().children("a").children("dl").children("dt").html("");
        $(obj).children().children("dt").html('<img src="/Content/MobileTemplate/images/g.png">');
    },
    //确认选择收货时间
    SelTimeOk: function() {
        var parameter = $.trim($("#p").val());
        var couponId = $.trim($("#couponId").val());
        var sendTimeType = $.trim($("#sendTimeType").val());
        var sendDate = $.trim($("#sendDate").val());
        window.location.href = "/Mobile/home/CheckOut/" + couponId + "?p=" + encodeURIComponent(parameter) + "&sendTimeType=" + sendTimeType + "&sendDate=" + sendDate;
    }
};


