$(document).ready(function () {

    const xKBNMS029 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Plant', 'Dock Code', 'Start Date', 'End Date'],
            "TH": ['Plant', 'Dock Code', 'Start Date', 'End Date'],
            "JP": ['Plant', 'Dock Code', 'Start Date', 'End Date'],
        },
        ColumnValue: [
            { "data": "F_Plant_Name" },
            { "data": "F_Dock_Code" },
            { "data": "F_Start_Date" },
            { "data": "F_End_Date" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNMS029.prepare();

    xKBNMS029.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS029.search();
    });

    onSave = function () {
        xKBNMS029.save(function () {
            xKBNMS029.search();
        });
    }

    onDelete = function () {
        xKBNMS029.delete(function () {
            xKBNMS029.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS029.deleteall(function () {
            xKBNMS029.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }

    xAjax.onChange('#frmCondition #F_Plant', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNMS029.search();
    });





})

