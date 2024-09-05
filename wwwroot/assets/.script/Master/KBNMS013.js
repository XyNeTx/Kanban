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
            { title: "Supplier Code", data: "F_Supplier_Code", },
            { title: "Cycle", data: "F_Cycle", },
            { title: "Kanban No", data: "F_Kanban_No", },
            { title: "Store Code", data: "F_Store_Code", },
            { title: "Part No", data: "F_Part_No" },
            { title: "Start Date", data: "F_Start_Date" },
            { title: "End Date", data: "F_End_Date", },
            { title: "Type Order", data: "F_Type_Order", },
            { title: "Dock Code", data: "F_Dock_Code", },
            { title: "PDS Group", data: "F_PDS_Group", },
        ],
        select: true,
        order: [[0, "asc"]]
    });

    await $("#inpStartDate").datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true,
        modal: true,
        header: true,
        footer: true,
        showRightIcon: false
    });
    await $("#inpEndDate").datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true,
        modal: true,
        header: true,
        footer: true,
        showRightIcon: false
    });


    //await Inquiry("all");
    

    $("table thead tr th , table tbody tr td").addClass("text-center");

    $("#divDetail").find("select").prop("disabled", true);
    $("#divDetail").find("input[type='text']").prop("readonly", true);
    $(".selectpicker").selectpicker("refresh");

    xSplash.hide();
});

function ObjGet(action) {
    var obj = {
        action: action == null ? "inquiry" : action,
        partNo: !$("#inpPartNo").val() ? "" : $("#inpPartNo").val(),
        supplier: !$("#inpSupplierCode").val() ? "" : $("#inpSupplierCode").val(),
        store: !$("#inpStore").val() ? "" : $("#inpStore").val(),
        kanban: !$("#inpKanban").val() ? "" : $("#inpKanban").val(),
        typeOrder: !$("#inpTypeOrder").val() ? "" : $("#inpTypeOrder").val()
    }

    return obj;
}

async function Inquiry(option,action) {

    var obj = ObjGet(action);

    if (option.includes("supplier") || option == "all") {
        if (!$("#inpSupplierCode").val()) {

            await _xLib.AJAX_Get("/api/KBNMS013/GetSupplier", obj,
                function (success) {
                    if (success.status == "200") {
                        $("#inpSupplierCode").empty();
                        $("#inpSupplierCode").append(`<option value="" hidden></option>`);
                        success.data.forEach((item) => {
                            $("#inpSupplierCode").append(`<option value="${item.f_Supplier}">${item.f_Supplier}</option>`);
                        });

                        $("#inpSupplierCode").selectpicker("refresh");
                        $("#inpSupplierCode").parent().find(".filter-option-inner-inner").text("Nothing selected");
                    }
                },
                function (error) {
                    xSwal.error("Error", "Can't Get Supplier")
                }
            );
        }

    }

    if (option.includes("kanban") || option == "all") {
        if (!$("#inpKanban").val()) {

            await _xLib.AJAX_Get("/api/KBNMS013/GetKanban", obj,
                function (success) {
                    if (success.status == "200") {
                        $("#inpKanban").empty();
                        $("#inpKanban").append(`<option value="" hidden></option>`);
                        success.data.forEach((item) => {
                            $("#inpKanban").append(`<option value="${item.f_Kanban}">${item.f_Kanban}</option>`);
                        });

                        $("#inpKanban").selectpicker("refresh");
                        $("#inpKanban").parent().find(".filter-option-inner-inner").text("Nothing selected");
                    }
                },
                function (error) {
                    xSwal.error("Error", "Can't Get Kanban")
                }
            );
        }
    }

    if (option.includes("store") || option == "all") {
        if (!$("#inpStore").val()) {

            await _xLib.AJAX_Get("/api/KBNMS013/GetStore", obj,
                function (success) {
                    if (success.status == "200") {
                        $("#inpStore").empty();
                        $("#inpStore").append(`<option value="" hidden></option>`);
                        success.data.forEach((item) => {
                            $("#inpStore").append(`<option value="${item.f_Store}">${item.f_Store}</option>`);
                        });

                        $("#inpStore").selectpicker("refresh");
                        $("#inpStore").parent().find(".filter-option-inner-inner").text("Nothing selected");
                    }
                },
                function (error) {
                    xSwal.error("Error", "Can't Get Kanban")
                }
            );
        }
    }

    if (option.includes("partNo") || option == "all") {
        if (!$("#inpPartNo").val()) {

            await _xLib.AJAX_Get("/api/KBNMS013/GetPartNo", obj,
                function (success) {
                    if (success.status == "200") {
                        $("#inpPartNo").empty();
                        $("#inpPartNo").append(`<option value="" hidden></option>`);
                        success.data.forEach((item) => {
                            $("#inpPartNo").append(`<option value="${item.f_PartNo}">${item.f_PartNo}</option>`);
                        });

                        $("#inpPartNo").selectpicker("refresh");
                        $("#inpPartNo").parent().find(".filter-option-inner-inner").text("Nothing selected");
                    }
                },
                function (error) {
                    xSwal.error("Error", "Can't Get Kanban")
                }
            );
        }
    }
}

