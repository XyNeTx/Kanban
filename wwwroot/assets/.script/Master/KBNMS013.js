$(document).ready(function () {

    const xKBNMS013 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Supplier Code', 'Cycle', 'Kanban No', 'Store Code', 'Part No', 'Start Date', 'End Date', 'Type Order', 'Dock Code', 'PDS Group'],
            "TH": ['Supplier Code', 'Cycle', 'Kanban No', 'Store Code', 'Part No', 'Start Date', 'End Date', 'Type Order', 'Dock Code', 'PDS Group'],
            "JP": ['Supplier Code', 'Cycle', 'Kanban No', 'Store Code', 'Part No', 'Start Date', 'End Date', 'Type Order', 'Dock Code', 'PDS Group'],
        },
        ColumnValue: [
            { "data": "F_Supplier_Code" },
            { "data": "F_Cycle" },
            { "data": "F_Kanban_No" },
            { "data": "F_Store_Code" },
            { "data": "F_Part_No" },
            { "data": "F_Start_Date" },
            { "data": "F_End_Date" },
            { "data": "F_Type_Order" },
            { "data": "F_Dock_Code" },
            { "data": "F_PDS_Group" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNMS013.prepare();

    xKBNMS013.initial(function (result) {
        //xDropDownList.bind('frmCondition #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('frmCondition #F_Supplier', result.data.TB_Supplier, 'F_Supplier_Code', 'F_Supplier_Code');

        //xDropDownList.bind('frmMaster #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS013.search();
    });

    onSave = function () {
        xKBNMS013.save(function () {
            xKBNMS013.search();
        });
    }

    onDelete = function () {
        xKBNMS013.delete(function () {
            xKBNMS013.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS013.deleteall(function () {
            xKBNMS013.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }

    //xAjax.onChange('frmCondition #F_Supplier', function () {
    //    $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

    //    xKBNMS013.search();
    //});





})

