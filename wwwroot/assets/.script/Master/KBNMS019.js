$(document).ready(function () {

    const xKBNMS019 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Supplier Code', 'Kanban No', 'Part No', 'Part Name', 'Store Code', 'Qty', 'Max Trip'],
            "TH": ['Supplier Code', 'Kanban No', 'Part No', 'Part Name', 'Store Code', 'Qty', 'Max Trip'],
            "JP": ['Supplier Code', 'Kanban No', 'Part No', 'Part Name', 'Store Code', 'Qty', 'Max Trip'],
        },
        ColumnValue: [
            { "data": "F_Supplier" },
            { "data": "F_Kanban_No" },
            { "data": "F_Part" },
            { "data": "F_Part_Name" },
            { "data": "F_Store_CD" },
            { "data": "F_Qty" },
            { "data": "F_Max_Trip" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNMS019.prepare();

    xKBNMS019.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS019.search();
    });

    onSave = function () {
        xKBNMS019.save(function () {
            xKBNMS019.search();
        });
    }

    onDelete = function () {
        xKBNMS019.delete(function () {
            xKBNMS019.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS019.deleteall(function () {
            xKBNMS019.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }

    xAjax.onChange('#frmCondition #F_Plant', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNMS019.search();
    });





})

