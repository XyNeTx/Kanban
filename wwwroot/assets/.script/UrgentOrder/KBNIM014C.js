$(document).ready(async function () {

    const KBNIM014C = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Order No.', 'Order Issued Date', 'Delivery Date'],
            "TH": ['Order No.', 'Order Issued Date', 'Delivery Date'],
            "JP": ['Order No.', 'Order Issued Date', 'Delivery Date'],
        },
        ColumnValue: [
            { "data": "F_PDS_No" },
            { "data": "F_PDS_Issued_Date" },
            { "data": "F_Delivery_Date" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });

    await _xLib.AJAX_Get('/api/KBNIM014C/search', '', async function (response) {
        if (response.status === "200") {
            var _jsondata = await JSON.parse(response.data);
            await _xLib.TrimArrayJSON(_jsondata);
            return await _jsondata.forEach(function (item) {
                $("#SelectPDSNo").append('<option value="' + item.F_PDS_No + '">' + item.F_PDS_No + '</option>');
            });
        }
    },
    function (err) {
        return xSwal.error("Error !!", err.responseJSON.message);
    });

    await KBNIM014C.prepare();
    await KBNIM014C.initial(function (result) {

        xAjax.onCheck('#chkDeliveryDate', function () {
            if ($('#chkDeliveryDate').val() == 0) $('#fldDeliveryDate').prop('disabled', 'disabled');
            if ($('#chkDeliveryDate').val() == 1) $('#fldDeliveryDate').prop('disabled', false);
        });

    });

});

$("#buttonSearch").click(async function () {
    console.log("Clicked !!! !! !!")
    $.ajax({
        type: "GET",
        url: "/api/KBNIM014C/search",
        data: { F_PDS_NO: $("#SelectPDSNo").val() },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            //console.log("Success: ", response);
            if (response.status === "200") {
                var jsonResult = JSON.parse(response.data);
                //console.log("jsonResult: ", jsonResult);
                _xLib.TrimArrayJSON(jsonResult);
                $("#tblMaster").DataTable().clear().rows.add(jsonResult).draw();
            }
            else {
                return xSwal.error(response.title, response.message);
            }
        },
        error: function (xhr, status, response) {
            console.error("Error: ", xhr.responseJSON);
            return xSwal.error(xhr.responseJSON.title, xhr.responseJSON.message);
        }
    });
});