"use strict";
$(document).ready(function () {
    const xKBNIM006C = new MasterTemplate({
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
    xKBNIM006C.prepare();
    xKBNIM006C.initial(function (result) {
        xAjax.onCheck('#chkDeliveryDate', function () {
            if ($('#chkDeliveryDate').val() == 0)
                $('#fldDeliveryDate').prop('disabled', 'disabled');
            if ($('#chkDeliveryDate').val() == 1)
                $('#fldDeliveryDate').prop('disabled', false);
        });
        xKBNIM006C.search();
    });
    onSave = function () {
        xKBNIM006C.save(function () {
            xKBNIM006C.search();
        });
    };
    onDelete = function () {
        xKBNIM006C.delete(function () {
            xKBNIM006C.search();
        });
    };
    onDeleteAll = function () {
        xKBNIM006C.deleteall(function () {
            xKBNIM006C.search();
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
