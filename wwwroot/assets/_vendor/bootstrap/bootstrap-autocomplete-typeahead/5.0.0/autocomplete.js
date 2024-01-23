const DEFAULTS = {
    treshold: 2,
    maximumItems: 5,
    highlightTyped: true,
    highlightClass: 'text-primary',
    showValue: false,
    showValueBeforeLabel: false,

    displayLabel: 'label',
    displayValue: 'label',
};
class Autocomplete {
    constructor(field, options) {
        this.field = field;
        this.options = Object.assign({}, DEFAULTS, options);
        this.dropdown = null;
        this.data = this.options.data;
        this.row = null;
        field.parentNode.classList.add('dropdown');
        field.setAttribute('data-toggle', 'dropdown');
        field.classList.add('dropdown-toggle');
        const dropdown = ce(`<div class="dropdown-menu" ></div>`);
        if (this.options.dropdownClass)
            dropdown.classList.add(this.options.dropdownClass);
        insertAfter(dropdown, field);
        this.dropdown = new bootstrap.Dropdown(field, this.options.dropdownOptions)
        field.addEventListener('click', (e) => {
            if (this.createItems() === 0) {
                e.stopPropagation();
                this.dropdown.hide();
            }
        });
        field.addEventListener('input', () => {
            if (this.options.onInput)
                this.options.onInput(this.field.value);
            this.renderIfNeeded();

        });
        field.addEventListener('keydown', (e) => {
            if (e.keyCode === 27) {
                this.dropdown.hide();
                return;
            }
        });

    }
    setData(data) {
        this.options.data = data;
    }
    renderIfNeeded() {
        if (this.createItems() > 0)
            this.dropdown.show();
        else
            this.field.click();

    }
    createItem(lookup, item, row) {
        let _label = item[this.options.displayLabel];
        if (this.options.highlightTyped) {
            const idx = _label.toLowerCase().indexOf(lookup.toLowerCase());
            const className = Array.isArray(this.options.highlightClass) ? this.options.highlightClass.join(' ') : (typeof this.options.highlightClass == 'string' ? this.options.highlightClass : '')
            _label = _label.substring(0, idx)
                + `<span class="${className}">${_label.substring(idx, idx + lookup.length)}</span>`
                + _label.substring(idx + lookup.length, _label.length);

        } //else
            //_label = item[this.options.displayLabel];
            //label = (this.options.displayLabel == 'label' ? item.label : this.options.data[row][this.options.displayLabel]);
        //console.log(_label);
        return ce(`<button type="button" class="dropdown-item" data-row="${row}" data-value="${item.value}">${_label}</button>`);

    }
    createItems() {
        const lookup = this.field.value;
        if (lookup.length < this.options.treshold) {
            this.dropdown.hide();
            return 0;
        }
        const items = this.field.nextSibling;
        items.innerHTML = '';
        let count = 0;
        for (let i = 0; i < this.options.data.length; i++) {
            const row = i;
            //const { label, value, labelTH} = this.options.data[i];
            //const item = { label, value, labelTH };
            const item = this.options.data[i];
            if (item[this.options.displayLabel].toLowerCase().indexOf(lookup.toLowerCase()) >= 0) {
                items.appendChild(this.createItem(lookup, item, row));
                if (this.options.maximumItems > 0 && ++count >= this.options.maximumItems)
                    break;

            }
        }
        this.field.nextSibling.querySelectorAll('.dropdown-item').forEach((item) => {
            item.addEventListener('click', (e) => {
                let _value = e.target.getAttribute('data-value');
                let _row = e.target.getAttribute('data-row');
                this.field.value = e.target.innerText;
                if (this.options.displayValue != 'label') this.field.value = this.options.data[_row][this.options.displayValue];
                if (this.options.onSelectItem) this.options.onSelectItem({ value: _value, label: e.target.innerText, row: _row, data: this.options.data[_row], });
                this.dropdown.hide();

            })
        });
        return items.childNodes.length;

    }
}
function ce(html) {
    let div = document.createElement('div');
    div.innerHTML = html;
    return div.firstChild;
}
function insertAfter(elem, refElem) {
    return refElem.parentNode.insertBefore(elem, refElem.nextSibling)
}