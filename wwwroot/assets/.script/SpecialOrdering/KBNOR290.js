$(document).ready(async () => {
    await _xDataTable.InitialDataTable("#tableMain",
        {
            columns: [
                { title: "Survey Doc. No.", data: "F_Supplier_CD" },
                { title: "Supplier CD.", data: "F_Name" },
                { title: "Part No.", data: "F_Survey_Doc" },
                { title: "Qty", data: "F_Survey_Doc" },
                { title: "Delivery Date", data: "F_Survey_Doc" },
                { title: "Status", data: "F_Survey_Doc" },
                { title: "Comment", data: "F_Survey_Doc" },
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