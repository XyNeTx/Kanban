$(document).ready(async function () {

    await InitDatatables();

    $("#tableMain").DataTable().destroy();
    $("#tableMain").empty();

    $("table thead tr th").addClass("text-center");
    $("table tbody tr td").addClass("text-center");

    //await GetDropDown();

    $("#F_Supplier_Code").disableSelectPicker();
    $("#F_Kanban_No").disableSelectPicker();
    $("#F_Store_Cd").disableSelectPicker();
    $("#F_Part_No").disableSelectPicker();
    $("#F_Supplier_Code2").disableSelectPicker();
    $("#F_Store_Cd2").disableSelectPicker();
    $("#btnExit2").prop("disabled", true);
    $("#btnSearch2").prop("disabled", true);
    $("#btnSearch").prop("disabled", true);
    $("#btnExit").prop("disabled", true);

    xSplash.hide();
})

async function InitDatatables() {

    var optionDataTable = {
        processing: false,
        serverSide: false,
        width: '100%',
        paging: false,
        sorting: false,
        searching: false,
        scrollX: true,
        scrollY: "200px",
        scrollCollapse: false,
        select: false,
        columns:
            [
                {
                    title: "SUP CD", data: "f_Supplier_Code", render: function (data, type, row) {
                        return row.f_Supplier_Code + "-" + row.f_Supplier_Plant;
                    }
                },
                {
                    title: "ST", data: "f_Store_Cd"
                },
                {
                    title: "Part No.", data: "f_Part_No", render: function (data, type, row) {
                        return row.f_Part_No + "-" + row.f_Ruibetsu;
                    }
                },
                {
                    title: "Kanban", data: "f_Kanban_No"
                },
            ],
        order: [[0, "asc"]]
    }

    let cycle = 0;

    if ($("#F_Cycle").val() != "") {
        cycle = parseInt($("#F_Cycle").val().slice(3, 5));

    }
    else if ($("#F_Cycle2").val() != "") {
        cycle = parseInt($("#F_Cycle2").val().slice(3, 5));
    }

    for (let i = 1; i <= cycle; i++) {
        //console.log(i);
        let _i = i.toString().length === 1 ? "0" + i : i;
        optionDataTable.columns.push({
            title: _i,
            data: `f_Trip` + i,
            render: function (data, type, row) {
                let checked = data == "0" ? "" : "checked"
                return `<input type="checkbox" class="chkbox" id="f_Trip${i}" name="f_Trip${i}" value='${data}' ${checked}>`;
            }
        })
    }

    if ($("#tableMain.dataTable").length > 0) {
        //console.log(78);
        $("#tableMain").DataTable().destroy();
        $("#tableMain").empty();
        //console.log(80);
    }

    _xDataTable.InitialDataTable("#tableMain", optionDataTable);
}

$("input[name='radioBtn']").change(async function () {
    await GetDropDown();

    $("#F_Supplier_Code").resetSelectPicker();
    $("#F_Kanban_No").resetSelectPicker();
    $("#F_Store_Cd").resetSelectPicker();
    $("#F_Part_No").resetSelectPicker();
    $("#F_Supplier_Code2").resetSelectPicker();
    $("#F_Store_Cd2").resetSelectPicker();
    $("#formMain").trigger("reset");

    $(this).prop("checked", true);

    if ($(this).attr("id") === "rdoMain" && $(this).prop("checked")) {
        $("#F_Supplier_Code").enableSelectPicker();
        $("#F_Kanban_No").enableSelectPicker();
        $("#F_Store_Cd").enableSelectPicker();
        $("#F_Part_No").enableSelectPicker();
        $("#F_Supplier_Code2").disableSelectPicker();
        $("#F_Store_Cd2").disableSelectPicker();
        $("#btnExit2").prop("disabled", true);
        $("#btnSearch2").prop("disabled", true);
        $("#btnSearch").prop("disabled", false);
        $("#btnExit").prop("disabled", false);
    }
    else {
        $("#F_Supplier_Code").disableSelectPicker();
        $("#F_Kanban_No").disableSelectPicker();
        $("#F_Store_Cd").disableSelectPicker();
        $("#F_Part_No").disableSelectPicker();
        $("#F_Supplier_Code2").enableSelectPicker();
        $("#F_Store_Cd2").enableSelectPicker();
        $("#btnExit2").prop("disabled", false);
        $("#btnSearch2").prop("disabled", false);
        $("#btnSearch").prop("disabled", true);
        $("#btnExit").prop("disabled", true);
    }


})

