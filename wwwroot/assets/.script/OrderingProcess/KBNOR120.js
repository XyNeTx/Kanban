$(document).ready(function () {
    xAjax.onClick('btnExit', function () {
        xAjax.redirect('KBNOR100');
    });

    _xLib.AJAX_Get("/api/KBNOR120/OnLoad", { Shift : $("#txtProcessShift").val() },
        function (success) {
            if (success.status == "200") {
                xSplash.hide();
            }
        }
    );
});
$("#btnCalculate").click(function () {
    _xLib.AJAX_Get("/api/KBNOR120/Process_Order", { sDate: $("#txtProcessForDate").val() },
        function (success) {
            if (success.status == "200") {
                $("#divProgressBar").css("visibility", "visible");
                setInterval(function () {
                    _xLib.AJAX_Get("/api/KBNOR120/GetProcessCount", '',
                        function (success) {
                            if (success.status == "200") {
                                if(success.data == 100) {
                                    $("#divProgressBar").css("visibility", "hidden");
                                    clearInterval();

                                    _xLib.AJAX_Get("/api/KBNOR120/Calculate", '',
                                        function (success) {
                                            if (success.status == "200") {
                                                xSplash.hide();
                                                xSwal.success("Success", "Data Calculated Successfully");
                                            }
                                        }
                                    );
                                }
                                $("#widthProgressBar").css("width", success.data + "%").attr("aria-valuenow", success.data);
                                $("#spanProgressBar").text("Calculating Data" + success.data + "%");
                            }
                        }
                    );
                },1000);
            }
        }
    );
});