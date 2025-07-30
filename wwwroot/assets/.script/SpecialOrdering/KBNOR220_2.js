$(document).ready(async () => {
    await ShowCalendar();
    await GetSurvey();
    xSplash.hide();
});

$("#btnInq").click(() => {
    _command = "inquiry";
    $("#mthDeliYM").trigger("change");

    $("#btnUpd").prop("disabled", true);

    $("#mthDeliYM").prop("disabled", false);
    $("#selSurDoc").prop("disabled", false);
    $("#selSupCode").prop("disabled", false);
    $("#selPartNo").prop("disabled", false);

    $("#selSurDoc").selectpicker("refresh");
    $("#selSupCode").selectpicker("refresh");
    $("#selPartNo").selectpicker("refresh");
});

$("#btnUpd").click(() => {
    _command = "update";
    $("#btnInq").prop("disabled", true);
    $("#btnSave").prop("disabled", false);
    $("#mthDeliYM").trigger("change");

    $("#mthDeliYM").prop("disabled", false);
    $("#selSurDoc").prop("disabled", false);
    $("#selSupCode").prop("disabled", false);
    $("#selPartNo").prop("disabled", false);

    $("#selSurDoc").selectpicker("refresh");
    $("#selSupCode").selectpicker("refresh");
    $("#selPartNo").selectpicker("refresh");
});

$("#mthDeliYM").change(() => {
    $("#tableMain").find("tbody").remove();
    ShowCalendar();

    $("#selSurDoc").resetSelectPicker();
    $("#selSupCode").resetSelectPicker();
    $("#selPartNo").resetSelectPicker();
});

$("#selSurDoc").change(() => {
    GetSuppCD();
});

$("#selSupCode").change(() => {
    GetPartNo();
});

$("#selPartNo").change(async () => {
    $("#tableMain").find("tbody").remove();
    await ShowCalendar();
    await GetPOQty();

    GetCalendarQty();
});

$("#modalSelCustOrd").change(() => {
    let getQuery = {
        YM: "",
        Survey: $("#modalSelCustOrd").val()
    }
    _xLib.AJAX_Get("/api/KBNOR220_2/GetSuppCD", getQuery,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#modalSelSuppCd").empty();
            $("#modalSelSuppCd").append("<option value='' hidden></option>");
            success.data.forEach(function (item) {
                $("#modalSelSuppCd").append("<option value='" + item.F_Supplier_Code + "'>" + item.F_Supplier_Code + "</option>");
            });
            $("#modalSelSuppCd").selectpicker("refresh");

        },
        async (error) => {
            console.log(error);
        }
    );
});

$("#modalSelSuppCd").change(() => {
    let getQuery = {
        SuppCD: $("#modalSelSuppCd").val().split("-")[0] ?? "",
        SuppPlant: $("#modalSelSuppCd").val().split("-")[1] ?? ""
    }

    _xLib.AJAX_Get("/api/KBNOR220_2/GetSupplierName", getQuery,
        async (success) => {
            //console.log(success);
            $("#modalTxtSupName").val(success.data);
        },
        async (error) => {
            console.log(error);
        }
    );

});



$("#btnCan").click(() => {
    $("#btnInq").prop("disabled", false);
    $("#btnUpd").prop("disabled", false);

    $("#mthDeliYM").val(moment().format("YYYY-MM"));
    $("#selSurDoc").val("");
    $("#selSupCode").val("");
    $("#selPartNo").val("");
    $("#txtPOQty").val("");
    $("#tableMain").find("input").val("");
    $("#tableMain").find("span[id*='span']").text("");
    $("#mthDeliYM").focus();

    $("#mthDeliYM").prop("disabled", true);
    $("#selSurDoc").prop("disabled", true);
    $("#selSupCode").prop("disabled", true);
    $("#selPartNo").prop("disabled", true);

    $("#mthDeliYM").trigger("change");
    $("#selSurDoc").selectpicker("refresh");
    $("#selSupCode").selectpicker("refresh");
    $("#selPartNo").selectpicker("refresh");

    $("#selSurDoc").parent().find("div[class='filter-option-inner-inner']").text("Nothing Selected");
    $("#selSupCode").parent().find("div[class='filter-option-inner-inner']").text("Nothing Selected");
    $("#selPartNo").parent().find("div[class='filter-option-inner-inner']").text("Nothing Selected");

});

