$(document).ready(async function () {

    _xDataTable.InitialDataTable("#tableMain",
        {
            columns: [
                {
                    title: "Flag", render: function (data, type, row) {
                        return '<input type="checkbox" class="chkboxTable" id="chkFlag" name="chkFlag">';
                    }
                },
                { title: "Supplier Code", data: "f_Supplier_Code" ,width:"10%"},
                { title: "StoreCD", data: "f_Store_cd"},
                { title: "Delivery Date", data: "f_Delivery_Date" },
                { title: "Trip", data: "f_Delivery_Trip" },
                { title: "PartNo", data: "f_Part_No" ,width:"15%"},
                { title: "Keep Order", data: "f_Keep_Order" },
                { title: "SlideDateTo", data: "f_Slide_Date" },
                { title: "TripNext", data: "f_Slide_Trip" },
            ],
            scrollY: "250px",
            scrollCollapse: false,
            ordering: false,
            order: [[1, "asc"]],
        }
    );

    $(document).find("input[class='datepicker form-control']").each(function () {
        $(this).initDatepicker();
    });

    await GetSupplier();
    await GetStoreCD();
    await GetPartNo();

    xSplash.hide();

});

async function GetSupplier()
{
    _xLib.AJAX_Get("/api/KBNOC121/GetSupplier", "",
        async function (success)
        {
            await $("#inpSupplier").addListSelectPicker(success.data, "f_Supplier_Code");
            $("#inpSupplier").resetSelectPicker();
        },
        function (error)
        {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );
}

async function GetStoreCD() {

    var getQuery = {
        SupplierCD: $("#inpSupplier").val(),
    }

    _xLib.AJAX_Get("/api/KBNOC121/GetStore", getQuery,
        async function (success) {
            await $("#inpStoreCd").addListSelectPicker(success.data, "f_Store_cd");
            $("#inpStoreCd").resetSelectPicker();
        },
        function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );

}

async function GetPartNo() {

    var getQuery = {
        SupplierCD: $("#inpSupplier").val(),
        StoreCD: $("#inpStoreCd").val(),
    }

    _xLib.AJAX_Get("/api/KBNOC121/GetPartNo", getQuery,
        async function (success) {
            await $("#inpPartNo").addListSelectPicker(success.data, "f_Part_No");
            $("#inpPartNo").resetSelectPicker();
        },
        function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );
}
async function GetListData() {
    var getQuery = {
        SupplierCD: $("#inpSupplier").val(),
        StoreCD: $("#inpStoreCd").val(),
        PartNo: $("#inpPartNo").val(),
    }

    _xLib.AJAX_Get("/api/KBNOC121/GetListData", getQuery,
        async function (success) {
            return _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        },
        function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );
}

$("#inpSupplier").on("change", async function () {
    await GetStoreCD();
    await GetPartNo();
    await GetListData();
});

$("#inpStoreCd").on("change", async function () {
    await GetPartNo();
    await GetListData();
});

$("#btnSelectAll").click(function () {
    $(".chkboxTable").prop("checked", true);
});

$('#btnDeselectAll').click(function () {
    $(".chkboxTable").prop("checked", false);
});


$("#divBtn").on("click", "button[type='button']", async function () {
    $("#divBtn").find("button").prop("disabled", true);
    $(this).prop("disabled", false);

    $("#inpSupplier").prop("disabled", false);
    $("#inpStoreCd").prop("disabled", false);
    $("#inpPartNo").prop("disabled", false);
    $("#inpSupplier").resetSelectPicker();
    $("#inpStoreCd").resetSelectPicker();
    $("#inpPartNo").resetSelectPicker();


    if ($(this).attr("id") != "btnNew") {
        await GetListData();
    }

    if ($(this).attr("id") == "btnInq" || $(this).attr("id") == "btnDel") {
        return;
    }

    $("#inpDeliveryDate").prop("disabled", false);
    $("#inpTrip").prop("disabled", false);
    $("#chkSlideOrder").prop("disabled", false);
    $("#inpSlideDate").prop("disabled", true);
    $("#inpTripNext").prop("disabled", true);


});

$(document).on("dblclick", "#tableMain tbody tr", function () {
    $(this).closest("tr").find("input[type='checkbox']").prop("checked", true);
    var data = _xDataTable.GetDataDT("#tableMain", $(this).closest("tr"));
    //console.log(data);
    $("#inpSupplier").val(data.f_Supplier_Code);
    $("#inpStoreCd").val(data.f_Store_cd);
    $("#inpDeliveryDate").val(data.f_Delivery_Date);
    $("#inpTrip").val(data.f_Delivery_Trip);
    $("#chkSlideOrder").prop("checked", data.f_Keep_Order == "Cancel" ? false : true);
    $("#inpSlideDate").val(data.f_Slide_Date);
    $("#inpTripNext").val(data.f_Slide_Trip);
    $("#inpPartNo").val(data.f_Part_No);

    $("#inpSupplier").selectpicker("refresh");
    $("#inpStoreCd").selectpicker("refresh");
    $("#inpPartNo").selectpicker("refresh");

    if ($("#divBtn #btnUpd").prop("disabled") === false)
    {
        $("#inpSupplier").prop("disabled", true);
        $("#inpStoreCd").prop("disabled", true);
        $("#inpPartNo").prop("disabled", true);
        $("#inpDeliveryDate").prop("disabled", true);
        $("#inpTrip").prop("disabled", true);

        $("#inpSupplier").selectpicker("refresh");
        $("#inpStoreCd").selectpicker("refresh");
        $("#inpPartNo").selectpicker("refresh");
    }

});

$("#btnCan").on("click", async function () {
    $("#divBtn").find("button").prop("disabled", false);
    $("#tableMain").DataTable().clear().draw();

    $("#inpDeliveryDate").val(moment().format("DD/MM/YYYY"));
    $("#inpTrip").val("1");
    $("#chkSlideOrder").prop("checked", false);
    $("#inpSlideDate").val(moment().format("DD/MM/YYYY"));
    $("#inpTripNext").val("1");

    $("#inpSupplier").prop("disabled", true);
    $("#inpStoreCd").prop("disabled", true);
    $("#inpPartNo").prop("disabled", true);
    $("#inpDeliveryDate").prop("disabled", true);
    $("#inpTrip").prop("disabled", true);
    $("#chkSlideOrder").prop("disabled", true);
    $("#inpSlideDate").prop("disabled", true);
    $("#inpTripNext").prop("disabled", true);

    //$("#inpSupplier").val("");
    //$("#inpStoreCd").val("");

    await $("#inpSupplier").resetSelectPicker();
    await $("#inpStoreCd").resetSelectPicker();
    await $("#inpPartNo").resetSelectPicker();

    //$("#formMain").trigger("reset");

});

$("#chkSlideOrder").change(function () {
    if ($(this).prop("checked")) {
        $("#inpSlideDate").prop("disabled", false);
        $("#inpTripNext").prop("disabled", false);
    } else {
        $("#inpSlideDate").prop("disabled", true);
        $("#inpTripNext").prop("disabled", true);
    }
});

$("#btnSave").on("click", async function () {
    var postObj = await PostObj();
    let action = $("#divBtn").find("button[type='button']:not([disabled])").attr("id").split("btn")[1];

    _xLib.AJAX_Post("/api/KBNOC121/Save?action=" + action, postObj,
        function (success) {
            xSwal.success("Success", "Data has been saved.");
            GetListData();
        },
        function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );
});


async function PostObj() {

    var listObj = [];

    let action = $("#divBtn").find("button[type='button']:not([disabled])").attr("id").split("btn")[1];

    //console.log($("#chkSlideOrder").prop("checked"));

    var postObj = {
        Supplier: $("#inpSupplier").val(),
        StoreCD: $("#inpStoreCd").val(),
        DeliveryDate: moment($("#inpDeliveryDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        Trip: $("#inpTrip").val(),
        IsSlideOrder: $("#chkSlideOrder").prop("checked"),
        SlideDateTo: moment($("#inpSlideDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        TripNext: $("#inpTripNext").val(),
        PartNo: $("#inpPartNo").val(),
    }

    if (action == "Del")
    {
        listObj = _xDataTable.GetSelectedDataDT("#tableMain");

        if (listObj.length == 0)
        {
            xSwal.error("Error", "Please select data.");
            return false;
        }

        listObj = listObj.map(function (data) {
            //console.log(data.f_Keep_Order);
            return {
                Supplier: data.f_Supplier_Code,
                StoreCD: data.f_Store_cd,
                DeliveryDate: moment(data.f_Delivery_Date,"DD/MM/YYYY").format("YYYYMMDD"),
                Trip: data.f_Delivery_Trip.toString(),
                IsSlideOrder: data.f_Keep_Order == "Cancel" ? false : true,
                SlideDateTo: moment(data.f_Slide_Date, "DD/MM/YYYY").format("YYYYMMDD"),
                TripNext: data.f_Slide_Trip.toString(),
                PartNo: data.f_Part_No,
            }
        });

        return listObj;
    }

    listObj.push(postObj);

    return listObj;
}

let inpFile = "";

$("#inpFile").change(function () {
    inpFile = this.files[0];
});

$("#btnRpt").click(async function () {

    var obj = {
        UserName: _xLib.GetUserName(),
    }

    return _xLib.OpenReportObj("/KBNOC121", obj);

});

$("#btnImport").click(async function () {

    //console.log('Files:', inpFile);
    const arrayBuffer = await inpFile.arrayBuffer();
    const read = await XLSX.read(arrayBuffer);

    let newRead = read;

    for (var key in newRead.Sheets[newRead.SheetNames[0]]) {
        newRead.Sheets[newRead.SheetNames[0]][key].v = newRead.Sheets[newRead.SheetNames[0]][key].w;
    }

    const data = XLSX.utils.sheet_to_json(newRead.Sheets[newRead.SheetNames[0]]);

    for (let i = 0; i < data.length ; i++)
    {
        try {
            let mockObj = {
                Supplier: data[i].SupplierCode,
                StoreCD: data[i]["Store Code"],
                DeliveryDate: moment(data[i].DeliveryDate, "DD/MM/YYYY").format("YYYYMMDD"),
                Trip: data[i].DeliveryTrip,
                IsSlideOrder: data[i].KeepOrder == "Y" ? true : false,
                SlideDateTo: moment(data[i]["Slide to Date"], "DD/MM/YYYY").format("YYYYMMDD"),
                TripNext: data[i]["Slide to Trip"],
                PartNo: data[i].PartNo.slice(0,10) + "-" + data[i].PartNo.slice(10,12),
            }

            let postObj = [];

            postObj.push(mockObj);

            _xLib.AJAX_Post("/api/KBNOC121/Save?action=New", postObj,
                function (success) {
                    console.log(postObj[0]);
                    console.log("Success");
                },
                function (error) {
                    console.log("Error");
                    throw new Error(error.responseJSON.message);
                }
            );
        }
        catch (e) {
            xSwal.error("Error", `Row ${i + 1} : ${e.message}`);
        }
    }

    await xSwal.success("Success", "Data has been imported.");
    GetListData();

});