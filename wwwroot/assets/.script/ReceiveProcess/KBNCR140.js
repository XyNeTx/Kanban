$(document).ready(function () {
    xAjax.Post({
        url: 'KBNCR140/Initial',
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

    xAjax.onClick("#ReportBtn", function () {
        var devDate = $("#F_DeliveryFrom").val().replaceAll('-', '');
        var toDate = $("#F_DeliveryTo").val().replaceAll('-', '');
        var type = $('input[name="radioType"]').filter(":checked").val();

        if (devDate > toDate) {
            return alert("Please Don't select Delivery date to less than Delivery date from");
        }

        var supFrom = $("#F_SupplierFrom").val();
        var supTo = $("#F_SupplierTo").val();

        if (supFrom > supTo) {
            return alert("Please Don't select Supplier To less than Supplier From");
        }
        if (supFrom == "") {
            supFrom = "0000-Z";
        }
        if (supTo == "") {
            supTo = "9999-Z";
        }

        console.log(devDate + " devDate ", toDate + " toDate ", type + " type ", supFrom + " supfrom ", supTo + " supto ");

        var reportUrl = "http://hmmt-app03/Reportserver/report/KB3/";

        window.location.href = reportUrl + 'KBNCR140' + '?spcDateFrom=' + devDate + '&spcDateTo=' + toDate +
            '&spcSupFrom=' + supFrom + '&spcSupTo=' + supTo + '&spcType=' + type;
    });
});