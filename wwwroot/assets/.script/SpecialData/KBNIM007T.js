$(document).ready(async function () {

    await _xDataTable.InitialDataTable("#tableSub",
        {
            columns: [
                { title: "Order No.", data: "F_PDS_No" },
                { title: "Parent Part", data: "F_Part_No" },
                { title: "Store Cd.", data: "F_Store_CD" },
                { title: "Supplier", data: "F_Supplier" },
                { title: "Deli Date", data: "F_Delivery_Date" },
                { title: "Trip", data: "F_Round" },
                { title: "Qty", data: "F_Qty" },
                { title: "Child Part", data: "F_Child_Part" },
                { title: "ST. Child", data: "F_Store_Child" },
                { title: "Child Name", data: "F_Part_Name" },
                { title: "Order Type", data: "F_OrderType" },
            ],
            scrollX: false,
            order: [[1, "asc"]],
            scrollCollapse: false,
            scrollY: 1000,
        }
    );

    await $("#inpDate").initDatepicker();
   

    await GetPO();

    xSplash.hide();

    await ShowCalendar();
    //$("#inpDate").parent().parent().find("div").remove();
});

var _command = "";

$("#btnNew").click(async () => {
    await GetParentStoreCD();
    await GetCompStoreCD();

    if ($("#selCustomerOrder").prop("tagName") == "SELECT") {

        $("#selCustomerOrder").parent().remove();
        $("label[for='selCustomerOrder']").parent().append("<input type='text' class='form-control col-5' id='selCustomerOrder' name='selCustomerOrder' />");
        $("#selCustomerOrder").val("");
    }


    $("#divInput").find("input").prop("disabled", false);
    $("#divInput").find("select").prop("disabled", false);
    $(".selectpicker").selectpicker("refresh");
    $("#divBtn").find("button").prop("disabled", true);

    $("#btnNew").prop("disabled", false);
    $("#btnCan").prop("disabled", false);
    $("#btnSave").prop("disabled", false);

    _command = "NEW";

});

$("#btnDel").click(async () => {
    $("#divInput").find("input").prop("disabled", false);
    $("#divInput").find("select").prop("disabled", false);
    $("#divInput").find("select").each(function () {
        $(this).selectpicker("refresh");
    });

    $("#divBtn").find("button").prop("disabled", true);
    $("#btnDel").prop("disabled", false);
    $("#btnCan").prop("disabled", false);
    $("#btnSave").prop("disabled", false);

    await ShowCalendar();
    await GetPO();

    _command = "DEL";
});

$("#btnUpd").click(async () => {
    $("#divInput").find("input").prop("disabled", false);
    $("#divInput").find("select").prop("disabled", false);
    $("#divInput").find("select").each(function () {
        $(this).selectpicker("refresh");
    });
    $("#divBtn").find("button").prop("disabled", true);
    $("#btnUpd").prop("disabled", false);
    $("#btnCan").prop("disabled", false);
    await ShowCalendar();
    await GetPO();
    _command = "UPD";

    $("#btnSave").prop("disabled", false);
});

$("#btnInq").click(async () => {
    $("#divInput").find("input").prop("disabled", false);
    $("#divInput").find("select").prop("disabled", false);
    $("#divInput").find("select").each(function () {
        $(this).selectpicker("refresh");
    });
    $("#divBtn").find("button").prop("disabled", true);
    $("#btnInq").prop("disabled", false);
    $("#btnCan").prop("disabled", false);
    $("#btnRptPar").prop("disabled", false);
    $("#btnRpt").prop("disabled", false);
    await ShowCalendar();
    await GetPO();

    _command = "INQ";

});

$("#mthDeliYM").change(async () => {
    await ShowCalendar();
    await GetPO();
});

$("#btnRptPar").click(async () => {
    let obj = {
        Plant: _xLib.GetCookie("plantCode"),
        UserName: _xLib.GetUserName(),
        F_PDS_No: $("#selCustomerOrder").val().trim(),
        F_Part_Order: $("#selPartNo").val() ? $("#selPartNo").val().split("-")[0] : "",
        F_Ruibetsu_Order: $("#selPartNo").val() ? $("#selPartNo").val().split("-")[1] : "",
        F_Store_CD: $("#selStoreCode").val(),
        F_Delivery_Date: $("#mthDeliYM").val().replaceAll("-", ""),
    }

    _xLib.OpenReportObj("/KBNIM007_Parent", obj);

});

