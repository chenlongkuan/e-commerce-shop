//是否为空
function isEmpty(str) {
    var idval = str;
    if (idval == '' || $.trim(idval).length < 1) {
        return true;
    }
    return false;
}


//判断字符串是否为中文
function isChinese(str) {
    var lst = /[u00-uFF]/;
    return !lst.test(str);
}
//获取字符串实际长度
function strlen(str) {
    var strlength = 0;
    for (i = 0; i < str.length; i++) {
        if (isChinese(str.charAt(i)) == true)
            strlength = strlength + 2;
        else
            strlength = strlength + 1;
    }
    return strlength;
}
/***电话号码验证 开始***/
function IsTelephone(obj)// 正则判断
{
    var pattern = /(^[0-9]{3,4}\-[0-9]{3,8}$)|(^[0-9]{3,8}$)|(^\([0-9]{3,4}\)[0-9]{3,8}$)|(^13[0-9]\d{8}$)|(^15[012356789]\d{8}$)|(^18[012356789]\d{8}$)|(^14[57]\d{8}$)/;
    if (pattern.test(obj)) {
        return true;
    }
    else {
        return false;
    }
};
//手机验证
function ISMobile(val) {
    if (val.length != 11 || isNaN(val)) {
        return false;
    }
    var re = /^(13[0-9]|15[012356789]|17[678]|18[0-9]|14[57])[0-9]{8}$/;
    if (!re.test(val)) {
        return false;
    }
    return true;
}
/***电话号码验证  结束***/

function IsEmail(str) {
    var myRegExp = /[a-z0-9-.]{1,30}@[a-z0-9-]{1,65}.(com|net|org|info|biz|([a-z]{2,3}.[a-z]{2}))/;
    return myRegExp.test(str);
}
function setCookie(cName, value, exdate) {
    var exdateExp = new Date();
    if (exdate != null) {
        exdateExp.getMinutes() + exdate;
    }
    document.cookie = cName + "=" + escape(value) + ((exdate == null) ? "" : ";expires=" + exdateExp.toGMTString());
}
function getCookie(cName) {
    if (document.cookie.length > 0) {
        var cStart = document.cookie.indexOf(cName + "=");
        if (cStart != -1) {
            cStart = cStart + cName.length + 1;
            var cEnd = document.cookie.indexOf(";", cStart);
            if (cEnd == -1) cEnd = document.cookie.length;
            return unescape(document.cookie.substring(cStart, cEnd));
        }
    }
    return null;
}
var MsgDialog = {

    Alert: function (status, msg, callBack) {
        var title = "成功!";
        var dialogType = BootstrapDialog.TYPE_SUCCESS;
        if (!status) {
            title = "失败!";
            dialogType = BootstrapDialog.TYPE_DANGER;
        }
        BootstrapDialog.alert({
            title: title,
            message: msg,
            type: dialogType, // <-- Default value is BootstrapDialog.TYPE_PRIMARY
            closable: false, // <-- Default value is true
            buttonLabel: 'OK', // <-- Default value is 'OK',
            callback: callBack
        });
    },

    /**
       * Confirm window
       * 
       * @param {type} message
       * @param {type} callback
       * @returns {undefined}
       */
    Confirm: function (message, callback) {
        new BootstrapDialog({
            title: '确认提示',
            type: BootstrapDialog.TYPE_WARNING,
            message: message,
            closable: false,
            data: {
                'callback': callback
            },
            buttons: [{
                label: '取消',
                action: function (dialog) {
                    //typeof dialog.getData('callback') === 'function' && dialog.getData('callback')(false);
                    dialog.close();
                }
            }, {
                label: '确认',
                cssClass: 'btn-primary',
                action: function (dialog) {
                    typeof dialog.getData('callback') === 'function' && dialog.getData('callback')(true);
                    dialog.close();
                }
            }]
        }).open();
    }
}

var MsgFunc = {
    //复选框全选
    //btnobj=按钮#Id 
    //cboxname=复选框name
    CboxCheckAll: function (btnobj, cboxname) {
        var $btn = $(btnobj);
        var checked = $btn.attr("title") == "全选";
        var $cbox = $("input[name='" + cboxname + "']:enabled");
        if (checked) {
            $cbox.each(function () {
                $(this).prop("checked", "checked");
            });
            $btn.attr("title", "全不选");
        } else {
            $cbox.removeAttr("checked");
            $btn.attr("title", "全选");
        }
    },
    //复选框反选
    CboxInverse: function (cboxname) {
        $("input[name='" + cboxname + "']:enabled").each(function () {
            $(this).prop("checked", !this.checked);
        });
    },
    //获取复选框选中的值
    CboxCheckedValues: function (cboxname) {
        var checkedvalue = "";
        $("input[name='" + cboxname + "']:enabled").each(function () {
            if (this.checked) {
                if (checkedvalue == "") {
                    checkedvalue += $(this).val();
                } else {
                    checkedvalue += "," + $(this).val();
                }
            }
        });
        return checkedvalue;
    },
    //获取复选框选中的属性值
    CboxCheckedattrs: function (cboxname, attr) {
        var checkedvalue = "";
        $("input[name='" + cboxname + "']:enabled").each(function () {
            if (this.checked) {
                if (checkedvalue == "") {
                    checkedvalue += $(this).attr(attr);
                } else {
                    checkedvalue += "," + $(this).attr(attr);
                }
            }
        });
        return checkedvalue;
    }
}



//加入购物车
function updateCart(goodsId, quantity, type, callback, obj) {
    if (goodsId <= 0) {
        MsgDialog.Alert(false, "商品Id不正确");
        return;
    }
    if (quantity == 0) {
        quantity = parseInt($("#txt_detail_buyCount").val());
    }
    if (isNaN(quantity) || quantity <= 0) {
        MsgDialog.Alert(false, "商品数量不正确");
        return;
    }
    if ($(obj).attr("disabled") != 'disabled') {
        var img = $(obj).attr("img");
        $("#flyItem").find("img").attr("src", img);
        $("#flyItem").show();
    } else {
        MsgDialog.Alert(false, "该商品库存不足");
        return;
    }

    $.get("/home/UpdateCart?t=" + Math.random(), { goodsId: goodsId, quantity: quantity, type: type }, function (data) {
        if (!data.status) {
            MsgDialog.Alert(data.status, data.msg);
            return;
        } else {
            $("#div_cart_head_quantity").html(data.update.Quantity);
            $("#sp_cart_right_quantity").html(data.update.Quantity);

            if (callback) {
                callback();
            }
        }

        if (data.update.LeftQuantity - parseInt(quantity) <= 1) {
            $(obj).disabledButton(true);
            $(obj).html("库存不足");
        }
    });
}
(function ($) {

    $.fn.disabledButton = function (disabled) {
        if (disabled) {
            $(this).attr("disabled", true);
            $(this).children().addClass("msg-ui-button-disabled");
        } else {
            $(this).attr("disabled", "");
            $(this).children().removeClass("msg-ui-button-disabled");
        }
    };
})(jQuery);