$(document).ready(async () => {

    await _xDataTable.InitialDataTable("#tableMain",
        {
            columns: [
                {
                    title: "Flag", render: function (data, type, row, meta)
                    {
                        return "<input type='checkbox' class='chkSelect' checked/>";
                    },
                },
                { title: "Prod YM", data: "F_Prod_YM" },
                { title: "Supplier", data: "F_Supplier_CD" },
                { title: "Supplier Name", data: "F_Name" },
                { title: "Survey Document", data: "F_Survey_Doc" },
                { title: "Price Status", data: "F_Price_Status" },
            ],
            order: [[1, "asc"]],
            scrollCollapse: false,
        }
    );

    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");

    _xLib.AJAX_Get("/api/KBNOR230/LoadSurvey", null,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            success = await GetCheckFlag(success);
            await _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        },
        async (error) => {
            console.error(error);
        }
    );


    xSplash.hide();
});

$("#chkAll").click(() => {
    $(".chkSelect").prop("checked", true);
});

$("#unChkAll").click(() => {
    $(".chkSelect").prop("checked", false);
});

$("#btnUpload").click(async () => {
    await Upload();
});

GetCheckFlag = async (success) => {

    for (let i = 0; i < success.data.length; i++) {
        await _xLib.AJAX_Get("/api/KBNOR230/CheckPriceFlag", { SurveyDoc: success.data[i].F_Survey_Doc },
            async (chkSuccess) => {
                console.log(chkSuccess);
                if (chkSuccess.data == "0") {
                    success.data[i].F_Price_Status = "Price Zero";
                }
                else {
                    success.data[i].F_Price_Status = "";
                }
            },
            async (error) => {
                console.error(error);
            }
        );
    }

    return success;
};

Upload = async () => {
    let data = _xDataTable.GetSelectedDataDT("#tableMain");
    if (data.length == 0) {
        return xSwal.error("Error!!", "Please Select Data to Upload.");
    }

    xSplash.show();

    await _xLib.AJAX_Post("/api/KBNOR230/Upload", data,
        async (success) => {
            xSplash.hide();
            xSwal.success(success.response, success.message);
        },
        async (error) => {
            xSplash.hide();
            console.error(error);
        }
    );
};