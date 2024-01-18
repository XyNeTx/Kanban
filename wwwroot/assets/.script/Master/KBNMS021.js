$(document).ready(function () {

    const xKBNMS021 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Line', 'Part Code', 'Part No', 'Part Name', 'Bridge', 'Detail'],
            "TH": ['Line', 'Part Code', 'Part No', 'Part Name', 'Bridge', 'Detail'],
            "JP": ['Line', 'Part Code', 'Part No', 'Part Name', 'Bridge', 'Detail'],
        },
        ColumnValue: [
            { "data": "F_Line" },
            { "data": "F_Code" },
            { "data": "F_Part" },
            { "data": "F_Part_Name" },
            { "data": "F_Bridge" },
            { "data": "F_Detail" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNMS021.prepare();

    xKBNMS021.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS021.search();
    });

    onSave = function () {
        xKBNMS021.save(function () {
            xKBNMS021.search();
        });
    }

    onDelete = function () {
        xKBNMS021.delete(function () {
            xKBNMS021.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS021.deleteall(function () {
            xKBNMS021.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }

    xAjax.onChange('#frmCondition #F_Plant', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNMS021.search();
    });





})

