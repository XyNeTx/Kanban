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
    }

    xAjax.Post({
        url: 'KBNRT120/Initial',
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
                $("#F_PartFrom").append($("<option>", { value: v.Prt_No, text: v.Prt_No }, "</option>"));
                $("#F_PartTo").append($("<option>", { value: v.Prt_No, text: v.Prt_No }, "</option>"));
            });
            $.each(result.data4, function (i, v) {
                $("#F_StoreFrom").append($("<option>", { value: v.F_Store_CD, text: v.F_Store_CD }, "</option>"));
                $("#F_StoreTo").append($("<option>", { value: v.F_Store_CD, text: v.F_Store_CD }, "</option>"));
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
        if ($("#F_SupTo").val() == null || $("#F_SupTo").val() == "") {
            $("#F_SupTo").val($("#F_SupFrom").val()).change();
        }
        var supFrom = $("#F_SupFrom").val();
        var supTo = $("#F_SupTo").val();
        xAjax.Post({
            url: 'KBNRT120/OnSupplierChange',
            data: {
                supFrom: supFrom,
                supTo: supTo
            },
            then: function (result) {
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
                    $("#F_PartFrom").append($("<option>", { value: v.Prt_No, text: v.Prt_No }, "</option>"));
                    $("#F_PartTo").append($("<option>", { value: v.Prt_No, text: v.Prt_No }, "</option>"));
                });
                $.each(result.data3, function (i, v) {
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

    xAjax.onChange("#F_KBNFrom, #F_KBNTo", function () {
        if ($("#F_KBNTo").val() == null || $("#F_KBNTo").val() == "") {
            $("#F_KBNTo").val($("#F_KBNFrom").val()).change();
        }
        var supFrom = $("#F_SupFrom").val();
        var supTo = $("#F_SupTo").val();
        var kbnFrom = $("#F_KBNFrom").val();
        var kbnTo = $("#F_KBNTo").val();
        xAjax.Post({
            url: 'KBNRT120/OnKANBANChange',
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
                    $("#F_PartFrom").append($("<option>", { value: v.Prt_No, text: v.Prt_No }, "</option>"));
                    $("#F_PartTo").append($("<option>", { value: v.Prt_No, text: v.Prt_No }, "</option>"));
                });
                $.each(result.data2, function (i, v) {
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

    xAjax.onChange("#F_PartFrom, #F_PartTo", function () {
        if ($("#F_PartTo").val() == null || $("#F_PartTo").val() == "") {
            $("#F_PartTo").val($("#F_PartFrom").val()).change();
        }
        var supFrom = $("#F_SupFrom").val();
        var supTo = $("#F_SupTo").val();
        var kbnFrom = $("#F_KBNFrom").val();
        var kbnTo = $("#F_KBNTo").val();
        var partFrom = $("#F_PartFrom").val();
        var partTo = $("#F_PartTo").val();
        xAjax.Post({
            url: 'KBNRT120/OnPartChange',
            data: {
                supFrom: supFrom,
                supTo: supTo,
                kbnFrom: kbnFrom,
                kbnTo: kbnTo,
                partFrom: partFrom,
                partTo: partTo
            },
            then: function (result) {
                $("#F_StoreFrom").empty();
                $("#F_StoreTo").empty();
                $("#F_StoreFrom").append($("<option id='StoreFromBlank'></option>"));
                $("#F_StoreTo").append($("<option id='StoreToBlank'></option>"));
                $.each(result.data, function (i, v) {
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

    xAjax.onClick("#ReportStatusBtn", function () {
        var supFrom = $("#F_SupFrom").val();
        var supTo = $("#F_SupTo").val();
        var kbnFrom = $("#F_KBNFrom").val();
        var kbnTo = $("#F_KBNTo").val();
        var partFrom = $("#F_PartFrom").val();
        var partTo = $("#F_PartTo").val();
        var storeFrom = $("#F_StoreFrom").val();
        var storeTo = $("#F_StoreTo").val();

        if (supFrom === "" || supFrom === null || supTo === "" || supTo === null) {
            return xSwal.error("Data Error", "Please Select Supplier Code From And Supplier Code To");
        }

        if (supFrom > supTo) {
            return xSwal.error("Data Error", "Please Select Supplier Code From Less than Supplier Code To");
        }


        if (kbnFrom === "" || kbnFrom === null || kbnTo === "" || kbnTo === null) {
            return xSwal.error("Data Error", "Please Select Kanban No. From And Kanban No. To");
        }

        if (kbnFrom > kbnTo) {
            return xSwal.error("Data Error", "Please Select Kanban No. From Less than Kanban No. To");
        }

        if (partFrom === "" || partFrom === null || partTo === "" || partTo === null) {
            return xSwal.error("Data Error", "Please Select Part No. From And Part No. To");
        }

        if (partFrom > partTo) {
            return xSwal.error("Data Error", "Please Select Part No. From Less than Part No. To");
        }


        if (storeFrom === "" || storeFrom === null || storeTo === "" || storeTo === null) {
            console.log(storeFrom, storeTo);
            return xSwal.error("Data Error", "Please Select Store Code From And Store Code To");
        }

        if (storeFrom > storeTo) {
            return xSwal.error("Data Error", "Please Select Store Code From Less than Store Code To");
        }

        var userName = $("#profile-avatar").prop("title");

        var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
        var reportUrl = "http://hmmt-app03/Reportserver/report/KB3/";

        window.location.href = reportUrl + filename + '_Status' + '?SupFrom=' + supFrom + '&SupTo=' + supTo
            + '&KBNFrom=' + kbnFrom + '&KBNTo=' + kbnTo + '&PartFrom=' + partFrom + '&PartTo=' + partTo
            + '&StoreFrom=' + storeFrom + '&StoreTo=' + storeTo + '&UserName=' + userName;
    });
    xAjax.onClick("#ReportDetailBtn", function () {
        var supFrom = $("#F_SupFrom").val();
        var supTo = $("#F_SupTo").val();
        var kbnFrom = $("#F_KBNFrom").val();
        var kbnTo = $("#F_KBNTo").val();
        var partFrom = $("#F_PartFrom").val();
        var partTo = $("#F_PartTo").val();
        var storeFrom = $("#F_StoreFrom").val();
        var storeTo = $("#F_StoreTo").val();

        if (supFrom === "" || supFrom === null || supTo === "" || supTo === null) {
            return xSwal.error("Data Error", "Please Select Supplier Code From And Supplier Code To");
        }

        if (supFrom > supTo) {
            return xSwal.error("Data Error", "Please Select Supplier Code From Less than Supplier Code To");
        }


        if (kbnFrom === "" || kbnFrom === null || kbnTo === "" || kbnTo === null) {
            return xSwal.error("Data Error", "Please Select Kanban No. From And Kanban No. To");
        }

        if (kbnFrom > kbnTo) {
            return xSwal.error("Data Error", "Please Select Kanban No. From Less than Kanban No. To");
        }

        if (partFrom === "" || partFrom === null || partTo === "" || partTo === null) {
            return xSwal.error("Data Error", "Please Select Part No. From And Part No. To");
        }

        if (partFrom > partTo) {
            return xSwal.error("Data Error", "Please Select Part No. From Less than Part No. To");
        }


        if (storeFrom === "" || storeFrom === null || storeTo === "" || storeTo === null) {
            console.log(storeFrom, storeTo);
            return xSwal.error("Data Error", "Please Select Store Code From And Store Code To");
        }

        if (storeFrom > storeTo) {
            return xSwal.error("Data Error", "Please Select Store Code From Less than Store Code To");
        }

        var userName = $("#profile-avatar").prop("title");

        var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
        var reportUrl = "http://hmmt-app03/Reportserver/report/KB3/";

        window.location.href = reportUrl + filename + '_Detail' + '?SupFrom=' + supFrom + '&SupTo=' + supTo
            + '&KBNFrom=' + kbnFrom + '&KBNTo=' + kbnTo + '&PartFrom=' + partFrom + '&PartTo=' + partTo
            + '&StoreFrom=' + storeFrom + '&StoreTo=' + storeTo + '&UserName=' + userName;
    });
});