$("#btnRpt").click(async () => {
    let obj = {
        Plant: _xLib.GetCookie("plantCode"),
        UserName: _xLib.GetUserName(),
        F_PDS_No: $("#selCustomerOrder").val().trim(),
        F_Part_Order: $("#selPartNo").val() ? $("#selPartNo").val().split("-")[0] : "",
        F_Ruibetsu_Order: $("#selPartNo").val() ? $("#selPartNo").val().split("-")[1] : "",
    }

    _xLib.OpenReportObj("/KBNIM007", obj);
});

$(document).on("change", "#selCustomerOrder", async () => {
    await GetParentStoreCD();
    await GetCompStoreCD()
    //await ListDataTable();
});

$("#selStoreCode").change(async () => {
    await GetParentPartNo();
});

$("#selPartNo").change(async () => {
    //await PartNoSelected();
    await GetCompStoreCD();
    await SetCalendar();
    await GetParentPartDetail();
    await ListCalendar();
    await ListDataTable();
});

$("#selCompStoreCode").change(async () => {
    await GetCompPartNo();
});

$("#selCompPartNo").change(async () => {
    await ComponentStoreSelected();
    await ComponentPartSelected();
    await SetCalendar();
    await ListCalendar();
    await ListDataTable();
});

$("#btnCan").click(async () => {
    if ($("#selCustomerOrder").prop("tagName") == "INPUT") {
        $("#selCustomerOrder").remove();
        $("label[for='selCustomerOrder']").parent().append("<select class='selectpicker col-5 p-0' id='selCustomerOrder' name='selCustomerOrder' disabled></select>");
        $("#selCustomerOrder").append("<option value='' hidden></option>");
        $("#selCustomerOrder").selectpicker("refresh");
    }
    $("#divInput").find("select").each(function () {
        $(this).empty();
        $(this).prop("disabled", true);
        $(this).resetSelectPicker();
    });

    $("#divBtn").find("button").prop("disabled", false);
    $("#btnCan").prop("disabled", true);
    $("#btnSave").prop("disabled", true);

    $("#spanPartName").text("");
    $("#spanPartName").addClass("invisible");
    $("#spanCompPartName").text("");
    $("#spanCompPartName").addClass("invisible");

    $("#tableSub").DataTable().clear().draw();
    $("#tableMain").find("input").val("");
    _command = "";

    await ShowCalendar();
});

$("#btnSave").click(async () => {
    let isConfirm = await xSwal.confirm("Are you sure?", "This action can't be undone");
    if (!isConfirm) return;
    await Save();
});

ShowCalendar = async () => {
    $("#tableMain").find("tbody").remove();
    let YM = $("#mthDeliYM").val();

    let Table = $("#tableMain");
    Table.append("<tbody></tbody>");

    let day = 1;
    const startDate = new Date(YM + "-01");
    const endDate = new Date(startDate.getFullYear(), startDate.getMonth() + 1, 0);

    for (let mockDay = 1; mockDay <= 42; mockDay++) {
        //let readonly = "readonly";

        if (mockDay % 7 == 1) {
            Table.find("tbody").append("<tr>");
        }

        if (mockDay > startDate.getDay() && mockDay <= endDate.getDate() + startDate.getDay()) {
            Table.find("tbody").find("tr:last").append(
                `<td class='text-center'>
                            <span class='fs-6'>${day}</span></br>
                            <div class='row justify-content-center mt-1'>
                                <input type='number' class='form-control fw-bolder' min='0' max='9999' style='width: 85%;'
                                    id='QTY_${YM.replaceAll("-", "")}${day.toString().length == 1 ? "0" + day : day}'
                                    name='QTY_${YM.replaceAll("-", "")}${day.toString().length == 1 ? "0" + day : day}' />
                            </div>    
                        </td >`
            );
            day++;
            //console.log(_command);
            if (_command == "INQ") {
                Table.find("tbody").find("tr:last").find("input").prop("readonly", true);
            }
        } else {
            Table.find("tbody").find("tr:last").append("<td></td>");
        }

        if (mockDay % 7 == 0) {
            Table.find("tbody").append("</tr>");
        }
    }

    $("#tableMain").find("tbody tr:last").find("td").each(function () {
        if ($(this).text() == "") {
            $(this).remove();
        }
    });

}

