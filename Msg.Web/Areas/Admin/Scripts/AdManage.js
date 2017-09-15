

var fileManage = {
    // 上级目录
    ParentDic: function () {
        var SysPath = $('#sysPath').val();

        SysPath = SysPath.substring(0, SysPath.length - 1);
        var intLastIndex = SysPath.lastIndexOf('\\') + 1;
        SysPath = SysPath.substring(0, intLastIndex);
        window.location.href = "http://www.meisugou.com/admin/admanage/FileManage?path=" + SysPath;
    },
    // 创建目录
    CreateDic: function (obj) {
        $(obj).attr('disabled', true);
        var dicName = $.trim($('#dicName').val());
        if (dicName == '') {
            xkcom.alert('请填写新建文件夹名称！', function () {
                $(obj).attr('disabled', false);
            });
            return;
        }

        $.get('http://www.meisugou.com/admin/admanage/CreateDic?t=' + Math.random(), { dicName: dicName }, function (data) {
            if (!data.isError) {
                xkcom.TimerAlert('创建成功！', function () {
                    location.href = location.href;
                });
            } else {
                xkcom.alert(data.msg, function () {
                    $(obj).attr('disabled', false);
                });
            }
        });
    },
    // 删除目录
    DelDic: function (obj) {
        var dic = $(obj).parent().find('#dic').val();
        xkcom.confirm('\'' + dic + '\'文件夹下面所有文件和文件夹将都被删除！是否确定？', function () {
            $.get('http://www.meisugou.com/admin/admanage/DeleteDic?t=' + Math.random(), { dicName: dic }, function (data) {
                if (!data.isError) {
                    xkcom.TimerAlert('删除成功！', function () {
                        location.href = location.href;
                    });
                } else {
                    xkcom.alert(data.msg);
                }
            });
        });
    },
    // 删除文件
    DelFile: function (fileName) {
        xkcom.confirm('文件删除后将不能恢复！是否继续？', function () {
            $.get('http://www.meisugou.com/admin/admanage/DeleteFile?t=' + Math.random(), { fileName: fileName }, function (data) {
                if (!data.isError) {
                    xkcom.TimerAlert('删除成功！', function () {
                        location.href = location.href;
                    });
                } else {
                    xkcom.alert(data.msg);
                }
            });
        });
    },
    // 复制链接
    CopyUrl: function (url) {
        if (window.clipboardData) {
            window.clipboardData.setData("Text", url);
            if (window.clipboardData.getData("Text") != url) {
                xkcom.alert('浏览器拒绝复制内容，请手动复制。');
                return;
            }
            xkcom.TimerAlert('复制成功！用 Ctrl + V 粘贴。');
        } else {
            xkcom.alert('你的浏览器不支持复制，请手动复制。');
        }
    }
};

    var adManage = {
        // 修改幻灯片
        EditSlide: function (id) {
        if (id) {
            $.get('http://www.meisugou.com/admin/admanage/getslide?t=' + Math.random(), { id: id }, function (data) {
                if (data.slide) {
                    var chtml =
                        '<div>' +
                            '<div class="newjob_tc_3 clearfix">' +
                            '<input type="hidden" id="Id" value="' + data.slide.Id + '" />' +
                            '<div class="newjob_tc_3_1">标题：</div>' +
                            '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Title" maxlength="30" value="' + data.slide.Title + '" type="text" /></div></div></div>' +
                            '<div class="clear"></div>' +
                            '<div class="newjob_tc_3_1">链接：</div>' +
                            '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Url" maxlength="200" value="' + data.slide.Url + '" type="text" /></div></div></div>' +
                            '<div class="clear"></div>' +
                            '<div class="newjob_tc_3_1">图片：</div>' +
                            '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Image" maxlength="200" value="' + data.slide.Image + '" type="text" /></div></div></div>' +
                            '<div class="clear"></div>' +
                            '<div class="newjob_tc_3_1">描述：</div>' +
                            '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Remark" maxlength="30" value="' + data.slide.Remark + '" type="text" /></div></div></div>' +
                            '<div class="clear"></div>' +
                            '<div class="newjob_tc_3_1">排序号：</div>' +
                            '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:130px;" class="newjob_input1" id="OrderNo" maxlength="5" value="' + data.slide.OrderNo + '" type="text" /></div></div></div>' +
                            '<div class="clear"></div>' +
                            '<div class="newjob_tc_3_1"></div>' +
                            '<div class="newjob_tc_3_2 b_btn_10_24" style="margin-top:20px">' +
                            '<a href="javascript:;" id="btn_submit" class="button">修改</a>' +
                            '</div>' +
                            '<div class="clear"></div>' +
                            '</div>' +
                            '</div>';

                    var editSlide = new Boxy(chtml, {
                        title: "修改幻灯片",
                        modal: true,
                        cancel: true,
                        afterShow: function () {
                            $(".button").initButton();

                            $('#btn_submit').bind('click', function () {
                                var title = $('#Title').val();
                                var orderNo = $('#OrderNo').val();
                                var url = $('#Url').val();
                                var image = $('#Image').val();
                                var remark = $('#Remark').val();

                                if (title == '') {
                                    xkcom.alert('请填写标题');
                                    return;
                                }
                                if (orderNo == '') {
                                    xkcom.alert('请填写排序号');
                                    return;
                                }
                                if (isNaN(orderNo)) {
                                    xkcom.alert('排序号只能是数字');
                                    return;
                                }
                                if (url == '') {
                                    xkcom.alert('请填写链接');
                                    return;
                                }
                                if (image == '') {
                                    xkcom.alert('请填写图片地址');
                                    return;
                                }

                                var model = {
                                    Id: $('#Id').val(),
                                    Title: title,
                                    OrderNo: orderNo,
                                    Url: url,
                                    Image: image,
                                    Remark: remark
                                };

                                $.get('/admin/admanage/ModifySlide?t=' + Math.random(), model, function (result) {
                                    if (!result.isError) {
                                        xkcom.TimerAlert('修改成功！', function () {
                                            location.href = location.href;
                                        });
                                    } else {
                                        xkcom.alert(result.msg);
                                    }
                                });
                            });
                        }
                    });
                }
            });
        } else {
            var chtml =
                '<div>' +
                    '<div class="newjob_tc_3 clearfix">' +
                    '<div class="newjob_tc_3_1">标题：</div>' +
                    '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Title" maxlength="30" type="text" /></div></div></div>' +
                    '<div class="clear"></div>' +
                    '<div class="newjob_tc_3_1">链接：</div>' +
                    '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Url" maxlength="200" type="text" /></div></div></div>' +
                    '<div class="clear"></div>' +
                    '<div class="newjob_tc_3_1">图片：</div>' +
                    '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Image" maxlength="200" type="text" /></div></div></div>' +
                    '<div class="clear"></div>' +
                    '<div class="newjob_tc_3_1">描述：</div>' +
                    '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Remark" maxlength="30" type="text" /></div></div></div>' +
                    '<div class="clear"></div>' +
                    '<div class="newjob_tc_3_1">排序号：</div>' +
                    '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:130px;" class="newjob_input1" id="OrderNo" maxlength="5" type="text" /></div></div></div>' +
                    '<div class="clear"></div>' +
                    '<div class="newjob_tc_3_1"></div>' +
                    '<div class="newjob_tc_3_2 b_btn_10_24" style="margin-top:20px">' +
                    '<a href="javascript:;" id="btn_submit" class="button">保存</a>' +
                    '</div>' +
                    '<div class="clear"></div>' +
                    '</div>' +
                    '</div>';

            var addSlide = new Boxy(chtml, {
                title: "添加幻灯片",
                modal: true,
                cancel: true,
                afterShow: function () {
                    $(".button").initButton();

                    $('#btn_submit').bind('click', function () {
                        var title = $('#Title').val();
                        var orderNo = $('#OrderNo').val();
                        var url = $('#Url').val();
                        var image = $('#Image').val();
                        var remark = $('#Remark').val();

                        if (title == '') {
                            xkcom.alert('请填写标题');
                            return;
                        }
                        if (orderNo == '') {
                            xkcom.alert('请填写排序号');
                            return;
                        }
                        if (isNaN(orderNo)) {
                            xkcom.alert('排序号只能是数字');
                            return;
                        }
                        if (url == '') {
                            xkcom.alert('请填写链接');
                            return;
                        }
                        if (image == '') {
                            xkcom.alert('请填写图片地址');
                            return;
                        }

                        var model = {
                            Title: title,
                            OrderNo: orderNo,
                            Url: url,
                            Image: image,
                            Remark: remark
                        };

                        $.get('http://www.meisugou.com/admin/admanage/AddSlide?t=' + Math.random(), model, function (result) {
                            if (!result.isError) {
                                xkcom.TimerAlert('添加成功！', function () {
                                    location.href = location.href;
                                });
                            } else {
                                xkcom.alert(result.msg);
                            }
                        });
                    });
                }
            });
        }
    },
    // 删除幻灯片
    DelSlide: function (id) {
        xkcom.confirm('确定删除该幻灯片吗？', function () {
            $.get('http://www.meisugou.com/admin/admanage/DeleteSlide?t=' + Math.random(), { id: id }, function (data) {
                if (!data.isError) {
                    xkcom.TimerAlert('删除成功！', function () {
                        location.href = location.href;
                    });
                } else {
                    xkcom.alert(data.msg);
                }
            });
        });
    },


    // 修改首页右侧广告
    EditRight: function (title, url, image) {
        var chtml =
                '<div>' +
                    '<div class="newjob_tc_3 clearfix">' +
                    '<div class="newjob_tc_3_1">标题：</div>' +
                    '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Title" value="' + title + '" maxlength="30" type="text" /></div></div></div>' +
                    '<div class="clear"></div>' +
                    '<div class="newjob_tc_3_1">链接：</div>' +
                    '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Url" value="' + url + '" maxlength="200" type="text" /></div></div></div>' +
                    '<div class="clear"></div>' +
                    '<div class="newjob_tc_3_1">图片：</div>' +
                    '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Image" value="' + image + '" maxlength="200" type="text" /></div></div></div>' +
                    '<div class="clear"></div>' +
                    '<div class="newjob_tc_3_1"></div>' +
                    '<div class="newjob_tc_3_2 b_btn_10_24" style="margin-top:20px">' +
                    '<a href="javascript:;" id="btn_submit" class="button">保存</a>' +
                    '</div>' +
                    '<div class="clear"></div>' +
                    '</div>' +
                    '</div>';

        var editRight = new Boxy(chtml, {
            title: "修改广告",
            modal: true,
            cancel: true,
            afterShow: function () {
                $(".button").initButton();

                $('#btn_submit').bind('click', function () {
                    var vTitle = $('#Title').val();
                    var vUrl = $('#Url').val();
                    var vImage = $('#Image').val();

                    if (vTitle == '') {
                        xkcom.alert('请填写标题');
                        return;
                    }
                    if (vUrl == '') {
                        xkcom.alert('请填写链接');
                        return;
                    }
                    if (vImage == '') {
                        xkcom.alert('请填写图片地址');
                        return;
                    }

                    var model = {
                        title: vTitle,
                        url: vUrl,
                        image: vImage
                    };

                    $.get('http://www.meisugou.com/admin/admanage/EditRight?t=' + Math.random(), model, function (result) {
                        if (!result.isError) {
                            xkcom.TimerAlert('修改成功！', function () {
                                location.href = location.href;
                            });
                        } else {
                            xkcom.alert(result.msg);
                        }
                    });
                });
            }
        });
    },


    // 修改底部广告
    EditButtom: function (title, url, image) {
        var chtml =
                '<div>' +
                    '<div class="newjob_tc_3 clearfix">' +
                    '<div class="newjob_tc_3_1">标题：</div>' +
                    '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Title" value="' + title + '" maxlength="30" type="text" /></div></div></div>' +
                    '<div class="clear"></div>' +
                    '<div class="newjob_tc_3_1">链接：</div>' +
                    '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Url" value="' + url + '" maxlength="200" type="text" /></div></div></div>' +
                    '<div class="clear"></div>' +
                    '<div class="newjob_tc_3_1">图片：</div>' +
                    '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Image" value="' + image + '" maxlength="200" type="text" /></div></div></div>' +
                    '<div class="clear"></div>' +
                    '<div class="newjob_tc_3_1"></div>' +
                    '<div class="newjob_tc_3_2 b_btn_10_24" style="margin-top:20px">' +
                    '<a href="javascript:;" id="btn_submit" class="button">保存</a>' +
                    '</div>' +
                    '<div class="clear"></div>' +
                    '</div>' +
                    '</div>';

        var editRight = new Boxy(chtml, {
            title: "修改广告",
            modal: true,
            cancel: true,
            afterShow: function () {
                $(".button").initButton();

                $('#btn_submit').bind('click', function () {
                    var vTitle = $('#Title').val();
                    var vUrl = $('#Url').val();
                    var vImage = $('#Image').val();

                    if (vTitle == '') {
                        xkcom.alert('请填写标题');
                        return;
                    }
                    if (vUrl == '') {
                        xkcom.alert('请填写链接');
                        return;
                    }
                    if (vImage == '') {
                        xkcom.alert('请填写图片地址');
                        return;
                    }

                    var model = {
                        title: vTitle,
                        url: vUrl,
                        image: vImage
                    };

                    $.get('http://www.meisugou.com/admin/admanage/EditButtom?t=' + Math.random(), model, function (result) {
                        if (!result.isError) {
                            xkcom.TimerAlert('修改成功！', function () {
                                location.href = location.href;
                            });
                        } else {
                            xkcom.alert(result.msg);
                        }
                    });
                });
            }
        });
    },


    // 添加找位置右侧广告
    AddJosbRight: function () {
        var chtml =
                '<div>' +
                    '<div class="newjob_tc_3 clearfix">' +
                    '<div class="newjob_tc_3_1">标题：</div>' +
                    '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Title" maxlength="30" type="text" /></div></div></div>' +
                    '<div class="clear"></div>' +
                    '<div class="newjob_tc_3_1">链接：</div>' +
                    '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Url" maxlength="200" type="text" /></div></div></div>' +
                    '<div class="clear"></div>' +
                    '<div class="newjob_tc_3_1">图片：</div>' +
                    '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Image" maxlength="200" type="text" /></div></div></div>' +
                    '<div class="clear"></div>' +
                    '<div class="newjob_tc_3_1">排序号：</div>' +
                    '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="OrderNo" maxlength="5" type="text" /></div></div></div>' +
                    '<div class="clear"></div>' +
                    '<div class="newjob_tc_3_1"></div>' +
                    '<div class="newjob_tc_3_2 b_btn_10_24" style="margin-top:20px">' +
                    '<a href="javascript:;" id="btn_submit" class="button">保存</a>' +
                    '</div>' +
                    '<div class="clear"></div>' +
                    '</div>' +
                    '</div>';

        var addRight = new Boxy(chtml, {
            title: "添加广告",
            modal: true,
            cancel: true,
            afterShow: function () {
                $(".button").initButton();

                $('#btn_submit').bind('click', function () {
                    var vTitle = $('#Title').val();
                    var vUrl = $('#Url').val();
                    var vImage = $('#Image').val();
                    var vOrderNo = $('#OrderNo').val();

                    if (vTitle == '') {
                        xkcom.alert('请填写标题');
                        return;
                    }
                    if (vUrl == '') {
                        xkcom.alert('请填写链接');
                        return;
                    }
                    if (vImage == '') {
                        xkcom.alert('请填写图片地址');
                        return;
                    }
                    if (vOrderNo == '') {
                        xkcom.alert('请填写排序号');
                        return;
                    }
                    if (isNaN(vOrderNo)) {
                        xkcom.alert('排序号只能是数字');
                        return;
                    }

                    var model = {
                        title: vTitle,
                        url: vUrl,
                        image: vImage,
                        orderNo: vOrderNo
                    };

                    $.get('http://www.meisugou.com/admin/admanage/AddJobsRight?t=' + Math.random(), model, function (result) {
                        if (!result.isError) {
                            xkcom.TimerAlert('添加成功！', function () {
                                location.href = location.href;
                            });
                        } else {
                            xkcom.alert(result.msg);
                        }
                    });
                });
            }
        });
    },
    // 修改找位置右侧广告
    EditJobsRight: function (id, title, orderNo, url, image) {
        var chtml =
            '<div>' +
                '<div class="newjob_tc_3 clearfix">' +
                '<div class="newjob_tc_3_1">标题：</div>' +
                '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Title" value="' + title + '" maxlength="30" type="text" /></div></div></div>' +
                '<div class="clear"></div>' +
                '<div class="newjob_tc_3_1">链接：</div>' +
                '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Url" value="' + url + '" maxlength="200" type="text" /></div></div></div>' +
                '<div class="clear"></div>' +
                '<div class="newjob_tc_3_1">图片：</div>' +
                '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="Image" value="' + image + '" maxlength="200" type="text" /></div></div></div>' +
                '<div class="clear"></div>' +
                '<div class="newjob_tc_3_1">图片：</div>' +
                '<div class="newjob_tc_3_2"><div class="newjob_input"><div class="newjob_input_right"><input style="width:430px;" class="newjob_input1" id="OrderNo" value="' + orderNo + '" maxlength="5" type="text" /></div></div></div>' +
                '<div class="clear"></div>' +
                '<div class="newjob_tc_3_1"></div>' +
                '<div class="newjob_tc_3_2 b_btn_10_24" style="margin-top:20px">' +
                '<a href="javascript:;" id="btn_submit" class="button">保存</a>' +
                '</div>' +
                '<div class="clear"></div>' +
                '</div>' +
            '</div>';

        var editRight = new Boxy(chtml, {
            title: "修改广告",
            modal: true,
            cancel: true,
            afterShow: function () {
                $(".button").initButton();

                $('#btn_submit').bind('click', function () {
                    var vTitle = $('#Title').val();
                    var vUrl = $('#Url').val();
                    var vImage = $('#Image').val();
                    var vOrderNo = $('#OrderNo').val();

                    if (vTitle == '') {
                        xkcom.alert('请填写标题');
                        return;
                    }
                    if (vUrl == '') {
                        xkcom.alert('请填写链接');
                        return;
                    }
                    if (vImage == '') {
                        xkcom.alert('请填写图片地址');
                        return;
                    }
                    if (vOrderNo == '') {
                        xkcom.alert('请填写排序号');
                        return;
                    }
                    if (isNaN(vOrderNo)) {
                        xkcom.alert('排序号只能是数字');
                        return;
                    }

                    var model = {
                        id: id,
                        title: vTitle,
                        url: vUrl,
                        image: vImage,
                        orderNo: vOrderNo
                    };

                    $.get('http://www.meisugou.com/admin/admanage/ModifyJobsRight?t=' + Math.random(), model, function (result) {
                        if (!result.isError) {
                            xkcom.TimerAlert('修改成功！', function () {
                                location.href = location.href;
                            });
                        } else {
                            xkcom.alert(result.msg);
                        }
                    });
                });
            }
        });
    },
    // 删除找位置右侧广告
    DelJobsRight: function (id) {
        xkcom.confirm('确定删除该广告吗？', function () {
            $.get('http://www.meisugou.com/admin/admanage/DeleteJobsRight?t=' + Math.random(), { id: id }, function (data) {
                if (!data.isError) {
                    xkcom.TimerAlert('删除成功！', function () {
                        location.href = location.href;
                    });
                } else {
                    xkcom.alert(data.msg);
                }
            });
        });
    },
        
    EditImage: function (FileName) {
        //var title = "";
        //var orderNo = $('#' + FileName + '_OrderNo').val();
        //var url = $('#' + FileName + '_Url').val();
        //var image = $('#' + FileName + '_Image').val();
        //var remark = $('#' + FileName + '_Remark').val();
        

        //elementModel.SetAttribute("Title", model.Title);
        //elementModel.SetAttribute("Url", model.Url);
        //elementModel.SetAttribute("Image", model.Image);
        //elementModel.SetAttribute("Remark", model.Remark);
        //elementModel.SetAttribute("OrderNo", model.OrderNo.ToString());
        //elementModel.SetAttribute("AdSiteName", model.AdSiteName);
        var model = {
            //id: id,
            title: $('#'+FileName+'_Title').val(),
            url: $('#' + FileName + '_Url').val(),
            image: $('#' + FileName + '_Image').val(),
            orderNo: $('#' + FileName + '_OerderNo').val(),
            adsitename: $('#' + FileName + "_AdSiteName"),
            remark:$('#'+FileName+'_Remark'),
            filename:FileName,
        };


        $.get('~/Admin/Controllers/AdManage?t=' + Math.random(), model, function (result) {

            if (!result.isError) {
                alert('修改成功！', function () {
                    location.href = location.href;
                });
            } else {
               alert(result.msg);
            }
        }); //F:\Vsworkspace\Projects\Meisugou\Msg.Web\Areas/Admin/Controllers/AdManageController.cs
    },
        
    EditSlider: function (Filename) {
        
        }
    };