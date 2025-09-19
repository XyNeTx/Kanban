$(document).ready(async function () {
    await GetSupplier();
    xSplash.hide();

    async function GetSupplier() {
        _xLib.AJAX_Get("/api/KBNLC200/GetSupplier", "",
            async function (success) {
                console.log(success.data);


                success.data.forEach(item => {

                    $('#inpSupplierFrom').append(new Option(item, item));
                    $('#inpSupplierTo').append(new Option(item, item));

                });
                $("#inpSupplierFrom, #inpSupplierTo").resetSelectPicker();

                //await $("#inpSupplier").addListSelectPicker(success.data, "f_Supplier_Code");
            },
            function (error) {
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

    $("#btnReport").click(async function () {

        xSplash.show("Loading...");

        var postObj = {
            Cmb_SupF: $("#inpSupplierFrom").val(),
            Cmb_SupT: $("#inpSupplierTo").val(),
            ProdYM: $("#datPeriod").val(),
        }


        await _xLib.AJAX_Post("/api/KBNLC200/Print", postObj,
            function (success) {

                let userName = _xLib.GetUserName();
                let ProdYM = $("#datPeriod").val();
                let Cmb_SupF = $("#inpSupplierFrom").val();
                let Cmb_SupT = $("#inpSupplierTo").val();
                let period = (Cmb_SupF !== "" && Cmb_SupT !== "")
                           ? `${ProdYM} Supplier : ${Cmb_SupF} - ${Cmb_SupT}` : ProdYM;

                let reportUrl = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx?%2fKB3%2fKBNLC200&rs:Command=Render";
                window.open(reportUrl + '&UserName=' + userName + '&Period=' + period , '_blank');

                xSplash.hide();
                xSwal.success("Success", "Process Success");
            },
            function (error) {
                xSplash.hide();
                xSwal.error("Not Found Data", error.responseJSON.message);
            }
        );

    });

});