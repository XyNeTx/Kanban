$(document).ready(function () {

    const KBNMS013 = new MasterTemplate({
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

    KBNMS013.prepare();

    KBNMS013.initial(function (result) {
        //xDropDownList.bind('frmCondition #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('frmCondition #F_Supplier', result.data.TB_Supplier, 'F_Supplier_Code', 'F_Supplier_Code');

        //xDropDownList.bind('frmMaster #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');

        KBNMS013.search();
    });

    onSave = function () {
        KBNMS013.save(function () {
            KBNMS013.search();
        });
    }

    onDelete = function () {
        KBNMS013.delete(function () {
            KBNMS013.search();
        });
    }

    onDeleteAll = function () {
        KBNMS013.deleteall(function () {
            KBNMS013.search();
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

    //xAjax.onChange('frmCondition #F_Supplier', function () {
    //    $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

    //    KBNMS013.search();
    //});





})

