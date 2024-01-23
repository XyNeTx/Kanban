$(document).ready(function () {

    const KBNIM003C = new ActionTemplate({
        Controller: _PAGE_,
        Form: 'frmCondition',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ]
    });

    KBNIM003C.prepare(function () {

        var tblPDS = xDataTable.Initial({
            name: 'tblMaster',
            checking: 0,
            //dom: '<"clear">',
            columnTitle: {
                "EN": ['Order No.', 'Order Issued Date', 'Delivery Date'],
                "TH": ['Order No.', 'Order Issued Date', 'Delivery Date'],
                "JP": ['Order No.', 'Order Issued Date', 'Delivery Date'],
            },
            column: [
                { "data": "F_PDS_No" },
                { "data": "F_PDS_ISSUED_DATE" },
                { "data": "F_Delivery_Date" }
            ],
            addnew: false,
            rowclick: (row) => {
            }
        });

    });


    KBNIM003C.initial(function () {

        xAjax.onClick('#chkDeliveryDate', function () {
            if ($('#chkDeliveryDate').val() == 0) $('#fldDeliveryDate').prop('disabled', 'disabled');
            if ($('#chkDeliveryDate').val() == 1) $('#fldDeliveryDate').prop('disabled', false);
        });

        xSplash.hide();
    })


    onSave = function () {

        xSwal.questionPack(
            _i18n.model.update,
            function (result) {
                if (result.isConfirmed) {
                    xSplash.show();
                    xSplash.text('PROCESSING');

                    xAjax.Post({
                        url: 'KBNIM003C/update',
                        data: `{ 
                            "F_Plant": "`+ _PLANT_ + `",
                            "F_Type": "EKanban",
                            "F_Start_Date": "`+ ReplaceAll(_PROCESSDATE_, '-', '') + `",
                            "F_End_Date": "`+ ReplaceAll(_PROCESSDATE_, '-', '') + `"
                        }`,
                        then: function (result) {
                            if (result.response == 'OK') {
                                //console.log(result);
                                xSwal.infoPack(_i18n.model.update.success);
                                xSplash.hide();
                            }
                        }
                    })


                }
            });
    }


    onExecute = function () {

        xSwal.questionPack(
            _i18n.model.confirm,
            function (result) {
                if (result.isConfirmed) {
                    xSplash.show();
                    xSplash.text('PROCESSING');



                    xAjax.Post({
                        url: 'KBNIM003C/confirm',
                        data: `{ 
                            "F_Plant": "`+ _PLANT_ + `",
                            "F_Type": "EKanban",
                            "F_PDS_NO": "`+ ReplaceAll(_PROCESSDATE_, '-', '') + `",
                            "F_PDS_Issued_Date": "`+ ReplaceAll(_PROCESSDATE_, '-', '') + `",
                            "F_Delivery_Date": "`+ ReplaceAll(_PROCESSDATE_, '-', '') + `"
                        }`,
                        then: function (result) {
                            if (result.response == 'OK') {
                                //console.log(result);
                                xSwal.infoPack(_i18n.model.confirm.success);
                                xSplash.hide();
                            }
                        }
                    })


                }
            });
    }



});