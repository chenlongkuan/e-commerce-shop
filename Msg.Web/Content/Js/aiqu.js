// JavaScript Document




$(document).ready(function(){
	//首页 我的小美
	
	$(".sea_w").hover(function(){
		$(".sea_xl").show();
	},function(){
		$(".sea_xl").hide();	
	});
	
	$(".msg_nav_right ul li").hover(function(){
		$(this).css({"width":"auto"},300);
	},function(){
		$(".msg_nav_right ul li").css({"width":"28px"},300);	
	});
	
	//1F
	
	$(".nk_right ul li").hover(function(){
		$(this).find(".li1").animate({"bottom":"0px"},200);
	},function(){
		$(".nk_right ul li .li1").animate({"bottom":"-70px"},100);
	});	
	
	$(".sh_right ul li .li_q").hover(function(){
		$(this).find(".li4").animate({"bottom":"0px"},200);
	},function(){
		$(".sh_right ul li .li_q .li4").animate({"bottom":"-70px"},100);
	});	
	
	
	$(".nk_left ul li").each(function(i) {
        $(this).attr("nk_left",i);
    });
	$(".nk_left ul li").hover(function() {
		$(".nk_left ul li a").removeClass("over");
		$(this).find("a").addClass("over");
		$(".nk_qh").hide();
		$(".nk_qh").eq($(this).attr("nk_left")).show();
	});
	
	$(".ls_left ul li").each(function(i) {
        $(this).attr("ls_left",i);
    });
	
	$(".ls_left ul li").hover(function () {
		$(".ls_left ul li").removeClass("over");
		$(this).addClass("over");
		$(".ls_qh").hide();
		$(".ls_qh").eq($(this).attr("ls_left")).show();
	});
	
	$(".sh_left ul li").each(function(i) {
        $(this).attr("sh_left",i);
    });
	
	$(".sh_left ul li").hover(function () {
		$(".sh_left ul li").removeClass("over");
		$(this).addClass("over");
		$(".sh_right").hide();
		$(".sh_right").eq($(this).attr("sh_left")).show();
	});
	
	
	//积分兑换
	$(".jf_left a").each(function(i) {
        $(this).attr("jf_left",i);
    });
	
	$(".jf_left a").hover(function () {
		$(".jf_left a").removeClass("over");
		$(this).addClass("over");
		$(".jf_right").hide();
		$(".jf_right").eq($(this).attr("jf_left")).show();
	});
	
	
	
	//小美列表页切换
	$(".nk_list_li ul li").each(function(i) {
        $(this).attr("nk_list_li",i);
    });
	$(".nk_list_li ul li").bind("click",function(){
		$(".nk_list_li ul li").removeClass("over");
		$(this).addClass("over");
		$(".nk_list_li ul li .li1").removeClass("over");
		$(this).find(".li1").addClass("over");
		$(".list_main").hide();
		$(".list_main").eq($(this).attr("nk_list_li")).show();
	});
	
	$(".ls_list_li ul li").each(function(i) {
        $(this).attr("ls_list_li",i);
    });
	$(".ls_list_li ul li").bind("click",function(){
		$(".ls_list_li ul li").removeClass("over");
		$(this).addClass("over");
		$(".ls_list_li ul li .li1").removeClass("over");
		$(this).find(".li1").addClass("over");
		$(".list_main").hide();
		$(".list_main").eq($(this).attr("ls_list_li")).show();
	});
	
	
	//个人中心
	
	//替换头像
	$(".info_t_img").hover(function(){
	    $(".info_t_img .img").show();
	},function(){
		$(".info_t_img .img").hide();
	});
	
	//站内信
	$(".station_l .st_show").each(function(i) {
        $(this).attr("st_show",i);
    });
	$(".station_l .st_show").click(function(){
		$(".st_hide").hide();
		$(".st_hide").eq($(this).attr("st_show")).show();
	});
	
	
	//购物车
	$(".shop_li li").each(function(i) {
        $(this).attr("shop_li",i);
    });
	$(".shop_li li").click(function(){
		$(".shop_li li").removeClass("over");
		$(this).addClass("over");
		$(".shop_dp").hide();
		$(".shop_dp").eq($(this).attr("shop_li")).show();
	});
	
	
	//右边导航
	$(".dh_1").hover(function(){
		$(this).find(".dh_1_p").animate({
										  left:'-90px',
										  opacity:'1'
										});
	},function(){
		$(this).find(".dh_1_p").animate({
										  left:'-130px',
										  opacity:'0'
										});
	});
	
	//当滚动条的位置处于距顶部100像素以下时，跳转链接出现，否则消失  
	$(function () {   
		//当点击跳转链接后，回到页面顶部位置  
		$(".top").click(function(){  
			$('body,html').animate({scrollTop:0},500);  
			return false;  
		});  
	}); 
	
	
});

