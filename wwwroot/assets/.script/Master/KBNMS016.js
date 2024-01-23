$(document).ready(function () {

    const xKBNMS016 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Group Name', 'Supplier Code', 'Kanban No', 'Store Code', 'Part No', 'Qty', 'Start Date', 'End Date'],
            "TH": ['Group Name', 'Supplier Code', 'Kanban No', 'Store Code', 'Part No', 'Qty', 'Start Date', 'End Date'],
            "JP": ['Group Name', 'Supplier Code', 'Kanban No', 'Store Code', 'Part No', 'Qty', 'Start Date', 'End Date'],
        },
        ColumnValue: [
            { "data": "F_Group" },
            { "data": "F_Supplier_Code" },
            { "data": "F_Kanban_No" },
            { "data": "F_Store_Code" },
            { "data": "F_Part_No" },
            { "data": "F_Qty" },
            { "data": "F_Start_Date" },
            { "data": "F_End_Date" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNMS016.prepare();

    xKBNMS016.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmCondition #F_Supplier', result.data.TB_Supplier, 'F_Supplier_Code', 'F_Supplier_Code');

        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_Group', result.data.TB_Group, 'F_Group', 'F_Group');

        xKBNMS016.search();
    });



    onSave = function () {
        xKBNMS016.save(function () {
            xKBNMS016.search();
        });
    }

    onDelete = function () {
        xKBNMS016.delete(function () {
            xKBNMS016.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS016.deleteall(function () {
            xKBNMS016.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }


    xAjax.onChange('#frmCondition #F_Supplier', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNMS016.search();
    });


    xAjax.on('#frmMaster #txtColor', function () {
        $('#frmMaster #F_Color').val($('#frmMaster #txtColor').val());
    });





})

