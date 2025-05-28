"use strict";
var stringConstructor = "test".constructor;
var arrayConstructor = [].constructor;
var objectConstructor = ({}).constructor;
function TypeOf(object) {
    if (object === null) {
        return "null";
    }
    if (object === undefined) {
        return "undefined";
    }
    if (object.constructor === stringConstructor) {
        return "String";
    }
    if (object.constructor === arrayConstructor) {
        return "Array";
    }
    if (object.constructor === objectConstructor) {
        return "Object";
    }
    {
        return "Don't know";
    }
}
function CheckObject(pObject) {
    return (pObject.indexOf('#') == 0 ? pObject : '#' + pObject);
    //return (pObject.indexOf('#') != 0 ? '#' + pObject : pObject);
}
function ReplaceAll(pValue = '', pSearch = '', pReplace = '') {
    return pValue.replace(new RegExp(pSearch, 'g'), pReplace);
}
function replaceall(pValue = '', pSearch = '', pReplace = '') {
    return ReplaceAll(pValue, pSearch, pReplace);
}
function Trim(pValue = '') {
    pValue = '' + pValue;
    return (pValue != '' ? pValue.trim() : '');
}
function trim(pValue = '') {
    return Trim(pValue);
}
function MaskCurrency(value, decimal = 2, showZero = false, keep = false) {
    value = ReplaceAll(value, ',', '');
    if (value != '') {
        if (value.indexOf('.') >= 0) {
            var _digit = value.substring(0, value.indexOf('.'));
            _digit = (_digit == '' ? 0 : _digit);
            var _decimal = value.substring(value.indexOf('.') + 1);
            _decimal = _decimal.replace('.', '').replace(',', '');
            var _length = _decimal.length;
            //console.log(value);
            //console.log(_digit);
            //console.log(_decimal);
            //console.log(_length);
            ////console.log(_decimal.replace('.', '').replace(',', ''));
            value = parseFloat(value);
            if (_length < decimal)
                value = parseFloat(value + '0');
            if (value == 0)
                value = (showZero ? '0.00' : '');
            if (!keep) {
                if (value != 0)
                    value = value.toFixed(decimal).replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
            }
            {
                value = parseFloat(_digit);
                if (value >= 0)
                    value = value.toFixed(0).replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,") + '.' + (_length < decimal ? _decimal + '0' : _decimal);
            }
        }
        else {
            value = parseFloat(value);
            if (value == 0)
                value = (showZero ? '0' : '');
            if (value != 0)
                value = value.toFixed(decimal).replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
        }
    }
    else {
        value = (showZero ? '0.00' : '');
    }
    return value;
}
function MaskNumber(value, decimal = 0, showZero = false) {
    value = ReplaceAll(value, ',', '');
    if (value != '') {
        value = parseFloat(value);
        if (value == 0)
            value = (showZero ? '0' : '');
        if (value != 0)
            value = value.toFixed(decimal).replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    }
    else {
        value = (showZero ? '0' : '');
    }
    return value;
}
function MaskDate(value, format = "yyyy-MM-dd") {
    if (value != '') {
        var _now = new Date(value);
        var _yy = ('20' + _now.getFullYear()).slice(-4);
        var _mm = ('0' + (_now.getMonth() + 1)).slice(-2);
        var _dd = ('0' + _now.getDate()).slice(-2);
        format = format.toString().replace('yyyy', _yy);
        format = format.toString().replace('MM', _mm);
        format = format.toString().replace('dd', _dd);
        value = format;
    }
    else {
        value = '';
    }
    return value;
}
//function jump(h) {
//    var top = document.getElementById(h).offsetTop;
//    $('#mCSB_1_container').attr('style', 'position: relative; top: -' + top+'px; left: 0px; width: 100%;');   
//}
function colorToRGBCode(pColorName) {
    var _tempElement = $("<div>").css("color", pColorName).appendTo("body");
    var _rgb = _tempElement.css("color");
    //console.log('rgb : ' + _rgb);
    _tempElement.remove();
    return _rgb;
}
function colorToHexCode(pColorName) {
    var _rgb = colorToRGBCode(pColorName);
    var _hex = rgbToHex(_rgb);
    //console.log('hex : ' + _hex);
    return (pColorName == 0 ? '#ffffff' : _hex);
}
function rgbToHex(rgb) {
    // Extract the individual RGB components
    var rgbArray = rgb.match(/\d+/g);
    var r = parseInt(rgbArray[0]);
    var g = parseInt(rgbArray[1]);
    var b = parseInt(rgbArray[2]);
    // Convert each component to its hexadecimal equivalent
    var hexR = r.toString(16).padStart(2, "0");
    var hexG = g.toString(16).padStart(2, "0");
    var hexB = b.toString(16).padStart(2, "0");
    // Combine the components to form the hex color code
    var hexColor = "#" + hexR + hexG + hexB;
    return hexColor;
}
function RadioValue(pObject) {
    var _ret = '';
    $('input[name=' + pObject + ']').each(function (index, obj) {
        if ($('#' + obj.id).prop('checked') == true)
            _ret = $('#' + obj.id).val();
    });
    return _ret;
}
function ajaxPostData(pData) {
    try {
        const jsonObject = JSON.parse(pData);
        if (typeof (pData) === 'string') {
            // pData : var _string = `{ "id": "String" }`;
            return JSON.stringify(JSON.stringify(JSON.parse(pData)));
        }
        else if (typeof (pData) === 'object') {
            // pData : var _json = { "id": "JSON" };
            return JSON.stringify(JSON.stringify(pData));
        }
    }
    catch (error) {
        if (pData.length > 0) {
            // pData : $('#frmCondition')
            const _formData = $('#' + pData.attr('id')).serializeArray();
            const _jsonData = {};
            $(_formData).each(function (index, obj) {
                _jsonData[obj.name] = obj.value;
            });
            return JSON.stringify(JSON.stringify(_jsonData));
        }
        else {
            //console.log(pData);
            var _jObject = JSON.stringify(JSON.stringify(pData));
            if (_jObject === '"{}"') {
                // pData : var _formData = new FormData();
                return JSON.stringify(JSON.stringify(Object.fromEntries(pData)));
            }
            else {
                // pData :  var _object = {};   _object.id = "Object.id";
                return _jObject;
            }
        }
    }
}
