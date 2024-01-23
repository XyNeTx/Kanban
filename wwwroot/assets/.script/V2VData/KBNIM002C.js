$(document).ready(function () {

    const xKBNIM002C = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Order No.', 'Order Issued Date', 'Delivery Date (ADV)', 'Country'],
            "TH": ['Order No.', 'Order Issued Date', 'Delivery Date (ADV)', 'Country'],
            "JP": ['Order No.', 'Order Issued Date', 'Delivery Date (ADV)', 'Country'],
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


    xKBNIM002C.prepare();

    xKBNIM002C.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNIM002C.search();
    });

    onSave = function () {
        xKBNIM002C.save(function () {
            xKBNIM002C.search();
        });
    }

    onDelete = function () {
        xKBNIM002C.delete(function () {
            xKBNIM002C.search();
        });
    }

    onDeleteAll = function () {
        xKBNIM002C.deleteall(function () {
            xKBNIM002C.search();
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

        xKBNIM002C.search();
    });


});