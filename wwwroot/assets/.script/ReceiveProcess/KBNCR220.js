$(document).ready(function () {
    var pdsSet = new Set();

    const table = $('#tblMaster').DataTable({
        columns: [
            { data: "No" },
            { data: "F_OrderNo" },
            { data: "F_Supplier" },
            { data: "F_Delivery_Date" },
            { data: "F_Receive_Date" },
            { data: "F_PDS_Status" },
            { data: "F_Receive_Status" }
        ],
        order: [[0, 'asc']],
        paging: false,
        scrollY: "150px",
        scrollCollapse: true,
    });

    function clearTable() {
        $('#tblMaster').DataTable().clear();
        $('#tblMaster').DataTable().draw();
    };

    function clearOption() {
        $('#F_SupplierFrom').find('option').remove().end();
        $('#F_SupplierTo').find('option').remove().end();
        $("#F_SupplierFrom").append($('<option id="supFromBlank"></option>'));
        $("#F_SupplierTo").append($('<option id="supToBlank"></option>'));
    }

    xAjax.Post({
        url: 'KBNCR220/Initial',
        then: function (result) {
            //console.log(result);
            $.each(result.data, function (e, t) {
                $("#F_SupplierFrom").append($("<option>", { value: t, text: t }, "</option>"));
                $("#F_SupplierTo").append($("<option>", { value: t, text: t }, "</option>"));
            });
            $('#supFromBlank').hide();
            $('#supToBlank').hide();
            xSplash.hide();
        },
        error: function (result) {
            console.error("Initial Error from Receive Special Report");
            xSplash.hide();
        },
    });


    xAjax.onChange("allRad, #7zRad, #7yRad", function () {
        var type = $('input[name="radio"]').filter(":checked").val();
        var devDate = $("#F_DeliveryFrom").val();
        var toDate = $("#F_DeliveryTo").val();
        //console.log(type);
        xAjax.Post({
            url: 'KBNCR220/Initial',
            data: {
                devDate: devDate,
                toDate: toDate,
                type: type
            },
            then: function (result) {
                clearOption();
                clearTable();
                if (result.data != null) {
                    $.each(result.data, function (e, t) {
                        $("#F_SupplierFrom").append($("<option>", { value: t, text: t }, "</option>"));
                        $("#F_SupplierTo").append($("<option>", { value: t, text: t }, "</option>"));
                    });
                    $('#supFromBlank').hide();
                    $('#supToBlank').hide();
                }
                else {
                    xSwal.error(result.title, result.message);
                }
                //console.log(result);
            },
            error: function (result) {
                console.error(result);
            },
        });
    });

    xAjax.onChange("F_DeliveryFrom, #F_DeliveryTo", async function () { //, #
        var devDate = $("#F_DeliveryFrom").val();
        var toDate = $("#F_DeliveryTo").val();
        var type = $('input[name="radio"]').filter(":checked").val();
        xAjax.Post({
            url: 'KBNCR220/Initial',
            data: {
                devDate: devDate,
                toDate: toDate,
                type: type
            },
            then: function (result) {
                // console.log(result);
                clearOption();
                clearTable();
                $.each(result.data, function (e, t) {
                    $("#F_SupplierFrom").append($("<option>", { value: t, text: t }, "</option>"));
                    $("#F_SupplierTo").append($("<option>", { value: t, text: t }, "</option>"));
                });
                $('#supFromBlank').hide();
                $('#supToBlank').hide();
            },
            error: function (result) {
                console.error(result);
            },
        });
    });

    xAjax.onClick("#SearchBtn", function () {
        var devDate = $("#F_DeliveryFrom").val().replaceAll('-','');
        var toDate = $("#F_DeliveryTo").val().replaceAll('-', '');
        if (devDate > toDate) {
            return alert("Please Don't select Delivery date to less than Delivery date from");
        }
        var supFrom = $("#F_SupplierFrom").val().substring(0, 4);
        var supTo = $("#F_SupplierTo").val().substring(0, 4);
        if (supFrom > supTo) {
            return alert("Please Don't select Supplier To less than Supplier From");
        }

        var type = $('input[name="radio"]').filter(":checked").val();
        xAjax.Post({
            url: 'KBNCR220/Search',
            data: {
                devDate: devDate,
                toDate: toDate,
                supFrom: supFrom,
                supTo: supTo,
                type: type
            },
            then: function (result) {
                if (result.status == "200") {
                    if (result.data != null) {
                        clearTable();
                        $('#tblMaster').dataTable().fnAddData(result.data);
                    }
                }
                else {
                    xSwal.error(result.title, result.message);
                }
            },
            error: function (result) {
                console.error(_Controller + '.Search: ' + result.responseText);
                xSplash.hide();
            }
        });
    });

    xAjax.onClick("#ReportBtn", function () {
        var devDate = $("#F_DeliveryFrom").val().replaceAll('-', '');
        var toDate = $("#F_DeliveryTo").val().replaceAll('-', '');
        if (devDate > toDate) {
            return alert("Please Don't select Delivery date to less than Delivery date from");
        }
        var supFrom = $("#F_SupplierFrom").val().substring(0, 4);
        var supTo = $("#F_SupplierTo").val().substring(0, 4);
        if (supFrom > supTo) {
            return alert("Please Don't select Supplier To less than Supplier From");
        }
        if (supFrom == "") {
            supFrom = "0000";
        }
        if (supTo == "") {
            supTo = "9999";
        }

        var type = $('input[name="radio"]').filter(":checked").val();
        var start1 = "";
        var start2 = "";

        if (type === "All") {
            start1 = "7Z"
            start2 = "7Y"
        }
        else if (type === "7Z") {
            start1 = "7Z"
            start2 = "7Z"
        }
        else if (type === "7Y") {
            start1 = "7Y"
            start2 = "7Y"
        }

        var reportUrl = "http://hmmt-app03/Reportserver/report/KB3/";

        window.location.href = reportUrl + 'KBNCR220' + '?spcDateFrom=' + devDate + '&spcDateTo=' + toDate +
            '&spcSupFrom=' + supFrom + '&spcSupTo=' + supTo + '&spcType=' + type +
            '&spcStart1=' + start1 + '&spcStart2=' + start2;
    });
});