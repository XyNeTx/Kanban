$(document).ready(function () {

    const xKBNMS017 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Part No', 'Store Code', 'Address1', 'Ratio1', 'Address2', 'Ratio2', 'Address3', 'Ratio3'],
            "TH": ['Part No', 'Store Code', 'Address1', 'Ratio1', 'Address2', 'Ratio2', 'Address3', 'Ratio3'],
            "JP": ['Part No', 'Store Code', 'Address1', 'Ratio1', 'Address2', 'Ratio2', 'Address3', 'Ratio3'],
        },
        ColumnValue: [
            { "data": "F_Part_No" },
            { "data": "F_Store_Code" },
            { "data": "F_Address1" },
            { "data": "F_Ratio1" },
            { "data": "F_Address2" },
            { "data": "F_Ratio2" },
            { "data": "F_Address3" },
            { "data": "F_Ratio3" }
        ],
        //order: [[3, 'asc']],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNMS017.prepare();

    xKBNMS017.initial(function (result) {
        //console.log(result);
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmCondition #F_Supplier', result.data.TB_Supplier, 'F_Supplier_Code', 'F_Supplier_Code');

        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_Group', result.data.TB_Group, 'F_Group', 'F_Group');

        xDropDownList.bind('#frmMaster #F_Address1', result.data.PPM_T_Address, 'F_Address', 'F_Address');
        xDropDownList.bind('#frmMaster #F_Address2', result.data.PPM_T_Address, 'F_Address', 'F_Address');
        xDropDownList.bind('#frmMaster #F_Address3', result.data.PPM_T_Address, 'F_Address', 'F_Address');

        xKBNMS017.search();
    });



    onSave = function () {
        xKBNMS017.save(function () {
            xKBNMS017.search();
        });
    }

    onDelete = function () {
        xKBNMS017.delete(function () {
            xKBNMS017.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS017.deleteall(function () {
            xKBNMS017.search();
        });
    }

    onPrint = function () { }

    onExecute = function () {
        console.log('onExecute');
    }


    xAjax.onChange('#frmCondition #F_Supplier', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNMS017.search();
    });


    xAjax.on('#frmMaster #txtColor', function () {
        $('#frmMaster #F_Color').val($('#frmMaster #txtColor').val());
    });





})

