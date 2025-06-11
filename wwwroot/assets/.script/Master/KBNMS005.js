$(document).ready(async function () {
    let addColumns = [];

    addColumns.push({ title: "TRIP", data: "F_TRIP" });

    for (let i = 1; i <= 24; i++) {
        addColumns.push({ title: i.toString(), data: "F_R" + i });
    }

    await _xDataTable.InitialDataTable("#tableMain",
        {
            columns: addColumns,
            order: [[1, "asc"]],
            scrollCollapse: false,
            scrollY: "150px",
        }
    );

    GetListOption();
    $("#inpDate").initDatepicker();
    $("#inpDateT").initDatepicker();

    xSplash.hide();

})

$("#inpSup").change(function () {
    GetListOption();
});

$("#inpKB").change(function () {
    $("#inpKBT").val($("#inpKB").val());
    GetListOption();
});

$("#inpPart").change(function () {
    $("#inpPartT").val($("#inpPart").val());
    GetListOption();
});

$("#inpStore").change(function () {
    $("#inpStoreT").val($("#inpStore").val());
    GetListOption();
});

$("#btnCancel").click(function () {
    $("#frmData").trigger("reset");
    $("#formReadData").trigger("reset");
    $("#inpDate").val(moment().format("DD/MM/YYYY"));
    $("#inpDateT").val(moment().format("DD/MM/YYYY"));
    $("#inpPage").val("1/1");
    GetListOption();

    $("#h6Date").text("Date ==> ");
    $("#h6UpdateStock").text("Update Stock Last : ");
    $("#h6Shift").text("Shift : ");

    $("#tableMain").DataTable().clear().draw();

});

$("#btnSearch").click(function () {
    GetAllData();
});

async function GetListOption() {
    let obj = {
        Sup : $("#inpSup").val(),
        Part: $("#inpPart").val(),
        PartT: $("#inpPartT").val(),
        Store: $("#inpStore").val(),
        StoreT: $("#inpStoreT").val(),
    }

    _xLib.AJAX_Get("/api/KBNMS005/GetListOption", obj,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success)

            if ($("#inpSup").val() === "") {
                $("#dlSup").empty();
                success.data[0].forEach(function (item) {
                    $("#dlSup").append(`<option value="${item}">${item}</option>`);
                });
            }
            if ($("#inpKB").val() === "") {
                $("#dlKB").empty();
                $("#dlKBT").empty();
                success.data[1].forEach(function (item) {
                    $("#dlKB").append(`<option value="${item}">${item}</option>`);
                    $("#dlKBT").append(`<option value="${item}">${item}</option>`);
                });
            }
            if ($("#inpStore").val() === "") {
                $("#dlStore").empty();
                $("#dlStoreT").empty();
                success.data[3].forEach(function (item) {
                    $("#dlStore").append(`<option value="${item}">${item}</option>`);
                    $("#dlStoreT").append(`<option value="${item}">${item}</option>`);
                });
            }
            if ($("#inpPart").val() === "") {
                $("#dlPart").empty();
                $("#dlPartT").empty();
                success.data[2].forEach(function (item) {
                    $("#dlPart").append(`<option value="${item}">${item}</option>`);
                    $("#dlPartT").append(`<option value="${item}">${item}</option>`);
                });
            }

        });

}
async function GetAllData() {
    xSplash.show("Loading...");
    var query = await $("#frmData").formToJSON();

    _xLib.AJAX_Get("/api/KBNMS005/GetAllData", query,
        async function (success) {
            success = _xLib.JSONparseMixData(success);
            const setData0 = [...new Set(success.data[0].map(item => item.F_No))];
            $("#inpPage").val("1/" + setData0.length);
            console.log(success);

            let firstNo = 0;
            let ProcessedData = [];
            if (success.data[0][0].F_No !== 1) {
                firstNo = success.data[0][0].F_No;

                for (let i = success.data[1].length - 1; i >= 0; i--) {
                    if (success.data[1][i].F_No < firstNo) {
                        success.data[1].splice(i, 1);
                    }
                }

                success.data[0].filter(item => item.F_No >= firstNo).forEach(function (item) {
                    //console.log(item);
                    item.F_No = item.F_No - firstNo + 1;
                });

                success.data[1].filter(item => item.F_No >= firstNo).forEach(function (item) {
                    //console.log(item);
                    item.F_No = item.F_No - firstNo + 1;
                });

            }

            console.log(success.data[0]);
            console.log(success.data[1]);

            await ShowData(success.data);
            xSplash.hide();
        },
        async function (error) {
            await xSwal.xErrorAwait(error);
        }
    );

}

let SearchData = [];

