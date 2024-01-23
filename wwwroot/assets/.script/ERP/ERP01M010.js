$(document).ready(function () {

    const xERP01M010 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Code', 'Name', 'Surname', 'Name (Thai)', 'Surname (Thai)', 'Group', 'Status'],
            "TH": ['Code', 'Name', 'Surname', 'Name (Thai)', 'Surname (Thai)', 'Group', 'Status'],
            "JP": ['Code', 'Name', 'Surname', 'Name (Thai)', 'Surname (Thai)', 'Group', 'Status'],
        },
        ColumnValue: [
            { "data": "Code" },
            { "data": "Name" },
            { "data": "Surname" },
            { "data": "NameTH" },
            { "data": "SurnameTH" },
            { "data": "GroupName" },
            { "data": "Status" }
        ],
        orderNo: true,
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: '#frmCondition #F_Plant' }
        ],
    });


    xERP01M010.prepare();

    xERP01M010.initial(function (result) {
        xDropDownList.bind('#frmMaster #GroupID', result.data.erpGroup, '_ID', 'Name');

        xERP01M010.search();
    });

    onSave = function () {
        xERP01M010.save(function () {
            xERP01M010.search();
        });
    }

    onDelete = function () {
        xERP01M010.delete(function () {
            xERP01M010.search();
        });
    }

    onDeleteAll = function () {
        xERP01M010.deleteall(function () {
            xERP01M010.search();
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

        xERP01M010.search();
    });


});