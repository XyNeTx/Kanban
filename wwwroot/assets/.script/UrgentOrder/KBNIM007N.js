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
                title: '<input type="checkbox" class="chkBoxHeadDT" id="chkBoxHeadID" name="chkBoxHeadName"/>', width: '10%', render: function (data, type, row, meta) {
                    return '<input type="checkbox" class="chkBoxDT" name="inputChkBox" value=' + meta.row + '>';
                },
                orderable: false
            },
            { title: 'Part No', data: 'F_Part_No', width: '20%' },
            { title: 'Kanban No', data: 'F_Kanban_No', width: '10%' },
            { title: 'Qty/Pack', data: 'F_Qty_Pack', width: '10%' },
            { title: 'Pack', data: 'F_Pack', width: '10%' },
            { title: 'Qty', data: 'F_Qty', width: '10%' },
            { title: 'Delivery Date', data: 'F_Delivery_Date', width: '15%' },
            { title: 'Delivery Trip', data: 'F_Delivery_Trip', width: '15%' },
        ],
        order: [[1, 'asc']]
    });

    xSplash.hide();
});

$(document).on("click", "#chkBoxHeadID", async function () {
    if ($(this).is(":checked")) {
        $(".chkBoxDT").prop("checked", true);
    }
    else {
        $(".chkBoxDT").prop("checked", false);
    }
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
            F_Delivery_Trip: DeliveryTrip,
            F_Supplier: $("#selectSupplier").val(),
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

$("#buttonOK").click(async function () {
    var rows = $("input[name='inputChkBox']:checked").closest("tr");
    if (rows.length === 0) return xSwal.error("Error !!", "No data selected.");
    let _arrObj = $("#table").DataTable().rows(rows).data().toArray();

    return await OkClicked(_arrObj);
});


async function OkClicked(_arrObj) {

    for (const item in _arrObj) {
        if (await _arrObj[item].hasOwnProperty("F_Delivery_Date")) {
            _arrObj[item].Delivery_Date = _arrObj[item].F_Delivery_Date;
            _arrObj[item].Delivery_Trip = _arrObj[item].F_Delivery_Trip;
            _arrObj[item].Part_No = _arrObj[item].F_Part_No;
            _arrObj[item].Supplier = _arrObj[item].F_Supplier;
            _arrObj[item].Qty = _arrObj[item].F_Qty;
            _arrObj[item].Kanban_No = _arrObj[item].F_Kanban_No;

            delete _arrObj[item].F_Delivery_Date;
            delete _arrObj[item].F_Delivery_Trip;
            delete _arrObj[item].F_Part_No;
            delete _arrObj[item].F_Supplier;
            delete _arrObj[item].F_Qty;
            delete _arrObj[item].F_Kanban_No;
        }
        _arrObj[item].Delivery_Date = _arrObj[item].Delivery_Date.toString();
        var _json = JSON.stringify(_arrObj[item]);

        await _xLib.AJAX_Post('/api/KBNIM007N/OKClicked', _json,
            function (success) {
                if (success.status === "200") {
                    console.log("Success: ", success);
                    if ($("input[name='inputChkBox']:checked").length > 0) {
                        $("#table").DataTable().row(rows[item]).remove().draw();
                    }
                }
            },
            function (error) {
                return xSwal.error("Error !!", error.responseJSON.message);
            }
        );
    }

    await _xLib.AJAX_Get('/api/KBNIM007N/AllDataWasSaved', { F_PDS_No: $("#txtOrderNo").val() },
        function (success) {
            if (success.status === "200") {
                return xSwal.success(success.title, success.message);
            }
        },
        function (error) {
            return xSwal.error("Error !!", error.responseJSON.message);
        }
    )

    $("#frmCondition").trigger("reset");
    $("#table").DataTable().clear().draw();
    return;
}



let _File = [];
$("#inputFile").change(function (e) {
    _File = e.target.files;
});
$("#buttonImport").click(async function () {
    if (!_File.length) return xSwal.error("Error !!", "No file selected.");
    const file = _File[0];
    const arrayBuffer = await file.arrayBuffer();
    const read = await XLSX.read(arrayBuffer);
    const _arrObj = XLSX.utils.sheet_to_json(read.Sheets[read.SheetNames[0]]);
    //console.log('Data: ', _arrObj);

    return await OkClicked(_arrObj);
});
