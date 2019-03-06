import 'select2/dist/css/select2.css';
import 'select2';
require('select2/dist/js/i18n/zh-CN.js');

Mozlite.queue(context => {
    $('.js-select2', context).exec(current => {
        var options = current.jsAttrs('select2');
        var placeholder = current.attr('placeholder');
        if (placeholder) options.placeholder = placeholder;
        options.language = 'zh-CN';
        current.addClass('select2').select2(options);
    });
});