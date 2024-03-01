$(document).ready(async function () {
    setTimeout(300);
    await ProdMonthChange();
});

function hideBlank() {
    $('#FlagFromBlank').hide();
    $('#FlagToBlank').hide();
}

xAjax.onChange("#F_ProdMonth", async function () {
    await ProdMonthChange();
});

function ProdMonthChange() {
    var prodMonth = $("#F_ProdMonth").val().trim();
    return xAjax.Post({
        url: 'KBNRT260/Display_Detail',
        data: { prodMonth: prodMonth },
        then: async function (result) {
            console.log(result);
            await $("#Revision").val(result.data[0].F_Revision);
            await $("#Version").val(result.data[0].F_Version);
            xSplash.hide();
        },
        error: function (result) {
            console.error("Initial Error", result);
        },
    });
}

xAjax.onClick("#ReportBtn", async function () {
    await ReportClick();
});

async function ReportClick() {
    var checkedValue = $("input[name='radio']:checked").val();
    var prodMonth = $("#F_ProdMonth").val().trim();
    var Revision = $("#Revision").val().trim();
    xAjax.Post({
        url: 'KBNRT260/OnReportClicked',
        data: {
            checkedValue: checkedValue,
            prodMonth: prodMonth
        },
        then: function (result) {
            if (result.status === "200") {
                var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
                var reportUrl = "http://hmmt-app03/Reportserver/report/KB3/";
                prodMonth = prodMonth.replaceAll("-", "");
                window.location.href = reportUrl + "KBNRT260" + '?prodMonth=' + prodMonth + '&checkedValue=' + checkedValue + '&UserName=' + result.data + '&Revision=' + Revision;
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
