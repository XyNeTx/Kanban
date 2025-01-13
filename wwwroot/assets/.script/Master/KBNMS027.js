$(document).ready(function () {


    _xDataTable.InitialDataTable("#tableMain",
        {
            "processing": false,
            "serverSide": false,
            width: '100%',
            paging: false,
            sorting: false,
            searching: false,
            scrollX: false,
            scrollY: "200px",
            scrollCollapse: true,
            "columns": [
                {
                    title: "Flag", render(data, type, row) {
                        return `<input type="checkbox" class="chkbox" id="chkbox" name="chkbox" value="${row.f_Supplier_Plant}">`;
                    }
                },
                {
                    title: "SupplierLogistic", data: "F_Logistic"
                },
                {
                    title: "SupplierOrder", data: "F_Start_Date"
                },
                {
                    title: "SupplierCode", data: "F_End_Date"
                },
                {
                    title: "SupplierName", data: "F_Truck_Type"
                },
            ],
            select: false,
            order: [[0, "asc"]]
        });

    $("table tbody tr td").addClass("text-center");
    $("table thead tr th").addClass("text-center");


    xSplash.hide();


});

