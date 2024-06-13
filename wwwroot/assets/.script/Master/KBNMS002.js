$(document).ready(function () {

    const xKBNMS002 = new MasterTemplate({
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

    xKBNMS002.prepare();

    xKBNMS002.initial(function (result) {
        //xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        //xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS002.search();
    });

    onSave = function () {
        xKBNMS002.save(function () {
            xKBNMS002.search();
        });
    }

    onDelete = function () {
        xKBNMS002.delete(function () {
            xKBNMS002.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS002.deleteall(function () {
            xKBNMS002.search();
        });
    }

    onPrint = function () {
        xDataTableExport.setConfigPDF({
            title: 'OLD PART CHECK LIST'
        });
    }

    onExecute = function () {
        console.log('onExecute');
    }

    //xAjax.onChange('#frmCondition #F_Plant', function () {
    //    $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

    //    xKBNMS002.search();
    //});





})

