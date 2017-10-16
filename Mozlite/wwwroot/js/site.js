(function ($) {
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
    window.$alert = function (message, type, func) {
        ///<summary>显示警告消息。</summary>
        if (typeof message === 'object' && message) {
            type = message.type;
            message = message.message;
        }
        if (!message) return;
        var modal = $(document.body)
            .dset('alert',
            function () {
                return $('<div class="js-alert modal fade"><div class="modal-dialog"><div class="modal-content"><div class="modal-body" style="padding: 50px 30px 30px;"><div class="col-sm-2"><i style="font-size: 50px;"></i></div> <span class="col-sm-10" style="line-height: 26px; padding-left: 0;"></span></div><div class="modal-footer"><button type="button" class="btn btn-primary"><i class="fa fa-check"></i> 确定</button></div></div></div></div>')
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
                    $alert('需要登陆才能够执行此操作！<a onclick="$(this).jsModal(\'/login\');" href="javascript:;">点击登陆...</a>', 'warning');
                    return;
                }
                if (error) error(resp);
                else document.write(resp.responseText);
            }
        });
    };
})(jQuery);