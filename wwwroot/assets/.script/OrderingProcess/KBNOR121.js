
var dT_Actual_Receive = "";
var dT_AdjustOrder_Trip = "";
var dT_Date = "";
var dT_DeliveryDate = "";
var dT_Detail = "";
var dT_Header = "";
var dT_PartControl = "";
var dT_Period = "";
var dT_Volume = "";

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

        dT_Actual_Receive = result.data.dT_Actual_Receive;
        dT_AdjustOrder_Trip = result.data.dT_AdjustOrder_Trip;
        dT_Date = result.data.dT_Date;
        dT_DeliveryDate = result.data.dT_DeliveryDate;
        dT_Detail = result.data.dT_Detail;
        dT_Header = result.data.dT_Header;
        dT_PartControl = result.data.dT_PartControl;
        dT_Period = result.data.dT_Period;
        dT_Volume = result.data.dT_Volume;

        //console.log(dT_PartControl.length);

        $("#txtPage").val("1/" + dT_PartControl.length);
        $("#readSupplier").val(dT_PartControl[0].F_Supplier_Code + "-" + dT_PartControl[0].F_Supplier_Plant);
        $("#readPartNo").val(dT_PartControl[0].F_Part_No + "-" + dT_PartControl[0].F_Ruibetsu);
        $("#readStoreCode").val(dT_PartControl[0].F_Store_Code);
        $("#readKanbanNo").val(dT_PartControl[0].F_Kanban_No);
        $("#readCycleTime").val(dT_Date[dT_Date.length - 1].F_Cycle.slice(0, 2) + "-" + dT_Date[dT_Date.length - 1].F_Cycle.slice(2, 4) + "-" + dT_Date[dT_Date.length - 1].F_Cycle.slice(4, 6));
        $("#readQtyPack").val(dT_Header[0].F_Qty_Box);


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

            //if it first of date then pass to save _dayColSpan then save date to dateSet
            if (dateSet.some(f => f.includes(itemDate))) {
                // Sum Colspan pcs,kb,day and night shift in same date
                THeadR1.append(`<th style="border-bottom:0px" colspan="${2 + _dayColSpan + item.F_Period}">${itemDate}</th>`)
                THeadR2.append(`<th style="border:0px" colspan="2"></th>`);

                THeadR3.append(`<th>Pcs</th>`);
                THeadR3.append(`<th>KB</th>`);

                if (_dayColSpan != 0) THeadR2.append(`<th style="border:0px" colspan="${_dayColSpan}" class="bg-danger">Day</th>`);

                if (item.F_Period != 0) THeadR2.append(`<th style="border:0px" colspan="${item.F_Period}" class="bg-primary">Night</th>`);

                count = 1; // Reset count Use Count for assign T + Count
                for (let i = 0; i < _dayColSpan + item.F_Period; i++) {
                    THeadR3.append(`<th>T${count}</th>`);
                    count++;
                }

                // reset dayColSpan
                _dayColSpan = 0;
                return;
            }
            // save dayColSpan when it first date(Save DeliveryTrip Day Shift)
            _dayColSpan = item.F_Period;
            // add to dateSet for check duplicate date and know F_Period is for DeliveryTrip is Night Shift
            dateSet.push(itemDate);

        });
        addDetailToTable(dateSet);
        //console.log("Preview");
    });
});

