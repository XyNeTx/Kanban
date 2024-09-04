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
        action: action == null ? "Inquiry" : action,
        partNo: !$("#inpPartNo").val() ? "" : $("#inpPartNo").val(),
        supplier: !$("#inpSupplierCode").val() ? "" : $("#inpSupplierCode").val(),
        store: !$("#inpStore").val() ? "" : $("#inpStore").val(),
        kanban: !$("#inpKanban").val() ? "" : $("#inpKanban").val(),
        typeOrder: !$("#inpTypeOrder").val() ? "" : $("#inpTypeOrder").val()
    }

    return obj;
}

async function Inquiry(option) {

    var obj = ObjGet();

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

    await Inquiry("all");
});

$("#inpStore").on("change", async function () {
    $("#inpStoreRead").val($("#inpStore").val());

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

    await Inquiry("all");

});

$("#btnInquiry").on("click", async function () {
    await Inquiry("all");
    await $("#divDetail").find("select").prop("disabled", false);
    $("#inpStartDate, #inpEndDate").prop("readonly", false);
    $(".selectpicker").selectpicker("refresh");
    $(".filter-option-inner-inner").text("Nothing selected");
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



});
