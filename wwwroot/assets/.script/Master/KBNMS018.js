$(document).ready(function () {

    const xKBNMS018 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Cycle B'],
            "TH": ['Cycle B'],
            "JP": ['Cycle B'],
        },
        ColumnValue: [
            { "data": "F_CycleB" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNMS018.prepare();

    xKBNMS018.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');

        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_CycleB', result.data.TB_Heijunka, 'F_CycleB', 'F_CycleB');

        xKBNMS018.search();
    });



    onSave = function () {
        xKBNMS018.save(function () {
            xKBNMS018.search();
        });
    }

    onDelete = function () {
        xKBNMS018.delete(function () {
            xKBNMS018.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS018.deleteall(function () {
            xKBNMS018.search();
        });
    }

    onPrint = function () {
    }

    onExecute = function () { }





})

