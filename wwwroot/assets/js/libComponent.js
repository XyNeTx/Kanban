

$(document).ready(function () {

    $('[format^="yyyy"][format$="mm"]').each(function (e) {
        $(this).attr('autocomplete', 'true');
        var _format = $(this).attr("format");
        var _value = ($(this).attr("value") != undefined ? $(this).attr("value") : _now);
        var _onshow = ($(this).attr("onshow") != undefined ? $(this).attr("onshow").replace('()', '') : '');
        var _onselect = ($(this).attr("onselect") != undefined ? $(this).attr("onselect").replace('()', '') : '');

        var _now = new Date();
        var _yy = ('20' + _now.getFullYear()).slice(-4);
        var _mm = ('0' + (_now.getMonth() + 1)).slice(-2);
        var _isDate = 0;
        var _split = _value;

        if (_value != undefined) {
            if (new Date(_value) == 'Invalid Date') _isDate = 1;
            if (_value.indexOf('mm') >= 0) _isDate = 1;
            if (_value.indexOf('yy') >= 0) _isDate = 1;

            if (_value.indexOf(';') >= 0) {
                _split = _value.split(';');
                _isDate = _split.length;
            }

            for (var i = 0; i < _isDate; i++) {

                _value = ((_isDate > 0 && _split != _value) ? _split[i] : _value);

                //### Year
                if (_value.indexOf('yy+') >= 0) {
                    _now.setFullYear(_now.getFullYear() + parseInt(_value.replace('yy+', '')));
                    _yy = ('1900' + _now.getFullYear()).slice(-4);
                }
                if (_value.indexOf('yy-') >= 0) {
                    _now.setFullYear(_now.getFullYear() - parseInt(_value.replace('yy-', '')));
                    _yy = ('1900' + _now.getFullYear()).slice(-4);
                }
                if (_value.indexOf('yy=') >= 0) {
                    _yy = ('1900' + _value.replace('yy=', '')).slice(-4);
                }

                //### Month
                if (_value.indexOf('mm+') >= 0) {
                    _now.setMonth(_now.getMonth() + parseInt(_value.replace('mm+', '')));
                    _yy = ('1900' + _now.getFullYear()).slice(-4);
                    _mm = ('0' + (_now.getMonth() + 1)).slice(-2);
                }
                if (_value.indexOf('mm-') >= 0) {
                    _now.setMonth(_now.getMonth() - parseInt(_value.replace('mm-', '')));
                    _yy = ('1900' + _now.getFullYear()).slice(-4);
                    _mm = ('0' + (_now.getMonth() + 1)).slice(-2);
                }
                if (_value.indexOf('mm=') >= 0) {
                    if (_value.replace('mm=', '') == 'first') {
                        _mm = '01';
                    } else if (_value.replace('mm=', '') == 'last') {
                        _mm = '12';
                    } else {
                        _mm = ('0' + _value.replace('mm=', '')).slice(-2);
                    }
                }

            }

            _value = _format.replace('yyyy', _yy).replace('mm', _mm);
        }
        _now = _format.replace('yyyy', _yy).replace('mm', _mm);

        _value = (_value == undefined ? _now : _value);



        $(this).datepicker({
            uiLibrary: 'bootstrap5',
            format: _format,
            viewMode: "months",
            minViewMode: "months",
            setDate: new Date(),
            autoclose: true
        }).on("show", function () {

            var _val = $(this).val();
            if (_val == undefined) _val = _value;

            //$(this).val(_val).datepicker('update');
            if (typeof window[_onshow] === 'function') formok = window[_onshow]();

        }).on("changeDate", function (e) {

            //$(this).val(_value).datepicker('update');
            if (typeof window[_onselect] === 'function') formok = window[_onselect](e);
        });

        $(this).val(_value);
    });



    $('[format^="yyyy"][format$="dd"]').each(function (e) {
        $(this).attr('autocomplete', 'true');
        var _format = $(this).attr("format");
        var _value = $(this).attr("value");
        var _default = $(this).attr("default");
        var _onshow = ($(this).attr("onshow") != undefined ? $(this).attr("onshow").replace('()', '') : '');
        var _onselect = ($(this).attr("onselect") != undefined ? $(this).attr("onselect").replace('()', '') : '');


        var _now = new Date();
        var _yy = ('20' + _now.getFullYear()).slice(-4);
        var _mm = ('0' + (_now.getMonth() + 1)).slice(-2);
        var _dd = ('0' + _now.getDate()).slice(-2);
        var _isDate = 0;
        var _split = _value;

        //console.log(_default);

        if (_value != undefined) {
            if (new Date(_value) == 'Invalid Date') _isDate = 1;
            if (_value.indexOf('dd') >= 0) _isDate = 1;
            if (_value.indexOf('mm') >= 0) _isDate = 1;
            if (_value.indexOf('yy') >= 0) _isDate = 1;

            if (_value.indexOf(';') >= 0) {
                _split = _value.split(';');
                //_isDate = _value.indexOf(';');
                _isDate = _split.length;
            }
            //console.log(_isDate);

            for (var i = 0; i < _isDate; i++) {
                //console.log(_split[i]);

                //console.log(i);
                //console.log(_split);
                //console.log(_value);
                //console.log((_isDate > 0 && _split != _value));

                _value = ((_isDate > 0 && _split != _value) ? _split[i] : _value);


                //console.log(_value);

                //### Year
                if (_value.indexOf('yy+') >= 0) {
                    _now.setFullYear(_now.getFullYear() + parseInt(_value.replace('yy+', '')));
                    _yy = ('1900' + _now.getFullYear()).slice(-4);
                }
                if (_value.indexOf('yy-') >= 0) {
                    _now.setFullYear(_now.getFullYear() - parseInt(_value.replace('yy-', '')));
                    _yy = ('1900' + _now.getFullYear()).slice(-4);
                }
                if (_value.indexOf('yy=') >= 0) {
                    _yy = ('1900' + _value.replace('yy=', '')).slice(-4);
                }

                //### Month
                if (_value.indexOf('mm+') >= 0) {
                    _now.setMonth(_now.getMonth() + parseInt(_value.replace('mm+', '')));
                    _yy = ('1900' + _now.getFullYear()).slice(-4);
                    _mm = ('0' + (_now.getMonth() + 1)).slice(-2);
                }
                if (_value.indexOf('mm-') >= 0) {
                    _now.setMonth(_now.getMonth() - parseInt(_value.replace('mm-', '')));
                    _yy = ('1900' + _now.getFullYear()).slice(-4);
                    _mm = ('0' + (_now.getMonth() + 1)).slice(-2);
                }
                if (_value.indexOf('mm=') >= 0) {
                    if (_value.replace('mm=', '') == 'first') {
                        _mm = '01';
                    } else if (_value.replace('mm=', '') == 'last') {
                        _mm = '12';
                    } else {
                        _mm = ('0' + _value.replace('mm=', '')).slice(-2);
                    }
                }

                //### Day
                if (_value.indexOf('dd+') >= 0) {
                    _now.setDate(_now.getDate() + parseInt(_value.replace('dd+', '')));
                    _yy = ('1900' + _now.getFullYear()).slice(-4);
                    _mm = ('0' + (_now.getMonth() + 1)).slice(-2);
                    _dd = ('0' + _now.getDate()).slice(-2);
                }
                if (_value.indexOf('dd-') >= 0) {
                    _now.setDate(_now.getDate() - parseInt(_value.replace('dd-', '')));
                    _yy = ('1900' + _now.getFullYear()).slice(-4);
                    _mm = ('0' + (_now.getMonth() + 1)).slice(-2);
                    _dd = ('0' + _now.getDate()).slice(-2);
                }
                if (_value.indexOf('dd=') >= 0) {
                    if (_value.replace('dd=', '') == 'first') {
                        _dd = "01";
                    } else if (_value.replace('dd=', '') == 'last') {
                        var _last = new Date(_now.getFullYear(), _now.getMonth() + 1, 0);
                        _dd = ('0' + _last.getDate()).slice(-2);
                    } else {
                        _dd = ('0' + _value.replace('dd=', '')).slice(-2);
                    }
                }
            }
            
            _value = (_isDate == 0 ? _default : _format.replace('yyyy', _yy).replace('mm', _mm).replace('dd', _dd));
            //console.log(_value);
        }
        _now = _format.replace('yyyy', _yy).replace('mm', _mm).replace('dd', _dd);


        _value = (_value == undefined ? _now : '');
        _value = (_default != undefined ? (_default.toUpperCase() == 'NOW' ? _now : _default) : _value);


        $(this).off("click");

        $(this).datepicker({
            uiLibrary: 'bootstrap5',
            format: _format,
            todayHighlight: true,
            setDate: new Date(),
            autoclose: true
        }).on('click', function (e) {
            //console.log('xxx')
            e.preventDefault();
            //return;
        }).on("show", function () {

            //console.log('show');

            var _val = $(this).val();
            if (_val == undefined) _val = _value;

            //console.log(_val);

            $(this).val(_val).datepicker('update');
            if (typeof window[_onshow] === 'function') formok = window[_onshow]();

        }).on("changeDate", function (e) {
            console.log('changeDate');

            //$(this).val(_value).datepicker('update');
            if (typeof window[_onselect] === 'function') formok = window[_onselect](e);
        });

        $(this).val(_value);
    });

    //$('input [noedit]').on('click', function () {
    //    console.log('xxx')
    //});



    $('[format^="hh"][format$="mm"]').each(function (e) {
        $(this).attr('autocomplete', 'true');
        var _format = $(this).attr("format");
        var _value = $(this).attr("value");
        var _default = $(this).attr("default");
        var _onshow = ($(this).attr("onshow") != undefined ? $(this).attr("onshow").replace('()', '') : '');
        var _onselect = ($(this).attr("onselect") != undefined ? $(this).attr("onselect").replace('()', '') : '');



        $(this).timepicker({
            uiLibrary: 'bootstrap',
            format: 'HH.MM',
            mode: '24hr',
            modal: false,
            header: false,
            footer: false,
            autoclose: true
        }).on('click', function (e) {
        }).on("show", function () {
        }).on("changeDate", function (e) {
        });

        //$(this).val(_value);
    });


    $('input').off("click");

});

















class labDropDownList {
    constructor() {

    }


    bind(pObject, pData = null, pValue='value', pText='text') {
        //console.log(pData);

        $.each(pData, function (key, value) {
            //console.log(value);

            $('#' + pObject).append($('<option>', {
                value: value[pValue],
                text: value[pText]
            }));
        });


        //renderSelectBox();
        var _value = $("#" + pObject).attr('value');
        $("#" + pObject).val(_value);

    }

}



const xDropDownList = new labDropDownList();