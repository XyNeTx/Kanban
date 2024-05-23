class Label {

    constructor(pID) {
        this.id = pID;
        this.property = new property(pID);
    }

    label = function (plabel = '') {
        if (plabel != '') this.property.label = plabel;
        if (plabel != '') $('[for=' + this.id + ']').html(this.property.label);
        if (plabel == '') return this.property.label;
    }
}


class TextBox {
    constructor(pID) {
        this.id = pID;
        this.property = new property(pID);
    }

    set type(Value) {
        this.property.type = Value;
    }
    get type() {
        return this.property.type;
    }

    set label(Value) {
        this.property.label = Value;
        $('[for=' + this.id + ']').html(this.property.label);
    }
    get label() {
        return this.property.label;
    }

    set value(Value) {
        this.property.value = Value;
        if (this.property.type == 'date') this.property.value = $('#' + this.id).val();
        if (this.property.type == 'time') this.property.value = $('#' + this.id).val();
        if (this.property.type == 'month') this.property.value = $('#' + this.id).val();
        $('#' + this.id).val(this.property.value);
    }
    get value() {
        if (this.property.type == 'date') return $('#' + this.id).val();
        if (this.property.type == 'time') return $('#' + this.id).val();
        if (this.property.type == 'month') return $('#' + this.id).val();
        return this.property.value;
    }

    set readonly(Value) {
        this.property.readonly = Value;
        if (this.property.readonly) $('#' + this.id).attr('readonly', this.property.readonly);
        if (!this.property.readonly) $('#' + this.id).removeAttr('readonly');
    }
    get readonly() {
        return this.property.readonly;
    }

    //value = function (pValue = '') {
    //    if (pValue != '') this.property.value = pValue;
    //    if (pValue != '') $('#' + this.id).val(this.property.value);
    //    if (pValue == '') return this.property.value;
    //}
    //val = function (pValue = '') {
    //    this.value(pValue);
    //}



    //readonly = function (pValue = '') {
    //    //console.log(this.id +' >> '+ pValue);
    //    this.property.readonly = (pValue == '' || pValue == false ? false : true);
    //    //console.log(this.id + ' >> ' +this.property.readonly);
    //    //if (this.property.readonly == true) $('#' + this.id).attr('readonly', this.property.readonly);
    //    //if (this.property.readonly == false) $('#' + this.id).removeAttr('readonly');
    //    if (pValue == '') return this.property.readonly;
    //}
}


class Button {
    constructor(pID) {
        this.id = pID;
        this.property = new property(pID);
    }

    set type(Value) {
        this.property.type = Value;
    }
    get type() {
        return this.property.type;
    }

    set label(Value) {
        this.property.label = Value;
        $('#' + this.id).html(this.property.label);
    }
    get label() {
        return this.property.label;
    }

    set value(Value) {
        this.property.value = Value;
        $('#' + this.id).val(this.property.value);
    }
    get value() {
        return this.property.value;
    }

    set readonly(Value) {
        this.property.readonly = Value;
        if (this.property.readonly) $('#' + this.id).attr('readonly', this.property.readonly);
        if (!this.property.readonly) $('#' + this.id).removeAttr('readonly');
    }
    get readonly() {
        return this.property.readonly;
    }
}


class Select {
    constructor(pID) {
        this.id = pID;
        this.property = new property(pID);
    }

    set type(Value) {
        this.property.type = Value;
    }
    get type() {
        return this.property.type;
    }

    set label(Value) {
        this.property.label = Value;
        $('[for=' + this.id + ']').html(this.property.label);
    }
    get label() {
        return this.property.label;
    }

    set value(Value) {
        this.property.value = Value;
        $('#' + this.id).val(this.property.value);
        this.title = $('#' + this.id + ' option:selected').text();
    }
    get value() {
        return this.property.value;
    }

    set title(Value) {
        this.property.title = Value;
        $('#' + this.id + ' option:selected').text(this.property.title);
    }
    get title() {
        return this.property.title;
    }

    set readonly(Value) {
        this.property.readonly = Value;
        if (this.property.readonly) $('#' + this.id).attr('readonly', this.property.readonly);
        if (!this.property.readonly) $('#' + this.id).removeAttr('readonly');
    }
    get readonly() {
        return this.property.readonly;
    }
}



class Radio {
    constructor(pID) {
        this.id = pID;
        this.property = new property(pID);
    }

    set type(Value) {
        this.property.type = Value;
    }
    get type() {
        return this.property.type;
    }

    set default(Value) {
        this.property.default = Value;
    }

    get default() {
        return this.property.default;
    }

    set label(Value) {
        this.property.label = Value;
        $('[for=' + this.id + ']').html(this.property.label);
    }
    get label() {
        return this.property.label;
    }

    set value(Value) {
        this.property.value = Value;
    }

    get value() {
        return this.property.value;
    }

    set checked(Value) {
        this.property.checked = Value;
    }
    get checked() {
        return this.property.checked;
    }

    set check(Value) {
        let _a = this.property.id + this.property.value;
        let _b = this.property.id + Value;
        let _ap = '', _bp = '';
        console.log(_a + ' >>> ' + _b);

        eval('_ap = ' + _a + '.property');
        eval('_bp = ' + _b + '.property');
        this.property.label = _bp.label;
        this.property.value = _bp.value;
        _ap.checked = false;
        _bp.checked = true;

        $('#' + _a).prop('checked', false);
        $('#' + _b).prop('checked', true);

        $('#' + _a).removeAttr('checked');
        $('#' + _b).attr('checked', 'checked');
    }
    get check() {
        return this.property.checked;
    }

}

class table {

    constructor(pID) {
        this.id = pID;
        this.property = new property(pID);
    }

    label = function (plabel = '') {
        if (plabel != '') this.property.label = plabel;
        if (plabel != '') $('[for=' + this.id + ']').html(this.property.label);
        if (plabel == '') return this.property.label;
    }

    value = function (pValue = '') {
        if (pValue != '') this.property.value = pValue;
        if (pValue != '') $('#' + this.id).val(this.property.value);
        if (pValue == '') return this.property.value;
    }

    readonly = function (pValue = '') {
        if (pValue != '') this.property.readonly = pValue;
        if (pValue != '') $('#' + this.id).attr('readonly', this.property.readonly);
        if (pValue == '') return this.property.readonly;
    }
}


class property {
    constructor(pID) {
        this.id = pID;
        this.name = pID;
        //this._type = null;
        //this._label = null;
        //this._value = null;
        //this._readonly = null;

        this.type = null;
        this.label = null;
        this.value = null;
        this.readonly = null;
        this.style = null;
        this.title = null;
        this.form = null;
        this.max = null;
        this.min = null;
        this.checked = null;
        this.rows = null;

        this.subfix = null;
        this.noedit = null;
        this.format = null;
        this.default = null;
        this.required = null;
        this.datacontroller = null;
        this.datavalue = null;
        this.datadescription = null;
        this.class = null;
        this.effect = null;
        this.legend = null;
    }
//    //label = function (value = '') {        
//    //    console.log(value);
//    //    if (value != '') $('[for=' + this.id + ']').html(value);
//    //    if (value == '') return this.property.label;
//    //}

//    set label(pValue) {
//        this._label = pValue;
//        $('[for=' + this.id + ']').html(this._label);
//    }

//    get label() {
//        return this._label;
//    }

//    set value(pValue) {
//        this._value = pValue;
//        $('#' + this.id).val(this._value);
//    }

//    get value() {
//        return this._value;
//    }

}

