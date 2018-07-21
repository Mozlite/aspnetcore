interface HTMLElement {
    /**
     * 设置当前原始样式。
     * @param key 样式键。
     * @param value 样式值。
     * @returns {HTMLElement} 当前元素实例。
     */
    css(key: string, value: string): HTMLElement;
    css(style: object): HTMLElement;
};

HTMLElement.prototype.css = function () {
    var styles = {};
    var style = this.getAttribute("style");
    if (style) {
        style.split(";").forEach(s => {
            var ss = s.split(":") as string[];
            if (ss.length == 2) {
                styles[ss[0].trim()] = ss[1].trim();
            }
        });
    }
    if (arguments.length == 2) {
        styles[arguments[0]] = arguments[1];
    } else if (typeof arguments[0] == "object") {
        var obj = arguments[0];
        Object.keys(obj).forEach(key => {
            styles[key] = obj[key];
        });
    }
    var current = [];
    Object.keys(styles).forEach(key => {
        current.push(key + ":" + styles[key]);
    });
    if (current.length > 0) {
        this.setAttribute("style", current.join(";"));
    }
    return this;
};

class Designer {
    private selector: HTMLElement;
    private draggedElement: HTMLElement;
    /**
     * 实例化一个HMI。
     * @param selector 选择器。
     * @param editable 是否可编辑。
     */
    constructor(selector: string | HTMLElement, public editable: boolean = false) {
        if (typeof selector == "string")
            this.selector = document.querySelector(selector) as HTMLElement;
        else
            this.selector = selector;
        this.selector.scrollTo(990, 990);
        this.init();
    }

    init() {

    }
};

$(function() { new Designer(".moz-designer", true); });