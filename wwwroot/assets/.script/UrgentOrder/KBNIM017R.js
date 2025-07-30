$(document).ready(async function () {
    $(".btn-toolbar").remove();

    await _xDataTable.InitialDataTable("#tableImpData",
        {
            columns: [
                {
                    title: "No", data: "",width: "7%", render: function (data, type, row, meta) {
                        console.log("data:", data);
                        console.log("type:", type);
                        console.log("row:", row);
                        console.log("meta:", meta);
                        return meta.row + 1;
                    }
                },
                { title: "Part No", data: "Part No", width: "17%" },
                { title: "Sebango", data: "Sebango", width: "12%" },
                { title: "Part Name", data: "Part_Name", width: "24%" },
                { title: "Unit Price", data: "Unit_Price", width: "10%" },
                { title: "Delivery Qty", data: "Delivery_Qty.", width: "10%" },
                { title: "Pack", data: "Pack", width: "10%" },
                { title: "Lot Size", data: "Lot_Size", width: "10%" },
            ],
            order: [[0, "asc"]],
            scrollCollapse: true,
            scrollX: true,
            scrollY: true,
        }
    );

    xSplash.hide();
});