var MobileAddress = {
    //地址设为默认
    setDefault: function (id) {
        $.get("/me/SetDefaultAddress?t=" + Math.random(), { id: id }, function (data) {
            MsgDialog.Alert(data.status, data.msg, function () {
                if (data.status) {
                    window.location.href = window.location.href;
                }
            });
        });
    },
    //删除地址
    deleteAddress: function (id) {
        MsgDialog.Confirm("确定要删除此收货地址吗?", function () {
            $.get("/Me/DeleteAddress?t=" + Math.random(), { id: id }, function (data) {
                MsgDialog.Alert(data.status, data.msg, function () {
                    if (data.status) {
                        window.location.href = window.location.href;
                    }
                });
            });
        });
    },
    //手机端保存收货地址
    SaveMobileAddress: function () {
        var id = $.trim($("#h_currentEditAddress_Id").val());
        var receiverName = $.trim($("#txt_cart_address_new_receiver").val());
        var tel = $("#txt_cart_address_new_mobile").val();
        var regionId = parseInt($("#regionId").val());
        var regionName = $.trim($("#sel_cart_address_new_region").val());
        var schoolId = parseInt($("#schoolId").val());
        var school = $.trim($("#sel_cart_address_new_school").val());
        var detail = $.trim($("#txt_cart_address_new_detail").val());
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
        $.cookie("receiverName", "");
        $.cookie("mobile", "");
        $.cookie("detail", "");
        var cityName = $.trim($("#sel_cart_address_new_ctiy").val());
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
                if (parameter == "") {
                    window.location.href = "/Mobile/me/MyAddresses";
                } else {
                    window.location.href = "/Mobile/home/SelectAddresses?p=" + encodeURIComponent(parameter) + "&couponId=" + $.trim($("#couponId").val());
                }
            } else {
                $("#errorMsg").text(data.msg);
            }
        });
    },
    //选择地址
    ClickAddress: function (obj, addressId) {
        $("#allAddress").children().removeClass("msg_dz");
        $(obj).addClass("msg_dz");
        $("#addressId").val(addressId);
    }
};