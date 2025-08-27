$(document).ready(async function () {
    $(".btn-toolbar").remove();

    await _xDataTable.InitialDataTable("#tableHeader",
        {
            columns: [
                {
                    // title: "No", data: "",width: "7%", render: function (data, type, row, meta) {
                    //     //console.log("data:", data);
                    //     //console.log("type:", type);
                    //     //console.log("row:", row);
                    //     //console.log("meta:", meta);
                    //     return meta.row + 1;
                    // }
                    title :"Flag",data:"",width:"7%",render: function(data, type, row, meta) {
                        return "<input type='checkbox' class='chkFlag' checked/>";
                    }
                },
                { title: "Seperate Declaration No", data: "F_Declare_No", width: "17%" },
                { title: "Customer PO", data: "F_Pds_No", width: "12%" },
                { title: "Delivery Date", data: "F_Delivery_Date", width: "24%" },
            ],
            order: [[1, "asc"]],
            scrollCollapse: false,
            width: "100%",
            scrollX: true,
            scrollY: '150px',
        }
    );

    await _xDataTable.InitialDataTable("#tableDetail",
        {
            columns: [
                {
                    // title: "No", data: "", render: function (data, type, row, meta) {
                    //     //console.log("data:", data);
                    //     //console.log("type:", type);
                    //     //console.log("row:", row);
                    //     //console.log("meta:", meta);
                    //     return meta.row + 1;
                    // }

                    title :"Flag",data:"",width:"7%",render: function(data, type, row, meta) {
                        return "<input type='checkbox' class='chkFlagDetail' />";
                    }
                },
                {
                    title: "Seperate Declaration No", data: "F_Declare_No"
                },
                {
                    title: "Refer. Doc.", data: "F_Pds_No"
                },
                {
                    title: "Delivery Date", data: "F_Delivery_Date"
                },
                {
                    title: "Part No.", data: "F_Part_No"
                },
                {
                    title: "Kanban No", data: "F_Kanban_NO"
                },
                {
                    title: "Supplier", data: "F_SUpplier"
                },
                {
                    title: "Qty", data: "F_QTY"
                },
            ],
            order: [[1, "asc"]],
            scrollCollapse: false,
            width: "100%",
            scrollX: true,
            scrollY: '225px',
        }
    );

    $("#txtDeliDate").initDatepicker();

    await GetList_Header();
    
    xSplash.hide();
});

$("#btnSearch").click(async () => {
    await GetList_Detail();
})
$("input[name='radSelect']").change(async (x) => {
    if (x.target.value == "all") {
        $(".chkFlag").prop("checked", true);
    }
    else {
        $(".chkFlag").prop("checked", false);
    }
})
$("input[name='radSelect2']").change(async (x) => {
    console.log(x.target.value);
    if (x.target.value == "all") {
        $(".chkFlagDetail").prop("checked", true);
    }
    else {
        $(".chkFlagDetail").prop("checked", false);
    }
})
//$("input[name='radPDS']").change(async (x) => {
//    console.log(x.target.id);
//    if (x.target.id == "rad1PDS") {
//        //$("#rad1PDS").attr("checked","")
//        $("#rad2PDS").removeAttr("checked")
//    }
//    else {
//        //$("#rad2PDS").attr("checked","")
//        $("#rad1PDS").removeAttr("checked")
//    }
//})

$("#chkChangeDeliDate").change(function () {
    if ($(this).val() == "1") {
        $("#txtDeliDate").prop("readonly", false)
    }
    else {
        $("#txtDeliDate").prop("readonly", true)
    }
});

$("#btnInf").click(async function () {
    await InterfaceDataToTransactionTMP();
})

$("#btnDel").click(async function () {
    await Delete();
})

async function GetList_Header() {

    _xLib.AJAX_Get("/api/KBNIM013_INV/GetList_Header","",
        function success(success) {
            console.log(success);
            _xDataTable.ClearAndAddDataDT("tableHeader", success);
            _xDataTable.ClearData("tableDetail");
        },
        function error(error) {
            console.error(error);
        }
    );
}

async function GetList_Detail() {

    var listObj = _xDataTable.GetSelectedDataDT("#tableHeader");
    let stringBuilder = "";

    listObj.forEach(async (x) => {
        return stringBuilder += `'${x.F_Declare_No}',`
    })

    stringBuilder = stringBuilder.slice(0, stringBuilder.length - 1);

    console.log(stringBuilder);
    var obj = {
        inDeclareNo: stringBuilder
    };

    _xLib.AJAX_Get("/api/KBNIM013_INV/GetList_Detail", obj,
        function success(success) {
            console.log(success);
            _xDataTable.ClearAndAddDataDT("tableDetail", success);
        },
        function error(error) {
            console.error(error);
        }
    );
}


async function InterfaceDataToTransactionTMP() {

    var selectedDetail = _xDataTable.GetSelectedDataDT("tableDetail");
    var PDS = $("input[name='radPDS']:checked").val();
    //return console.log(PDS);
    if ($("#chkChangeDeliDate").prop("checked")) {
        selectedDetail.forEach((x) => {
            x.F_Delivery_Date = moment($("#txtDeliDate").val(), "DD/MM/YYYY").format("YYYYMMDD");
        })
    }

    await _xLib.AJAX_Post("/api/KBNIM013_INV/InterfaceDataToTransactionTemp?PDS=" + PDS, selectedDetail,
        async function (successMsg) {
            xSwal.xSuccess(successMsg);
            await GetList_Header();
            await GetList_Detail();
        },
        function (errorObj) {
            xSwal.xError(errorObj);
        }
    );

}

async function Delete() {
    var selectedDetail = _xDataTable.GetSelectedDataDT("tableDetail");

    await _xLib.AJAX_Post("/api/KBNIM013_INV/Delete", selectedDetail,
        async function (successMsg) {
            xSwal.xSuccess(successMsg);
            await GetList_Header();
            await GetList_Detail();
        },
        function (errorObj) {
            xSwal.xError(errorObj);
        }
    );
}