$(document).ready(function () {

    const xKBNMS024 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Supplier Code', 'Part No', 'Store Code', 'Start Date', 'End Date', 'Package Type', 'Package Code', 'Weight', 'Qty', 'Total'],
            "TH": ['Supplier Code', 'Part No', 'Store Code', 'Start Date', 'End Date', 'Package Type', 'Package Code', 'Weight', 'Qty', 'Total'],
            "JP": ['Supplier Code', 'Part No', 'Store Code', 'Start Date', 'End Date', 'Package Type', 'Package Code', 'Weight', 'Qty', 'Total'],
        },
        ColumnValue: [
            { "data": "F_SupplierName" },
            { "data": "F_Part_No" },
            { "data": "F_Store_Cd" },
            { "data": "F_Start_Date" },
            { "data": "F_End_Date" },
            { "data": "F_Package_Type" },
            { "data": "F_Package_Code" },
            { "data": "F_Part_Weight" },
            { "data": "F_Qty" },
            { "data": "F_Total" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNMS024.prepare();

    xKBNMS024.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS024.search();
    });

    onSave = function () {
        xKBNMS024.save(function () {
            xKBNMS024.search();
        });
    }

    onDelete = function () {
        xKBNMS024.delete(function () {
            xKBNMS024.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS024.deleteall(function () {
            xKBNMS024.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }

    xAjax.onChange('#frmCondition #F_Plant', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNMS024.search();
    });





})

