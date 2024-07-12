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
    $("#widthProgressBar").css("width", "0%").attr("aria-valuenow", 0).removeClass("bg-success");
    $("#btnCalculate").prop("disabled", true);
    const _interval = setInterval(function () {
        _xLib.AJAX_Get("/api/KBNOR120/GetProcessCount", '',
            async function (success) {
                if (success.status == "200") {
                    if (success.data == 100) {
                        await clearInterval(_interval);
                    }
                    $("#widthProgressBar").css("width", success.data + "%").attr("aria-valuenow", success.data)
                    $("#spanProgressBar").text("Calculating Normal Order " + success.data + "%");
                }
            }
        );
    }, 3000);

    var _url = "/api/KBNOR120/Process_Order";

    if ($("#txtProcessForShift").val().includes("Night")) {
        _url = "/api/KBNOR120/Process_Order_Night";
    }

    _xLib.AJAX_Get(_url, { sDate: $("#txtProcessForDate").val() },
        function (success) {
            if (success.status == "200") {
                //xSwal.success("Success", "Order Processed Successfully");
                $("#widthProgressBar").css("width", "100%").attr("aria-valuenow", 100).addClass("bg-success");
                $("#spanProgressBar").text("Calculating Normal Order Completed");
                _xLib.AJAX_Get("/api/KBNOR120/Calculate", '',
                    function (success) {
                        if (success.status == "200") {
                            xSplash.hide();
                            $("#btnCalculate").prop("disabled", false);
                            $("#divProgressBar").css("visibility", "hidden");
                            xSwal.success("Success", "Data Calculated Successfully");
                        }
                    }
                );
            }
        },
        function (error) {
            $("#btnCalculate").prop("disabled", false);
            $("#divProgressBar").css("visibility", "hidden");
            xSwal.error("Error", "Error in Processing Order");
            clearInterval(_interval);
        }
    );
    

});