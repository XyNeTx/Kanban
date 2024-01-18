$(document).ready(function () {

    const xKBNMS022 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Supplier Code', 'Cycle Time', 'Delivery Start', 'Delivery End', 'Order Start', 'Order End', 'Delivery Trip', 'Time'],
            "TH": ['Supplier Code', 'Cycle Time', 'Delivery Start', 'Delivery End', 'Order Start', 'Order End', 'Delivery Trip', 'Time'],
            "JP": ['Supplier Code', 'Cycle Time', 'Delivery Start', 'Delivery End', 'Order Start', 'Order End', 'Delivery Trip', 'Time'],
        },
        ColumnValue: [
            { "data": "F_Supplier_Code" },
            { "data": "F_Cycle" },
            { "data": "F_Start_Date" },
            { "data": "F_End_Date" },
            { "data": "F_Start_Order_Date" },
            { "data": "F_End_Order_Date" },
            { "data": "F_Delivery_Trip" },
            { "data": "F_Delivery_Time" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNMS022.prepare();

    xKBNMS022.initial(function (result) {
        xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
        xDropDownList.bind('#frmMaster #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

        xKBNMS022.search();
    });

    onSave = function () {
        xKBNMS022.save(function () {
            xKBNMS022.search();
        });
    }

    onDelete = function () {
        xKBNMS022.delete(function () {
            xKBNMS022.search();
        });
    }

    onDeleteAll = function () {
        xKBNMS022.deleteall(function () {
            xKBNMS022.search();
        });
    }

    onPrint = function () { }

    onExecute = function () { }

    xAjax.onChange('#frmCondition #F_Plant', function () {
        $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

        xKBNMS022.search();
    });





})

