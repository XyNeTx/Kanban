$(document).ready(function () {

    $("#txtProcessDate").val($("#nr_Date").text());
    $("#txtProcessShift").val($("#nr_Shift").text());

    initial = async function () {
        let _processCk = _xLib.GetProcessCookie();
        console.log(_processCk);
        if (_processCk != null) {
            _processCk.forEach(function (item) {
                let _btnName = `btn${item}`;
                $(`#${_btnName}`).removeClass('btn-success').css('background-color', '#faa2c1');
                $(`#${_btnName}`).css("color", "white");
            });
        }

        xSplash.hide();
    }
    initial();


    xAjax.onClick('btnKBNOR410', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });


    xAjax.onClick('btnKBNOR420', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });


    xAjax.onClick('btnKBNOR440', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });


    xAjax.onClick('btnKBNOR450', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });


    xAjax.onClick('btnKBNOR460', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });


    xAjax.onClick('btnKBNOR460EX', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });


    xAjax.onClick('btnKBNOR470', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });




});