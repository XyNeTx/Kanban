$(document).ready(function () {

    const xKBNMS027 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Supplier Logistic', 'Supplier Order', 'Supplier Code', 'Supplier Name'],
            "TH": ['Supplier Logistic', 'Supplier Order', 'Supplier Code', 'Supplier Name'],
            "JP": ['Supplier Logistic', 'Supplier Order', 'Supplier Code', 'Supplier Name'],
        },
        ColumnValue: [
            { "data": "F_short_Logistic" },
            { "data": "F_short_name" },
            { "data": "F_Supplier_Code" },
            { "data": "F_name" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNMS027.prepare();

    xKBNMS027.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS027.search();
    });

    onSave = function () {
        xKBNMS027.save(function () {
            xKBNMS027.search();
        });
    }

    onDelete = function () {
        xKBNMS027.delete(function () {
            xKBNMS027.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS027.deleteall(function () {
            xKBNMS027.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }

    xAjax.onChange('#frmCondition #F_Plant', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNMS027.search();
    });





})

