$(document).ready(async function () {
    await initial();
});

function hideBlank() {
    $('#supFromBlank').hide();
    $('#supToBlank').hide();
    $('#KBNFromBlank').hide();
    $('#KBNToBlank').hide();
    $('#PartFromBlank').hide();
    $('#PartToBlank').hide();
    $('#StoreFromBlank').hide();
    $('#StoreToBlank').hide();
    $('#DockBlank').hide();
}

function initial() {
    xAjax.Post({
        url: 'KBNRT310/Initial',
        then: function (result) {
            console.log(result);
            $.each(result.data, function (i, v) {
                $("#F_SupFrom").append($("<option>", { value: v.Sup_CD, text: v.Sup_CD }, "</option>"));
                $("#F_SupTo").append($("<option>", { value: v.Sup_CD, text: v.Sup_CD }, "</option>"));
            });
            $.each(result.data2, function (i, v) {
                $("#F_KBNFrom").append($("<option>", { value: v.F_Sebango, text: v.F_Sebango }, "</option>"));
                $("#F_KBNTo").append($("<option>", { value: v.F_Sebango, text: v.F_Sebango }, "</option>"));
            });
            $.each(result.data3, function (i, v) {
                $("#F_StoreFrom").append($("<option>", { value: v.F_Store_CD, text: v.F_Store_CD }, "</option>"));
                $("#F_StoreTo").append($("<option>", { value: v.F_Store_CD, text: v.F_Store_CD }, "</option>"));
            });
            $.each(result.data4, function (i, v) {
                $("#F_PartFrom").append($("<option>", { value: v.prt_no, text: v.prt_no }, "</option>"));
                $("#F_PartTo").append($("<option>", { value: v.prt_no, text: v.prt_no }, "</option>"));
            });
            $.each(result.data5, function (i, v) {
                $("#F_Dock").append($("<option>", { value: v, text: v }, "</option>"));
            });
            hideBlank();
            xSplash.hide();
        },
        error: function (result) {
            console.error("Initial Error from Receive Special Report");
            xSplash.hide();
        },
    });
}

xAjax.onChange("#F_Date, #F_Shift", function () {
    var date = $("#F_Date").val().trim().replaceAll("-", "");
    var shift = $("#F_Shift").val().trim();
    xAjax.Post({
        url: 'KBNRT310/Initial',
        data: {
            date: date,
            shift: shift
        },
        then: function (result) {
            console.log(result);
            $("#F_Dock").empty();
            $("#F_SupFrom").empty();
            $("#F_SupTo").empty();
            $("#F_KBNFrom").empty();
            $("#F_KBNTo").empty();
            $("#F_PartFrom").empty();
            $("#F_PartTo").empty();
            $("#F_StoreFrom").empty();
            $("#F_StoreTo").empty();
            $("#F_Dock").append($("<option id='DockBlank'></option>"));
            $("#F_SupFrom").append($("<option id='SupFromBlank'></option>"));
            $("#F_SupTo").append($("<option id='SupToBlank'></option>"));
            $("#F_KBNFrom").append($("<option id='KBNFromBlank'></option>"));
            $("#F_KBNTo").append($("<option id='KBNToBlank'></option>"));
            $("#F_PartFrom").append($("<option id='PartFromBlank'></option>"));
            $("#F_PartTo").append($("<option id='PartToBlank'></option>"));
            $("#F_StoreFrom").append($("<option id='StoreFromBlank'></option>"));
            $("#F_StoreTo").append($("<option id='StoreToBlank'></option>"));
            $.each(result.data, function (i, v) {
                $("#F_SupFrom").append($("<option>", { value: v.Sup_CD, text: v.Sup_CD }, "</option>"));
                $("#F_SupTo").append($("<option>", { value: v.Sup_CD, text: v.Sup_CD }, "</option>"));
            });
            $.each(result.data2, function (i, v) {
                $("#F_KBNFrom").append($("<option>", { value: v.F_Sebango, text: v.F_Sebango }, "</option>"));
                $("#F_KBNTo").append($("<option>", { value: v.F_Sebango, text: v.F_Sebango }, "</option>"));
            });
            $.each(result.data3, function (i, v) {
                $("#F_StoreFrom").append($("<option>", { value: v.F_Store_CD, text: v.F_Store_CD }, "</option>"));
                $("#F_StoreTo").append($("<option>", { value: v.F_Store_CD, text: v.F_Store_CD }, "</option>"));
            });
            $.each(result.data4, function (i, v) {
                $("#F_PartFrom").append($("<option>", { value: v.prt_no, text: v.prt_no }, "</option>"));
                $("#F_PartTo").append($("<option>", { value: v.prt_no, text: v.prt_no }, "</option>"));
            });
            $.each(result.data5, function (i, v) {
                $("#F_Dock").append($("<option>", { value: v, text: v }, "</option>"));
            });
            hideBlank();
        },
        error: function (result) {
            console.error("Initial Error from Receive Special Report");
            xSplash.hide();
        },
    });
});

