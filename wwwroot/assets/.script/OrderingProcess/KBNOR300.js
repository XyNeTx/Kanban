$(document).ready(async function () {
    await Onload();
    xSplash.hide();


    xAjax.onClick('btnKBNOR310', async function (e) {
            let _redirect = e.target.id.replace('btn', '');
            _xLib.SetProcessCookie(_redirect);
            xAjax.redirect('KBNOR310');
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
    await _xLib.AJAX_Get("/api/KBNOR310/Onload", null,
        async function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            processDate = moment(success.data[0].ProcessDate.slice(0, 10), "YYYY-MM-DD").format("YYYYMMDD") + success.data[0].ProcessShift;
            $("#txtProcessDate").val(moment(success.data[0].ProcessDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));
            $("#txtProcessShift").val(success.data[0].CurrentShift == "D" ? "1-Day Shift" : "2-Night Shift");
            $("#txtProcessDateLast").val(moment(success.data[0].LastProcessDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));
            $("#txtProcessShiftLast").val(success.data[0].LastProcessShift == "D" ? "1-Day Shift" : "2-Night Shift");
            //sessionStorage.setItem("loginDate", processDate);
            console.log(processDate);
        }
    );

    return await _xLib.AJAX_Get("/api/KBNOR300/Onload", null,
        async function (success) {
            console.log(success);
            $("div[class='card-body']").find("button").each(function (index, item) {
                let _btnName = $(item).attr('id').split("btn")[1]
                //console.log(_btnName);
                if (!success.data.some(x => x == _btnName)) {
                    $(item).prop("disabled", true)
                }
            });

            switch (success.param[1].f_Value2) {
                case 0: {
                    if (success.param[0].f_Value3 === processDate) {
                        console.log("0");
                        $("div[class='card-body']").find("button").each(function (index, item) {
                            console.log($(item).attr('id'));
                            $(item).removeClass("btn-success");
                            $(item).addClass("text-dark fw-bolder");
                            $(item).css("background-color", "Chocolate")
                            if ($(item).attr('id') === "btnKBNOR321" || $(item).attr('id') === "btnKBNOR370") {
                                $(item).prop("disabled", false);
                            }
                            else {
                                $(item).prop("disabled", true);
                            }
                        });
                        break;
                    }
                    else {
                        break;
                    }
                    //$("#btnKBNOR321").prop("disabled")
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
                    //console.log("4");
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