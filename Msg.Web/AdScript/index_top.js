//document.write('<div class="add_xunku" id="index_buttom_ad"></div>');

$(function () {

   // var width = 1440, height = 75;

    $.ajax({
        url: "/AdScript/index_top.xml",
        dataType: 'xml',
        type: 'GET',
        async: false,
        success: function (xml) {
            var obj = $(xml).find("Item").first();
            var image = $(obj).attr('Image');
            var a = $("<a></a>").attr('target', '_blank')
                .attr('title', $(obj).attr('Title'))
               .attr('href', $(obj).attr('Url')).attr('style', "background:url('" + image + "') no-repeat top center;");
            // var img = $('<img />').attr('src', $(obj).attr('Image'));//.attr('height', height).attr('width', width);
           // var a = "<a style='background:url('" + $(obj).attr('Image') + ")' target='_blank' title="+$(obj).attr('Title')+"  ></a>";
            //a.append(img);

            $('.header_gg').append(a);
        }
    });
});
