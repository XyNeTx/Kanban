$(document).ready(function () {

    initial = async function () {
        xSplash.show();

        await _xLib.AJAX_Get('/xapi/GetProcessDate', { dateShift: _xLib.GetCookie("loginDate").replaceAll("-", "") }, function (data) {
            console.log(data[0].Column1);
            var _Date = data[0].Column1.slice(0, 4) + '-' + data[0].Column1.slice(4, 6) + '-' + data[0].Column1.slice(6, 8) + data[0].Column1.slice(8, 9);
            document.cookie = `processDate=${_Date}`;
        });

        //console.log('xXx');

        xSplash.hide();
    }
    initial();




});