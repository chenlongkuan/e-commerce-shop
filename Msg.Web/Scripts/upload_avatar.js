function SEXY_upload_one_file() {
    if (arguments[0]) {
        this.instance = arguments[0];
    }
    if (arguments[1]) {
        this.temp_options = arguments[1];
    }

    //初始化flash调用函数
    this.error = function (str) { }
    this.start = function () { }
    this.progress = function (total, now) { }
    this.complete = function (str) { }
    this.debug = function (str) { }

    // 初始化默认参数
    this.swf_name = 'SEXY_upload_avatar_' + Math.random();
    this.swf_src = '/flash_script/upload_one_image.swf';
    this.swf_width = 690;
    this.swf_height = 445;
    this.save_url = '/home/SaveImageFileUpload';
    this.swf_obj = '';
    this.select_btn_src = '/content/images/upload_avatar/select_big.jpg';
    this.select_btn_w = 150;
    this.select_btn_h = 45;
    this.save_btn_src = '/content/images/upload_avatar/save_btn.jpg';
    this.save_btn_w = 106;
    this.save_btn_h = 33;
    this.reset_select_btn_src = '/content/images/upload_avatar/reset_select_btn.jpg';
    this.reset_select_btn_w = 106;
    this.reset_select_btn_h = 33;



    this.loaded = function () {
        if (navigator.appName.indexOf("Microsoft") != -1) {
            if (window[this.swf_name]) {
                this.swf_obj = window[this.swf_name];
            } else {
                this.swf_obj = document.forms[0][this.swf_name];
            }
        } else {
            this.swf_obj = document[this.swf_name];
        }

        this.set_options = function (opt) {
            if (opt.up_url) {
                opt.up_url = opt.up_url.replace(/&/gim, '$amp;');
            }
            this.swf_obj.set_options(opt);
        }
        this.btn_disable = function () {
            this.swf_obj.btn_disable();
        }
        this.btn_enable = function () {
            this.swf_obj.btn_enable();
        }
        this.btn_reset = function () {
            this.swf_obj.btn_reset();
        }
    }


    this.init = function () {
        $.extend(this, this.temp_options);

        var instance = this.instance;
        var swf_src = this.swf_src;
        var swf_name = this.swf_name;
        var swf_width = this.swf_width;
        var swf_height = this.swf_height;
        var save_url = this.save_url;
        var select_btn_src = this.select_btn_src;
        var select_btn_w = this.select_btn_w;
        var select_btn_h = this.select_btn_h;
        var save_btn_src = this.save_btn_src;
        var save_btn_w = this.save_btn_w;
        var save_btn_h = this.save_btn_h;
        var reset_select_btn_src = this.reset_select_btn_src;
        var reset_select_btn_w = this.reset_select_btn_w;
        var reset_select_btn_h = this.reset_select_btn_h;

        save_url = save_url.replace(/&/gim, '$amp;');

        this.flashMovie = $('#' + this.id);
        this.flashMovie.flash({
            swf: swf_src + '?rnd=' + Math.random(), width: swf_width, height: swf_height, name: swf_name, id: swf_name, menu: false, wmode: 'transparent', allowScriptAccess: 'sameDomain',
            flashvars: {
                instance: instance,
                swf_height: swf_height,
                swf_width: swf_width,
                save_url: save_url,
                select_btn_src: select_btn_src,
                select_btn_w: select_btn_w,
                select_btn_h: select_btn_h,
                save_btn_src: save_btn_src,
                save_btn_w: save_btn_w,
                save_btn_h: save_btn_h,
                reset_select_btn_src: reset_select_btn_src,
                reset_select_btn_w: reset_select_btn_w,
                reset_select_btn_h: reset_select_btn_h

            }
        });
    };

    this.init();
};