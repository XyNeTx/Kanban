let dT_Actual_Receive = [];
let dT_AdjustOrder_Trip = [];
let dT_Date = [];
let dT_DeliveryDate = [];
let dT_Detail = [];
let dT_Header = [];
let dT_PartControl = [];
let dT_Period = [];
let dT_Volume = [];
let action = "";

$(document).ready(async function () {
    await Onload();
    await GetDropDownData();
    //await Find_StartEnd_Date();
    xSplash.hide()
});

async function GetDropDownData() {
    let obj = {
        F_Supplier_Code: $("#inputSupplier").val(),
        F_Store_Code: $("#inputStore").val(),
    }
    return _xLib.AJAX_Get("/api/KBNOR321/GetDropDownData", obj,
        async function (success) {
            console.log(success);
            if ($("#inputSupplier").val() == "") {
                $("#inputSupplier").empty();
                $("#inputSupplier").append("<option value='' hidden></option>");
                success.data[0].forEach(function (e) {
                    $("#inputSupplier").append(`<option value=${e}>${e}</option>`)
                })
            }
            if ($("#inputKanban").val() == "") {
                $("#inputKanban").empty();
                $("#inputKanbanTo").empty();
                $("#inputKanban").append("<option value='' hidden></option>");
                $("#inputKanbanTo").append("<option value='' hidden></option>");
                success.data[1].forEach(function (e) {
                    $("#inputKanban").append(`<option value=${e}>${e}</option>`)
                    $("#inputKanbanTo").append(`<option value=${e}>${e}</option>`)
                })
            }
            if ($("#inputStore").val() == "") {
                $("#inputStore").empty();
                $("#inputStoreTo").empty();
                $("#inputStore").append("<option value='' hidden></option>");
                $("#inputStoreTo").append("<option value='' hidden></option>");
                success.data[2].forEach(function (e) {
                    $("#inputStore").append(`<option value=${e}>${e}</option>`)
                    $("#inputStoreTo").append(`<option value=${e}>${e}</option>`)
                })
            }
            if ($("#inputPart").val() == "") {
                $("#inputPart").empty();
                $("#inputPartTo").empty();
                $("#inputPart").append("<option value='' hidden></option>");
                $("#inputPartTo").append("<option value='' hidden></option>");
                success.data[3].forEach(function (e) {
                    $("#inputPart").append(`<option value=${e}>${e}</option>`)
                    $("#inputPartTo").append(`<option value=${e}>${e}</option>`)
                })
            }
        },
    )
}

$("#inputSupplier").change(function () {
    GetDropDownData();
});
$("#inputKanban").change(function () {
    $("#inputKanbanTo").val($(this).val());
    GetDropDownData();
});
$("#inputStore").change(function () {
    $("#inputStoreTo").val($(this).val());
    GetDropDownData();
});
$("#inputPart").change(function () {
    $("#inputPartTo").val($(this).val());
    GetDropDownData();
});

$("#btnPrevPage").click(async function () {
    var page = parseInt($("#txtPage").val().split("/")[0]) - 1;
    if (page == 0) return;
    $("#txtPage").val(page.toString() + "/" + dT_PartControl.length.toString());
    await Get_All_Data(action, parseInt(page) - 1);
    window.scrollTo(0, 0);
});

$("#btnNextPage").click(async function () {
    var page = parseInt($("#txtPage").val().split("/")[0]) + 1;
    if (page == parseInt($("#txtPage").val().split("/")[1]) + 1) return;
    $("#txtPage").val(page.toString() + "/" + dT_PartControl.length.toString());
    await Get_All_Data(action, parseInt(page) - 1);
    window.scrollTo(0, 0);
});

$("#btnProcess").click(async function () {
    action = "Process";
    await Get_All_Data("Process",0);
})
$("#btnPreview").click(async function () {
    action = "Preview";
    await Get_All_Data("Preview",0);
})

