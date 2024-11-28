$(document).ready(async function () {

    await _xDataTable.InitialDataTable("#tableSub",
        {
            columns: [
                { title: "Order No.", data: "F_PDS_No" },
                { title: "Parent Part", data: "F_Part_No" },
                { title: "Store Cd.", data: "F_Store_CD" },
                { title: "Supplier", data: "F_SUpplier" },
                { title: "Deli Date", data: "F_Delivery_Date" },
                { title: "Trip", data: "F_ROund" },
                { title: "Qty", data: "F_Qty" },
                { title: "Child Part", data: "F_Child_Part" },
                { title: "ST. Child", data: "F_Store_Child" },
                { title: "Child Name", data: "f_Child_Name" },
                { title: "Order Type", data: "F_OrderType" },
            ],
            scrollX : false,
            order: [[1, "asc"]],
            scrollCollapse: true,
            scrollY: 500,
        }
    );

    $("#inpDate").initDatepicker();

    await GetPO();

    xSplash.hide();

    await ShowCalendar();
});

var _command = "";

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

//$("#selCustomerOrder").change(async () => {
//    await GetStoreCD();
//});

$("#fileIMP").change(async (e) => {
    //console.log(e);
    let inputFile = e.target.files;
    if (inputFile.length == 0) return;
    //console.log(inputFile);
    let file = inputFile[0];
    if (await xSwal.confirm("Are you sure?", "This action can't be undone")) await Import(file);

});

$("#fileSCP").change(async (e) => {
    let inputFile = e.target.files;
    if (inputFile.length == 0) return;

    let file = inputFile[0];
    let result = await xSwal.text("Importing File", "Please input advanced date.");
    if (result.isConfirmed) {
        await ImportSCP(file, result.value);
    }
});

$("#fileIPMS").change(async (e) => {
    let inputFile = e.target.files;
    if (inputFile.length == 0) return;

    let file = inputFile[0];
    let result = await xSwal.text("Importing File", "Please input advanced date.");
    if (result.isConfirmed) {
        await ImportIPMS(file, result.value);
    }
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
    await GetStoreCD();
    await ListDataTable();
});

$("#selStoreCode").change(async () => {
    await GetPartNo();
});

$("#selPartNo").change(async () => {
    await PartNoSelected();
    await ListCalendar();
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

$("#btnSave").click(async () => {
    let isConfirm = await xSwal.confirm("Are you sure?", "This action can't be undone");
    if(!isConfirm) return;
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

GetStoreCD = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007/GetStoreCD", obj,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#selStoreCode").addListSelectPicker(success.data, "F_Store_CD");
        }
    );
}

ListCalendar = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007/ListCalendar", obj,
        async (success) => {
            //success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#tableMain").find("input").val("");
            $(success.data).each(function () {
                let date = moment(this.f_Delivery_Date, "DD/MM/YYYY").format("YYYYMMDD");
                $(`#QTY_${date}`).val(this.f_Qty);
            });
        }
    );
}

SetCalendar = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007/SetCalendar", obj,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
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
            //console.log(success);
            $("#selPartNo").addListSelectPicker(success.data, "F_Part_No");
        }
    );
}

PartNoSelected = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007/PartNoSelected", obj,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#spanPartName").text("");
            if (success.data.length > 0) {
                $("#spanPartName").text(success.data[0].F_Part_NM);
            }
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

getSaveObj = async () => {
    let list = [];
    $("#tableMain").find("input").each(function () {

        let obj = {
            PDS: $("#selCustomerOrder").val(),
            IssuedDate: moment($("#inpDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
            PartNo: $("#selPartNo").val(),
            StoreCD: $("#selStoreCode").val(),
            DeliveryDate: $("#mthDeliYM").val().replaceAll("-", ""),
            Qty: 0,
            Trip: 1,
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

    _xLib.AJAX_Post("/api/KBNIM007/Save?action=" + _command, listObj,
        async (success) => {
            //console.log(success);
            xSwal.success(success.response, success.message);
            $("#selCustomerOrder").trigger("change");
            $("#selPartNo").trigger("change");
        }
    );
}

Import = async (inputFile) => {
    let obj = new FormData();
    let file = new Blob([inputFile], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
    obj.append("file", file, inputFile.name);


    _xLib.AJAX_PostFile("/api/KBNIM007/Import", obj,
        async (success) => {
            //console.log(success);
            xSwal.success(success.response, success.message);
        }
    );
}

ImportIPMS = async (inputFile,advanceDate) => {
    let formData = new FormData();
    //console.log(inputFile);
    let file = new Blob([inputFile], { type: "text/csv" });
    formData.append("file", file, inputFile.name);

    _xLib.AJAX_PostFile("/api/KBNIM007/ImportIPMS?BackDate=" + advanceDate, formData,
        async (success) => {
            //console.log(success);
            xSwal.success(success.response, success.message);
        }
    );
}

ImportSCP = async (inputFile,advanceDate) => {
    let formData = new FormData();
    console.log(inputFile);
    let file = new Blob([inputFile], { type: "text/txt" });
    formData.append("file", file, inputFile.name);

    _xLib.AJAX_PostFile("/api/KBNIM007/ImportSCP?BackDate=" + advanceDate, formData,
        async (success) => {
            //console.log(success);
            xSwal.success(success.response, success.message);
        }
    );
}

ListDataTable = async () => {
    let obj = await getQueryObj();

    _xLib.AJAX_Get("/api/KBNIM007/ListDataTable", obj,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            _xDataTable.ClearAndAddDataDT("#tableSub", success.data);
            //$("#tableSub").DataTable().clear().draw();
            //$("#tableSub").DataTable().rows.add(success.data).draw();
        }
    );
}