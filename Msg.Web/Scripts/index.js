$(function () {
    //// load market data
    $("#div_index_market").showLoading();
    $.get("/home/IndexAjaxData", { further: "market" }, function (data) {
        var html = '<ul class="clearfix">';
        $(data.esdata).each(function () {
            html += '<li>' +
                '<a href="/details/' + this.Id + '" target="_blank"><img  class="lazy"  data-original="' + this.Logo + '" /></a>' +
                '<dl class="clearfix">' +
                '<dd>￥' + this.SellPrice + '</dd>' +
                '<dt>已售出 ' + this.SoldCount + '</dt>' +
                '</dl>' +
                '<div class="li1"><a href="/details/' + this.Id + '" target="_blank">' + this.ShortTitle + '</a></div>' +
                '<div class="li2">' + this.SchoolName + '-' + this.UserName + '</div>' +
                '</li>';
        });
        html += '</ul>';
        $("#div_index_market").html(html);
        $("#div_index_market").hideLoading();
    });
    LoadExchangeGoods(0,1000,1);

});

//load credit goods
function LoadExchangeGoods(cbegin, cend, navId) {

    var content = $("#div_index_exchange_content_" + navId).html();
    if (!isEmpty(content)) {
        return;
    }
    $("#div_index_exchange_warpper").showLoading();
    $.get("/home/IndexAjaxData", { further: "credits", cbegin: cbegin, cend: cend }, function (data) {
        var html = '<ul class="clearfix">';
        $(data.credits).each(function () {
            html += '<li>' +
                      '<a href="/exchanges/item/' + this.Id + '" target="_blank"><img  class="lazy"  src="' + this.Logo + '" /></a>' +
                        '<div class="li1">' +
                        '<a href="/exchanges/item/' + this.Id + '" target="_blank">' + this.Name + '</a>' +
                        '<b>' + this.NeedCredits + '</b>&nbsp;积分' +
                        '</div>' +
                        '</li>';
        });
        html += '</ul>';
        $("#div_index_exchange_content_" + navId).html(html);
        $("#div_index_exchange_warpper").hideLoading();
    });
}