async function GetSelectObject(action) {
    let obj = {
        action: action,
        F_Supplier_Code: $("#inputSupplier").val(),
        F_KanbanFrom: $("#inputKanban").val(),
        F_KanbanTo: $("#inputKanbanTo").val(),
        F_StoreFrom: $("#inputStore").val(),
        F_StoreTo: $("#inputStoreTo").val(),
        F_PartFrom: $("#inputPart").val(),
        F_PartTo: $("#inputPartTo").val(),
    }

    return obj;
}

async function Get_All_Data(action,intRow) {
    let obj = await GetSelectObject(action);
    return _xLib.AJAX_Get("/api/KBNOR321/Get_All_Data", obj,
        async function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            dT_Actual_Receive = success.data.dT_Actual_Receive
            dT_AdjustOrder_Trip = success.data.dT_AdjustOrder_Trip
            dT_Date = success.data.dT_Date
            dT_DeliveryDate = success.data.dT_DeliveryDate
            dT_Detail = success.data.dT_Detail
            dT_Header = success.data.dT_Header
            dT_PartControl = success.data.dT_PartControl
            dT_Period = success.data.dT_Period
            dT_Volume = success.data.dT_Volume

            //console.log(dT_PartControl);

            $("#txtPage").val((parseInt(intRow) + 1) + "/" + dT_PartControl.length);
            await Set_Dynamic_Table();
            Set_Data(intRow);
        },
        async function (error) {
            xSwal.xError(error);
        }
    )
}

async function Set_Dynamic_Table()
{
    let _oldDate = "";
    let _headColumn = 0;
    let _sumT = 0;

    if ($(".tempHead").length > 0)
    {
        $(".tempHead").remove();
        $(".tempDetail").remove();
    }

    dT_Period.forEach(function (e,i) {
        _headColumn += (e.F_Period + 2);

        if (i == 0) {
            _headColumn += (e.F_Period + 2);
        }

        if (_oldDate != e.Date_Now) {
            let DateFormated = moment(e.Date_Now, "YYYYMMDD").format("DD-MM-YYYY");
            $("#THeadR1").append(`<th class='tempHead' id='thDate_${e.Date_Now}' colspan='${_headColumn}'>${DateFormated}</th>`)
            _oldDate = e.Date_Now;

            for (let i = 1; i <= 18; i++) {
                let T = 1;
                for (let j = 1; j <= _headColumn; j++) {
                    if (T > (_headColumn - 4)) {
                        T = 1;
                    }
                    let _id = "";
                    if (j % 3 == 1) {
                        _id = `TD_R${i}_T${T}_Pcs_` + e.Date_Now;
                    }
                    else if (j % 3 == 2) {
                        _id = `TD_R${i}_T${T}_KB_` + e.Date_Now;
                    }
                    else {
                        _id = `TD_R${i}_T${T}_` + e.Date_Now;
                        T++;
                    }
                    //$(`#TBodyR${i}`).append(`<td class='tempDetail' id='${_id}'>${_id}</td>`)
                    $(`#TBodyR${i}`).append(`<td class='tempDetail' id='${_id}'></td>`)
                }
            }
            _headColumn = 0;
            _sumT = 0;
        }

        $("#THeadR2").append(`<th class='tempHead' id='thBlankCol' colspan='2'></th>`)

        if (e.Row_Num == "1") $("#THeadR2").append(`<th class='tempHead text-danger' id='thDay_${e.Date_Now}' style='background-color:#fba8a1;' >Day</th>`);
        else $("#THeadR2").append(`<th class='tempHead' id='thNight_${e.Date_Now}' style='background-color:#86b6fd;color:#001eff'>Night</th>`)

        $("#THeadR3").append(`<th class='tempHead' id='thPcs_${e.Date_Now}'>Pcs</th>`)
        $("#THeadR3").append(`<th class='tempHead' id='thKB_${e.Date_Now}'>KB</th>`)
        for (let i = 1; i <= e.F_Period; i++) {
            $("#THeadR3").append(`<th class='tempHead' id='thT${_sumT + i}_${e.Date_Now}'>T${_sumT + i}</th>`)
            _sumT += e.F_Period;
        }
    })

    return;

}

