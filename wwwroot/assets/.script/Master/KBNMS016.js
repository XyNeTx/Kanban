$(document).ready(function () {

    // Initialize the DataTable with the dynamically created columns
    _xDataTable.InitialDataTable("#tableMain", {
        processing: false,
        serverSide: false,
        width: '100%',
        paging: false,
        sorting: false,
        searching: false,
        scrollX: true,
        scrollY: "200px",
        scrollCollapse: false,
        select: false,
        columns:
        [
                {
                    title: "Flag",
                    render(data, type, row) {
                        return `<input type="checkbox" class="chkbox" id="chkbox" name="chkbox">`;
                    }
                },
                {
                    title: "Group Name",data: "F_Group"
                },
                {
                    title: "Supplier Code", data: "F_Supplier_Cd"
                },
                {
                    title: "Kanban No", data: "F_Kanban_No"
                },
                {
                    title: "Store Code", data: "F_Store_Cd"
                },
                {
                    title: "Part No.", data: "F_Part_No"
                },
                {
                    title: "Q'ty", data: "F_Qty"
                },
                {
                    title: "Start Date", data: "F_Start_Date"
                },
                {
                    title: "End Date", data: "F_End_Date"
                },
        ],
        order: [[0, "asc"]]
    });

    $("#F_Start_Date").initDatepicker();
    $("#F_End_Date").initDatepicker("31/12/2999");

    GetDropDown();
    List_Data();

    xSplash.hide();
})

var action = "";

$("#divBtn").on("click", "button", async function () {
    $("#divBtn").find("button").prop("disabled", true);
    $(this).prop("disabled", false);

    action = $(this).attr("id").split("btn")[1].toLowerCase();

    if (action == "new" || action == "upd") {
        $("#F_Group").SelectToInput("text");
    }

    GetDropDown();

});

$("#btnCan").click(function () {
    $("#divBtn").find("button").prop("disabled", false)
    $("#formMain").trigger("reset");
    $().resetAllSelectPicker();
    $("#tableMain").DataTable().clear().draw();

    $("#F_Group").InputToSelect("8");


    $("#F_Start_Date").val(moment().format("DD/MM/YYYY"));
    $("#F_End_Date").val("31/12/2999");

    action = "";
    List_Data();
})

$(document).on("click","table tbody tr",function () {
    var row = $(this).closest("tr");
    //console.log(row);

    var obj = $("#tableMain").DataTable().row(row).data();

    _xLib.ObjSetVal(obj);
    //console.log(obj);

})

$("#btnSave").click(function () {
    Save();
})

$("#F_Supplier_Cd").change(function () {
    GetDropDown();
    GetSupplierDetail();
    //$("#F_Kanban_No_Rd").val($(this).val());
})
$("#F_Kanban_No").change(function () {
    $("#F_Kanban_No_Rd").val($(this).val());
    GetDropDown();
})
$("#F_Store_Cd").change(function () {
    $("#F_Store_Cd_Rd").val($(this).val());
    GetDropDown();
})
$("#F_Part_No").change(function () {
    $("#F_Part_No_Rd").val($(this).val());
    GetDropDown();
    GetPartNoDetail();
})


async function List_Data() {
    let getObj = await $("#formMain").formToJSON();
    getObj.F_Supplier_Plant = getObj.F_Supplier_Cd !== "" ? getObj.F_Supplier_Cd.split("-")[1] : "";
    getObj.F_Supplier_Cd = getObj.F_Supplier_Cd !== "" ? getObj.F_Supplier_Cd.split("-")[0] : "";
    getObj.F_Ruibetsu = getObj.F_Part_No !== "" ? getObj.F_Part_No.split("-")[1] : "";
    getObj.F_Part_No = getObj.F_Part_No !== "" ? getObj.F_Part_No.split("-")[0] : "";


    _xLib.AJAX_Get("/api/KBNMS016/List_Data", getObj,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        },
    );
}

