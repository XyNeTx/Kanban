$(document).ready(async function () {

    $("#table").DataTable({
        width: '100%',
        paging: false,
        scrollCollapse: true,
        "processing": false,
        "serverSide": false,
        scrollX: false,
        scrollY: '350px',
        columns: [
            {
                title: '<input type="checkbox" name="ChkAll"/>', width: '10%', render: function (data, type, row, meta) {
                    return '<input type="checkbox" name="inputChkBox" value=' + meta.row + '>';
                },
                orderable: false
            },
            { title: 'Part No', data: 'F_Part_No', width: '20%' },
            { title: 'Kanban No', data: 'F_Kanban_No', width: '10%' },
            { title: 'Qty/Pack', data: 'F_Qty_Pack', width: '10%' },
            { title: 'Pack', data: 'F_Pack', width: '10%' },
            { title: 'Qty', data: 'F_Qty', width: '10%' },
            { title: 'Delivery Date', data: 'F_Delivery_Date', width: '15%' },
            { title: 'Delivery Trip', data: 'F_Round', width: '15%' },
        ],
        order: [[1, 'asc']]
    });

    xSplash.hide();
});
$("#buttonNew").click(function () {
    _xLib.AJAX_Get('/api/KBNIM007N/GenPDSNo', '',
        function(success) {
            if (success.status === "200") {
                $("#txtOrderNo").val(success.data);
            }
        },
        function(error) {
            return xSwal.error("Error !!", "Can't New PDS No.");
        }
    );

    _xLib.AJAX_Get('/api/KBNIM007N/GetSupplier', '',
        function (success) {
            if (success.status === "200") {
                $("#selectSupplier").empty();
                $("#selectSupplier").append('<option value="" hidden></option>');
                success.data.forEach(function (item) {
                    $("#selectSupplier").append('<option value="' + item.f_Supplier_Cd + '">' + item.f_Supplier_Cd + '</option>');
                });
            }
        },
        function (error) {
            return xSwal.error("Error !!", error.responseJSON.message);
        }
    );

    $("#selectSupplier").prop("disabled", false);
});

$("#selectSupplier").change(function () {
    _xLib.AJAX_Get('/api/KBNIM007N/GetSupplierDetail', { F_Supplier_Cd: $("#selectSupplier").val() },
        function (success) {
            if (success.status === "200") {
                _xLib.TrimJSON(success)
                //console.log(success.data);
                $("#txtSupplierShortName").val(success.data.f_short_name);
                $("#txtSupplierName").val(success.data.f_name);
                $("#txtCycleTime").val(success.data2.f_Cycle);
                $("#inputKanbanNo").empty();
                $("#inputKanbanNo").append('<option value="" hidden></option>');
                success.data3.forEach(function (item) {
                    $("#inputKanbanNo").append('<option value="' + item.f_Kanban_No + '">' + item.f_Kanban_No + '</option>');
                });
            }
        },
        function (error) {
            $("#txtSupplierShortName").val('');
            $("#txtSupplierName").val('');
            $("#txtCycleTime").val('');
            return xSwal.error("Error !!", error.responseJSON.message);
        }
    );
    _xLib.AJAX_Get('/api/KBNIM007N/GetPartNo', { F_Supplier_Cd: $("#selectSupplier").val() },
        function (success) {
            if (success.status === "200") {
                $("#inputPartNo").empty();
                //console.log(JSON.parse(success.data));
                $("#inputPartNo").append('<option value="" hidden></option>');
                JSON.parse(success.data).forEach(function (item) {
                    $("#inputPartNo").append('<option value="' + item.F_Part_No + '">' + item.F_Part_No + '</option>');
                });
            }
        },
        function (error) {
            $("#inputPartNo").empty();
            return xSwal.error("Error !!", error.responseJSON.message);
        }
    )
});

$("#inputPartNo, #inputKanbanNo").change(function () {
    var obj = {
        F_Part_No: $("#inputPartNo").val(),
        F_Kanban_No: $("#inputKanbanNo").val(),
        F_Supplier_Cd : $("#selectSupplier").val()
    };

    _xLib.AJAX_Get('/api/KBNIM007N/PartNoChanged', obj,
        function (success) {
            if (success.status === "200") {
                success.data = JSON.parse(success.data);
                _xLib.TrimJSON(success.data);
                $("#txtStoreCode").val(success.data[0].F_Store_CD);
                $("#inputPartNo").val(success.data[0].F_Part_No);
                $("#inputKanbanNo").val(success.data[0].F_Kanban_No);
                $("#inputQtyPack").val(success.data[0].F_qty_box);
            }
        },
        function (error) {
            $("#inputPartNo").val('');
            $("#inputKanbanNo").val('');
            $("#inputQtyPack").val('');
            $("#txtStoreCode").val('');
            return xSwal.error("Error !!", error.responseJSON.message);
        }
    )
});

$("#inputPack").change(function () {
    $("#inputQty").val($("#inputQtyPack").val() * $("#inputPack").val());
});
$("#inputQty").change(function () {
    var roundup = Math.ceil($("#inputQty").val() / $("#inputQtyPack").val());
    $("#inputPack").val(roundup);
});
$("#inputDeliveryTrip").on("keypress", function (e) {
    //console.log(e.which);
    if (e.which === 13) { // when press enter
        if(!($("#inputPartNo").val())) {
            return xSwal.error("Error !!", "Please select Part No.");
        }
        if(!($("#inputKanbanNo").val())) {
            return xSwal.error("Error !!", "Please select Kanban No.");
        }
        if(!($("#inputPack").val())) {
            return xSwal.error("Error !!", "Please input Pack.");
        }
        if(!($("#inputQty").val())) {
            return xSwal.error("Error !!", "Please input Qty.");
        }
        if (!($("#txtCycleTime").val())) {
            return xSwal.error("Error !!", "Please select Supplier.");
        }
        let CycleB = $("#txtCycleTime").val().split("0")[1];
        let DeliveryTrip = $(this).val();
        if (DeliveryTrip > CycleB) {
            return xSwal.error("Error !!", "Delivery Trip is over than Cycle Time.");
        }

        var obj = {
            F_Part_No: $("#inputPartNo").val(),
            F_Kanban_No: $("#inputKanbanNo").val(),
            F_Qty_Pack: $("#inputQtyPack").val(),
            F_Pack: $("#inputPack").val(),
            F_Qty: $("#inputQty").val(),
            F_Delivery_Date: $("#inputDeliveryDate").val().replaceAll("-",""),
            F_Round: DeliveryTrip,
            F_Supplier_CD: $("#selectSupplier").val().split("-")[0],
            F_Supplier_Plant: $("#selectSupplier").val().split("-")[1],
            F_Store_CD: $("#txtStoreCode").val(),
            F_PDS_No: $("#txtOrderNo").val()
        }

        $("#table").DataTable().row.add(obj).draw();

        $("#inputPartNo").val('');
        $("#inputKanbanNo").val('');
        $("#inputQtyPack").val('');
        $("#inputPack").val('');
        $("#inputQty").val('');
        $("#inputDeliveryTrip").val('');
        $("#txtStoreCode").val('');
    }
});