async function Set_Data(intRow)
{
    let keyHead = ["F_TMT_FO", "F_MRP", "F_AbNormal_Part", "F_Total", "F_Remain_LastTrip", "F_Remain_LastClear"
        , "F_Order_Base", "F_Lot_SizeOrder + F_Qty_Box", "F_KB_CutAdd", "F_Actual_Order"
        , "F_Urgent_Order", "F_UrgentTemp_Order", "F_Pattern_Order", "","F_Delivery_Total"];

    let keyDetail = ["F_TMT_FO", "F_MRP", "", "", "", "", "", "F_Lot_SizeOrder"
        , "F_KB_CutADD", "F_Actual_order", "F_Urgent_Order", "F_UrgentTemp_Order", "", "", "F_Receive_Plan", "F_BL_Plan", "","F_BL_Actual"];

    //console.log(keyHead);

    $("#readSupplier").val(dT_PartControl[intRow].F_Supplier_Code + "-" + dT_PartControl[intRow].F_Supplier_Plant);
    $("#readPartNo").val(dT_PartControl[intRow].F_Part_No + "-" + dT_PartControl[intRow].F_Ruibetsu);
    $("#readStoreCode").val(dT_PartControl[intRow].F_Store_Code);
    $("#readKanbanNo").val(dT_PartControl[intRow].F_Kanban_No);
    $("#readCycleTime").val(dT_Date[dT_Date.length - 1].F_Cycle.slice(0, 2) + "-" + dT_Date[dT_Date.length - 1].F_Cycle.slice(2, 4) + "-" + dT_Date[dT_Date.length - 1].F_Cycle.slice(4, 6));


    for (let j = 0; j < dT_Period.length; j++) {
        var headData = dT_Header.filter(x =>
            x.F_Supplier_Code == dT_PartControl[intRow].F_Supplier_Code &&
            x.F_Supplier_Plant == dT_PartControl[intRow].F_Supplier_Plant &&
            x.F_Part_No == dT_PartControl[intRow].F_Part_No &&
            x.F_Ruibetsu == dT_PartControl[intRow].F_Ruibetsu &&
            x.F_Store_Code == dT_PartControl[intRow].F_Store_Code &&
            x.F_Kanban_No == dT_PartControl[intRow].F_Kanban_No &&
            x.F_Process_Date == dT_Period[j].Date_Now &&
            x.F_Process_Shift == (dT_Period[j].Row_Num == "1" ? "D" : "N")
        )[0];

        $("#readQtyPack").val((headData.F_Qty_Box == 0 ? 1 : headData.F_Qty_Box));
        $("#readUseDay").val(headData.F_TMT_FO);

        var detailData = dT_Detail.filter(x =>
            x.F_Supplier_Code == dT_PartControl[intRow].F_Supplier_Code &&
            x.F_Supplier_Plant == dT_PartControl[intRow].F_Supplier_Plant &&
            x.F_Part_No == dT_PartControl[intRow].F_Part_No &&
            x.F_Ruibetsu == dT_PartControl[intRow].F_Ruibetsu &&
            x.F_Kanban_No == dT_PartControl[intRow].F_Kanban_No &&
            x.F_Store_Code == dT_PartControl[intRow].F_Store_Code &&
            x.F_Process_Date == dT_Period[j].Date_Now &&
            x.F_Process_Shift == (dT_Period[j].Row_Num == "1" ? "D" : "N") &&
            x.F_Process_Round == dT_Period[j].Row_Num
        )[0];

        var volumeData = dT_Volume.filter(x =>
            x.F_Supplier_Code == dT_PartControl[intRow].F_Supplier_Code &&
            x.F_Supplier_Plant == dT_PartControl[intRow].F_Supplier_Plant &&
            x.F_Part_No == dT_PartControl[intRow].F_Part_No &&
            x.F_Ruibetsu == dT_PartControl[intRow].F_Ruibetsu &&
            x.F_Kanban_No == dT_PartControl[intRow].F_Kanban_No &&
            x.F_Store_Code == dT_PartControl[intRow].F_Store_Code &&
            x.F_Delivery_Date == dT_Period[j].Date_Now &&
            //x.F_Process_Shift == (dT_Period[j].Row_Num == "1" ? "D" : "N") &&
            x.F_Delivery_Round == dT_Period[j].Row_Num
        )[0];

        var actualData = dT_Actual_Receive.filter(x =>
            x.F_Supplier_code == dT_PartControl[intRow].F_Supplier_Code &&
            x.F_Supplier_Plant == dT_PartControl[intRow].F_Supplier_Plant &&
            x.F_PART_No == dT_PartControl[intRow].F_Part_No &&
            x.F_RUibetsu == dT_PartControl[intRow].F_Ruibetsu &&
            // x.F_Kanban_No == dT_PartControl[intRow].F_Kanban_No &&
            x.F_Store_Cd == dT_PartControl[intRow].F_Store_Code &&
            x.F_Receive_Date == dT_Period[j].Date_Now
            //x.F_Process_Shift == (dT_Period[j].Row_Num == "1" ? "D" : "N") &&
            // x.F_Delivery_Trip == dT_Period[j].Row_Num
        );

        await GetBL(dT_Period[j].Date_Now, dT_Period[j].Row_Num, intRow);

        //console.log(headData);
        //console.log(detailData);
        //console.log(volumeData);
        //console.log(dT_Period[j].Date_Now);
        //console.log(dT_Period[j].Row_Num);
        //console.log(actualData);

        for (let i = 1; i <= 18; i++) {
            try {
                //console.log(dT_Period);
                //console.log(dT_PartControl[intRow].F_Part_No);
                //console.log(dT_PartControl[intRow].F_Ruibetsu);
                //console.log(dT_PartControl[intRow].F_Kanban_No);
                //console.log(dT_PartControl[intRow].F_Store_Code);
                //console.log(dT_Period[j].Date_Now);
                //console.log(dT_Period[j].Row_Num == "1" ? "D" : "N");

                let _idPcs = `TD_R${i}_T${dT_Period[j].Row_Num}_Pcs_${dT_Period[j].Date_Now}`;
                let _idKB = `TD_R${i}_T${dT_Period[j].Row_Num}_KB_${dT_Period[j].Date_Now}`;
                let _idT = `TD_R${i}_T${dT_Period[j].Row_Num}_${dT_Period[j].Date_Now}`;

                //console.log(keyHead[i - 1]);
                //console.log(headData[keyHead[i - 1]]);
                //console.log(_idPcs);

                if (i == 8) {
                    let Total_Order = headData.F_Lot_SizeOrder * headData.F_Qty_Box;
                    $(`#${_idPcs}`).text(Total_Order);
                    $(`#${_idKB}`).text(headData.F_Lot_SizeOrder);
                }
                else if (i == 9 || i == 10) {
                    $(`#${_idKB}`).text((headData[keyHead[i - 1]] == undefined ? 0 : headData[keyHead[i - 1]]));
                    if (i == 10) $(`#${_idPcs}`).text((headData[keyHead[i - 1]] == undefined ? 0 : headData[keyHead[i - 1]] * headData.F_Qty_Box));
                    else if (i != 9) $(`#${_idPcs}`).text((headData[keyHead[i - 1]] == undefined ? 0 : headData[keyHead[i - 1]]));
                }
                else if (i == 12) {
                    $(`#${_idPcs}`).text((headData[keyHead[i - 1]] == undefined ? 0 : headData[keyHead[i - 1]]));
                    //$(`#${_idT}`).text((headData[keyHead[i - 1]] == undefined ? "" : headData[keyHead[i - 1]] == 0 ? "" : headData[keyHead[i - 1]]));
                }
                else if (i < 13) {
                    $(`#${_idPcs}`).text((headData[keyHead[i - 1]] == undefined ? 0 : headData[keyHead[i - 1]]));
                }
                else if (i == 13) {
                    //$(`#${_idKB}`).text((headData[keyHead[i - 1]] == undefined ? "" : headData[keyHead[i - 1]] == 0 ? "" : headData[keyHead[i - 1]]));
                    //$(`#${_idPcs}`).text((headData[keyHead[i - 1]] == undefined ? "" : headData[keyHead[i - 1]] == 0 ? "" : headData[keyHead[i - 1]]));
                    $(`#${_idKB}`).text((headData[keyHead[i - 1]] == undefined ? headData[keyHead[i - 1]] == 0 ? "" : headData[keyHead[i - 1]] : headData[keyHead[i - 1]]));
                    $(`#${_idPcs}`).text((headData[keyHead[i - 1]] == undefined ? headData[keyHead[i - 1]] == 0 ? "" : headData[keyHead[i - 1]] : headData[keyHead[i - 1]]));
                }
                //else if (i == 15)
                //{
                //    console.log(keyHead);
                //    conzole.log(i - 1);
                //    console.log(headData[keyHead[i - 1]]);
                //    $(`#${_idKB}`).text((headData[keyHead[i - 1]] == undefined ? 0 : headData[keyHead[i - 1]]));
                //    $(`#${_idPcs}`).text((headData[keyHead[i - 1]] == undefined ? 0 : headData[keyHead[i - 1]] * headData.F_Qty_Box));
                //}

                //for DetailData
                if (i <= 2 || (i >= 8 && i <= 12) || i == 16 || i == 18)
                {
                    if (i === 11 || i === 12) {
                        $(`#${_idT}`).text(detailData[keyDetail[i - 1]] == 0 ? "" : detailData[keyDetail[i - 1]]);
                    }
                    else $(`#${_idT}`).text(detailData[keyDetail[i - 1]]);
                }

                //for VolumeData
                if (i == 15) { //|| i == 17) {
                    if (volumeData !== undefined) {
                        if (volumeData.F_Qty !== undefined) {
                            $(`#${_idPcs}`).text(volumeData.F_Qty);
                            $(`#${_idKB}`).text(volumeData.F_Receive_Plan);
                            $(`#${_idT}`).text(volumeData.F_Receive_Plan);
                        }
                        else {
                            $(`#${_idPcs}`).text("0");
                            $(`#${_idKB}`).text("0");
                            $(`#${_idT}`).text("0");
                        }
                    }
                    else if (headData !== undefined && detailData !== undefined) {
                        $(`#${_idPcs}`).text("0");
                        $(`#${_idKB}`).text("0");
                        $(`#${_idT}`).text("0");
                    }
                }

                if (i === 17)
                {
                    let sumReceiveActual = 0;
                    $(`#${_idT}`).text("0");
                    actualData.forEach(function (x) {
                        sumReceiveActual += x.F_Receive_QTY;
                    })

                    $(`#${_idKB}`).text(sumReceiveActual / headData.F_Qty_Box);
                    $(`#${_idPcs}`).text(sumReceiveActual);

                    if (dT_Period[j].Row_Num == "1") {
                        if (actualData != undefined) {
                            $(`#${_idT}`).text(actualData[0].F_Receive_QTY / headData.F_Qty_Box);
                        }
                    }
                    if (dT_Period[j].Row_Num == "2") {
                        if (actualData != undefined) {
                            $(`#${_idT}`).text(actualData[1].F_Receive_QTY / headData.F_Qty_Box);
                        }
                    }

                }

            }
            catch (e) {
                //console.log(e);
                continue;
            }
        }
    }

}

async function GetBL(strDate, Row_Num,intRow) {
    let obj = {
        strDate: strDate,
        Row_Num: Row_Num,
        intRow: intRow
    }
    return _xLib.AJAX_Get("/api/KBNOR321/GetBL", obj,
        async function (success) {
            console.log(success);

            $(`#TD_R16_T${Row_Num}_Pcs_${strDate}`).text(success.data[0])
            $(`#TD_R18_T${Row_Num}_Pcs_${strDate}`).text(success.data[1])

            if (success.data[2] === "True") {
                $(`#TD_R16_T${Row_Num}_Pcs_${strDate}`).css("font-weight", "800");
                $(`#TD_R18_T${Row_Num}_Pcs_${strDate}`).css("font-weight", "800");
            }

        },
    )
}

async function Onload() {
    let obj = {
        _loginDate: _xLib.GetCookie('loginDate')
    }
    return _xLib.AJAX_Get("/api/KBNOR321/Onload", obj,
        async function (success) {
            console.log(success);
        },
    )
}