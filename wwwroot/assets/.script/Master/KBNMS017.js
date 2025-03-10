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
                    title: "Part No.", data: "f_Part_No", render(data, type, row) {
                        //console.log(row);
                        return row.f_Part_No + "-" + row.f_Ruibetsu
                    }
                },
                {
                    title: "Store Code.", data: "f_Store_Cd"
                },
                {
                    title: "Address1.", data: "f_Address1"
                },
                {
                    title: "Ratio1", data: "f_Ratio1"
                },
                {
                    title: "Address2", data: "f_Address2"
                },
                {
                    title: "Ratio2", data: "f_Ratio2"
                },
                {
                    title: "Address3", data: "f_Address3"
                },
                {
                    title: "Ratio3", data: "f_Ratio3"
                },
            ],
            select: false,
            order: [[0, "asc"]]
        });

    //GetDropDownList();
    ListData();

    xSplash.hide();
})

let action = "";
$("#divBtn").on("click", "button", async function () {
    $("#divBtn").find("button").prop("disabled", true);
    $(this).prop("disabled", false);

    action = $(this).attr("id").split("btn")[1].toLowerCase();

    if (action == "new" || action == "upd") {
        $("#F_Address1").SelectToInput("text", "7");
        $("#F_Address2").SelectToInput("text", "7");
        $("#F_Address3").SelectToInput("text", "7");
    }

    GetDropDownList();

});

$("#btnCan").click(function () {
    $("#divBtn").find("button").prop("disabled", false)
    $("#formMain").trigger("reset");
    $().resetAllSelectPicker();
    $("#tableMain").DataTable().clear().draw();

    if (action == "new" || action == "upd") {
        $("#F_Address1").InputToSelect("8");
        $("#F_Address2").InputToSelect("8");
        $("#F_Address3").InputToSelect("8");
    }

    ListData();

    action = "";
})

$("#F_Part_No").change(function () {
    GetPartName();
    GetDropDownList();
    ListData();
})

$("#F_Store_Cd").change(function () {
    ListData();
})

$("#btnSave").click(function () {
    Save();
})

async function GetDropDownList() {
    let getObj = await $("#formMain").formToJSON();
    getObj.action = action
    getObj.F_Ruibetsu = getObj.F_Part_No.split("-")[1];
    getObj.F_Part_No = getObj.F_Part_No.split("-")[0];

    _xLib.AJAX_Get("/api/KBNMS017/GetDropDownList", getObj,
        function (success) {
            //console.log(success);
            if (!getObj.F_Part_No) {
                $("#F_Part_No").addListSelectPicker(success.partNo, "f_Part_No");
            }
            if (!getObj.F_Store_Cd) {
                $("#F_Store_Cd").addListSelectPicker(success.storeCode, "f_Store_Cd");
            }
        }
    );

}
async function ListData() {
    let getObj = await $("#formMain").formToJSON();
    getObj.action = action
    getObj.F_Ruibetsu = getObj.F_Part_No.split("-")[1];
    getObj.F_Part_No = getObj.F_Part_No.split("-")[0];

    _xLib.AJAX_Get("/api/KBNMS017/ListData", getObj,
        function (success) {
            //console.log(success);
            success.data = _xLib.TrimArrayJSON(success.data);
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        }
    );

}

async function Save() {
    let getObj = await $("#formMain").formToJSON();
    getObj.action = action
    getObj.F_Ruibetsu = getObj.F_Part_No.split("-")[1];
    getObj.F_Part_No = getObj.F_Part_No.split("-")[0];
    getObj.F_Plant = ajexHeader.Plant;
    getObj.F_Ratio2 = (getObj.F_Ratio2) ?? null
    getObj.F_Ratio3 = (getObj.F_Ratio3) ?? null
    getObj.F_Address2 = (getObj.F_Address2) ?? null
    getObj.F_Address3 = (getObj.F_Address3) ?? null

    let listObj = [];

    listObj.push(getObj);

    if (action == "del") {
        listObj = _xDataTable.GetSelectedDataDT("#tableMain");

        listObj.forEach(function (each) {
            each.f_Ratio2 = null;
            each.f_Ratio3 = null;
        })

    }

    _xLib.AJAX_Post("/api/KBNMS017/Save?action=" + getObj.action, listObj,
        function (success) {
            ListData();
            xSwal.xSuccess(success);
        }
    );

}

async function GetPartName() {
    let getObj = await $("#formMain").formToJSON();
    getObj.action = action;
    getObj.F_Ruibetsu = getObj.F_Part_No.split("-")[1];
    getObj.F_Part_No = getObj.F_Part_No.split("-")[0];

    _xLib.AJAX_Get("/api/KBNMS017/GetPartName", getObj,
        function (success) {
            //console.log(success);
            $("#F_Part_Name").val(success.data);
        }
    );

}

