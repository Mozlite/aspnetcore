import './site.scss';

Mozlite.queue(context => {
    $('[js-toggle=nav]', context).exec(current => {
        var toggles = current.find('.nav-toggle').on('click', function () {
            toggles.parent().removeClass('open').find('.nav-toggle>span').attr('class', 'fa fa-angle-right');
            $(this).parent().addClass('open').find('.nav-toggle>span').attr('class', 'fa fa-angle-down');
        });
    });
});