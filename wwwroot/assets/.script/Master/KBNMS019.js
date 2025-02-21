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
                    title: "Supplier Code", data: "f_Supplier"
                },
                {
                    title: "Kanban No.", data: "f_KanbanNo"
                },
                {
                    title: "Part No.", data: "f_PartNo"
                },
                {
                    title: "Part Name", data: "f_PartName"
                },
                {
                    title: "Store Code", data: "f_StoreCD"
                },
                {
                    title: "Max/Trip", data: "f_Max_Trip"
                },
            ],
            select: false,
            order: [[0, "asc"]]
        });

    //await GetSupplier();

    xSplash.hide();
})

$(document).on("dblclick", "#tableMain tbody tr", async function () {
    var row = $(this).closest("tr");
    $(row).find("input[type='checkbox']").prop("checked", true);
    $(this).addClass("selected");
    $(".selected").removeClass("selected");
    $(this).closest("tr").toggleClass("selected");
    var data = _xDataTable.GetDataDT("#tableMain", row);
    data.f_Total_Max_Trip = $("#F_Total_Max_Trip").val();
    //console.log($("#F_Total_Max_Trip").val());
    await $("#formMain").trigger("reset");
    await _xLib.ObjSetVal(data, true);

});

$("#divBtn").on("click", "button", async function () {
    $("#divBtn").find("button").prop("disabled", true);
    $(this).prop("disabled", false);
    GetSupplier();
});

$("#F_Supplier").change(function () {
    GetPartNo();
    GetStoreCD();
    GetKanbanNo();
    GetListMaxArea();
});

$("#F_PartNo").change(function () {
    GetStoreCD();
    GetPartName();
});

$("#F_StoreCD").change(function () {
    GetKanbanNo();
});
$("#F_KanbanNo").change(function () {
    GetMaxTrip();
});

$("#btnSave").click(function () {
    Save();
});

$("#btnCan").click(function () {
    $("#divBtn").find("button").prop("disabled",false)
    $("#formMain").trigger("reset");
    $().resetAllSelectPicker();
    $("#tableMain").DataTable().clear().draw();
})

async function ObjGet() {
    var obj = await $("#formMain").formToJSON();
    obj.isNew = await $("#divBtn").find("button:not(:disabled)").attr("id") == "btnNew";

    return obj
}

async function GetSupplier() {
    var getObj = await ObjGet();

    _xLib.AJAX_Get("/api/KBNMS019/GetSupplier", getObj,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            $("#F_Supplier").addListSelectPicker(success.data, "f_Supplier_Code");
        }
    )
}

async function GetPartNo() {
    var getObj = await ObjGet();

    _xLib.AJAX_Get("/api/KBNMS019/GetPartNo", getObj,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            $("#F_PartNo").addListSelectPicker(success.data, "f_Part_No");
        }
    )
}

async function GetListMaxArea() {
    var getObj = await ObjGet();

    _xLib.AJAX_Get("/api/KBNMS019/GetListMaxArea", getObj,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
            if (success.data.length > 1) {
                $("#F_Total_Max_Trip").val(success.maxTotal);
            }
            else {
                $("#F_Total_Max_Trip").val(success.data[0].f_Max_Trip);
            }
        }
    )
}

async function GetStoreCD() {
    var getObj = await ObjGet();

    _xLib.AJAX_Get("/api/KBNMS019/GetStoreCD", getObj,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            $("#F_StoreCD").addListSelectPicker(success.data, "f_Store_Code");
        }
    )
}

async function GetKanbanNo() {
    var getObj = await ObjGet();

    _xLib.AJAX_Get("/api/KBNMS019/GetKanbanNo", getObj,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            $("#F_KanbanNo").addListSelectPicker(success.data, "f_Kanban_No");
        }
    )
}

async function GetPartName() {
    var getObj = await ObjGet();

    _xLib.AJAX_Get("/api/KBNMS019/GetPartName", getObj,
        function (success) {
            //success.data = _xLib.TrimArrayJSON(success.data);
            $("#F_PartName").val(success.data.trim());
        }
    )
}

async function GetMaxTrip() {
    var getObj = await ObjGet();

    _xLib.AJAX_Get("/api/KBNMS019/GetMaxTrip", getObj,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
        }
    )
}

async function Save() {
    var Obj = await ObjGet();
    var action = $("#divBtn").find("button:not(:disabled)").attr("id").split("btn")[1];
    var listObj = [];
    if (action.toLowerCase() != "del") {
        Obj.F_Total_Max_Trip = Obj.F_Total_Max_Trip == "" ? 0 : Obj.F_Total_Max_Trip;
        Obj.F_Max_Trip = Obj.F_Max_Trip == "" ? 0 : Obj.F_Max_Trip;
        listObj.push(Obj);
    }
    else {
        listObj = _xDataTable.GetSelectedDataDT("#tableMain");
    }

    _xLib.AJAX_Post("/api/KBNMS019/Save?action=" + action, listObj,
        function (success) {
            GetListMaxArea();
            xSwal.xSuccess(success);
        },
        function (error) {
            xSwal.xError(error);
        }
    )
}
