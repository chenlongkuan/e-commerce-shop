$(function () {
    $("#btn_regist_submit").bind("click", function () {
        var nickName = $("#txt_regist_nickName").val();
        var pwd = $("#txt_regist_pwd").val();
        var confirmPwd = $("#txt_regist_confirmPwd").val();
        var mobile = $("#txt_regist_mobile").val();
        var email = $("#txt_regist_email").val();
        var schoolId = $("#sel_regist_school").val();
        if (!IsEmail(email)) {
            $("#errorMsg").show().text("邮箱格式不正确");
            window.setTimeout('$("#errorMsg").hide().text("");', 10000);
            return;
        }
        if (isEmpty(nickName)) {
            $("#errorMsg").show().text("请输入昵称");
            window.setTimeout('$("#errorMsg").hide().text("");', 10000);
            return;
        }
        if (pwd.length < 8 || pwd.length > 16) {
            $("#errorMsg").show().text("请输入8-16位密码");
            window.setTimeout('$("#errorMsg").hide().text("");', 10000);
            return;
        }
        if (pwd != confirmPwd) {
            $("#errorMsg").show().text("两次输入密码不一致");
            window.setTimeout('$("#errorMsg").hide().text("");', 10000);
            return;
        }
        if (!ISMobile(mobile)) {
            $("#errorMsg").show().text("手机号码格式不正确");
            window.setTimeout('$("#errorMsg").hide().text("");', 10000);
            return;
        }

        if (schoolId == '-1') {
            $("#errorMsg").show().text("请选择所在学校");
            window.setTimeout('$("#errorMsg").hide().text("");', 10000);
            return;
        }
        $.cookie("email", "");
        $.cookie("nickName", "");
        $.cookie("pwd", "");
        $.cookie("mobile", "");
        $("#registForm").submit();
    });
    //检测邮箱
    $("#txt_regist_email").bind("blur", function () {
        var email = $("#txt_regist_email").val();
        $.post("/Account/CheckEmail?t=" + Math.random(), { email: email }, function (data) {
            if (data.status) {
                $("#errorMsg").show().text("该邮箱已被注册，请修改");
                window.setTimeout('$("#errorMsg").hide().text("");', 10000);
            }

        });
    });

    //检测昵称
    $("#txt_regist_nickName").bind("blur", function () {
        var nickName = $("#txt_regist_nickName").val();
        $.post("/Account/CheckUserName?t=" + Math.random(), { username: nickName }, function (data) {
            if (data.status) {
                $("#errorMsg").show().text("该昵称已经存在，请修改");
                window.setTimeout('$("#errorMsg").hide().text("");', 10000);
            }

        });
    });
    $("#txt_regist_email").bind("blur", function () {
        $.cookie("email", $.trim($("#txt_regist_email").val()));
    });
    $("#txt_regist_nickName").bind("blur", function () {
        $.cookie("nickName", $.trim($("#txt_regist_nickName").val()));
    });
    $("#txt_regist_pwd").bind("blur", function () {
        $.cookie("pwd", $.trim($("#txt_regist_pwd").val()));
    });
    $("#txt_regist_mobile").bind("blur", function () {
        $.cookie("mobile", $.trim($("#txt_regist_mobile").val()));
    });

    $("#sel_regist_region").bind("change", function () {
        var regionId = $(this).val();
        var html = "<option value='-1'>--请选择--</option>";
        if (regionId == '-1') {
            $("#sel_regist_school").html(html);
            return;
        } else {
            $.get("/common/GetSchoolByRegion?t=" + Math.random(), { regionId: regionId }, function (data) {
                if (data != '') {
                    $(data).each(function () {
                        html += "<option value='" + this.Id + "'>" + this.Name + "</option>";
                    });
                }
                $("#sel_regist_school").html(html);
            });
        }
    });

    $("#btn_login_submit").bind("click", function () {
        var email = $.trim($("#txt_login_Account").val());
        var pwd = $.trim($("#txt_login_Password").val());
        var rem = $("#cbx_login_IsRememberLogin").val();
        if (!IsEmail(email)) {
            $("#errorMsg").show().text("邮箱格式不正确");
            window.setTimeout('$("#errorMsg").hide().text("");', 10000);
            return;
        }
        if (pwd.length < 8 || pwd.length > 16) {
            $("#errorMsg").show().text("请输入8-16位密码");
            window.setTimeout('$("#errorMsg").hide().text("");', 10000);
            return;
        }
        $("form").submit();
    });
});

function remLoginChange(obj) {
    var ischecked = $(obj).is(":checked");
    if (ischecked) {
        $(obj).val("true");
    } else {
        $(obj).val("false");
    }
}
