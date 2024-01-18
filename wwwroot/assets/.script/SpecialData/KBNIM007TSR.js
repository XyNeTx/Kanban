$(document).ready(function () {

    const xKBNIM007TSR = new ActionTemplate({
        Controller: _PAGE_,
        Form: 'frmCondition'
    });

    xKBNIM007TSR.prepare(function () {

        var tblPDS = xDataTable.Initial({
            name: 'tblPDS',
            running: 0,
            dom: '<"clear">',
            columnTitle: {
                "EN": ['Customer PO', 'Require Date'],
                "TH": ['Customer PO', 'Require Date'],
                "JP": ['Customer PO', 'Require Date'],
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
                "EN": ['Purchase Request No.', 'Customer PO', 'Delivery Date', 'Part No.', 'Kanban No.', 'Supplier', 'Qty'],
                "TH": ['Purchase Request No.', 'Customer PO', 'Delivery Date', 'Part No.', 'Kanban No.', 'Supplier', 'Qty'],
                "JP": ['Purchase Request No.', 'Customer PO', 'Delivery Date', 'Part No.', 'Kanban No.', 'Supplier', 'Qty'],
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


    xKBNIM007TSR.initial(function () {

        xAjax.onClick('#chkDeliveryDate', function () {
            if ($('#chkDeliveryDate').val() == 0) $('#fldDeliveryDate').prop('disabled', 'disabled');
            if ($('#chkDeliveryDate').val() == 1) $('#fldDeliveryDate').prop('disabled', false);
        });

        xSplash.hide();
    })

    onExecute = function () {
        xKBNIM007TSR.execute(function () {
            console.log('onExecute');
        })
    }

    onDeleteAll = function () {
        xKBNIM007TSR.delete(function () {
            console.log('onDelete');
        })
    }

});

