$(document).ready(function () {

    xDataTableExport.ButtonPDF.orientation = 'landscape';

    const tblMaster = xDataTable.Initial({
        name: 'tblMaster',
        scrollbar: "300px",
        dom: '<"top">t<"clear">',
        //checking: 0,
        //running: 0,
        columnTitle: {
            "EN": ['Supplier Code', 'Supplier Name', 'Route', 'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
            "TH": ['Supplier Code', 'Supplier Name', 'Route', 'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
            "JP": ['Supplier Code', 'Supplier Name', 'Route', 'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        },
        column: [
            { "data": "F_Supplier_Code" },
            { "data": "F_Supplier_Name" },
            { "data": "F_Truck_Card" },
            { "data": "M1" },
            { "data": "M2" },
            { "data": "M3" },
            { "data": "M4" },
            { "data": "M5" },
            { "data": "M6" },
            { "data": "M7" },
            { "data": "M8" },
            { "data": "M9" },
            { "data": "M10" },
            { "data": "M11" },
            { "data": "M12" }
        ],
        //order: 0,
        addnew: false,
        then: function (config) {
        }
    });

    initial = function () {
        xAjax.Post({
            url: 'KBNLC200/initial',
            then: function (result) {
                xDropDownList.bind('#frmCondition #selPlant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
                xDropDownList.bind('#frmCondition #selSupplierFrom', result.data.PPM_T_Supplier, 'F_supplier_cd', 'F_supplier_cd');
                xDropDownList.bind('#frmCondition #selSupplierTo', result.data.PPM_T_Supplier, 'F_supplier_cd', 'F_supplier_cd');

            }
        })

        xSplash.hide();
    }
    initial();



    xAjax.onClick('#btnReport', function () { 
        xAjax.Post({
            url: 'KBNLC200/report',
            data: {
                "Plant": _PLANT_,
                "Period": $('#frmCondition #datPeriod').val(),
                "SupplierFrom": $('#frmCondition #selSupplierFrom').val(),
                "SupplierTo": $('#frmCondition #selSupplierTo').val(),
            },
            then: function (result) {
                //console.log(result);

                xDataTableExport.setConfigPDF({
                    title: 'History Delivery Time Report\n\rSupplier : ' + $('#frmCondition #selSupplierFrom').val() + ' to ' + $('#frmCondition #selSupplierTo').val()
                });

                xDataTable.Bind('tblMaster', result.data);
                $('#tblMaster').DataTable().buttons(3).trigger();
                
            }
        })
    })



});