$(document).ready(function () {

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
                    title: "Plant", data: "F_PlantCode"
                },
                {
                    title: "Dock Code", data: "F_Dock_Code"
                },
                {
                    title: "Start Date", data: "F_Start_Date", render: function (row, meta) {
                        return moment(row, "YYYYMMDD").format("DD/MM/YYYY");
                    }
                },
                {
                    title: "End Date", data: "F_End_Date", render: function (row, meta) {
                        return moment(row, "YYYYMMDD").format("DD/MM/YYYY");
                    }
                },
            ],
            select: false,
            order: [[0, "asc"]]
        });

    $("#F_Start_Date").initDatepicker();
    $("#F_End_Date").initDatepicker("31/12/2999");

    GetListData();
    GetDockCode();

    xSplash.hide();

})


$("#btnSelAll").click(function () {
    $(".chkbox").prop("checked", true);
});
$("#btnDeselAll").click(function () {
    $(".chkbox").prop("checked", false);
});

$("#divBtn").on("click", "button", async function () {
    $("#divBtn").find("button").prop("disabled", true);
    $(this).prop("disabled", false);
    let _id = $(this).attr("id").toLowerCase();
    if (_id.includes("new"))
    {
        console.log($("#formMain").find("input[name='F_Dock_Code']").length);
        if ($("#formMain").find("input[name='F_Dock_Code']").length > 0)
        {
            return;
        }

        $("#F_Dock_Code").parent().remove();
        $("label[for='F_Dock_Code']").parent()
        .append(`
            <input type='text' class="form-control col-4" 
                data-val="true" data-val-length="The field Dock Code :  must be a string with a maximum length of 10."
                data-val-length-max="10" data-val-required="The Dock Code :  field is required." 
                id="F_Dock_Code" name="F_Dock_Code">
            </input>
        `);

    }

});

$(document).on("dblclick", "table tbody tr td", async function () {

    await $("#formMain").trigger("reset");
    var row = $(this).closest("tr");
    var obj = $("#tableMain").DataTable().row(row).data();

    $(".selected").removeClass("selected");

    $(this).closest("tr").toggleClass("selected");
    $(this).closest("tr").find("#chkbox").prop("checked", true);

    for (const e of Object.keys(obj)) {

        let _e = "F" + e.substring(1, e.length);

        if ($(`#${_e}`).attr("data-datepicker"))
        {
            $(`#${_e}`).val(moment(obj[e], "YYYYMMDD").format("DD/MM/YYYY"));
        }
        else $(`#${_e}`).val(obj[e]);
        
        if ($(`#${_e}`).prop("tagName") === "SELECT") {
            $(`#${_e}`).selectpicker('val',obj[e]);
        }

    }

    var action = $("#divBtn").find("button:not(:disabled)").attr("id").split("btn")[1];

    if (action.toLowerCase() == "upd") {
        $("#F_Dock_Code").disableSelectPicker();
        $("#F_Start_Date").prop("readonly", true);
    }

});

$("#btnCan").click(async function () {

    $("#divBtn").find("button").prop("disabled", false);

    if ($("#formMain").find("input[name='F_Dock_Code']").length > 0)
    {
        $("#F_Dock_Code").remove();
        $("label[for='F_Dock_Code']").parent().append(`
        <select class="selectpicker p-0 col-3" 
            data-live-search="true" data-size="6" 
            data-val="true" 
            data-val-length="The field Dock Code :  must be a string with a maximum length of 10." 
            data-val-length-max="10" 
            data-val-required="The Dock Code :  field is required." 
            id="F_Dock_Code" name="F_Dock_Code">
            <option value=""></option>
        </select>`);
        GetDockCode();
    }

    $("#formMain").trigger("reset");
    $().resetAllSelectPicker();

    $("#F_Start_Date").val(moment().format("DD/MM/YYYY"));
    $("#F_End_Date").val("31/12/2999");

    $("#F_Dock_Code").enableSelectPicker();
    $("#F_Start_Date").prop("readonly", false);

    GetListData();
});

$("#btnSave").click(async function () {
    await Save();
});

$("#btnReport").click(async function () {
    let obj = {
        UserName: _xLib.GetUserName(),
        Plant: "3",
        DockCode: $("#F_Dock_Code").val()
    }

    return _xLib.OpenReportObj("/KBNMS029", obj);

});

function GetListData() {
    var obj = {
        DockCode: $("#F_Dock_Code").val()
    }

    _xLib.AJAX_Get("/api/KBNMS029/GetListData", obj,
        function (success) {
            //success.data = _xLib.TrimArrayJSON(success.data);
            success = _xLib.JSONparseMixData(success);
            //console.log(success)
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
            //xSwal.xSuccess(success);
        },
        function (error) {
            xSwal.xError(error);
        }
    );

}

function GetDockCode() {

    _xLib.AJAX_Get("/api/KBNMS029/GetDockCode", null,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            $("#F_Dock_Code").addListSelectPicker(success.data, "f_Dock_Code");
            //xSwal.xSuccess(success);
        },
        function (error) {
            xSwal.xError(error);
        }
    );

}

async function Save()
{
    let listObj = [];

    var action = $("#divBtn").find("button:not(:disabled)").attr("id").split("btn")[1];

    if (action.toLowerCase() == "del")
    {
        listObj = _xDataTable.GetSelectedDataDT("#tableMain");
    }
    else
    {
        var obj = await $("#formMain").formToJSON();

        listObj.push(obj);
    }

    _xLib.AJAX_Post("/api/KBNMS029/Save?action=" + action, listObj,
        function (success) {
            $("#formMain").trigger("reset");
            GetListData();
            xSwal.xSuccess(success);
        },
        function (error) {
            xSwal.xError(error);
        }
    );

}
