var app = (function () {
    /**
     * 禁止默认事件
     */
    var forbid = function () {
        /**
         * 禁止webapp跳转到safari（for ios）
         */
        if ((navigator.userAgent.indexOf('iPhone') != -1) || (navigator.userAgent.indexOf('iPod') != -1) || (navigator.userAgent.indexOf('iPad') != -1)) {
            $("a").click(function (event) {
                if ($(this).attr("href").indexOf("javascript") == -1) {
                    document.location.href = $(this).attr("href");
                    event.stopPropagation();
                    event.preventDefault();
                    return false;
                }
            });
        }
        /**
         * 隐藏地址栏
         */
        if (app.isMobile()) {
            var win = window,
                doc = win.document;
            if (!location.hash || !win.addEventListener) {
                window.scrollTo(0, 1);
                var scrollTop = 1,
                    bodycheck = setInterval(function () {
                        if (doc.body) {
                            clearInterval(bodycheck);
                            scrollTop = "scrollTop" in doc.body ? doc.body.scrollTop : 1;
                            win.scrollTo(0, scrollTop === 1 ? 0 : 1);
                        }
                    }, 15);
                win.addEventListener('load', function () {
                    setTimeout(function () {
                        win.scrollTo(0, scrollTop === 1 ? 0 : 1);
                    }, 0);
                }, false);
            }
        }
    };
    return {
        /**
         * 判断是否是移动设备 只判断了是否是安卓系统和IOS系统
         * @returns boolean
         */
        isMobile: function () {
            return /AppleWebKit.*Mobile/i.test(navigator.userAgent) || (/MIDP|SymbianOS|NOKIA|SAMSUNG|LG|NEC|TCL|Alcatel|BIRD|DBTEL|Dopod|PHILIPS|HAIER|LENOVO|MOT-|Nokia|SonyEricsson|SIE-|Amoi|ZTE/.test(navigator.userAgent));
        },
        //选择城市 mod:来源地址
        SelectProvinces: function (fun, mod, provinceName) {
            window.location.href = '/app/home/SelectProvinces?from=' + mod + "&provinceName=" + provinceName;
        },
        //选择学校 mod:来源地址
        selectSchool: function (fun, mod, schoolName) {
            window.location.href = '/app/home/SelectSchool?from=' + mod + "&schoolName=" + schoolName;
        },
        /**
         * 事件触发延迟
         * @fun {函数} 触发后处理方法
         * @timeout {对象字面量} 触发延迟时间
         * @opt {对象字面量} 参数列表
         */
        throttle: function (fun, timeout, opt) {
            var timer;
            var def = {}
            return function () {
                var self = this;
                clearTimeout(timer);
                timer = setTimeout(function () {
                    fun(self, opt);
                }, timeout);
            };
        },
        /**
         * resize事件处理
         * @fun {函数} 触发后处理事件
         */
        resize: function (fun) {
            window.addEventListener("orientationchange", app.throttle(fun, 200, {}));
        },
        /**
         * app初始化
         */
        init: function () {
            forbid();
        }
    };
})($);

//启动程序
$(function () {
    app.init();
});

