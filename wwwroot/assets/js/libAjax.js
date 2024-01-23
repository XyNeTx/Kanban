
class labAjax {
    constructor() {
        this.Date = new labDate();
        this.Timer;
    }

    interval(pStart = 1000, pFinish = 3000, pInterval, pTimeout=null) {

        this.Timer = setInterval(pInterval, pStart);

        setTimeout(function () {
            if (typeof (pTimeout) == 'function') {
                pTimeout();
            };
            clearInterval(this.Timer);
        }, pFinish);

        return this.Timer
    }
    Interval(pStart = 1000, pFinish = 3000, pInterval, pTimeout = null) {
        this.interval(pStart, pFinish, pInterval, pTimeout);
    }


    IntervalStop(pTimer, pTimeout = null)
    {
        //console.log(pTimer);

        if (typeof (pTimeout) == 'function') {
            pTimeout();
        };
        clearInterval(pTimer);
    }





    on(pObjectName, pFunction = null) {
        pObjectName = (pObjectName.indexOf('#') == 0 ? pObjectName : '#' + pObjectName);
        $(pObjectName).on('input', function (e) {
            if ((pFunction != null) && (typeof (pFunction) == 'function')) {
                pFunction();
            }
        });
    }

    onEnter(pObjectName, pFunction = null) {
        pObjectName = (pObjectName.indexOf('#') == 0 ? pObjectName : '#' + pObjectName);
        $(pObjectName).on('keypress', function (e) {
            if (e.keyCode == 13) {
                if ((pFunction != null) && (typeof (pFunction) == 'function')) {
                    pFunction();
                    return false;
                } else {
                    return false; // prevent the button click from happening
                }
            }
        });
    }

    onClick(pObjectName, pFunction = null) {
        pObjectName = (pObjectName.indexOf('#') == 0 ? pObjectName : '#' + pObjectName);        
        $(pObjectName).on('click', function (e) {
            if ((pFunction != null) && (typeof (pFunction) == 'function')) {
                pFunction();
            }
        });
    }

    onChange(pObjectName, pFunction = null) {
        pObjectName = (pObjectName.indexOf('#') == 0 ? pObjectName : '#' + pObjectName);
        $(pObjectName).on('change', function (e) {
            if ((pFunction != null) && (typeof (pFunction) == 'function')) {
                pFunction();
            }
        });
    }

    onBlur(pObjectName, pFunction = null) {
        pObjectName = (pObjectName.indexOf('#') == 0 ? pObjectName : '#' + pObjectName);
        $(pObjectName).on('blur', function (e) {
            if ((pFunction != null) && (typeof (pFunction) == 'function')) {
                pFunction();
            }
        });
    }

    trim(pValue) {
        if (pValue != '' && pValue!=null) pValue = pValue.toString().replace(/^\s+|\s+$/gm, '');
        return pValue;
    }
}


class labDate {

    constructor(pFormat = 'yyyy-MM-dd HH:mm:ss') {
        this.Format = pFormat;
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

}


const xAjax = new labAjax();

const xDate = new labDate();

