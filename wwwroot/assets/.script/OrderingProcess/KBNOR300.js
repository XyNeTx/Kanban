$(document).ready(function () {

    initial = async function () {

        var _dt = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNOR300]",
                "@OrderType": "U",
                "@Plant": ajexHeader.Plant,
                "@UserCode": ajexHeader.UserCode
            },
        });
        if (_dt.rows[0].KBNOR310 == 0) {
            $('#btnKBNOR310').attr('readonly', true);
            $('#btnKBNOR310').attr('class', 'btn btn-light');
        }
        if (_dt.rows[0].KBNOR320 == 0) {
            $('#btnKBNOR320').attr('readonly', true);
            $('#btnKBNOR320').attr('class', 'btn btn-light');
        }
        if (_dt.rows[0].KBNOR321 == 0) {
            $('#btnKBNOR321').attr('readonly', true);
            $('#btnKBNOR321').attr('class', 'btn btn-light');
        }
        if (_dt.rows[0].KBNOR330 == 0) {
            $('#btnKBNOR330').attr('readonly', true);
            $('#btnKBNOR330').attr('class', 'btn btn-light');
        }
        if (_dt.rows[0].KBNOR360 == 0) {
            $('#btnKBNOR360').attr('readonly', true);
            $('#btnKBNOR360').attr('class', 'btn btn-light');
        }
        if (_dt.rows[0].KBNOR370 == 0) {
            $('#btnKBNOR370').attr('readonly', true);
            $('#btnKBNOR370').attr('class', 'btn btn-light');
        } 

        let _processCk = _xLib.GetProcessCookie();
        //console.log(_processCk);
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


    xAjax.onClick('btnKBNOR310', async function (e) {
        var _dt = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNOR300_310]",
                "@OrderType": "U",
                "@Plant": ajexHeader.Plant,
                "@UserCode": ajexHeader.UserCode
            },
        });
        //console.log(_dt.rows);
        if (_dt.rows[0].F_Value2 == 0) MsgBox("กรุณารอการยืนยันข้อมูลจากหน่วยงาน CCR", MsgBoxStyle.Information, "INTERFACE DATA");
        if (_dt.rows[0].F_Value2 != 0) {
            let _redirect = e.target.id.replace('btn', '');
            _xLib.SetProcessCookie(_redirect);
            xAjax.redirect('KBNOR310');
        }
    });


    xAjax.onClick('btnKBNOR320', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });


    xAjax.onClick('btnKBNOR321', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });


    xAjax.onClick('btnKBNOR330', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });


    xAjax.onClick('btnKBNOR360', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });


    xAjax.onClick('btnKBNOR370', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });




});