$("#btnSave").click(() => {
    let listObj = [];

    //$("#tableMain").find("input:not([readonly])").each(function () {
    $("#tableMain").find("input").each(function () {
        let obj = {
            POQty: $("#txtPOQty").val(),
            Survey: $("#selSurDoc").val(),
            SuppCD: $("#selSupCode").val(),
            PartNo: $("#selPartNo").val(),
            Delivery_Date: $(this).attr("id").split("_")[1],
            Qty: parseInt($(this).val()) ?? 0
        }
        listObj.push(obj);
    });

    _xLib.AJAX_Post("/api/KBNOR220_2/Save", listObj,
        async (success) => {
            console.log(success);
            xSwal.success(success.response, success.message);
            $("#selPartNo").trigger("change");
        },
        async (error) => {
            xSwal.xError(error);
            console.log(error);
        }
    );

});

$("#btnPrintRpt").click(() => {
    let obj = {
        F_PO_Customer: $("#modalSelCustOrd").val(),
        F_Supplier_CD: $("#modalSelSuppCd").val().split("-")[0] ?? "",
        F_Supplier_Plant: $("#modalSelSuppCd").val().split("-")[1] ?? "",
    }

    _xLib.OpenReportObj("/KBNOR220_2_Rpt", obj);
});

$(document).on("shown.bs.modal", "#KBNOR220_2_Rpt", async () => {
    await GetPOList();

    $("#modalSelCustOrd").resetSelectPicker();
    $("#modalSelSuppCd").resetSelectPicker();
    $("#modalTxtSupName").val("");
});

let _command = "inquiry";

