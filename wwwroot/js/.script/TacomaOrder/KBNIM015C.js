"use strict";
$(document).ready(function () {
    const xKBNIM015C = new MasterTemplate({
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
    xKBNIM015C.prepare();
    xKBNIM015C.initial(function (result) {
        xAjax.onCheck('#chkDeliveryDate', function () {
            if ($('#chkDeliveryDate').val() == 0)
                $('#fldDeliveryDate').prop('disabled', 'disabled');
            if ($('#chkDeliveryDate').val() == 1)
                $('#fldDeliveryDate').prop('disabled', false);
        });
        xKBNIM015C.search();
    });
    onSave = function () {
        xKBNIM015C.save(function () {
            xKBNIM015C.search();
        });
    };
    onDelete = function () {
        xKBNIM015C.delete(function () {
            xKBNIM015C.search();
        });
    };
    onDeleteAll = function () {
        xKBNIM015C.deleteall(function () {
            xKBNIM015C.search();
        });
    };
    onPrint = function () {
        xDataTableExport.setConfigPDF({
            title: 'OLD TYPE SERVICE CHECK LIST'
        });
    };
    onExecute = function () {
        console.log('onExecute');
    };
});
