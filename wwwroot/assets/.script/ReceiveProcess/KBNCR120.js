$(document).ready(function () {
    var pdsSet = new Set();

    const table = $('#tblMaster').DataTable({
        columns: [
            { data: "No"},
            { data: "F_Part_No" },
            { data: "F_Receive_Date" },
            { data: "F_Unit_Amount" },
            { data: "F_Receive_amount" },
            { data: "F_Dev_Qty"}
        ],
        order: [[0, 'asc']],
    });

    var isEditing = false;

    $('#tblMaster tbody').on('click', 'td', function () {
        if (isEditing) {
            return;
        }
            var columnIndex = table.cell(this).index().column;
            var cell = table.cell(this);
            var originalValue = cell.data();
            const orderQty = table.cell({ row: cell.index().row, column: 3 }).data();
            console.log(orderQty);

            if (columnIndex === 5) {

                var inputField = $('<input class="form-control F_Delivery_Qty_Class" type="number" id="F_Delivery_Qty" min="0" max="5000"/>').val(originalValue);

                cell.data(inputField[0].outerHTML).draw();
                $('.F_Delivery_Qty_Class').focus();
                $('.F_Delivery_Qty_Class').on('keypress', function (e) {
                    if (e.which === 13) {
                        var newValue = $(this).val();
                        //console.log(newValue);
                        if (newValue === "0" || newValue === null || newValue === '' || newValue < 0) {
                            alert("Please Input Delivery Qty Correctly.");
                        }
                        else if (parseInt(newValue) > parseInt(orderQty)) {
                            alert("Can't Input Delivery Qty more than Order Qty.");
                        }
                        else {
                            cell.data(newValue).draw();
                            isEditing = false;
                        }
                    }
                    else if (e.which === 99) {
                        cell.data(orderQty).draw();
                        isEditing = false;
                    }
                });
                isEditing = true; 
            }
    });

    document.addEventListener("wheel", function (event) {
        if (document.activeElement.type === "number") {
            document.activeElement.blur();
        }
    });
    xSplash.hide();
    onSave = function () {
        KBNCR120.save(function () {
            KBNCR120.search();
        });
    }

    onDelete = function () {
        KBNCR120.delete(function () {
            KBNCR120.search();
        });
    }

    onDeleteAll = function () {
        KBNCR120.deleteall(function () {
            KBNCR120.search();
        });
    }

    onPrint = function () {
        xDataTableExport.setConfigPDF({
            title: 'OLD TYPE SERVICE CHECK LIST'
        });
    }

    onExecute = function () {
        console.log('onExecute');
    }

    xAjax.onEnter('#F_PDS_No', function () {
        if (isEditing) {
            return alert("Please Save Edit Dev.Qty Before Receive Part!");
        }
        var pdsNo = $('#F_PDS_No').val();
        // console.log(pdsNo);
        xAjax.Post({
            url: 'KBNCR120/SearchPDSNo',
            data: {
                'F_PDS_No': pdsNo
            },
            then: function (result) {
                if (result.response == "OK") {
                    if (result.data != null) {
                        $('#F_PDS_No').val("");
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
        var _selData = [];

        // Get the column names from the DataTable
        var columnNames = $('#tblMaster').DataTable().columns().header().toArray().map(function (th) {
            return $(th).text().trim();
        });

        console.log(columnNames);

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

        console.log(_selData);
    });


    //xAjax.onClick("#ReceiveBtn", function () {
    //    var _selData = [];
    //    var allPages = $('#tblMaster').DataTable().tables().nodes();
    //    console.log(allPages);
    //    $(allPages).find('td').each(function () {
    //        var _val = $($(this)).text();
    //        console.log(_val);
    //        _selData.push(JSON.parse(`{` + ReplaceAll(_val, `'`, `"`) + `}`));;
    //    });
    //    console.log(_selData);

    //    xAjax.Post({
    //        url: 'test',
    //        data: {
    //            'F_PDS_No': _selData,
    //        },
    //    });
    //});


    //const KBNCR120 = new MasterTemplate({
    //    Controller: _PAGE_,
    //    Table: 'tblMaster',
    //    ColumnTitle: {
    //        "EN": ['Part No.', 'Delivery Date', 'Order Qty', 'Already Dev.','Dev. Qty'],
    //        "TH": ['Part No.', 'Delivery Date', 'Order Qty', 'Already Dev.', 'Dev. Qty'],
    //        "JP": ['Part No.', 'Delivery Date', 'Order Qty', 'Already Dev.', 'Dev. Qty'],
    //    },
    //    ColumnValue: [
    //        { "data": "F_Part_No" },
    //        { "data": "F_Receive_Date" },
    //        { "data": "F_Unit_Amount" },
    //        { "data": "F_Receive_amount" },
    //        { "data": "F_Dev_Qty" }
    //    ],
    //    Modal: 'modalMaster',
    //    Form: 'frmMaster',
    //    PostData: [
    //        { name: 'F_Plant', value: _PLANT_ }
    //    ],
    //});


    //KBNCR120.prepare();

    //KBNCR120.initial(function (result) {
    //    console.log(result);

    //    xAjax.onCheck('#chkDeliveryDate', function () {
    //        if ($('#chkDeliveryDate').val() == 0) $('#fldDeliveryDate').prop('disabled', 'disabled');
    //        if ($('#chkDeliveryDate').val() == 1) $('#fldDeliveryDate').prop('disabled', false);
    //    });
    //    xSplash.hide();
    //    //KBNCR120.search();
    //});
});