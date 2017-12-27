var HMI = /** @class */ (function () {
    /**
     * 实例化一个HMI。
     * @param selector 选择器。
     * @param isEditable 是否可编辑。
     */
    function HMI(selector, editable) {
        if (editable === void 0) { editable = false; }
        this.editable = editable;
        if (typeof selector == "string")
            this.selector = document.querySelector(selector);
        else
            this.selector = selector;
        this.init();
    }
    HMI.prototype.init = function () {
        var _this = this;
        var dragables = document.querySelectorAll("[draggable=true]");
        for (var i = 0; i < dragables.length; i++) {
            dragables[i].addEventListener('dragstart', function (e) {
                _this.draggedElement = e.srcElement;
            });
        }
        this.selector.addEventListener("dragover", function (e) { e.preventDefault(); });
        this.selector.addEventListener("drop", function (e) {
            e.preventDefault();
            _this.selector.appendChild(_this.draggedElement.cloneNode());
            _this.draggedElement = null;
        });
    };
    return HMI;
}());
//# sourceMappingURL=common.js.map