$("#inpSupplierCode").on("change", async function () {
    var obj = {
        F_Supplier_Code: $("#inpSupplierCode").val(),
        F_Store_Code: $("#inpStore").val() == null ? "" : $("#selectStore").val()
    }
    await _xLib.AJAX_Get("/api/KBNMS006/GetSupplierDetail", obj,
        function (success) {
            if (success.status == 200) {
                var data = success.data;
                $("#inpSupplierName").val(data.f_Supplier_Name);
                $("#inpCycle").val(data.f_Cycle);
            }
        },
        function (error) {
            xSwal.error("Error", "Supplier Detail Not Found");
        }
    );

    if ($("#btnNew").prop("disabled") == false) {
        return Inquiry("store,kanban,partNo", "new");
    }
    await Inquiry("store,kanban,partNo");

    obj = {
        supplier : $("#inpSupplierCode").val(),
        store : $("#inpStore").val() == null ? "" : $("#inpStore").val(),
        typeOrder: $("#inpTypeOrder").val() == null ? "" : $("#inpTypeOrder").val()
    }

    await _xLib.AJAX_Get("/api/KBNMS013/GetList", obj,
        function (success) {
            if (success.status == 200) {
                    var success = _xLib.JSONparseAndTrim(success);
                    console.table(success);
                    $("#tableMain").DataTable().clear().draw();
                    $("#tableMain").DataTable().rows.add(success.data).draw();
                    $("table thead tr th , table tbody tr td").addClass("text-center");
                }
            },
        function (error) {
            xSwal.error("Error", "Data Not Found");
        }
    );
});

$("#inpKanban").on("change", async function () {
    $("#inpKanbanRead").val($("#inpKanban").val());

    if ($("#btnNew").prop("disabled") == false) {
        return Inquiry("all", "new");
    }
    await Inquiry("all");
});

$("#inpStore").on("change", async function () {
    $("#inpStoreRead").val($("#inpStore").val());

    if ($("#btnNew").prop("disabled") == false) {
        return Inquiry("all", "new");
    }
    await Inquiry("all");
});

$("#inpPartNo").on("change", async function () {
    $("#inpPartNoRead").val($("#inpPartNo").val());

    var obj = {
        partNo: $("#inpPartNo").val(),
        supplier: $("#inpSupplierCode").val() == null ? "" : $("#inpSupplierCode").val(),
        store: $("#inpStore").val() == null ? "" : $("#inpStore").val(),
        kanban: $("#inpKanban").val() == null ? "" : $("#inpKanban").val()
    }

    await _xLib.AJAX_Get("/api/KBNMS013/GetPartNoDetail", obj,
        function (success) {
            if (success.status == 200) {
                var data = success.data;
                $("#inpPartNameRead").val(data.f_Part_nm);
            }
        },
        function (error) {
            xSwal.error("Error", "Part No Detail Not Found");
        }
    );

    if ($("#btnNew").prop("disabled") == false) {
        return Inquiry("all", "new");
    }
    await Inquiry("all");

});

$("#btnInquiry").on("click", async function () {
    await Inquiry("all");
    await $("#divDetail").find("select").prop("disabled", false);
    $("#inpStartDate, #inpEndDate").prop("readonly", false);
    $(".selectpicker").selectpicker("refresh");
    $(".filter-option-inner-inner").text("Nothing selected");

    $("#divButton").find("button").prop("disabled", true);
    $(this).prop("disabled", false);
});

$("#btnNew").on("click", async function () {

    await Inquiry("all", "new");

    $("#divDetail").find("select").prop("disabled", false);
    $("#divDetail").find("input[type='text']").prop("readonly", false);
    $(".selectpicker").selectpicker("refresh");
    $(".filter-option-inner-inner").text("Nothing selected");

    $("#divButton").find("button").prop("disabled", true);
    $(this).prop("disabled", false);

    $("#inpStartDate").val(moment().format("DD/MM/YYYY"));
    $("#inpEndDate").val("31/12/2999");
});

