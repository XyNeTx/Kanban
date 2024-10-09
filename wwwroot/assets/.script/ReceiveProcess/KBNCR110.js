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
        console.log(result);

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

    xAjax.onEnter('#F_PDS_No', function () {
        var pdsNo = $('#F_PDS_No').val();
        // console.log(pdsNo);
        xAjax.Post({
            url: 'KBNCR110/CheckPDSNo',
            data: {
                'F_PDS_No': pdsNo
            },
            then: function (result) {
                if (result.response == "OK") {
                    if (result.data != null) {
                        $('#F_PDS_No').val("");
                        // console.log(pdsSet.size + "90");
                        if (pdsSet.has(pdsNo)) {
                            xSwal.error("Duplicate PDS No.", "Please enter other PDS No.");
                        }
                        else {
                            // console.log(result + "line 88");
                            $('#tblMaster').dataTable().fnAddData(result.data);
                            pdsSet.add(pdsNo);
                            // console.log(pdsSet.size + "98")
                        }
                    }
                    else {
                        $('#F_PDS_No').val("");
                        xSwal.error(result.title, result.message);
                    }
                }
            },
            error: function (result) {
                console.error(_Controller + '.SearchPDSNo: ' + result.responseText);
                xSplash.hide();
            }
        });
    });

    xAjax.onClick("#ClearBtn", function () {
        $('#tblMaster').DataTable().clear();
        $('#tblMaster').DataTable().draw();
        pdsSet.clear();
    });


});

$("#ReceiveBtn").click(async function () {

    $("#ReceiveBtn").prop('disabled', true)
    var _selData = [];
    var allPages = $('#tblMaster').DataTable().cells().nodes();
    $("#ReceiveBtn").prop('disabled', true)
    $(allPages).find('input[type="checkbox"]').each(function () {
        if ($(this).prop('checked')) {
            var _val = $($(this)).val();
            _selData.push(JSON.parse(`{` + ReplaceAll(_val, `'`, `"`) + `}`));
        }
    });

    // console.log(_selData);
    xAjax.Post({
        url: 'KBNCR110/ReceiveAllPart',
        data: {
            'F_PDS_No': _selData,
        },
        then: async function (result) {
            console.log(result)
            if (result.status == "200") {
                $('#tblMaster').DataTable().clear();
                $('#tblMaster').DataTable().draw();
                pdsSet.clear();
                $("#ReceiveBtn").prop('disabled', false);
                xSwal.success(result.title, result.message);
            }
            else {
                $("#ReceiveBtn").prop('disabled', false);
                xSwal.error(result.title, result.message);
            }
        },
        error: function (result) {
            console.error(_Controller + '.ReceiveAllPart: ' + result.responseText);
            $("#ReceiveBtn").prop('disabled', false);
            xSplash.hide();
        }
    });

    xSplash.hide();
});

$("#uploadEpro").click(function () {
    var _selData = [];
    var allPages = $('#tblMaster').DataTable().cells().nodes();
    $(allPages).find('input[type="checkbox"]').each(function () {
        if ($(this).prop('checked')) {
            var _val = $($(this)).val();
            _selData.push(JSON.parse(`{` + ReplaceAll(_val, `'`, `"`) + `}`));
        }
    });
    xAjax.Post({
        url: 'KBNCR110/UploadToEpro',
        data: {
            'F_PDS_No': _selData,
        },
        then: function (result) {
            console.log(result)
            if (result.status == "200") {
                xSwal.success(result.title, result.message);
                //$('#tblMaster').DataTable().clear();
                //$('#tblMaster').DataTable().draw();
                //pdsSet.clear();
            }
            else {
                xSwal.error(result.title, result.message);
            }
        },
        error: function (result) {
            console.error(_Controller + '.ReceiveAllPart: ' + result.responseText);
            xSplash.hide();
        }
    });
});