class HMI {
    private selector: HTMLElement;
    private draggedElement: HTMLElement;
    /**
     * 实例化一个HMI。
     * @param selector 选择器。
     * @param isEditable 是否可编辑。
     */
    constructor(selector: string | HTMLElement, public editable: boolean = false) {
        if (typeof selector == "string")
            this.selector = document.querySelector(selector) as HTMLElement;
        else
            this.selector = selector;
        this.init();
    }

    private init(): void {
        let dragables = document.querySelectorAll("[draggable=true]");
        for (let i = 0; i < dragables.length; i++) {
            (<HTMLElement>dragables[i]).addEventListener('dragstart', e => {
                this.draggedElement = e.srcElement as HTMLElement;
            });
        }
        this.selector.addEventListener("dragover", e => { e.preventDefault(); });
        this.selector.addEventListener("drop", e => {
            e.preventDefault();
            this.selector.appendChild(this.draggedElement.cloneNode(true));
            this.draggedElement = null;
        });
    }
}