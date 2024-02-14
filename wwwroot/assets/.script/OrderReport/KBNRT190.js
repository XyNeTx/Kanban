$(document).ready(function () {
    xSplash.hide();

    function hideBlank() {
        $('#supFromBlank').hide();
        $('#supToBlank').hide();
        $('#KBNFromBlank').hide();
        $('#KBNToBlank').hide();
        $('#PartFromBlank').hide();
        $('#PartToBlank').hide();
        $('#StoreFromBlank').hide();
        $('#StoreToBlank').hide();
        $('#TripFromBlank').hide();
        $('#TripToBlank').hide();
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

    //xAjax.Post({
    //    url: 'KBNRT190/Initial',
    //    then: function (result) {
    //        //console.log(result);
    //        $.each(result.data, function (i, v) {
    //            $("#F_TripFrom").append($("<option>", { value: v, text: v }, "</option>"));
    //            $("#F_TripTo").append($("<option>", { value: v, text: v }, "</option>"));
    //        });
    //        hideBlank();
    //    },
    //    error: function (result) {
    //        console.error("Initial Error from Receive Special Report");
    //    },
    //});

    xAjax.onChange("#F_SupFrom, #F_SupTo", function () {
        if ($("#F_SupTo").val() == null || $("#F_SupTo").val() == "") {
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
        if ($("#F_KBNTo").val() == null || $("#F_KBNTo").val() == "") {
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
        if ($("#F_StoreTo").val() == null || $("#F_StoreTo").val() == "") {
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
        if ($("#F_PartTo").val() == null || $("#F_PartTo").val() == "") {
            $("#F_PartTo").val($("#F_PartFrom").val()).change();
        }
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
        if ($("#F_TripTo").val() == null || $("#F_TripTo").val() == "") {
            $("#F_TripTo").val($("#F_TripFrom").val()).change();
        }
    });

    $("#F_OrderDateFrom").attr("disabled", true);
    $("#F_OrderDateTo").attr("disabled", true);

    $("#OrderDiv").click(function () {
        var checked = $("input[name='radDate']:checked").val();
        if (checked === "Order") {
            $("#F_DeliDateFrom").attr("disabled", true);
            $("#F_DeliDateTo").attr("disabled", true);
            $("#F_TripFrom").attr("disabled", true);
            $("#F_TripTo").attr("disabled", true);
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
            $("#F_TripFrom").attr("disabled", false);
            $("#F_TripTo").attr("disabled", false);
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
        var checked = $("input[name='radDate']:checked").val();
        if (checked === "Order") {
            var dateFrom = $("#F_OrderDateFrom").val().replaceAll('-', '');
            var dateTo = $("#F_OrderDateTo").val().replaceAll('-', '');
            var tripFrom = null;
            var tripTo = null;
            var url = "KBNRT160/OnReportBtnClickDelivery";
        }
        else if (checked === "Delivery") {
            var dateFrom = $("#F_DeliDateFrom").val().replaceAll('-', '');
            var dateTo = $("#F_DeliDateTo").val().replaceAll('-', '');
            var tripFrom = $("#F_TripFrom").val();
            var tripTo = $("#F_TripTo").val();
            var url = "KBNRT160/OnReportBtnClickOrder";
        }

        if (supFrom == null || supFrom == undefined || supTo == null || supTo == undefined) {
            xSwal.error("Supplier Code is empty", "Please Select Supplier Code");
        }
        if (supFrom > supTo) {
            xSwal.error("Invalid Input Supplier Code", "Please Select Supplier Code From less than Supplier Code To");
        }
        if (kbnFrom == null || kbnFrom == undefined || kbnTo == null || kbnTo == undefined) {
            xSwal.error("Kanban No is empty", "Please Select Kanban No");
        }
        if (kbnFrom > kbnTo) {
            xSwal.error("Invalid Input Kanban No", "Please Select Kanban No From less than Kanban No To");
        }
        if (storeFrom == null || storeFrom == undefined || storeTo == null || storeTo == undefined) {
            xSwal.error("Store Code is empty", "Please Select Store Code");
        }
        if (storeFrom > storeTo) {
            xSwal.error("Invalid Input Store Code", "Please Select Store Code From less than Store Code To");
        }
        if (orderFrom > orderTo) {
            xSwal.error("Invalid Input Order Date", "Please Select Order Date From less than Order Date To");
        }
        if (partFrom == null || partFrom == undefined || partTo == null || partTo == undefined) {
            xSwal.error("Part No. is empty", "Please Select Part No.");
        }
        if (partFrom > partTo) {
            xSwal.error("Invalid Input Part No", "Please Select Part No From less than Part No To");
        }
        if (tripFrom == null || tripFrom == undefined || tripTo == null || tripTo == undefined) {
            xSwal.error("Delivery Trip is empty", "Please Select Delivery Trip");
        }
        xAjax.Post({
            url: url,
            data: {
                supFrom: supFrom,
                supTo: supTo,
                kbnFrom: kbnFrom,
                kbnTo: kbnTo,
                storeFrom: storeFrom,
                storeTo: storeTo,
                partFrom: partFrom,
                partTo: partTo,
                dateFrom: dateFrom,
                dateTo: dateTo,
                tripFrom: tripFrom,
                tripTo: tripTo
            },
            then: function (result) {
                console.log(result);
                console.log(result);
                var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
                var reportUrl = "http://hmmt-app03/Reportserver/report/KB3/";
                window.location.href = reportUrl + filename + '?HostName=' + result.data2 + '&UserName=' + result.data;
            },
            error: function (result) {
                console.error(result);
            }
        });
    });

});