GetPO = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007T/GetPO", obj,
        async (success) => {
            //console.log(success);
            $("#selCustomerOrder").empty();
            $("#selCustomerOrder").append("<option value='' hidden></option>");
            success.data.forEach((item) => {
                $("#selCustomerOrder").append(`<option value='${item}'>${item}</option>`);
            });
            $("#selCustomerOrder").selectpicker("refresh");
        }
    );
}

GetParentStoreCD = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007T/GetParentStore", obj,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#selStoreCode").empty();
            $("#selStoreCode").append("<option value='' hidden></option>");
            success.data.forEach((item) => {
                $("#selStoreCode").append(`<option value='${item.F_Store_Cd}'>${item.F_Store_Cd}</option>`);
            });
            $("#selStoreCode").selectpicker("refresh");
        }
    );
}

GetParentPartNo = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007T/GetParentPart", obj,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#selPartNo").empty();
            $("#selPartNo").append("<option value='' hidden></option>");
            success.data.forEach((item) => {
                $("#selPartNo").append(`<option value='${item.F_Part_No}'>${item.F_Part_No}</option>`);
            });
            $("#selPartNo").selectpicker("refresh");
        }
    );
}

GetCompStoreCD = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007T/GetComponentStore", obj,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#selCompStoreCode").empty();
            $("#selCompStoreCode").append("<option value='' hidden></option>");
            success.data.forEach((item) => {
                $("#selCompStoreCode").append(`<option value='${item.F_Store_Cd}'>${item.F_Store_Cd}</option>`);
            });
            $("#selCompStoreCode").selectpicker("refresh");
        }
    );
}

GetCompPartNo = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007T/GetComponentPartNo", obj,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#selCompPartNo").empty();
            $("#selCompPartNo").append("<option value='' hidden></option>");
            success.data.forEach((item) => {
                $("#selCompPartNo").append(`<option value='${item.F_Part_No}'>${item.F_Part_No}</option>`);
            });
            $("#selCompPartNo").selectpicker("refresh");
        }
    );
}

GetParentPartDetail = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007T/GetParentPartDetail", obj,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#spanPartName").text(success.data[0].F_Part_NM);
            $("#spanPartName").removeClass("invisible");
        }
    );
}
ComponentStoreSelected = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007T/ComponentStoreSelected", obj,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            //console.log($("#selCompSupplier"));
            //$("#selCompSupplier").addListSelectPicker(success.data[1], "F_Supplier_Cd");
            $("#selCompSupplier").empty();
            console.log(success.data[1]);
            console.log((success.data[1]));
            if ((success.data[1]) != undefined) {
                $("#selCompSupplier").append(`<option value=${success.data[1][0].F_Supplier_Cd}>${success.data[1][0].F_Supplier_Cd}</option>`);
                $("#selCompSupplier").selectpicker("val", success.data[1][0].F_Supplier_Cd);
            }
            else {
                $("#selCompSupplier").append(`<option value=${success.data[0][0].F_Supplier_Cd}>${success.data[0][0].F_Supplier_Cd}</option>`);
                $("#selCompSupplier").selectpicker("val", success.data[0][0].F_Supplier_Cd);
            }
            $("#selCompSupplier").selectpicker("refresh");
        }
    );
}

var QtyBox = 0;
var Sebango = "";
ComponentPartSelected = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007T/ComponentPartSelected", obj,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#spanCompPartName").text(success.data[0].F_Part_NM);
            $("#spanCompPartName").removeClass("invisible");
            $('#spanCompSupplierName').text(success.data[0].F_Sup_name);
            $('#spanCompSupplierName').removeClass("invisible");
            QtyBox = success.data[0].F_QTY_BOX;
            Sebango = success.data[0].F_Sebango;
        }
    );
}


