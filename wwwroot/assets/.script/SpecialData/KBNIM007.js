$(document).ready(async function () {

    await _xDataTable.InitialDataTable("#tableSub",
        {
            columns: [
                { title: "Order No.", data: "F_PDS_NO" },
                { title: "Parent Part", data: "F_Part_No" },
                { title: "Store Cd.", data: "F_Store_CD" },
                { title: "Supplier", data: "F_Supplier" },
                { title: "Deli Date", data: "F_Delivery_Date" },
                { title: "Trip", data: "F_Round" },
                { title: "Qty", data: "F_Qty" },
                { title: "Child Part", data: "F_Child_Part" },
                { title: "ST. Child", data: "F_Store_Child" },
                { title: "Child Name", data: "F_Child_Name" },
                { title: "Order Type", data: "F_OrderType" },
            ],
            scrollX : true,
            order: [[1, "asc"]],
            scrollCollapse: false,
            scrollY: '500px',
        }
    );

    $("#inpDate").initDatepicker();

    await GetPO();

    xSplash.hide();

    await ShowCalendar();
});

$("#btnNew").click(async () => {
    await GetStoreCD();
    //await GetPartNo();

    $("#selCustomerOrder").parent().remove();
    $("label[for='selCustomerOrder']").parent().append("<input type='text' class='form-control col-5' id='selCustomerOrder' name='selCustomerOrder' />");
    $("#selCustomerOrder").val("");
    $("#divInput").find("input").prop("disabled", false);
    $("#divInput").find("select").prop("disabled", false);
    $(".selectpicker").selectpicker("refresh");
    $("#divBtn").find("button").prop("disabled", true);
    $("#btnNew").prop("disabled", false);
    $("#btnCan").prop("disabled", false);
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
    await ShowCalendar();
    await GetPO();

});

$("#mthDeliYM").change(async () => {
    await ShowCalendar();
    await GetPO();
});

//$("#selCustomerOrder").change(async () => {
//    await GetStoreCD();
//});

$(document).on("change", "#selCustomerOrder", async () => {
    await GetStoreCD();
});

$("#selStoreCode").change(async () => {
    await GetPartNo();
});

$("#selPartNo").change(async () => {
    await PartNoSelected();
    await SetCalendar();
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
                                <input type='number' class='form-control' min='0' max='9999' style='width: 85%;'
                                    id='QTY_${YM.replaceAll("-", "")}${day.toString().length == 1 ? "0" + day : day}'
                                    name='QTY_${YM.replaceAll("-", "")}${day.toString().length == 1 ? "0" + day : day}' />
                            </div>    
                        </td >`
            );
            day++;
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

    _xLib.AJAX_Get("/api/KBNIM007/GetPO", obj,
        async (success) => {
            console.log(success);
            $("#selCustomerOrder").empty();
            $("#selCustomerOrder").append("<option value='' hidden></option>");
            success.data.forEach((item) => {
                $("#selCustomerOrder").append(`<option value='${item}'>${item}</option>`);
            });
            $("#selCustomerOrder").selectpicker("refresh");
        }
    );
}

GetStoreCD = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007/GetStoreCD", obj,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#selStoreCode").addListSelectPicker(success.data, "F_Store_CD");
        }
    );
}

SetCalendar = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007/SetCalendar", obj,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#tableMain").find("input").prop("readonly", false);
            $("#tableMain").find("input").removeClass("bg-warning");
            $("#tableMain").find("input").removeClass("bg-danger");
            Object.keys(success.data[0]).forEach((key) => {
                //console.log(key);
                if (key.includes("F_workCd_D")) {
                    let date = key.split("F_workCd_D")[1];
                    //console.log(date);
                    if (date.length == 1) date = "0" + date;
                    if (success.data[0][key] == "1") {
                        //console.log(obj.F_Date);
                        if (success.data[0].F_Date <= obj.YM + date) {
                            $(`#QTY_${obj.YM}${date}`).prop("readonly", false);
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

GetPartNo = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007/GetPartNo", obj,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#selPartNo").addListSelectPicker(success.data, "F_Part_No");
        }
    );
}

PartNoSelected = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007/PartNoSelected", obj,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#spanPartName").text(success.data[0].F_Part_NM);
        }
    );
}


getQueryObj = async () => {
    let obj = {
        YM: $("#mthDeliYM").val().replaceAll("-", ""),
        PO: $("#selCustomerOrder").val(),
        StoreCD: $("#selStoreCode").val(),
        PartNo: $("#selPartNo").val(),
        isNew: !($("#btnNew").prop("disabled"))
    }

    return obj;
}
