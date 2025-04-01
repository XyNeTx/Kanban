$(document).ready(async function () {

    $("#table").DataTable({
        width: '100%',
        paging: false,
        scrollCollapse: true,
        "processing": false,
        "serverSide": false,
        scrollX: false,
        scrollY: '350px',
        select : true,
        columns: [
            {
                title: '<input type="checkbox" class="chkBoxHeadDT" id="chkBoxHeadID" name="chkBoxHeadName"/>', width: '10%', render: function (data, type, row, meta) {
                    return '<input type="checkbox" class="chkBoxDT" name="inputChkBox" value=' + meta.row + ' checked>';
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

var _command = '';
$("#buttonInq").click(function () {
    _command = 'Inquiry';
    $("#buttonNew").prop("disabled", true);
    $("#buttonUpd").prop("disabled", false);
    $("#buttonDel").prop("disabled", false);
    _xLib.AJAX_Get('/api/KBNIM007N/Inquiry', '',
        function (success)
        {
            if (success.status === "200") {
                $("#txtOrderNo").remove();
                $("#labelOrderNo").append(`<select id="txtOrderNo" class="form-select ms-1"></select>`);
                $("#txtOrderNo").append('<option value="" hidden></option>');
                (JSON.parse(success.data)).forEach(function (item) {
                    $("#txtOrderNo").append('<option value="' + item.F_PDS_No + '">' + item.F_PDS_No + '</option>');
                });
                $("#inputQty").prop("readonly", true);
                $("#inputPack").prop("readonly", true);
                $("#inputPartNo").prop("disabled", true);
                $("#inputKanbanNo").prop("disabled", true);
            }
        },
        function (err)
        {
            $("#buttonNew").prop("disabled", false);
            $("#buttonUpd").prop("disabled", true);
            $("#buttonDel").prop("disabled", true);
            return xSwal.error("Error !!", err.responseJSON.message);
        }
    )
});

$("#buttonUpd").click(function () {
    _command = 'Update';
    $("#buttonNew").prop("disabled", true);
    $("#buttonInq").prop("disabled", true);
    $("#buttonDel").prop("disabled", true);
    $("#buttonOK").prop("disabled", false);
    $("#buttonImport").prop("disabled", false);
    $("#inputQty").prop("readonly", false);
    $("#inputPack").prop("readonly", false);
    $(".chkBoxDT").prop("checked", false);
});

$("#buttonDel").click(function () {
    _command = 'Delete';
    $("#buttonNew").prop("disabled", true);
    $("#buttonInq").prop("disabled", true);
    $("#buttonUpd").prop("disabled", true);
    $("#buttonOK").prop("disabled", false);
    $("#buttonImport").prop("disabled", false);
    $("#inputQty").prop("readonly", false);

});

$(document).on("change", "#txtOrderNo", function () { // when select Order No
    if (_command === 'Inquiry') {
        _xLib.AJAX_Get('/api/KBNIM007N/OrderNoSelected', { F_PDS_No: $("#txtOrderNo").val().trim() },
            async function (success) {
                if (success.status === "200") {
                    success.data = JSON.parse(success.data);
                    success.data = _xLib.TrimJSON(success.data);
                    console.log(success.data);
                    await $("#selectSupplier").val(success.data[0].F_Supplier_CD + "-" + success.data[0].F_Supplier_Plant);
                    $("#selectSupplier").trigger("change");
                    $("#table").DataTable().clear().rows.add(success.data).draw();
                }
            },
            function (error) {
                return xSwal.error("Error !!", error.responseJSON.message);
            }
        );
    }
});

$(document).on("click", "#table tbody tr", function () {
    if (_command === 'Update' || _command === "Inquiry") {
        var DatainRow = $("#table").DataTable().row(this).data();
        $("#inputPartNo").val(DatainRow.F_Part_No);
        $("#inputPartNo").prop("disabled", true);

        $("#inputKanbanNo").val(DatainRow.F_Kanban_No);
        $("#inputKanbanNo").prop("disabled", true);

        $("#inputQtyPack").val(DatainRow.F_Qty_Pack);
        $("#inputPack").val(DatainRow.F_Pack);
        $("#inputQty").val(DatainRow.F_Qty);

        $("#inputDeliveryDate").val(DatainRow.F_Delivery_Date.slice(0, 4) + "-" + DatainRow.F_Delivery_Date.slice(4, 6) + "-" + DatainRow.F_Delivery_Date.slice(6, 8));
        $("#inputDeliveryDate").prop("disabled", true);

        $("#inputDeliveryTrip").val(DatainRow.F_Delivery_Trip);
        $("#inputDeliveryTrip").prop("disabled", true);

        $("#txtStoreCode").val(DatainRow.F_Store_CD);
    }
});


$("#buttonNew").click(function () {
    _command = "New";
    $("#buttonInq").prop("disabled", true);
    $("#buttonUpd").prop("disabled", true);
    $("#buttonDel").prop("disabled", true);

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

$(document).on('change',"#selectSupplier , #inputDeliveryDate",function () {
    _xLib.AJAX_Get('/api/KBNIM007N/GetSupplierDetail',
        {
            F_Supplier_Cd: $("#selectSupplier").val(),
            F_ProcessDate: $("#inputDeliveryDate").val().replaceAll("-", "")
        },
        function (success) {
            if (success.status === "200") {
                _xLib.TrimJSON(success.data)
                _xLib.TrimJSON(success.data2)
                _xLib.TrimArrayJSON(success.data3)
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

$("#inputQty").on("keypress", async function (e) {
    if (e.which === 13) {
        if ($("#inputPack").val() !== Math.ceil($("#inputQty").val() / $("#inputQtyPack").val())) {
            await $("#inputPack").val(0);
        }
        $("#inputDeliveryTrip").trigger(e)
    }
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
        let CycleB = $("#txtCycleTime").val().split("-")[1];
        let DeliveryTrip = $("#inputDeliveryTrip").val();
        if (parseInt(DeliveryTrip) > parseInt(CycleB)) {
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

        if (_command === 'Update') {
            var selectedRow = $("#table").find(".selected").closest("tr");
            $("#table").DataTable().row(selectedRow).remove().draw();
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
    xSplash.show("Saving Data");
    if (_command === 'New') {
        $("#buttonOK").prop("disabled", true);
        var rows = $("input[name='inputChkBox']:checked").closest("tr");
        if (rows.length === 0) return xSwal.error("Error !!", "No data selected.");
        let _arrObj = $("#table").DataTable().rows(rows).data().toArray();

        return await OkClicked(_arrObj);
    }
    else if (_command === 'Update') {
        $("#buttonOK").prop("disabled", true);
        var rows = $("input[name='inputChkBox']:checked").closest("tr");
        if (rows.length === 0) return xSwal.error("Error !!", "No data selected.");
        let _arrObj = $("#table").DataTable().rows(rows).data().toArray();
        var _hasError = false;
        _arrObj.forEach(async function (item, i) {
            item.Supplier = item.F_Supplier;
            item.Part_No = item.F_Part_No;
            item.Qty = item.F_Qty;
            item.Pack = item.F_Pack;
            item.Delivery_Date = item.F_Delivery_Date;
            item.Delivery_Trip = item.F_Delivery_Trip;
            item.Kanban_No = item.F_Kanban_No;

            delete item.F_Supplier;
            delete item.F_Part_No;
            delete item.F_Qty;
            delete item.F_Pack;
            delete item.F_Delivery_Date;
            delete item.F_Delivery_Trip;
            delete item.F_Kanban_No;

            await _xLib.AJAX_Post('/api/KBNIM007N/Update', JSON.stringify(item),
                function (success) {
                    if (success.status === "200") {
                        console.log("Success: ", success);
                        if ($("input[name='inputChkBox']:checked").length > 0) {
                            $("#table").DataTable().row(rows[i]).remove().draw();
                            $("#buttonInq").trigger("click");
                        }
                    }
                },
                function (error) {
                    _hasError = true;
                    return xSwal.error("Error !!", error.responseJSON.message);
                }
            );
        });
        if (_hasError) return;
        else {
            $("#table").DataTable().clear().draw();
            xSplash.hide();
            return xSwal.success("Success !!", "Data was updated.");
        }
    }
    else if (_command === "Delete") {
        xSwal.question("Confirm !!", "Are you sure to delete data?", async function () {
            $("#buttonOK").prop("disabled", true);
            var rows = $("input[name='inputChkBox']:checked").closest("tr");
            if (rows.length === 0) return xSwal.error("Error !!", "No data selected.");
            let _Obj = $("#table").DataTable().row(rows[0]).data();
            await _xLib.AJAX_Post('/api/KBNIM007N/Delete', JSON.stringify(_Obj),
                function (success) {
                    if (success.status === "200") {
                        console.log("Success: ", success);
                        if ($("input[name='inputChkBox']:checked").length > 0) {
                            $("#table").DataTable().clear().draw();
                            $("#buttonInq").trigger("click");
                            xSplash.hide();
                            return xSwal.success("Success !!", "Data was deleted.");
                        }
                    }
                },
                function (error) {
                    return xSwal.error("Error !!", error.responseJSON.message);
                }
            );
        });
    }
    else {
        return xSwal.error("Error !!", "Please select command.");
    }


});


async function OkClicked(_arrObj,isImport) {
    //return console.log(_arrObj);

    for (const item in _arrObj) {
        if (await _arrObj[item].hasOwnProperty("F_Delivery_Date")) {
            _arrObj[item].Delivery_Date = _arrObj[item].F_Delivery_Date;
            _arrObj[item].Delivery_Trip = _arrObj[item].F_Delivery_Trip;
            _arrObj[item].Part_No = _arrObj[item].F_Part_No;
            _arrObj[item].Supplier = _arrObj[item].F_Supplier;
            _arrObj[item].Qty = _arrObj[item].F_Qty;
            _arrObj[item].Kanban_No = _arrObj[item].F_Kanban_No;
            _arrObj[item].Pack = _arrObj[item].F_Pack;

            if (_arrObj[item].F_Remark) {
                _arrObj[item].Remark = _arrObj[item].F_Remark;
                delete _arrObj[item].F_Remark;
            }

            delete _arrObj[item].F_Delivery_Date;
            delete _arrObj[item].F_Delivery_Trip;
            delete _arrObj[item].F_Part_No;
            delete _arrObj[item].F_Supplier;
            delete _arrObj[item].F_Qty;
            delete _arrObj[item].F_Kanban_No;
            delete _arrObj[item].F_Pack;
            
        }
        _arrObj[item].Delivery_Date = _arrObj[item].Delivery_Date.toString();
        var _json = JSON.stringify(_arrObj[item]);

        await _xLib.AJAX_Post('/api/KBNIM007N/OKClicked', _json,
            function (success) {
                if (success.status === "200") {
                    console.log("Success: ", success);
                    if ($("input[name='inputChkBox']:checked").length > 0) {
                        $("#table").DataTable().row().remove().draw();
                    }
                }

            },
            function (error) {
                return xSwal.error("Error !!", error.responseJSON.message);
            }
        );
    }

    let PDS_No = $("#txtOrderNo").val();

    PDS_No = isImport ? PDS_No + "IMP" : PDS_No;

    await _xLib.AJAX_Get('/api/KBNIM007N/AllDataWasSaved', { F_PDS_No: PDS_No },
        function (success) {
            if (success.status === "200") {
                return xSwal.success(success.title, success.message);
            }
        },
        function (error) {
            if (error.responseJSON.message.includes("Have Some Error")) {
                xSwal.error("Error !!", error.responseJSON.message);
                return _xLib.OpenReport("/KBNIMERR", `UserID=${error.responseJSON.userid}&Type=${error.responseJSON.type}`);
            }
            xSwal.error("Error !!", error.responseJSON.message);
        }
        
    )

    $("#frmCondition").trigger("reset");
    $("#table").DataTable().clear().draw();
    $("#buttonOK").prop("disabled", false);
    $("#buttonImport").prop("disabled", false);

    xSplash.hide();
    return;
}



let _File = [];
$("#inputFile").change(function (e) {
    _File = e.target.files;
});
$("#buttonImport").click(async function () {
    xSplash.show("Saving Data");
    $("#buttonImport").prop("disabled", true);
    if (!_File.length) return xSwal.error("Error !!", "No file selected.");
    const file = _File[0];
    const arrayBuffer = await file.arrayBuffer();
    const read = await XLSX.read(arrayBuffer);
    let _editRead = read.Sheets[read.SheetNames[0]];

    console.log("Edit Read: ", _editRead);

    for (let _edit in _editRead) {
        // Edit Sheet
        _editRead[_edit].v = _editRead[_edit].w;
    }

    const _arrObj = XLSX.utils.sheet_to_json(read.Sheets[read.SheetNames[0]]);

    //return console.log('Data: ', _arrObj);

    return await OkClicked(_arrObj, true);
});
