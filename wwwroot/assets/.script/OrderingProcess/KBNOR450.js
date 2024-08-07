$(document).ready(function () {

    const xKBNOR450 = new MasterTemplate({
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
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    xKBNOR450.prepare();

    xKBNOR450.initial(function (result) {
        //console.log(result);
        xDropDownList.bind('#frmCondition #itmPDSFrom', result.data.PDSNo, 'F_OrderNo', 'F_OrderNo');
        xDropDownList.bind('#frmCondition #itmPDSTo', result.data.PDSNo, 'F_OrderNo', 'F_OrderNo');
        xDropDownList.bind('#frmCondition #itmSupplierFrom', result.data.Supplier, 'F_Supplier_Code', 'F_Supplier_Plant');
        xDropDownList.bind('#frmCondition #itmSupplierTo', result.data.Supplier, 'F_Supplier_Code', 'F_Supplier_Plant');

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
    });



    xAjax.onClick('#btnPrintPDS', async function () {
        if ($('#chkPDSNo').val() == 1 && ($('#itmPDSFrom').val() == '' || $('#itmPDSTo').val() == ''))
            MsgBox("Please input PDS From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");

        var dateFrom = $('#itmDeliveryFrom').val();
        var dateTo = $('#itmDeliveryTo').val();
        var pdsFrom = $("#itmPDSFrom").val();
        var pdsTo = $("#itmPDSTo").val();
        
        await xAjax.Execute({
            data: {
                "Module": "[exec].[spKBNOR450_RPT_PDS]",
                "@OrderType": "U",
                "@Plant": ajexHeader.Plant,
                "@UserCode": ajexHeader.UserCode,
                "@itmPDSFrom": $('#itmPDSFrom').val(),
                "@itmPDSTo": $('#itmPDSTo').val(),
                "@itmDeliveryFrom": $('#itmDeliveryFrom').val(),
                "@itmDeliveryTo": $('#itmDeliveryTo').val()
            },
        });

        var pdsList = [];

        $("#itmPDSFrom option").each(function () {
            pdsList.push($(this).val());
        });
        console.log(pdsList);


        await pdsList.forEach(async function (each) {
            await xAjax.Post({
                url: 'KBNOR700/PDS_GENBARCODE',
                data: {
                    'PDSNO': each + "D"
                },
            });
        })

        xSwal.success('Success','Redirecting to View Report');
        console.log('spKBNOR450_RPT_PDS');
        window.open(`http://hmmta-tpcap/E-Report/Report.aspx?Register=PDS&PDSNoFrom=${$('#itmPDSFrom').val()}&PDSNoTo=${$('#itmPDSTo').val()}&DateFrom=${dateFrom}&DateTo=${dateTo}`)
        _xLib.OpenReport("/KBNOR700PDS", `pUserCode=${ajexHeader.UserCode}&OrderNo=${$('#itmPDSFrom').val()}&OrderNoTo=${$('#itmPDSTo').val()}&DeliveryDate=${dateFrom}&DeliveryDateTo=${dateTo}`)

    });



    xAjax.onClick('#btnPrintKanban', async function () {
        if ($('#chkPDSNo').val() == 1 && ($('#itmPDSFrom').val() == '' || $('#itmPDSTo').val() == ''))
            MsgBox("Please input PDS From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");

        var PDSFrom = $('#itmPDSFrom').val() ?? '';
        var PDSTo = $('#itmPDSTo').val() ?? '';
        var DeliveryFrom = $('#itmDeliveryFrom').val().replaceAll("-", "") ?? '';
        var DeliveryTo = $('#itmDeliveryTo').val().replaceAll("-", "") ?? '';

        var _dt = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNOR450_RPT_KANBAN]",
                "@OrderType": "U",
                "@Plant": ajexHeader.Plant,
                "@UserCode": ajexHeader.UserCode,
                "@itmPDSFrom": PDSFrom,
                "@itmPDSTo": PDSTo,
                "@itmDeliveryFrom": DeliveryFrom,
                "@itmDeliveryTo": DeliveryTo
            },
        });
        var _pds = '', _OrderType = '', _Plant = '', _DeliveryDate = '';
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
        await xAjax.Post({
            url: 'KBNOR700/PDS_GENQRCODE',
            data: {
                'PDSNO': _pds
            },
            then: function (result) {
                if (result.status == 200) {

                    xSwal.success('Success', 'Redirecting to View Report');

                    return _xLib.OpenReport("/KBNOR700KANBAN", `pUserCode=${ajexHeader.UserCode}` +
                        `&OrderNo=${PDSFrom}&OrderNoTo=${PDSTo}&DeliveryDate=${DeliveryFrom}&DeliveryDateTo=${DeliveryTo}`);

                    console.log('spKBNOR450_RPT_KANBAN');
                }
                return xSwal.error('Error', 'Error while generating report');
            }
        });
    });


    xAjax.onClick('btnExit', function () {
        xAjax.redirect('KBNOR400');
    });



})