const addDetailToTable = async (dateSet) => {

    //console.log("addDetailToTable : ", data);
    var _headLength = dT_Header.length; // Length of Header to for loop
    var _increase = dT_PartControl.length; // Increase for Skip each PartControl
    var _headColSpan = 0; // Colspan of Date in Header
    let _countDateSet = 0; // Count DateSet for loop to change Date in each loop
    let _setAccessHeader = ["F_TMT_FO", "F_HMMT_Prod", "F_HMMT_Order", "F_Cycle_Order", "F_MRP", "F_Diff_MRP_FC", "F_AbNormal_Part", "F_Total", "F_Remain_LastTrip", "F_Order_Base", "F_Lot_SizeOrder", "F_KB_CutAdd", "F_Actual_Order", "F_Urgent_Order"];

    let _setAccessDetail = ["F_TMT_FO", "F_HMMT_Prod", "F_HMMT_Order",
        "F_Cycle_Order", "F_MRP", "F_Diff_MRP_FC", "F_AbNormal_Part",
        "F_Total", "F_Remain_LastTrip", "F_Order_Base", "F_Lot_SizeOrder",
        "F_KB_CutAdd", "F_Actual_Order", "F_Urgent_Order", "", "", "F_BL_Plan", "","F_BL_Actual"];
    //console.log("Length : ", _headLength);
    //console.log("Increase : ", _increase);

    for (let i = 0; i <= _headLength; i += _increase) {
        let _header = dT_Header[i]; // Make the Object easier to access

        if (_header == undefined) break; //if out of index then break loop

        let _headerDate = dT_Header[i].F_Process_Date.slice(6, 8) + "-" + dT_Header[i].F_Process_Date.slice(4, 6) + "-" + dT_Header[i].F_Process_Date.slice(0, 4);

        //console.log($("#THeadR1").find(`th:contains('${_headerDate}')`).attr("colspan"));
        _headColSpan = $("#THeadR1").find(`th:contains('${_headerDate}')`).attr("colspan");
         
        //console.log(_header);
        //console.log(dateSet[_countDateSet])
        //insert DTHeader to Table Body Row was fixed at 14
        for (let k = 1; k <= 19; k++) {
            if (k >= 15 || k == 12) $(`#TBodyR${k}`).append(`<td id=tdR${k}Pcs${dateSet[_countDateSet]}></td>`)
            else $(`#TBodyR${k}`).append(`<td id=tdR${k}Pcs${dateSet[_countDateSet]}>${_header[_setAccessHeader[k - 1]]}</td>`)
        }

        for (let j = 0; j <= _headColSpan - 2; j++) {
            var _id = j == 0 ? "KB" : `T` + (j); // j == 0 is KB and j > 0 is T + j

            for (let k = 1; k <= 19; k++) {
                //$(`#TBodyR${k}`).append(`<td id=tdR${k}${_id}>${_header[_setAccessHeader[k-1]]}</td>`)
                if (k == 12 && _id == "KB") {
                    $(`#TBodyR${k}`).append(`<td id='tdR${k}${_id}${dateSet[_countDateSet]}'>${_header[_setAccessHeader[k - 1]]}</td>`)
                }
                else {
                    $(`#TBodyR${k}`).append(`<td id='tdR${k}${_id}${dateSet[_countDateSet]}'></td>`) // add id for easy to value

                }
            }
        }

        _countDateSet += 1;
        if (_countDateSet >= dateSet.length) _countDateSet = 0;
    }

    let _countT = 1;
    _countDateSet = 0; //reset countDateSet
    _headColSpan = $("#THeadR1").find(`th:contains('${dateSet[0]}')`).attr("colspan");

    for (let i = 0; i <= dT_Detail.length; i += _increase) {

        if (dT_Detail[i] == undefined) break; //if out of index then break loop

        let date = dateSet[_countDateSet];
        //if (date == undefined) date = dateSet[0];
        //_headColSpan = $("#THeadR1").find(`th:contains('${date}')`).attr("colspan");

        // if date was change and Cycle was change then reset countT
        if (_headColSpan != $("#THeadR1").find(`th:contains('${date}')`).attr("colspan")) {
            _countT = 1;
            _headColSpan = $("#THeadR1").find(`th:contains('${date}')`).attr("colspan");
        }
        else { //Didnt have Adjust CycleTime

            //check countT to change DateSet to another Date
            _countDateSet = _countT > _headColSpan - 2 ? _countDateSet + 1 : _countDateSet;
            // count == -1 if it's last column T
            _countT = _countT > _headColSpan - 2 ? 1 : _countT;
        }

        if (dT_Detail[i] == undefined) break; //if out of index then break loop

        for (let k = 1; k <= 19; k++) {

            let _insIdDetail = `tdR${k}T` + _countT + dateSet[_countDateSet];
            //console.log(i);
            //console.log(_insIdDetail);
            $(`#${_insIdDetail}`).text(dT_Detail[i][_setAccessDetail[k - 1]]);

            if (k == 19) { _countT += 1; } //new column T
        }
    }

    _countT = 1;

    for (let i = 0; i <= dT_Actual_Receive.length; i += 1) {
        if (dT_Actual_Receive[i] == undefined) break; //if out of index then break loop

        //console.log($("#readPartNo").val().includes(dT_Actual_Receive[i].F_PART_No));

        if (!($("#readPartNo").val().includes(dT_Actual_Receive[i].F_PART_No))) {
            break;
        }

        //console.log(dT_Volume[i]);
        let F_Receive_Date = dT_Actual_Receive[i].F_Receive_Date.slice(6, 8) + "-" + dT_Actual_Receive[i].F_Receive_Date.slice(4, 6) + "-" + dT_Actual_Receive[i].F_Receive_Date.slice(0, 4);
        //console.log(F_Delivery_Date);
        let F_Delivery_Trip = dT_Actual_Receive[i].F_Delivery_Trip;
        let _ActualReceiveKB = dT_Actual_Receive[i].F_Receive_QTY / parseInt($("#readQtyPack").val());

        let _insActualReceive = `tdR18T${F_Delivery_Trip}${F_Receive_Date}`;

        $(`#${_insActualReceive}`).text(_ActualReceiveKB);
    }
};