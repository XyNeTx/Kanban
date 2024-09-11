
var _CookieProcessDate = _xLib.GetCookie("processDate");
var _CookieLoginDate = _xLib.GetCookie("loginDate");
$(document).ready(function () {


    $("#txtProcessDate").val(moment(_CookieProcessDate.substring(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));
    var shift = _CookieProcessDate.substring(10, 11) == "D" ? "1 - Day Shift" : "2 - Night Shift";
    $("#txtProcessShift").val(shift);

    const KBNOR140 = new MasterTemplate({
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

    KBNOR140.prepare();

    KBNOR140.initial(function (result) {
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
        if ($('#chkPDSNo').val() == 1 && ($('#itmPDSFrom').val() != '' || $('#itmPDSTo').val() != ''))
            MsgBox("Please input PDS From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");

        if ($('#chkSupplierCode').val() == 1 && ($('#itmSupplierFrom').val() != '' || $('#itmSupplierTo').val() != ''))
            MsgBox("Please input Supplier From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");



        await xAjax.Execute({
            data: {
                "Module": "[exec].[spKBNOR140_RPT_PDS]",
                "@OrderType": "N",
                "@Plant": ajexHeader.Plant,
                "@UserCode": ajexHeader.UserCode,
                "@itmPDSFrom": $('#itmPDSFrom').val(),
                "@itmPDSTo": $('#itmPDSTo').val(),
                "@itmSupplierFrom": $('#itmSupplierFrom').val(),
                "@itmSupplierTo": $('#itmSupplierTo').val(),
                "@itmDeliveryFrom": $('#itmDeliveryFrom').val(),
                "@itmDeliveryTo": $('#itmDeliveryTo').val()
            },
        });


        console.log('spKBNOR140_RPT_PDS');


    });



    xAjax.onClick('#btnPrintKanban', async function () {
        if ($('#chkPDSNo').val() == 1 && ($('#itmPDSFrom').val() != '' || $('#itmPDSTo').val() != ''))
            MsgBox("Please input PDS From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");

        if ($('#chkSupplierCode').val() == 1 && ($('#itmSupplierFrom').val() != '' || $('#itmSupplierTo').val() != ''))
            MsgBox("Please input Supplier From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");



        await xAjax.Execute({
            data: {
                "Module": "[exec].[spKBNOR140_RPT_KANBAN]",
                "@OrderType": "N",
                "@Plant": ajexHeader.Plant,
                "@UserCode": ajexHeader.UserCode,
                "@itmPDSFrom": $('#itmPDSFrom').val(),
                "@itmPDSTo": $('#itmPDSTo').val(),
                "@itmSupplierFrom": $('#itmSupplierFrom').val(),
                "@itmSupplierTo": $('#itmSupplierTo').val(),
                "@itmDeliveryFrom": $('#itmDeliveryFrom').val(),
                "@itmDeliveryTo": $('#itmDeliveryTo').val()
            },
        });


        console.log('spKBNOR140_RPT_KANBAN');
    });


    xAjax.onClick('btnExit', function () {
        xAjax.redirect('KBNOR100');
    });



})

