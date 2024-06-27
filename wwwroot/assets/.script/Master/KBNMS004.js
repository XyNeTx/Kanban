$(document).ready(function () {

    const KBNMS004 = new MasterTemplate({
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

    KBNMS004.prepare();

    KBNMS004.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Supplier', result.data.TB_Supplier, 'F_Supplier_Code', 'F_Supplier_Code');

        KBNMS004.search();
    });

    onSave = function () {
        KBNMS004.save(function () {
            KBNMS004.search();
        });
    }

    onDelete = function () {
        KBNMS004.delete(function () {
            KBNMS004.search();
        });
    }

    onDeleteAll = function () {
        KBNMS004.deleteall(function () {
            KBNMS004.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }


    lovClick = function (ElementID) {
        var _SQL = '';

        if (ElementID == 'F_Kanban_No' && $('#F_Supplier_Code').val() != '') _SQL = "AND F_supplier_cd+'-'+F_plant = '" + $('#F_Supplier_Code').val() + "' ";
        //if (ElementID == 'F_Store_Code' && $('#F_Supplier_Code').val() != '') _SQL = "AND F_supplier_cd+'-'+F_plant = '" + $('#F_Supplier_Code').val() + "' ";
        if (ElementID == 'F_Part_No') {
            if ($('#F_Supplier_Code').val() != '') _SQL = "AND F_supplier_cd+'-'+F_plant = '" + $('#F_Supplier_Code').val() + "' ";
            if ($('#F_Kanban_No').val() != '') _SQL = "AND F_Sebango = '" + $('#F_Kanban_No').val() + "' ";
            if ($('#F_Store_Code').val() != '') _SQL = "AND F_Store_cd = '" + $('#F_Store_Code').val() + "' ";
        }

        ajexHeader.LOV = _SQL;
    }





})

