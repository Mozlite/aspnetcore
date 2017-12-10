$onready(function (context) {
    $('[_load]', context).css('cursor', 'pointer').exec(function (s) {
        s.on('click',
            function () {
                var win = $(document.body).dset('win',
                    function () {
                        return $('<div>').addClass('win').appendTo(document.body);
                    });
                var open = s.hasClass('open');
                if (open) {
                    s.removeClass('open');
                    var panel = win.find('.win-panel');
                    var position = {};
                    position[panel.hasClass('win-left') ? 'left' : 'right'] = '-' + panel.width() + 'px';
                    panel.animate(position, 'fast');
                    return false;
                }
                $('[_load]').removeClass('open');
                s.addClass('open');
                win.load(s.attr('_load'),
                    function (e) {
                        var panel = win.find('.win-panel');
                        if (panel.hasClass('win-right')) {
                            panel.animate({
                                right: 0
                            }, 'fast', function () {
                                $('.main-slider').width();
                            });
                            win.find('.close').click(function () {
                                s.removeClass('open');
                                panel.animate({
                                    right: '-' + panel.width() + 'px'
                                }, 'fast');
                            });
                        }
                        else if (panel.hasClass('win-left')) {
                            panel.animate({
                                left: 0
                            }, 'fast');
                        }
                        $readyExec(win);
                    });
            });
    });
    $('.data-view', context).find('.moz-checkbox').click(function () {
        if ($(this).hasClass('checked')) {
            $(this).parents('.data-row').addClass('active');
        } else {
            $(this).parents('.data-row').removeClass('active');
        }
    });
    $('.data-view', context).find('[post-id],[get-id]').click(function () {
        var id = $(this).parents('.data-row').find('input[type=checkbox]').val();
        var url = $(this).attr('get-id');
        if (url) {
            location.href = url + '?id=' + id;
            return;
        }
        $ajax($(this).attr('post-id'), { id: id });
    });
    $('.filterbar .checkbox-all', context).find('.moz-checkbox').click(function () {
        var target = $(this).targetElement($(this).parents('.content-container').find('.data-view').find('.moz-checkbox'));
        if ($(this).hasClass('checked')) {
            target.addClass('checked').parents('.data-row').addClass('active');
            target.find('input').each(function () { this.checked = 'checked'; });
        } else {
            target.removeClass('checked').parents('.data-row').removeClass('active');
            target.find('input').removeAttr('checked');
        }
    });
    $('.filterbar', context).find('[type=submit]').click(function () {
        var qs = [];
        $(this).parents('.filterbar').find('input[name],select[name]').each(function () {
            if ((this.type === 'radio' || this.type === 'checkbox') && !this.checked)
                return;
            if (this.value)
                qs.push(this.name.toLowerCase() + '=' + this.value);
        });
        if (qs.length > 0)
            location.href = '?' + qs.join('&');
        else
            location.href = location.href;
    });
    $('.actionbar', context).find('[post-ids],[get-ids]').click(function () {
        var ids = $('.data-view').checkedVal();
        if (!ids) {
            $alert('请选择项目后再执行相关操作！');
            return;
        }
        var url = $(this).attr('get-ids');
        if (url) {
            location.href = url + '?ids=' + ids;
            return;
        }
        url = $(this).attr('post-ids');
        $ajax(url, { ids: ids });
    });
});
$(document).ready(function () {
    if (!window.menuWidth) window.menuWidth = $('.menu-wrapper').width();
    $('.menu-toggle-button').click(function () {
        if ($(this).toggleClass('closed').hasClass('closed')) {
            $('.sub-menu-wrapper').hide();
            $('.menu-wrapper').width(48);
        } else {
            $('.sub-menu-wrapper').width(menuWidth - 48).show();
            $('.menu-wrapper').width(menuWidth);
        }
    });
    $('.sub-menu-tree-header').click(function () {
        if ($(this).parent().toggleClass('closed').hasClass('closed')) {
            $(this).find('i.fa').removeClass('fa-angle-down').addClass('fa-angle-right');
        } else {
            $(this).find('i.fa').addClass('fa-angle-down').removeClass('fa-angle-right');
        }
    });
    $('.sub-menu-item-new .text').click(function () {
        $(this).hide().parent().find('input').show().focus();
    });
    $('.sub-menu-item-new input').blur(function () {
        $(this).hide().parent().find('.text').show();
    });
});