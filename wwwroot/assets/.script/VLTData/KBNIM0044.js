$(document).ready(function () {
    setTimeout(() => xSplash.hide(), 1000);

    _xDataTable.InitialDataTable("#tableMain",
    {
        "processing": false,
        "serverSide": false,
        width: '100%',
        paging: false,
        sorting: false,
        searching: false,
        scrollX: "100%",
        scrollY: "245px",
        scrollCollapse: false,
        "columns": [
            {
                title: "Flag", render(data, type, row) {
                    return `<input type="checkbox" class="chkbox" id="chkbox" name="chkbox" value="${row.f_Supplier_Plant}">`;
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
                title: "Delivery Date", data: "f_Deli_Date"
            },
            {
                title: "Delivery Shift", data: "f_Deli_Shift"
            },
            {
                title: "Delivery Trip", data: "f_Deli_Trip"
            },
        ],
        select: false,
        order: [[0, "asc"]]
    });

    $("table tbody tr td").addClass("text-center");
    $("table thead tr th").addClass("text-center");

    $("#inpDeliveryDate").initDatepicker();

    GetListData();

});
var files = [];
$("#inpFile").on("change", function (e) {
    files = e.target.files;
});

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
        console.log(maxRow);

        let newRead = read;

        var checkRow = parseInt(newRead.Sheets.Sheet1["A" + maxRow].v.replace(/#/g, "").replace(/T/g, ""));
        console.log(checkRow);

        delete newRead.Sheets.Sheet1["A1"];
        delete newRead.Sheets.Sheet1["A" + maxRow];

        newRead.Sheets.Sheet1.A1 = { t: "s", v: "F_Date", w: "F_Date" };
        newRead.Sheets.Sheet1.B1 = { t: "s", v: "F_Customer", w: "F_Customer" };
        newRead.Sheets.Sheet1.C1 = { t: "s", v: "F_LineCode", w: "F_LineCode" };
        newRead.Sheets.Sheet1.D1 = { t: "s", v: "F_Seq", w: "F_Seq" };
        newRead.Sheets.Sheet1.E1 = { t: "s", v: "F_PartCode", w: "F_PartCode" };

        newRead.Sheets.Sheet1["!ref"] = "A1:E" + checkRow;

        console.log(newRead);

        Object.keys(newRead.Sheets.Sheet1).forEach((key, index) => {
            newRead.Sheets.Sheet1[key].v = newRead.Sheets.Sheet1[key].w;
        });


        const data = XLSX.utils.sheet_to_json(newRead.Sheets[newRead.SheetNames[0]]);
        console.log(data);

        data.forEach((item, index) => {
            item.F_Date = item.F_Date.toString();
            item.F_Seq = item.F_Seq.toString();
        });

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

$(document).on("click", "#tableMain tbody tr td:not(input[type='checkbox'])", function () {
    $(this).closest("tr").find("input[type='checkbox']").prop("checked", !$(this).closest("tr").find("input[type='checkbox']").prop("checked"));
});

$("#chkSlider").on("change", function () {
    GetListData();
});

function GetListData() {

    let obj = {
        isAll : !$("#chkSlider").prop("checked")
    };

    _xLib.AJAX_Get("/api/KBNIM0044/GetListData", obj,
        function (success) {
            //success = _xLib.JSONparseMixData(success);
            console.log(success);
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        },
        function (error) {
        }
    );
}

$("#btnPost").click(function () {
    UpdateFlag("P");
});
$("#btnDel").click(function () {
    UpdateFlag("D");
});

function UpdateFlag(Flag) {
    let listObj = _xDataTable.GetSelectedDataDT("#tableMain");

    listObj.forEach(function (item) {
        item.F_Flag = Flag
    });

    _xLib.AJAX_Post("/api/KBNIM0044/UpdateFlag", listObj,
        function (success) {
            xSwal.success(success.response, success.message);
            GetListData();
        },
        function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );
}