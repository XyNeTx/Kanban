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

$("#divBtn").on("click", "button", async function () {

    $("#divBtn").find("button").prop("disabled", true);
    $(this).prop("disabled", false);

    $("#F_Short_Logistic").prop("disabled", false);
    $("#F_Short_Logistic").selectpicker("refresh");

    $("#F_Supplier_CD").prop("disabled", false);
    $("#F_Supplier_CD").selectpicker("refresh");

    if ($(this).attr("id") == "btnUpd") {
        $("#F_Short_Name").prop("disabled", false);
        $("#F_Short_Name").selectpicker("refresh");
    }
    else if ($(this).attr("id") == "btnNew") {

        if ($(document).find("input[id='F_Short_Logistic']").length == 0) {

            await $("select[name='F_Short_Logistic']").parent().remove();

            $("label[for='F_Short_Logistic']").parent().append(
                `
             <input type='text' class="form-control col-7"
             data-val="true" data-val-length="The field Supplier Logistic must be a string
             with a maximum length of 15." data-val-length-max="15" data-val-required="The Supplier Logistic field is required."
             id="F_Short_Logistic" name="F_Short_Logistic"></input>
            `
            );

            await $("select[name='F_Supplier_CD']").parent().remove();

            $("label[for='F_Supplier_CD']").parent().append(
                `
            <input type='text' class="form-control col-7" data-val="true" data-val-length="The field Supplier Code must be a string with a maximum length of 6." data-val-length-max="6" data-val-required="The Supplier Code field is required." id="F_Supplier_CD" name="F_Supplier_CD">
            </input >
            `
            );

            await $("select[name='F_Short_Name']").parent().remove();

            $("label[for='F_Short_Name']").parent().append(
                `
            <input type='text' class="form-control col-7" data-val="true" data-val-length="The field Supplier Order must be a string with a maximum length of 10." data-val-length-max="10" data-val-required="The Supplier Order field is required." id="F_Short_Name" name="F_Short_Name" tabindex="null">
            </input>
            `
            );
        }
    }
    else {
        $("#F_Short_Name").prop("disabled", false);
        $("#F_Short_Name").selectpicker("refresh");
    }



});

$("#btnCan").click(async function () {

    if ($(document).find("select[name='F_Short_Logistic']").length == 0) {

        await $("input[name='F_Short_Logistic']").remove();

        $("label[for='F_Short_Logistic']").parent().append(
            `
        <select class="selectpicker form-control col-7 p-0" data-live-search="true" data-size="8" data-width="100%" disabled="" data-val="true" data-val-length="The field Supplier Logistic must be a string with a maximum length of 15." data-val-length-max="15" data-val-required="The Supplier Logistic field is required." id="F_Short_Logistic" name="F_Short_Logistic"></select>
        `
        );

        await $("input[name='F_Supplier_CD']").remove();

        $("label[for='F_Supplier_CD']").parent().append(
            `
        <select class="selectpicker form-control col-7 p-0" data-live-search="true" data-size="8" data-width="100%" disabled="" data-val="true" data-val-length="The field Supplier Code must be a string with a maximum length of 6." data-val-length-max="6" data-val-required="The Supplier Code field is required." id="F_Supplier_CD" name="F_Supplier_CD">
        </select>
        `
        );

        await $("input[name='F_Short_Name']").remove();

        $("label[for='F_Short_Name']").parent().append(
            `
        <select class="selectpicker form-control col-7 p-0" data-live-search="true" data-size="8" data-width="100%" disabled="" data-val="true" data-val-length="The field Supplier Order must be a string with a maximum length of 10." data-val-length-max="10" data-val-required="The Supplier Order field is required." id="F_Short_Name" name="F_Short_Name"></select>
        `
        );
    }

    $("#divBtn").find("button").prop("disabled", false);

    console.log($(document).find("#F_Short_Logistic"));

    $(document).find("#F_Short_Logistic").selectpicker("refresh");
    $(document).find("#F_Short_Logistic").prop("disabled", true);
    $(document).find("#F_Short_Logistic").resetSelectPicker();

    $(document).find("#F_Short_Name").selectpicker("refresh");
    $(document).find("#F_Short_Name").prop("disabled", true);
    $(document).find("#F_Short_Name").resetSelectPicker();

    $(document).find("#F_Supplier_CD").selectpicker("refresh");
    $(document).find("#F_Supplier_CD").prop("disabled", true);
    $(document).find("#F_Supplier_CD").resetSelectPicker(); 

    GetShortLogistic();
    GetShortName();

    $("#F_name").val("");
    $("#tableMain").DataTable().clear().draw();

});

$(document).on('change',"#F_Short_Logistic",function () {
    GetListData();
    $("#F_Short_Name").resetSelectPicker();
    $('#F_Supplier_CD').resetSelectPicker();
    $("#F_name").val("");

});

$(document).on('change', "#F_Short_Name",function () {
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

$("#btnRpt").click(function () {
    OpenReport();
});

async function GetShortLogistic() {
    _xLib.AJAX_Get('/api/KBNMS027/GetShortLogistic', null,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            $("#F_Short_Logistic").addListSelectPicker(success.data, "f_short_Logistic")
            console.log(success);
        },
        function (error) {
            console.log(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
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
            console.log(error);
            //xSwal.error(error.responseJSON.response, error.responseJSON.message);
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
            console.log(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
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

    let action = $("#divBtn").find("button:not(:disabled)").attr("id").split("btn")[1];

    _xLib.AJAX_Post('/api/KBNMS027/Save?action=' + action, listData,
        function (success) {
            console.log(success);
            success.data = _xLib.TrimArrayJSON(success.data);
            xSwal.success(success.response, success.message);
            GetListData();
        },
        function (error) {
            console.log(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );

}

async function OpenReport() {
    let data = {
        Plant: ajexHeader.Plant,
        UserName: _xLib.GetUserName(),
    }

    _xLib.OpenReportObj("/KBNMS027", data);

}
