$(document).ready(async () => {
    await _xDataTable.InitialDataTable("#tableMain",
        {
            columns: [
                { title: "Survey Doc. No.", data: "F_Survey_Doc" },
                { title: "Supplier CD.", data: "F_Supplier" },
                { title: "Part No.", data: "F_Part_No" },
                { title: "Qty", data: "F_Qty" },
                { title: "Delivery Date", data: "F_Deli_DT" },
                { title: "Status", data: "F_Status_D" },
                { title: "Comment", data: "F_Remark_Delivery" },
            ],
            order: [[1, "asc"]],
            scrollCollapse: false,
        }
    );

    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");

    YMChange();

    xSplash.hide();
});

$("#mthSurIssDate").change(async () => {
    await YMChange();
});

$("#selCustOrder").change(async () => {
    await CustOrderChange();
});

$("#selSuppCD").change(async () => {
    await SuppCDChange();
});

YMChange = async () => {
    let getQuery = {
        YM: $("#mthSurIssDate").val().replace("-", "")
    }

    _xLib.AJAX_Get("/api/KBNOR290/ProdYMChanged", getQuery,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            // console.log(success);
            $("#selCustOrder").empty();
            $("#selCustOrder").append("<option value='' hidden></option>");
            $.each(success.data, function (index, value) {
                //console.log(value);
                $("#selCustOrder").append(new Option(value.f_PO_Customer, value.f_PO_Customer));
            });
            $("#selCustOrder").selectpicker("refresh");
        },
        async (error) => {
            console.error(error);
        }
    );
};

CustOrderChange = async () => {

    let getQuery = {
        PO: $("#selCustOrder").val(),
    }

    _xLib.AJAX_Get("/api/KBNOR290/GetSuppCD", getQuery,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#selSuppCD").empty();
            $("#selSuppCD").append("<option value='' hidden></option>");
            $.each(success.data, function (index, value) {
                //console.log(value);
                $("#selSuppCD").append(new Option(value.f_Supplier_CD, value.f_Supplier_CD));
            });
            $("#selSuppCD").selectpicker("refresh");
        },
        async (error) => {
            console.error(error);
        }
    );
};

SuppCDChange = async () => {
    let getQuery = {
        PO: $("#selCustOrder").val(),
        Supplier: $("#selSuppCD").val(),
    }

    _xLib.AJAX_Get("/api/KBNOR290/GetData", getQuery,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            await _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        },
        async (error) => {
            console.error(error);
        }
    );
};

$("#btnPrint").click(() => {
    let obj = {
        F_PO_Customer: $("#selCustOrder").val(),
        F_Supplier_CD: $("#selSuppCD").val().split("-")[0],
        F_Supplier_Plant: $("#selSuppCD").val().split("-")[1]
    }

    for (let key in obj) {
        if (obj[key] === "") {
            return xSwal.error("Error!!", 'Please select all fields.');
        }
    };

    _xLib.OpenReportObj("/KBNOR220_2_Rpt", obj);
});