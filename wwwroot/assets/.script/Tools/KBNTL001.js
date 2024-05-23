$(document).ready(function () {
    var _i18n = '@ViewData["UserLanguage"].ToString()';


    var tblReceiveSpecial9Y, tblReceiveDate, tblReceivePrice;
    initial = function () {

        if (getCookie('ProcessYM') != '') $('#txtProcessYM').val(getCookie('ProcessYM'));

        tblReceiveSpecial9Y = xDataTable.Initial({
            name: 'tblReceiveSpecial9Y',
            running: 0,
            toolbar: "",
            columnTitle: {
                "EN": ['Order Number', 'Delivery Date', 'Issue Date', 'To'],
                "TH": ['Order Number', 'Delivery Date', 'Issue Date', 'To'],
                "JP": ['Order Number', 'Delivery Date', 'Issue Date', 'To'],
            },
            column:
                [
                    { "data": "F_OrderNo" },
                    { "data": "F_Delivery_Date" },
                    { "data": "F_ISSUED_DATE" },
                    { "data": "NewDate" }
                ],
            addnew: false,
            then: function (config) {
                xSplash.hide();
            }
        });

        tblReceiveDate = xDataTable.Initial({
            name: 'tblReceiveDate',
            running: 0,
            toolbar: "",
            columnTitle: {
                "EN": ['Part/Item', 'Name', 'Box Qty.', 'Unit Price', 'Unit Amt.', 'Receive Amt.', 'Receive At'],
                "TH": ['Part/Item', 'Name', 'Box Qty.', 'Unit Price', 'Unit Amt.', 'Receive Amt.', 'Receive At'],
                "JP": ['Part/Item', 'Name', 'Box Qty.', 'Unit Price', 'Unit Amt.', 'Receive Amt.', 'Receive At'],
            },
            column:
                [
                    { "data": "F_Part_No" },
                    { "data": "F_Part_Name" },
                    { "data": "F_Box_Qty" },
                    { "data": "F_Unit_price" },
                    { "data": "F_Unit_Amount" },
                    { "data": "F_Receive_amount" },
                    { "data": "F_Receive_Date" }
                ],
            addnew: false,
            then: function (config) {
                xSplash.hide();
            }
        });

        tblReceiveDatePCW = xDataTable.Initial({
            name: 'tblReceiveDatePCW',
            running: 0,
            toolbar: "",
            columnTitle: {
                "EN": ['Supplier Code', 'Supplier Plant', 'Receive Date'],
                "TH": ['Supplier Code', 'Supplier Plant', 'Receive Date'],
                "JP": ['Supplier Code', 'Supplier Plant', 'Receive Date'],
            },
            column:
                [
                    { "data": "F_sup_cd" },
                    { "data": "F_sup_plant" },
                    { "data": "F_rec_date" }
                ],
            addnew: false,
            then: function (config) {
                xSplash.hide();
            }
        });

        tblReceivePrice = xDataTable.Initial({
            name: 'tblReceivePrice',
            running: 0,
            toolbar: "",
            columnTitle: {
                "EN": ['Part/Item', 'Amt.(Proc Web)', 'Amt.(T_PDS692_Detail)', 'Prc.(Proc Web)', 'Prc.(T_PDS692_Detail)'],
                "TH": ['Part/Item', 'Amt.(Proc Web)', 'Amt.(T_PDS692_Detail)', 'Prc.(Proc Web)', 'Prc.(T_PDS692_Detail)'],
                "JP": ['Part/Item', 'Amt.(Proc Web)', 'Amt.(T_PDS692_Detail)', 'Prc.(Proc Web)', 'Prc.(T_PDS692_Detail)'],
            },
            column:
                [
                    { "data": "F_prt_no" },
                    { "data": "F_amount_proc" },
                    { "data": "F_amount_PDS" },
                    { "data": "F_unit_price_proc" },
                    { "data": "F_unit_price_PDS" }
                ],
            addnew: false,
            then: function (config) {
                xSplash.hide();
            }
        });

    }
    initial();


    onSearch = function () {
        xAjax.Post({
            url: 'KBNTL001/Search',
            data: {
                "pdsno": $('#txtPDSNo').val()
            },
            then: function (result) {
                console.log(result);
                if (result.response == 'OK') {

                    $('#tblReceiveSpecial9Y').dataTable().fnClearTable();
                    if (result.data.ReceiveSpecial9Y.length > 0) $('#tblReceiveSpecial9Y').dataTable().fnAddData(result.data.ReceiveSpecial9Y);

                    $('#tblReceiveDate').dataTable().fnClearTable();
                    if (result.data.ReceiveDate.length > 0) $('#tblReceiveDate').dataTable().fnAddData(result.data.ReceiveDate);

                    $('#tblReceiveDatePCW').dataTable().fnClearTable();
                    if (result.data.ReceiveDatePCW.length > 0) $('#tblReceiveDatePCW').dataTable().fnAddData(result.data.ReceiveDatePCW);

                    $('#tblReceivePrice').dataTable().fnClearTable();
                    if (result.data.ReceivePrice.length > 0) $('#tblReceivePrice').dataTable().fnAddData(result.data.ReceivePrice);
                }
            },
            error: function () {
                xSwal.Error('Error', result.message);
                console.log('error handling here');
            }
        });
    }



    xAjax.onClick('#btnSpecial9Y', function () {

        if ($('#txtPDSNo').val() == '') {
            xSwal.error({ "message": 'กรุณาใส่ Kanban number!' });
            return;
        }

        xSwal.question({
            "message": 'ต้องการปลด Receive Special 9Y หรือไม่?',
            "then": function () {

                xAjax.Post({
                    url: 'KBNTL001/execSpecial9Y',
                    data: {
                        "pdsno": $('#txtPDSNo').val()
                    },
                    then: function (result) {
                        //console.log(result);
                        if (result.response == 'OK') {
                            onSearch();

                            xSwal.Success('Success', result.message);
                        }
                    },
                    error: function () {
                        xSwal.Error('Error', result.message);
                        console.log('error handling here');
                    }
                });

            }
        })

    })


    xAjax.onClick('#btnReceiveDate', function () {

        if ($('#txtPDSNo').val() == '') {
            xSwal.error({ "message": 'กรุณาใส่ Kanban number!' });
            return;
        }

        xSwal.Input('กรุณาใส่วันที่ต้องการเปลี่ยน (yyyy-mm-dd)', xDate.Date(), function (result) {

            if (result.isConfirmed) {
                if (result.value == '') {
                    xSwal.error('error', 'วันที่ไม่ถูกต้อง');
                    return false;
                }

                xAjax.Post({
                    url: 'KBNTL001/execReceiveDate',
                    data: {
                        "pdsno": $('#txtPDSNo').val(),
                        "receivedate": result.value
                    },
                    then: function (result) {
                        if (result.response == 'OK') {
                            onSearch();

                            xSwal.Success('Success', result.message);
                        }
                    },
                    error: function () {
                        xSwal.Error('Error', result.message);
                        console.log('error handling here');
                    }
                });

            }
        });
    });



    xAjax.onClick('#execReceivePriceAmt', function () {

        if ($('#txtPDSNo').val() == '') {
            xSwal.error({ "message": 'กรุณาใส่ Kanban number!' });
            return;
        }

        xSwal.Question('Question', 'ต้องการปรับจำนวนหรือไม่?', function (result) {

            if (result.isConfirmed) {

                xAjax.Post({
                    url: 'KBNTL001/execReceivePriceAmt',
                    data: {
                        "pdsno": $('#txtPDSNo').val()
                    },
                    then: function (result) {
                        if (result.response == 'OK') {
                            onSearch();

                            xSwal.Success('Success', result.message);
                        }
                    },
                    error: function () {
                        xSwal.Error('Error', result.message);
                        console.log('error handling here');
                    }
                });

            }
        });
    });


    xAjax.onClick('#execReceivePricePrc', function () {

        if ($('#txtPDSNo').val() == '') {
            xSwal.error({ "message": 'กรุณาใส่ Kanban number!' });
            return;
        }

        xSwal.Question('Question', 'ต้องการปรับราคาหรือไม่?', function (result) {

            if (result.isConfirmed) {

                xAjax.Post({
                    url: 'KBNTL001/execReceivePricePrc',
                    data: {
                        "pdsno": $('#txtPDSNo').val()
                    },
                    then: function (result) {
                        if (result.response == 'OK') {
                            onSearch();

                            xSwal.Success('Success', result.message);
                        }
                    },
                    error: function () {
                        xSwal.Error('Error', result.message);
                        console.log('error handling here');
                    }
                });

            }
        });
    });


});