$("#F_Supplier_Code").change(async function () {
    $("#F_Supplier_Code_Rd").val($(this).val())
    await FindDetail();
    await InitDatatables();
    await GetDropDown();
})
$("#F_Kanban_No").change(async function () {
    $("#F_Kanban_No_Rd").val($(this).val())
    await FindDetail();
    await GetDropDown();
})
$("#F_Store_Cd").change(async function () {
    $("#F_Store_Cd_Rd").val($(this).val())
    await GetDropDown();
})
$("#F_Part_No").change(async function () {
    $("#F_Part_No_Rd").val($(this).val())
    await GetDropDown();
})
$("#F_Supplier_Code2").change(async function () {
    $("#F_Supplier_Code_Rd2").val($(this).val())
    await FindDetail();
    await InitDatatables();
    await GetDropDown();
})
$("#F_Store_Cd2").change(async function () {
    $("#F_Store_Cd_Rd2").val($(this).val())
    await GetDropDown();
})

$("#btnSave").click(async function () {
    await Save();
})

async function GetObj() {
    let obj = await $("#formMain").formToJSON();

    if ($("#rdoMain2").prop("checked")) {
        obj.F_Supplier_Code = obj.F_Supplier_Code2;
        obj.F_Store_Cd = obj.F_Store_Cd2;
    }
    //console.log(obj);
    return obj;
}
async function PostObj() {
    let obj = await $("#formMain").formToJSON();

    obj.F_Ruibetsu = obj.F_Part_No.split("-")[1];
    obj.F_Part_No = obj.F_Part_No.split("-")[0];
    obj.F_Supplier_Code = obj.F_Supplier_Code.split("-")[1];
    obj.F_Supplier_Plant = obj.F_Supplier_Code.split("-")[0];

    return obj
}

async function GetDropDown() {
    let obj = await GetObj();
    //console.log(obj);


    await _xLib.AJAX_Get("/api/KBNMS012/GetDropDown", obj,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            if ($("#F_Supplier_Code").val() === "") $("#F_Supplier_Code").addListSelectPicker(success.data.supcode, "f_Supplier_Code")
            if ($("#F_Supplier_Code2").val() === "") $("#F_Supplier_Code2").addListSelectPicker(success.data.supcode, "f_Supplier_Code")
            if ($("#F_Kanban_No").val() === "") $("#F_Kanban_No").addListSelectPicker(success.data.kanban, "f_Kanban_No")
            if ($("#F_Store_Cd").val() === "") $("#F_Store_Cd").addListSelectPicker(success.data.store, "f_Store_Cd")
            if ($("#F_Store_Cd2").val() === "") $("#F_Store_Cd2").addListSelectPicker(success.data.store, "f_Store_Cd")
            if ($("#F_Part_No").val() === "") $("#F_Part_No").addListSelectPicker(success.data.partno, "f_Part_No")
            //return console.log("GetDropDown");
        },
        function (error) {
            if ($("#F_Supplier_Code").val() === "") $("#F_Supplier_Code").removeListSelectPicker()
            if ($("#F_Supplier_Code2").val() === "") $("#F_Supplier_Code2").removeListSelectPicker()
            if ($("#F_Kanban_No").val() === "") $("#F_Kanban_No").removeListSelectPicker()
            if ($("#F_Store_Cd").val() === "") $("#F_Store_Cd").removeListSelectPicker()
            if ($("#F_Store_Cd2").val() === "") $("#F_Store_Cd2").removeListSelectPicker()
            if ($("#F_Part_No").val() === "") $("#F_Part_No").removeListSelectPicker()
        }
    );

}

async function GetSupplierDetail() {
    let obj = await GetObj();

    _xLib.AJAX_Get("/api/KBNMS012/GetSupplierDetail", obj,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            console.log(success);

        }
    );

}

async function Search() {
    let obj = await GetObj();

    _xLib.AJAX_Get("/api/KBNMS012/Search", obj,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            console.log(success);

            if (success.data.length === 0) {
                if (($("#F_Supplier_Code_Rd").val() === "" && $("#F_Kanban_No_Rd").val() === "")
                    || ($("#F_Part_No_Rd").val() === "" && $("#F_Kanban_No_Rd").val() === ""))
                {
                    return xSwal.error("Error !!!", "Please Select Supplier Code and Kanban No or Store Code and Part No");
                }
                let obj = 
                {
                    f_Plant: ajexHeader.Plant,
                    f_Supplier_Code: $("#F_Supplier_Code_Rd").val().split("-")[0],
                    f_Supplier_Plant: $("#F_Supplier_Code_Rd").val().split("-")[1],
                    f_Part_No: $("#F_Part_No_Rd").val().split("-")[0],
                    f_Ruibetsu: $("#F_Part_No_Rd").val().split("-")[1],
                    f_Kanban_No: $("#F_Kanban_No_Rd").val(),
                    f_Store_Cd: $("#F_Store_Cd_Rd").val(),
                    f_Cycle: $("#F_Cycle").val().replaceAll("-",""),
                    f_Trip1: "0",
                    f_Trip2: "0",
                    f_Trip3: "0",
                    f_Trip4: "0",
                    f_Trip5: "0",
                    f_Trip6: "0",
                    f_Trip7: "0",
                    f_Trip8: "0",
                    f_Trip9: "0",
                    f_Trip10: "0",
                    f_Trip11: "0",
                    f_Trip12: "0",
                    f_Trip13: "0",
                    f_Trip14: "0",
                    f_Trip15: "0",
                    f_Trip16: "0",
                    f_Trip17: "0",
                    f_Trip18: "0",
                    f_Trip19: "0",
                    f_Trip20: "0",
                    f_Trip21: "0",
                    f_Trip22: "0",
                    f_Trip23: "0",
                    f_Trip24: "0",
                    f_Create_By: ajexHeader.UserCode,
                    f_Create_Date: "2015-06-25T08:31:00",
                    f_Update_By: ajexHeader.UserCode,
                    f_Update_Date: "2015-06-25T08:31:00"
                }
                success.data.push(obj);
            }

            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        }
    );

}

