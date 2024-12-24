$(document).ready(async function () {

    await $("#tableMain").DataTable({
        "processing": true,
        "serverSide": false,
        width: '100%',
        paging: false,
        sorting: false,
        searching: false,
        scrollX: false,
        scrollY: true,
        scrollCollapse: true,
        "columns": [
            {
                title: "Flag", render: function (data, type, row)
                {
                    return "<input type='checkbox' name='chkFlag' value='" + row.F_Supplier_Code + "-" + row.F_Supplier_Plant + "'>";
                }
            },
            { title: "Supplier Code", data: "F_Supplier_Code", },
            { title: "Cycle", data: "F_Cycle", },
            { title: "Kanban No", data: "F_Kanban_No", },
            { title: "Store Code", data: "F_Store_Code", },
            { title: "Part No", data: "F_Part_No" },
            { title: "Start Date", data: "F_Start_Date" },
            { title: "End Date", data: "F_End_Date", },
            { title: "Type Order", data: "F_Type_Special", },
        ],
        select: false,
        order: [[0, "asc"]]
    });

    await $("#inpStartDate").datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true,
        uilibrary: 'bootstrap5',
        showRightIcon: false,
        value: moment().format("DD/MM/YYYY")
    });
    await $("#inpEndDate").datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true,
        uilibrary: 'bootstrap5',
        showRightIcon: false,
        value: "31/12/2999"
    });

    //await GetSelectList();

    $("table thead tr th , table tbody tr td").addClass("text-center");

    $("#divDetail").find("select").prop("disabled", true);
    $("#divDetail").find("input[type='text']").prop("readonly", true);

    xSplash.hide();
});

$("#divButton").find("button").on("click", async function () {
    //await Inquiry();

    $("#tableMain").DataTable().clear().draw();

    //$(".selectpicker").each(function () {
    //    //$(this).prop("disabled", false);
    //    $(this).selectpicker("refresh");
    //});

    $("#inpStartDate").prop("readonly", false);
    $("#inpEndDate").prop("readonly", false);

    await $("#divButton").find("button").prop("disabled", true);

    $(this).prop("disabled", false);

    await GetSelectList();

    $(".selectpicker").each(function () {
        $(this).prop("disabled", false);
        $(this).resetSelectPicker();
    });
});

async function GetSelectList() {
    xSplash.show();

    var obj = {
        kanban: $("#inpKanban").val(),
        supplier: $("#inpSupplierCode").val(),
        partno: $("#inpPartNo").val(),
        storecd: $("#inpStore").val(),
        isNew : $("#divButton").find("button:not([disabled])").attr("id") == "btnNew" ? true : false,
    }

    _xLib.AJAX_Get("/api/KBNMS004/GetSelectList", obj,
        async function (success) {
            success = await _xLib.JSONparseMixData(success);
            //console.log(success);
            //let SupplierSet = [... new Set(data.data.map(x => x.F_supplier_cd + "-" + x.F_plant))];
            //SupplierSet = SupplierSet.sort();
            //let PartNoSet = [... new Set(data.data.map(x => x.F_Part_no + "-" + x.F_Ruibetsu))];
            //PartNoSet = PartNoSet.sort();
            //let StoreSet = [... new Set(data.data.map(x => x.F_Store_cd))];
            //StoreSet = StoreSet.sort();
            //let KanbanNoSet = [... new Set(data.data.map(x => "0" + x.F_Sebango))];
            //KanbanNoSet = KanbanNoSet.sort();

            //SupplierSet = SupplierSet.map(x => {
            //    return { Supplier: x }
            //});

            //PartNoSet = PartNoSet.map(x => {
            //    return { PartNo : x }
            //});

            //StoreSet = StoreSet.map(x => {
            //    return { Store : x }
            //});

            //KanbanNoSet = KanbanNoSet.filter(x => x != "0").map(x => {
            //    return { Kanban: x }
            //});

            //if ($("#inpSupplierCode").val() == "") {
            //    await $("#inpSupplierCode").addListSelectPicker(SupplierSet, "Supplier");
            //}

            //if ($("#inpPartNo").val() == "") {
            //    await $("#inpPartNo").addListSelectPicker(PartNoSet, "PartNo");
            //}

            //if ($("#inpStore").val() == "") {
            //    await $("#inpStore").addListSelectPicker(StoreSet, "Store");
            //}

            //if ($("#inpKanban").val() == "") {
            //    await $("#inpKanban").addListSelectPicker(KanbanNoSet, "Kanban");
            //}

            if ($("#inpSupplierCode").val() == "") {
                await $("#inpSupplierCode").addListSelectPicker(success.data[0], "F_Supplier");
            }

            if ($("#inpPartNo").val() == "") {
                await $("#inpPartNo").addListSelectPicker(success.data[1], "F_PartNo");
            }

            if ($("#inpStore").val() == "") {
                await $("#inpStore").addListSelectPicker(success.data[2], "F_StoreCD");
            }

            if ($("#inpKanban").val() == "") {
                await $("#inpKanban").addListSelectPicker(success.data[3], "F_KanbanNo");
            }

            return xSplash.hide();
        },
    );
}

$(".selectpicker").on("change", async function () {

    await GetListData();
    await GetSelectList();

    if ($(this).attr("id") == "inpSupplierCode") {
        await SelectedSupplier();
    }
    else if ($(this).attr("id") == "inpPartNo") {
        await SelectedPartNo();
    }

    $(`#${$(this).attr("id")}Read`).val($(this).val());

});

$("#btnSave").on("click", async function () {
    await Save();
});

