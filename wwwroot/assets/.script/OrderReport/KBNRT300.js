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
                //console.log(result);
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

    xAjax.onClick("#ReportBtn", function () {
        var supFrom = $("#F_SupFrom").val().trim();
        var supTo = $("#F_SupTo").val().trim();
        var kbnFrom = $("#F_KBNFrom").val().trim();
        var kbnTo = $("#F_KBNTo").val().trim();
        var dateFrom = $("#F_DeliDateFrom").val().trim().replaceAll("-", "");
        var dateTo = $("#F_DeliDateTo").val().trim().replaceAll("-", "");
        var shiftFrom = $("#F_ShiftFrom").val().trim();
        var shiftTo = $("#F_ShiftTo").val().trim();
        var tripFrom = $("#F_TripFrom").val().trim();
        var tripTo = $("#F_TripTo").val().trim();
        var kbnType = $("input[name='KBNRadio']:checked").val();
        xAjax.Post({
            url: "KBNRT300/ClickReport",
            data: {
                supFrom: supFrom,
                supTo: supTo,
                kbnFrom: kbnFrom,
                kbnTo: kbnTo,
                dateFrom: dateFrom,
                dateTo: dateTo,
                shiftFrom: shiftFrom,
                shiftTo: shiftTo,
                tripFrom: tripFrom,
                tripTo: tripTo,
                kbnType: kbnType
            },
            then: function (result) {
                console.log(result);
                if (result.status === "200") {
                    var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
                    var reportUrl = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx?/KB3/";
                    window.location.href = reportUrl + filename + kbnType + '&SupFrom=' + supFrom + '&SupTo=' + supTo + '&UserName=' + result.data
                        + '&KBNFrom=' + kbnFrom + '&KBNTo=' + kbnTo + '&DateFrom=' + dateFrom + '&DateTo=' + dateTo
                        + '&ShiftFrom=' + shiftFrom + '&ShiftTo=' + shiftTo + '&TripFrom=' + tripFrom + '&TripTo=' + tripTo + '&KBNType=' + kbnType;
                }
                else {
                    return xSwal.error(result.title, result.message);
                }
            },
            error: function (result) {
                console.error(result);
            }
        });
    });
});