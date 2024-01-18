$(document).ready(function () {

    const xKBNMS020 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Part No', 'Store Cd', 'Part Name', 'Supplier Code', 'Supplier Name', 'Kanban No', 'Qty.', 'Address', 'STD Stock(Day)', 'STD Stock(KB', 'Dock', 'Package Type', 'Max Area'],
            "TH": ['Part No', 'Store Cd', 'Part Name', 'Supplier Code', 'Supplier Name', 'Kanban No', 'Qty.', 'Address', 'STD Stock(Day)', 'STD Stock(KB', 'Dock', 'Package Type', 'Max Area'],
            "JP": ['Part No', 'Store Cd', 'Part Name', 'Supplier Code', 'Supplier Name', 'Kanban No', 'Qty.', 'Address', 'STD Stock(Day)', 'STD Stock(KB', 'Dock', 'Package Type', 'Max Area'],
        },
        ColumnValue: [
            { "data": "F_Part" },
            { "data": "F_Store_Code" },
            { "data": "F_Part_Name" },
            { "data": "F_Supplier" },
            { "data": "F_Supplier_Name" },
            { "data": "F_Kanban_No" },
            { "data": "F_Box_Qty" },
            { "data": "F_Address" },
            { "data": "F_STD_Stock" },
            { "data": "F_STD_Stock_KB" },
            { "data": "Dock" },
            { "data": "F_Package_Type" },
            { "data": "F_Max_Area" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNMS020.prepare();

    xKBNMS020.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS020.search();
    });

    onSave = function () {
        xKBNMS020.save(function () {
            xKBNMS020.search();
        });
    }

    onDelete = function () {
        xKBNMS020.delete(function () {
            xKBNMS020.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS020.deleteall(function () {
            xKBNMS020.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }

    xAjax.onChange('#frmCondition #F_Plant', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNMS020.search();
    });





})

