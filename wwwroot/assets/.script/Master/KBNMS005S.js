let _cmd = "";
$(document).ready(async function () {

    await $("#tableMaster").DataTable({
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
        scrollX: false,
        scrollY: '350px',
        scrollCollapse: true,
        select: true,
        info: false,
        searching: false,
    });

    $("#divButton").find("button").prop("disabled", false);

    await _xLib.AJAX_Get('/api/KBNMS005S/GetDateAndShift', "",
        function (success) {
            if (success.status == "200") {
                let data = JSON.parse(success.data);
                let date = data[0].F_Date.slice(0, 4) + "-" + data[0].F_Date.slice(4, 6) + "-" + data[0].F_Date.slice(6, 8);
                $("#SelectShift").val(data[0].F_Shift);
                $("#inputDate").val(date);
            }
        },
        function (error) {
            xSwal.error("Error !!", error.responseJSON.message);
        }
    )
    $(".dataTable  thead tr th").addClass("text-center");

    xSplash.hide();
});
$("#buttonInq, #buttonNew, #buttonUpd, #buttonDel").click(async function (e) {
    var id = $(this).attr("id");
    //console.log(id);
    $("#formSearchData").find("input, select").prop("disabled", false);
    if (id == "buttonNew") {
        _cmd = "New";
        await _xLib.AJAX_Get('/api/KBNMS005S/GetSupplierCode', { IsCmdNew: true },
            function (success) {
                if (success.status == "200") {
                    $("#SelectSupplierCode").empty();
                    $("#SelectSupplierCode").append('<option value="" hidden></option>');
                    success.data.forEach(function (item) {
                        $("#SelectSupplierCode").append('<option value="' + item.f_Supplier_Code + '">' + item.f_Supplier_Code + '</option>');
                    });
                }
            },
            function (error) {
                xSwal.error("Error !!", error.responseJSON.message);
            }
        );
        return GetStoreCode();
    }
    _cmd = id.replace("button", "");
    await _xLib.AJAX_Get('/api/KBNMS005S/GetSupplierCode', { IsCmdNew: false },
        function (success) {
            if (success.status == "200") {
                $("#SelectSupplierCode").empty();
                $("#SelectSupplierCode").append('<option value="" hidden></option>');
                success.data.forEach(function (item) {
                    $("#SelectSupplierCode").append('<option value="' + item.f_Supplier_Code + '">' + item.f_Supplier_Code + '</option>');
                });
            }
        },
        function (error) {
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );
    return GetStoreCode();
    
});

async function GetStoreCode() {
    if (_cmd == "New") {
        return _xLib.AJAX_Get('/api/KBNMS005S/GetStoreCode', { IsCmdNew: true, F_Supplier_Code: $("#SelectSupplierCode").val() },
            function (success) {
                if (success.status == "200") {
                    $("#SelectStoreCode").empty();
                    $("#SelectStoreCode").append('<option value="" hidden></option>');
                    success.data.forEach(function (item) {
                        $("#SelectStoreCode").append('<option value="' + item.f_Store_Code + '">' + item.f_Store_Code + '</option>');
                    });
                }
            },
            function (error) {
                xSwal.error("Error !!", error.responseJSON.message);
            }
        );
    }
    return _xLib.AJAX_Get('/api/KBNMS005S/GetStoreCode', { IsCmdNew: false, F_Supplier_Code: $("#SelectSupplierCode").val() },
        function (success) {
            if (success.status == "200") {
                $("#SelectStoreCode").empty();
                $("#SelectStoreCode").append('<option value="" hidden></option>');
                success.data.forEach(function (item) {
                    $("#SelectStoreCode").append('<option value="' + item.f_Store_Code + '">' + item.f_Store_Code + '</option>');
                });
            }
        },
        function (error) {
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );
}

$("#SelectSupplierCode").change(function () {
    return GetStoreCode();
});

$("#buttonShow").click(async function () {
    let data = {
        IsCmdNew : _cmd == "New" ? true : false,
        F_Supplier_Code: $("#SelectSupplierCode").val(),
        F_Store_Code: $("#SelectStoreCode").val(),
        F_Date: $("#inputDate").val().replaceAll(/-/g, ""),
        F_Shift: $("#SelectShift").val()
    }
    await _xLib.AJAX_Get('/api/KBNMS005S/SearchClick', data,
        function (success) {
            if (success.status == "200") {
                success.data = JSON.parse(success.data);
                const table = $("#tableMaster").DataTable();
                table.scrollY = "300px";
                table.clear().rows.add(success.data).draw();
            }
        },
        function (error) {
            return xSwal.error("Error !!", error.responseJSON.message);
        }
    );
});

$(document).on("click", "#tableMaster tbody tr td", function () {
    var _columnIndex = $("#tableMaster").DataTable().cell(this).index().column;
    if ($(this).find("input").length > 0) return;
    if (_columnIndex == 5 || _columnIndex == 6) {
        let value = $("#tableMaster").DataTable().cell(this).data();
        //console.log("value", value);
        $(this).empty();
        $(this).append('<input type="number" class="form-control" value="' + value + '">');
        $(this).find("input").focus();
    } 
});
$(document).on("blur", "#tableMaster tbody tr td", async function () {
    let _columnIndex = $("#tableMaster").DataTable().cell(this).index().column;
    let _rowIndex = $("#tableMaster").DataTable().cell(this).index().row;
    if (_columnIndex == 5 || _columnIndex == 6) {
        let _newValue = $(this).find("input").val();
        await $("#tableMaster").DataTable().cell({ row: _rowIndex, column: _columnIndex }).data(_newValue).draw();

        let QtyPack = $("#tableMaster").DataTable().cell({ row: _rowIndex, column: 4 }).data();
        let ActualKB = $("#tableMaster").DataTable().cell({ row: _rowIndex, column: 5 }).data();
        let ActualPCS = $("#tableMaster").DataTable().cell({ row: _rowIndex, column: 6 }).data();

        //console.log(QtyPack, ActualKB, ActualPCS);

        let TotalActual = (parseInt(QtyPack) * parseInt(ActualKB)) + parseInt(ActualPCS);

        //console.log(TotalActual);

        await $("#tableMaster").DataTable().cell({ row: _rowIndex, column: 7 }).data(TotalActual).draw();
    }
});

$("#buttonSave").click(async function () {
    var arrayObj = $("#tableMaster").DataTable().rows().data().toArray();

    for (var each of arrayObj) {
        arrayObj[arrayObj.indexOf(each)].F_Date = $("#inputDate").val().replaceAll(/-/g, "");
        arrayObj[arrayObj.indexOf(each)].F_Shift = $("#SelectShift").val();
        arrayObj[arrayObj.indexOf(each)].F_Sup_Cd = arrayObj[arrayObj.indexOf(each)].F_Supplier_Code.split("-")[0];
        arrayObj[arrayObj.indexOf(each)].F_Sup_Plant = arrayObj[arrayObj.indexOf(each)].F_Supplier_Code.split("-")[1];
        arrayObj[arrayObj.indexOf(each)].F_Ruibetsu = arrayObj[arrayObj.indexOf(each)].F_Part_No.split("-")[1];
        arrayObj[arrayObj.indexOf(each)].F_Part_No = arrayObj[arrayObj.indexOf(each)].F_Part_No.split("-")[0];
        arrayObj[arrayObj.indexOf(each)].F_BL_PCS = parseInt(arrayObj[arrayObj.indexOf(each)].F_BL_PCS);
        arrayObj[arrayObj.indexOf(each)].F_BL_Kanban = parseInt(arrayObj[arrayObj.indexOf(each)].F_BL_Kanban);
    }
    _xLib.AJAX_Post('/api/KBNMS005S/Save', JSON.stringify(arrayObj),
        function (success) {
            if (success.status == "200") {
                xSwal.success("Success !!", success.message);
                $("#tableMaster").DataTable().clear().draw();
            }
        },
        function (error) {
            xSwal.error("Error !!", error.responseJSON.message);
        }
    );
});