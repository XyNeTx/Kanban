$(document).ready(async () => {

    await _xDataTable.InitialDataTable("#tableMain",
        {
            columns: [
                {
                    title: "Flag", render: function (data, type, row, meta) {
                        return "<input type='checkbox' class='chkSelect' checked/>";
                    },
                },
                { title: "Supplier", data: "F_Supplier_CD" },
                { title: "Supplier Name", data: "F_Name" },
                { title: "Survey Document", data: "F_Survey_Doc" },
            ],
            order: [[1, "asc"]],
            scrollCollapse: false,
        }
    );

    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");

    await LoadSurvey();

    xSplash.hide();
});

$("#chkAll").click(() => {
    $(".chkSelect").prop("checked", true);
});

$("#unChkAll").click(() => {
    $(".chkSelect").prop("checked", false);
});

$("#btnGetStatus").click(async () => {
    let data = _xDataTable.GetSelectedDataDT("#tableMain");
    if (data.length === 0) {
        return xSwal.error("Please select at least one row.");
    }

    xSplash.show();

    _xLib.AJAX_Post("/api/KBNOR240/DownloadClicked", data,
        async (success) => {
            xSplash.hide();
            xSwal.success(success.response, success.message);
            await LoadSurvey();
        },
        async (error) => {
            xSplash.hide();
            console.error(error);
        }
    );
});

LoadSurvey = async () => {
    _xLib.AJAX_Get("/api/KBNOR240/LoadSurvey", null,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //success = await GetCheckFlag(success);
            await _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        },
        async (error) => {
            console.error(error);
        }
    );
}