$("#btnUpdate").on("click", async function () {

    await Inquiry("all");

    $("#divDetail").find("select").prop("disabled", false);
    //$("#divDetail").find("input[type='text']").prop("readonly", false);
    $("#inpEndDate ,#inpPDSGroup").prop("readonly", false);
    $(".selectpicker").selectpicker("refresh");
    $(".filter-option-inner-inner").text("Nothing selected");

    $("#divButton").find("button").prop("disabled", true);
    $(this).prop("disabled", false);
});

$("#btnDelete").on("click", async function () {

    await Inquiry("all");

    $("#divDetail").find("select").prop("disabled", false);
    //$("#divDetail").find("input[type='text']").prop("readonly", false);
    //$("#inpStartDate, #inpEndDate ,#inpPDSGroup").prop("readonly", false);
    $(".selectpicker").selectpicker("refresh");
    $(".filter-option-inner-inner").text("Nothing selected");

    $("#divButton").find("button").prop("disabled", true);
    $(this).prop("disabled", false);
});

$("#btnCancel").on("click", async function () {

    //$("#divDetail").find("select").val("");
    $("#divDetail").find("select").selectpicker('val', "");
    $("#divDetail").find("input[type='text']").prop("readonly", true);
    $("#divDetail").find("input[type='text']").val("");
    $("#tableMain").DataTable().clear().draw();
    $(".selectpicker").selectpicker("render");
    $("#divDetail").find("select").prop("disabled", true);
    $(".selectpicker").selectpicker("refresh");
    $(".filter-option-inner-inner").text("Nothing selected");
    $("#divButton").find("button").prop("disabled", false);


});

$(document).on("click","#tableMain tbody tr", function () {
    if($("#btnUpdate").prop("disabled") == false || $("#btnDelete").prop("disabled") == false){
        var data = $("#tableMain").DataTable().row(this).data();
        console.log(data);
        $("#inpSupplierCode").val(data.F_Supplier_Code).selectpicker("refresh");
        $("#inpPartNo").val(data.F_Part_No).selectpicker("refresh");
        $("#inpPartNoRead").val(data.F_Part_No);
        $("#inpPartNameRead").val(data.F_Part_nm);
        $("#inpKanban").val(data.F_Kanban_No).selectpicker("refresh");
        $("#inpKanbanRead").val(data.F_Kanban_No);
        $("#inpStore").val(data.F_Store_Code).selectpicker("refresh");
        $("#inpStoreRead").val(data.F_Store_Code);
        $("#inpDockCode").val(data.F_Dock_Code);
        $("#inpStartDate").val(data.F_Start_Date);
        $("#inpEndDate").val(data.F_End_Date);
        $("#inpTypeOrder").val(data.F_Type_Order).selectpicker("refresh");
        $("#inpCycle").val(data.F_Cycle);
        $("#inpPDSGroup").val(data.F_PDS_Group);
    }
});

$("#btnSave").on("click", async function () {

    var obj = {
        F_Plant: _xLib.GetCookie("plantCode"),
        F_Supplier_Cd: $("#inpSupplierCode").val().split("-")[0],
        F_Supplier_Plant: $("#inpSupplierCode").val().split("-")[1],
        F_Part_No: $("#inpPartNo").val().split("-")[0],
        F_Ruibetsu: $("#inpPartNo").val().split("-")[1],
        F_Kanban_No: $("#inpKanban").val(),
        F_Store_Code: $("#inpStore").val(),
        F_Start_Date: moment($("#inpStartDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        F_End_Date: moment($("#inpEndDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        F_Type_Order: $("#inpTypeOrder").val(),
        F_Cycle: $("#inpCycle").val().replaceAll("-", ""),
        F_PDS_Group: $("#inpPDSGroup").val(),
        F_Check_Shift: 0,
        F_Last_Check: "   ",
        F_Next_Check: "   ",
    }

    var _action = $("#btnNew").prop("disabled") == false ? "new" : $("#btnUpdate").prop("disabled") == false ? "update" : "delete";

    _xLib.AJAX_Post("/api/KBNMS013/save?action=" + _action, JSON.stringify(obj),
        function (success) {
            if (success.status == 200) {
                xSwal.success("Success", "Data Saved");
                $("#btnCancel").click();
            }
        },
        function (error) {
            xSwal.error("Error", error.responseJSON.message);
        }
    );

});
