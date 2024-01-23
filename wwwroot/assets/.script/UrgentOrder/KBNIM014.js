$(document).ready(function () {

    const xKBNIM001C = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Flag', 'Order No.', 'PDS Issued Date', 'Delivery Date'],
            "TH": ['Flag', 'Order No.', 'PDS Issued Date', 'Delivery Date'],
            "JP": ['Flag', 'Order No.', 'PDS Issued Date', 'Delivery Date'],
        },
        ColumnValue: [
            { "data": "F_Plant" },
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


    xKBNIM001C.prepare();

    xKBNIM001C.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNIM001C.search();
    });

    onSave = function () {
        xKBNIM001C.save(function () {
            xKBNIM001C.search();
        });
    }

    onDelete = function () {
        xKBNIM001C.delete(function () {
            xKBNIM001C.search();
        });
    }

    onDeleteAll = function () {
        xKBNIM001C.deleteall(function () {
            xKBNIM001C.search();
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

    xAjax.onChange('#frmCondition #F_Plant', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNIM001C.search();
    });


});