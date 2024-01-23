$(document).ready(function () {

    const xKBNMS012 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Plant', 'Supplier Code', 'Store Code', 'Part No', 'Kanban No', 'Cycle'],
            "TH": ['Plant', 'Supplier Code', 'Store Code', 'Part No', 'Kanban No', 'Cycle'],
        },
        ColumnValue: [
            { "data": "F_Plant" },
            { "data": "F_Supplier_Code" },
            { "data": "F_Store_Code" },
            { "data": "F_Part_No" },
            { "data": "F_Kanban_No" },
            { "data": "F_Cycle" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ },
            { name: 'rdoOrder', value: '#frmCondition [name=rdoOrder]:checked' }
        ],
    });


    const tblDetail = xDataTable.Initial({
        name: 'tblDetail',
        running: 0,
        columnTitle: {
            "EN": ['Supplier', 'Store', 'Part', 'Kanban', '01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23', '24', '25', '26', '27', '28', '29', '30'],
            "TH": ['Supplier', 'Store', 'Part', 'Kanban', '01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23', '24', '25', '26', '27', '28', '29', '30'],
            "JP": ['Supplier', 'Store', 'Part', 'Kanban', '01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23', '24', '25', '26', '27', '28', '29', '30'],
        },
        column: [
            { "data": "F_Supplier_Code" },
            { "data": "F_Store_Code" },
            { "data": "F_Part_No" },
            { "data": "F_Kanban_No" },
            { "data": "F1" }, { "data": "F2" }, { "data": "F3" }, { "data": "F4" }, { "data": "F5" }, { "data": "F6" }, { "data": "F7" }, { "data": "F8" }, { "data": "F9" }, { "data": "F10" },
            { "data": "F11" }, { "data": "F12" }, { "data": "F13" }, { "data": "F14" }, { "data": "F15" }, { "data": "F16" }, { "data": "F17" }, { "data": "F18" }, { "data": "F19" }, { "data": "F20" },
            { "data": "F21" }, { "data": "F22" }, { "data": "F23" }, { "data": "F24" }, { "data": "F25" }, { "data": "F26" }, { "data": "F27" }, { "data": "F28" }, { "data": "F29" }, { "data": "F30" }
        ],
        addnew: false,
        then: function (config) {
        }
    });

    

    xKBNMS012.prepare();

    xKBNMS012.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS012.search();
    });

    xKBNMS012.onEditCallback = function (row) {

        xSplash.show();
        //console.log(row);

        let _data = [
            {
                "RunningNo": "",
                "F_Supplier_Code": $('#frmMaster #F_Supplier_Code').val(),
                "F_Store_Code": $('#frmMaster #F_Store_Code').val(),
                "F_Part_No": $('#frmMaster #F_Part_No').val(),
                "F_Kanban_No": $('#frmMaster #F_Kanban_No').val(),
            }
        ];

        for (var i = 1; i <= 30; i++) {
            _data[0]["F" + i] = "<center><input type='checkbox' id='' name='' value=''></center>";
        }
        //console.log(row.F_Delivery_Trip);

        $('#tblDetail').dataTable().fnClearTable();
        if (_data.length > 0) {
            $('#tblDetail').dataTable().fnAddData(_data);


            for (var v = 0; v <= 30; v++) $('#tblDetail').DataTable().column(v).visible(true);

            let _c = row.F_Delivery_Trip;
            for (var i = _c; i < 30; i++) $('#tblDetail').DataTable().column(5 + i).visible(false);

            xSplash.hide();
        }


    }


    onSave = function () {
        xKBNMS012.save(function () {
            xKBNMS012.search();
        });
    }

    onDelete = function () {
        xKBNMS012.delete(function () {
            xKBNMS012.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS012.deleteall(function () {
            xKBNMS012.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }

    xAjax.onChange('#frmCondition #F_Plant', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNMS012.search();
    });
    xAjax.onChange('#frmCondition [name=rdoOrder]', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNMS012.search();
    });





})

