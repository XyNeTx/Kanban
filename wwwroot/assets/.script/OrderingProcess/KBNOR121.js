
var dT_Actual_Receive = "";
var dT_AdjustOrder_Trip = "";
var dT_Date = "";
var dT_DeliveryDate = "";
var dT_Detail = "";
var dT_Header = "";
var dT_PartControl = "";
var dT_Period = "";
var dT_Volume = "";
var _command = "";
var _CookieProcessDate = _xLib.GetCookie("processDate");
var _CookieLoginDate = _xLib.GetCookie("loginDate");

$(document).ready(async function () {
    var text = $(".nav-right li span").text();
    var date = text.split("Date : ")[1].trim().split(" ")[0];
    var plant = text.split("Plant : ")[1].split(" ")[0];
    var shift = text.split("Shift : ")[1].split(" ")[0];
    shift == 1 ? shift = "1 - Day Shift" : shift = "2 - Night Shift";
    //console.log(date.slice(8, 10));
    $("#inputMRP").val("MRP : " + date.slice(8, 10) + "/" + date.slice(5, 7) + "/" + date.slice(0, 4))

    //console.log(date);
    $("#inputPlant").val(plant);

    $("#inputProcessDateFor").val(_CookieProcessDate.slice(0, 10));
    $("#inputProcessShiftFor").val(_CookieProcessDate.slice(10, 11) == "D" ? "1 - Day Shift" : "2 - Night Shift");

    await _xLib.AJAX_Get('/api/KBNOR121/OnLoad', { Login_Date: _CookieLoginDate , Shift: shift, Process_Date: $("#inputProcessDateFor").val() });

    await _xLib.AJAX_Get('/api/KBNOR121/SupplierDropDown', '', function (result) {
        result = _xLib.JSONparseAndTrim(result);
        $("#inputSupplier").empty();
        $("#inputSupplier").append("<option value='' hidden></option>");
        result.data.forEach(function (item) {
            $("#inputSupplier").append(new Option(item.F_Supplier, item.F_Supplier));
        });

    });
    var obj = {
        action: "Onload",
        supplier: "Onload",
    }
    await _xLib.AJAX_Post('/api/KBNOR121/Chk_Status_CCR', JSON.stringify(obj),
        function (success) {
            if (success.status == 200) {
                success.data = _xLib.JSONparseAndTrim(success.data);
                console.log(success.data);
                $("#btnPreview").prop("disabled", !success.data.includes("Preview"));
                $("#btnProcess").prop("disabled", !success.data.includes("Search"));
                $("#btnReCal").prop("disabled", !success.data.includes("Recalculate"));
            }
        }
    )

    $("thead tr th").each(function () {
        $(this).addClass("text-center");
    });

    xSplash.hide();
});

$(document).on("click", "#radioKbStop , #radioKbAdd ,#radioKbCut , #MRP-20 ,#MRP20",function (e) {
    e.preventDefault();
});

$("#btnEditNews").click(function () {
    //var textArea = $("#txtNews").val();
    //console.log(textArea);

    var obj = {
        F_Supplier_Code: $("#readSupplier").val().split("-")[0],
        F_Supplier_Plant: $("#readSupplier").val().split("-")[1],
        F_Part_No: $("#readPartNo").val().split("-")[0],
        F_Ruibetsu: $("#readPartNo").val().split("-")[1],
        F_Store_Code: $("#readStoreCode").val(),
        F_Kanban_No: $("#readKanbanNo").val(),
        F_Text: $("#txtNews").val()
    }

    if (obj.F_Kanban_No == "" && obj.F_Supplier_Code == "" && obj.F_Supplier_Plant == "" && obj.F_Part_No == "" && obj.F_Ruibetsu == "" && obj.F_Store_Code == "") {
        return xSwal.error("Error", "Please Select Data Before Update News");
    }

    console.log(obj);

    _xLib.AJAX_Post('/api/KBNOR121/UpdateInformNews', JSON.stringify(obj),
        function (result) {
            if (result.status == 200) {
                xSwal.success("Success", "Update News Success");
            }
        },
        function (error) {
            xSwal.error("Error", "Update News Fail");
        }
    );
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
    _command = "Preview";
    $("#txtPage").val("1/1");
    previewFunction(0, _command);
    window.scrollTo(0, 0);
    $("#btnBlCal").prop("disabled", false);
    $("#btnReCal").prop("disabled", true);
});

$("#btnProcess").click(async function () {
    _command = "Process";
    $("#txtPage").val("1/1");
    previewFunction(0, _command);
    window.scrollTo(0, 0);
    $("#btnBlCal").prop("disabled", false);
    $("#btnReCal").prop("disabled", false);
});

$("#btnPrevPage").click(async function () {
    var page = parseInt($("#txtPage").val().split("/")[0]);
    if (page == 1) return;
    $("#txtPage").val((parseInt(page) - 1) + "/" + dT_PartControl.length);
    previewFunction($("#txtPage").val().split("/")[0] - 1, _command); // -1 because index start at 0
    window.scrollTo(0, 0);
});