async function Search2() {
    let obj = await GetObj();


    _xLib.AJAX_Get("/api/KBNMS012/Search", obj,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            console.log(success);
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        }
    );

}

async function FindDetail() {
    let obj = await GetObj();

    await _xLib.AJAX_Get("/api/KBNMS012/FindDetail", obj,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);

            if ($("#rdoMain").prop("checked")) {

                $("#F_Supplier_Name").val(success.data.dt[0].F_name);
                let cycle = success.data.cycle[0].F_Cycle;
                let formatedCycle = cycle.slice(0, 2) + "-" + cycle.slice(2, 4) + "-" + cycle.slice(4, 6);
                $("#F_Cycle").val(formatedCycle);

                if ($("#F_Kanban_No").val() != "") {
                    $("#F_Store_Cd_Rd").val(success.data.dt[0].F_Store_cd);
                    $("#F_Part_No_Rd").val(success.data.dt[0].F_Part_no);
                    $("#F_Part_Name").val(success.data.dt[0].F_Part_nm);
                    $("#F_Qty").val(success.data.dt[0].F_qty_box);
                }

            }
            else {
                $("#F_Supplier_Name2").val(success.data.dt[0].F_name);
                let cycle = success.data.cycle[0].F_Cycle;
                let formatedCycle = cycle.slice(0, 2) + "-" + cycle.slice(2, 4) + "-" + cycle.slice(4, 6);
                $("#F_Cycle2").val(formatedCycle);
            }
        }
    );

}

$("#btnSearch").click(async function () {
    await Search();
    $("#btnEdit").prop("disabled", false);
    $("#btnReport").prop("disabled", false);
})

$("#btnSearch2").click(async function () {
    await Search2();
    $("#btnEdit").prop("disabled", false);
    $("#btnReport").prop("disabled", false);
})

$("#btnEdit").click(async function () {
    $("#btnCancel").prop("disabled", false);
    $("#btnSave").prop("disabled", false);
    $("#btnEdit").prop("disabled", true);
    $("#btnReport").prop("disabled", true);
})
$("#btnCancel").click(async function () {
    $("#btnCancel").prop("disabled", true);
    $("#btnSave").prop("disabled", true);
    $("#btnEdit").prop("disabled", false);
    $("#btnReport").prop("disabled", false);
})

$("#btnReport").click(async function () {
    let obj = {
        Plant: ajexHeader.Plant,
        UserName: _xLib.GetUserName(),
        F_Supplier_Code: $("#F_Supplier_Code_Rd").val().split("-")[0],
        F_Supplier_Plant: $("#F_Supplier_Code_Rd").val().split("-")[1],
        F_Part_No: $("#F_Part_No_Rd").val().split("-")[0],
        F_Ruibetsu: $("#F_Part_No_Rd").val().split("-")[1],
        F_Kanban_No: $("#F_Kanban_No_Rd").val(),
        F_Store_Cd: $("#F_Store_Cd_Rd").val(),
    }
    console.log(obj);
    _xLib.OpenReportObj("/KBNMS012", obj);
})

$(document).on("click", "#tableMain tbody tr td input[name*='f_Trip']", function (e) {
    if ($("#btnEdit").prop("disabled")) {
    }
    else {
        e.preventDefault();
    }
})

async function Save() {
    let listObj = $("#tableMain").DataTable().rows().data().toArray();

    //let obj = listObj[0];
    for (let i = 0; i < listObj.length; i++) {
        $("#tableMain tbody tr td input[name*='f_Trip']").each(function (index, html) {
            listObj[i][`${$(this).attr("name")}`] = $(this).prop("checked") === true ? "1" : "0";
            //console.log($(this));
            //console.log(index);
            //console.log(val);
        })
    }
    

    console.log(listObj);

    _xLib.AJAX_Post("/api/KBNMS012/Save", obj,
        function (success) {
            xSwal.xSuccess(success);
            console.log(success);
        },
        function (error) {
            xSwal.xError(error);
        }
    );

}
