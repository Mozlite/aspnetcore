import './admin.scss';
var timer, delay = 3 * 1000;
//小数点
function fixed(format, value) {
    return format.replace(/\$(\d+)/ig,
        function (all, current) {
            if (current == 0 || value == '0' || value == '1') return value;
            value = +value;
            if (isNaN(value)) return value;
            return value.toFixed(current);
        });
}
//替换绑定数据
function replaceData(format, item) {
    return format.replace(/\$([a-z0-9\\-]+)/ig,
        function (all, current) {
            return item[current];
        });
}

/**
 * 获取属性值数组.
 * @param {String} name 属性名称。
 * @param {JQuery|undefined} context 上下文。
 * @return {Array<Number>} 数组。
 */
window.$gets = function (name, context) {
    var values = [];
    $('[' + name + ']', context).exec(current => {
        var value = +current.attr(name);
        if (!isNaN(value)) {
            values.push(value);
        }
    });
    return values;
};

//获取id
window.$bind = function (context) {
    //通过设备`id`以及属性名称`v-prop`来获取当前变量的值
    $('[v-prop]', context).exec(current => {
        if (!current.attr('v-id')) {
            Mozlite.ajax('/data-apis/prop-vid', { id: current.attr('v-device'), prop: current.attr('v-prop') }, d => {
                current.attr('v-id', d.id);
                return true;
            });
        }
    });
    //通过设备`id`以及属性ID`v-propid`来获取当前变量的值
    $('[v-propid]', context).exec(current => {
        if (!current.attr('v-id')) {
            Mozlite.ajax('/data-apis/propid-vid', { id: current.attr('v-device'), propid: current.attr('v-propid') }, d => {
                current.attr('v-id', d.id);
                return true;
            });
        }
    });
    //通过设备`id`以及属性特殊标志`v-mode`来获取当前变量的值
    $('[v-mode]', context).exec(current => {
        if (!current.attr('v-id')) {
            Mozlite.ajax('/data-apis/mode-vid', { id: current.attr('v-device'), mode: current.attr('v-mode') }, d => {
                current.attr('v-id', d.id);
                return true;
            });
        }
    });
    //通过设备`id`以及属性名称`vset-prop`来获取当前变量的值
    $('[vset-prop]', context).exec(current => {
        if (!current.attr('vset-id')) {
            Mozlite.ajax('/data-apis/prop-vid', { id: current.attr('v-device'), prop: current.attr('vset-prop') }, d => {
                current.attr('vset-id', d.id);
                return true;
            });
        }
    });
    //通过设备`id`以及属性ID`vset-propid`来获取当前变量的值
    $('[vset-propid]', context).exec(current => {
        if (!current.attr('vset-id')) {
            Mozlite.ajax('/data-apis/propid-vid', { id: current.attr('v-device'), propid: current.attr('vset-propid') }, d => {
                current.attr('vset-id', d.id);
                return true;
            });
        }
    });
    //通过设备`id`以及属性特殊标志`vset-mode`来获取当前变量的值
    $('[vset-mode]', context).exec(current => {
        if (!current.attr('vset-id')) {
            Mozlite.ajax('/data-apis/mode-vid', { id: current.attr('v-device'), mode: current.attr('vset-mode') }, d => {
                current.attr('vset-id', d.id);
                return true;
            });
        }
    });
    //故障点
    var devices = $gets('v-device', context);
    if (devices.length > 0) {
        Mozlite.ajax('/data-apis/mode-vids', { ids: devices }, d => {
            for (var i in d) {
                $('[v-device=' + i + ']').attr('v-errid', d[i]);
            }
            return true;
        });
    }
    //通过变量`id`获取变量实例{id,name,unit,standardUnit}
    var ids = $gets('v-bind', context);
    if (ids.length) {
        Mozlite.ajax('/data-apis/variable-vid', { ids }, d => {
            d.forEach(item => {
                $('[v-bind=' + item.id + ']', context).exec(current => {
                    for (var name in item) {
                        if (name == 'id') continue;
                        current.attr('v-data-' + name.toCamelCase(), item[name]);
                    }
                    current.trigger('bind', item, current);
                    if (window.d3) window.d3.select(current[0]).dispatch('bind');
                    var html = current.attr('v-bind-html');
                    if (html) {
                        current.html(replaceData(html, item));
                    }
                    html = current.attr('v-bind-text');
                    if (html) {
                        current.text(replaceData(html, item));
                    }
                });
            });
            return true;
        });
    }
};

