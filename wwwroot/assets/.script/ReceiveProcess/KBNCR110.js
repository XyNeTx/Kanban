$(document).ready(function () {

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
            { "data": "F_Delivery_Time" }
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
        const _TableName = this.TableName;
        console.log(this.TableName);
        xAjax.Post({
            url: 'KBNCR110/SearchPDSNo',
            data: {
                'F_PDS_No': $('#F_PDS_No').val(),
                'F_Delivery_Date': $('#F_DeliveryFrom').val()
            },
            then: function (result) {
                console.log(result);
                if (result.response == "OK") {
                    if (result.data.length > 0) {
                        $('#F_PDS_No').val("");
                        console.log(result.data.length);
                        $('#tblMaster').dataTable().fnAddData(result.data);
                    }
                }
            },
            error: function (result) {
                console.error(_Controller + '.Search: ' + result.responseText);
                xSplash.hide();
            }
        });
    });

    //$('#F_PDS_No').on('Keypress', function (e) {
    //    if (e.keycode == 13) {
    //    }
    //})
});