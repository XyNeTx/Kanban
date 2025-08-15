$(document).ready(function () {


    loadOrderType = async function () {
        //console.log(rdoOrderType.value);

        var _dt = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNOR700]",
                "pUserCode": ajexHeader.UserCode,
                "F_Plant": ajexHeader.Plant,
                "F_orderType": (rdoOrderType.value == 'Special' ? 'S' : (rdoOrderType.value == 'Urgent' ? 'U' : 'N')),
                "pType": 'PDS'
            },
        });
        if (_dt.rows == null) MsgBox("Order data not found.", MsgBoxStyle.Information, "Information");

        $('#itmPDS').empty();
        $('#itmPDSTo').empty();

        $('#itmPDS').append(`<option value="" hidden></option>`);
        $('#itmPDSTo').append(`<option value="" hidden></option>`);

        $("#itmPDS").selectpicker('refresh');
        $("#itmPDSTo").selectpicker('refresh');
        _dt.rows.forEach(function (item) {
            $('#itmPDS').append(`<option value="${item.F_OrderNo}">${item.F_OrderNo}</option>`);
            $('#itmPDSTo').append(`<option value="${item.F_OrderNo}">${item.F_OrderNo}</option>`);
        });

        $("#itmPDS").selectpicker('refresh');
        $("#itmPDSTo").selectpicker('refresh');

    };

    $("#itmPDS").change(function () {
        itmPDS.value = $(this).val();
    });


    initial = async function () {
        loadOrderType();
        $("#itmPDS").selectpicker();
        $("#itmPDSTo").selectpicker();

        xSplash.hide();
    };
    initial();






    xAjax.onChange('#chkPDSNo', function () {
        if ($('#chkPDSNo').val() == 0) $('#fldPDSNo').prop('disabled', 'disabled');
        if ($('#chkPDSNo').val() == 1) $('#fldPDSNo').prop('disabled', false);
    });

    xAjax.onChange('#chkSupplierCode', function () {
        if ($('#chkSupplierCode').val() == 0) $('#fldSupplierCode').prop('disabled', 'disabled');
        if ($('#chkSupplierCode').val() == 1) $('#fldSupplierCode').prop('disabled', false);
    });

    xAjax.onChange('#chkDeliveryDate', function () {
        if ($('#chkDeliveryDate').val() == 0) $('#fldDeliveryDate').prop('disabled', 'disabled');
        if ($('#chkDeliveryDate').val() == 1) $('#fldDeliveryDate').prop('disabled', false);
    });












    xAjax.onClick('#btnPlanning', async function () {

        if (rdoOrderType.value == 'Normal') {
            rdoOrderType.check = 'Special';
        } else if (rdoOrderType.value == 'Special') {
            rdoOrderType.check = 'Urgent';
        } else {
            rdoOrderType.check = 'Normal';
        }

        //MsgBox("Do you want to get planning data?", MsgBoxStyle.OkCancel, async function () {

        //    var _dt = await xAjax.ExecuteJSON({
        //        data: {
        //            "Module": "[exec].[spKBNDL001_PLANNING]"
        //        },
        //    });

        //    console.log(_dt);
        //})
    });






    xAjax.onClick('#btnPrintPDS', async function () {
        //console.log(itmSupplier.value);
        //console.log(itmSupplierTo.value);
        //console.log(chkPDSNo.value);
        //console.log($('#chkPDSNo').val());
        //console.log($('#chkSupplierCode').val());
        //console.log($('#chkDeliveryDate').val());

        if ($('#chkPDSNo').val() == 0 && $('#chkSupplierCode').val() == 0 && $('#chkDeliveryDate').val() == 0) {
            MsgBox("Please select criteria, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");
            return false;
        }

        if ($('#chkPDSNo').val() == 1 && (itmPDS.value === undefined || itmPDS.value === undefined)) {
            MsgBox("Please input PDS From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");
            return false;
        }
        
        if ($('#chkDeliveryDate').val() == 1 && (itmDelivery.value === undefined || itmDeliveryTo.value === undefined)) {
            MsgBox("Please select Delivery Date From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");
            return false;
        }

        let isDeleted = await DeleteKBNOR_450();
        console.log(isDeleted);
        if (isDeleted !== true) {
            return xSwal.error("Can't Delete Previous Data");
        }

        //console.log(($('#chkSupplierCode').val() == 1 ? $('#itmDelivery').val() : ''));
        //ajexHeader.Plant = '1';
        var _dt = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNOR700_PDS]",
                "@pUserCode": ajexHeader.UserCode,
                "@pPlant": ajexHeader.Plant,
                "@pDeliveryDate": xDate.Date('yyyyMMdd', 'MM=-3'),
                "@F_orderType": (rdoOrderType.value == 'Special' ? 'S' : (rdoOrderType.value == 'Urgent' ? 'U' : 'N')),
                "@F_OrderNo": ($('#chkPDSNo').val() == 1 ? itmPDS.value : ''),
                "@F_OrderNoTo": ($('#chkPDSNo').val() == 1 ? itmPDSTo.value : ''),
                "@F_Delivery_Date": ($('#chkDeliveryDate').val() == 1 ? ReplaceAll(itmDelivery.value,'-','') : ''),
                "@F_Delivery_DateTo": ($('#chkDeliveryDate').val() == 1 ? ReplaceAll(itmDeliveryTo.value, '-', '') : '')
            },
        });
        if (_dt.rows == null) MsgBox("Order data not found.", MsgBoxStyle.Information, "Information");
        if (_dt.rows != null) {
            var _pds = '';
            for (var i=0; i < _dt.rows.length; i++) {
                _pds = _pds + Trim(_dt.rows[i].F_Barcode) + ',';
            }

            console.log(_pds);

            let dateFrom = ($('#chkDeliveryDate').val() == 1 ? itmDelivery.value : '2024-07-01');
            let dateTo = ($('#chkDeliveryDate').val() == 1 ? itmDeliveryTo.value : '2999-12-31');

            window.open(`http://hmmta-tpcap/E-Report/Report.aspx?Register=REC&PDSNoFrom=${itmPDS.value}&PDSNoTo=${itmPDSTo.value}&DateFrom=${dateFrom}&DateTo=${dateTo}`);

            await xAjax.Post({
                url: 'KBNOR700/PDS_GENBARCODE',
                data: {
                    'PDSNO': _pds
                },
                then: function (result) {
                    //console.log(result);
                    if (_xLib.GetCookie('isDev') == "1") {
                        window.open(
                            _REPORTINGSERVER_ + '%2fKB3%2fKBNOR700PDS_X2&rs:Command=Render'
                            + '&pUserCode=' + ajexHeader.UserCode
                            + '&OrderNo=' + ($('#chkPDSNo').val() == 1 ? itmPDS.value : '')
                            + '&OrderNoTo=' + ($('#chkPDSNo').val() == 1 ? itmPDSTo.value : '')
                            + '&DeliveryDate=' + ($('#chkDeliveryDate').val() == 1 ? ReplaceAll(itmDelivery.value, '-', '') : '')
                            + '&DeliveryDateTo=' + ($('#chkDeliveryDate').val() == 1 ? ReplaceAll(itmDeliveryTo.value, '-', '') : '')
                            + '&F_Plant=' + ajexHeader.Plant
                            , '_blank'
                        );
                    }
                    else {
                        window.open(
                            _REPORTINGSERVER_ + '%2fKB3%2fKBNOR700PDS&rs:Command=Render'
                            + '&pUserCode=' + ajexHeader.UserCode
                            + '&OrderNo=' + ($('#chkPDSNo').val() == 1 ? itmPDS.value : '')
                            + '&OrderNoTo=' + ($('#chkPDSNo').val() == 1 ? itmPDSTo.value : '')
                            + '&DeliveryDate=' + ($('#chkDeliveryDate').val() == 1 ? ReplaceAll(itmDelivery.value, '-', '') : '')
                            + '&DeliveryDateTo=' + ($('#chkDeliveryDate').val() == 1 ? ReplaceAll(itmDeliveryTo.value, '-', '') : '')
                            + '&F_Plant=' + ajexHeader.Plant
                            , '_blank'
                        );
                    }

                }
            })

        }


    });



    xAjax.onClick('#btnPrintKanban', async function () {

        if ($('#chkPDSNo').val() == 0 && $('#chkSupplierCode').val() == 0 && $('#chkDeliveryDate').val() == 0) {
            MsgBox("Please select criteria, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");
            return false;
        }

        if ($('#chkPDSNo').val() == 1 && (itmPDS.value === undefined || itmPDS.value === undefined)) {
            MsgBox("Please input PDS From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");
            return false;
        }

        if ($('#chkDeliveryDate').val() == 1 && (itmDelivery.value === undefined || itmDeliveryTo.value === undefined)) {
            MsgBox("Please select Delivery Date From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");
            return false;
        }


        if ($('#chkPrnitKanban').val() == 1) {
            MsgBox("Do you want to print with 1 KANBAN in full A4 ?", MsgBoxStyle.OkCancel, async function () {
                printKANBAN('KBNOR700KANBANA4');
            })
        } else {
            printKANBAN('KBNOR700KANBAN');
        }
    });



    printKANBAN = async function (pRPTNAME = 'KBNOR700KANBAN') {
        //console.log(pRPTNAME);
        //console.log($('#chkPDSNo').val());
        //console.log($('#itmPDSFrom').val());
        //console.log($('#itmPDSTo').val());
        //console.log(xDate.Date('yyyyMMdd', 'MM=-3'));

        let isDeleted = await DeleteKBNOR_140_KB();
        console.log(isDeleted);
        if (isDeleted !== true) {
            return xSwal.error("Can't Delete Previous Data");
        }

        var _dtKanban = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNOR700_KANBAN]",
                "@pUserCode": ajexHeader.UserCode,
                "@pPlant": ajexHeader.Plant,
                "@pDeliveryDate": xDate.Date('yyyyMMdd', 'MM=-3'),
                "@F_orderType": (rdoOrderType.value == 'Special' ? 'S' : (rdoOrderType.value == 'Urgent' ? 'U' : 'N')),
                "@F_OrderNo": ($('#chkPDSNo').val() == 1 ? itmPDS.value : ''),
                "@F_OrderNoTo": ($('#chkPDSNo').val() == 1 ? itmPDSTo.value : ''),
                "@F_Delivery_Date": ($('#chkDeliveryDate').val() == 1 ? ReplaceAll(itmDelivery.value, '-', '') : ''),
                "@F_Delivery_DateTo": ($('#chkDeliveryDate').val() == 1 ? ReplaceAll(itmDeliveryTo.value, '-', '') : '')
            },
        });

        console.log(_dtKanban);

        if (_dtKanban.rows == null) MsgBox("Order data not found.", MsgBoxStyle.Information, "Information");
        if (_dtKanban.rows != null) {

            xSplash.show();

            var _pds = '', _OrderType = '', _Plant = '', _DeliveryDate = '';
            for (var i = 0; i < _dtKanban.rows.length; i++) {

                var _BoxQty = _dtKanban.rows[i].F_Box_Qty;
                var _UnitAmount = _dtKanban.rows[i].F_Unit_Amount;

                var _page_total = Math.floor(_UnitAmount / _BoxQty);
                var _ceil = Math.ceil(_UnitAmount / _BoxQty);
                var _amt = _UnitAmount - (_page_total * _BoxQty);

                var _dt = await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR700_KANBANA4]",
                        "@pUserCode": ajexHeader.UserCode,
                        "@F_OrderNo": Trim(_dtKanban.rows[i].F_OrderNO),
                        "@F_PartNO": Trim(_dtKanban.rows[i].F_PartNO),
                        "@F_Kanban_No": Trim(_dtKanban.rows[i].F_Kanban_No),
                        "@F_Remark_KB": Trim(_dtKanban.rows[i].F_Remark_KB),
                        "@pMax": _ceil,
                        "@pBoxQty": (_amt > 0 ? _amt : '')
                    },
                });
                //console.log(i);
                var _pcent = (i / _dtKanban.rows.length) * 100;
                xItem.progress({ id: 'prgProcess', current: _pcent, label: 'Processing, Please wait : ' + i + '/' + _dtKanban.rows.length + ' ({{##.##}}) %' });

            }

            console.log(_dt);

            for (var i = 0; i < _dt.rows.length; i++) {
                _OrderType = Trim(_dt.rows[i].F_Order_Type);
                _OrderType = (_OrderType == 'N' ? 'NORMAL' : (_OrderType == 'S' ? 'SPECIAL' : (_OrderType == 'U' ? 'URGENT' : '')));

                _Plant = Trim(_dt.rows[i].F_Plant);
                _Plant = (_Plant == '1' ? 'SAMRONG PLANT' : (_Plant == '2' ? 'BANGPEE PLANT' : (_Plant == 'B' ? 'SAMRONG WAREHOUSE' : 'BANGPRAKONG PLANT')));

                _DeliveryDate = Trim(_dt.rows[i].F_Delivery_Date);
                _DeliveryDate = _DeliveryDate.substring(6, 8) + '/' + _DeliveryDate.substring(4, 6) + '/' + _DeliveryDate.substring(0, 4);

                _pds = _pds + '00' + Trim(_dt.rows[i].F_PartNO) + '0|';
                _pds = _pds + Trim(_dt.rows[i].F_Supplier_INT) + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Supplier_CD) + '|';
                _pds = _pds + _DeliveryDate + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Delivery_Trip) + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Delivery_Time) + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Delivery_Dock) + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Dock_Code) + '|';
                _pds = _pds + _Plant + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Kanban_No) + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Part_name) + '|';
                _pds = _pds + Trim(_dt.rows[i].F_OrderNO) + '|';
                _pds = _pds + _OrderType + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Box_Qty) + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Page) + '/' + Trim(_dt.rows[i].F_Page_Total) + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Unit_Amount) + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Remark) + '|';

                _pds = (_pds != '' ? _pds + ',@@,' : _pds + '');
                //console.log(i);
            }
            xItem.progress({ id: 'prgProcess', current: 100, label: 'Processing, Please wait : {{##.##}} %' });


            await xAjax.Post({
                url: 'KBNOR700/PDS_GENQRCODE',
                data: {
                    'PDSNO': _pds
                },
                then: function (result) {
                    //console.log(result);
                    if (_xLib.GetCookie('isDev') == "1") {
                        return _xLib.OpenReport("/KBNOR700KANBAN_X2", `pUserCode=${ajexHeader.UserCode}` +
                            `&OrderNo=${itmPDS.value}&OrderNoTo=${itmPDSTo.value}&DeliveryDate=${itmDelivery.value}&DeliveryDateTo=${itmDeliveryTo.value}`);
                    }
                    else {

                        window.open(
                            _REPORTINGSERVER_ + '%2fKB3%2f' + pRPTNAME + '&rs:Command=Render'
                            + '&pUserCode=' + ajexHeader.UserCode
                            + '&OrderNo=' + itmPDS.value
                            + '&OrderNoTo=' + itmPDSTo.value
                            + '&DeliveryDate=' + ($('#chkDeliveryDate').val() == 1 ? ReplaceAll(itmDelivery.value, '-', '') : '')
                            + '&DeliveryDateTo=' + ($('#chkDeliveryDate').val() == 1 ? ReplaceAll(itmDeliveryTo.value, '-', '') : '')
                            , '_blank'
                        );
                    }
                }
            });

            xSplash.hide();

        }
    }

})


xAjax.onChange('#rdoOrderTypeNormal', async function () {
    rdoOrderType.check = 'Normal';
    loadOrderType();
});
xAjax.onChange('#rdoOrderTypeSpecial', async function () {
    rdoOrderType.check = 'Special';
    loadOrderType();
});
xAjax.onChange('#rdoOrderTypeUrgent', async function () {
    rdoOrderType.check = 'Urgent';
    loadOrderType();
});

async function DeleteKBNOR_140_KB() {
    let isDel = false;
    await _xLib.AJAX_Post(`/xapi/DeleteKBNOR_140_KB`, '',
        function (success) {
            console.log(success);
            isDel = true;
        }
    );

    return isDel;
}
async function DeleteKBNOR_450() {
    let isDel = false;
    await _xLib.AJAX_Post(`/xapi/DeleteKBNOR_450`, '',
        function (success) {
            console.log(success);
            isDel = true;
        }
    );

    return isDel;
}