"use strict";
var pdsSet = new Set();
$(document).ready(function () {
    const table = $('#tblMaster').DataTable({
        columns: [
            { data: "F_No" },
            { data: "F_Part_No" },
            { data: "F_Part_Name" },
            { data: "F_Receive_Date" },
            { data: "F_Unit_Amount" },
            { data: "F_Receive_amount" },
            { data: "F_Dev_Qty" }
        ],
        order: [[0, 'asc']],
        paging: false,
    });
    var isEditing = false;
    $('#tblMaster tbody').on('click', 'td', function () {
        if (isEditing) {
            return;
        }
        var columnIndex = table.cell(this).index().column;
        var cell = table.cell(this);
        var originalValue = cell.data();
        const orderQty = table.cell({ row: cell.index().row, column: 4 }).data();
        const alreadyQty = table.cell({ row: cell.index().row, column: 5 }).data();
        if (columnIndex === 6 && orderQty == alreadyQty) {
            return xSwal.error("Receive Seperate Error", "This Part Have Been Received.!");
        }
        else if (columnIndex === 6) {
            var inputField = $('<input class="form-control F_Delivery_Qty_Class" type="number" id="F_Delivery_Qty"/>').val(originalValue);
            cell.data(inputField[0].outerHTML).draw();
            isEditing = true;
            $('.F_Delivery_Qty_Class').focus();
            $('.F_Delivery_Qty_Class').on('keypress blur', function (e) {
                var devQty = $(this).val();
                //console.log(devQty);
                //console.log(e);
                if (e.type === 'keypress' && e.which !== 13) {
                    return;
                }
                if (devQty === "0") {
                    cell.data(0).draw();
                    isEditing = false;
                }
                else if (devQty === null || devQty === '' || devQty < 0) {
                    cell.data(originalValue).draw();
                    isEditing = false;
                }
                else if (parseInt(devQty) + parseInt(alreadyQty) > (parseInt(orderQty))) {
                    xSwal.error("Input Delivery Qty Error!", "Dont't input Delivery Qty. more than difference of Order Qty. and Already Dev.");
                }
                else {
                    cell.data(devQty).draw();
                    isEditing = false;
                }
            });
        }
    });
    document.addEventListener("wheel", function (event) {
        if (document.activeElement.type === "number") {
            document.activeElement.blur();
        }
    });
    xSplash.hide();
    xAjax.onEnter('#F_PDS_No', function () {
        var pdsNo = $('#F_PDS_No').val();
        // console.log(pdsNo);
        xAjax.Post({
            url: 'KBNRC210/CheckPDSNo',
            data: {
                'F_PDS_No': pdsNo
            },
            then: function (result) {
                if (result.response == "OK") {
                    if (result.data != null) {
                        //$('#F_PDS_No').val("");
                        // console.log(pdsSet.size + "90");
                        if (pdsSet.size >= 1) {
                            xSwal.error("PDS No. Should have 1 only", "Please receive current PDS No. before");
                        }
                        else {
                            // console.log(result + "line 88");
                            $('#tblMaster').dataTable().fnAddData(result.data);
                            pdsSet.add(pdsNo);
                            // console.log(pdsSet.size + "98")
                        }
                    }
                    else {
                        $('#F_PDS_No').val("");
                        xSwal.error(result.title, result.message);
                    }
                }
            },
            error: function (result) {
                console.error(_Controller + '.SearchPDSNo: ' + result.responseText);
                xSplash.hide();
            }
        });
    });
    xAjax.onClick("#ClearBtn", function () {
        $('#tblMaster').DataTable().clear();
        $('#tblMaster').DataTable().draw();
        pdsSet.clear();
        isEditing = false;
    });
    xAjax.onClick("#ReceiveBtn", function () {
        try {
            xSplash.show();
            $("#ReceiveBtn").prop("disabled", true);
            if (isEditing) {
                return alert("Please Save Edit Dev.Qty Before Receive Part!");
            }
            var _selData = [];
            // Get the column names from the DataTable
            var columnNames = $('#tblMaster').DataTable().columns().header().toArray().map(function (th) {
                return $(th).text().trim();
            });
            //console.log(columnNames);
            // Iterate over the rows of the DataTable
            $('#tblMaster').DataTable().rows().nodes().each(function (row, index) {
                var rowData = {};
                // Iterate over each cell in the row
                $(row).find('td').each(function (cellIndex) {
                    // Add the data to the rowData object using the corresponding column name
                    rowData[columnNames[cellIndex]] = $(this).text().trim();
                });
                // Push the rowData object to the _selData array
                _selData.push(rowData);
            });
            _selData.push({ "PDSNo": pdsSet.values().next().value });
            // console.log(_selData);
            $("#tblMaster").DataTable().clear().draw();
            pdsSet.clear();
            xAjax.Post({
                url: 'KBNRC210/Receive',
                data: {
                    'JsonData': _selData,
                },
                then: function (result) {
                    // console.log(result)
                    if (result.status == "200") {
                        xSwal.success(result.title, result.message);
                        $('#tblMaster').DataTable().clear();
                        $('#tblMaster').DataTable().draw();
                        pdsSet.clear();
                    }
                    else {
                        xSwal.error(result.title, result.message);
                    }
                },
                error: function (result) {
                    xSplash.hide();
                    xSwal.error("Error", "An error occurred during the request.");
                    console.error(_Controller + '.ReceiveSeparate: ' + result.responseText);
                }
            });
        }
        catch (e) {
            console.error(e);
            xSplash.hide();
            xSwal.error("Error", "An error occurred during the request.");
        }
        finally {
            xSplash.hide();
            $("#ReceiveBtn").prop("disabled", false);
        }
    });
});
