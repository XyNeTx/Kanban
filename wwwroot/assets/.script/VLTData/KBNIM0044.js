let isSimulate = false;
var listObjChecked = [];
var files = [];
$(document).ready(async function () {
    _xDataTable.InitialDataTable("#tableMain",
    {
        width: '100%',
        scrollY: "245px",
        "columns": [
            {
                title: "Flag", render(data, type, row) {
                    var checked = row.f_Flag === "S" ? "checked" : "";
                    var readonly = isSimulate ? "" : "";
                    return `<input type="checkbox" class="chkbox" id="chkbox" name="chkbox" value="${row.f_Supplier_Plant}" ${checked} ${readonly}>`;
                }
            },
            {
                title: "Prod.Date", data: "f_Date", render(data, type, row) {
                    return moment(data,"YYYYMMDD").format("DD/MM/YYYY");
                }
            },
            {
                title: "Line Name", data: "f_Customer"
            },
            {
                title: "F/P Code", data: "f_PartCode"
            },
            {
                title: "Part No.", data: "f_Parent_Part"
            },
            {
                title: "ID Line", data: "f_Line_ID"
            },
            {
                title: "Jig Seq.", data: "f_Seq"
            },
            {
                title: "Delivery Date", data: "f_Deli_Date", render(data, type, row) {
                    return data === null ? "" : moment(data, "YYYYMMDD").format("DD/MM/YYYY");
                }
            },
            {
                title: "Delivery Shift", data: "f_Deli_Shift", render(data, type, row) {
                    return data === "D" ? "Day" : data === "N" ? "Night" : "";
                }
            },
            {
                title: "Delivery Trip", data: "f_Deli_Trip"
            },
        ],
        order: [[0, "asc"]]
    });

    $("table tbody tr td").addClass("text-center");
    $("table thead tr th").addClass("text-center");
    $(".dataTables_scrollHeadInner").addClass("pe-0")
    $("#inpDeliveryDate").initDatepicker();

    await GetListData();

});

$("#inpFile").on("change", function (e) {
    files = e.target.files;
});
//let SeqQtyPds = 0;
//let TripShift = 0;
//let SeqQty = 0;
//$("#inpSeqPds").change(function () {
//    SeqQtyPds = parseInt($("#inpSeqPds").val());
//    TripShift = parseInt($("#inpTripShift").val());

//    SeqQty = SeqQtyPds * TripShift;

//    $("#inpSeqQty").val(SeqQty);
//});
//$("#inpTripShift").change(function () {
//    SeqQtyPds = parseInt($("#inpSeqPds").val());
//    TripShift = parseInt($("#inpTripShift").val());

//    SeqQty = SeqQtyPds * TripShift;

//    $("#inpSeqQty").val(SeqQty);
//});

