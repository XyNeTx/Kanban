$(document).ready(function () {

    function hideBlank() {
        $('#supFromBlank').hide();
        $('#supToBlank').hide();
        $('#KBNFromBlank').hide();
        $('#KBNToBlank').hide();
        $('#TripFromBlank').hide();
        $('#TripToBlank').hide();
        $('#ShiftFromBlank').hide();
        $('#ShiftToBlank').hide();
    }

    xAjax.Post({
        url: 'KBNRT160/Initial',
        then: function (result) {
            //console.log(result);
            $.each(result.data, function (i, v) {
                $("#F_SupFrom").append($("<option>", { value: v.Sup_CD, text: v.Sup_CD }, "</option>"));
                $("#F_SupTo").append($("<option>", { value: v.Sup_CD, text: v.Sup_CD }, "</option>"));
            });
            $.each(result.data2, function (i, v) {
                $("#F_KBNFrom").append($("<option>", { value: v.F_Sebango, text: v.F_Sebango }, "</option>"));
                $("#F_KBNTo").append($("<option>", { value: v.F_Sebango, text: v.F_Sebango }, "</option>"));
            });
            hideBlank();
            xSplash.hide();
        },
        error: function (result) {
            console.error("Initial Error from Receive Special Report");
            xSplash.hide();
        },
    });


    xAjax.onChange("#F_SupFrom, #F_SupTo", function () {
        if ($("#F_SupTo").val() === null || $("#F_SupTo").val() === "") {
            $("#F_SupTo").val($("#F_SupFrom").val()).change();
        }
        var supFrom = $("#F_SupFrom").val();
        var supTo = $("#F_SupTo").val();
        xAjax.Post({
            url: 'KBNRT160/OnSupplierChange',
            data: {
                supFrom: supFrom,
                supTo: supTo
            },
            then: function (result) {
                //console.log(result);
                $("#F_KBNFrom").empty();
                $("#F_KBNTo").empty();
                $("#F_KBNFrom").append($("<option id='KBNFromBlank'></option>"));
                $("#F_KBNTo").append($("<option id='KBNToBlank'></option>"));
                $.each(result.data, function (i, v) {
                    $("#F_KBNFrom").append($("<option>", { value: v.F_Sebango, text: v.F_Sebango }, "</option>"));
                    $("#F_KBNTo").append($("<option>", { value: v.F_Sebango, text: v.F_Sebango }, "</option>"));
                });
                hideBlank();
            },
            error: function (result) {
                console.error(result);
            },
        });
    });

    xAjax.onChange("#F_DeliDateFrom, #F_DeliDateTo", function () {
        var dateFrom = $("#F_DeliDateFrom").val().replaceAll('-', '');
        var dateTo = $("#F_DeliDateTo").val().replaceAll('-', '');
        var supFrom = $("#F_SupFrom").val();
        var supTo = $("#F_SupTo").val();
        xAjax.Post({
            url: "KBNRT190/OnDeliDateChange",
            data: {
                dateFrom: dateFrom,
                dateTo: dateTo,
                supFrom: supFrom,
                supTo: supTo
            },
            then: function (result) {
                console.log(result);
                $("#F_TripFrom").empty();
                $("#F_TripTo").empty();
                $("#F_TripFrom").append($("<option id='TripFromBlank'></option>"));
                $("#F_TripTo").append($("<option id='TripToBlank'></option>"));
                $.each(result.data, function (i, v) {
                    $("#F_TripFrom").append($("<option>", { value: v, text: v }, "</option>"));
                    $("#F_TripTo").append($("<option>", { value: v, text: v }, "</option>"));
                });
                hideBlank();
            },
            error: function (result) {
                console.error(result);
            }
        });
    });

    xAjax.onChange("#F_TripFrom, #F_TripTo", function () {
        if ($("#F_TripTo").val() === null || $("#F_TripTo").val() === "") {
            $("#F_TripTo").val($("#F_TripFrom").val()).change();
        }
    });
});