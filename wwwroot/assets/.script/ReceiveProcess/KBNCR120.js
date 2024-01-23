$(document).ready(function () {

    const KBNCR110 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['PDS No.', 'Delivery Trip', 'Delivery Date'],
            "TH": ['PDS No.', 'Delivery Trip', 'Delivery Date'],
            "JP": ['PDS No.', 'Delivery Trip', 'Delivery Date'],
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


    KBNCR110.prepare();

    KBNCR110.initial(function (result) {

        xAjax.onCheck('#chkDeliveryDate', function () {
            if ($('#chkDeliveryDate').val() == 0) $('#fldDeliveryDate').prop('disabled', 'disabled');
            if ($('#chkDeliveryDate').val() == 1) $('#fldDeliveryDate').prop('disabled', false);
        });

        KBNCR110.search();
    });

    onSave = function () {
        KBNCR110.save(function () {
            KBNCR110.search();
        });
    }

    onDelete = function () {
        KBNCR110.delete(function () {
            KBNCR110.search();
        });
    }

    onDeleteAll = function () {
        KBNCR110.deleteall(function () {
            KBNCR110.search();
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