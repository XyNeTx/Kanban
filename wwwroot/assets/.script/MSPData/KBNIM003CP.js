$(document).ready(function () {

    const xKBNMS001 = new MasterTemplate({
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


    xKBNMS001.prepare();

    xKBNMS001.initial(function (result) {

        xKBNMS001.search();
    });

    onSave = function () {
        xKBNMS001.save(function () {
            xKBNMS001.search();
        });
    }

    onDelete = function () {
        xKBNMS001.delete(function () {
            xKBNMS001.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS001.deleteall(function () {
            xKBNMS001.search();
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