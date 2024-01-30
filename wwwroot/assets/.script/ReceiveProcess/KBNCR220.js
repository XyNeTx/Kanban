$(document).ready(function () {
    var pdsSet = new Set();

    const table = $('#tblMaster').DataTable({
        columns: [
            { data: "No" },
            { data: "F_OrderNo" },
            { data: "F_Supplier" },
            { data: "F_Delivery_Date" },
            { data: "F_Receive_Date" },
            { data: "F_PDS_Status" },
            { data: "F_Receive_Status" }
        ],
        order: [[0, 'asc']],
        paging: false,
        scrollY: "150px",
        scrollCollapse: true,
    });

    xAjax.Post({
        url: 'KBNCR220/Initial',
        then: function (result) {
            console.log(result);
            xSplash.hide();
        },
        error: function (result) {
            console.error("Initial Error from Receive Special Report");
            xSplash.hide();
        },
    });

    xAjax.onClick("#radio", function () {
        var data = $("#radio").prop("checked", true).val();
        xAjax.post({
            url: 'KBNCR220/Initial',
            data: data,
            then: function (result) {
                console.log(result);
                xSplash.hide();
            },
        })
    });

    xAjax.onClick("#SearchBtn", function () {
        xAjax.Post({
            url: 'KBNCR220/Check',
            data: {
                'JsonData': 'test',
            },
            then: function (result) {
                if (result.response == "OK") {
                    if (result.data != null) {
                        $('#tblMaster').dataTable().fnAddData(result.data);
                    }
                }
                else {
                    xSwal.error("Get Receive Special Report Error", "Error!!!");
                }
            },
            error: function (result) {
                console.error(_Controller + '.SearchPDSNo: ' + result.responseText);
                xSplash.hide();
            }
        });
    });
});