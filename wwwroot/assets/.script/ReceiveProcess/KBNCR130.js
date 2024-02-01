$(document).ready(function () {
    xAjax.Post({
        url: 'KBNCR130/Initial',
        then: function (result) {
            // console.log(result);
            $.each(result.data, function (e, t) {
                // console.log(e + " eeeeeeeeeeeee ", t.F_Supplier_Code + " tttttttttttt ")
                $("#F_SupplierFrom").append($("<option>", { value: t.F_Supplier_Code, text: t.F_Supplier_Code }, "</option>"));
                $("#F_SupplierTo").append($("<option>", { value: t.F_Supplier_Code, text: t.F_Supplier_Code }, "</option>"));
            });
            $('#supFromBlank').hide();
            $('#supToBlank').hide();
            xSplash.hide();
        },
        error: function (result) {
            console.error("Initial Error from Receive Special Report");
            xSplash.hide();
        },
    });
});