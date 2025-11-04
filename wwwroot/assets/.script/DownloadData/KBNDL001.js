
let itmDelivery = "";

let itmDeliveryTo = "";

$(document).ready(async function () {
    await xSplash.hide();

    const KBNDL001 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Plant', 'Parent Part', 'Ruibetsu', 'Effective Date', 'End Date'],
            "TH": ['Plant', 'Parent Part', 'Ruibetsu', 'Effective Date', 'End Date'],
            "JP": ['Plant', 'Parent Part', 'Ruibetsu', 'Effective Date', 'End Date'],
        },
        ColumnValue: [
            { "data": "F_Plant" },
            { "data": "F_Parent_Part" },
            { "data": "F_Part_Name" },
            { "data": "F_Effect_Date" },
            { "data": "F_End_Date" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ },
        ],
    });
    

    KBNDL001.prepare();
    KBNDL001.initial(function (result) {
        //console.log(result);

        $('#itmPDSFrom').empty();
        $('#itmPDSTo').empty();

        $("#itmPDSFrom").selectpicker('refresh');
        $("#itmPDSTo").selectpicker('refresh');

        xDropDownList.bind('#frmCondition #itmPDSFrom', result.data.PDSNo, 'F_OrderNo', 'F_OrderNo');
        xDropDownList.bind('#frmCondition #itmPDSTo', result.data.PDSNo, 'F_OrderNo', 'F_OrderNo');
        xDropDownList.bind('#frmCondition #itmSupplier', result.data.Supplier, 'SupplierCode', 'SupplierCode');
        xDropDownList.bind('#frmCondition #itmSupplierTo', result.data.Supplier, 'SupplierCode', 'SupplierCode');

        $('#itmSupplier').val('8888-A');
        $('#itmSupplierTo').val('8888-A');

        $("#itmPDSFrom").selectpicker('refresh');
        $("#itmPDSTo").selectpicker('refresh');

        xAjax.onClick('#chkPDSNo', function () {
            if ($('#chkPDSNo').val() == 0) $('#fldPDSNo').prop('disabled', 'disabled');
            if ($('#chkPDSNo').val() == 1) $('#fldPDSNo').prop('disabled', false);
        });

        xAjax.onClick('#chkSupplierCode', function () {
            if ($('#chkSupplierCode').val() == 0) $('#fldSupplierCode').prop('disabled', 'disabled');
            if ($('#chkSupplierCode').val() == 1) $('#fldSupplierCode').prop('disabled', false);
        });

        xAjax.onClick('#chkDeliveryDate', function () {
            if ($('#chkDeliveryDate').val() == 0) $('#fldDeliveryDate').prop('disabled', 'disabled');
            if ($('#chkDeliveryDate').val() == 1) $('#fldDeliveryDate').prop('disabled', false);
        });


        xSplash.hide();

        console.log(moment($("#itmDelivery").val(), "YYYY-MM-DD").format("YYYYMMDD"));
    });

    $("#itmDeliveryTo").on("change", function () {
        if (itmDeliveryTo === $(this).val()) {
            return;
        }

        itmDelivery = $("#itmDelivery").val();
        itmDeliveryTo = $("#itmDeliveryTo").val();

        _xLib.AJAX_Post("/KBNDL001/initial",
            {
                F_Delivery_Date: (moment(itmDelivery) == "Invalid date") ? "" : moment(itmDelivery, "YYYY-MM-DD").format("YYYYMMDD"),
                F_Delivery_DateTo: (moment(itmDeliveryTo) == "Invalid date") ? "" : moment(itmDeliveryTo, "YYYY-MM-DD").format("YYYYMMDD"),
            },
            function (success) {
                console.log(success);
                $("#itmPDSFrom").empty();
                $("#itmPDSTo").empty();
                $("#itmSupplier").empty();
                $("#itmSupplierTo").empty();
                $("#itmPDSFrom").append(`<option value="" hidden></option>`);
                $("#itmPDSTo").append(`<option value="" hidden></option>`);

                success.data.PDSNo.forEach(function (item) {
                    $("#itmPDSFrom").append(`<option value="${item.F_OrderNo}">${item.F_OrderNo}</option>`);
                    $("#itmPDSTo").append(`<option value="${item.F_OrderNo}">${item.F_OrderNo}</option>`);
                });

                $("#itmPDSFrom").selectpicker('refresh');
                $("#itmPDSTo").selectpicker('refresh');

                success.data.Supplier.forEach(function (item) {
                    $("#itmSupplier").append(`<option value="${item.SupplierCode}">${item.SupplierCode}</option>`);
                    $("#itmSupplierTo").append(`<option value="${item.SupplierCode}">${item.SupplierCode}</option>`);
                });

            },
            function (error) {
                console.log(error);
            }
        );
    });


    $("#itmDelivery").on("change", function () {
        if (itmDelivery === $(this).val()) {
            return;
        }

        itmDelivery = $("#itmDelivery").val();
        itmDeliveryTo = $("#itmDeliveryTo").val();

        let obj = {
            F_Delivery_Date: (moment(itmDelivery) == "Invalid date") ? "" : moment(itmDelivery, "YYYY-MM-DD").format("YYYYMMDD"),
            F_Delivery_DateTo: (moment(itmDeliveryTo) == "Invalid date") ? "" : moment(itmDeliveryTo, "YYYY-MM-DD").format("YYYYMMDD"),
        }
        let pData = JSON.stringify(JSON.stringify(obj));

        _xLib.AJAX_Post("/KBNDL001/initial", pData,
            function (success) {
                $("#itmPDSFrom").empty();
                $("#itmPDSTo").empty();
                $("#itmSupplier").empty();
                $("#itmSupplierTo").empty();
                $("#itmPDSFrom").append(`<option value="" hidden></option>`);
                $("#itmPDSTo").append(`<option value="" hidden></option>`);
                success.data.PDSNo.forEach(function (item) {
                    $("#itmPDSFrom").append(`<option value="${item.F_OrderNo}">${item.F_OrderNo}</option>`);
                    $("#itmPDSTo").append(`<option value="${item.F_OrderNo}">${item.F_OrderNo}</option>`);
                });
                $("#itmPDSFrom").selectpicker('refresh');
                $("#itmPDSTo").selectpicker('refresh');
                success.data.Supplier.forEach(function (item) {
                    $("#itmSupplier").append(`<option value="${item.SupplierCode}">${item.SupplierCode}</option>`);
                    $("#itmSupplierTo").append(`<option value="${item.SupplierCode}">${item.SupplierCode}</option>`);
                });
            },
            function (error) {
                console.log(error);
            }
        );
    });


    xAjax.onClick('#btnPlanning', async function () {
        MsgBox("Do you want to get planning data?", MsgBoxStyle.OkCancel, async function () {

            var _dt = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNDL001_PLANNING]"
                },
            });

            console.log(_dt);
        })
    });




    xAjax.onClick('#btnPrintPDS', async function () {
        //console.log($('#chkPDSNo').val());
        //console.log($('#itmPDSFrom').val());
        //console.log($('#itmPDSTo').val());
        if ($('#chkPDSNo').val() == 1 && ($('#itmPDSFrom').val() == null || $('#itmPDSTo').val() == null)) {
            MsgBox("Please input PDS From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");
            return false;
        }

        if ($('#chkSupplierCode').val() == 1 && ($('#itmSupplier').val() != '' || $('#itmSupplierTo').val() != '')) {
            MsgBox("Please input Supplier From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");
            return false;
        }

        //console.log($('#chkDeliveryDate').val());
        //console.log($('#itmDelivery').val());
        //console.log($('#itmDeliveryTo').val());
        if ($('#chkDeliveryDate').val() == 1 && ($('#itmDelivery').val() == '' || $('#itmDeliveryTo').val() == '')) {
            MsgBox("Please select Delivery Date From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");
            return false;
        }

        //console.log(($('#chkSupplierCode').val() == 1 ? $('#itmDelivery').val() : ''));
        //ajexHeader.Plant = '1';
        var _dt = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNDL001_PDS]",
                "@pUserCode": ajexHeader.UserCode,
                "@pPlant": '1',
                "@pDeliveryDate": xDate.Date('yyyyMMdd', 'MM=-3'),
                "@F_OrderNo": ($('#chkPDSNo').val() == 1 ? $('#itmPDSFrom').val() : ''),
                "@F_OrderNoTo": ($('#chkPDSNo').val() == 1 ? $('#itmPDSTo').val() : ''),
                "@F_Delivery_Date": ($('#chkDeliveryDate').val() == 1 ? ReplaceAll($('#itmDelivery').val(),'-','') : ''),
                "@F_Delivery_DateTo": ($('#chkDeliveryDate').val() == 1 ? ReplaceAll($('#itmDeliveryTo').val(), '-', '') : '')
            },
        });
        //console.log(($('#chkPDSNo').val() == 1 ? $('#itmPDSFrom').val() : ''));
        //console.log(($('#chkPDSNo').val() == 1 ? $('#itmPDSTo').val() : ''));
        //console.log(($('#chkDeliveryDate').val() == 1 ? ReplaceAll($('#itmDelivery').val(), '-', '') : ''));
        //console.log(($('#chkDeliveryDate').val() == 1 ? ReplaceAll($('#itmDeliveryTo').val(), '-', '') : ''));

        if (_dt.rows == null) MsgBox("Order data not found.", MsgBoxStyle.Information, "Information");
        if (_dt.rows != null) {
            var _pds = '';
            for (var i=0; i < _dt.rows.length; i++) {
                _pds = _pds + Trim(_dt.rows[i].F_Barcode) + ',';
            }

            //console.log(_pds);

            await xAjax.Post({
                url: 'KBNDL001/PDS_GENBARCODE',
                data: {
                    'PDSNO': _pds
                },
                then: function (result) {
                    console.log(result);

                    var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1) + 'PDS';
                    //var reportUrl = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx";

                    window.open(
                        _REPORTINGSERVER_ + '%2fKB3%2fKBNDL001PDS&rs:Command=Render'
                        + '&pUserCode=' + ajexHeader.UserCode
                        + '&OrderNo=' + ($('#chkPDSNo').val() == 1 ? $('#itmPDSFrom').val() : '')
                        + '&OrderNoTo=' + ($('#chkPDSNo').val() == 1 ? $('#itmPDSTo').val() : '')
                        + '&DeliveryDate=' + ($('#chkDeliveryDate').val() == 1 ? ReplaceAll($('#itmDelivery').val(), '-', '') : '')
                        + '&DeliveryDateTo=' + ($('#chkDeliveryDate').val() == 1 ? ReplaceAll($('#itmDeliveryTo').val(), '-', '') : '')
                        ,'_blank'
                    );

                }
            })

        }


    });



    xAjax.onClick('#btnPrintKanban', async function () {

        if ($('#chkPDSNo').val() == 1 && ($('#itmPDSFrom').val() == null || $('#itmPDSTo').val() == null)) {
            MsgBox("Please input PDS From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");
            return false;
        }

        if ($('#chkSupplierCode').val() == 1 && ($('#itmSupplier').val() != '' || $('#itmSupplierTo').val() != '')) {
            MsgBox("Please input Supplier From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");
            return false;
        }

        if ($('#chkDeliveryDate').val() == 1 && ($('#itmDelivery').val() == '' || $('#itmDeliveryTo').val() == '')) {
            MsgBox("Please select Delivery Date From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");
            return false;
        }


        if ($('#chkPrnitKanban').val() == 1) {
            MsgBox("Do you want to print with 1 KANBAN in full A4 ?", MsgBoxStyle.OkCancel, async function () {
                printKANBAN('KBNDL001KANBANA4');
            })
        } else {
            printKANBAN('KBNDL001KANBAN');
        }
    });



    printKANBAN = async function (pRPTNAME = 'KBNDL001KANBAN') {
        console.log(pRPTNAME);
        //console.log($('#chkPDSNo').val());
        //console.log($('#itmPDSFrom').val());
        //console.log($('#itmPDSTo').val());
        //console.log(xDate.Date('yyyyMMdd', 'MM=-3'));

        var _dtKanban = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNDL001_KANBAN]",
                "@pUserCode": ajexHeader.UserCode,
                "@pPlant": '1',
                "@pDeliveryDate": xDate.Date('yyyyMMdd', 'MM=-3'),
                "@F_OrderNo": ($('#chkPDSNo').val() == 1 ? $('#itmPDSFrom').val() : ''),
                "@F_OrderNoTo": ($('#chkPDSNo').val() == 1 ? $('#itmPDSTo').val() : ''),
                "@F_Delivery_Date": ($('#chkDeliveryDate').val() == 1 ? ReplaceAll($('#itmDelivery').val(), '-', '') : ''),
                "@F_Delivery_DateTo": ($('#chkDeliveryDate').val() == 1 ? ReplaceAll($('#itmDeliveryTo').val(), '-', '') : '')
            },
        });


        //console.log(_dtKanban);

        if (_dtKanban.rows == null) MsgBox("Order data not found.", MsgBoxStyle.Information, "Information");
        if (_dtKanban.rows != null) {


            var _pds = '', _OrderType = '', _Plant = '', _DeliveryDate = '';
            for (var i = 0; i < _dtKanban.rows.length; i++) {

                var _BoxQty = _dtKanban.rows[i].F_Box_Qty;
                var _UnitAmount = _dtKanban.rows[i].F_Unit_Amount;

                var _page_total = Math.floor(_UnitAmount / _BoxQty);
                var _ceil = Math.ceil(_UnitAmount / _BoxQty);
                var _amt = _UnitAmount - (_page_total * _BoxQty);
                //console.log('PAGE>> ' + _ceil + ' : TOTAL>> ' + _page_total);
                //console.log('Box>> ' + _amt + ' : ALL>> ' + _UnitAmount);

                var _dt = await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNDL001_KANBAN_PAGE]",
                        "@pUserCode": ajexHeader.UserCode,
                        "@F_OrderNo": Trim(_dtKanban.rows[i].F_OrderNO),
                        "@F_PartNO": Trim(_dtKanban.rows[i].F_PartNO),
                        "@F_Kanban_No": Trim(_dtKanban.rows[i].F_Kanban_No),
                        "@pMax": _ceil,
                        "@pBoxQty": (_amt > 0 ? _amt : '')
                    },
                });

            }

            //console.log(_dt);
            //return false;

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


                //console.log(_pds);

                _pds = (_pds != '' ? _pds + ',' : _pds + '');
            }

            //console.log(_pds);

            await xAjax.Post({
                url: 'KBNDL001/PDS_GENQRCODE',
                data: {
                    'PDSNO': _pds
                },
                then: function (result) {
                    console.log(result);

                    var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1) + 'PDS';
                    //var reportUrl = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx";

                    window.open(
                        _REPORTINGSERVER_ + '%2fKB3%2f' + pRPTNAME +'&rs:Command=Render'
                        + '&pUserCode=' + ajexHeader.UserCode
                        + '&OrderNo=' + $('#itmPDSFrom').val()
                        + '&OrderNoTo=' + $('#itmPDSTo').val()
                        + '&DeliveryDate=' + ($('#chkDeliveryDate').val() == 1 ? ReplaceAll($('#itmDelivery').val(), '-', '') : '')
                        + '&DeliveryDateTo=' + ($('#chkDeliveryDate').val() == 1 ? ReplaceAll($('#itmDeliveryTo').val(), '-', '') : '')
                        , '_blank'
                    );

                }
            })

        }
    }

})

