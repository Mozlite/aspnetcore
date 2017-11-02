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
                    return;
                }
                $('[_load]').removeClass('open');
                s.addClass('open');
                win.load(s.attr('_load'),
                    function () {
                        var panel = win.find('.win-panel');
                        if (panel.hasClass('win-right')) {
                            panel.animate({
                                right: 0
                            }, 'fast');
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
});