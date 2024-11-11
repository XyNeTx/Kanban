$(document).ready(async () => {
    await _xDataTable.InitialDataTable("#tableMain",
        {
            columns: [
                {
                    title: "No.", render: function (data, type, row, meta) {
                        return meta.row + 1;
                    }
                },
                { title: "PDS No.", data: "F_OrderNo" },
                {
                    title: "Part No.", render: function (data, type, row, meta) {
                        return row.F_Part_No + "-" + row.F_Ruibetsu;
                    }
                },
                { title: "Part Name", data: "F_Part_Name" },
                { title: "Kanban No.", data: "F_Kanban_No" },
                { title: "Qty", data: "F_Unit_Amount" },
                { title: "Supplier", data: "F_Supplier_Code" },
                { title: "Supplier Name", data: "F_Short_Name" },
            ],
            order: [[1, "asc"]],
            scrollCollapse: false,
        }
    );

    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");

    await GetCustomerPO();

    xSplash.hide();
});

$("#selCustOrder").change(async () => {
    await GetDetail();
});

$("#mthDeliYM").change(async () => {
    await GetCustomerPO();
});

$("#btnReport").click(async () => {
    await GenReportExcel();
});

GetCustomerPO = () => {
    let getQuery = {
        DeliYM: $("#mthDeliYM").val().replace("-", ""),
    }

    _xLib.AJAX_Get("/api/KBNOR297/GetCustomerPO", getQuery,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#selCustOrder").empty();
            $("#selCustOrder").append("<option value='' hidden></option>");
            $.each(success.data, function (index, value) {
                $("#selCustOrder").append("<option value='" + value.F_PO_customer + "'>" + value.F_PO_customer + "</option>");
            });
            $("#selCustOrder").selectpicker("refresh");
        },
    );
};

GetDetail = () => {
    let getQuery = {
        CustOrderNo: $("#selCustOrder").val(),
    }

    _xLib.AJAX_Get("/api/KBNOR297/GetAllCustomerPODetail", getQuery,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success)
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        }
    );
}

GenReportExcel = () => {
    let getQuery = {
        CustOrderNo: $("#selCustOrder").val(),
    }

    _xLib.AJAX_Get("/api/KBNOR297/GenReportExcel", getQuery,
        async (success) => {
            //success = _xLib.JSONparseMixData(success);
            console.log(success)
            let url = success.data;
            let a = document.createElement("a");
            a.href = url;
            a.download = getQuery.CustOrderNo + "_Report.xlsx";
            a.click();
            console.log(a);
        }
    );

}