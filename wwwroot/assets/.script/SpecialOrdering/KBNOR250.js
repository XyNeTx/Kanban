$(document).ready(async () => {
    await _xDataTable.InitialDataTable("#tableMain",
        {
            columns: [
                {
                    title: "Flag", render: function (data, type, row, meta) {
                        return "<input type='checkbox' class='chkFlag' data-id='" + row.F_Survey_Doc + "' />";
                    }
                },
                {
                    title: "No.", render: function (data, type, row, meta) {
                        return meta.row + 1;
                    }
                },
                { title: "Plant", data: "F_Factory_Code" },
                { title: "Supplier Code", data: "F_Supplier_code" },
                { title: "Survey Document", data: "F_Survey_Doc" },
                { title: "Delivery Date", data: "F_Delivery_Date" },
                { title: "PDS No./Customer PO.", data: "F_PO_Customer" },
                {
                    title: "Status", render: function (data, type, row, meta) {
                        return "";
                    }
                },
            ],
            order: [[1, "asc"]],
            scrollCollapse: false,
        }
    );

    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");

    await GetSurveyNoPDS();

});

$("#mthDeliYM").change(async () => {
    await GetSurveyNoPDS()
});

$("#btnRefresh").click(async () => {
    await Refresh();
});

$("#btnUnlock").click(async () => {
    let isConfrim = await xSwal.confirm("This action can not undone !", "Are you sure to unlock the selected rows?")
    if(!isConfrim) return;
    await Unlock();
});

$("#btnGenerate").click(async () => {
    let isConfrim = await xSwal.confirm("This action can not undone !", "Are you sure to generate the selected rows?")
    if (!isConfrim) return;
    await Generate();
});

GetSurveyNoPDS = async () => {
    let getQuery = {
        DeliYM: $("#mthDeliYM").val().replace("-", ""),
        Fac: _xLib.GetCookie("plantCode"),
    }
    _xLib.AJAX_Get("/api/KBNOR250/GetSurveyNoPDS", getQuery,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            await _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
            $.each(success.data, async function (index, value) {
                await CheckPriceAndPackageFlag(value.F_Survey_Doc);
            });
            xSplash.hide();
        },
        (error) => {
            console.log(error);
        }
    );
};

CheckPriceAndPackageFlag = async (surveyDoc) => {
    let getQuery = {
        SurveyDoc: surveyDoc
    }
    _xLib.AJAX_Get("/api/KBNOR250/CheckPriceAndPackageFlag", getQuery,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            if (success.data.length > 0) {
                let status = success.data[0].F_Status
                if (status.toLowerCase() == "package not found") {
                    $("#tableMain tbody tr").find("td:eq(4) == " + surveyDoc).text(status);

                    if (status.toLowerCase() == "price zero" || status.toLowerCase() == "package not found"
                        || status.toLowerCase() == "price zero & package not found") {
                        $("#tableMain tbody tr").find("td:eq(0) == " + surveyDoc).find("input").prop("checked", true);
                    }
                }
            }
        },
        (error) => {
            console.log(error);
        }
    );
}

Refresh = async () => {
    _xLib.AJAX_Get("/api/KBNOR250/Refresh", null,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            xSplash.hide();
            xSwal.success(success.response, success.message);

            await GetSurveyNoPDS();
        }
    );
}

Unlock = async () => {
    let listObj = [];

    $("input.chkFlag:checked").each(function () {
        let row = $(this).closest("tr");
        let obj = $("#tableMain").DataTable().row(row).data();
        listObj.push(obj);
    });

    if (listObj.length == 0) {
        xSwal.error("Please select at least one row.");
        return;
    }

    let postQuery = {
        ListObj: listObj
    }

    _xLib.AJAX_Post("/api/KBNOR250/Unlock", postQuery,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            xSplash.hide();
            xSwal.success(success.response, success.message);

            await $("#mthDeliYM").trigger("change");
        }
    );
}

Generate = async () => {
    let postQuery = [];

    $("input.chkFlag:checked").each(function () {
        let row = $(this).closest("tr");
        let obj = $("#tableMain").DataTable().row(row).data();
        postQuery.push(obj);
    });

    if (postQuery.length == 0) {
        xSwal.error("Please select at least one row.");
        return;
    }

    _xLib.AJAX_Post("/api/KBNOR250/Generate?DeliYM=" + $("#mthDeliYM").val().replace("-", ""), postQuery,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            xSplash.hide();
            xSwal.success(success.response, success.message);

            await $("#mthDeliYM").trigger("change");
        }
    );
}