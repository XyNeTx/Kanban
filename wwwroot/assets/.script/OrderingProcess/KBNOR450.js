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

        xSwal.success('Success','Redirecting to View Report');
        console.log('spKBNOR450_RPT_PDS');


    });



    xAjax.onClick('#btnPrintKanban', async function () {
        if ($('#chkPDSNo').val() == 1 && ($('#itmPDSFrom').val() == '' || $('#itmPDSTo').val() == ''))
            MsgBox("Please input PDS From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");

        var PDSFrom = $('#itmPDSFrom').val() ?? '';
        var PDSTo = $('#itmPDSTo').val() ?? '';
        var DeliveryFrom = $('#itmDeliveryFrom').val().replaceAll("-", "") ?? '';
        var DeliveryTo = $('#itmDeliveryTo').val().replaceAll("-", "") ?? '';

        var result = await xAjax.Execute({
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

        if (result.status == 200) {

            xSwal.success('Success', 'Redirecting to View Report');

            return _xLib.OpenReport("/KBNOR700KANBAN", `pUserCode=${ajexHeader.UserCode}` +
            `&OrderNo=${PDSFrom}&OrderNoTo=${PDSTo}&DeliveryDate=${DeliveryFrom}&DeliveryDateTo=${DeliveryTo}`);

            console.log('spKBNOR450_RPT_KANBAN');
        }
        return xSwal.error('Error', 'Error while generating report');
    });


    xAjax.onClick('btnExit', function () {
        xAjax.redirect('KBNOR400');
    });



})

