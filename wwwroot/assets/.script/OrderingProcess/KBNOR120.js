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
    $("#divProgressBar").css("visibility", "visible");

    const _interval = setInterval(function () {
        _xLib.AJAX_Get("/api/KBNOR120/GetProcessCount", '',
            async function (success) {
                if (success.status == "200") {
                    if (success.data == 100) {
                        await clearInterval(_interval);
                    }
                    $("#widthProgressBar").css("width", success.data + "%").attr("aria-valuenow", success.data);
                    $("#spanProgressBar").text("Calculating Normal Order " + success.data + "%");
                }
            }
        );
    }, 5000);

    _xLib.AJAX_Get("/api/KBNOR120/Process_Order", { sDate: $("#txtProcessForDate").val() },
        function (success) {
            if (success.status == "200") {
                xSwal.success("Success", "Order Processed Successfully");
                _xLib.AJAX_Get("/api/KBNOR120/Calculate", '',
                    function (success) {
                        if (success.status == "200") {
                            xSplash.hide();
                            xSwal.success("Success", "Data Calculated Successfully");
                        }
                    }
                );
            }
        }
    );

});