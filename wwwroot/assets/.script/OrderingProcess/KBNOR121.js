$(document).ready(async function () {
    var text = $(".nav-right li span").text();
    var date = text.split("Date : ")[1].trim().split(" ")[0];
    var plant = text.split("Plant : ")[1].split(" ")[0];
    var shift = text.split("Shift : ")[1].split(" ")[0];
    shift == 1 ? shift = "1 - Day Shift" : shift = "2 - Night Shift";
    //console.log(date.slice(8, 10));
    $("#inputMRP").val("MRP : " + date.slice(8, 10) + "/" + date.slice(5, 7) + "/" + date.slice(0, 4))
    var addDate = parseInt((date.slice(8, 10))) + 1;
    addDate = addDate.toString().length == 1 ? "0" + addDate : addDate;
    date = date.slice(0, 8) + addDate;
    $("#inputPlant").val(plant);
    $("#inputProcessDateFor").val(date);
    $("#inputProcessShiftFor").val(shift);
    //$("#tableMaster").DataTable({
    //    "paging": false,
    //    "info": false,
    //    "searching": false,
    //    "ordering": false,
    //    "autoWidth": false,
    //    "scrollX": true
    //});

    await _xLib.AJAX_Get('/api/KBNOR121/OnLoad', { Shift: shift, Process_Date: $("#inputProcessDateFor").val() });

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

$("#btnPreview").click(async function () {
    var obj = {
        action: "Preview",
        supplier : $("#inputSupplier").val(),
    }

    await _xLib.AJAX_Post('/api/KBNOR121/Find_StartEnd_Date', JSON.stringify(obj), function (result) {
        result = _xLib.JSONparseAndTrim(result);
        console.log(result);
    });

    await _xLib.AJAX_Post('/api/KBNOR121/Get_All_Data', JSON.stringify(obj), function (result) {
        result = _xLib.JSONparseAndTrimArray(result);
        console.log(result.data);

        var dT_Actual_Receive = result.data.dT_Actual_Receive;
        var dT_AdjustOrder_Trip = result.data.dT_AdjustOrder_Trip;
        var dT_Date = result.data.dT_Date;
        var dT_DeliveryDate = result.data.dT_DeliveryDate;
        var dT_Detail = result.data.dT_Detail;
        var dT_Header = result.data.dT_Header;
        var dT_PartControl = result.data.dT_PartControl;
        var dT_Period = result.data.dT_Period;
        var dT_Volume = result.data.dT_Volume;

        //console.log(dT_PartControl.length);

        $("#txtPage").val("1/" + dT_PartControl.length);
        $("#readSupplier").val(dT_PartControl[0].F_Supplier_Code + "-" + dT_PartControl[0].F_Supplier_Plant);
        $("#readPartNo").val(dT_PartControl[0].F_Part_No + "-" + dT_PartControl[0].F_Ruibetsu);
        $("#readStoreCode").val(dT_PartControl[0].F_Store_Code);
        $("#readKanbanNo").val(dT_PartControl[0].F_Kanban_No);
        $("#readCycleTime").val(dT_Date[0].F_Cycle.slice(0, 2) + "-" + dT_Date[0].F_Cycle.slice(2, 4) + "-" + dT_Date[0].F_Cycle.slice(4,6));


        //$("#tableMaster").DataTable().destroy();

        $("#tableMaster thead tr").empty();
        $("#tableMaster tbody tr td").remove();

        var THeadR1 = $("#THeadR1");
        var THeadR2 = $("#THeadR2");
        var THeadR3 = $("#THeadR3");
        var dateSet = [];

        THeadR1.append('<th style="border:0px"><input class="form-control" value="MRP : 15/07/2024" id="inputMRP" readonly /></th>');
        THeadR2.append('<th style="border:0px"><input type="radio" name="radMRP" id="MRP-20" /> MRP-20% </th>');
        THeadR3.append(`<th style="border:0px"><input type="radio" name="radMRP" id="MRP20" /> MRP+20%</th>`);

        var count = 1;
        var _dayColSpan = 0; // Collect Day Colspan in first loop each Date
        dT_Period.forEach(function (item) {

            var itemDate = item.Date_Now.slice(6, 8) + "-" + item.Date_Now.slice(4, 6) + "-" + item.Date_Now.slice(0, 4);

            if (dateSet.some(f => f.includes(itemDate))) {
                // Sum Colspan pcs,kb,day and night shift in same date
                THeadR1.append(`<th style="border-bottom:0px" colspan="${2 + _dayColSpan + item.F_Period}">${itemDate}</th>`)
                THeadR2.append(`<th style="border:0px" colspan="2"></th>`);

                THeadR3.append(`<th>Pcs</th>`);
                THeadR3.append(`<th>KB</th>`);

                if (_dayColSpan != 0) THeadR2.append(`<th style="border:0px" colspan="${_dayColSpan}" class="bg-danger">Day</th>`);


                if (item.F_Period != 0) THeadR2.append(`<th style="border:0px" colspan="${item.F_Period}" class="bg-primary">Night</th>`);


                count = 1; // Reset count Use Count for assign T + Count
                for (let i = 1; i <= _dayColSpan + item.F_Period; i++) {
                    THeadR3.append(`<th>T${count}</th>`);
                    count++;
                }
                // reset dayColSpan
                _dayColSpan = 0;
                return;
            }

            _dayColSpan = item.F_Period;

            dateSet.push(itemDate);

        });

    });
});
