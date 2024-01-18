$(document).ready(function () {

    const xKBNMS025 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Logistic', 'Start Date', 'End Date', 'Truck Type', 'Weight', 'Width', 'High', 'Long'],
            "TH": ['Logistic', 'Start Date', 'End Date', 'Truck Type', 'Weight', 'Width', 'High', 'Long'],
            "JP": ['Logistic', 'Start Date', 'End Date', 'Truck Type', 'Weight', 'Width', 'High', 'Long'],
        },
        ColumnValue: [
            { "data": "F_Logistic" },
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

    xKBNMS025.prepare();

    xKBNMS025.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS025.search();
    });

    onSave = function () {
        xKBNMS025.save(function () {
            xKBNMS025.search();
        });
    }

    onDelete = function () {
        xKBNMS025.delete(function () {
            xKBNMS025.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS025.deleteall(function () {
            xKBNMS025.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }

    xAjax.onChange('#frmCondition #F_Plant', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNMS025.search();
    });





})

