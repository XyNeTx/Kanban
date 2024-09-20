$(document).ready(async function () {
    await OrderTypeChange();
});

function OrderTypeChange() {
    var yearMonth = $("#F_DeliMonth").val().trim().replaceAll("-", "");
    var OrderType = $("#F_Order").val().trim();
    return xAjax.Post({
        url: 'KBNRT296/Onload',
        data: {
            yearMonth: yearMonth,
            OrderType: OrderType
        },
        then: function (result) {
            $("#F_SupFrom").empty();
            $("#F_SupTo").empty();
            $("#F_SupFrom").append($("<option id='supFromBlank'></option>"));
            $("#F_SupTo").append($("<option id='supToBlank'></option>"));
            $.each(result.data, function (i, v) {
                $("#F_SupFrom").append($("<option>", { value: v, text: v }, "</option>"));
                $("#F_SupTo").append($("<option>", { value: v, text: v }, "</option>"));
                $("#supFromBlank").hide();
                $("#supToBlank").hide();
            });
            xSplash.hide();
            $("#prgProcess_label_").hide();
            //console.log($("#prgProcess").val());
        },
        error: function (result) {
            console.error("Initial Error", result);
        },
    });
};

$("#F_Order").on("change", async function () {
    await OrderTypeChange();
});
$("#SummaryReportBtn").on("click", async function () {
    var yearMonth = $("#F_DeliMonth").val().trim().replaceAll("-", "");
    var MonthYear = yearMonth.substring(4, 6) + "/" + yearMonth.substring(0, 4);
    var OrderType = $("#F_Order").val().trim();
    var supFrom = $("#F_SupFrom").val().trim();
    var supTo = $("#F_SupTo").val().trim();
    xItem.progress({ id: 'prgProcess', current: 10, label: 'Calculate Delay Invoice Date : {{##.##}} %' });
    $("#prgProcess_label_").show();
    var _dt = await LoadInvoiceData();
    console.log(_dt);

    if (_dt.rows != null) {
        for (var i = 0; i < _dt.rows.length; i++) {

            xItem.progress({ id: 'prgProcess', current: (((i + 1) / _dt.rows.length) * 90.00) + 10.00, label: 'Calculate Delay Invoice Date : {{##.##}} %' });

            await UpdateInvoiceData(_dt.rows[i]);
        }
    }
    else {
        return xSwal.error("Data Not Found", "Please Try Other Option");
    }

    setTimeout(100);

    xItem.progress({ id: 'prgProcess', current: 0.00, label: 'Get Invoice Data from E-procurement : {{##.##}} %' });
    var _dtRun = await GetDataRunagain();
    console.log(_dtRun);

    if (_dtRun.rows != null) {
        for (var j = 0; j < _dtRun.rows.length; j++) {

            xItem.progress({ id: 'prgProcess', current: (((j + 1) / _dt.rows.length) * 100.00), label: 'Get Invoice Data from E-procurement : {{##.##}} %' });

            /*const result = await InsertDataRunagain(_dtRun.rows[j]);*/
            var result = await xAjax.PostAsync({
                url: 'KBNRT296/InsertDataRunagain',
                data: {
                    DT: _dtRun.rows[j]
                },
            });
            console.log(result);
        }
        console.log("Complete 74");
        console.log(result);
        var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
        var reportUrl = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx?/KB3/";
        window.location.href = reportUrl + filename + '&userName=' + result.data + '&YearMonth=' + yearMonth + '&MonthYear=' + MonthYear +
            '&Plant=' + result.data2 + '&OrderType=' + OrderType + '&SupFrom=' + supFrom + '&SupTo=' + supTo;
    }
    else {
        return xSwal.error("Data Not Found", "Please Try Other Option");
    }
});

$("#DetailReportBtn").on("click", async function () {
    var yearMonth = $("#F_DeliMonth").val().trim().replaceAll("-", "");
    var MonthYear = yearMonth.substring(4, 6) + "/" + yearMonth.substring(0, 4);
    var OrderType = $("#F_Order").val().trim();
    var supFrom = $("#F_SupFrom").val().trim();
    var supTo = $("#F_SupTo").val().trim();
    xItem.progress({ id: 'prgProcess', current: 10, label: 'Calculate Delay Invoice Date : {{##.##}} %' });
    $("#prgProcess_label_").show();
    var _dt = await LoadInvoiceData();
    console.log(_dt);

    if (_dt.rows != null) {
        for (var i = 0; i < _dt.rows.length; i++) {

            xItem.progress({ id: 'prgProcess', current: (((i + 1) / _dt.rows.length) * 90.00) + 10.00, label: 'Calculate Delay Invoice Date : {{##.##}} %' });

            await UpdateInvoiceData(_dt.rows[i]);
        }
    }
    else {
        return xSwal.error("Data Not Found", "Please Try Other Option");
    }

    setTimeout(100);

    xItem.progress({ id: 'prgProcess', current: 0.00, label: 'Get Invoice Data from E-procurement : {{##.##}} %' });
    var _dtRun = await GetDataRunagain();
    console.log(_dtRun);

    if (_dtRun.rows != null) {
        for (var j = 0; j < _dtRun.rows.length; j++) {

            xItem.progress({ id: 'prgProcess', current: (((j + 1) / _dt.rows.length) * 100.00), label: 'Get Invoice Data from E-procurement : {{##.##}} %' });

            /*const result = await InsertDataRunagain(_dtRun.rows[j]);*/
            var result = await xAjax.PostAsync({
                url: 'KBNRT296/InsertDataRunagain',
                data: {
                    DT: _dtRun.rows[j]
                },
            });
        }
        var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1)+"Detail";
        var reportUrl = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx?/KB3/";
        window.location.href = reportUrl + filename + '&userName=' + result.data + '&YearMonth=' + yearMonth + '&MonthYear=' + MonthYear +
            '&Plant=' + result.data2 + '&OrderType=' + OrderType + '&SupFrom=' + supFrom + '&SupTo=' + supTo;
    }
    else {
        return xSwal.error("Data Not Found", "Please Try Other Option");
    }
});
function LoadInvoiceData() {
    var yearMonth = $("#F_DeliMonth").val().trim().replaceAll("-", "");
    var OrderType = $("#F_Order").val().trim();
    var supFrom = $("#F_SupFrom").val().trim();
    var supTo = $("#F_SupTo").val().trim();
    return xAjax.PostAsync({
        url: 'KBNRT296/LoadInvoiceData',
        data: {
            yearMonth: yearMonth,
            OrderType: OrderType,
            supFrom: supFrom,
            supTo: supTo
        },
    });
};
async function UpdateInvoiceData(data) {
    await xAjax.PostAsync({
        url: 'KBNRT296/UpdateInvoiceData',
        data: {
            DT: data
        },
    });
};
async function GetDataRunagain() {
    var yearMonth = $("#F_DeliMonth").val().trim().replaceAll("-", "");
    var supFrom = $("#F_SupFrom").val().trim();
    var supTo = $("#F_SupTo").val().trim();
    return await xAjax.PostAsync({
        url: 'KBNRT296/GetDataRunagain',
        data: {
            yearMonth: yearMonth,
            supFrom: supFrom,
            supTo: supTo
        },
    });
};
async function InsertDataRunagain(data) {
    return await xAjax.PostAsync({
        url: 'KBNRT296/InsertDataRunagain',
        data: {
            DT: data
        },
    });
};