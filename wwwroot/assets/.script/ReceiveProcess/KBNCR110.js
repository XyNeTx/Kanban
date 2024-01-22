$(document).ready(function () {
    var pdsSet = new Set();
    const KBNCR110 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['PDS No.', 'Delivery Date', 'Delivery Trip'],
            "TH": ['PDS No.', 'Delivery Date', 'Delivery Trip'],
            "JP": ['PDS No.', 'Delivery Date', 'Delivery Trip'],
        },
        ColumnValue: [
            { "data": "F_OrderNo" },
            { "data": "F_Delivery_Date" },
            { "data": "F_Delivery_Trip" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });


    KBNCR110.prepare();

    KBNCR110.initial(function (result) {

        xAjax.onCheck('#chkDeliveryDate', function () {
            if ($('#chkDeliveryDate').val() == 0) $('#fldDeliveryDate').prop('disabled', 'disabled');
            if ($('#chkDeliveryDate').val() == 1) $('#fldDeliveryDate').prop('disabled', false);
        });
        xSplash.hide();
        //KBNCR110.search();
    });

    onSave = function () {
        KBNCR110.save(function () {
            KBNCR110.search();
        });
    }

    onDelete = function () {
        KBNCR110.delete(function () {
            KBNCR110.search();
        });
    }

    onDeleteAll = function () {
        KBNCR110.deleteall(function () {
            KBNCR110.search();
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

    //xSwal.question('', '',
    //    function (result) {
    //        console.log('callback')
    //    },
    //    function () {

    //    }

    //)

    xAjax.onEnter('#F_PDS_No', function () {
        var pdsNo = $('#F_PDS_No').val();
        console.log(pdsNo);
        xAjax.Post({
            url: 'KBNCR110/SearchPDSNo',
            data: {
                'F_PDS_No': pdsNo,
                'F_Delivery_Date': $('#F_DeliveryFrom').val()
            },
            then: function (result) {
                if (result.response == "OK") {
                    if (result.data != null) {
                        $('#F_PDS_No').val("");
                        console.log(pdsSet.size + "90");
                        if (pdsSet.has(pdsNo)) {
                            alert("Duplicate PDS No.")
                        }
                        else {
                            console.log(result + "line 88");
                            $('#tblMaster').dataTable().fnAddData(result.data);
                            pdsSet.add(pdsNo);
                            console.log(pdsSet.size + "98")
                        }
                    }
                    else {
                        $('#F_PDS_No').val("");
                        alert(result.message);
                    }
                }
            },
            error: function (result) {
                console.error(_Controller + '.SearchPDSNo: ' + result.responseText);
                xSplash.hide();
            }
        });
    });
});