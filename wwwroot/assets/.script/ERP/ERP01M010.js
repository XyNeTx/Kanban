$(document).ready(function () {

    const ERP01M010 = new MasterTemplate({
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


    let UID;
    initial = async function () {

        var tblMenu = xDataTable.Initial({
            name: 'tblMenu',
            //dom: '<"clear">',
            running: 0,
            orderNo: 0,
            checking: 0,
            pageLength: -1,
            columnTitle: {
                "EN": ['Code', 'Name', 'Name (TH)', 'Name (JP)', 'Current Status'],
                "TH": ['Code', 'Name', 'Name (TH)', 'Name (JP)', 'Current Status'],
                "JP": ['Code', 'Name', 'Name (TH)', 'Name (JP)', 'Current Status'],
            },
            column: [
                { "data": "Code" },
                { "data": "Name" },
                { "data": "NameTH" },
                { "data": "NameJP" },
                { "data": "Checked" }
            ],
            addnew: false,
            rowclick: (data) => {
                //console.log(data);
            },
            eventclick: async (data, r, c, e) => {

                //console.log(data);

                var _dt = await xAjax.xExecuteJSON({
                    data: {
                        "Module": "[exec].[spERP01M010_SetMenu]",
                        "pUserCode": ajexHeader.UserCode,
                        "User_ID": UID,
                        "Menu_ID": data._ID,
                        "Remark": data.Code + ' : ' + data.Name,
                        "Check": ($('#' + e.id).is(':checked') ? '1' : '0')
                    },
                });
                //console.log(_dt);

                if (_dt.rows != null) xDataTable.bind('#tblMenu', _dt.rows);
                if (_dt.rows != null) {
                    for (var i = 0; i < _dt.rows.length; i++) {
                        if (_dt.rows[i].isCheck == 1) $('#tblMenu_SELECTALL_' + (i + 1)).attr('checked', 'checked');
                    }
                }



            },
            then: function (config) {
                xSplash.hide();
            }
        });


    }

    initial();



    ERP01M010.prepare();

    ERP01M010.initial(function (result) {
        xDropDownList.bind('#frmMaster #GroupID', result.data.erpGroup, '_ID', 'Name');
        xDropDownList.bind('#frmMaster #Title_ID', result.data.erpTitle, '_ID', 'Title');
        xDropDownList.bind('#frmMaster #TitleTH_ID', result.data.erpTitle, '_ID', 'TitleTH');
        xDropDownList.bind('#frmMaster #TitleJP_ID', result.data.erpTitle, '_ID', 'TitleJP');

        ERP01M010.search();
    });


    onEdit = async function (data) {
        //console.log(data);
        if (data == null) {
            xDataTable.bind('#tblMenu', null);
            return false;
        }

        UID = data._ID;
        var _dt = await xAjax.xExecuteJSON({
            data: {
                "Module": "[exec].[spERP01M010_MenuList]",
                "pUserCode": data.Code
            },
        });
        //console.log(_dt);
        if (_dt.rows != null) xDataTable.bind('#tblMenu', _dt.rows);

        //console.log(data);
        if (_dt.rows != null) {
            for (var i = 0; i < _dt.rows.length; i++) {
                if (_dt.rows[i].isCheck == 1) $('#tblMenu_SELECTALL_' + (i+1)).attr('checked', 'checked');
            }
        }
    }


    onSave = function () {
        ERP01M010.save(function () {
            ERP01M010.search();
        });
    }

    onDelete = function () {
        ERP01M010.delete(function () {
            ERP01M010.search();
        });
    }

    onDeleteAll = function () {
        ERP01M010.deleteall(function () {
            ERP01M010.search();
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

        ERP01M010.search();
    });


});