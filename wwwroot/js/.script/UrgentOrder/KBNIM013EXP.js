"use strict";
$(document).ready(function () {
    const xKBNIM013EXP = new ActionTemplate({
        //Controller: _PAGE_,
        Form: 'frmCondition'
    });
    xKBNIM013EXP.prepare(function () {
        var tblPDS = xDataTable.Initial({
            name: 'tblPDS',
            checking: 0,
            dom: '<"clear">',
            columnTitle: {
                "EN": ['Separate Declaration No.', 'Refer. Doc.', 'Delivery Date'],
                "TH": ['Separate Declaration No.', 'Refer. Doc.', 'Delivery Date'],
                "JP": ['Separate Declaration No.', 'Refer. Doc.', 'Delivery Date'],
            },
            column: [
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_OrderType" }
            ],
            addnew: false,
            rowclick: (row) => {
            }
        });
        var tblSeparate = xDataTable.Initial({
            name: 'tblSeparate',
            running: 0,
            dom: '<"clear">',
            columnTitle: {
                "EN": ['Separate Declaration No.', 'Refer. Doc.', 'Delivery Date', 'Part No.', 'Kanban No.', 'Supplier', 'Qty'],
                "TH": ['Separate Declaration No.', 'Refer. Doc.', 'Delivery Date', 'Part No.', 'Kanban No.', 'Supplier', 'Qty'],
                "JP": ['Separate Declaration No.', 'Refer. Doc.', 'Delivery Date', 'Part No.', 'Kanban No.', 'Supplier', 'Qty'],
            },
            column: [
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_OrderType" }
            ],
            addnew: false,
            rowclick: (row) => {
            }
        });
    });
    xKBNIM013EXP.initial(function () {
        xAjax.onClick('#chkDeliveryDate', function () {
            if ($('#chkDeliveryDate').val() == 0)
                $('#fldDeliveryDate').prop('disabled', 'disabled');
            if ($('#chkDeliveryDate').val() == 1)
                $('#fldDeliveryDate').prop('disabled', false);
        });
        xSplash.hide();
    });
    onExecute = function () {
        xKBNIM013EXP.execute(function () {
            console.log('onExecute');
        });
    };
    //onDelete = function () {
    //    xKBNIM013EXP.execute(function () {
    //        console.log('onDelete');
    //    })
    //}
});
