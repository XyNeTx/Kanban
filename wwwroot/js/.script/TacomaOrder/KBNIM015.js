"use strict";
$(document).ready(function () {
    const KBNIM015 = new ActionTemplate({
        Controller: _PAGE_,
        Form: 'frmCondition'
    });
    KBNIM015.prepare(function () {
        var tblPDS = xDataTable.Initial({
            name: 'tblPDS',
            checking: 0,
            running: 0,
            dom: '<"clear">',
            columnTitle: {
                "EN": ['Order Date', 'Country', 'Cust Type', 'Type Part'],
                "TH": ['Order Date', 'Country', 'Cust Type', 'Type Part'],
                "JP": ['Order Date', 'Country', 'Cust Type', 'Type Part'],
            },
            column: [
                { "data": "F_Plant" },
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
            checking: 0,
            dom: '<"clear">',
            columnTitle: {
                "EN": ['Item', 'Type Part', 'Customer Order Type', 'HMMT Order Type', 'Part No.', 'Part Name', 'Kanban', 'Supplier Code', 'Supplier Name', 'Store Code', 'Cycle Time', 'Qty/Pack', 'TACOMA Order PCS.', 'KANBAN Order PCS.', 'Order KB', 'Order Date', 'Pack Date', 'Amount Adv.', 'Adv Pack Date', 'PDS No.', 'Country', 'Remark'],
                "TH": ['Item', 'Type Part', 'Customer Order Type', 'HMMT Order Type', 'Part No.', 'Part Name', 'Kanban', 'Supplier Code', 'Supplier Name', 'Store Code', 'Cycle Time', 'Qty/Pack', 'TACOMA Order PCS.', 'KANBAN Order PCS.', 'Order KB', 'Order Date', 'Pack Date', 'Amount Adv.', 'Adv Pack Date', 'PDS No.', 'Country', 'Remark'],
                "JP": ['Item', 'Type Part', 'Customer Order Type', 'HMMT Order Type', 'Part No.', 'Part Name', 'Kanban', 'Supplier Code', 'Supplier Name', 'Store Code', 'Cycle Time', 'Qty/Pack', 'TACOMA Order PCS.', 'KANBAN Order PCS.', 'Order KB', 'Order Date', 'Pack Date', 'Amount Adv.', 'Adv Pack Date', 'PDS No.', 'Country', 'Remark'],
            },
            column: [
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_Plant" },
                { "data": "F_Plant" },
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
    KBNIM015.initial(function () {
        xAjax.onClick('#chkDeliveryDate', function () {
            if ($('#chkDeliveryDate').val() == 0)
                $('#fldDeliveryDate').prop('disabled', 'disabled');
            if ($('#chkDeliveryDate').val() == 1)
                $('#fldDeliveryDate').prop('disabled', false);
        });
        xSplash.hide();
    });
    onExecute = function () {
        KBNIM015.execute(function () {
            console.log('onExecute');
        });
    };
    onDeleteAll = function () {
        KBNIM015.delete(function () {
            console.log('onDelete');
        });
    };
});
