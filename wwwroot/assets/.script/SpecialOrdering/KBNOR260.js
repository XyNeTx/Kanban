$(document).ready(async () => {
    await _xDataTable.InitialDataTable("#tableMain",
        {
            columns: [
                {
                    title: "Flag", render: function (data, type, row, meta) {
                        return "<input type='checkbox' class='chkFlag' data-id='" + row.F_Survey_Doc + "' />";
                    }
                },
                { title: "PDS No", data: "F_Factory_Code" },
                { title: "Customer OrderNo", data: "F_Supplier_code" },
                { title: "Supplier Code", data: "F_Survey_Doc" },
                { title: "Delivery Date", data: "F_Delivery_Date" },
                { title: "Status", data: "F_PO_Customer" },
                { title: "Approve", data: "F_PO_Customer" },
            ],
            order: [[1, "asc"]],
            scrollCollapse: false,
        }
    );

    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");

    xSplash.hide();
});