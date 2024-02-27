$(document).ready(async function () {
    await xSplash.hide();
});

async function ReportClick() {
    var dateFrom = $("#F_DateFrom").val().trim().replaceAll("-", "");
    var dateTo = $("#F_DateTo").val().trim().replaceAll("-", "");
    if (dateFrom > dateTo) {
        return xSwal.error("Select Order Date ERROR!", "Please Select Date From less than Date To")
    }
    xAjax.Post({
        url: 'KBNRT240/OnReportClick',
        data: {
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