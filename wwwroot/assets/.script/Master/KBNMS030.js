$(document).ready(async function () {


    _xDataTable.InitialDataTable("#tableMain",
    {
        columns:
            [
                {
                    title: "Flag", render: function (data, type, row, meta) {
                        return `<input type="checkbox" class="chkbox">`;
                    }, width: "8%"
                },
                { title: "Line ID", data: "f_Line_ID" },
                { title: "Description", data: "f_Description" },
                { title: "Customer", data: "f_Customer" },
            ],
        scrollX: true,
        scrollY: "175px",
        scrollCollapse: false,
        paging: false,
        ordering: false,
        width: "100%",
        searching: true,
    });

    $("table thead tr th").addClass("text-center");
    $("table tbody tr td").addClass("text-center");

    GetListData();

    await xSplash.hide();

})

$("#divBtn").on("click", "button", async function () {
    //console.log("click");

    $("#divBtn").find("button").prop("disabled", true);
    await $(this).prop("disabled", false);
    //console.log("click 1");

    $("#divInput").find("input").prop("disabled", false);

});

$("#btnSelAll").click(function () {
    $(".chkbox").prop("checked", true);
});

$("#btnDeselAll").click(function () {
    $(".chkbox").prop("checked", false);
});

$(document).on("click", "#tableMain tbody tr", function () {
    $(this).toggleClass("selected");
    let row = $(this).closest("tr");

    let obj = _xDataTable.GetDataDT("#tableMain", row);

    $("#F_Line_ID").val(obj.f_Line_ID);
    $("#F_Description").val(obj.f_Description);
    $("#F_Customer").val(obj.f_Customer);
    $("#F_Line_Customer").val(obj.f_Line_Customer);

});

$("#btnCan").click(function () {

    $("#divBtn").find("button").prop("disabled", false);

    $("#F_Line_ID").val("");
    $("#F_Description").val("");
    $("#F_Customer").val("");

    $("#divInput").find("input").prop("disabled", true);


    GetListData();
});

$("#btnSave").click(function () {
    Save();
});

function GetListData() {

    let obj = {
        f_Line_ID: $("#F_Line_ID").val(),
        f_Description: $("#F_Description").val(),
        f_Customer: $("#F_Customer").val(),
        f_Line_Customer: $("#F_Line_ID").val() + "_" + $("#F_Customer").val()
    };

    _xLib.AJAX_Get("/api/KBNMS030/GetListData", obj,
        function (success) {
            console.log(success);
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        },
        function (error) {
        }
    );
}

function Save() {
    let listObj = [];
    let obj = {
        f_Line_ID: $("#F_Line_ID").val(),
        f_Description: $("#F_Description").val(),
        f_Customer: $("#F_Customer").val(),
        f_Line_Customer: $("#F_Line_ID").val() + "_" + $("#F_Customer").val()
    };
    listObj.push(obj);

    let action = $("#divBtn").find("button:not(:disabled)").attr("id").split("btn")[1];

    if (action == "Del") {
        listObj = _xDataTable.GetSelectedDataDT("#tableMain");
    }

    _xLib.AJAX_Post("/api/KBNMS030/Save?Action=" + action, listObj,
        function (success) {
            xSwal.success(success.response, success.message);
            $("#btnCan").trigger("click");
        },
        function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );
}

