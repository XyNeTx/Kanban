$(document).ready(function () {

    const tblMaster = xDataTable.Initial({
        name: 'tblMaster',
        scrollbar: "300px",
        dom:'<"top"f>t<"clear">',
        columnTitle: {
            "EN": ['Supplier Code'],
            "TH": ['Supplier Code'],
            "JP": ['Supplier Code'],
        },
        column: [
            { "data": "F_Supplier" }
        ],
        addnew: false,
        then: function (config) {
        },
        rowclick: function (row) {
            //console.log(row);
            //xSplash.show();

            xAjax.Post({
                url: 'KBNMS009/search',
                data: {
                    "F_Plant": $('#frmCondition #F_Plant').val(),
                    "SupplierCode": row.F_Supplier,
                    "UID": _UID_
                },
                then: function (result) {
                    //console.log(result);
                    xDataTable.Bind('tblDetail', result.data);

                    //xSplash.hide();
                }
            })        
        }
    });

    const tblDetail = xDataTable.Initial({
        name: 'tblDetail',
        scrollbar: "300px",
        dom: '<"top"f>t<"clear">',
        //checking: 0,
        //running: 0,
        columnTitle: {
            "EN": ['Supplier Code', 'Supplier Plant', 'Kanban No', 'Supply Code', 'Number', 'Store Code'],
            "TH": ['Supplier Code', 'Supplier Plant', 'Kanban No', 'Supply Code', 'Number', 'Store Code'],
            "JP": ['Supplier Code', 'Supplier Plant', 'Kanban No', 'Supply Code', 'Number', 'Store Code'],
        },
        column: [
            { "data": "F_Supplier_Code" },
            { "data": "F_Supplier_Plant" },
            { "data": "F_Kanban_No" },
            { "data": "F_Supply_Code" },
            { "data": "F_Number" },
            { "data": "F_Store_Code" }
        ],
        //order: 0,
        addnew: false, 
        then: function (config) {
        }
    });


    initial = function () {
        xAjax.Post({
            url: 'KBNMS009/initial',
            callback: function (result) {
                //console.log(result);
                xDropDownList.Bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');

                xDataTable.Bind('tblMaster', result.data.PPM_T_Construction);

                xSplash.hide();
            }
        })        
    }
    initial();



})

