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
    this.complete = function (str) {  }
    this.debug = function (str) { }

    // 初始化默认参数
    this.swf_name = 'SEXY_upload_one_' + Math.random();
    this.swf_src = 'upload_one_file.swf';
    this.btn_w = 120;
    this.btn_h = 26;
    this.btn_src = 'swf_btn.png';
    this.up_url = 'upload_one.php';
    this.file_type_name = 'Web Images (*.jpg, *.jpeg, *.gif, *.png, *.bmp)';
    this.file_type = '*.jpg; *.jpeg; *.gif; *.png; *.bmp;';
    this.file_max = 10000;
    this.file_min = 1;
    this.loading = 'true';
    this.img_wh = 'false';
    this.swf_obj = '';

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
        var btn_src = this.btn_src;
        var btn_w = this.btn_w;
        var btn_h = this.btn_h;
        var up_url = this.up_url;
        var file_type_name = this.file_type_name;
        var file_type = this.file_type;
        var file_max = this.file_max;
        var file_min = this.file_min;
        var loading = this.loading;
        var img_wh = this.img_wh;

        up_url = up_url.replace(/&/gim, '$amp;');

        this.flashMovie = $('#' + this.id);
        this.flashMovie.flash({
            swf: swf_src + '?rnd=' + Math.random(), width: btn_w, height: btn_h, name: swf_name, id: swf_name, menu: false, wmode: 'transparent', allowScriptAccess: 'sameDomain',
            flashvars: {
                instance: instance,
                btn_src: btn_src,
                btn_w: btn_w,
                btn_h: btn_h,
                up_url: up_url,
                file_type_name: file_type_name,
                file_type: file_type,
                file_max: file_max,
                file_min: file_min,
                loading: loading,
                img_wh: img_wh
            }
        });
    };

    this.init();
};