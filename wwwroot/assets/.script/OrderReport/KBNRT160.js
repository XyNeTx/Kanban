$(document).ready(function () {
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
    xAjax.onChange("#F_ShiftFrom, #F_ShiftTo", function () {
        if ($("#F_ShiftTo").val() === null || $("#F_ShiftTo").val() === "") {
            $("#F_ShiftTo").val($("#F_ShiftFrom").val()).change();
        }
    });

    xAjax.onClick("#ReportBtn", function () {
        var supFrom = $("#F_SupFrom").val();
        var supTo = $("#F_SupTo").val();
        var kbnFrom = $("#F_KBNFrom").val();
        var kbnTo = $("#F_KBNTo").val();
        var storeFrom = $("#F_StoreFrom").val();
        var storeTo = $("#F_StoreTo").val();
        var partFrom = $("#F_PartFrom").val();
        var partTo = $("#F_PartTo").val();
        var orderFrom = $("#F_OrderFrom").val().replaceAll('-', '');
        var orderTo = $("#F_OrderTo").val().replaceAll('-', '');
        var shiftFrom = $("#F_ShiftFrom").val().substring(0,1);
        var shiftTo = $("#F_ShiftTo").val().substring(0, 1);

        if (supFrom === "" || supFrom === undefined || supTo === "" || supTo === undefined) {
            return xSwal.error("Supplier Code is empty", "Please Select Supplier Code");
        }
        if (supFrom > supTo) {
            return xSwal.error("Invalid Input Supplier Code", "Please Select Supplier Code From less than Supplier Code To");
        }
        if (kbnFrom === "" || kbnFrom === undefined || kbnTo === "" || kbnTo === undefined) {
            return xSwal.error("Kanban No is empty", "Please Select Kanban No");
        }
        if (kbnFrom > kbnTo) {
            return xSwal.error("Invalid Input Kanban No", "Please Select Kanban No From less than Kanban No To");
        }
        if (storeFrom === "" || storeFrom === undefined || storeTo === "" || storeTo === undefined) {
            return xSwal.error("Store Code is empty", "Please Select Store Code");
        }
        if (storeFrom > storeTo) {
            return xSwal.error("Invalid Input Store Code", "Please Select Store Code From less than Store Code To");
        }
        if (orderFrom > orderTo) {
            return xSwal.error("Invalid Input Order Date", "Please Select Order Date From less than Order Date To");
        }
        if (partFrom === "" || partFrom === undefined || partTo === "" || partTo === undefined) {
            return xSwal.error("Part No. is empty", "Please Select Part No.");
        }
        if (partFrom > partTo) {
            return xSwal.error("Invalid Input Part No", "Please Select Part No From less than Part No To");
        }
        if (shiftFrom === "" || shiftFrom === undefined || shiftTo === "" || shiftTo === undefined) {
            return xSwal.error("Order Shift is empty", "Please Select Order Shift");
        }
        xAjax.Post({
            url: 'KBNRT160/OnReportBtnClick',
            data: {
                supFrom: supFrom,
                supTo: supTo,
                kbnFrom: kbnFrom,
                kbnTo: kbnTo,
                storeFrom: storeFrom,
                storeTo: storeTo,
                partFrom: partFrom,
                partTo: partTo,
                orderFrom: orderFrom,
                orderTo: orderTo,
                shiftFrom: shiftFrom,
                shiftTo: shiftTo
            },
            then: function (result) {
                if (result.status == 200) {
                    var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
                    var reportUrl = "http://hmmt-app03/Reportserver/report/KB3/";
                    window.location.href = reportUrl + filename + '?HostName=' + result.data2 + '&UserName=' + result.data;
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