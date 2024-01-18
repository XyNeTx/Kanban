$(document).ready(function () {

    const KBNIM003M = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Order No.', 'Order Issued', 'Part No.', 'Store Code', 'Delivery Date', 'Qty', 'Remark'],
            "TH": ['Order No.', 'Order Issued', 'Part No.', 'Store Code', 'Delivery Date', 'Qty', 'Remark'],
            "JP": ['Order No.', 'Order Issued', 'Part No.', 'Store Code', 'Delivery Date', 'Qty', 'Remark'],
        },
        ColumnValue: [
            { "data": "F_PDS_No" },
            { "data": "F_PDS_Issued_Date" },
            { "data": "F_Part_No" },
            { "data": "F_Store_Order" },
            { "data": "F_Delivery_Date" },
            { "data": "F_Qty" },
            { "data": "F_Remark" }
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