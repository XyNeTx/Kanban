$(document).ready(function () {

    const KBNIM001O = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Order No.', 'Order Issued Date', 'Part Order (Old)', 'Delivery Date', 'Qty.'],
            "TH": ['Order No.', 'Order Issued Date', 'Part Order (Old)', 'Delivery Date', 'Qty.'],
            "JP": ['Order No.', 'Order Issued Date', 'Part Order (Old)', 'Delivery Date', 'Qty.'],
        },
        ColumnValue: [
            { "data": "F_OrderType" },
            { "data": "F_Effect_Date" },
            { "data": "F_OrderType" },
            { "data": "F_End_Date" },
            { "data": "F_Plant" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });
    

    xSplash.hide();

    KBNIM001O.prepare();

    KBNIM001O.initial(function (result) {
        KBNIM001O.search();
    });

    onSave = function () {
        KBNIM001O.save(function () {
            KBNIM001O.search();
        });
    }

    onDelete = function () {
        KBNIM001O.delete(function () {
            KBNIM001O.search();
        });
    }

    onDeleteAll = function () {
        KBNIM001O.deleteall(function () {
            KBNIM001O.search();
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