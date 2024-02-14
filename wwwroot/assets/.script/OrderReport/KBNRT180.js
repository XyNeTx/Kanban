$(document).ready(function () {
    xAjax.Post({
        url: 'KBNRT180/Initial',
        then: function (result) {
            //console.log(result);
            $.each(result.data, function (i, v) {
                $("#F_SupFrom").append($("<option>", { value: v.F_Supplier, text: v.F_Supplier }, "</option>"));
                $("#F_SupTo").append($("<option>", { value: v.F_Supplier, text: v.F_Supplier }, "</option>"));
            });
            $("#SupFromBlank").hide();
            xSplash.hide();
        },
        error: function (result) {
            console.error("Initial Error from Receive Special Report");
            xSplash.hide();
        },
    });

    $("#F_DeliDateFrom").attr("disabled", true);
    $("#F_DeliDateTo").attr("disabled", true);

    $("#OrderDiv").click(function () {
        $("#F_DeliDateFrom").attr("disabled", true);
        $("#F_DeliDateTo").attr("disabled", true);
        $("#F_OrderDateFrom").attr("disabled", false);
        $("#F_OrderDateTo").attr("disabled", false);
    });
    $("#DeliveryDiv").click(function () {
        $("#F_OrderDateFrom").attr("disabled", true);
        $("#F_OrderDateTo").attr("disabled", true);
        $("#F_DeliDateFrom").attr("disabled", false);
        $("#F_DeliDateTo").attr("disabled", false);
    });

});