/*插件*/
/*下拉加载数据*/
(function ($) {
    $.fn.loadData = function (opt) {
        var def = {
            "class": "u-loading",//默认样式
            "url": "",//加载数据请求地址 必需
            "page": 2,//分页加载
            "type": "",//其它参数
            "limit": 10,//分页加载每次加载数量
            "ajaxType": "json",//数据格式
            "funStart": function () {
            },//加载前触发的事件
            "funEnd": function () {
            },//加载结束触发的事件
            "funError": function () {
            } //加载失败触发的事件
        };
        opt = $.extend(def, opt);
        $(this).each(function (index) {
            var $self = $(this),
                $loading = $('<div class="u-loading"><div class="box"><span class="arrow z-load"></span><span class="txt">正在加载...</span></div></div>'),
                $loading_arrow = $loading.find(".arrow").eq(0),
                $loading_txt = $loading.find(".txt").eq(0);
            $self.append($loading.hide());
            var isload = true;
            if (app.isMobile()) {
                $(window).on("touchmove", function () {
                    scroll();
                });
                $(window).on("touchend", function () {
                    if ($(document).height() <= $(window).height() + $(window).scrollTop() && isload) {
                        $loading.show();
                        ajax();
                    }
                });
            } else {
                $(window).on("scroll mousewheel", app.throttle(function (obj) {
                    scroll();
                }, 100, {}));
            }
            var scroll = function () {
                if ($(document).height() <= $(window).height() + $(window).scrollTop() && isload) {
                    $loading.show();
                    if (!app.isMobile()) {
                        ajax();
                    }
                }
            };
            var ajax = function () {
                isload = false;
                opt.funStart();
                var url = opt.url + "?page=" + opt.page;
                if (opt.type != "") {
                    url += "&type=" + opt.type;
                }
                $.ajax({
                    url: url,
                    dataType: opt.ajaxType,
                    type: 'get',
                    success: function (data) {
                        $loading.hide();
                        opt.funEnd(data);
                        opt.page++;
                        isload = true;
                        if ((opt.ajaxType == "json" && data.length < opt.limit) || (opt.ajaxType == "html" && data == "")) {
                            isload = false;
                        }
                    },
                    error: function (e) {
                        $loading.hide();
                        isload = true;
                        opt.funError();
                    }
                });
            };
        });
    };
    $.fn.loadData2 = function (opt) {
        var def = {
            "class": "u-loading",//默认样式
            "url": "",//加载数据请求地址 必需
            "page": 2,//分页加载
            "order": "",//其它参数
            "key": "",//其它参数
            "limit": 10,//分页加载每次加载数量
            "ajaxType": "json",//数据格式
            "funStart": function () {
            },//加载前触发的事件
            "funEnd": function () {
            },//加载结束触发的事件
            "funError": function () {
            } //加载失败触发的事件
        };
        opt = $.extend(def, opt);
        $(this).each(function (index) {
            var $self = $(this),
                $loading = $('<div class="u-loading"><div class="box"><span class="arrow z-load"></span><span class="txt">正在加载...</span></div></div>'),
                $loading_arrow = $loading.find(".arrow").eq(0),
                $loading_txt = $loading.find(".txt").eq(0);
            $self.append($loading.hide());
            var isload = true;
            if (app.isMobile()) {
                $(window).on("touchmove", function () {
                    scroll();
                });
                $(window).on("touchend", function () {
                    if ($(document).height() <= $(window).height() + $(window).scrollTop() && isload) {
                        $loading.show();
                        ajax();
                    }
                });
            } else {
                $(window).on("scroll mousewheel", app.throttle(function (obj) {
                    scroll();
                }, 100, {}));
            }
            var scroll = function () {
                if ($(document).height() <= $(window).height() + $(window).scrollTop() && isload) {
                    $loading.show();
                    if (!app.isMobile()) {
                        ajax();
                    }
                }
            };
            var ajax = function () {
                isload = false;
                opt.funStart();
                var url = opt.url + "?page=" + opt.page;
                if (opt.order != "") {
                    url += "&order=" + opt.order;
                }
                if (opt.key != "") {
                    url += "&key=" + opt.key;
                }
                $.ajax({
                    url: url,
                    dataType: opt.ajaxType,
                    type: 'get',
                    success: function (data) {
                        $loading.hide();
                        opt.funEnd(data);
                        opt.page++;
                        isload = true;
                        if ((opt.ajaxType == "json" && data.length < opt.limit) || (opt.ajaxType == "html" && data == "")) {
                            isload = false;
                        }
                    },
                    error: function (e) {
                        $loading.hide();
                        isload = true;
                        opt.funError();
                    }
                });
            };
        });
    };
})($);

//手机验证
function ISMobile(val) {
    if (val.length != 11 || isNaN(val)) {
        return false;
    }
    var re = /^1(3\d{9})|(5[012356789]\d{8})|(8[012356789]\d{8})|(4[57]\d{8})$/;
    if (!re.test(val)) {
        return false;
    }
    return true;
}