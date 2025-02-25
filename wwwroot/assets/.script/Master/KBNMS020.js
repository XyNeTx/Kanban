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
                    title: "Part No.", data: "F_PartNo"
                },
                {
                    title: "Store Code", data: "F_StoreCD"
                },
                {
                    title: "Part Name", data: "F_Part_nm"
                },
                {
                    title: "Supplier Code", data: "F_Supplier"
                },
                {
                    title: "Supplier Name", data: "F_name"
                },
                {
                    title: "Kanban No.", data: "F_KanbanNo"
                },
                {
                    title: "Q'ty", data: "F_BoxQty"
                },
                {
                    title: "Address", data: "Group_Part"
                },
                {
                    title: "STD Stock (Day)", data: "F_BoxQty"
                },
                {
                    title: "STD Stock (KB)", data: "", render(data, type, row) {
                        console.log(data);
                        console.log(type);
                        console.log(row);
                        return row.F_STD_Stock / row.F_BoxQty;
                    }
                },
                {
                    title: "Dock", data: "Dock"
                },
                {
                    title: "Package Type", data: "F_Package_Type"
                },
                {
                    title: "Max Area", data: "F_Max_Area"
                },
            ],
            select: false,
            order: [[0, "asc"]]
        });

    GetDropDownList();

})

$("#divBtn").on("click", "button", async function () {
    $("#divBtn").find("button").prop("disabled", true);
    $(this).prop("disabled", false);

    var action = $(this).attr("id").split("btn")[1].toLowerCase();

    if (action == "new" || action == "upd") {
        $("#F_Max_Area").prop("readonly", false);
    }
});

$("#btnCan").click(function () {
    $("#divBtn").find("button").prop("disabled", false)
    $("#formMain").trigger("reset");
    $().resetAllSelectPicker();
    $("#tableMain").DataTable().clear().draw();
    $("#F_Max_Area").prop("readonly", true);

})

$("#F_Supplier").change(function () {
    $("#F_Read_Supplier").val($(this).val());
    GetSupplierName();
    GetDropDownList();
    GetListData();
});

$("#F_KanbanNo").change(function () {
    $("#F_Read_Kanban").val($(this).val());
    GetQtyBox();
    GetDropDownList();
    GetListData();
});
$("#F_StoreCD").change(function () {
    $("#F_Read_Store").val($(this).val());
    GetDropDownList();
    GetListData();
});
$("#F_PartNo").change(function () {
    $("#F_Read_PartNo").val($(this).val());
    GetDropDownList();
    GetPartName();
    GetListData();
});

$("#btnSave").click(function () {
    Save();
});

async function ObjGet() {
    var obj = await $("#formMain").formToJSON();
    obj.isNew = await $("#divBtn").find("button:not(:disabled)").attr("id") == "btnNew";

    return obj
}

async function GetDropDownList() {
    var getObj = await ObjGet();

    _xLib.AJAX_Get("/api/KBNMS020/GetDropDownList", getObj,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            console.log(success)

            if (!$("#F_Supplier").val()) {
                $("#F_Supplier").addListSelectPicker(success.data.f_Supplier, "f_Supplier");
            }

            if (!$("#F_KanbanNo").val()) {
                $("#F_KanbanNo").addListSelectPicker(success.data.f_KanbanNo, "f_KanbanNo");
            }

            if (!$("#F_StoreCD").val()) {
                $("#F_StoreCD").addListSelectPicker(success.data.f_StoreCD, "f_StoreCD");
            }

            if (!$("#F_PartNo").val()) {
                $("#F_PartNo").addListSelectPicker(success.data.f_PartNo, "f_PartNo");
            }
            xSplash.hide();
        }
    )
}

async function GetSupplierName() {
    var getObj = await ObjGet();

    _xLib.AJAX_Get("/api/KBNMS020/GetSupplierName", getObj,
        function (success) {
            success.data = _xLib.TrimJSON(success.data);
            $("#F_Read_Cycle").val(success.data.cycle);
            $("#F_Read_SupplierName").val(success.data.name);
            console.log(success);
        }
    )
}

async function GetQtyBox() {
    var getObj = await ObjGet();

    _xLib.AJAX_Get("/api/KBNMS020/GetQtyBox", getObj,
        function (success) {
            success.data = _xLib.TrimJSON(success.data);
            console.log(success);
            $("#F_BoxQty").val(success.data);
        }
    )
}

async function GetPartName() {
    var getObj = await ObjGet();

    _xLib.AJAX_Get("/api/KBNMS020/GetPartName", getObj,
        function (success) {
            success.data = _xLib.TrimJSON(success.data);
            console.log(success);
            $("#F_Read_PartName").val(success.data);
        }
    )
}

async function GetListData() {
    var getObj = await ObjGet();

    _xLib.AJAX_Get("/api/KBNMS020/GetListData", getObj,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
            console.log(success);
            //$("#F_Read_PartName").val(success.data);
        }
    )
}

async function Save() {
    var getObj = await ObjGet();
    var listObj = [];
    listObj.push(getObj);

    let action = $("#divBtn").find("button:not(:disabled)").attr("id").split("btn")[1].toLowerCase();

    if (action == "del") {
        listObj = _xDataTable.GetSelectedDataDT("#tableMain");
    }

    _xLib.AJAX_Post("/api/KBNMS020/Save?action=" + action, listObj,
        function (success) {
            GetListData();
            xSwal.xSuccess(success);
        }
    )
}