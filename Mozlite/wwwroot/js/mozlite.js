(function ($) {
    var debug = true;
    window.IE = !!window.ActiveXObject || 'ActiveXObject' in window;
    window.Moblie = /(iPhone|iPod|Android|ios|SymbianOS)/i.test(navigator.userAgent);
    window['mozlite-ready-functions'] = [];
    window.$onready = function (func) {
        ///<summary>当前文档或弹窗完成时候执行的方法。</summary>
        ///<param name="func" type="Function">方法。</param>
        window['mozlite-ready-functions'].push(func);
    };
    $.fn.checkedVal = function () {
        var values = [];
        this.find('input[type=checkbox], input[type=radio]').each(function () {
            if (this.checked)
                values.push(this.value);
        });
        return values.join(',');
    };
    $.fn.dset = function (key, func) {
        ///<summary>获取对象的缓存数据。</summary>
        ///<param name="key" type="String">缓存键。</param>
        ///<param name="func" type="Function">如果数据不存在返回的数据值函数。</param>
        ///<returns type="String">返回当前存储的值。</returns>
        var value = this.data(key);
        if (value) {
            return this.data(key);
        }
        value = func();
        this.data(key, value);
        return value;
    };
    $.fn.exec = function (func) {
        ///<summary>如果当前选择器的元素存在[可以使用no-js样式忽略]，则执行func方法，并将当前元素对象作为参数。</summary>
        ///<param name="func" type="Function">执行方法。</param>
        if (this.length > 0) {
            return this.each(function () {
                var current = $(this);
                if (!current.hasClass('no-js')) {
                    func(current);
                }
            });
        }
        return this;
    };
    $.fn.createObjectURL = function () {
        ///<summary>预览文件地址，当前元素必须为input[type=file]。</summary>
        if (!this.is('input') || this.attr('type') !== 'file')
            return null;
        if (navigator.userAgent.indexOf("MSIE") > 0) return this.val();
        if (window.createObjectURL) return window.createObjectURL(this[0].files[0]);
        if (window.URL) return window.URL.createObjectURL(this[0].files[0]);
        if (window.webkitURL) return window.webkitURL.createObjectURL(this[0].files[0]);
        return null;
    };
    $.fn.js = function (name, value) {
        ///<summary>获取或设置js-开头的属性。</summary>
        ///<param name="name" type="String">属性名称。</param>
        ///<param name="value" type="Object">属性值。</param>
        if (value) return this.attr('js-' + name, value);
        return this.attr('js-' + name);
    };
    $.fn.targetElement = function (def) {
        ///<summary>返回当前元素内js-target属性指示的元素对象，如果不存在就为当前实例对象。</summary>
        ///<param name="def" type="Object">没找到对象默认对象，没设置表示当前对象。</param>
        var target = $(this.js('target'));
        if (target.length > 0)
            return target;
        return def || this;
    };
    $.fn.formSubmit = function (success, error) {
        ///<summary>提交表单。</summary>
        var form = this;
        var data = new FormData(this[0]);
        var submit = form.find('[js-submit=true],[type=submit]').attr('disabled', 'disabled');
        var icon = submit.find('i.fa');
        var css = icon.attr('class');
        icon.attr('class', 'fa fa-spinner fa-spin');
        $.ajax({
            type: "POST",
            url: form.attr('action'),
            contentType: false,
            processData: false,
            data: data,
            success: function (d) {
                submit.removeAttr('disabled').find('i.fa').attr('class', css);
                if (success) {
                    success(d, form);
                    return;
                }
                if (d.message) {
                    $alert(d.message, d.type, function () {
                        if (d.data && d.data.url)
                            location.href = d.data.url;
                        else if (d.type === 'success')
                            location.href = location.href;
                    });
                }
            },
            error: function (e) {
                submit.removeAttr('disabled').find('i.fa').attr('class', css);
                if (e.status === 401) {
                    $alert('需要登入才能够执行此操作！<a onclick="location.href = \'/login\';" href="javascript:;">点击登入...</a>', 'warning');
                    return;
                }
                else if (error) error(e);
                else if (debug) document.write(e.responseText);
                else $alert('很抱歉，发生了错误！');
            }
        });
    };
    $.fn.loadModal = function (url) {
        ///<summary>显示当前地址的Modal模式窗口。</summary>
        ///<param name="url" type="string">URL地址。</param>
        var s = this;
        var current = s.dset('js-modal', function () {
            return $('<div class="js-modal modal fade" data-backdrop="static"><div>')
                .appendTo(document.body)
                .data('target', s.targetElement());
        });
        current.load(url,
            function () {
                var form = current.find('form');
                if (form.length > 0) {
                    if (!form.attr('action'))
                        form.attr('action', url);
                    if (form.find('input[type=file]').length > 0)
                        form.attr('enctype', 'multipart/form-data');
                    current.find('[js-submit=true]').click(function () {
                        form.formSubmit(function (d, form) {
                            var func = s.js('submit');
                            if (func) {
                                $call(s.js('submit'), d, form);
                                return;
                            }
                            if (d.message) {
                                var errmsg = current.find('div.modal-alert');
                                if (errmsg.length > 0 && d.type !== 'success') {
                                    errmsg.attr('class', 'modal-alert text-' + d.type).show().find('.errmsg').html(d.message);
                                    return;
                                }
                                $alert(d.message, d.type, function () {
                                    if (d.data && d.data.url)
                                        location.href = d.data.url;
                                    else if (d.type === 'success')
                                        location.href = location.href;
                                });
                                if (d.type === 'success')
                                    current.data('bs.modal').hide();
                            }
                            else if (d.data && d.data.url)
                                location.href = d.data.url;
                            else if (d.type === 'success')
                                location.href = location.href;
                        });
                    });
                }
                $readyExec(current);
            }).modal();
    };
    window.$call = function (name) {
        ///<summary>执行方法。</summary>
        ///<param name="name" type="String">方法名称。</param>
        var func = window;
        name = name.split('.');
        for (var i in name) {
            func = func[name[i]];
        }
        if (typeof func === 'function') {
            var args = [];
            for (var j = 1; j < arguments.length; j++) {
                args.push(arguments[j]);
            }
            return func.apply(null, args);
        }
        return null;
    };
    window.$alert = function (message, type, func) {
        ///<summary>显示警告消息。</summary>
        if (typeof message === 'object' && message) {
            type = message.type;
            message = message.message;
        }
        if (!message) return;
        var modal = $(document.body)
            .dset('js-alert',
            function () {
                return $('<div class="js-alert modal fade" data-backdrop="static"><div class="modal-dialog"><div class="modal-content"><div class="modal-body" style="padding: 50px 30px 30px;"><div class="col-sm-2"><i style="font-size: 50px;"></i></div> <span class="col-sm-10" style="line-height: 26px; padding-left: 0;"></span></div><div class="modal-footer"><button type="button" class="btn btn-primary"><i class="fa fa-check"></i> 确定</button></div></div></div></div>')
                    .appendTo(document.body);
            });
        var body = modal.find('.modal-body');
        type = type || 'warning';
        if (type === 'success')
            body.attr('class', 'modal-body row text-success').find('i').attr('class', 'fa fa-check');
        else
            body.attr('class', 'modal-body row text-' + type).find('i').attr('class', 'fa fa-warning');
        body.find('span').html(message);
        var button = modal.find('button').attr('class', 'btn btn-' + type);
        if (func) {
            button.removeAttr('data-dismiss').bind('click', function () {
                func(modal.data('bs.modal'));
                modal.data('bs.modal').hide();
            });
        }
        else button.attr('data-dismiss', 'modal').unbind('click');
        modal.modal('show');
    };
    window.$ajax = function (url, data, success, error) {
        $('#js-loading').fadeIn();
        $.ajax({
            url: url,
            data: data,
            dataType: 'JSON',
            type: 'POST',
            success: function (d) {
                $('#js-loading').fadeOut();
                var callback = d.data && success;
                if (d.message && d.type)
                    $alert(d.message, d.type, d.type === 'success' && !callback);
                if (callback)
                    success(d.data);
            },
            error: function (resp) {
                $('#js-loading').fadeOut();
                if (resp.status === 401) {
                    $alert('需要登陆才能够执行此操作！<a href="/login">点击登陆...</a>', 'warning');
                    return;
                }
                if (error) error(resp);
                else document.write(resp.responseText);
            }
        });
    };
    window.$readyExec = function (context) {
        function exec(name, func) {
            var selector = $('[' + name + ']', context);
            if (selector.length > 0) {
                selector.each(function () {
                    var current = $(this);
                    var value = current.attr(name);
                    if (!current.hasClass('no-js') && value !== 'no-js') {
                        func(current, value);
                    }
                });
            }
        };
        //href
        exec('_href',
            function (s, v) {
                s.css('cursor', 'pointer')
                    .click(function () {
                        var target = s.attr('target') || s.js('target');
                        if (target)
                            window.open(v, target);
                        else
                            location.href = v;
                    });
            });
        //hover
        exec('_hover',
            function (s, v) {
                s.mouseenter(function (e) {
                    var $this = $(this);
                    var target = $this.target();
                    target.addClass(v);
                    target.mouseleave(function () {
                        target.removeClass(v);
                    }).click(function (ev) {
                        ev.stopPropagation();
                    });
                    $(document).one("click", function () {
                        target.removeClass(v);
                    });
                    e.stopPropagation();
                });
            });
        //focus
        exec('_focus',
            function (s, v) {
                s.focusin(function () {
                    $(this).targetElement().toggleClass(v);
                }).focusout(function () {
                    $(this).targetElement().toggleClass(v);
                });
            });
        //click
        exec('_click',
            function (s, v) {
                s.click(function () {
                    $(this).jsTarget().toggleClass(v);
                });
            });
        //modal
        exec('_modal', function (s, v) {
            s.on('click', function () {
                s.loadModal(s.js('url') || s.attr('href') || v);
                return false;
            });
        });
        //action
        exec('_action', function (s, v) {
            s.on('click',
                function () {
                    var confirmStr = s.js('confirm');
                    if (confirmStr && !confirm(confirmStr))
                        return;
                    var error = s.js('error');
                    if (error)
                        error = function (d) { $call(s.js('error'), s, d); }
                    $ajax(v, { id: s.js('value') }, function (d) {
                        var success = s.js('success');
                        if (success)
                            $call(success, s, d);
                        else
                            location.href = location.origin + location.pathname;
                    }, error);
                });
        });
        //maxlength
        exec('_maxlength',
            function (s, v) {
                v = parseInt(v);
                if (isNaN(v))
                    return;
                s.keyup(function () {
                    var length = s.val().length;
                    if (length > v) {
                        s.val(s.val().substr(0, v));
                        length = v;
                    }
                    s.targetElement().html(length + ' 个字符');
                });
            });
        window['mozlite-ready-functions'].forEach(function (func) {
            func(context);
        });
    };
    Date.prototype.toFormatString = function (fmt) {
        ///<summary>格式化日期字符串。</summary>
        ///<param name="fmt" type="String">格式化字符串：yyyy-MM-dd HH:mm:ss</param>
        var o = {
            "M+": this.getMonth() + 1, //月份 
            "d+": this.getDate(), //日 
            "h+": this.getHours() % 12, //小时 
            "H+": this.getHours(), //小时 
            "m+": this.getMinutes(), //分 
            "s+": this.getSeconds(), //秒 
            "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
            "S": this.getMilliseconds() //毫秒 
        };
        if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o)
            if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length === 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
    };
    String.prototype.toJsonDateString = function (fmt) {
        ///<summary>格式化日期字符串。</summary>
        ///<param name="fmt" type="String">格式化字符串：yyyy-MM-dd HH:mm:ss</param>
        var date = new Date(this.replace('T', ' '));
        return date.toFormatString(fmt || 'yyyy-MM-dd hh:mm:ss');
    };
    String.prototype.randomSuffix = function () {
        ///<summary>添加随机码。</summary>
        if (this.indexOf('?') === -1)
            return this + '?_=' + (+new Date);
        return this + '&_=' + (+new Date);
    };
})(jQuery);
$(document).ready(function () {
    window.$container = $('#modal-container');
    if (window.$container.length === 0)
        window.$container = document.body;
    $readyExec();
});
$onready(function (context) {
    $('.moz-checkbox', context).click(function () {
        $(this).toggleClass('checked');
        if ($(this).hasClass('checked')) {
            $(this).find('input')[0].checked = 'checked';
        } else {
            $(this).find('input').removeAttr('checked');
        }
    });
    $('.moz-radiobox', context).click(function () {
        if ($(this).hasClass('checked')) {
            return;
        }
        $(this).parents('.moz-radioboxlist')
            .find('.moz-radiobox')
            .removeClass('checked')
            .each(function () {
                $(this).find('input').removeAttr('checked');
            });
        $(this).addClass('checked');
        $(this).find('input')[0].checked = 'checked';
    });
});