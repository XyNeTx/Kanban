$(document).ready(function () {

    const xKBNMS014 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Print', 'Supplier Code', 'Supplier Plant', 'Short Name'],
            "TH": ['Print', 'Supplier Code', 'Supplier Plant', 'Short Name'],
            "JP": ['Print', 'Supplier Code', 'Supplier Plant', 'Short Name'],
        },
        ColumnValue: [
            { "data": "F_Flag_Print" },
            { "data": "F_Supplier_Code" },
            { "data": "F_Supplier_Plant" },
            { "data": "F_Short_Name" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNMS014.prepare();

    xKBNMS014.initial(function (result) {
        //xDropDownList.bind('frmCondition #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');
        //xDropDownList.bind('frmCondition #F_Supplier', result.data.TB_Supplier, 'F_Supplier_Code', 'F_Supplier_Code');

        //xDropDownList.bind('frmMaster #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS014.search();
    });

    onSave = function () {
        xKBNMS014.save(function () {
            xKBNMS014.search();
        });
    }

    onDelete = function () {
        xKBNMS014.delete(function () {
            xKBNMS014.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS014.deleteall(function () {
            xKBNMS014.search();
        });
    }

    onPrint = function () {
        //_DATATABLE_EXPORT.PDF.title = 'ABC';
    }

    onExecute = function () { }

    xAjax.onChange('frmCondition #F_Supplier', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNMS014.search();
    });





})

