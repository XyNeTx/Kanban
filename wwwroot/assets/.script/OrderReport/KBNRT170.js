$(document).ready(function () {
    xAjax.Post({
        url: 'KBNRT170/Initial',
        then: function (result) {
            //console.log(result);
            $.each(result.data, function (i, v) {
                $("#F_SupFrom").append($("<option>", { value: v.F_Supplier, text: v.F_Supplier }, "</option>"));
            });
            $.each(result.data2, function (i, v) {
                $("#F_CycleFrom").append($("<option>", { value: v, text: v }, "</option>"));
                $("#F_CycleTo").append($("<option>", { value: v, text: v }, "</option>"));
            });
            $("#SupFromBlank").hide();
            $("#CycleFromBlank").hide();
            $("#CycleToBlank").hide();
            xSplash.hide();
        },
        error: function (result) {
            console.error("Initial Error from Receive Special Report");
            xSplash.hide();
        },
    });

    $("#OrderDiv").click(function () {
        $("#F_CycleFrom").val("");
        $("#F_CycleTo").val("");
        $("#F_CycleFrom").attr("disabled", true);
        $("#F_CycleTo").attr("disabled", true);
    });
    $("#DeliveryDiv").click(function () {
        $("#F_CycleFrom").attr("disabled", false);
        $("#F_CycleTo").attr("disabled", false);
    });

    xAjax.onChange("#F_SupFrom , #F_DateFrom, #F_DateTo", function () {
        var supFrom = $("#F_SupFrom").val();
        var typeDate = $("input[type='radio'][name='radDate']:checked").val();
        var dateFrom = $("#F_DateFrom").val().replaceAll('-', '');
        var dateTo = $("#F_DateTo").val().replaceAll('-', '');
        console.log(supFrom);
        console.log(typeDate);
        xAjax.Post({
            url: 'KBNRT170/OnChange',
            data: {
                supFrom: supFrom,
                typeDate: typeDate,
                dateFrom: dateFrom,
                dateTo:dateTo
            },
            then: function (result) {
                //console.log(result);
                $("#F_CycleFrom").empty();
                $("#F_CycleTo").empty();
                $("#F_CycleFrom").append($("<option id='CycleFromBlank'></option>"));
                $("#F_CycleTo").append($("<option id='CycleToBlank'></option>"));

                $.each(result.data, function (i, v) {
                    $("#F_CycleFrom").append($("<option>", { value: v, text: v }, "</option>"));
                    $("#F_CycleTo").append($("<option>", { value: v, text: v }, "</option>"));
                });
                $("#CycleFromBlank").hide();
                $("#CycleToBlank").hide();
            },
            error: function (result) {
                console.error(result);
            }
        });
    });
});