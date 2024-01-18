$(document).ready(function () {

    const KBNIM014C = new MasterTemplate({
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


    KBNIM014C.prepare();

    KBNIM014C.initial(function (result) {

        xAjax.onCheck('#chkDeliveryDate', function () {
            if ($('#chkDeliveryDate').val() == 0) $('#fldDeliveryDate').prop('disabled', 'disabled');
            if ($('#chkDeliveryDate').val() == 1) $('#fldDeliveryDate').prop('disabled', false);
        });

        KBNIM014C.search();
    });

    onSave = function () {
        KBNIM014C.save(function () {
            KBNIM014C.search();
        });
    }

    onDelete = function () {
        KBNIM014C.delete(function () {
            KBNIM014C.search();
        });
    }

    onDeleteAll = function () {
        KBNIM014C.deleteall(function () {
            KBNIM014C.search();
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