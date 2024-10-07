$(document).ready(async function () {

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


    await Search();
    xSplash.hide();
});

async function Search() {
    await _xLib.AJAX_Get('/api/KBNIM003C/Search', '',
        async function (success) {
            if (success.status === "200") {
                var _arrJson = JSON.parse(success.data);
                _xLib.TrimArrayJSON(_arrJson);
                console.log(_arrJson);
                await $("#tblMaster").DataTable().clear().rows.add(_arrJson).draw();
                await $("#tblMaster tr td").find('input[type="checkbox"]').prop("checked", true);
                xSplash.hide();
            }
        },
        async function (errror) {
            await xSplash.hide();
            xSwal.error("Error !!", errror.responseJSON.message);
        },
    );
}

$("#buttonConfirm").click(function () {
    var _arrData = $("#tblMaster").DataTable().rows().data().toArray();
    xSplash.show();
    _xLib.AJAX_Post('/api/KBNIM003C/Confirm', JSON.stringify(_arrData),
        async function (success) {
            if (success.status === "200") {
                $("#tblMaster").DataTable().clear().draw();
                xSwal.success("Success !!", success.message);
                //await Search();
            }
        },
        async function (error) {
            xSwal.error("Error !!", error.responseJSON.message);
        },
    );
    xSplash.hide();
});