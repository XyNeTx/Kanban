$(document).ready(async () => {
    await ShowCalendar();
    await GetSurvey();
    xSplash.hide();
});

$("#mthDeliYM").change(() => {
    $("#tableMain").find("tbody").remove();
    ShowCalendar();
});

$("#selSurDoc").change(() => {
    GetSuppCD();
});

$("#selSupCode").change(() => {
    GetPartNo();
});

$("#selPartNo").change(() => {
    GetPOQty();
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
                        <span>${day}</span></br>
                        <span class='bg-warning' id=span${YM.replaceAll("-", "")}${day.toString().length == 1 ? "0" + day : day}></span></br>
                        <div class='row justify-content-center'>
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

//GetPOList = async () => {
//    _xLib.AJAX_Get("/api/KBNOR220_2/GetPOList", {},
//        async (success) => {
//            console.log(success);
//            $("#selSurDoc").empty();
//            $("#selSurDoc").append("<option value='' hidden></option>");
//            success.data.forEach(function (item) {
//                $("#selSurDoc").append("<option value='" + item.f_PO_Customer + "'>" + item.f_PO_Customer + "</option>");
//            });
//            $("#selSurDoc").selectpicker("refresh");
//        },
//        async (error) => {
//            console.log(error);
//        }
//    );
//}

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