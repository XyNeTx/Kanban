$(document).ready(function () {

    const xKBNMS003 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Plant', 'Parent Part', 'Part Name', 'Effective Date', 'End Date'],
            "TH": ['Plant', 'Parent Part', 'Part Name', 'Effective Date', 'End Date'],
            "JP": ['Plant', 'Parent Part', 'Part Name', 'Effective Date', 'End Date'],
        },
        ColumnValue: [
            { "data": "F_Plant" },
            { "data": "F_Parent_Part_Name" },
            { "data": "F_Part_Name" },
            { "data": "F_Start_Date" },
            { "data": "F_End_Date" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNMS003.prepare();

    xKBNMS003.initial(function (result) {
        //xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        //xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS003.search();
    });

    onSave = function () {
        xKBNMS003.save(function () {
            xKBNMS003.search();
        });
    }

    onDelete = function () {
        xKBNMS003.delete(function () {
            xKBNMS003.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS003.deleteall(function () {
            xKBNMS003.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }

    //xAjax.onChange('#frmCondition #F_Plant', function () {
    //    $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

    //    xKBNMS003.search();
    //});





})

