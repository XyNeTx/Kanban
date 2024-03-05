$(document).ready(async function () {
    await ProdMonthChange();
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
            $.each(result.data3, function (i, v) {
                $("#F_StoreFrom").append($("<option>", { value: v.F_Store_CD, text: v.F_Store_CD }, "</option>"));
                $("#F_StoreTo").append($("<option>", { value: v.F_Store_CD, text: v.F_Store_CD }, "</option>"));
            });
            $.each(result.data4, function (i, v) {
                $("#F_PartFrom").append($("<option>", { value: v.prt_no, text: v.prt_no }, "</option>"));
                $("#F_PartTo").append($("<option>", { value: v.prt_no, text: v.prt_no }, "</option>"));
            });
            hideBlank();
            xSplash.hide();
        },
        error: function (result) {
            console.error("Initial Error from Receive Special Report");
            xSplash.hide();
        },
    });
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
    $('#ShiftFromBlank').hide();
    $('#ShiftToBlank').hide();
}


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

xAjax.onChange("#F_KBNFrom, #F_KBNTo", function () {
    if ($("#F_KBNTo").val() === null || $("#F_KBNTo").val() === "") {
        $("#F_KBNTo").val($("#F_KBNFrom").val()).change();
    }
    var supFrom = $("#F_SupFrom").val();
    var supTo = $("#F_SupTo").val();
    var kbnFrom = $("#F_KBNFrom").val();
    var kbnTo = $("#F_KBNTo").val();
    xAjax.Post({
        url: 'KBNRT160/OnKANBANChange',
        data: {
            supFrom: supFrom,
            supTo: supTo,
            kbnFrom: kbnFrom,
            kbnTo: kbnTo
        },
        then: function (result) {
            $("#F_PartFrom").empty();
            $("#F_PartTo").empty();
            $("#F_StoreFrom").empty();
            $("#F_StoreTo").empty();
            $("#F_PartFrom").append($("<option id='PartFromBlank'></option>"));
            $("#F_PartTo").append($("<option id='PartToBlank'></option>"));
            $("#F_StoreFrom").append($("<option id='StoreFromBlank'></option>"));
            $("#F_StoreTo").append($("<option id='StoreToBlank'></option>"));
            $.each(result.data, function (i, v) {
                $("#F_StoreFrom").append($("<option>", { value: v.F_Store_CD, text: v.F_Store_CD }, "</option>"));
                $("#F_StoreTo").append($("<option>", { value: v.F_Store_CD, text: v.F_Store_CD }, "</option>"));
            });
            $.each(result.data2, function (i, v) {
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

xAjax.onChange("#F_StoreFrom, #F_StoreTo", function () {
    if ($("#F_StoreTo").val() === null || $("#F_StoreTo").val() === "") {
        $("#F_StoreTo").val($("#F_StoreFrom").val()).change();
    }
    var supFrom = $("#F_SupFrom").val();
    var supTo = $("#F_SupTo").val();
    var kbnFrom = $("#F_KBNFrom").val();
    var kbnTo = $("#F_KBNTo").val();
    var storeFrom = $("#F_StoreFrom").val();
    var storeTo = $("#F_StoreTo").val();

    xAjax.Post({
        url: 'KBNRT160/OnStoreChange',
        data: {
            supFrom: supFrom,
            supTo: supTo,
            kbnFrom: kbnFrom,
            kbnTo: kbnTo,
            storeFrom: storeFrom,
            storeTo: storeTo
        },
        then: function (result) {
            $("#F_PartFrom").empty();
            $("#F_PartTo").empty();
            $("#F_PartFrom").append($("<option id='PartFromBlank'></option>"));
            $("#F_PartTo").append($("<option id='PartToBlank'></option>"));
            $.each(result.data, function (i, v) {
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
});
xAjax.onChange("#F_ProdMonth", async function () {
    await ProdMonthChange();
});

function ProdMonthChange() {
    var prodMonth = $("#F_ProdMonth").val().trim();
    $("#Revision").val("");
    $("#Version").val("");
    return xAjax.Post({
        url: 'KBNRT260/Display_Detail',
        data: { prodMonth: prodMonth },
        then: async function (result) {
            await $("#Revision").val(result.data[0].F_Revision);
            await $("#Version").val(result.data[0].F_Version);
        },
        error: function (result) {
            console.error("Initial Error", result);
        },
    });
}


xAjax.onClick("#ReportBtn", async function () {
    await ReportClick();
});
xAjax.onClick("#ReportAbnormalBtn", async function () {
    await AbnormalReport();
});

async function ReportClick() {
    var prodMonth = $("#F_ProdMonth").val().trim();
    var supFrom = $("#F_SupFrom").val();
    var supTo = $("#F_SupTo").val();
    var kbnFrom = $("#F_KBNFrom").val();
    var kbnTo = $("#F_KBNTo").val();
    var storeFrom = $("#F_StoreFrom").val();
    var storeTo = $("#F_StoreTo").val();
    var partFrom = $("#F_PartFrom").val();
    var partTo = $("#F_PartTo").val();
    xAjax.Post({
        url: 'KBNRT270/NormalReportClick',
        data: {
            prodMonth: prodMonth,
            supFrom: supFrom,
            supTo: supTo,
            kbnFrom: kbnFrom,
            kbnTo: kbnTo,
            storeFrom: storeFrom,
            storeTo: storeTo,
            partFrom: partFrom,
            partTo: partTo
        },
        then: function (result) {
            if (result.status === "200") {
                var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
                var reportUrl = "http://hmmt-app03/Reportserver/report/KB3/";
                prodMonth = prodMonth.replaceAll("-", "");
                window.location.href = reportUrl + filename + '?prodMonth=' + prodMonth + '&Revision=' + result.data2 +
                    '&SupFrom=' + supFrom + '&SupTo=' + supTo + '&KBNFrom=' + kbnFrom + '&KBNTo=' + kbnTo +
                    '&PartFrom=' + partFrom + '&PartTo=' + partTo + '&StoreFrom=' + storeFrom + '&StoreTo=' + storeTo +
                    '&UserName=' + result.data + '&Status=' + status;
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
async function AbnormalReport() {
    var prodMonth = $("#F_ProdMonth").val().trim();
    var version = $("#Version").val().trim().substring(0, 1);
    var supFrom = $("#F_SupFrom").val();
    var supTo = $("#F_SupTo").val();
    var kbnFrom = $("#F_KBNFrom").val();
    var kbnTo = $("#F_KBNTo").val();
    var storeFrom = $("#F_StoreFrom").val();
    var storeTo = $("#F_StoreTo").val();
    var partFrom = $("#F_PartFrom").val();
    var partTo = $("#F_PartTo").val();
    var status = "ABNORMAL DATA";
    xAjax.Post({
        url: 'KBNRT270/ABNormalReportClick',
        data: {
            prodMonth: prodMonth,
            version: version,
            supFrom: supFrom,
            supTo: supTo,
            kbnFrom: kbnFrom,
            kbnTo: kbnTo,
            storeFrom: storeFrom,
            storeTo: storeTo,
            partFrom: partFrom,
            partTo: partTo
        },
        then: function (result) {
            if (result.status === "200") {
                var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
                var reportUrl = "http://hmmt-app03/Reportserver/report/KB3/";
                prodMonth = prodMonth.replaceAll("-", "");
                window.location.href = reportUrl + filename + '?prodMonth=' + prodMonth + '&Revision=' + result.data2 +
                    '&SupFrom=' + supFrom + '&SupTo=' + supTo + '&KBNFrom=' + kbnFrom + '&KBNTo=' + kbnTo +
                    '&PartFrom=' + partFrom + '&PartTo=' + partTo + '&StoreFrom=' + storeFrom + '&StoreTo=' + storeTo +
                    '&UserName=' + result.data + '&Status=' + status;
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