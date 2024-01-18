$(document).ready(function () {

    const xKBNIM007C = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Order No.', 'Order Issued Date', 'Delivery Date'],
            "TH": ['Order No.', 'Order Issued Date', 'Delivery Date'],
            "JP": ['Order No.', 'Order Issued Date', 'Delivery Date'],
        },
        ColumnValue: [
            { "data": "F_OrderType" },
            { "data": "F_Effect_Date" },
            { "data": "F_End_Date" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });


    xKBNIM007C.prepare();

    xKBNIM007C.initial(function (result) {

        xAjax.onCheck('#chkDeliveryDate', function () {
            if ($('#chkDeliveryDate').val() == 0) $('#fldDeliveryDate').prop('disabled', 'disabled');
            if ($('#chkDeliveryDate').val() == 1) $('#fldDeliveryDate').prop('disabled', false);
        });

        xKBNIM007C.search();
    });

    onSave = function () {
        xKBNIM007C.save(function () {
            xKBNIM007C.search();
        });
    }

    onDelete = function () {
        xKBNIM007C.delete(function () {
            xKBNIM007C.search();
        });
    }

    onDeleteAll = function () {
        xKBNIM007C.deleteall(function () {
            xKBNIM007C.search();
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
});