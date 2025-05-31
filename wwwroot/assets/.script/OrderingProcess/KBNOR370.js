$(document).ready(function () {

    loadOrderType = async function () {
        //console.log(rdoOrderType.value);

        var _dt = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNOR370_INI_PDS]",
                "pPlant": ajexHeader.Plant,
                "F_Issued_Date": moment($("#txtProcessDate").val(), "DD/MM/YYYY").add(-1, 'days').format("YYYYMMDD"),
                "F_Issued_Shift": $("#txtProcessShift").val().slice(0, 1) == "1" ? "D" : "N",
            },
        });
        if (_dt.rows == null) MsgBox("Order data not found.", MsgBoxStyle.Information, "Information");

        $('#itmPDS').empty();
        $('#itmPDSTo').empty();

        $('#itmPDS').append(`<option value="" hidden></option>`);
        $('#itmPDSTo').append(`<option value="" hidden></option>`);

        $("#itmPDS").selectpicker('refresh');
        $("#itmPDSTo").selectpicker('refresh');

        console.log(_dt);

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


    xAjax.onClick('btnExit', async function () {
        xAjax.redirect('KBNOR300');
    });



    xAjax.onClick('#btnPrintPDS', async function () {

        //console.log(($('#chkSupplierCode').val() == 1 ? $('#itmDelivery').val() : ''));
        //ajexHeader.Plant = '1';
        var _dt = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNOR700_PDS_CKD]",
                "@pUserCode": ajexHeader.UserCode,
                "@pPlant": ajexHeader.Plant,
                "@pDeliveryDate": xDate.Date('yyyyMMdd', 'MM=-3'),
                //"@F_orderType": (rdoOrderType.value == 'Special' ? 'S' : (rdoOrderType.value == 'Urgent' ? 'U' : 'N')),
                "@F_OrderNo": itmPDS.value,
                "@F_OrderNoTo": itmPDSTo.value,
                //"@F_Delivery_Date": ($('#chkDeliveryDate').val() == 1 ? ReplaceAll(itmDelivery.value, '-', '') : ''),
                //"@F_Delivery_DateTo": ($('#chkDeliveryDate').val() == 1 ? ReplaceAll(itmDeliveryTo.value, '-', '') : '')
            },
        });
        if (_dt.rows == null) MsgBox("Order data not found.", MsgBoxStyle.Information, "Information");
        if (_dt.rows != null) {
            var _pds = '';
            for (var i = 0; i < _dt.rows.length; i++) {
                _pds = _pds + Trim(_dt.rows[i].F_Barcode) + ',';
            }

            console.log(_pds);

            //window.open(`http://hmmta-tpcap/E-Report/Report.aspx?Register=REC&PDSNoFrom=${itmPDS.value}&PDSNoTo=${itmPDSTo.value}&DateFrom=${dateFrom}&DateTo=${dateTo}`);

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
                            + '&OrderNo=' + itmPDS.value
                            + '&OrderNoTo=' + itmPDSTo.value
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
                            + '&OrderNo=' + itmPDS.value
                            + '&OrderNoTo=' + itmPDSTo.value
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

        //if ($('#chkPDSNo').val() == 0 && $('#chkSupplierCode').val() == 0 && $('#chkDeliveryDate').val() == 0) {
        //    MsgBox("Please select criteria, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");
        //    return false;
        //}

        //if ($('#chkPDSNo').val() == 1 && (itmPDS.value === undefined || itmPDS.value === undefined)) {
        //    MsgBox("Please input PDS From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");
        //    return false;
        //}

        //if ($('#chkDeliveryDate').val() == 1 && (itmDelivery.value === undefined || itmDeliveryTo.value === undefined)) {
        //    MsgBox("Please select Delivery Date From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");
        //    return false;
        //}


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

        var _dtKanban = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNOR700_KANBAN]",
                "@pUserCode": ajexHeader.UserCode,
                "@pPlant": ajexHeader.Plant,
                "@pDeliveryDate": xDate.Date('yyyyMMdd', 'MM=-3'),
                //"@F_orderType": (rdoOrderType.value == 'Special' ? 'S' : (rdoOrderType.value == 'Urgent' ? 'U' : 'N')),
                "@F_OrderNo": itmPDS.value,
                "@F_OrderNoTo": itmPDSTo.value,
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
                            `&OrderNo=${itmPDS.value}&OrderNoTo=${itmPDSTo.value}
                            &DeliveryDate=${itmDelivery.value}
                            &DeliveryDateTo=${itmDeliveryTo.value}`
                            );
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