async function ShowData(data) {
    //console.log(SearchData);
    var Page = $("#inpPage").val().split("/")[0];
    //console.log(Page);
    SearchData = data;
    const ShowDataDetail = SearchData[0].filter(item => item.F_No === parseInt(Page));
    const ShowDataHead = SearchData[1].filter(item => item.F_No === parseInt(Page));
    //console.log(ShowDataDetail);
    //console.log(ShowDataHead);

    if (ShowDataDetail === 0 || ShowDataHead === 0) {
        return xSwal.Error("No Data Found");
    }

    $("#readPart").val(ShowDataHead[0].F_Part_No);
    $("#readStore").val(ShowDataHead[0].F_Store_Cd);
    $("#readPartName").val(ShowDataHead[0].F_Part_NM);
    $("#readSup").val(ShowDataHead[0].F_Short_Name);
    $("#readSup").val(ShowDataHead[0].F_Short_Name);
    $("#readKanban").val("0"+ShowDataHead[0].F_Sebango);
    $("#readCycle").val(ShowDataHead[0].F_Cycle);
    $("#readQty").val(ShowDataHead[0].F_Qty);
    $("#readForeMax").val(ShowDataHead[0].F_FC_Max);
    $("#readCurrent").val(ShowDataHead[0].F_CR_Use);
    $("#readAvg").val(ShowDataHead[0].F_AVG);
    $("#readStd").val(ShowDataHead[0].F_Std_Stock);
    $("#readSafe").val(ShowDataHead[0].F_Safety_Stock);
    $("#h6Date").text("Date ==> " + moment(ShowDataHead[0].F_Date, "YYYYMMDD").format("DD/MM/YYYY"));
    $("#h6UpdateStock").text("Update Stock Last : " + moment(ShowDataHead[0].F_UpdateDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));

    let Time = ShowDataHead[0].F_UpdateDate.split("T")[1];
    let Shift = (Time <= "19:30:00" && Time >= "07:30:00") ? "Day" : "Night";

    $("#h6Shift").text("Shift : " + Shift);

    if (ShowDataHead[0].F_Status === "OK") {
        $("#statusOK").addClass("bg-success text-white").removeClass("text-white");
        $("#statusNO").removeClass("bg-danger text-white");
    }
    else {
        $("#statusNO").addClass("bg-danger text-white").removeClass("text-white");
        $("#statusOK").removeClass("bg-success text-white");
    }

    _xDataTable.ClearAndAddDataDT("#tableMain", ShowDataDetail);

    addColorRow(ShowDataHead);
}

$("#btnNext").click(async function () {
    let Page = $("#inpPage").val().split("/")[0];
    let TotalPage = $("#inpPage").val().split("/")[1];

    if (parseInt(Page) < parseInt(TotalPage)) {
        Page = parseInt(Page) + 1;
        $("#inpPage").val(Page + "/" + TotalPage);
        await ShowData(SearchData);
    }
});

$("#btnPrev").click(async function () {
    let Page = $("#inpPage").val().split("/")[0];
    let TotalPage = $("#inpPage").val().split("/")[1];

    if (parseInt(Page) > 1) {
        Page = parseInt(Page) - 1;
        $("#inpPage").val(Page + "/" + TotalPage);
        await ShowData(SearchData);
    }
});

async function addColorRow(ShowDataHead) {
    if (ShowDataHead.length > 0) {
        if (ShowDataHead[0].F_BF_State.startsWith("Y")) {

            let cell = $("#tableMain").find("tbody tr:eq(0) td:eq(1)");
            //console.log(cell.text());
            cell.addClass("bg-warning text-white");

        }
    }
    $("#tableMain").find("tbody tr").each(function () {
        var firstTd = $(this).find("td:eq(0)");
        //console.log(firstTd.text());
        firstTd.css("background-color", "#89CFF0");
    });

    for (let i = 1; i <= 24; i++) {
        $("#tableMain").find("tbody tr:eq(3)").each(function () {
            //console.log("Row 4");
            //console.log($(this).find(`td:eq(${i})`).text());
            let Cell = $(this).find(`td:eq(${i})`);
            let intCell = parseInt($(this).find(`td:eq(${i})`).text());
            //console.log("intCell ", intCell);
            let intAvg = parseFloat(ShowDataHead[0].F_AVG);
            //console.log("intAvg ", (intAvg - (intAvg * 0.3)));
            //console.log("intCell ", (intAvg + (intAvg * 0.3)));
            if (intCell < (intAvg - (intAvg * 0.3))) {
                return Cell.addClass("text-danger");
            }
            else if (intCell > (intAvg + (intAvg * 0.3))) {
                return Cell.addClass("text-primary");
            }
            else {
                return Cell.removeClass("text-danger text-primary");
            }

        });

        $("#tableMain").find("tbody tr:eq(5)").each(function () {
            //console.log("Row 6");
            //console.log($(this).find(`td:eq(${i})`).text());
            let Cell = $(this).find(`td:eq(${i})`);
            let intCell = parseInt($(this).find(`td:eq(${i})`).text());
            //console.log("intCell ", intCell);
            let intSafe = parseFloat(ShowDataHead[0].F_Safety_Stock);
            let intFCMax = parseFloat(ShowDataHead[0].F_FC_Max);

            if (intCell < (intFCMax * intSafe) * 0.6) {
                return Cell.addClass("text-danger");
            }
            else if (intCell > (intFCMax * intSafe) * 1.4) {
                return Cell.addClass("text-primary");
            }
            else {
                return Cell.removeClass("text-danger text-primary");
            }

        });
    }

}