$("#btnNextPage").click(async function () {
    var page = parseInt($("#txtPage").val().split("/")[0]);
    if (page == parseInt($("#txtPage").val().split("/")[1])) return;
    $("#txtPage").val((parseInt(page) + 1) + "/" + dT_PartControl.length);
    previewFunction($("#txtPage").val().split("/")[0] - 1, _command); // -1 because index start at 0
    window.scrollTo(0, 0);
});

const previewFunction = async (intRow,_command) => {
    //console.log("IntRow : ", intRow);
    $("#divSpinner").css("visibility", "visible");
    $("#divRead").css("visibility", "hidden");
    $("#tableMaster").css("visibility", "hidden");
    var obj = {
        action: _command,
        supplier: $("#inputSupplier").val(),
        partNo: $("#inputPart").val(),
        partNoTo: $("#inputPartTo").val(),
        store: $("#inputStore").val(),
        storeTo: $("#inputStoreTo").val(),
        kanban : $("#inputKanban").val(),
        kanbanTo: $("#inputKanbanTo").val(),
    }

    //await _xLib.AJAX_Post('/api/KBNOR121/Find_StartEnd_Date', JSON.stringify(obj), function (result) {
    //    result = _xLib.JSONparseAndTrim(result);
    //    //console.log(result);
    //});

    await _xLib.AJAX_Post('/api/KBNOR121/Get_All_Data', JSON.stringify(obj), async function (result) {
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
        if ($("#readSupplier").val() != dT_PartControl[0].F_Supplier_Code + "-" + dT_PartControl[0].F_Supplier_Plant) {
            $("#txtPage").val("1/" + dT_PartControl.length);
        }
        else { $("#txtPage").val(`${intRow + 1}/${dT_PartControl.length}`); }
        $("#readSupplier").val(dT_PartControl[intRow].F_Supplier_Code + "-" + dT_PartControl[intRow].F_Supplier_Plant);
        $("#readPartNo").val(dT_PartControl[intRow].F_Part_No + "-" + dT_PartControl[intRow].F_Ruibetsu);
        $("#readStoreCode").val(dT_PartControl[intRow].F_Store_Code);
        $("#readKanbanNo").val(dT_PartControl[intRow].F_Kanban_No);
        $("#readCycleTime").val(dT_Date[dT_Date.length - 1].F_Cycle.slice(0, 2) + "-" + dT_Date[dT_Date.length - 1].F_Cycle.slice(2, 4) + "-" + dT_Date[dT_Date.length - 1].F_Cycle.slice(4, 6));
        $("#readQtyPack").val(dT_Header[intRow + dT_PartControl.length].F_Qty_Box);

        $("#spanProcessDate").text(_CookieProcessDate.slice(8, 10) + "/" + _CookieProcessDate.slice(5, 7) + "/" + _CookieProcessDate.slice(0, 4));
        $("#spanDeliveryDate").text(dT_DeliveryDate[0].F_Delivery_Date.slice(6, 8) + "/" + dT_DeliveryDate[0].F_Delivery_Date.slice(4, 6) + "/" + dT_DeliveryDate[0].F_Delivery_Date.slice(0, 4));

        //$("#tableMaster").DataTable().destroy();

        $("#tableMaster thead tr").empty();
        $("#tableMaster tbody tr td").remove();
        //$("#divRead").css("visibility", "hidden");
        //$("#tableMaster").css("visibility", "hidden");

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

                if (_dayColSpan != 0) THeadR2.append(`<th style="border:0px;background-color:#F65900" colspan="${_dayColSpan}" class="text-light">Day</th>`);

                if (item.F_Period != 0) THeadR2.append(`<th style="border:0px" colspan="${item.F_Period}" class="bg-primary text-light">Night</th>`);

                count = 1; // Reset count Use Count for assign T + Count

                for (let i = 0; i < _dayColSpan; i++) {
                    if (i == 0) { THeadR3.append(`<th id='1stTDay'>T${count}</th>`); }
                    else { THeadR3.append(`<th id='TDay${count}'>T${count}</th>`); }
                    count++;
                }
                for (let i = 0; i < item.F_Period; i++) {
                    if (i == 0) { THeadR3.append(`<th id='1stTNight'>T${count}</th>`); }
                    else { THeadR3.append(`<th id='TNight${count}'>T${count}</th>`); }
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
        await addDetailToTable(dateSet, intRow);
        await sumKB(dateSet);
        $("#inputMRP").val("MRP : " + _CookieLoginDate.slice(8, 10) + "/" + _CookieLoginDate.slice(5, 7) + "/" + _CookieLoginDate.slice(0, 4))

        await _xLib.AJAX_Post('/api/KBNOR121/Chk_Status_CCR', JSON.stringify(obj),
            function (success) {
                if (success.status == 200) {
                    success.data = _xLib.JSONparseAndTrim(success.data);
                    //console.log(success.data);
                    $("#btnPreview").prop("disabled", !success.data.includes("Preview"));
                    $("#btnProcess").prop("disabled", !success.data.includes("Search"));
                    $("#btnReCal").prop("disabled", !success.data.includes("Recalculate"));
                }
            }
        )

        $("#divSpinner").css("visibility", "hidden");
        $("#divRead").css("visibility", "visible");
        $("#tableMaster").css("visibility", "visible");
        //console.log("Preview");
    });
};

const addDetailToTable = async (dateSet, intRow) => {

    //console.log("addDetailToTable : ", data);
    var _headLength = dT_Header.length;         // Length of Header to for loop
    var _increase = dT_PartControl.length;      // Increase for Skip each PartControl
    var _headColSpan = 0;                       // Colspan of Date in Header
    let _countDateSet = 0;                      // Count DateSet for loop to change Date in each loop
    let _setAccessHeader = ["F_TMT_FO", "F_HMMT_Prod", "F_HMMT_Order",
        "F_Cycle_Order", "F_MRP", "F_Diff_MRP_FC", "F_AbNormal_Part",
        "F_Total", "F_Remain_LastTrip", "F_Order_Base", "F_Lot_SizeOrder",
        "F_KB_CutAdd", "F_Actual_Order", "F_Urgent_Order"];

    let _setAccessDetail = ["F_TMT_FO", "F_HMMT_Prod", "F_HMMT_Order",
        "F_Cycle_Order", "F_MRP", "F_Diff_MRP_FC", "F_AbNormal_Part",
        "F_Total", "F_Remain_LastTrip", "F_Order_Base", "F_Lot_SizeOrder",
        "F_KB_CutADD", "F_Actual_order", "F_Urgent_Order","", "", "", "F_BL_Plan", "", "F_BL_Actual"];
    //console.log("Length : ", _headLength);
    //console.log("Increase : ", _increase);

    for (let i = intRow; i <= _headLength; i += _increase) {
        let _header = dT_Header[i]; // Make the Object easier to access
        //console.log(_header);

        if (_header == undefined) break; //if out of index then break loop

        let _headerDate = dT_Header[i].F_Process_Date.slice(6, 8) + "-" + dT_Header[i].F_Process_Date.slice(4, 6) + "-" + dT_Header[i].F_Process_Date.slice(0, 4);
        if(!dateSet.some(f => f.includes(_headerDate))) continue; //if date not in dateSet then skip loop


        //console.log($("#THeadR1").find(`th:contains('${_headerDate}')`).attr("colspan"));
        _headColSpan = $("#THeadR1").find(`th:contains('${_headerDate}')`).attr("colspan");
        if(_headColSpan == undefined) continue;
        //console.log(_header);
        //console.log(dateSet[_countDateSet])

        //insert DTHeader to Table Body Row was fixed at 15
        for (let k = 1; k <= 20; k++) {
            if (k >= 15 || k == 12) $(`#TBodyR${k}`).append(`<td id=tdR${k}Pcs${dateSet[_countDateSet]}></td>`)
            else $(`#TBodyR${k}`).append(`<td id=tdR${k}Pcs${dateSet[_countDateSet]}>${_header[_setAccessHeader[k - 1]]}</td>`)
        }

        for (let j = 0; j <= _headColSpan - 2; j++) {
            var _id = j == 0 ? "KB" : `T` + (j); // j == 0 is KB and j > 0 is T + j

            for (let k = 1; k <= 20; k++) {
                //$(`#TBodyR${k}`).append(`<td id=tdR${k}${_id}>${_header[_setAccessHeader[k-1]]}</td>`)
                if (k == 12 && _id == "KB") {
                    _header["F_KB_CutAdd"] = parseInt(_header["F_KB_CutAdd"]) / parseInt($("#readQtyPack").val());

                    if (isNaN(_header["F_KB_CutAdd"])) _header["F_KB_CutAdd"] = 0;
                    $(`#TBodyR${k}`).append(`<td id='tdR${k}${_id}${dateSet[_countDateSet]}'>${_header["F_KB_CutAdd"]}</td>`)
                }
                else {
                    if (dateSet[_countDateSet] != _CookieProcessDate) {
                        $(`#TBodyR${k}`).append(`<td id='tdR${k}${_id}${dateSet[_countDateSet]}'></td>`) // add id for easy to value
                    }
                    //if (k == 1) {
                    //    $(`#tdR${k}${_id}${dateSet[_countDateSet]}`).text(_header[_setAccessHeader[k - 1]]))
                    //}
                }
            }
        }

        var _headDate = $("#THeadR1").find(`th:contains('${dateSet[_countDateSet]}')`).text().replaceAll("-", "");
        var _headYMD = _headDate.slice(4, 8) + _headDate.slice(2, 4) + _headDate.slice(0, 2);
        var _intHeadDate = parseInt(_headYMD);
        var _intDeliveryDate = parseInt(dT_DeliveryDate[0]["F_Delivery_Date"]);

        const obj = {
            CurrentDate: _headYMD.slice(0, 4) + "-" + _headYMD.slice(4, 6) + "-" + _headYMD.slice(6, 8),
            Supplier: $("#readSupplier").val(),
            PartNo: $("#readPartNo").val(),
            Store: $("#readStoreCode").val(),
            Kanban: $("#readKanbanNo").val()
        }

        await _xLib.AJAX_Post('/api/KBNOR121/Get_BL', JSON.stringify(obj),
            async function (success) {
                if (success.status == 200) {
                    success = await _xLib.JSONparseAndTrim(success);
                    //console.log(success.data);
                    $(`#tdR18Pcs${dateSet[_countDateSet]}`).text(success.data[0].F_BL_SET_Plan);
                    $(`#tdR20Pcs${dateSet[_countDateSet]}`).text(success.data[0].F_BL_SET_Actual);
                    if(success.data[0].F_Not_Recalculate){
                        $(`#tdR18Pcs${dateSet[_countDateSet]}`).css("font-weight", "900");
                        $(`#tdR20Pcs${dateSet[_countDateSet]}`).css("font-weight", "900");
                    }
                }
            }
        );


        if (_intHeadDate == _intDeliveryDate) {
            $("#THeadR1").find(`th:contains('${dateSet[_countDateSet]}')`).addClass("bg-danger");
        }
        else if (_intHeadDate == parseInt(_CookieProcessDate.slice(0, 10).replaceAll("-", ""))) {
            $("#THeadR1").find(`th:contains('${dateSet[_countDateSet]}')`).addClass("bg-warning");
        }
        else if (_intHeadDate > _intDeliveryDate) {
            $("#THeadR1").find(`th:contains('${dateSet[_countDateSet]}')`).addClass("bg-success");
        }
        else {
            $("#THeadR1").find(`th:contains('${dateSet[_countDateSet]}')`).css("background-color", "rgb(206, 212, 218)");
        }

        _countDateSet += 1;
        if (_countDateSet >= dateSet.length) _countDateSet = 0;
    }

    let _countT = 1;
    _countDateSet = 0; //reset countDateSet

    _headColSpan = $("#THeadR1").find(`th:contains('${dateSet[0]}')`).attr("colspan");

    dT_Detail.filter(x => x.F_Part_No + "-" + x.F_Ruibetsu == $("#readPartNo").val()
        && x.F_Supplier_Code == $("#inputSupplier").val().split("-")[0]
        && x.F_Supplier_Plant == $("#inputSupplier").val().split("-")[1]
        && x.F_Store_Code == $("#readStoreCode").val()
        && x.F_Kanban_No == $("#readKanbanNo").val()
    ).forEach(function (item, i) {
        //console.log(item);
        let F_Process_Date = item.F_Process_Date.slice(6, 8) + "-" + item.F_Process_Date.slice(4, 6) + "-" + item.F_Process_Date.slice(0, 4);
        if (!dateSet.some(f => f.includes(F_Process_Date))) {
            return;
        }

        let date = dateSet[_countDateSet];
        // if date was change and Cycle was change then reset countT
        if (_headColSpan != $("#THeadR1").find(`th:contains('${date}')`).attr("colspan")) {
            //console.log($("#THeadR1").find(`th:contains('${date}')`).attr("colspan"));
            //if header colspan increase then CountT = 1 else remain CountT
            _countT = _headColSpan < $("#THeadR1").find(`th:contains('${date}')`).attr("colspan") ? 1 : _countT
            _headColSpan = $("#THeadR1").find(`th:contains('${date}')`).attr("colspan");
        }

        else { //Didnt have Adjust CycleTime
            //check countT to change DateSet to another Date
            _countDateSet = _countT > _headColSpan - 2 ? _countDateSet + 1 : _countDateSet;
            // count == -1 if it's last column T
            _countT = _countT > _headColSpan - 2 ? 1 : _countT;
        }

        for (let k = 1; k <= 20; k++) {
            let _insIdDetail = `tdR${k}T` + _countT + dateSet[_countDateSet];

            if (k == 11 || k == 12 || k == 13) { //convert qty to KB fixed by row
                let _KB = parseInt(item[_setAccessDetail[k - 1]]) / parseInt($("#readQtyPack").val());
                if (isNaN(_KB) || _KB == Infinity) _KB = 0;
                $(`#${_insIdDetail}`).text(_KB);
            }

            else if (k == 4) {
                $(`#${_insIdDetail}`).text();
            }

            else if (k == 14) {
                item[_setAccessDetail[k - 1]] != 0 ? $(`#${_insIdDetail}`).text(item[_setAccessDetail[k - 1]]) : $(`#${_insIdDetail}`).text("");
            }

            else $(`#${_insIdDetail}`).text(item[_setAccessDetail[k - 1]]);

            //if (k == 18 || k == 20) {
            //    console.log(item);
            //    if (item.F_Not_Recalculate) {
            //        $("#tdR" + k + "Pcs" + dateSet[_countDateSet]).css("font-weight", "900");
            //    }
            //}

            if (k == 20) { _countT += 1; } //new column T

        }

    });

    // ------------------------------------ DT AdjustOrder_Trip -----------------------------------------

    dT_AdjustOrder_Trip.filter(x => x.F_Part_No + "-" + x.F_Ruibetsu == $("#readPartNo").val()
        && x.F_Supplier_Code == $("#inputSupplier").val().split("-")[0]
        && x.F_Supplier_Plant == $("#inputSupplier").val().split("-")[1]
        && x.F_Store_Code == $("#readStoreCode").val()
        && x.F_Kanban_No == $("#readKanbanNo").val()
    ).forEach(function (item, i) {
        //console.log(item);

        let _idAdjQty = `tdR16T${item.F_Delivery_Trip}${item.F_Delivery_Date.slice(6, 8)}-${item.F_Delivery_Date.slice(4, 6)}-${item.F_Delivery_Date.slice(0, 4)}`;
        if (item.F_Adj_Qty != 0) {

            $(`#${_idAdjQty}`).text(item.F_Adj_Qty);

            let _intColSpan = $("#THeadR1").find(`th:contains('${item.F_Delivery_Date.slice(6, 8)}-${item.F_Delivery_Date.slice(4, 6)}-${item.F_Delivery_Date.slice(0, 4)}')`).attr("colspan");

            for (let i = 1; i < _intColSpan - 1; i++) { //insert 0 to another T in same date

                let _idAdjQty = `tdR16T${i}${item.F_Delivery_Date.slice(6, 8)}-${item.F_Delivery_Date.slice(4, 6)}-${item.F_Delivery_Date.slice(0, 4)}`;

                $(`#${_idAdjQty}`).text() == "" ? $(`#${_idAdjQty}`).text("0") : $(`#${_idAdjQty}`).text();

            }
        }
    });

    // ------------------------------------ DT AdjustOrder_Trip -----------------------------------------

    // ------------------------------------ DT Volume -----------------------------------------

    await dT_Volume.filter(x => x.F_Part_No + "-" + x.F_Ruibetsu == $("#readPartNo").val()
        && x.F_Supplier_Code == $("#inputSupplier").val().split("-")[0]
        && x.F_Supplier_Plant == $("#inputSupplier").val().split("-")[1]
        && x.F_Store_Code == $("#readStoreCode").val()
        && x.F_Kanban_No == $("#readKanbanNo").val()
    ).forEach(function (item, i) {
        let F_Delivery_Date = item.F_Delivery_Date.slice(6, 8) + "-" + item.F_Delivery_Date.slice(4, 6) + "-" + item.F_Delivery_Date.slice(0, 4);
        let F_Delivery_Trip = item.F_Delivery_Trip;

        var _Row = [15, 17];
        var _Key = ["F_Adj_Pattern", "F_Qty"];

        _Row.forEach(function (row, i) {
            if (item[_Key[i]] == undefined || item[_Key[i]] == 0 || item[_Key[i]] == 0) return;
            let _insPattern = `tdR${row}T${F_Delivery_Trip}${F_Delivery_Date}`;
            let _PatternKB = parseInt(item[_Key[i]]) / parseInt($("#readQtyPack").val()); // Convert Qty to KB
            _PatternKB = _PatternKB == undefined ? 0 : _PatternKB;
            if (isNaN(_PatternKB) || _PatternKB == Infinity) _PatternKB = 0;
            $(`#${_insPattern}`).text(_PatternKB);
        });

    });

    $("table tbody tr td[id*='tdR17T']").each(function () {
        if ($(this).text() == "") {
            $(this).text("0");
        }
    });


    $("table tbody tr td[id*='tdR15T']").each(function () {
        if ($(this).text() == "") {
            $(this).text("0");
        }
    });

    // ------------------------------------ DT Volume -----------------------------------------


    // ------------------------------------ DT Actual -----------------------------------------

    dT_Actual_Receive.filter(x => x.F_PART_No + "-" + x.F_RUibetsu == $("#readPartNo").val()
        && x.F_Supplier_code == $("#inputSupplier").val().split("-")[0]
        && x.F_Supplier_Plant == $("#inputSupplier").val().split("-")[1]
        && x.F_Store_Cd == $("#readStoreCode").val()
    ).forEach(function (item, i) {
        let F_Receive_Date = item.F_Receive_Date.slice(6, 8) + "-" + item.F_Receive_Date.slice(4, 6) + "-" + item.F_Receive_Date.slice(0, 4);
        let F_Delivery_Trip = item.F_Delivery_Trip;
        let _ActualReceiveKB = item.F_Receive_QTY / parseInt($("#readQtyPack").val());

        let _insActualReceive = `tdR19T${F_Delivery_Trip}${F_Receive_Date}`;

        $(`#${_insActualReceive}`).text(_ActualReceiveKB);
    });

    $("table tbody tr td[id*='tdR19T']").each(function () {
        if ($(this).text() == "") {
            $(this).text("0");
        }
    });

    // ------------------------------------ DT Actual -----------------------------------------

    await _xLib.AJAX_Get('/api/KBNOR121/Detail_Data', { intRow: $("#txtPage").val().split("/")[0] - 1, F_Supplier_Cd: $("#inputSupplier").val() },
        function (success) {
            if (success.status == 200) {
                success.data = _xLib.JSONparseAndTrim(success.data);
                console.log(success.data);
                $("#readForecastMax").val(success.data.forecastMaxInt);
                $("#readSafetyStock").val(success.data.safety_Stock);
                $("#readMaxArea").val(success.data.max_Area);
                $("#readLimitTrip").val(success.data.avgTrip);
                $("#readStdBPcs").val(success.data.std_B);
                $("#readStdStock").val(success.data.stdStock);
                $("#readMinStock").val(success.data.minStock);
                $("#readLine").val(success.data.f_Address);
                $("#readPartName").val(success.data.f_Part_nm);
                $("#readSupplierName").val(success.data.f_short_name);
                $("#txtNews").val(success.data.informNews);

                if (success.data.mrpCheck.includes("More")) {
                    $("#MRP20").prop("checked", true);
                    $("#THeadR3 th").find("input").parent().addClass("text-danger");
                }
                else if (success.data.mrpCheck.includes("Less")) {
                    $("#MRP-20").prop("checked", true);
                    $("#THeadR2 th").find("input").parent().addClass("text-danger");
                }

                const accessValue = ["recSlideOrderCheck", "setOrderCheck", "slideOrderCheck", "chgCycleCheck","kb_Add","kb_Cut","kb_Stop"];
                const accessLabel = ["lbRecSlice", "lbOrderTable", "lbSliceOrder", "lbChgCycleTime", "lbKbAdd", "lbKbCut", "lbKbStop"];
                const accessCheck = ["chkBoxRecSlice", "chkBoxOrderTable", "chkBoxSliceOrder", "chkBoxChgCycleTime", "radioKbAdd", "radioKbCut","radioKbStop"];

                for (let i = 0; i < accessCheck.length; i++) {
                    if (success.data[accessValue[i]] != 0) {
                        $(`#${accessCheck[i]}`).prop("checked", true);
                        $(`#${accessLabel[i]}`).removeClass("text-primary").addClass("text-danger");
                    }
                    else {
                        $(`#${accessCheck[i]}`).prop("checked", false);
                        $(`#${accessLabel[i]}`).removeClass("text-danger").addClass("text-primary");
                    }
                }


                var Process_date = dT_DeliveryDate[0].F_Delivery_Date;
                //filter by process date, part no, supplier code, supplier plant, store code to get TMT_FO
                var FilteredHeader = dT_Header.filter(x => x.F_Process_Date == Process_date
                    && x.F_Part_No == $("#readPartNo").val().split("-")[0]
                    && x.F_Ruibetsu == $("#readPartNo").val().split("-")[1]
                    && x.F_Supplier_Code == $("#readSupplier").val().split("-")[0]
                    && x.F_Supplier_Plant == $("#readSupplier").val().split("-")[1]
                    && x.F_Store_Code == $("#readStoreCode").val()
                )
                //console.log("FilteredHeader : ", FilteredHeader);
                $("#readUseDay").val(FilteredHeader[0].F_TMT_FO);

                //console.log(success.data);
            }
        }
    );

    let _tableShift = "";
    $(`table tbody tr td:not([id*='Pcs']):not([id*='KB'])`).each(async function () {
        let index = $(this).index();
        let _id = $(this).attr("id"); // _id can be undefined if the element does not have an id
        //console.log(" id : ", _id);

        if (_id == undefined)
        {
            return;
        }

        if ($(`table thead tr[id='THeadR2'] th`).eq(`${index}`).text().slice(0, 1) != "") {
            _tableShift = $(`table thead tr[id='THeadR2'] th`).eq(`${index}`).text().slice(0, 1);
        }

        $(this).css("background-color", "#ced4da");
    });

    // Set Style for Table
    $("table tbody tr td[id*='Pcs'],table tbody tr td[id*='KB']").each(async function () {
        $(this).css("background-color", "#b9b9b9");
        $(this).css("border", "1px solid #e3e6f0");
    });

    $("table thead tr th[id*='TDay'], table thead tr th[id*='TNight']").each(async function () {
        //console.log($(this).attr("id"));
        if ($(this).attr("id").includes("1stT")) {
            let index = $(this).index() - 1; // -1 because index start at 0
            let $td = $("table tbody tr").find("td").eq(index);


            if ($(this).attr("id").includes("Day")) {
                if ($td.attr("id").includes("R1")) {
                    //$td.addClass("bg-danger");
                    //console.log($td.attr("id"));
                    $td.css("background-color", "#F65900");
                    $td.css("font-weight", "900");
                }
            }
            else if ($(this).attr("id").includes("Night")) {
                if ($td.attr("id").includes("R1")) {
                    $td.addClass("bg-primary");
                    $td.css("font-weight", "900");
                }
            }
        }
        else {
            let index = $(this).index() - 1; // -1 because index start at 0
            let $td = $("table tbody tr").find("td").eq(index);
            //console.log($td.attr("id"));
            $td.text(""); // empty it if it in same shift

            let _prev$td = $("table tbody tr").find("td").eq(index - 1);
            await $td.attr("style", "");
            //console.log(_prev$td.attr("style"));
            $td.attr("style",_prev$td.attr("style")); // add bg color from previous td)
            $td.addClass(_prev$td.attr("class")); // add bg class from previous td
        }
    });

    var _Row = [1, 18, 20];
    for (let i = 0; i < _Row.length; i++) {
        if (_Row[i] == 1) {
            $(`table tbody tr td[id*='tdR${_Row[i]}Pcs']`).each(function () {
                $(this).css("font-weight", "900");
            });
        }
        //console.log(_Row[i]);
        else if (_Row[i] == 18 || _Row[i] == 20) {
            //console.log("18,20");
            var _intStockB = parseInt($("#readStdBPcs").val());
            var _intMinStock = parseInt($("#readMinStock").val());
            $(`table tbody tr td[id*='tdR${_Row[i]}T']`).each(function () {
                var _bl = parseInt($(this).text());
                //console.log(_bl);
                //console.log(_intStockB);
                //console.log(_intMinStock);
                if (_bl > _intStockB) $(this).addClass("bg-warning");
                else if (_bl < _intMinStock) $(this).addClass("bg-danger");
                else if (_bl <= _intStockB && _bl >= _intMinStock) $(this).addClass("bg-success");
            });

            //console.log(`#tdR${_Row[i]}Pcs${_CookieLoginDate.slice(8, 10) + "-" + _CookieLoginDate.slice(5, 7) + "-" + _CookieLoginDate.slice(0, 4)}`);

            //$(`#tdR${_Row[i]}Pcs${_CookieLoginDate.slice(8, 10) + "-" + _CookieLoginDate.slice(5, 7) + "-" + _CookieLoginDate.slice(0, 4)}`).css("font-weight", "900");

        }
    }

    await periodFilter();
};

const periodFilter = async () => {

    if (_command == "Preview") return;
    let arr = [];

    for (let i = 0; i < dT_DeliveryDate.length; i++)
    {
        let deliveryDate = dT_DeliveryDate[i].F_Delivery_Date.slice(6, 8) + "-" + dT_DeliveryDate[i].F_Delivery_Date.slice(4, 6) + "-" + dT_DeliveryDate[i].F_Delivery_Date.slice(0, 4);
        let deliveryTrip = dT_DeliveryDate[i].F_Delivery_Trip;

        let trip = parseInt(dT_Period[0]["F_Period"]);

        if (_CookieLoginDate.slice(10, 11) == "N") trip = parseInt(dT_Period[0]["F_Period"]) + parseInt(dT_Period[1]["F_Period"]);


        if (arr.some(f => f.deliveryDate == deliveryDate && f.deliveryTrip == deliveryTrip)) continue;

        arr.push({ deliveryDate: deliveryDate, deliveryTrip: deliveryTrip });

        if (_CookieLoginDate.includes("D"))
        {

            for (trip; trip > 0; trip--)
            {
                for (let k = 2; k <= 20; k++)
                {
                    let _id = `tdR${k}T${trip}${deliveryDate}`;
                    $(`#${_id}`).css("background-color", "#FFFFFF");
                }
            }
        }
        else
        {
            for (trip; trip > parseInt(dT_Period[0]["F_Period"]); trip--)
            {
                for (let k = 2; k <= 20; k++) {
                    let _id = `tdR${k}T${trip}${deliveryDate}`;
                    $(`#${_id}`).css("background-color", "#FFFFFF");
                }
            }
        }
    }
}

const sumKB = async (dateSet) => {

    var _Row = [11, 13, 15, 17, 19];

    for (let i = 0; i < _Row.length; i++)
    {
        //var sum = 0;
        var _countDateSet = -1;

        //if (i == 0 || i == 1) {
            let $Id = "";
            $(`table tbody tr td[id*='tdR${_Row[i]}T']`).each(function () {
                $Id = $(this).attr("id");

                if ($Id.slice(-12, -10) == "T1") _countDateSet += 1;
                if ($Id.includes(dateSet[_countDateSet])) {
                    let kbToSum = parseInt($(`#${$Id}`).text());
                    //console.log(kbToSum);
                    //if (isNaN(sum) || sum === Infinity) sum = 0;
                    if (isNaN(kbToSum) || kbToSum === Infinity) kbToSum = 0;
                    let _kb = (parseInt($(`#tdR${_Row[i]}KB${dateSet[_countDateSet]}`).text())) == NaN ? 0 : parseInt($(`#tdR${_Row[i]}KB${dateSet[_countDateSet]}`).text());
                    _kb = isNaN(_kb) || _kb === Infinity ? 0 : _kb;

                    let kbResult = _kb + kbToSum;
                    //console.log($(`#tdR${_Row[i]}KB${dateSet[_countDateSet]}`).text());
                    //console.log(kbResult);
                    $(`#tdR${_Row[i]}KB${dateSet[_countDateSet]}`).text(kbResult);
                }

            });
            
        //}

        if (i >= 2) {
            _countDateSet = 0;
            $(`table tbody tr td[id*='tdR${_Row[i]}KB']`).each(function () {
                let $Id = $(this).attr("id");
                //console.log($Id);
                let $KB = parseInt($(`#${$Id}`).text());
                let $Pcs = $KB * parseInt($("#readQtyPack").val());
                if (isNaN($Pcs) || $Pcs === Infinity) $Pcs = 0;
                if (isNaN($KB) || $KB === Infinity) $KB = 0;

                //console.log("KB ", $KB, "date", dateSet[_countDateSet], "Pcs ", $Pcs);
                //console.log(`tdR${_Row[i]}Pcs${dateSet[_countDateSet]}`);

                $(`#tdR${_Row[i]}Pcs${dateSet[_countDateSet]}`).text($Pcs);
                $Pcs === 0 ? $(`#tdR${_Row[i]}KB${dateSet[_countDateSet]}`).text($Pcs) : $(`#tdR${_Row[i]}Kb${dateSet[_countDateSet]}`).text($KB);
                _countDateSet += 1;

            });
        }
    }

    let _OldDate = moment($("table thead tr[id='THeadR1'] th").eq(1).text(), "DD-MM-YYYY").format("YYYYMMDD");
    let _sumPattern = 0;

    dT_AdjustOrder_Trip.filter(x => x.F_Part_No + "-" + x.F_Ruibetsu == $("#readPartNo").val()
        && x.F_Supplier_Code == $("#readSupplier").val().split("-")[0]
        && x.F_Supplier_Plant == $("#readSupplier").val().split("-")[1]
        && x.F_Store_Code == $("#readStoreCode").val()
        && x.F_Kanban_No == $("#readKanbanNo").val()
        && x.F_Adj_Pattern != 0
    ).forEach(function (item, i) {
        console.log(item);
        let _pattern = parseInt(item.F_Adj_Pattern / parseInt($("#readQtyPack").val()));
        let _oldPattern = $(`#tdR15KB${item.F_Delivery_Date.slice(6, 8) + "-" + item.F_Delivery_Date.slice(4, 6) + "-" + item.F_Delivery_Date.slice(0, 4)}`).text();

        let _sumPattern = _pattern + parseInt(_oldPattern);

        //console.log(_sumPattern);
        $(`#tdR15KB${item.F_Delivery_Date.slice(6, 8) + "-" + item.F_Delivery_Date.slice(4, 6) + "-" + item.F_Delivery_Date.slice(0, 4)}`).text(_sumPattern);
        $(`#tdR15Pcs${item.F_Delivery_Date.slice(6, 8) + "-" + item.F_Delivery_Date.slice(4, 6) + "-" + item.F_Delivery_Date.slice(0, 4)}`).text(_sumPattern * parseInt($("#readQtyPack").val()));

    });

}

$(document).on('click', '#divCheckBox', function (e) {
    e.preventDefault();
    return;
});

$("#btnBlCal").click(function () {

    var obj = {
        action: "Re-Calculate BL",
        supplier: $("#readSupplier").val(),
        partNo: $("#readPartNo").val(),
        store: $("#readStoreCode").val(),
        kanban: $("#readKanbanNo").val(),
    }

    _xLib.AJAX_Post('/api/KBNOR121/Bl_Recalculate', JSON.stringify(obj),
        function (result) {
            if (result.status == 200) {
                previewFunction($("#txtPage").val().split("/")[0] - 1, _command);
                xSwal.success("Success", "Recalculate BL Success");
            }
        }, function (error) {
            xSwal.error("Error", error.responseJSON.message);
            console.error(error);
    });
})

$("#btnReCal").click(function () {

    var obj = {
        action: "Re-Calculate",
        supplier: $("#readSupplier").val(),
        partNo: $("#readPartNo").val(),
        store: $("#readStoreCode").val(),
        kanban: $("#readKanbanNo").val(),
    }

    _xLib.AJAX_Post('/api/KBNOR121/Recalculate', JSON.stringify(obj),
        function (result) {
            if (result.status == 200) {
                //previewFunction($("#txtPage").val().split("/")[0] - 1, _command);
                xSwal.success("Success", "Recalculate Success");

                _xLib.AJAX_Post('/api/KBNOR121/Bl_Recalculate', JSON.stringify(obj),
                    function (result) {
                        if (result.status == 200) {
                            previewFunction($("#txtPage").val().split("/")[0] - 1, _command);
                            xSwal.success("Success", "Recalculate BL Success");
                        }
                    },
                    function (error) {
                        xSwal.error("Error", error.responseJSON.message);
                        console.error(error);
                    }
                );

            }
        },
        function (error) {
            xSwal.error("Error", error.responseJSON.message);
            console.error(error);
        }
    );

});

$("#buttonCancel").click(function () {
    $("#divSelect , #divRead").find("input").val("");
    $("#divSelect , #divRead").find("select").val("");
    $("#divSelect , #divRead").find("textarea").val("");
    $("#txtPage").val("1/1");
    $("#tableMaster thead tr").empty();
    $("#tableMaster tbody tr td").remove();
});

$("#buttonExit").click(function () {
    window.location.href = "KBNOR100";
});