//获取点数据，并且一起进行赋值以及事件的触发
//1.更新`v-value`的属性值
//2.如果设置`v-html`则设置html值，并替换格式（$N，N表示小数点个数）
//3.如果设置`v-text`则设置text值，并替换格式（$N，N表示小数点个数）
//4.触发`v.update`事件
$(function fetch() {
    var ids = $gets('v-id');
    var errids = $gets('v-errid');
    var allids = ids.concat(errids);
    if (allids.length) {
        allids = allids.distinct();
        Mozlite.ajax('/data-apis/getvs',
            { ids: allids },
            d => {
                d.forEach(item => {
                    //显示变量
                    if (ids.indexOf(item.id) != -1) {
                        $('[v-id=' + item.id + ']').exec(current => {
                            if (current.attr('v-value') == item.value)
                                return;
                            current.attr('v-value', item.value);
                            current.trigger('update', item, current);
                            if (window.d3) window.d3.select(current[0]).dispatch('update');
                            var html = current.attr('v-html');
                            if (html) {
                                current.html(fixed(html, item.value));
                            }
                            html = current.attr('v-text');
                            if (html) {
                                current.text(fixed(html, item.value));
                            }
                        });
                    }
                    //错误变量
                    if (errids.indexOf(item.id) != -1) {
                        $('[v-errid=' + item.id + ']').exec(current => {
                            var errsrc = +current.attr('v-errval') || 0;
                            var errnow = +item.value || 0;
                            if (errsrc == errnow)
                                return;
                            current.attr('v-errval', item.value);
                            var key = errnow == 0 ? 'success' : 'error';
                            current.trigger(key, item, current);
                            if (window.d3) window.d3.select(current[0]).dispatch(key);
                        });
                    }
                });
                if (timer) clearTimeout(timer);
                timer = setTimeout(function () { fetch(); }, delay);
                return true;
            },
            e => {
                if (timer) clearTimeout(timer);
                timer = setTimeout(function () { fetch(); }, delay);
            });
    } else {
        if (timer) clearTimeout(timer);
        timer = setTimeout(function () { fetch(); }, delay);
    }
});

function totalSize(context) {
    var notify = $('.size-notify', context).sizeVal();
    $('.size-notify', context).sizeVal(notify, x => '(' + x + ')');
    var msg = $('.size-msg', context).sizeVal();
    $('.size-msg', context).sizeVal(msg, x => '(' + x + ')');
    var task = $('.size-task', context).sizeVal();
    $('.size-task', context).sizeVal(task, x => '(' + x + ')');
    $('.size-total').sizeVal(notify + msg + task);
}

$.fn.sizeVal = function (value, format) {
    if (typeof value == 'undefined')
        return +this.attr('data-size') || 0;
    return this.each(function () {
        if (format) format = format(value);
        else format = value;
        var current = $(this).attr('data-size', value).html(format);
        if (value > 0) current.show(); else current.hide();
        return current;
    });
};

$.fn.sizeAdd = function (value, format) {
    value = value || 1;
    return this.each(function () {
        var current = $(this).sizeVal() + value;
        if (current < 0)
            current = 0;
        return $(this).sizeVal(current, format);
    });
};

