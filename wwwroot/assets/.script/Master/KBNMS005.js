$(document).ready(function () {

    const xKBNMS005 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['No.', 'Supplier_Code', 'Supplier_Plant', 'Part_No', 'Ruibetsu', 'Kanban_No'],
            "TH": ['No.', 'Plant', 'F_Parent_Part', 'F_Part_Name', 'Effective Date', 'End Date'],
        },
        ColumnValue: [
            { "data": "Supplier_Code" },
            { "data": "Supplier_Code" },
            { "data": "Supplier_Plant" },
            { "data": "Part_No" },
            { "data": "Ruibetsu" },
            { "data": "Kanban_No" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ },
            { name: 'F_Supplier', value: '#frmCondition #F_Supplier' },
            { name: 'F_Start_Date', value: '#frmCondition #F_Start_Date' },
            { name: 'F_End_Date', value: '#frmCondition #F_End_Date' }
        ],
    });

    xKBNMS005.prepare();

    xKBNMS005.initial(function (result) {
        //console.log(result);
        //xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmCondition #F_Supplier', result.data.TB_Supplier, 'F_Supplier_Code', 'F_Supplier_Code');

        //xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS005.search();
    });

    onSave = function () {
        xKBNMS005.save(function () {
            xKBNMS005.search();
        });
    }

    onDelete = function () {
        xKBNMS005.delete(function () {
            xKBNMS005.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS005.deleteall(function () {
            xKBNMS005.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }

    //xAjax.onChange('#frmCondition #F_Supplier', function () {
    //    $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

    //    xKBNMS005.search();
    //});





})

