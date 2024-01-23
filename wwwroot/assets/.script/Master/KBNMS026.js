$(document).ready(function () {

    const xKBNMS026 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Route', 'Start Date', 'End Date', 'Truck Type', 'Weight', 'Width', 'High', 'Long'],
            "TH": ['Route', 'Start Date', 'End Date', 'Truck Type', 'Weight', 'Width', 'High', 'Long'],
            "JP": ['Route', 'Start Date', 'End Date', 'Truck Type', 'Weight', 'Width', 'High', 'Long'],
        },
        ColumnValue: [
            { "data": "F_Route" },
            { "data": "F_Start_Date" },
            { "data": "F_End_Date" },
            { "data": "F_Truck_Type" },
            { "data": "F_Weight" },
            { "data": "F_Width" },
            { "data": "F_High" },
            { "data": "F_Long" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNMS026.prepare();

    xKBNMS026.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS026.search();
    });

    onSave = function () {
        xKBNMS026.save(function () {
            xKBNMS026.search();
        });
    }

    onDelete = function () {
        xKBNMS026.delete(function () {
            xKBNMS026.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS026.deleteall(function () {
            xKBNMS026.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }

    xAjax.onChange('#frmCondition #F_Plant', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNMS026.search();
    });





})

