$(document).ready(function () {

    console.log('xERP01M011');

    const xERP01M011 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Code', 'Name', 'Name (Thai)', 'Status'],
            "TH": ['Code', 'Name', 'Name (Thai)', 'Status'],
            "JP": ['Code', 'Name', 'Name (Thai)', 'Status'],
        },
        ColumnValue: [
            { "data": "Code" },
            { "data": "Name" },
            { "data": "NameTH" },
            { "data": "Status" }
        ],
        orderNo: true,
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: '#frmCondition #F_Plant' }
        ],
    });


    xERP01M011.prepare();

    xERP01M011.initial(function (result) {
        //xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        //xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

        xERP01M011.search();
    });

    onSave = function () {
        xERP01M011.save(function () {
            xERP01M011.search();
        });
    }

    onDelete = function () {
        xERP01M011.delete(function () {
            xERP01M011.search();
        });
    }

    onDeleteAll = function () {
        xERP01M011.deleteall(function () {
            xERP01M011.search();
        });
    }

    onPrint = function () {
        xDataTableExport.setConfigPDF({
            title: 'OLD TYPE SERVICE CHECK LIST'
        });
    }

    onExecute = function () {
        console.log('onExecute');
    }

    xAjax.onChange('#frmCondition #F_Plant', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xERP01M011.search();
    });


});