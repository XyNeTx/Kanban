$(document).ready(function () {

    $("#tableMaster").DataTable({
        columns: [
            { title: "Supplier Code", data: "F_Supplier_Code" },
            { title: "Part No.", data: "F_Part_No" },
            { title: "Kanban No.", data: "F_Kanban_No" },
            { title: "Store Code", data: "F_Store_Code" },
            { title: "Qty/Pack", data: "F_Qty" },
            { title: "Actual KB", data: "F_BL_Kanban" },
            { title: "Actual PCS", data: "F_BL_PCS" },
            { title: "Total Actual", data: "F_Total_Actual" },
            { title: "Total System", data: "F_Total_System" }
        ],
        order: [[0, "asc"]],
        width: '100%',
        paging: false,
        scrollCollapse: true,
        processing: false,
        serverSide: false,
        scrollX: false,
        scrollY: '350px',
        info: false,
        searching: false,

    });

    $("#divButton").find("button").prop("disabled", false);

    xSplash.hide();
});

$("#buttonInq, #buttonNew, #buttonUpd, #buttonDel").click(function (e) {
    var id = $(this).attr("id");
    //console.log(id);
    $("#formSearchData").find("input, select").prop("disabled", false);
    if (id == "buttonNew") {

        _xLib.AJAX_Get('/api/KBNMS005S/GetSupplierCode', { IsCmdNew: true },
            function (success) {
                $("#SelectSupplierCode").empty();
                $("#SelectSupplierCode").append('<option value="" hidden></option>');
                success.data.forEach(function (item) {
                    $("#SelectSupplierCode").append('<option value="' + item.f_Supplier_Code + '">' + item.f_Supplier_Code + '</option>');
                });
            },
            function (error) {
                xSwal.error("Error !!", error.responseJSON.message);
            }
        );

        return _xLib.AJAX_Get('/api/KBNMS005S/GetStoreCode', { IsCmdNew: true },
            function (success) {
                $("#SelectStoreCode").empty();
                $("#SelectStoreCode").append('<option value="" hidden></option>');
                success.data.forEach(function (item) {
                    $("#SelectStoreCode").append('<option value="' + item.f_Store_Code + '">' + item.f_Store_Code + '</option>');
                });
            },
            function (error) {
                xSwal.error("Error !!", error.responseJSON.message);
            }
        );
    }

    _xLib.AJAX_Get('/api/KBNMS005S/GetSupplierCode', { IsCmdNew: false },
        function (success) {
            $("#SelectSupplierCode").empty();
            $("#SelectSupplierCode").append('<option value="" hidden></option>');
            success.data.forEach(function (item) {
                $("#SelectSupplierCode").append('<option value="' + item.f_Supplier_Code + '">' + item.f_Supplier_Code + '</option>');
            });
        },
        function (error) {
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );
    return _xLib.AJAX_Get('/api/KBNMS005S/GetStoreCode', { IsCmdNew: true },
        function (success) {
            $("#SelectStoreCode").empty();
            $("#SelectStoreCode").append('<option value="" hidden></option>');
            success.data.forEach(function (item) {
                $("#SelectStoreCode").append('<option value="' + item.f_Store_Code + '">' + item.f_Store_Code + '</option>');
            });
        },
        function (error) {
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );
});