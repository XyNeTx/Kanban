$(document).ready(function () {

    const KBNIM003M = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Order No.', 'Order Issued', 'Plant No.', 'Store Code', 'Delivery Date', 'Qty.', 'Remark'],
            "TH": ['Order No.', 'Order Issued', 'Plant No.', 'Store Code', 'Delivery Date', 'Qty.', 'Remark'],
            "JP": ['Order No.', 'Order Issued', 'Plant No.', 'Store Code', 'Delivery Date', 'Qty.', 'Remark'],
        },
        ColumnValue: [
            { "data": "F_OrderType" },
            { "data": "F_Effect_Date" },
            { "data": "F_Effect_Date" },
            { "data": "F_Effect_Date" },
            { "data": "F_Effect_Date" },
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

    KBNIM003M.prepare();

    KBNIM003M.initial(function (result) {
        KBNIM003M.search();
    });

    onSave = function () {
        KBNIM003M.save(function () {
            KBNIM003M.search();
        });
    }

    onDelete = function () {
        KBNIM003M.delete(function () {
            KBNIM003M.search();
        });
    }

    onDeleteAll = function () {
        KBNIM003M.deleteall(function () {
            KBNIM003M.search();
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