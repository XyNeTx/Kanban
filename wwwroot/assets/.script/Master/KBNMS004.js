$(document).ready(function () {

    const xKBNMS004 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Supplier Code', 'Cycle', 'Kanban No', 'Store Code', 'Part No', 'Start Date', 'End Date', 'Type Special'],
            "TH": ['Supplier Code', 'Cycle', 'Kanban No', 'Store Code', 'Part No', 'Start Date', 'End Date', 'Type Special'],
            "JP": ['Supplier Code', 'Cycle', 'Kanban No', 'Store Code', 'Part No', 'Start Date', 'End Date', 'Type Special'],
        },
        ColumnValue: [
            { "data": "F_Supplier_Code" },
            { "data": "F_Cycle" },
            { "data": "F_Kanban_No" },
            { "data": "F_Store_Code" },
            { "data": "F_Part_No" },
            { "data": "F_Start_Date" },
            { "data": "F_End_Date" },
            { "data": "F_Type_Special" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNMS004.prepare();

    xKBNMS004.initial(function (result) {
        //xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmCondition #F_Supplier', result.data.TB_Supplier, 'F_Supplier_Code', 'F_Supplier_Code');

        //xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS004.search();
    });

    onSave = function () {
        xKBNMS004.save(function () {
            xKBNMS004.search();
        });
    }

    onDelete = function () {
        xKBNMS004.delete(function () {
            xKBNMS004.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS004.deleteall(function () {
            xKBNMS004.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }

    //xAjax.onChange('#frmCondition #F_Supplier', function () {
    //    $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

    //    xKBNMS004.search();
    //});






})

