//<div class="add_xunku">
//    <a target="_blank" href="#">
//        <img src="/Content/themes/images/shouy_29.jpg"></a>
//</div>
//document.write('<div class="add_xunku" id="index_buttom_ad"></div>');

$(function () {

    //var width = 1200, height = 166;

    $.ajax({
        url: "/AdScript/index_buttom.xml",
        dataType: 'xml',
        type: 'GET',
        async: false,
        success: function (xml) {
            var obj = $(xml).find("Item").first();

            var a = $('<a></a>').attr('target', '_blank')
                .attr('title', $(obj).attr('Title'))
                .attr('href', $(obj).attr('Url'));

            var img = $('<img />').attr('src', $(obj).attr('Image'));//.attr('height', height).attr('width', width);
            a.append(img);

            $('.index_gg1').append(a);
        }
    });
});
