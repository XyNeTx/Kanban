$(document).ready(function () {



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