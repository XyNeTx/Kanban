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
    });
    xSplash.hide();
});