ListCalendar = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007T/ListCalendar", obj,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#tableMain").find("input").val("");
            $(success.data).each(function () {
                let date = moment(this.F_Delivery_Date, "DD/MM/YYYY").format("YYYYMMDD");
                $(`#QTY_${date}`).val(this.F_Qty);
            });
        }
    );
}

SetCalendar = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007T/SetCalendar", obj,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            if (_command == "INQ") {
                $("#tableMain").find("input").prop("readonly", true);
            }
            else {
                $("#tableMain").find("input").prop("readonly", false);
            }
            $("#tableMain").find("input").removeClass("bg-warning");
            $("#tableMain").find("input").removeClass("bg-danger");
            Object.keys(success.data[0]).forEach((key) => {
                //console.log(key);
                if (key.includes("F_workCd_D")) {
                    let date = key.split("F_workCd_D")[1];
                    //console.log(date);
                    if (date.length == 1) date = "0" + date;
                    if (success.data[0][key] == "1") {
                        //console.log(success.data[0].F_Date);
                        if (success.data[0].F_Date <= obj.YM + date) {
                            if (_command != "INQ") {
                                $(`#QTY_${obj.YM}${date}`).prop("readonly", false);
                            }
                        }
                        else {
                            $(`#QTY_${obj.YM}${date}`).prop("readonly", true);
                            $(`#QTY_${obj.YM}${date}`).addClass("bg-warning");
                        }
                    }
                    else {
                        $(`#QTY_${obj.YM}${date}`).prop("readonly", true);
                        $(`#QTY_${obj.YM}${date}`).addClass("bg-danger");
                    }
                }
            });
        }
    );
}

getQueryObj = async () => {
    let obj = {
        YM: $("#mthDeliYM").val().replaceAll("-", ""),
        PO: $("#selCustomerOrder").val(),
        IssuedDate : moment($("#inpDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        ParentStoreCD: $("#selStoreCode").val(),
        ParentPartNo: $("#selPartNo").val(),
        CompStoreCD: $("#selCompStoreCode").val(),
        CompPartNo: $("#selCompPartNo").val(),
        isNew: !($("#btnNew").prop("disabled")),
        TypeSpc: $("input[name='rdoType']:checked").val(),
    }
    return obj;
}

getSaveObj = async () => {
    let list = [];
    $("#tableMain").find("input").each(function () {

        let obj = {
            PO: $("#selCustomerOrder").val(),
            IssuedDate: moment($("#inpDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
            ParentPartNo: $("#selPartNo").val(),
            ParentStore: $("#selStoreCode").val(),
            DeliveryDate: $("#mthDeliYM").val().replaceAll("-", ""),
            Qty: 0,
            Trip: 1,
            CompPartNo: $("#selCompPartNo").val(),
            CompStoreCD: $("#selCompStoreCode").val(),
            CompSebango: Sebango,
            SupplierCD: $("#selCompSupplier").val(),
            TypeSpc: $("input[name='rdoType']:checked").val(),
            CompPartName: $("#spanCompPartName").text(),
            QtyBox: QtyBox,
        }


        if ($(this).val() != "") {
            obj.Qty = $(this).val();
            let date = $(this).attr("id").split("_")[1];
            obj.DeliveryDate = date;
            list.push(obj);
        }

        //for Delete Command and Update Command
        if (_command == "DEL") {
            list.push(obj);
        }


    });

    return list;
}

Save = async () => {
    let listObj = await getSaveObj();

    _xLib.AJAX_Post("/api/KBNIM007T/Save?action=" + _command, listObj,
        async (success) => {
            //console.log(success);
            xSwal.success(success.response, success.message);
            $("#btnCan").click();
        }
    );
}

ListDataTable = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007T/ListDatatable", obj,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            _xDataTable.ClearAndAddDataDT("#tableSub", success.data);
            //$("#tableSub").DataTable().clear().draw();
            //$("#tableSub").DataTable().rows.add(success.data).draw();
        }
    );
}