$(document).ready(async function () {

    $("#tblMaster").DataTable({
        "processing": true,
        "serverSide": false,
        width: '100%',
        paging: false,
        sorting: false,
        searching: false,
        scrollX: "100%", // "300px""
        scrollY: "300px", // "300px"
        scrollCollapse: true,
        ordering: false,
        "columns": [
            { title: "Supplier Cd", data: "F_Supplier_Code", },
            { title: "Short Name", data: "F_Cycle", },
            { title: "Part No", data: "F_Kanban_No", },
            { title: "Name", data: "F_Store_Code", },
            { title: "Store Cd", data: "F_Part_No" },
            { title: "KB", data: "F_Start_Date" },
            { title: "Box/Qty", data: "F_End_Date", },
            { title: "Cycle", data: "F_Type_Order", },
            { title: "Day1", data: "F_Dock_Code", },
            { title: "Day2", data: "F_PDS_Group", },
            { title: "Day3", data: "F_Dock_Code", },
            { title: "Day4", data: "F_PDS_Group", },
            { title: "Day5", data: "F_Dock_Code", },
            { title: "Day6", data: "F_PDS_Group", },
            { title: "Day7", data: "F_Dock_Code", },
            { title: "Day8", data: "F_PDS_Group", },
            { title: "Day9", data: "F_Dock_Code", },
            { title: "Day10", data: "F_PDS_Group", },
            { title: "Day11", data: "F_Dock_Code", },
            { title: "Day12", data: "F_PDS_Group", },
            { title: "Day13", data: "F_Dock_Code", },
            { title: "Day14", data: "F_PDS_Group", },
            { title: "Day15", data: "F_Dock_Code", },
            { title: "Day16", data: "F_PDS_Group", },
            { title: "Day17", data: "F_Dock_Code", },
            { title: "Day18", data: "F_PDS_Group", },
            { title: "Day19", data: "F_Dock_Code", },
            { title: "Day20", data: "F_PDS_Group", },
            { title: "Day21", data: "F_Dock_Code", },
            { title: "Day22", data: "F_PDS_Group", },
            { title: "Day23", data: "F_Dock_Code", },
            { title: "Day24", data: "F_PDS_Group", },
            { title: "Day25", data: "F_Dock_Code", },
            { title: "Day26", data: "F_PDS_Group", },
            { title: "Day27", data: "F_Dock_Code", },
            { title: "Day28", data: "F_PDS_Group", },
            { title: "Day29", data: "F_Dock_Code", },
            { title: "Day30", data: "F_PDS_Group", },
            { title: "Day31", data: "F_Dock_Code", },
            { title: "M", data: "F_PDS_Group", },
            { title: "M1", data: "F_PDS_Group", },
            { title: "M2", data: "F_PDS_Group", },
            { title: "M3", data: "F_PDS_Group", },

        ],
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


    if ($('#TypeForImportConditionDetail').prop('checked')) $('#fldConditionSupplier').prop('disabled', 'disabled');
    if ($('#TypeForImportConditionSupplier').prop('checked')) $('#fldConditionDetail').prop('disabled', 'disabled');


    xAjax.onCheck('#TypeForImportConditionDetail', function () {
        console.log('TypeForImportConditionDetail');
        $('#fldConditionSupplier').prop('disabled', 'disabled');
        $('#fldConditionDetail').prop('disabled', false);
    });
    xAjax.onCheck('#TypeForImportConditionSupplier', function () {
        console.log('TypeForImportConditionSupplier');
        $('#fldConditionSupplier').prop('disabled', false);
        $('#fldConditionDetail').prop('disabled', 'disabled');
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

    xSplash.hide();
});