$("#btnImp").on("click", async function () {
    if (files.length == 0) {
        xSwal.error("Error", "Please select file to import.");
        return;
    }
    const file = files[0];
    try {
        const arrayBuffer = await file.arrayBuffer();
        const read = await XLSX.read(arrayBuffer);

        var maxRow = read.Sheets.Sheet1["!ref"].split(":")[1].replace(/[A-z]/g, "");
        //console.log(maxRow);

        let newRead = read;

        var checkRow = parseInt(newRead.Sheets.Sheet1["A" + maxRow].v.replace(/#/g, "").replace(/T/g, ""));
        //console.log(checkRow);

        delete newRead.Sheets.Sheet1["A1"];
        delete newRead.Sheets.Sheet1["A" + maxRow];

        newRead.Sheets.Sheet1.A1 = { t: "s", v: "F_Date", w: "F_Date" };
        newRead.Sheets.Sheet1.B1 = { t: "s", v: "F_Customer", w: "F_Customer" };
        newRead.Sheets.Sheet1.C1 = { t: "s", v: "F_LineCode", w: "F_LineCode" };
        newRead.Sheets.Sheet1.D1 = { t: "s", v: "F_Seq", w: "F_Seq" };
        newRead.Sheets.Sheet1.E1 = { t: "s", v: "F_PartCode", w: "F_PartCode" };

        newRead.Sheets.Sheet1["!ref"] = "A1:E" + checkRow;

        //console.log(newRead);

        Object.keys(newRead.Sheets.Sheet1).forEach((key, index) => {
            newRead.Sheets.Sheet1[key].v = newRead.Sheets.Sheet1[key].w;
        });


        const data = XLSX.utils.sheet_to_json(newRead.Sheets[newRead.SheetNames[0]]);
        //console.log(data);

        data.forEach((item, index) => {
            item.F_Date = item.F_Date.toString();
            item.F_Seq = item.F_Seq.toString();
        });

        //return console.log(data);

        if (parseInt(maxRow) !== checkRow) {
            return xSwal.error("Error", "Please check row of import file.");
        }

        _xLib.AJAX_Post("/api/KBNIM0044/SaveImportData", data,
            function (res) {
                xSwal.success("Success", res.message);
                GetListData();
            },
            function (err) {
                xSwal.error(err.responseJSON.response, err.responseJSON.message);
            }
        );


    }
    catch (error) {
        console.error(error);
    }
});

$("#btnSimulate").on("click", async function () {
    xSplash.show();
    isSimulate = true;
    //$("#tableMain tbody tr td input[type='checkbox']").prop("readonly", isSimulate);

    let listObj = $("#tableMain").DataTable().rows().data().toArray();
    //console.log(listObj);
    let notFlaggedLength = listObj.filter(x => x.f_Flag == null).length;
    //console.log(notFlaggedLength);
    if (listObj.length < notFlaggedLength) {
        return xSwal.error("Error", "Data in table is not enough.");
    }

    //listObj.forEach(function (item) {
    //    item.f_Flag = null;
    //    item.f_Deli_Date = null;
    //    item.f_Deli_Shift = null;
    //    item.f_Deli_Trip = null;
    //});

    await UpdateFlag("S");

    $("#btnPost").prop("disabled", true);
    await _xDataTable.ClearData("tableMain");
    await GetListData();
    //$("#btnDel").prop("disabled", true);

});

//$(document).on("dblclick", "#tableMain tbody tr td:not(input[type='checkbox'])", function () {
//    if (isSimulate) {
//        return;
//    }
//    $(this).closest("tr").find("input[type='checkbox']").prop("checked", !$(this).closest("tr").find("input[type='checkbox']").prop("checked"));
//});

$("#chkSlider").on("change", async function () {
    _xDataTable.ClearData("tableMain");
    if ($(this).prop("checked")) {
        $("#btnPost").addClass("d-none");
        $("#btnPost").removeClass("d-block");
        $("#btnRest").addClass("d-block");
        $("#btnRest").removeClass("d-none");
    }
    else {
        $("#btnPost").removeClass("d-none");
        $("#btnPost").addClass("d-block");
        $("#btnRest").removeClass("d-block");
        $("#btnRest").addClass("d-none");
    }
    await GetListData();
});

async function GetListData() {

    let obj = {
        isAll : !$("#chkSlider").prop("checked")
    };
    _xDataTable.ClearData("tableMain");
    await _xLib.AJAX_Get("/api/KBNIM0044/GetListData", obj,
        async function (success) {
            //success = _xLib.JSONparseMixData(success);
            //console.log(success);
            await _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
            listObjChecked = _xDataTable.GetSelectedDataDT("#tableMain");
            xSplash.hide();
            //console.log(listObjChecked);
        },
        function (error) {

        }
    );
}

$("#btnMain").click(function () {
    const changeList = listObjChecked.map(obj => ({ ...obj }));
    //let listObj = $("#tableMain").DataTable().rows().data().toArray();
    //let listCheckBox = $("#tableMain tbody tr td input[type='checkbox']");
    let listCheckedBox = $("#tableMain tbody tr td input[type='checkbox']:checked");


    if (listCheckedBox.length > changeList.length && !$("#chkSlider").prop("checked")) {
        return xSwal.error("Error", "Check box was over simulate data.");
    }

    UpdateFlag("M");

});

$("#btnPost").click(async function () {
    await UpdateFlag("P"); //Postpone
});
$("#btnRest").click(async function () {
    await UpdateFlag("R"); //Restore
});

$("#btnConf").click(function () {
    $("#exampleModal").modal('show');
    //Confirm();
})

$("#btnConfirmModal").click(function () {
    //$("#exampleModal").modal('show');
    Confirm();
})

async function Confirm() {
    let listObj = _xDataTable.GetSelectedDataDT("#tableMain");
    let InchargeUser = $("#inpUserInfModal").val();

    if (listObj.length == 0) {
        return xSwal.error("Error", "Please Select Data before Confirm");
    }
    //if (listObj.length < SeqQty) {
    //    return xSwal.error("Error", "Data in table is not enough.");
    //}
    //return console.log(listObj);

    if (InchargeUser == null) {
        return xSwal.error("Error", "Please Select User to Interface Order");
    }

    _xLib.AJAX_Post("/api/KBNIM0044/Confirm?InchargeUser=" + InchargeUser, listObj,
        async function (success) {
            await GetListData();
            await xSwal.xSuccessAwait(success);
            $("#exampleModal").modal('hide');
        },
        function (error) {
            xSwal.xError(error);
        }
    );
}

async function UpdateFlag(Flag) {
    xSplash.show();

    let listObj = $("#tableMain").DataTable().rows().data().toArray()
    console.log(listObj)
    console.log(listObj.map(x => x.f_Flag));
    let listUpdateObj = listObj.filter(x => !x.f_Flag || x.f_Flag === "");
    console.log(listUpdateObj)
    if (Flag === "P" || Flag === "D") {
        listObj = _xDataTable.GetSelectedDataDT("#tableMain");
        listObj.forEach(function (item) {
            item.f_Flag = Flag
            item.f_Deli_Date = null;
            item.f_Deli_Shift = null;
            item.f_Deli_Trip = null;
        });
    }
    else if (Flag === "R") {
        listObj = _xDataTable.GetSelectedDataDT("#tableMain");
        listObj.forEach(function (item) {
            item.f_Flag = null;
            item.f_Deli_Date = null;
            item.f_Deli_Shift = null;
            item.f_Deli_Trip = null;
        });
    }
    else if (Flag === "S") {
        let Trip = $("#inpTripShift").val();
        let Seq = $("#inpSeqPds").val();
        //let n = 0;
        if ($("#chkSlider").prop("checked")) {
            let TMP_ListObj = [];
            for (let x = 0; x < SeqQty; x++) {
                TMP_ListObj.push(listObj[x])
            }
            listObj = TMP_ListObj;
            console.log(listObj);
        }
        for (let i = 0; i < Seq; i++) {
            listUpdateObj[i].f_Flag = "S";
            listUpdateObj[i].f_Deli_Trip = Trip;
            listUpdateObj[i].f_Deli_Date = moment($("#inpDeliveryDate").val(), "DD/MM/YYYY").format("YYYYMMDD");
            listUpdateObj[i].f_Deli_Shift = $("#inpShift").val().substring(0, 1);
        }
    }
    else if (Flag === "M") {
        const changeList = listObjChecked.map(obj => ({ ...obj }));
        let listObj = $("#tableMain").DataTable().rows().data().toArray();
        let listCheckBox = $("#tableMain tbody tr td input[type='checkbox']");
        let objTrip = {
            Trip1: 0,
            Trip2: 0
        }
        listObj.forEach((item) => {
            console.log(`${'Trip' + item.f_Deli_Trip}`);
            console.log(item.f_Deli_Trip);
            objTrip[`${'Trip' + item.f_Deli_Trip}`] = item.f_Deli_Trip ? objTrip[`${'Trip' + item.f_Deli_Trip}`] += 1 : objTrip[`${'Trip' + item.f_Deli_Trip}`]
        })

        let j = -1;
        for (let i = 0; i < listObj.length; i++) {
            if (j >= changeList.length) break;
            if ($("#chkSlider").prop("checked")) {
                listObj[i].f_Flag = null;
                listObj[i].f_Deli_Date = null;
                listObj[i].f_Deli_Shift = null;
                listObj[i].f_Deli_Trip = null;
            }
            else if (listCheckBox[i].checked) {
                j++;
                listObj[i].f_Flag = "S";
                listObj[i].f_Deli_Date = changeList[j].f_Deli_Date;
                listObj[i].f_Deli_Shift = changeList[j].f_Deli_Shift;
                listObj[i].f_Deli_Trip = changeList[j].f_Deli_Trip;
            }
            else {
                listObj[i].f_Flag = null;
                listObj[i].f_Deli_Date = null;
                listObj[i].f_Deli_Shift = null;
                listObj[i].f_Deli_Trip = null;
            }
        }
    }
    else {
        return xSwal.error("Invalid Action");
    }

    let shift = $("#inpShift").val();

    await _xLib.AJAX_Post("/api/KBNIM0044/UpdateFlag?shift=" + shift, listObj,
        async function (success) {
            await GetListData();
            await xSplash.hide();
            xSwal.success(success.response, success.message);
        },
        function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );

}