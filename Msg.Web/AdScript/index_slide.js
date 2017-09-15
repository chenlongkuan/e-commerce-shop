//




var bannerHtml = "";



//var title_arr = new Array();
//var link_arr = new Array();
//var image_arr = new Array();
//var remark_arr = new Array();

var slidersArr = new Array();


//var title = $('#Title_' + fileName + "-0").text();
//var url = $('#Url_' + fileName + "-0").text();
//var image = $('#Image_' + fileName + "-0").attr("src");
//var orderNo = $('#OrderNo_' + fileName + "-0").text();
//var adsitename = $('#AdSiteName_' + fileName + "-0").text();
//var remark = $('#Remark_' + fileName + "-0").text();

$.ajax({
    url: "/AdScript/index_slide.xml",
    dataType: 'xml',
    type: 'GET',
    async: false,
    success: function (xml) {
        $(xml).find("Item").each(function () {
            //title_arr.push($(this).attr('Title'));
            //link_arr.push($(this).attr('Url'));
            //image_arr.push($(this).attr('Image'));
            //remark_arr.push($(this).attr('Remark'));

            var sliderJson = {
                Title: $(this).attr('Title'), Url: $(this).attr("Url"), Image: $(this).attr("Image"), Remark: $(this).attr("Remark"), OrderNo: $(this).attr("OrderNo")
            };
            slidersArr.push(sliderJson);

        });

        slidersArr.sort(function (a, b) {
            return a.OrderNo - b.OrderNo;
        });        



        //var a = $('<a></a>').attr('target', '_blank')
        //      .attr('title', $(obj).attr('Title'))
        //      .attr('href', $(obj).attr('Url'));



        //for (var i = 0; i < slidersArr.length; i++) {
        //    var a = $("<a></a>").attr('herf', slidersArr[i].Url).attr('class', 'd1').attr('style', 'background:url(' + slidersArr[i].Image + ') center no-repeat;');
        //    $("#banner").append(a);
        //}


        for (var i = 0; i < slidersArr.length; i++) {
            bannerHtml += "<a href='" + slidersArr[i].Url + "' class='d1' style='background:url(" + slidersArr[i].Image + ") center no-repeat;'></a>";
        }
        bannerHtml += "    <div class='d2' id='banner_id'><ul>";
        for (var i = 0; i < slidersArr.length; i++) {
            bannerHtml += "<li></li>";
        }
        bannerHtml += "</ul></div>";
        $("#banner").html(bannerHtml);
        


    }
});