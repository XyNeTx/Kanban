$(document).ready(async function () {
    //xSwal.success("Success", "Data has been saved.");

    $(document).find("input[class='datepicker form-control']").each(function () {
        $(this).initDatepicker();
    });

    await GetSupplier();
    xSplash.hide();

    $('.input-group-text.event').on('click', function () {
        $(this).closest('.input-group').find('input.datepicker').focus();
    });
});

async function GetSupplier()
{
    _xLib.AJAX_Get("/api/KBNOC150/GetSupplier", "",
        async function (success)
        {
            console.log(success.data);


            success.data.forEach(item => {

                $('#inpSupplierFrom').append(new Option(item, item));
                $('#inpSupplierTo').append(new Option(item, item));

            });
            $("#inpSupplierFrom, #inpSupplierTo").resetSelectPicker();

            //await $("#inpSupplier").addListSelectPicker(success.data, "f_Supplier_Code");
        },
        function (error)
        {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );
}

$("#inpSupplierFrom").on("change", function () {
    let supfrom = $("#inpSupplierFrom").val();
    console.log(supfrom);
    $("#inpSupplierTo").val(supfrom);
    $("#inpSupplierTo").selectpicker("refresh");
});

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

$("#Print").click(async function () {

    xSplash.show("Loading...");

    var suipplierFrom = $("#inpSupplierFrom").val().split(" : ");
    var suipplierTo = $("#inpSupplierTo").val().split(" : ");

    var postObj = {
        Cmb_SupF: suipplierFrom[0],
        Cmb_SupT: suipplierTo[0],
        Txt_DeliveryF: moment($("#F_DeliveryFrom").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        Txt_DeliveryT: moment($("#F_DeliveryTo").val(), "DD/MM/YYYY").format("YYYYMMDD"),
    }

    await _xLib.AJAX_Post("/api/KBNOC160/Print", postObj,
        function (success) {

            let userName = _xLib.GetUserName();
            let reportUrl = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx?%2fKB3%2fKBNOC160&rs:Command=Render";
            window.open(reportUrl + '&UserName=' + userName, '_blank');

            xSplash.hide();
            xSwal.success("Success", "Process Success");
        },
        function (error) {
            xSwal.error("Not Found Data", error.responseJSON.message);
        }
    );

});