//队列
Mozlite.queue(context => {
    totalSize(context);
    $('#notify', context).find('a.dropdown-panel-item').exec(current => {
        current.on('click',
            e => {
                e.stopPropagation();
                if (!current.hasClass('notification-new')) return false;
                Mozlite.ajax(current.attr('href'), current.jsAttrs('data'), d => {
                    if (d.type == 'success') {
                        current.removeClass('notification-new').addClass('notification-confirmed');
                        $('.size-notify', context).sizeAdd(-1, x => '(' + x + ')');
                        totalSize(context);
                    }
                    return true;
                });
                return false;
            });
    });
    $('#notify', context).find('.dropdown-panel-action a').exec(current => {
        current.on('click',
            e => {
                e.stopPropagation();
                Mozlite.ajax(current.attr('href'), {}, d => {
                    if (d.type == 'success') {
                        $('#notify', context).find('.dropdown-panel-content').html('');
                        $('.size-notify', context).sizeVal(0);
                        totalSize(context);
                    }
                    return true;
                });
                return false;
            });
    });
});
/**
 * 写入变量值。
 * @param {Number} id 变量Id；
 * @param {any} value 变量值；
 * @param {Number|undefined} priority 优先级，越大越靠前
 * @return {Boolean} 返回写入结果。
 */
window.$write = function (id, value, priority) {
    if (!id) {
        Mozlite.alert('变量不存在，变量id不能为空！');
        return false;
    }
    if (typeof value === 'undefined') {
        Mozlite.alert('写入值不能为空！');
        return false;
    }
    Mozlite.ajax('/data-apis/write-vid', { id, value, priority: priority || 0 }, d => {
        return d.type === 'success';
    });
    return true;
};
//延迟调用
$(function () { setTimeout($bind(), 1000); });

/*
//ajax加载页面
var scripts = [], links = [];
$('script[src]').each(function () {
    scripts.push(this.src.toLowerCase());
});
$('link[href]').each(function () {
    links.push(this.href.toLowerCase());
});

function render(html) {
    var doc = createDoc(html);
    var selector = window.ajaxSelector || 'div.main';
    document.head.title = doc.find('title').html();
    document.body.className = doc.find('[selector=body]').attr('class');
    $(selector).replaceWith(doc.find(selector));
    //style
    $('style').remove();
    doc.find('link[href]').each(function () {
        if (links.indexOf(this.href.toLowerCase()) === -1) {
            links.push(this.href.toLowerCase());
            document.head.appendChild(this);
        }
    });
    var fragment = '';
    doc.find('style').each(function () {
        fragment += '\r\n';
        fragment += this.innerHTML;
    });
    $('<style type="text/css" id="page-style"></style>').html(fragment).appendTo(document.head);
    //script
    $('script').each(function () {
        if (this.src) return;
        $(this).remove();
    });
    var script = '';
    doc.find('script').each(function () {
        if (this.src) {
            var src = this.src.toLowerCase();
            if (scripts.indexOf(src) === -1) {
                $(this).appendTo(document.body);
                scripts.push(src);
            }
        } else {
            script += '\r\n';
            script += this.innerHTML;
        }
    });
    $('<script type="text/javascript" id="page-script"></script>').html(script).appendTo(document.body);
    Mozlite.render(selector);
}

function createDoc(html) {
    var head = '<div selector="head"/>', body;
    var index = html.indexOf('<head');
    if (index !== -1) {
        html = html.substr(index + 5);
        index = html.indexOf('</head>');
        head = html.substr(0, index);
        head = '<div selector="head"' + head + '</div>';
        html = html.substr(index + 7);
    }
    index = html.indexOf('<body');
    if (index !== -1) {
        html = html.substr(index + 5);
        index = html.lastIndexOf('</body>');
        html = html.substr(0, index);
        body = '<div selector="body"' + html + '</div>';
    } else {
        body = '<div selector="body">' + html + '</div>';
    }
    return $('<div selector="html">' + head + body + '</div>');
}

Mozlite.queue(context => {
    $('a[js-ajax=true]', context).exec(current => {
        current.on('click',
            e => {
                var url = current.attr('href');
                if (url.startsWith('#'))
                    url = url.substr(1);
                else
                    location.href = '#' + url;
                if (url) $.get(url, html => {
                    render(html);
                    var menu = current.parents('.js-menu');
                    menu.find('li.active').removeClass('active');
                    current.parents('li.nav-item').addClass('active');
                });
                return false;
            });
    });
});
*/