async function SelectedSupplier() {
    let obj = await ObjGet();
    _xLib.AJAX_Get("/api/KBNMS004/SelectedSupplier", obj,
        async function (success) {
            //console.log(data);
            //console.log(success.data.f_Cycle_A.length);
            $("#inpSupplierName").val(success.data.f_name.trim());
            let cycle = (success.data.f_Cycle_A.length == 1) ? "0" + success.data.f_Cycle_A : success.data.f_Cycle_A;
            cycle += "-" + success.data.f_Cycle_B;
            cycle += "-" + success.data.f_Cycle_C;
            $("#inpCycle").val(cycle);
        },
    );
}

async function SelectedPartNo() {
    let obj = await ObjGet();
    _xLib.AJAX_Get("/api/KBNMS004/SelectedPartNo", obj,
        async function (success) {
            $("#inpPartNameRead").val(success.data.f_Part_nm.trim());
        },
    );
}

$("#btnCancel").on("click", async function () {
    $("#divDetail").find("select").prop("disabled", true);
    $("#divDetail").find("input[type='text']").prop("readonly", true);
    $("#divButton").find("button").prop("disabled", false);
    $(".selectpicker").each(function () {
        $(this).selectpicker("refresh");
        $(this).resetSelectPicker();
    });

    $("#divDetail").find("input[type='text']:not([class*='datepicker'])").val("");
    $("#inpStartDate").val(moment().format("DD/MM/YYYY"));
    $("#inpEndDate").val("31/12/2999");
});

$(document).on("click","#tableMain tbody tr td", function () {
    var row = $(this).closest("tr");
    //console.log(row);
    //row.find("input[type='checkbox']").prop("checked", true);

    var data = $("#tableMain").DataTable().row(row).data();
    //console.log(data);

    $("#inpSupplierCode").val(data.F_Supplier_Code);
    //$("#inpSupplierName").val(data.F_Supplier_Name);
    $("#inpPartNo").val(data.F_Part_No);
    //$("#inpPartNameRead").val(data.F_Part_Name);
    $("#inpStore").val(data.F_Store_Code);
    $("#inpKanban").val(data.F_Kanban_No);
    $("#inpCycle").val(data.F_Cycle);
    $("#inpStartDate").val(data.F_Start_Date);
    $("#inpEndDate").val(data.F_End_Date);
    $("#inpTypeOrder").val(data.F_Type_Special);

    $("#divDetail").find("select").each(function () {
        $("#" + $(this).attr("id") + "Read").val($(this).val());
    });

    SelectedPartNo();
    SelectedSupplier();

    $(".selectpicker").each(function () {
        $(this).selectpicker("refresh");
    });

    if ($("#btnUpdate").prop("disabled") == false) {
        //$("#tableMain").find("input[type='checkbox']").prop("checked", true);
        $("#tableMain").find("input[type='checkbox']").prop("checked", false);
        $(this).closest("tr").find("input[type='checkbox']").prop("checked", true);
        $("#divDetail").find("select:not([id*='inpTypeOrder'])").prop("disabled", true);
        $("#divDetail").find("input[type='text']:not([id*='inpEndDate'])").prop("readonly", true);
        $(".selectpicker").each(function () {
            $(this).selectpicker("refresh");
        });
    }
    
});

async function ObjGet() {

    var obj = {
        kanban: $("#inpKanban").val(),
        supplier: $("#inpSupplierCode").val(),
        partno: $("#inpPartNo").val(),
        storecd: $("#inpStore").val(),
        type: $("#inpTypeOrder").val(),
        isNew: $("#divButton").find("button:not([disabled])").attr("id") == "btnNew" ? true : false,
    }

    return obj;

}

async function ObjPost() {

    var obj = {
        kanban: $("#inpKanban").val(),
        supplier: $("#inpSupplierCode").val(),
        partno: $("#inpPartNo").val(),
        storecd: $("#inpStore").val(),
        TypeOrder: $("#inpTypeOrder").val(),
        startDate: moment($("#inpStartDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        endDate: moment($("#inpEndDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        cycle: $("#inpCycle").val(),
        action: $("#divButton").find("button:not([disabled])").attr("id"),
    }

    let listObj = [];

    listObj.push(obj);

    if (obj.action == "btnDelete") {

        listObj = [];

        _xDataTable.GetSelectedDataDT("#tableMain").forEach(function (data) {
            listObj.push({
                kanban: data.F_Kanban_No,
                supplier: data.F_Supplier_Code,
                partno: data.F_Part_No,
                storecd: data.F_Store_Code,
                TypeOrder: data.F_Type_Special,
                startDate: moment(data.F_Start_Date, "DD/MM/YYYY").format("YYYYMMDD"),
                endDate: moment(data.F_End_Date, "DD/MM/YYYY").format("YYYYMMDD"),
                cycle: data.F_Cycle,
                action: obj.action,
            });
        });

    }

    return listObj;
}

async function Save() {
    await xSplash.show();

    var obj = await ObjPost();

    if (obj.length == 0) {
        await xSplash.hide();
        xSwal.error("Error", "Please select data to Save");
        return;
    }

    _xLib.AJAX_Post("/api/KBNMS004/Save", obj,
        async function (success) {
            await xSplash.hide();
            xSwal.success("Success", success.message);
            await GetListData();
        },
        async function (error) {
            await xSplash.hide();
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );

}

async function GetListData() {

    var obj = {
        kanban: $("#inpKanban").val(),
        supplier: $("#inpSupplierCode").val(),
        partno: $("#inpPartNo").val(),
        storecd: $("#inpStore").val(),
        type: $("#inpTypeOrder").val(),
    }

    await _xLib.AJAX_Get("/api/KBNMS004/GetListData", obj,
        async function (data) {
            data = await _xLib.JSONparseMixData(data);
            //console.log(data);
            _xDataTable.ClearAndAddDataDT("#tableMain", data.data);
        },
    );

}