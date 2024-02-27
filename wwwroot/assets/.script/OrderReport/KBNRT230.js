$(document).ready(async function () {
    await initial();
});

function hideBlank() {
    $('#CustomerFromBlank').hide();
    $('#CustomerToBlank').hide();
}
function initial() {
    return xAjax.Post({
        url: 'KBNRT230/F_Customer_TB_MS',
        then: function (result) {
            $.each(result.data, async function (i, v) {
                $("#F_CustomerFrom").append($("<option>", { value: v, text: v }, "</option>"));
                $("#F_CustomerTo").append($("<option>", { value: v, text: v }, "</option>"));
                await hideBlank();
                await xSplash.hide();
            });
        },
        error: function (result) {
            console.error("Initial Error");
        },
    });
}
xAjax.onChange("#F_CustomerFrom", function () {
    var cusFrom = $("#F_CustomerFrom").val().trim();
    var cusTo = $("#F_CustomerTo").val().trim();
    console.log(cusFrom, cusTo);
    if (cusFrom != null || cusFrom != "") {
        if (cusTo == "" || cusTo == null) {
            $("#F_CustomerTo").val($("#F_CustomerFrom").val()).change();
        }
    }
});
async function ReportClick() {
    var cusFrom = $("#F_CustomerFrom").val().trim();
    var cusTo = $("#F_CustomerTo").val().trim();
    var dateFrom = $("#F_DateFrom").val().trim().replaceAll("-", "");
    var dateTo = $("#F_DateTo").val().trim().replaceAll("-", "");
    if (cusFrom === "" || cusFrom === undefined || cusTo === "" || cusTo === undefined) {
        return xSwal.error("Invalid Input Customer", "Please Select Customer From and Customer To");
    }
    if (cusFrom > cusTo) {
        return xSwal.error("Select Customer ERROR!", "Please Select Customer From less than Customer To")
    }
    if (dateFrom > dateTo) {
        return xSwal.error("Select Order Date ERROR!", "Please Select Date From less than Date To")
    }
    xAjax.Post({
        url: 'KBNRT230/OnReportClick',
        data: {
            cusFrom: cusFrom,
            cusTo: cusTo,
            dateFrom: dateFrom,
            dateTo: dateTo
        },
        then: function (result) {
            if (result.status === "200") {
                var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
                var reportUrl = "http://hmmt-app03/Reportserver/report/KB3/";
                window.location.href = reportUrl + filename + '?HostName=' + result.data2 + '&UserName=' + result.data;
            }
            else {
                return xSwal.Error(result.title, result.message);
            }
        },
        error: function (result) {
            console.error("Initial Error " + result);
        },
    });
}
xAjax.onClick("#ReportBtn", async function () {
    await ReportClick();
});