xAjax.onChange("#F_SupFrom, #F_SupTo", function () {
    if ($("#F_SupTo").val() === null || $("#F_SupTo").val() === "") {
        $("#F_SupTo").val($("#F_SupFrom").val()).change();
    }
    var supFrom = $("#F_SupFrom").val();
    var supTo = $("#F_SupTo").val();
    var date = $("#F_Date").val().trim().replaceAll("-", "");
    xAjax.Post({
        url: 'KBNRT310/OnSupplierChange',
        data: {
            supFrom: supFrom,
            supTo: supTo,
            date: date
        },
        then: function (result) {
            //console.log(result);
            $("#F_KBNFrom").empty();
            $("#F_KBNTo").empty();
            $("#F_PartFrom").empty();
            $("#F_PartTo").empty();
            $("#F_StoreFrom").empty();
            $("#F_StoreTo").empty();
            $("#F_KBNFrom").append($("<option id='KBNFromBlank'></option>"));
            $("#F_KBNTo").append($("<option id='KBNToBlank'></option>"));
            $("#F_PartFrom").append($("<option id='PartFromBlank'></option>"));
            $("#F_PartTo").append($("<option id='PartToBlank'></option>"));
            $("#F_StoreFrom").append($("<option id='StoreFromBlank'></option>"));
            $("#F_StoreTo").append($("<option id='StoreToBlank'></option>"));
            $.each(result.data, function (i, v) {
                $("#F_KBNFrom").append($("<option>", { value: v.F_Sebango, text: v.F_Sebango }, "</option>"));
                $("#F_KBNTo").append($("<option>", { value: v.F_Sebango, text: v.F_Sebango }, "</option>"));
            });
            $.each(result.data2, function (i, v) {
                $("#F_StoreFrom").append($("<option>", { value: v.F_Store_CD, text: v.F_Store_CD }, "</option>"));
                $("#F_StoreTo").append($("<option>", { value: v.F_Store_CD, text: v.F_Store_CD }, "</option>"));
            });
            $.each(result.data3, function (i, v) {
                $("#F_PartFrom").append($("<option>", { value: v.prt_no, text: v.prt_no }, "</option>"));
                $("#F_PartTo").append($("<option>", { value: v.prt_no, text: v.prt_no }, "</option>"));
            });
            hideBlank();
        },
        error: function (result) {
            console.error(result);
        },
    });
});