//首页大图切换
function banner(){	
	var bn_id = 0;
	var bn_id2= 1;
	var speed33=5000;
	var qhjg = 1;
    var MyMar33;
	$("#banner .d1").hide();
	$("#banner .d1").eq(0).fadeIn("slow");
	if($("#banner .d1").length>1)
	{
		$("#banner_id li").eq(0).addClass("nuw");
		function Marquee33(){
			bn_id2 = bn_id+1;
			if(bn_id2>$("#banner .d1").length-1)
			{
				bn_id2 = 0;
			}
			$("#banner .d1").eq(bn_id).css("z-index","2");
			$("#banner .d1").eq(bn_id2).css("z-index","1");
			$("#banner .d1").eq(bn_id2).show();
			$("#banner .d1").eq(bn_id).fadeOut("slow");
			$("#banner_id li").removeClass("nuw");
			$("#banner_id li").eq(bn_id2).addClass("nuw");
			bn_id=bn_id2;
		};
	
		MyMar33=setInterval(Marquee33,speed33);
		
		$("#banner_id li").click(function(){
			var bn_id3 = $("#banner_id li").index(this);
			if(bn_id3!=bn_id&&qhjg==1)
			{
				qhjg = 0;
				$("#banner .d1").eq(bn_id).css("z-index","2");
				$("#banner .d1").eq(bn_id3).css("z-index","1");
				$("#banner .d1").eq(bn_id3).show();
				$("#banner .d1").eq(bn_id).fadeOut("slow",function(){qhjg = 1;});
				$("#banner_id li").removeClass("nuw");
				$("#banner_id li").eq(bn_id3).addClass("nuw");
				bn_id=bn_id3;
			}
		})
		$("#banner_id").hover(
			function(){
				clearInterval(MyMar33);
			}
			,
			function(){
				MyMar33=setInterval(Marquee33,speed33);
			}
		)	
	}
	else
	{
		$("#banner_id").hide();
	}
}





// 设置为主页
function SetHome(obj, vrl) {
    try {
        obj.style.behavior = 'url(#default#homepage)'; obj.setHomePage(vrl);
    }
    catch (e) {
        if (window.netscape) {
            try {
                netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect");
            }
            catch (e) {
                alert("此操作被浏览器拒绝！\n请在浏览器地址栏输入“about:config”并回车\n然后将 [signed.applets.codebase_principal_support]的值设置为'true',双击即可。");
            }
            var prefs = Components.classes['@@mozilla.org/preferences-service;1'].getService(Components.interfaces.nsIPrefBranch);
            prefs.setCharPref('browser.startup.homepage', vrl);
        } else {
            alert("您的浏览器不支持，请按照下面步骤操作：1.打开浏览器设置。2.点击设置网页。3.输入：" + vrl + "点击确定。");
        }
    }
}
// 加入收藏 兼容360和IE6
function shoucang() {
    try {
        window.external.addFavorite("http://www.meisugou.com", "美速购");
    }
    catch (e) {
        try {
            window.sidebar.addPanel("美速购", "http://www.meisugou.com", "");
        }
        catch (e) {
            alert("加入收藏失败，请使用Ctrl+D进行添加");
        }
    }
}



