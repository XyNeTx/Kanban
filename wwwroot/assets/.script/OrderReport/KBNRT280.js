$(document).ready(async function () {
    await xSplash.hide();
});
$("#ReportRadioDiv").on("click", async function () {
    var ReportChecked = await $("#ReportRadioDiv input[name='ReportRadio']:checked").val();
    console.log(ReportChecked);
    if (ReportChecked === "D") {
        $("#delayCheckBox").attr('disabled', true);
    }
    else if (ReportChecked === "S") {
        $("#delayCheckBox").attr('disabled', false);
    }
});
$("#ReportBtn").on("click", async function () {
    var ReportChecked = await $("#ReportRadioDiv input[name='ReportRadio']:checked").val();
    var OrderChecked = await $("#OrderRadioDiv input[name='OrderRadio']:checked").val();
    var DateFrom = await $("#F_DeliDateFrom").val().trim().replaceAll("-", "");
    var DateTo = await $("#F_DeliDateTo").val().trim().replaceAll("-", "");
    var DateFromShow = DateFrom.substring(6, 8) + "/" + DateFrom.substring(4, 6) + "/" + DateFrom.substring(0, 4);
    var DateToShow = DateTo.substring(6, 8) + "/" + DateTo.substring(4, 6) + "/" + DateTo.substring(0, 4);
    var DelayCheck = await $("#delayCheckBox").prop("checked");
    var url = "";
    if (DateFrom > DateTo) {
        return xSwal.Error("Error", "Please select Delivery Date from less than Delivery Date To");
    }
    if (ReportChecked === "S") {
        url = "KBNRT280/PrintReportSummary";
    }
    else if (ReportChecked === "D") {
        url = "KBNRT280/PrintReportDetail";
    }
    xAjax.Post({
        url: url,
        data: {
            orderChecked: OrderChecked,
            delayChecked: DelayCheck,
            dateFrom: DateFrom,
            dateTo: DateTo,
        },
        then: function (result) {
            console.log(result);
            if (result.status === "200") {
                var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
                var reportUrl = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx?/KB3/";
                if (ReportChecked === "S") { filename += "SUM" }
                else if (ReportChecked === "D") { filename += "DETAIL" }
                window.location.href = reportUrl + filename + '&userName=' + result.data + '&delayChecked=' + DelayCheck +
                    '&Plant=' + result.data2 + '&Type=' + OrderChecked + '&DateFrom=' + DateFromShow + '&DateTo=' + DateToShow;
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