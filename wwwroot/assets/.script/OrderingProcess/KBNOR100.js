var _CookieProcessDate = _xLib.GetCookie("processDate");
var _CookieLoginDate = _xLib.GetCookie("loginDate");
$(document).ready(function () {



    $("#txtProcessDate").val(_CookieProcessDate.substring(0, 4) + "-" + _CookieProcessDate.substring(5, 7) + "-" + _CookieProcessDate.substring(8, 10));
    var shift = _CookieProcessDate.substring(10, 11) == "D" ? "1 - Day Shift" : "2 - Night Shift";
    $("#txtProcessShift").val(shift);

    initial = async function () {
        let _processCk = await _xLib.GetProcessCookie();
        console.log(_processCk);
        if (_processCk != null) {
            _processCk.forEach(function (item) {
                let _btnName = `btn${item}`;
                $(`#${_btnName}`).removeClass('btn-success').css('background-color', '#faa2c1');
                $(`#${_btnName}`).css("color", "white");
            });
        }

        await _xLib.AJAX_Get('/api/KBNOR100/Onload', { dateShift: _xLib.GetCookie("loginDate").replaceAll("-", "") },
            function (data) {
                console.log(data);
            },
            function (error) {
                if (error.responseJSON.cmd) {
                    error.responseJSON.cmd.forEach(function (item) {
                        let _btnName = `btn${item}`;
                        $(`#${_btnName}`).prop('disabled', true);
                    });
                }
            }
        );

        xSplash.hide();
    }
    initial();

    xAjax.onClick('btnKBNOR110', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });


    xAjax.onClick('btnKBNOR120', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR121', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR122', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR123', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });


    xAjax.onClick('btnKBNOR130', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });



    xAjax.onClick('btnKBNOR140', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });


    xAjax.onClick('btnKBNOR150', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR150EX', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });


    xAjax.onClick('btnKBNOR160', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });


});