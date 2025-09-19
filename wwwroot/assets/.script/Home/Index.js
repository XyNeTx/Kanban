$(document).ready(function () {

    initial = async function () {
        xSplash.show();

        //_xLib.SetCookie("loginDate", sessionStorage.getItem("loginDate"));
        //_xLib.SetCookie("isDev", sessionStorage.getItem("isDev"));
        //_xLib.SetCookie("plantCode", sessionStorage.getItem("plantCode"));

        //console.log("loginDate Cookie: " + _xLib.GetCookie("loginDate") == "null");
        //console.log("isDev Cookie: " + _xLib.GetCookie("isDev") == "null");
        //console.log("plantCode Cookie: " + _xLib.GetCookie("plantCode") == "null");

        //if(_xLib.GetCookie("loginDate") == null || _xLib.GetCookie("loginDate") == "null"
        //    // || _xLib.GetCookie("isDev") == null || _xLib.GetCookie("isDev") == "null"
        //    // || _xLib.GetCookie("plantCode") == null || _xLib.GetCookie("plantCode") == "null")
        //    )
        //{
        //    window.location.href = "/Login";
        //    console.log("No loginDate cookie");
        //    return;
        //}

        await _xLib.AJAX_Get('/xapi/GetProcessDate', { dateShift: _xLib.GetCookie("loginDate").replaceAll("-", "") }, function (data) {
            console.log(data[0].Column1);
            var _Date = data[0].Column1.slice(0, 4) + '-' + data[0].Column1.slice(4, 6) + '-' + data[0].Column1.slice(6, 8) + data[0].Column1.slice(8, 9);
            document.cookie = `processDate=${_Date}`;
        });

        //console.log('xXx');

        console.log(localStorage.getItem("TOKEN"));
        xSplash.hide();
    }
    initial();




});