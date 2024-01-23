$(document).ready(function () {

    const xERP01M020 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Code', 'Name', 'Title', 'Icon', 'Seq', 'Status'],
            "TH": ['Code', 'Name', 'Title', 'Icon', 'Seq', 'Status'],
            "JP": ['Code', 'Name', 'Title', 'Icon', 'Seq', 'Status'],
        },
        ColumnValue: [
            { "data": "Code" },
            { "data": "Name" },
            { "data": "Title" },
            { "data": "Icon" },
            { "data": "Seq" },
            { "data": "Status" }
        ],
        orderNo: true,
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: '#frmCondition #F_Plant' }
        ],
    });


    xERP01M020.prepare();

    xERP01M020.initial(function (result) {
        xDropDownList.bind('#frmMaster #Parent_ID', result.data.erpMenu, '_ID', 'Name');
        xDropDownList.bind('#frmMaster #Group_ID', result.data.erpGroup, '_ID', 'Name');


        xAjax.onCheck('#Toolbar', function () {
            console.log('Toolbar');
            if ($('#Toolbar').val() == 0) $('#fldToobar').prop('readonly', 'readonly');
            if ($('#Toolbar').val() == 1) $('#fldToobar').prop('readonly', false);
        });

        xERP01M020.search();
    });

    onEdit = function () {
        $('#IconDisplay').attr('class', $('#Icon').val());
    }

    onSave = function () {
        xERP01M020.save(function () {
            xERP01M020.search();
        });
    }

    onDelete = function () {
        xERP01M020.delete(function () {
            xERP01M020.search();
        });
    }

    onDeleteAll = function () {
        xERP01M020.deleteall(function () {
            xERP01M020.search();
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

    xAjax.onKeyPress('#frmMaster #Icon', function () {
        $('#IconDisplay').attr('class', $('#Icon').val());
    });

});