ShowCalendar = async () => {

    let YM = $("#mthDeliYM").val();

    let Table = $("#tableMain");
    Table.append("<tbody></tbody>");

    _xLib.AJAX_Get("/api/KBNOR220_2/GetCalendar", { YM: YM.replaceAll("-", "") },
        async (success) => {
            //console.log(success);
            let day = 1;
            const startDate = new Date(YM + "-01");
            const endDate = new Date(startDate.getFullYear(), startDate.getMonth() + 1, 0);
            //endDate.setMonth(endDate.getMonth() + 1);
            //endDate.setDate(endDate.getDate() - 1);
            //console.log(startDate);
            //console.log(endDate);

            for (let mockDay = 1; mockDay <= 42; mockDay++) {
                let readonly = "readonly";

                if (mockDay % 7 == 1) {
                    Table.find("tbody").append("<tr>");
                }

                success.data["f_workCd_D" + day] == "1" ? readonly = "" : readonly = "readonly";

                if (_command == "inquiry") {
                    readonly = "readonly";
                }

                //console.log("mockDay", mockDay);
                //console.log("startDate.getDay()", startDate.getDay());
                //console.log("endDate.getDate() + startDate.getDay()", endDate.getDate() + startDate.getDay());

                if (mockDay > startDate.getDay() && mockDay <= endDate.getDate() + startDate.getDay()) {

                    Table.find("tbody").find("tr:last").append(
                        `<td class='text-center'>
                        <span class='fs-6'>${day}</span></br>
                        <p class='bg-warning text-dark fw-bolder m-0 p-1' id='span${YM.replaceAll("-", "")}${day.toString().length == 1 ? "0" + day : day}'>
                            <span style='visibility:hidden;' >for bg color</span>
                        </p>
                        <div class='row justify-content-center mt-1'>
                            <input type='number' class='form-control w-75' min='0' max='9999'
                            id='QTY_${YM.replaceAll("-", "")}${day.toString().length == 1 ? "0" + day : day}' 
                            name='QTY_${YM.replaceAll("-", "")}${day.toString().length == 1 ? "0" + day : day}' ${readonly} />
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
                if($(this).text() == ""){
                    $(this).remove();
                }
            });
        },
        function (error) {
            console.log(error);
        }
    );
}

GetSurvey = async () => {

    _xLib.AJAX_Get("/api/KBNOR220_2/GetSurvey", { YM : $("#mthDeliYM").val().replaceAll("-", "") },
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#selSurDoc").empty();
            $("#selSurDoc").append("<option value='' hidden></option>");
            success.data.forEach(function (item) {
                $("#selSurDoc").append("<option value='" + item.F_Survey_Doc + "'>" + item.F_Survey_Doc + "</option>");
            });
            $("#selSurDoc").selectpicker("refresh");
        },
        async (error) => {
            console.log(error);
        }
    );
}

GetSuppCD = async () => {
    let getQuery = {
        YM: $("#mthDeliYM").val().replaceAll("-", ""),
        Survey : $("#selSurDoc").val()
    }
    _xLib.AJAX_Get("/api/KBNOR220_2/GetSuppCD", getQuery,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#selSupCode").empty();
            $("#selSupCode").append("<option value='' hidden></option>");
            success.data.forEach(function (item) {
                $("#selSupCode").append("<option value='" + item.F_Supplier_Code + "'>" + item.F_Supplier_Code + "</option>");
            });
            $("#selSupCode").selectpicker("refresh");

        },
        async (error) => {
            console.log(error);
        }
    );
}

GetPartNo = async () => {
    let getQuery = {
        Survey: $("#selSurDoc").val(),
        SuppCD : $("#selSupCode").val()
    }
    _xLib.AJAX_Get("/api/KBNOR220_2/GetPartNo", getQuery,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#selPartNo").empty();
            $("#selPartNo").append("<option value='' hidden></option>");
            success.data.forEach(function (item) {
                $("#selPartNo").append("<option value='" + item.f_Part_No + "'>" + item.f_Part_No + "</option>");
            });
            $("#selPartNo").selectpicker("refresh");

        },
        async (error) => {
            console.log(error);
        }
    );
}

GetPOQty = async () => {
    let getQuery = {
        Survey: $("#selSurDoc").val(),
        SuppCD: $("#selSupCode").val(),
        PartNo: $("#selPartNo").val()
    }
    _xLib.AJAX_Get("/api/KBNOR220_2/GetPOQty", getQuery,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#txtPOQty").val(success.data);
        },
        async (error) => {
            console.log(error);
        }
    );
}

GetCalendarQty = async () => {
    let YM = $("#mthDeliYM").val().replaceAll("-", "");
    let getQuery = {
        YM: YM,
        Survey: $("#selSurDoc").val(),
        SuppCD: $("#selSupCode").val(),
        PartNo: $("#selPartNo").val()
    }
    _xLib.AJAX_Get("/api/KBNOR220_2/GetCalendarQty", getQuery,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);

            $("#tableMain").find("input").val("");
            $("#tableMain").find("span[id*='span']").text("");
            success.data.forEach(function (item) {
                //$("#QTY_" + YM + item.f_Day).val(item.f_Qty);
                $("#span" + item.d.F_Delivery_Date).text(item.d.F_Qty);
            });
            //for (let i = 1; i <= 31; i++) {
            //    $("#QTY_" + YM + (i.toString().length == 1 ? "0" + i : i)).val(success.data["f_Qty_D" + i]);
            //}
        },
        async (error) => {
            console.log(error);
        }
    );
}

GetPOList = async () => {
    _xLib.AJAX_Get("/api/KBNOR220_2/GetPOList", {},
        async (success) => {
            console.log(success);
            $("#modalSelCustOrd").empty();
            $("#modalSelCustOrd").append("<option value='' hidden></option>");
            success.data.forEach(function (item) {
                $("#modalSelCustOrd").append("<option value='" + item.f_PO_Customer + "'>" + item.f_PO_Customer + "</option>");
            });
            $("#modalSelCustOrd").selectpicker("refresh");
        },
        async (error) => {
            console.log(error);
        }
    );
}