$(document).ready(async function () {

    //xDataTableExport.ButtonPDF.orientation = 'landscape';

    //const tblMaster = xDataTable.Initial({
    //    name: 'tblMaster',
    //    scrollbar: "300px",
    //    dom: '<"top">t<"clear">',
    //    //checking: 0,
    //    //running: 0,
    //    columnTitle: {
    //        "EN": ['Supplier Code', 'Supplier Name', 'Route', 'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
    //        "TH": ['Supplier Code', 'Supplier Name', 'Route', 'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
    //        "JP": ['Supplier Code', 'Supplier Name', 'Route', 'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
    //    },
    //    column: [
    //        { "data": "F_Supplier_Code" },
    //        { "data": "F_Supplier_Name" },
    //        { "data": "F_Truck_Card" },
    //        { "data": "M1" },
    //        { "data": "M2" },
    //        { "data": "M3" },
    //        { "data": "M4" },
    //        { "data": "M5" },
    //        { "data": "M6" },
    //        { "data": "M7" },
    //        { "data": "M8" },
    //        { "data": "M9" },
    //        { "data": "M10" },
    //        { "data": "M11" },
    //        { "data": "M12" }
    //    ],
    //    //order: 0,
    //    addnew: false,
    //    then: function (config) {
    //    }
    //});

    //initial = function () {
    //    xAjax.Post({
    //        url: 'KBNLC200/initial',
    //        then: function (result) {
    //            xDropDownList.bind('#frmCondition #selPlant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
    //            xDropDownList.bind('#frmCondition #selSupplierFrom', result.data.PPM_T_Supplier, 'F_supplier_cd', 'F_supplier_cd');
    //            xDropDownList.bind('#frmCondition #selSupplierTo', result.data.PPM_T_Supplier, 'F_supplier_cd', 'F_supplier_cd');

    //        }
    //    })

    //    xSplash.hide();
    //}
    //initial();



    //xAjax.onClick('#btnReport', function () {
    //    xAjax.Post({
    //        url: 'KBNLC200/report',
    //        data: {
    //            "Plant": _PLANT_,
    //            "Period": $('#frmCondition #datPeriod').val(),
    //            "SupplierFrom": $('#frmCondition #selSupplierFrom').val(),
    //            "SupplierTo": $('#frmCondition #selSupplierTo').val(),
    //        },
    //        then: function (result) {
    //            //console.log(result);

    //            xDataTableExport.setConfigPDF({
    //                title: 'History Delivery Time Report\n\rSupplier : ' + $('#frmCondition #selSupplierFrom').val() + ' to ' + $('#frmCondition #selSupplierTo').val()
    //            });

    //            xDataTable.Bind('tblMaster', result.data);
    //            $('#tblMaster').DataTable().buttons(3).trigger();

    //        }
    //    })
    //})

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

                let userName = $("#userId").val();
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