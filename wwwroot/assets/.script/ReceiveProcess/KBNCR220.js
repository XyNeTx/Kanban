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
        console.log(type);
        xAjax.Post({
            url: 'KBNCR220/Initial',
            data: {
                devDate: devDate,
                toDate: toDate,
                type: type
            },
            then: function (result) {
                $('#F_SupplierFrom').find('option').remove().end();
                $('#F_SupplierTo').find('option').remove().end();
                $("#F_SupplierFrom").append($('<option id="supFromBlank"></option>'));
                $("#F_SupplierTo").append($('<option id="supToBlank"></option>'));
                $.each(result.data, function (e, t) {
                    $("#F_SupplierFrom").append($("<option>", { value: t, text: t }, "</option>"));
                    $("#F_SupplierTo").append($("<option>", { value: t, text: t }, "</option>"));
                });
                $('#supFromBlank').hide();
                $('#supToBlank').hide();
                console.log(result);
                xSplash.hide();
            },
            error: function (result) {
                console.error(result);
            },
        });
    });

    xAjax.onChange("F_DeliveryFrom, #F_DeliveryTo", function () { //, #
        var devDate = $("#F_DeliveryFrom").val();
        var toDate = $("#F_DeliveryTo").val();
        var type = $('input[name="radio"]').filter(":checked").val();
        console.log(devDate + "devdateeeeeee \n" + toDate + "toDateeeeee \n");
        if (toDate < devDate) {
            console.error("วันเกินแล้ววววววววว");
        }
        var type = document.querySelector('input[name="radio"]:checked').value;
        xAjax.Post({
            url: 'KBNCR220/Initial',
            data: {
                devDate: devDate,
                toDate: toDate,
                type: type
            },
            success: function (result) {
                console.log(result + "Line 60");
                $.each(result, function (e, t) {
                    $("#supFromBlank").append($("<option>", { value: t.id, text: t.id + ": " + t.name }));
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
        xAjax.Post({
            url: 'KBNCR220/Check',
            data: {
                'JsonData': 'test',
            },
            then: function (result) {
                if (result.response == "OK") {
                    if (result.data != null) {
                        $('#tblMaster').dataTable().fnAddData(result.data);
                    }
                }
                else {
                    xSwal.error("Get Receive Special Report Error", "Error!!!");
                }
            },
            error: function (result) {
                console.error(_Controller + '.SearchPDSNo: ' + result.responseText);
                xSplash.hide();
            }
        });
    });
});