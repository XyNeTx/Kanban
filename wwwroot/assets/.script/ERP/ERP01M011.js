$(document).ready(function () {

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


    this.tblUser = xDataTable.Initial({
        name: 'tblUser',
        checking: 0,
        running: 0,
        columnTitle: {
            "EN": ['Code', 'Name', 'Name', 'Name (Thai)', 'Status'],
            "TH": ['Code', 'Name', 'Name', 'Name (Thai)', 'Status'],
            "JP": ['Code', 'Name', 'Name', 'Name (Thai)', 'Status'],
        },
        column: [
            { "data": "Code" },
            { "data": "Name" },
            { "data": "NameTH" },
            { "data": "NameTH" },
            { "data": "Status" }
        ],
        orderNo: true,
        addnew: false,
        rowclick: (row) => {
        },
        then: function (config) {
            //xSplash.hide();
        }
    });
    this.tblMenu = xDataTable.Initial({
        name: 'tblMenu',
        checking: 0,
        running: 0,
        columnTitle: {
            "EN": ['Code', 'Name', 'Title', 'Icon', 'Seq', 'Status'],
            "TH": ['Code', 'Name', 'Title', 'Icon', 'Seq', 'Status'],
            "JP": ['Code', 'Name', 'Title', 'Icon', 'Seq', 'Status'],
        },
        column: [
            { "data": "Code" },
            { "data": "Name" },
            { "data": "Title" },
            { "data": "Icon" },
            { "data": "Seq" },
            { "data": "Status" }
        ],
        orderNo: true,
        addnew: false,
        rowclick: (row) => {
            onEditMenu(row);
        },
        then: function (config) {
            //xSplash.hide();
        }
    });

    xERP01M011.prepare();

    xERP01M011.initial(function (result) {
        xERP01M011.search();
    });

    onEdit = function () {
        xAjax.Post({
            url: 'ERP01M011/Detail',
            data: {
                'GroupID': $('#_ID').val()
            },
            then: function (result) {
                $('#tblUser').dataTable().fnClearTable();
                $('#tblMenu').dataTable().fnClearTable();
                if (result.data.User.length > 0) {
                    $('#tblUser').dataTable().fnAddData(result.data.User);
                    $('#tblMenu').dataTable().fnAddData(result.data.Menu);
                }
            }
        })
    }

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

    //xAjax.onChange('#frmCondition #F_Plant', function () {
    //    $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

    //    xERP01M011.search();
    //});



    onEditMenu = function (data) {
        console.log(data);

        
        $('#frmMenu').removeClass('was-validated');
        $('#modalMenu').modal('toggle');

        //xAjax.Post({
        //    url: 'ERP01M011/Detail',
        //    data: {
        //        'GroupID': $('#_ID').val()
        //    },
        //    then: function (result) {

        //        $('#tblUser').dataTable().fnClearTable();
        //        $('#tblMenu').dataTable().fnClearTable();
        //        if (result.data.User.length > 0) {
        //            $('#tblUser').dataTable().fnAddData(result.data.User);
        //            $('#tblMenu').dataTable().fnAddData(result.data.Menu);
        //        }
        //    }
        //})
    }

});