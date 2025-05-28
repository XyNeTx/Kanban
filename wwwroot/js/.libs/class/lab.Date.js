"use strict";
class labDate {
    constructor(pFormat = 'yyyy-MM-dd HH:mm:ss') {
        this.Format = function (pDate = '', pFormat = 'yyyyMMdd', pFunction = '') {
            var _yy = '', _mm = '', _dd = '';
            if (pDate.length == 8) {
                _yy = pDate.substring(0, 4);
                _mm = pDate.substring(4, 6);
                _dd = pDate.substring(6, 8);
            }
            else {
                if (pDate.indexOf('-') >= 0) {
                    let _arr = pDate.split("-");
                    _yy = _arr[0];
                    _mm = _arr[1];
                    _dd = _arr[2];
                }
                if (pDate.indexOf('/') >= 0) {
                    let _arr = pDate.split("-");
                    _yy = _arr[2];
                    _mm = _arr[1];
                    _dd = _arr[0];
                }
            }
            pDate = new Date(_yy, _mm, _dd);
            if (pFunction != '') {
                let _arr = pFunction.split("=");
                if (_arr[0].toUpperCase() === 'DD')
                    pDate = new Date(_yy, _mm, eval(parseInt(_dd) + parseInt(_arr[1])));
                if (_arr[0].toUpperCase() === 'MM')
                    pDate = new Date(_yy, eval(parseInt(_mm) + parseInt(_arr[1])), _dd);
                if (_arr[0].toUpperCase() === 'YY' || _arr[0].toUpperCase() === 'YYYY')
                    pDate = new Date(eval(parseInt(_yy) + parseInt(_arr[1])), _mm, _dd);
            }
            let _year = pDate.getFullYear();
            let _month = String(pDate.getMonth()).padStart(2, '0');
            let _date = String(pDate.getDate()).padStart(2, '0');
            pFormat = pFormat.replace('yyyy', _year);
            pFormat = pFormat.replace('yy', _year);
            pFormat = pFormat.replace('MM', _month);
            pFormat = pFormat.replace('mm', _month);
            pFormat = pFormat.replace('dd', _date);
            return pFormat;
        };
        //this.Format = pFormat;
        this.Value = pFormat;
        const _now = new Date();
        const _year = _now.getFullYear();
        const _month = String(_now.getMonth() + 1).padStart(2, '0');
        const _date = String(_now.getDate()).padStart(2, '0');
        const _hours = String(_now.getHours()).padStart(2, '0');
        const _minutes = String(_now.getMinutes()).padStart(2, '0');
        const _seconds = String(_now.getSeconds()).padStart(2, '0');
        this.Value = this.Value.replace('yyyy', _year);
        this.Value = this.Value.replace('yy', _year);
        this.Value = this.Value.replace('MM', _month);
        this.Value = this.Value.replace('dd', _date);
        this.Value = this.Value.replace('HH', _hours);
        this.Value = this.Value.replace('mm', _minutes);
        this.Value = this.Value.replace('ss', _seconds);
    }
    Now(pFormat = 'yyyy-MM-dd HH:mm:ss') {
        const _now = new Date();
        const _year = _now.getFullYear();
        const _month = String(_now.getMonth() + 1).padStart(2, '0');
        const _date = String(_now.getDate()).padStart(2, '0');
        const _hours = String(_now.getHours()).padStart(2, '0');
        const _minutes = String(_now.getMinutes()).padStart(2, '0');
        const _seconds = String(_now.getSeconds()).padStart(2, '0');
        pFormat = pFormat.replace('yyyy', _year);
        pFormat = pFormat.replace('yy', _year);
        pFormat = pFormat.replace('MM', _month);
        pFormat = pFormat.replace('dd', _date);
        pFormat = pFormat.replace('HH', _hours);
        pFormat = pFormat.replace('mm', _minutes);
        pFormat = pFormat.replace('ss', _seconds);
        return pFormat;
    }
    now(pFormat = 'yyyy-MM-dd HH:mm:ss') {
        return this.Now(pFormat);
    }
    Date(pFormat = 'yyyy-MM-dd', pFunction = '') {
        let _now = new Date();
        if (pFunction != '') {
            let _arr = pFunction.split("=");
            //console.log(_arr);
            //console.log(eval(parseInt(_now.getDate()) + parseInt(_arr[1])));
            if (_arr[0].toUpperCase() === 'DD')
                _now = new Date(_now.getFullYear(), _now.getMonth(), eval(parseInt(_now.getDate()) + parseInt(_arr[1])));
            if (_arr[0].toUpperCase() === 'MM')
                _now = new Date(_now.getFullYear(), eval(parseInt(_now.getMonth()) + parseInt(_arr[1])), _now.getDate());
            if (_arr[0].toUpperCase() === 'YY' || _arr[0].toUpperCase() === 'YYYY')
                _now = new Date(eval(parseInt(_now.getFullYear()) + parseInt(_arr[1])), _now.getMonth(), _now.getDate());
            //console.log(_now);
        }
        let _year = _now.getFullYear();
        let _month = String(_now.getMonth() + 1).padStart(2, '0');
        let _date = String(_now.getDate()).padStart(2, '0');
        pFormat = pFormat.replace('yyyy', _year);
        pFormat = pFormat.replace('yy', _year);
        pFormat = pFormat.replace('MM', _month);
        pFormat = pFormat.replace('mm', _month);
        pFormat = pFormat.replace('dd', _date);
        return pFormat;
    }
    date(pFormat = 'yyyy-MM-dd', pFunction = '') {
        return this.Date(pFormat, pFunction);
    }
    Time(pFormat = 'HH:mm:ss') {
        const _now = new Date();
        const _hours = String(_now.getHours()).padStart(2, '0');
        const _minutes = String(_now.getMinutes()).padStart(2, '0');
        const _seconds = String(_now.getSeconds()).padStart(2, '0');
        pFormat = pFormat.replace('HH', _hours);
        pFormat = pFormat.replace('hh', _hours);
        pFormat = pFormat.replace('mm', _minutes);
        pFormat = pFormat.replace('ss', _seconds);
        return pFormat;
    }
    time(pFormat = 'HH:mm:ss') {
        return this.Time(pFormat);
    }
}
const xDate = new labDate();
