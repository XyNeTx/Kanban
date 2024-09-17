$(document).ready(async function () {

    await Initial();

    let loginDate = _xLib.GetCookie("loginDate");
    let plant = _xLib.GetCookie("plantCode");
    console.log(loginDate);
    console.log(plant);


    $("#readProcessDate").val(moment(loginDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));
    $("#readProcessShift").val(loginDate.slice(10, 11) == "D" ? "1:Day Shift" : "2:Night Shift");
    $("#readPlant").val(plant);

    $("#chkDeliveryDate").prop("checked", true);

    await GetCustomerPO();
    await xSplash.hide();

});

$("#btnExit").click(function () {
    window.location.replace("/OrderingProcess/KBNOR200");
});

$("#btnCheckAll").click(function () {
    $("#tableMain tbody tr").each(function () {
        $(this).find("input[type='checkbox']").prop("checked", true);
    });
});

$("#btnUncheckAll").click(function () {
    $("#tableMain tbody tr").each(function () {
        $(this).find("input[type='checkbox']").prop("checked", false);
    });
});

async function Initial() {
    await $("#tableMain").DataTable({
        width: '100%',
        paging: false,
        scrollCollapse: true,
        "processing": false,
        "serverSide": false,
        scrollX: false,
        scrollY: '400px',
        searching: false,
        info: false,
        ordering: false,
        columns: [
            {
                title: "Flag", data: "Flag", render: function ()
                {
                    return `<input type="checkbox" class="chkboxTable" id="chkFlag" name="chkFlag">`
                }
            },
            { title: "Prod YM", data: "f_ProdYM" },
            { title: "Customer OrderNo.", data: "f_PDS_No" },
            { title: "Delivery Date", data: "f_Delivery_Date" },
            { title: "Cust.OrderType", data: "f_CusOrderType_CD" },
        ],
        order: [[1, 'asc']]
    });

    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");
}

$("#chkDeliveryDate").on("change", function () {
    if ($(this).is(":checked")) {
        $("#inpDeliveryDate").prop("disabled", false);
    } else {
        $("#inpDeliveryDate").prop("disabled", true);
    }
});

$("#chkCustomerOrderNo").on("change", function () {
    if ($(this).is(":checked")) {
        $("#inpCustomerOrderNo").prop("disabled", false);
    } else {
        $("#inpCustomerOrderNo").prop("disabled", true);
    }
});


async function GetCustomerPO() {

    await xSplash.show();

    let obj = {};

    if ($("#chkDeliveryDate").is(":checked")) {
        obj.DeliDT = $("#inpDeliveryDate").val().replaceAll("-", "");
    }
    if ($("#chkCustomerOrderNo").is(":checked")) {
        obj.OrderNo = $("#inpCustomerOrderNo").val();
    }

    _xLib.AJAX_Get("/api/KBNOR210_2/GetCustomerPO", obj,
        async function (success) {
            console.log(success);
            success.data = _xLib.TrimArrayJSON(success.data);
            $("#tableMain").DataTable().clear().rows.add(success.data).columns.adjust().draw();
            $("table thead tr th").css("text-align", "center");
            $("table tbody tr td").css("text-align", "center");

            $("#btnCheckAll").prop("disabled", false);
            $("#btnUncheckAll").prop("disabled", false);
            $("#btnMerge").prop("disabled", false);

            await xSplash.hide();
        },
        function (error) {
            console.log(error);
            $("#btnCheckAll").prop("disabled", true);
            $("#btnUncheckAll").prop("disabled", true);
            $("#btnMerge").prop("disabled", true);
        }
    );


};

$("#inpDeliveryDate , #inpCustomerOrderNo , #chkDeliveryDate , #chkCustomerOrderNo").change(async function () {
    await GetCustomerPO();
});

$("#btnMerge").click(async function () {

    await xSplash.show();

    let listObj = [];

    $("#tableMain input[type='checkbox']:checked").each(function () {
        let obj = $("#tableMain").DataTable().row($(this).closest("tr")).data();
        obj.F_PDS_No_New = $("#inpNewCustomerOrderNo").val();
        listObj.push(obj);
    });

    console.log(listObj);


    if ($("#inpNewCustomerOrderNo").val().includes(",")) {
        return xSwal.error("Cannot Use ',' in Customer Order No");
    }
    if (listObj.length == 0) {
        await xSplash.hide();
        xSwal.error("Please select one customer orderno for merge data!");
        return;
    }
    if (listObj.length > 22) {
        // 22 is max row for merge
        await xSplash.hide();
        xSwal.error("Please select Customer OrderNo not over than 22 OrderNo.");
        return;
    }

    _xLib.AJAX_Post("/api/KBNOR210_2/Merge", JSON.stringify(listObj),
        function(success) {
            console.log(success);
            if (success.status) {
                xSwal.success("Merge Success!");
                $("#inpNewCustomerOrderNo").val("");
                GetCustomerPO();
            } else {
                xSwal.error("Merge Fail!");
            }
        })

    await xSplash.hide();
});