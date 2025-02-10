$(document).ready(async function () {

    _xDataTable.InitialDataTable("#tableMain",
        {
            "processing": false,
            "serverSide": false,
            width: '100%',
            paging: false,
            sorting: false,
            searching: false,
            scrollX: true,
            scrollY: "200px",
            scrollCollapse: false,
            "columns": [
                {
                    title: "Flag", render(data, type, row) {
                        return `<input type="checkbox" class="chkbox" id="chkbox" name="chkbox">`;
                    }
                },
                {
                    title: "DockCD", data: "f_Dock_Cd"
                },
                {
                    title: "LP1", data: "f_short_Logistic1", width:'50%'
                },
                {
                    title: "Remark1", data: "f_Remark1"
                },
                {
                    title: "LP2", data: "f_short_Logistic2"
                },
                {
                    title: "Remark2", data: "f_Remark2"
                },
                {
                    title: "LP3", data: "f_short_Logistic3"
                },
                {
                    title: "Remark3", data: "f_Remark3"
                },
                {
                    title: "LP4", data: "f_short_Logistic4"
                },
                {
                    title: "Remark4", data: "f_Remark4"
                },
                {
                    title: "LP5", data: "f_short_Logistic5"
                },
                {
                    title: "Remark5", data: "f_Remark5"
                },
                {
                    title: "LP6", data: "f_short_Logistic6"
                },
                {
                    title: "Remark6", data: "f_Remark6"
                },
                {
                    title: "LP7", data: "f_short_Logistic7"
                },
                {
                    title: "Remark7", data: "f_Remark7"
                },
                {
                    title: "LP8", data: "f_short_Logistic8"
                },
                {
                    title: "Remark8", data: "f_Remark8"
                },
                {
                    title: "LP9", data: "f_short_Logistic9"
                },
                {
                    title: "Remark9", data: "f_Remark9"
                },
                {
                    title: "LP10", data: "f_short_Logistic10"
                },
                {
                    title: "Remark10", data: "f_Remark10"
                },
                {
                    title: "LP11", data: "f_short_Logistic11"
                },
                {
                    title: "Remark11", data: "f_Remark11"
                },
                {
                    title: "LP12", data: "f_short_Logistic12"
                },
                {
                    title: "Remark12", data: "f_Remark12"
                },
                {
                    title: "LP13", data: "f_short_Logistic13"
                },
                {
                    title: "Remark13", data: "f_Remark13"
                },
                {
                    title: "LP14", data: "f_short_Logistic14"
                },
                {
                    title: "Remark14", data: "f_Remark14"
                },
                {
                    title: "LP15", data: "f_short_Logistic15"
                },
                {
                    title: "Remark15", data: "f_Remark15"
                },
                {
                    title: "LP16", data: "f_short_Logistic16"
                },
                {
                    title: "Remark16", data: "f_Remark16"
                },
            ],
            select: false,
            order: [[0, "asc"]]
        });

    await GetShortLogistic();
    await GetDockCode();
    await GetListData();

    xSplash.hide();
});

$("#divBtn").on("click", "button", async function () {
    GetListData();
    $("#divBtn").find("button").prop("disabled", true);
    $(this).prop("disabled", false);
});

$("#btnCan").click(async function () {

    $("#divBtn").find("button").prop("disabled", false);

    $("#formMain").trigger("reset");
    $().resetAllSelectPicker();

});

$(document).on("dblclick","table tbody tr td", async function () {

    var row = $(this).closest("tr");
    var obj = $("#tableMain").DataTable().row(row).data();

    $(".selected").removeClass("selected");

    $(this).closest("tr").toggleClass("selected");

    //console.log(obj);

    Object.keys(obj).forEach(async function (e) {
        let newE = "F" + e.substring(1, e.length);
        //console.log(e);
        //console.log(newE);
        //console.log(obj[e]);
        $(`#${newE}`).val(obj[e]);
        if ($(`#${newE}`).prop("tagName") === "SELECT") {
            $(`#${newE}`).selectpicker("refresh");
        }
    });

});

$("#btnSave").click(function () {
    Save();
});

$("#F_Dock_Cd").change(function () {
    GetListData();
});

function GetDockCode() {
    return _xLib.AJAX_Get("/api/KBNMS028/GetDockCode", null,
        function (success) {
            _xLib.TrimArrayJSON(success.data);
            $("#F_Dock_Cd").addListSelectPicker(success.data, "f_Dock_Cd");
        },
        function (error) {
            xSwal.xError(error);
        }
    );
}
function GetShortLogistic() {
    return _xLib.AJAX_Get("/api/KBNMS028/GetShortLogistic", null,
        function (success) {
            _xLib.TrimArrayJSON(success.data);
            $("select[name*='F_short_Logistic']").each(function () {
                //console.log($(this).attr("id"));
                $(this).addListSelectPicker(success.data, "f_short_Logistic")
            })
        },
        function (error) {
            xSwal.xError(error);
        }
    );
}

async function GetListData() {
    let obj = await $('#formMain').formToJSON();
    return _xLib.AJAX_Get("/api/KBNMS028/GetListData", obj,
        function (success) {
            _xLib.TrimArrayJSON(success.data);
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);

            $("table tbody tr td").addClass("ps-4 pb-2 pt-2 pe-4");
            $("#tableMain").DataTable().columns.adjust().draw();

        },
        function (error) {
            xSwal.xError(error);
        }
    );
}

async function Save() {
    var data = await $('#formMain').formToJSON();
    //console.log($("#divBtn").find("button:not(:disabled)").attr("id"));
    var action = $("#divBtn").find("button:not(:disabled)").attr("id").split("btn")[1];

    _xLib.AJAX_Post("/api/KBNMS028/Save?action=" + action, data,
        function (success) {
            $("#formMain").trigger("clear");
            $().resetAllSelectPicker();
            xSwal.xSuccess(success);
        },
        function (error) {
            xSwal.xError(error);
        }
    );
}

