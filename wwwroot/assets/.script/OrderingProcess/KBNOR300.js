$(document).ready(async function () {

    //initial = async function () {

    //    var _dt = await xAjax.ExecuteJSON({
    //        data: {
    //            "Module": "[exec].[spKBNOR300]",
    //            "@OrderType": "U",
    //            "@Plant": ajexHeader.Plant,
    //            "@UserCode": ajexHeader.UserCode
    //        },
    //    });
    //    if (_dt.rows[0].KBNOR310 == 0) {
    //        $('#btnKBNOR310').attr('readonly', true);
    //        $('#btnKBNOR310').attr('class', 'btn btn-light');
    //    }
    //    if (_dt.rows[0].KBNOR320 == 0) {
    //        $('#btnKBNOR320').attr('readonly', true);
    //        $('#btnKBNOR320').attr('class', 'btn btn-light');
    //    }
    //    if (_dt.rows[0].KBNOR321 == 0) {
    //        $('#btnKBNOR321').attr('readonly', true);
    //        $('#btnKBNOR321').attr('class', 'btn btn-light');
    //    }
    //    if (_dt.rows[0].KBNOR330 == 0) {
    //        $('#btnKBNOR330').attr('readonly', true);
    //        $('#btnKBNOR330').attr('class', 'btn btn-light');
    //    }
    //    if (_dt.rows[0].KBNOR360 == 0) {
    //        $('#btnKBNOR360').attr('readonly', true);
    //        $('#btnKBNOR360').attr('class', 'btn btn-light');
    //    }
    //    if (_dt.rows[0].KBNOR370 == 0) {
    //        $('#btnKBNOR370').attr('readonly', true);
    //        $('#btnKBNOR370').attr('class', 'btn btn-light');
    //    } 

    //    let _processCk = _xLib.GetProcessCookie();
    //    //console.log(_processCk);
    //    if (_processCk != null) {
    //        _processCk.forEach(function (item) {
    //            let _btnName = `btn${item}`;
    //            $(`#${_btnName}`).removeClass('btn-success').css('background-color', '#faa2c1');
    //            $(`#${_btnName}`).css("color", "white");
    //        });
    //    }

        
    //}
    //initial();
    await Onload();
    xSplash.hide();


    xAjax.onClick('btnKBNOR310', async function (e) {
        //var _dt = await xAjax.ExecuteJSON({
        //    data: {
        //        "Module": "[exec].[spKBNOR300_310]",
        //        "@OrderType": "U",
        //        "@Plant": ajexHeader.Plant,
        //        "@UserCode": ajexHeader.UserCode
        //    },
        //});
        ////console.log(_dt.rows);
        //if (_dt.rows[0].F_Value2 == 0) MsgBox("กรุณารอการยืนยันข้อมูลจากหน่วยงาน CCR", MsgBoxStyle.Information, "INTERFACE DATA");
        //if (_dt.rows[0].F_Value2 != 0) {
            let _redirect = e.target.id.replace('btn', '');
            _xLib.SetProcessCookie(_redirect);
            xAjax.redirect('KBNOR310');
        //}
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

async function Onload() {
    var processDate = "";
    _xLib.AJAX_Get("/api/KBNOR310/Onload", null,
        async function (success) {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            processDate = moment(success.data[0].ProcessDate.slice(0, 10), "YYYY-MM-DD").format("YYYYMMDD") + success.data[0].ProcessShift;
            //console.log(processDate);
        }
    );

    return _xLib.AJAX_Get("/api/KBNOR300/Onload", null,
        async function (success) {
            console.log(success);
            $("div[class='card-body']").find("button").each(function (index, item) {
                let _btnName = $(item).attr('id').split("btn")[1]
                //console.log(_btnName);
                if (!success.data.some(x => x == _btnName)) {
                    $(item).prop("disabled", true)
                }
            });
            //console.log(success.param[0].f_Value3 === processDate);
            //console.log(success.param[0].f_Value3);
            //console.log(processDate);
            //console.log(success.param[1].f_Value2);
           
            switch (success.param[1].f_Value2) {
                case 0: {
                    if (success.param[0].f_Value3 === processDate) {
                        console.log("0");
                        $("div[class='card-body']").find("button").each(function (index, item) {
                            //console.log($(item).attr('id'));
                            $(item).removeClass("btn-success");
                            $(item).addClass("text-dark fw-bolder");
                            $(item).css("background-color", "Chocolate")
                        });
                        break;
                    }
                    else {
                        break;
                    }

                }
                case 1: {
                    $("#btnKBNOR310").removeClass("btn-success");
                    $("#btnKBNOR310").addClass("text-dark fw-bolder");
                    $("#btnKBNOR310").css("background-color", "Chocolate")
                    break;
                }
                case 2: {
                    $("#btnKBNOR310").removeClass("btn-success");
                    $("#btnKBNOR310").addClass("text-dark fw-bolder");
                    $("#btnKBNOR310").css("background-color", "Chocolate")
                    $("#btnKBNOR320").removeClass("btn-success");
                    $("#btnKBNOR320").addClass("text-dark fw-bolder");
                    $("#btnKBNOR320").css("background-color", "Chocolate")
                    break;
                }
                case 4: {
                    console.log("4");
                    $("#btnKBNOR310").removeClass("btn-success");
                    $("#btnKBNOR310").addClass("text-dark fw-bolder");
                    $("#btnKBNOR310").css("background-color", "Chocolate")
                    $("#btnKBNOR320").removeClass("btn-success");
                    $("#btnKBNOR320").addClass("text-dark fw-bolder");
                    $("#btnKBNOR320").css("background-color", "Chocolate")
                    $("#btnKBNOR321").removeClass("btn-success");
                    $("#btnKBNOR321").addClass("text-dark fw-bolder");
                    $("#btnKBNOR321").css("background-color", "Chocolate")
                    $("#btnKBNOR330").removeClass("btn-success");
                    $("#btnKBNOR330").addClass("text-dark fw-bolder");
                    $("#btnKBNOR330").css("background-color", "Chocolate")
                    break;
                }
                default: {
                    break;
                }
            }
        },
    )
}