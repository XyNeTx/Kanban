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
            $("#SupToBlank").hide();
            xSplash.hide();
        },
        error: function (result) {
            console.error("Initial Error from Receive Special Report");
            xSplash.hide();
        },
    });

    $("#F_DeliDateFrom").attr("disabled", true);
    $("#F_DeliDateTo").attr("disabled", true);

    xAjax.onChange("#F_SupFrom, #F_SupTo", function () {
        if ($("#F_SupTo").val() === null || $("#F_SupTo").val() === "") {
            $("#F_SupTo").val($("#F_SupFrom").val()).change();
        }
    });

    $("#OrderDiv").click(function () {
        var checked = $("input[name='radDate']:checked").val();
        if (checked === "Order") {
            $("#F_DeliDateFrom").attr("disabled", true);
            $("#F_DeliDateTo").attr("disabled", true);
            $("#F_OrderDateFrom").attr("disabled", false);
            $("#F_OrderDateTo").attr("disabled", false);
        }
    });
    $("#DeliveryDiv").click(function () {
        var checked = $("input[name='radDate']:checked").val();
        if (checked === "Delivery") {
            $("#F_OrderDateFrom").attr("disabled", true);
            $("#F_OrderDateTo").attr("disabled", true);
            $("#F_DeliDateFrom").attr("disabled", false);
            $("#F_DeliDateTo").attr("disabled", false);
        }
    });

    xAjax.onClick("#ReportBtn", function () {
        var supFrom = $("#F_SupFrom").val();
        var supTo = $("#F_SupTo").val();
        var checked = $("input[name='radDate']:checked").val();
        if (checked === "Order") {
            var dateFrom = $("#F_OrderDateFrom").val().replaceAll('-', '');
            var dateTo = $("#F_OrderDateTo").val().replaceAll('-', '');
            var url = "KBNRT180/OnReportBtnOrder";
        }
        else if (checked === "Delivery") {
            var dateFrom = $("#F_DeliDateFrom").val().replaceAll('-', '');
            var dateTo = $("#F_DeliDateTo").val().replaceAll('-', '');
            var url = "KBNRT180/OnReportBtnDelivery";
        }

        if (supFrom === "" || supFrom === undefined || supTo === "" || supTo === undefined) {
            return xSwal.error("Supplier Code is empty", "Please Select Supplier Code");
        }
        if (supFrom > supTo) {
            return xSwal.error("Invalid Input Supplier Code", "Please Select Supplier Code From less than Supplier Code To");
        }
        if (dateFrom > dateTo) {
            return xSwal.error("Invalid Input Date", "Please Select Date From less than Date To");
        }

        xAjax.Post({
            url: url,
            data: {
                supFrom: supFrom,
                supTo: supTo,
                dateFrom: dateFrom,
                dateTo: dateTo,
            },
            then: function (result) {
                if (result.status == 200) {
                    var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
                    var reportUrl = "http://hmmt-app03/Reportserver/report/KB3/";
                    window.location.href = reportUrl + filename + '?HostName=' + result.data2 + '&UserName=' + result.data
                        + '&UserCode=' + result.data3;
                }
                else {
                    return xSwal.error(result.title, result.message);
                }
            },
            error: function (result) {
                return console.error(result);
            }
        });
    });
});