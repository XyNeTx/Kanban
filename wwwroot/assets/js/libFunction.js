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
        return "don't know";
    }
}


function ReplaceAll(pValue = '', pSearch = '', pReplace = '') {
    return pValue.replace(new RegExp(pSearch, 'g'), pReplace);
}
function replaceall(pValue = '', pSearch = '', pReplace = '') {
    return ReplaceAll(pValue, pSearch, pReplace);
}


function MaskCurrency(value, decimal = 2, showZero = false) {
    value = ReplaceAll(value, ',', '');
    if (value != '') {
        value = parseFloat(value);
        if (value == 0) value = (showZero ? '0.00' : '');
        if (value != 0) value = value.toFixed(decimal).replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    } else {
        value = (showZero ? '0.00' : '');
    }
    return value;
}

function MaskNumber(value, decimal = 0, showZero = false) {
    value = ReplaceAll(value, ',', '');
    if (value != '') {
        value = parseFloat(value);
        if (value == 0) value = (showZero ? '0' : '');
        if (value != 0) value = value.toFixed(decimal).replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    } else {
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
    } else {
        value = '';
    }
    return value;
}





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










function ajaxPostData(pData) {
    try {
        const jsonObject = JSON.parse(pData);

        if (typeof (pData) === 'string') {
            // pData : var _string = `{ "id": "String" }`;
            return JSON.stringify(JSON.stringify(JSON.parse(pData)));

        } else if (typeof (pData) === 'object') {
            // pData : var _json = { "id": "JSON" };
            return JSON.stringify(JSON.stringify(pData));
        }

    } catch (error) {

        if (pData.length > 0) {
            // pData : $('#frmCondition')

            const _formData = $('#' + pData.attr('id')).serializeArray();
            const _jsonData = {};

            $(_formData).each(function (index, obj) {
                _jsonData[obj.name] = obj.value;
            });
            return JSON.stringify(JSON.stringify(_jsonData));

        } else {
            //console.log(pData);
            var _jObject = JSON.stringify(JSON.stringify(pData));

            if (_jObject === '"{}"') {
                // pData : var _formData = new FormData();
                return JSON.stringify(JSON.stringify(Object.fromEntries(pData)));

            } else {
                // pData :  var _object = {};   _object.id = "Object.id";
                return _jObject;
            }

        }

    }


}