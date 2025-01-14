$(document).ready(function () {


    _xDataTable.InitialDataTable("#tableMain",
        {
            "processing": false,
            "serverSide": false,
            width: '100%',
            paging: false,
            sorting: false,
            searching: false,
            scrollX: false,
            scrollY: "200px",
            scrollCollapse: true,
            "columns": [
                {
                    title: "Flag", render(data, type, row) {
                        return `<input type="checkbox" class="chkbox" id="chkbox" name="chkbox" value="${row.f_Supplier_Plant}">`;
                    }
                },
                {
                    title: "SupplierLogistic", data: "f_short_Logistic"
                },
                {
                    title: "SupplierOrder", data: "f_short_name"
                },
                {
                    title: "SupplierCode", data: "f_Supplier_CD", render(data, type, row) {
                        return `${row.f_Supplier_CD}-${row.f_Supplier_Plant}`;
                    }
                },
                {
                    title: "SupplierName", data: "f_name"
                },
            ],
            select: false,
            order: [[0, "asc"]]
        });

    $("table tbody tr td").addClass("text-center");
    $("table thead tr th").addClass("text-center");

    GetShortLogistic();
    GetShortName();

    xSplash.hide();


});

$("#divBtn").on("click", "button", function () {

    $("#divBtn").find("button").prop("disabled", true);
    $(this).prop("disabled", false);

    $("#F_Short_Logistic").prop("disabled", false);
    $("#F_Short_Logistic").selectpicker("refresh");

    if ($(this).attr("id") == "btnNew" || $(this).attr("id") == "btnUpd") {
        $("#F_Short_Name").prop("disabled", false);
        $("#F_Short_Name").selectpicker("refresh");
    }
    else {
        $("#F_Short_Name").prop("disabled", true);
        $("#F_Short_Name").selectpicker("refresh");
    }
   

    $("#F_Supplier_CD").prop("disabled", false);
    $("#F_Supplier_CD").selectpicker("refresh");


});

$("#btnCan").click(function () {

    $("#divBtn").find("button").prop("disabled", false);
    $("#F_Short_Logistic").prop("disabled", true);
    $("#F_Short_Logistic").resetSelectPicker();
    $("#F_Short_Name").prop("disabled", true);
    $("#F_Short_Name").resetSelectPicker();
    $("#F_Supplier_CD").prop("disabled", true);
    $("#F_Supplier_CD").resetSelectPicker();
    $("#F_name").val("");
    $("#tableMain").DataTable().clear().draw();

});

$("#F_Short_Logistic").change(function () {
    GetListData();
    $("#F_Short_Name").resetSelectPicker();
    $('#F_Supplier_CD').resetSelectPicker();
    $("#F_name").val("");

});

$("#F_Short_Name").change(function () {
    SupOrderSelected();
});

$("#btnSelAll").click(function () {
    $(".chkbox").prop("checked", true);
});

$("#btnDeselAll").click(function () {
    $(".chkbox").prop("checked", false);
});

$("#btnSave").click(function () {
    Save();
});

async function GetShortLogistic() {
    _xLib.AJAX_Get('/api/KBNMS027/GetShortLogistic', null,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            $("#F_Short_Logistic").addListSelectPicker(success.data, "f_short_Logistic")
            console.log(success);
        },
        function (error) {
        }
    );
}

async function GetShortName() {
    _xLib.AJAX_Get('/api/KBNMS027/GetShortName', null,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            $("#F_Short_Name").addListSelectPicker(success.data, "f_short_name")
            console.log(success);
        },
        function (error) {
        }
    );
}

async function GetListData() {
    let data = {
        F_Short_Logistic: $("#F_Short_Logistic").val(),
    };

    _xLib.AJAX_Get('/api/KBNMS027/GetListData', data,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            console.log(success);
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        },
        function (error) {
        }
    );

}

async function SupOrderSelected() {
    let data = {
        F_Short_Name: $("#F_Short_Name").val(),
    };

    _xLib.AJAX_Get('/api/KBNMS027/SupOrderSelected', data,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            console.log(success);
            $("#F_name").val(success.data[0].f_name);
            $("#F_Supplier_CD").addListSelectPicker(success.data, "f_supplier_cd");
        },
        function (error) {
        }
    );
}

async function Save() {
    let listData = [];

    let data = {
        F_Short_Logistic: $("#F_Short_Logistic").val(),
        F_Short_Name: $("#F_Short_Name").val(),
        F_Supplier_CD: $("#F_Supplier_CD").val(),
        F_name: $("#F_name").val(),
    };

    listData.push(data);

    if ($("#divBtn").find("button:not(:disabled)").attr("id") == "btnDel") {
        listData = _xDataTable.GetSelectedDataDT("#tableMain");
    }

    console.log(listData);
    console.log($("#divBtn").find("button:not(:disabled)").attr("id"));


    _xLib.AJAX_Post('/api/KBNMS027/Save', listData,
        function (success) {
            console.log(success);
            GetListData();
        },
        function (error) {
        }
    );

}
