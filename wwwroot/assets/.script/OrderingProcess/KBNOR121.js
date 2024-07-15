$(document).ready(async function () {
    var text = $(".nav-right li span").text();
    var date = text.split("Date : ")[1].trim().split(" ")[0];
    var plant = text.split("Plant : ")[1].split(" ")[0];
    var shift = text.split("Shift : ")[1].split(" ")[0];
    shift == 1 ? shift = "1 - Day Shift" : shift = "2 - Night Shift";
    $("#inputPlant").val(plant);
    $("#inputProcessDateFor").val(date);
    $("#inputProcessShiftFor").val(shift);

    $("#tableMaster").DataTable({
        "paging": false,
        "info": false,
        "searching": false,
        "ordering": false,
        "autoWidth": false,
        "scrollX": true
    });

    await _xLib.AJAX_Get('/api/KBNOR121/OnLoad', { Shift: shift });

    await _xLib.AJAX_Get('/api/KBNOR121/SupplierDropDown', '', function (result) {
        result = _xLib.JSONparseAndTrim(result);
        $("#inputSupplier").empty();
        $("#inputSupplier").append("<option value='' hidden></option>");
        result.data.forEach(function (item) {
            $("#inputSupplier").append(new Option(item.F_Supplier, item.F_Supplier));
        });

    });

    $("thead tr th").each(function () {
        $(this).addClass("text-center");
    });


    xSplash.hide();
});

$("#btnEditNews").click(function () {
    var textArea = document.getElementById("txtNews").innerHTML;
    console.log(textArea);
});

$("#inputSupplier").change(function () {
    var supplier = $(this).val();
    _xLib.AJAX_Get('/api/KBNOR121/KanbanDropDown', { Supplier: supplier }, function (result) {
        result = _xLib.JSONparseAndTrim(result);

        $("#inputKanban").empty();
        $("#inputKanban").append("<option value='' hidden></option>");
        $("#inputKanbanTo").empty();
        $("#inputKanbanTo").append("<option value='' hidden></option>");
        result.data.forEach(function (item) {
            $("#inputKanban").append(new Option(item.F_Kanban_No, item.F_Kanban_No));
            $("#inputKanbanTo").append(new Option(item.F_Kanban_No, item.F_Kanban_No));
        });
    });
});

$("#inputKanban").change(function () {
    _xLib.AJAX_Get('/api/KBNOR121/StoreDropDown', '', function (result) {
        result = _xLib.JSONparseAndTrim(result);

        $("#inputStore").empty();
        $("#inputStore").append("<option value='' hidden></option>");
        $("#inputStoreTo").empty();
        $("#inputStoreTo").append("<option value='' hidden></option>");
        result.data.forEach(function (item) {
            $("#inputStore").append(new Option(item.F_Store_CD, item.F_Store_CD));
            $("#inputStoreTo").append(new Option(item.F_Store_CD, item.F_Store_CD));
        });
    });
});

$("#inputStore , #inputStoreTo").change(function () {
    var storeFrom = $(this).val();
    var storeTo = $("#inputStoreTo").val();

    _xLib.AJAX_Get('/api/KBNOR121/PartNoDropDown', { Store_Cd_From: storeFrom, Store_Cd_To: storeTo }, function (result) {
        result = _xLib.JSONparseAndTrim(result);

        $("#inputPart").empty();
        $("#inputPart").append("<option value='' hidden></option>");
        $("#inputPartTo").empty();
        $("#inputPartTo").append("<option value='' hidden></option>");
        result.data.forEach(function (item) {
            $("#inputPart").append(new Option(item.F_Part_No, item.F_Part_No));
            $("#inputPartTo").append(new Option(item.F_Part_No, item.F_Part_No));
        });
    });
});

$("#btnPreview").click(function () {
    var obj = {
        action: "Preview",
        supplier : $("#inputSupplier").val(),
    }

    _xLib.AJAX_Post('/api/KBNOR121/Find_StartEnd_Date', JSON.stringify(obj), function (result) {
        result = _xLib.JSONparseAndTrim(result);
        console.log(result);
    });
});
