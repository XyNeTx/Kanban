$(document).ready(async function () {

    $("#tblMaster").DataTable({
        "processing": true,
        "serverSide": false,
        width: '100%',
        paging: false,
        sorting: false,
        searching: false,
        scrollX: true,
        scrollY: "300px", // "300px"
        scrollCollapse: true,
        ordering: false,
        "columns": [
            { title: "Supplier Cd", data: "F_Supplier_Code", },
            { title: "Short Name", data: "F_Short_Name", },
            { title: "Part No", data: "F_Part_No", width: "20%" },
            { title: "Name", data: "F_Part_Name", },
            { title: "Store Cd", data: "F_Store_cd" },
            { title: "KB", data: "F_Sebango" },
            { title: "Box/Qty", data: "F_Delivery_qty", },
            { title: "Cycle", data: "F_cycle_supply", },
            { title: "Day1", data: "F_Amount_MD1", },
            { title: "Day2", data: "F_Amount_MD2", },
            { title: "Day3", data: "F_Amount_MD3", },
            { title: "Day4", data: "F_Amount_MD4", },
            { title: "Day5", data: "F_Amount_MD5", },
            { title: "Day6", data: "F_Amount_MD6", },
            { title: "Day7", data: "F_Amount_MD7", },
            { title: "Day8", data: "F_Amount_MD8", },
            { title: "Day9", data: "F_Amount_MD9", },
            { title: "Day10", data: "F_Amount_MD10", },
            { title: "Day11", data: "F_Amount_MD11", },
            { title: "Day12", data: "F_Amount_MD12", },
            { title: "Day13", data: "F_Amount_MD13", },
            { title: "Day14", data: "F_Amount_MD14", },
            { title: "Day15", data: "F_Amount_MD15", },
            { title: "Day16", data: "F_Amount_MD16", },
            { title: "Day17", data: "F_Amount_MD17", },
            { title: "Day18", data: "F_Amount_MD18", },
            { title: "Day19", data: "F_Amount_MD19", },
            { title: "Day20", data: "F_Amount_MD20", },
            { title: "Day21", data: "F_Amount_MD21", },
            { title: "Day22", data: "F_Amount_MD22", },
            { title: "Day23", data: "F_Amount_MD23", },
            { title: "Day24", data: "F_Amount_MD24", },
            { title: "Day25", data: "F_Amount_MD25", },
            { title: "Day26", data: "F_Amount_MD26", },
            { title: "Day27", data: "F_Amount_MD27", },
            { title: "Day28", data: "F_Amount_MD28", },
            { title: "Day29", data: "F_Amount_MD29", },
            { title: "Day30", data: "F_Amount_MD30", },
            { title: "Day31", data: "F_Amount_MD31", },
            { title: "M", data: "F_Amount_M", },
            { title: "M1", data: "F_Amount_M1", },
            { title: "M2", data: "F_Amount_M2", },
            { title: "M3", data: "F_Amount_M3", },

        ],
        fixedColumns: {
            left : 7
        },

        order: [[0, "asc"]]
    });

    $("#ImportDate").datepicker({
        format: "dd/mm/yyyy",
        showRightIcon: false,
        autoclose: true,
    });
    $("#ImportMonth").datepicker({
        format: "mm/yyyy",
        showRightIcon: false,
        autoclose: true,
    });
    $("#ConditionMonth").datepicker({
        format: "mm/yyyy",
        showRightIcon: false,
        autoclose: true,
    });


    await _xLib.AJAX_Get("/api/KBNIM012/Onload", null,
        function (success) {
            if (success.status == "200") {
                console.log(success);
                $("#ImportVersion").val(success.data.maxVersion)
                $("#ImportRevision").val(success.data.maxPO.substring(6, 9))
                $("#ImportMonth").val(moment(success.data.maxPO.substring(0, 6), "YYYYMM").format("MM/YYYY"));
                $("#ImportDate").val(moment(_xLib.GetCookie("processDate").substring(0,10), "YYYY-MM-DD").format("DD/MM/YYYY"));

                $("#ConditionRevision").val(success.data.revisionNo);
                $("#ConditionVersion").val(success.data.version == "C" ? "CONFIRM" : "TENTATIVE");
                $("#ConditionMonth").val(moment(success.data.productionDate, "YYYYMM").format("MM/YYYY"));

                $("#divCurrentTxt").text(`Current Data Interface : Production Month : ${moment(success.data.productionDate, "YYYYMM").format("MM/YYYY") } 
                Version : ${success.data.version == "C" ? "CONFIRM" : "TENTATIVE"} Revision No. : ${success.data.revisionNo}`);
            }

        },
        function (error) {
            console.error(data);
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );

    await GetListData();


    await GetSupplier();
    await GetKanban();
    await GetStore();
    await GetPart();
    await GetStore2nd();


    xSplash.hide();
});

$("input[name='TypeForImport']").change(function () {
    if ($(this).attr("id") == "TypeForImportConditionSupplier") {
        $("#selectSupplier").prop("disabled", true);
        $("#selectSupplier2nd").prop("disabled", false);
        $("#selectStore").prop("disabled", true);
        $("#selectStore2nd").prop("disabled", false);
        $("#selectKanban").prop("disabled", true);
        $("#selectPart").prop("disabled", true);

        $("#selectSupplier").selectpicker('val', '');
        $("#selectStore").selectpicker('val', '');
        $("#selectKanban").selectpicker('val', '');
        $("#selectPart").selectpicker('val', '');

        $("#readSupplier").val('');
        $("#readPart").val('');

        $(".selectpicker").selectpicker('render');
        $(".selectpicker").selectpicker('refresh');
    }
    else {
        $("#selectSupplier").prop("disabled", false);
        $("#selectSupplier2nd").prop("disabled", true);
        $("#selectStore").prop("disabled", false);
        $("#selectStore2nd").prop("disabled", true);
        $("#selectKanban").prop("disabled", false);
        $("#selectPart").prop("disabled", false);

        $("#selectSupplier2nd").selectpicker('val', '');
        $("#selectStore2nd").selectpicker('val', '');

        $("#readSupplier2nd").val('');

        $(".selectpicker").selectpicker('render');
        $(".selectpicker").selectpicker('refresh');
    }

    $(".filter-option-inner-inner").text("Nothing Selected");
});

function ObjGet() {
    return {
        supplier: $("#selectSupplier").val() ? $("#selectSupplier").val() : $("#selectSupplier2nd").val() ? $("#selectSupplier2nd").val() : "",
        kanban: $("#selectKanban").val() ? $("#selectKanban").val() : "",
        store: $("#selectStore").val() ? $("#selectStore").val() : $("#selectStore2nd").val() ? $("#selectStore2nd").val() : "",
        part: $("#selectPart").val() ? $("#selectPart").val() : "",
    }
}

function GetSupplier() {
    var obj = ObjGet();
    _xLib.AJAX_Get("/api/KBNIM012/GetSupplier", obj,
        function (success) {
            if (success.status == "200") {
                success = _xLib.JSONparseAndTrim(success);
                console.log(success);
                $("#selectSupplier").empty();
                $("#selectSupplier2nd").empty();
                $("#selectSupplier").append(`<option value="" hidden></option>`);
                $("#selectSupplier2nd").append(`<option value="" hidden></option>`);

                success.data.forEach(function (item) {
                    $("#selectSupplier").append(`<option value="${item.F_Supplier}">${item.F_Supplier}</option>`);
                    $("#selectSupplier2nd").append(`<option value="${item.F_Supplier}">${item.F_Supplier}</option>`);
                });

                $("#selectSupplier").selectpicker('refresh');
                $("#selectSupplier2nd").selectpicker('refresh');
                $("#selectSupplier").parent().find(".filter-option-inner-inner").text("Nothing Selected");
                $("#selectSupplier2nd").parent().find(".filter-option-inner-inner").text("Nothing Selected");
            }

        },
        function (error) {
            console.error(data);
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );
}

function GetKanban () {
    var obj = ObjGet();

    _xLib.AJAX_Get("/api/KBNIM012/GetKanban", obj,
        function (success) {
            if (success.status == "200") {
                success = _xLib.JSONparseAndTrim(success);
                console.log(success);
                $("#selectKanban").empty();
                $("#selectKanban").append(`<option value="" hidden></option>`);
                success.data.forEach(function (item) {
                    $("#selectKanban").append(`<option value="${item.F_sebango}">${item.F_sebango}</option>`);
                });

                $("#selectKanban").selectpicker('refresh');
                $("#selectKanban").parent().find(".filter-option-inner-inner").text("Nothing Selected");
            }

        },
        function (error) {
            console.error(data);
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );
}

function GetStore() {
    var obj = ObjGet();

    _xLib.AJAX_Get("/api/KBNIM012/GetStore", obj,
        function (success) {
            if (success.status == "200") {
                success = _xLib.JSONparseAndTrim(success);
                console.log(success);

                $("#selectStore").empty();
                $("#selectStore").append(`<option value="" hidden></option>`);
                success.data.forEach(function (item) {
                    $("#selectStore").append(`<option value="${item.F_Store_cd}">${item.F_Store_cd}</option>`);
                });
                $("#selectStore").selectpicker('refresh');
                $("#selectStore").parent().find(".filter-option-inner-inner").text("Nothing Selected");
            }

        },
        function (error) {
            console.error(data);
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );
}

function GetStore2nd() {
    var obj = ObjGet();

    _xLib.AJAX_Get("/api/KBNIM012/GetStore2nd", obj,
        function (success) {
            if (success.status == "200") {
                success = _xLib.JSONparseAndTrim(success);
                console.log(success);

                $("#selectStore2nd").empty();
                $("#selectStore2nd").append(`<option value="" hidden></option>`);
                success.data.forEach(function (item) {
                    $("#selectStore2nd").append(`<option value="${item.F_Store_cd}">${item.F_Store_cd}</option>`);
                });
                $("#selectStore2nd").selectpicker('refresh');
                $("#selectStore2nd").parent().find(".filter-option-inner-inner").text("Nothing Selected");
            }

        },
        function (error) {
            console.error(data);
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );
}

function GetPart() {
    var obj = ObjGet();

    _xLib.AJAX_Get("/api/KBNIM012/GetPart", obj,
        function (success) {
            if (success.status == "200") {
                success = _xLib.JSONparseAndTrim(success);
                console.log(success);
                $("#selectPart").empty();
                $("#selectPart").append(`<option value="" hidden></option>`);
                success.data.forEach(function (item) {
                    $("#selectPart").append(`<option value="${item.F_PART_NO}">${item.F_PART_NO}</option>`);
                });
                $("#selectPart").selectpicker('refresh');
                $("#selectPart").parent().find(".filter-option-inner-inner").text("Nothing Selected");
            }

        },
        function (error) {
            console.error(data);
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );
}

async function GetListData() {
    var obj = ObjGet();

    obj.YM = moment($("#ConditionMonth").val(), "MM/YYYY").format("YYYYMM");
    obj.Rev = $("#ConditionRevision").val();

    await _xLib.AJAX_Get("/api/KBNIM012/GetListData", obj,
        function (success) {
            if (success.status == "200") {
                success = _xLib.JSONparseAndTrim(success);
                console.log(success);

                $("#tblMaster").DataTable().clear().rows.add(success.data).draw();
            }

        },
        function (error) {
            console.error(data);
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );


    $("table thead tr th").addClass("text-center");
    $("table tbody tr td").addClass("text-center p-2");

    $("#tblMaster").DataTable().columns.adjust().draw();
}

$("#btnSearch").click(function () {
    GetListData();
});


$("#selectSupplier").change(function () {
    var obj = ObjGet();

    _xLib.AJAX_Get("/api/KBNIM012/GetSupplierDetail", obj,
        function (success) {
            if (success.status == "200") {
                console.log(success);
                $("#readSupplier").val(success.data);
            }

        },
        function (error) {
            console.error(error);
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );

    GetKanban();
    GetStore();
    GetPart();
});

$("#selectSupplier2nd").change(function () {
    var obj = ObjGet();

    _xLib.AJAX_Get("/api/KBNIM012/GetSupplierDetail", obj,
        function (success) {
            if (success.status == "200") {
                console.log(success);
                $("#readSupplier2nd").val(success.data);
            }

        },
        function (error) {
            console.error(error);
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );

    GetStore2nd();
});

$("#selectKanban").change(function () {
    var obj = ObjGet();

    _xLib.AJAX_Get("/api/KBNIM012/GetKanbanDetail", obj,
        function (success) {
            if (success.status == "200") {
                console.log(success);
                $("#readKanban").val(success.data);
            }

        },
        function (error) {
            console.error(error);
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );

    GetStore();
    GetPart();
});

$("#selectStore").change(function () {
    var obj = ObjGet();

    _xLib.AJAX_Get("/api/KBNIM012/GetStoreDetail", obj,
        function (success) {
            if (success.status == "200") {
                console.log(success);
                $("#readStore").val(success.data);
            }

        },
        function (error) {
            console.error(error);
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );

    GetPart();
});

$("#selectPart").change(function () {
    var obj = ObjGet();

    _xLib.AJAX_Get("/api/KBNIM012/GetPartDetail", obj,
        function (success) {
            if (success.status == "200") {
                console.log(success);
                $("#readPart").val(success.data);
            }

        },
        function (error) {
            console.error(error);
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );
});

$("#btnInterface").click(async function () {

    await _xLib.AJAX_Get("/api/KBNIM012/CheckBeforeInterface", null,
        function (success) {
            if (success.status == "200") {
                console.log(success);
                //xSwal.success("Success !!", success.message);
                return InterfaceData();
            }

        },
        function (error) {
            console.error(error);
            return xSwal.error("Error !!", error.responseJSON.message);
        }
    );
});

$("#btnNextMonth").click(async function () {

    await _xLib.AJAX_Get("/api/KBNIM012/CheckBeforeInterface", null,
        function (success) {
            if (success.status == "200") {
                CheckInterfaceN1();
            }

        },
        function (error) {
            console.error(error);
            return xSwal.error("Error !!", error.responseJSON.message);
        }
    );
});


function InterfaceData() {

    var obj = {
        YM : moment($("#ImportMonth").val(), "MM/YYYY").format("YYYYMM"),
        Rev: $("#ImportRevision").val(),
        Ver: $("#ImportVersion").val(),
        YM_L: moment($("#ConditionMonth").val(), "MM/YYYY").format("YYYYMM"),
        Rev_L: $("#ConditionRevision").val(),
    }

    _xLib.AJAX_Post("/api/KBNIM012/Interface", JSON.stringify(obj),
        function (success) {
            if (success.status == "200") {
                console.log(success);
                GetListData();
                xSwal.success("Success !!", success.message);
            }

        },
        function (error) {
            console.error(error);
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );
}


function CheckInterfaceN1() {
    _xLib.AJAX_Post("/api/KBNIM012/CheckInterfaceN1", null,
        function (success) {
            if (success.status == "200") {

                if (success.confirm == "Yes") {
                    if (xSwal.confirm("Are you sure ?", success.message)) {
                        InterfaceN1();
                    }

                }
            }

        },
        function (error) {
            console.error(error);
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );
}


function InterfaceN1() {
    _xLib.AJAX_Post("/api/KBNIM012/InterfaceN1", null,
        function (success) {
            if (success.status == "200") {
                console.log(success);
                xSwal.success("Success !!", success.message);
            }

        },
        function (error) {
            console.error(error);
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );
}