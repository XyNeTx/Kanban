$(document).ready(function () {

    const xKBNMS015 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Supplier Code', 'Cycle', 'Kanban No', 'Store Code', 'Part No', 'Start Date', 'End Date', 'Color', 'Description'],
            "TH": ['Supplier Code', 'Cycle', 'Kanban No', 'Store Code', 'Part No', 'Start Date', 'End Date', 'Color', 'Description'],
            "JP": ['Supplier Code', 'Cycle', 'Kanban No', 'Store Code', 'Part No', 'Start Date', 'End Date', 'Color', 'Description'],
        },
        ColumnValue: [
            { "data": "F_Supplier_Code" },
            { "data": "F_Cycle" },
            { "data": "F_Kanban_No" },
            { "data": "F_Store_Code" },
            { "data": "F_Part_No" },
            { "data": "F_Start_Date" },
            { "data": "F_End_Date" },
            { "data": "F_Color" },
            { "data": "F_Text" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNMS015.prepare();

    xKBNMS015.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmCondition #F_Supplier', result.data.TB_Supplier, 'F_Supplier_Code', 'F_Supplier_Code');

        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS015.search();
    });


    xKBNMS015.onEditCallback = function () {
        $('#frmMaster #F_Color_Button').val('#ffffff');
        if ($('#frmMaster #F_Color').val() != '') {

            if ($('#frmMaster #F_Color').val().indexOf('#') == 0) {
                $('#frmMaster #F_Color_Button').val($('#frmMaster #F_Color').val());
            } else {
                var hexColor = colorToHexCode($('#frmMaster #F_Color').val());
                $('#frmMaster #F_Color_Button').val(hexColor);
            }
        }
    }

    onSave = function () {
        xKBNMS015.save(function () {
            xKBNMS015.search();
        });
    }

    onDelete = function () {
        xKBNMS015.delete(function () {
            xKBNMS015.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS015.deleteall(function () {
            xKBNMS015.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }


    xAjax.onChange('#frmCondition #F_Supplier', function () {

        console.log($('#frmCondition #F_Plant').val());
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNMS015.search();
    });


    xAjax.on('#frmMaster #F_Color_Button', function () {
        $('#frmMaster #F_Color').val($('#frmMaster #F_Color_Button').val());
    });





})

