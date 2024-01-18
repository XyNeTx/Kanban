$(document).ready(function () {

    const xKBNMS007 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['No.', 'Plant', 'F_Parent_Part', 'F_Part_Name', 'Effective Date', 'End Date'],
            "TH": ['No.', 'Plant', 'F_Parent_Part', 'F_Part_Name', 'Effective Date', 'End Date'],
        },
        ColumnValue: [
            { "data": "RunningNo" },
            { "data": "F_Plant" },
            { "data": "F_Parent_Part" },
            { "data": "F_Part_Name" },
            { "data": "F_Start_Date" },
            { "data": "F_End_Date" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: '#frmCondition #F_Plant' }
        ],
    });

    xKBNMS007.prepare();

    xKBNMS007.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS007.search();
    });

    onSave = function () {
        xKBNMS007.save(function () {
            xKBNMS007.search();
        });
    }

    onDelete = function () {
        xKBNMS007.delete(function () {
            xKBNMS007.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS007.deleteall(function () {
            xKBNMS007.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }

    xAjax.onChange('#frmCondition #F_Plant', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNMS007.search();
    });





})