xAjax.onChange("#F_PartFrom, #F_PartTo", function () {
    if ($("#F_PartTo").val() === null || $("#F_PartTo").val() === "") {
        $("#F_PartTo").val($("#F_PartFrom").val()).change();
    }
    var supFrom = $("#F_SupFrom").val();
    var supTo = $("#F_SupTo").val();
    var partFrom = $("#F_PartFrom").val();
    var partTo = $("#F_PartTo").val();
    var date = $("#F_Date").val().trim().replaceAll("-", "");
    xAjax.Post({
        url: 'KBNRT310/OnPartNoChange',
        data: {
            supFrom: supFrom,
            supTo: supTo,
            partFrom: partFrom,
            partTo: partTo,
            date: date
        },
        then: function (result) {
            $("#F_KBNFrom").empty();
            $("#F_KBNTo").empty();
            $("#F_StoreFrom").empty();
            $("#F_StoreTo").empty();
            $("#F_KBNFrom").append($("<option id='KBNFromBlank'></option>"));
            $("#F_KBNTo").append($("<option id='KBNToBlank'></option>"));
            $("#F_StoreFrom").append($("<option id='StoreFromBlank'></option>"));
            $("#F_StoreTo").append($("<option id='StoreToBlank'></option>"));
            $.each(result.store, function (i, v) {
                $("#F_StoreFrom").append($("<option>", { value: v.F_Store_CD, text: v.F_Store_CD }, "</option>"));
                $("#F_StoreTo").append($("<option>", { value: v.F_Store_CD, text: v.F_Store_CD }, "</option>"));
            });
            $.each(result.kanban_no, function (i, v) {
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

xAjax.onChange("#F_KBNFrom, #F_KBNTo", function () {
    if ($("#F_KBNTo").val() === null || $("#F_KBNTo").val() === "") {
        $("#F_KBNTo").val($("#F_KBNFrom").val()).change();
    }
    var supFrom = $("#F_SupFrom").val();
    var supTo = $("#F_SupTo").val();
    var partFrom = $("#F_PartFrom").val();
    var partTo = $("#F_PartTo").val();
    var kbnFrom = $("#F_KBNFrom").val();
    var kbnTo = $("#F_KBNTo").val();
    var date = $("#F_Date").val().trim().replaceAll("-", "");
    xAjax.Post({
        url: 'KBNRT310/OnKANBANChange',
        data: {
            supFrom: supFrom,
            supTo: supTo,
            partFrom: partFrom,
            partTo: partTo,
            kbnFrom: kbnFrom,
            kbnTo: kbnTo,
            date: date
        },
        then: function (result) {
            $("#F_StoreFrom").empty();
            $("#F_StoreTo").empty();
            $("#F_StoreFrom").append($("<option id='StoreFromBlank'></option>"));
            $("#F_StoreTo").append($("<option id='StoreToBlank'></option>"));
            $.each(result.store, function (i, v) {
                $("#F_StoreFrom").append($("<option>", { value: v.F_Store_CD, text: v.F_Store_CD }, "</option>"));
                $("#F_StoreTo").append($("<option>", { value: v.F_Store_CD, text: v.F_Store_CD }, "</option>"));
            });
            hideBlank();
        },
        error: function (result) {
            console.error(result);
        },
    });
});

xAjax.onChange("#F_StoreFrom, #F_StoreTo", function () {
    if ($("#F_StoreTo").val() === null || $("#F_StoreTo").val() === "") {
        $("#F_StoreTo").val($("#F_StoreFrom").val()).change();
    }
});

xAjax.onClick("#ReportBtn", function () {
    var date = $("#F_Date").val().trim().replaceAll("-", "");
    var shift = $("#F_Shift").val().trim();
    var supFrom = $("#F_SupFrom").val();
    var supTo = $("#F_SupTo").val();
    var partFrom = $("#F_PartFrom").val();
    var partTo = $("#F_PartTo").val();
    var storeFrom = $("#F_StoreFrom").val();
    var storeTo = $("#F_StoreTo").val();
    var kbnFrom = $("#F_KBNFrom").val();
    var kbnTo = $("#F_KBNTo").val();
    var date = $("#F_Date").val().trim().replaceAll("-", "");
    var dock = $("#F_Dock").val().trim();
    xAjax.Post({
        url: "KBNRT310/OnReportClick",
        data: {
            date: date,
            shift: shift
        },
        then: function (result) {
            return console.log(result);
            if (result.status === "200") {
                var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
                var reportUrl = "http://hmmt-app03/Reportserver/report/KB3/";
                if (dock === "GW") {
                    var fullUrl = reportUrl + filename + 'GW?SupFrom=' + supFrom + '&SupTo=' + supTo + '&UserName=' + result.data
                        + '&KBNFrom=' + kbnFrom + '&KBNTo=' + kbnTo + '&Date=' + date + '&Shift=' + shift
                        + '&StoreFrom=' + storeFrom + '&StoreTo=' + storeTo + '&PartFrom=' + partFrom + '&PartTo=' + partTo + '&Dock=' + dock;
                    window.open(fullUrl);
                }
                else {
                    var fullUrl = reportUrl + filename + '?SupFrom=' + supFrom + '&SupTo=' + supTo + '&UserName=' + result.data
                        + '&KBNFrom=' + kbnFrom + '&KBNTo=' + kbnTo + '&Date=' + date + '&Shift=' + shift
                        + '&StoreFrom=' + storeFrom + '&StoreTo=' + storeTo + '&PartFrom=' + partFrom + '&PartTo=' + partTo + '&Dock=' + dock;
                    window.open(fullUrl);
                }
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

xAjax.onClick("#ReportAllBtn", function () {
    var date = $("#F_Date").val().trim().replaceAll("-", "");
    var shift = $("#F_Shift").val().trim();
    var supFrom = $("#F_SupFrom").val();
    var supTo = $("#F_SupTo").val();
    var partFrom = $("#F_PartFrom").val();
    var partTo = $("#F_PartTo").val();
    var storeFrom = $("#F_StoreFrom").val();
    var storeTo = $("#F_StoreTo").val();
    var kbnFrom = $("#F_KBNFrom").val();
    var kbnTo = $("#F_KBNTo").val();
    var date = $("#F_Date").val().trim().replaceAll("-", "");
    var dock = $("#F_Dock").val().trim();
    xAjax.Post({
        url: "KBNRT310/OnReportAllClick",
        data: {
            date: date,
            shift: shift
        },
        then: function (result) {
            console.log(result);
            if (result.status === "200") {
                var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
                var reportUrl = "http://hmmt-app03/Reportserver/report/KB3/";
                var fullUrl = reportUrl + filename + '?SupFrom=' + supFrom + '&SupTo=' + supTo + '&UserName=' + result.data
                    + '&KBNFrom=' + kbnFrom + '&KBNTo=' + kbnTo + '&Date=' + date + '&Shift=' + shift
                    + '&StoreFrom=' + storeFrom + '&StoreTo=' + storeTo + '&PartFrom=' + partFrom + '&PartTo=' + partTo + '&Dock=' + dock;
                window.open(fullUrl);
                var fullUrlGW = reportUrl + filename + 'GW?SupFrom=' + supFrom + '&SupTo=' + supTo + '&UserName=' + result.data
                    + '&KBNFrom=' + kbnFrom + '&KBNTo=' + kbnTo + '&Date=' + date + '&Shift=' + shift
                    + '&StoreFrom=' + storeFrom + '&StoreTo=' + storeTo + '&PartFrom=' + partFrom + '&PartTo=' + partTo + '&Dock=GW';
                window.open(fullUrlGW);
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