async function GetSupplierDetail() {
    let getObj = await $("#formMain").formToJSON();
    getObj.F_Supplier_Plant = getObj.F_Supplier_Cd !== "" ? getObj.F_Supplier_Cd.split("-")[1] : "";
    getObj.F_Supplier_Cd = getObj.F_Supplier_Cd !== "" ? getObj.F_Supplier_Cd.split("-")[0] : "";
    getObj.F_Ruibetsu = getObj.F_Part_No !== "" ? getObj.F_Part_No.split("-")[1] : "";
    getObj.F_Part_No = getObj.F_Part_No !== "" ? getObj.F_Part_No.split("-")[0] : "";

    _xLib.AJAX_Get("/api/KBNMS016/GetSupplierDetail", getObj,
        function (success) {
            success.data = _xLib.TrimJSON(success.data);
            console.log(success);
            $("#F_Supplier_Name").val(success.data.f_name);
            let cycle = success.data.f_Cycle_A.length === 1 ? "0" + success.data.f_Cycle_A : success.data.f_Cycle_A;
            cycle += "-" + success.data.f_Cycle_B;
            cycle += "-" + success.data.f_Cycle_C;
            $("#F_Cycle").val(cycle);
        },
    );
}
async function GetPartNoDetail() {
    let getObj = await $("#formMain").formToJSON();
    getObj.F_Supplier_Plant = getObj.F_Supplier_Cd !== "" ? getObj.F_Supplier_Cd.split("-")[1] : "";
    getObj.F_Supplier_Cd = getObj.F_Supplier_Cd !== "" ? getObj.F_Supplier_Cd.split("-")[0] : "";
    getObj.F_Ruibetsu = getObj.F_Part_No !== "" ? getObj.F_Part_No.split("-")[1] : "";
    getObj.F_Part_No = getObj.F_Part_No !== "" ? getObj.F_Part_No.split("-")[0] : "";

    _xLib.AJAX_Get("/api/KBNMS016/GetPartNoDetail", getObj,
        function (success) {
            success.data = _xLib.TrimJSON(success.data);
            console.log(success);
            $("#F_PartName").val(success.data.f_Part_nm);
        },
    );
}

async function Save() {
    let getObj = await $("#formMain").formToJSON();
    getObj.F_Plant = ajexHeader.Plant;
    getObj.F_Supplier_Plant = getObj.F_Supplier_Cd !== "" ? getObj.F_Supplier_Cd.split("-")[1] : "";
    getObj.F_Supplier_Cd = getObj.F_Supplier_Cd !== "" ? getObj.F_Supplier_Cd.split("-")[0] : "";
    getObj.F_Ruibetsu = getObj.F_Part_No !== "" ? getObj.F_Part_No.split("-")[1] : "";
    getObj.F_Part_No = getObj.F_Part_No !== "" ? getObj.F_Part_No.split("-")[0] : "";
    getObj.action = action;

    console.log(getObj);

    let listObj = [];

    listObj.push(getObj);

    if (action == "del") {
        listObj = _xDataTable.GetSelectedDataDT("#tableMain");

        listObj.map(function (obj, index) {
            console.log(obj);
            obj.F_Supplier_Plant = obj.F_Supplier_Cd !== "" ? obj.F_Supplier_Cd.split("-")[1] : "";
            obj.F_Supplier_Cd = obj.F_Supplier_Cd !== "" ? obj.F_Supplier_Cd.split("-")[0] : "";
            obj.F_Ruibetsu = obj.F_Part_No !== "" ? obj.F_Part_No.split("-")[1] : "";
            obj.F_Part_No = obj.F_Part_No !== "" ? obj.F_Part_No.split("-")[0] : "";
            obj.F_Start_Date = moment(obj.F_Start_Date, "DD/MM/YYYY").format("YYYYMMDD");
            obj.F_End_Date = moment(obj.F_End_Date, "DD/MM/YYYY").format("YYYYMMDD");
            obj.F_Plant = ajexHeader.Plant;
        });

    }

    //return;

    _xLib.AJAX_Post("/api/KBNMS016/Save?action=" + action, listObj,
        function (success) {
            xSwal.xSuccess(success);
            List_Data();
        },
        function (error) {
            xSwal.xError(error);
        }
    );
}

async function GetDropDown() {
    let getObj = await $("#formMain").formToJSON();
    getObj.F_Supplier_Plant = getObj.F_Supplier_Cd !== "" ? getObj.F_Supplier_Cd.split("-")[1] : "";
    getObj.F_Supplier_Cd = getObj.F_Supplier_Cd !== "" ? getObj.F_Supplier_Cd.split("-")[0] : "";
    getObj.F_Ruibetsu = getObj.F_Part_No !== "" ? getObj.F_Part_No.split("-")[1] : "";
    getObj.F_Part_No = getObj.F_Part_No !== "" ? getObj.F_Part_No.split("-")[0] : "";
    getObj.action = action;

    _xLib.AJAX_Get("/api/KBNMS016/GetDropDown", getObj,
        function (success) {
            //console.log(success);
            if ($("#F_Supplier_Cd").val() === "") $("#F_Supplier_Cd").addListSelectPicker(success.data.supplier, "f_Supplier_Cd")
            if ($("#F_Kanban_No").val() === "") $("#F_Kanban_No").addListSelectPicker(success.data.kanban, "f_Kanban_No")
            if ($("#F_Store_Cd").val() === "") $("#F_Store_Cd").addListSelectPicker(success.data.store, "f_Store_Cd")
            if ($("#F_Part_No").val() === "") $("#F_Part_No").addListSelectPicker(success.data.partno, "f_Part_No")
            if ($("#F_Group").val() === "") $("#F_Group").addListSelectPicker(success.data.group, "f_Group")
        },
    );

}