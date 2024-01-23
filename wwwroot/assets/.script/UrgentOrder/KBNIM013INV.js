$(document).ready(function () {

    const KBNIM013INV = new ActionTemplate({
        //Controller: _PAGE_,
        Form: 'frmCondition'
    });

    KBNIM013INV.prepare(function () {

        var tblPDS = xDataTable.Initial({
            name: 'tblPDS',
            checking: 0,
            dom: '<"clear">',
            columnTitle: {
                "EN": ['Separate Declaration No.', 'Refer. Doc.', 'Delivery Date'],
                "TH": ['Separate Declaration No.', 'Refer. Doc.', 'Delivery Date'],
                "JP": ['Separate Declaration No.', 'Refer. Doc.', 'Delivery Date'],
            },
            column: [
                { "data": "F_Declare_No" },
                { "data": "F_Pds_No" },
                { "data": "F_Delivery_Date" }
            ],
            addnew: false,
            rowclick: (row) => {
            }
        });

        var tblSeparate = xDataTable.Initial({
            name: 'tblSeparate',
            checking: 0,
            dom: '<"clear">',
            columnTitle: {
                "EN": ['Separate Declaration No.', 'Refer. Doc.', 'Delivery Date', 'Part No.', 'Kanban No.', 'Supplier', 'Qty'],
                "TH": ['Separate Declaration No.', 'Refer. Doc.', 'Delivery Date', 'Part No.', 'Kanban No.', 'Supplier', 'Qty'],
                "JP": ['Separate Declaration No.', 'Refer. Doc.', 'Delivery Date', 'Part No.', 'Kanban No.', 'Supplier', 'Qty'],
            },
            column: [
                { "data": "F_Declare_No" },
                { "data": "F_Pds_No" },
                { "data": "F_Delivery_Date" },
                { "data": "F_Part_No" },
                { "data": "F_Kanban_NO" },
                { "data": "F_SUpplier" },
                { "data": "F_QTY" }
            ],
            addnew: false,
            rowclick: (row) => {
            }
        });

    });



    KBNIM013INV.initial(function () {
        //xAjax.sql(` EXEC [exec].[spTB_MS_FACTORY] `);
        //xAjax.sql(` EXEC [exec].[spKBNIM013INV_SEARCH] 'F_Plant' `);
        //xAjax.sql(` EXEC [exec].[spKBNIM013INV_DETAIL_SEARCH] 'F_Plant' `);
        
        xAjax.onClick('#chkDeliveryDate', function () {
            if ($('#chkDeliveryDate').val() == 0) $('#fldDeliveryDate').prop('disabled', 'disabled');
            if ($('#chkDeliveryDate').val() == 1) $('#fldDeliveryDate').prop('disabled', false);
        });


        xAjax.Post({
            url: 'KBNIM013INV/Search',
            data: {
                'F_Plant': '3'
            },
            then: function (result) {
                //console.log(result);
                $('#tblPDS').dataTable().fnClearTable();
                $('#tblSeparate').dataTable().fnClearTable();
                if (result.data.PDS.length > 0) {
                    $('#tblPDS').dataTable().fnAddData(result.data.PDS);
                    $('#tblSeparate').dataTable().fnAddData(result.data.Separate);
                }
            }
        })


        xSplash.hide();
    })

    onExecute = function () {
        KBNIM013INV.execute(function () {
            console.log('onExecute');
        })
    }

    onDelete = function () {
        KBNIM013INV.delete(function () {
            console.log('onDelete');
        })
    }

});

