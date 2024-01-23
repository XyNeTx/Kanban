$(document).ready(function () {

    const xKBNMS023 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Package Type', 'Package Code', 'Package Weight', 'Width', 'Long', 'High', 'M3'],
            "TH": ['Package Type', 'Package Code', 'Package Weight', 'Width', 'Long', 'High', 'M3'],
            "JP": ['Package Type', 'Package Code', 'Package Weight', 'Width', 'Long', 'High', 'M3'],
        },
        ColumnValue: [
            { "data": "F_Package_Type" },
            { "data": "F_Package_Code" },
            { "data": "F_Package_Weight" },
            { "data": "F_Width" },
            { "data": "F_Long" },
            { "data": "F_High" },
            { "data": "F_M3" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNMS023.prepare();

    xKBNMS023.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS023.search();
    });

    onSave = function () {
        xKBNMS023.save(function () {
            xKBNMS023.search();
        });
    }

    onDelete = function () {
        xKBNMS023.delete(function () {
            xKBNMS023.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS023.deleteall(function () {
            xKBNMS023.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }

    xAjax.onChange('#frmCondition #F_Plant', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNMS023.search();
    });





})

