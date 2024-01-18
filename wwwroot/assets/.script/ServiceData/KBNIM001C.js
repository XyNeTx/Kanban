$(document).ready(function () {

    const KBNIM001C = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Order No.', 'PDS Issued Date', 'Delivery Date'],
            "TH": ['Order No.', 'PDS Issued Date', 'Delivery Date'],
            "JP": ['Order No.', 'PDS Issued Date', 'Delivery Date'],
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


    KBNIM001C.prepare();

    KBNIM001C.initial(function (result) {

        xAjax.onCheck('#chkDeliveryDate', function () {
            if ($('#chkDeliveryDate').val() == 0) $('#fldDeliveryDate').prop('disabled', 'disabled');
            if ($('#chkDeliveryDate').val() == 1) $('#fldDeliveryDate').prop('disabled', false);
        });

        KBNIM001C.search();
    });

    onSave = function () {
        KBNIM001C.save(function () {
            KBNIM001C.search();
        });
    }

    onDelete = function () {
        KBNIM001C.delete(function () {
            KBNIM001C.search();
        });
    }

    onDeleteAll = function () {
        KBNIM001C.deleteall(function () {
            KBNIM001C.search();
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