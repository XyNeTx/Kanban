$(document).ready(function () {
    xAjax.Post({
        url: 'KBNRC160/Initial',
        then: function (result) {
            // console.log(result);
            $.each(result.data, function (e, t) {
                // console.log(e + " eeeeeeeeeeeee ", t.F_Supplier_Code + " tttttttttttt ")
                $("#F_PartFrom").append($("<option>", { value: t.F_Part_No, text: t.F_Part_No }, "</option>"));
                $("#F_PartTo").append($("<option>", { value: t.F_Part_No, text: t.F_Part_No }, "</option>"));
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
        var userName = $("#profile-avatar").prop("title");

        if (devDate > toDate) {
            return alert("Please Don't select Delivery date to less than Delivery date from");
        }

        var partFrom = $("#F_PartFrom").val();
        var partTo = $("#F_PartTo").val();

        if (partFrom > partTo) {
            return alert("Please Don't select Supplier To less than Supplier From");
        }
        if (partFrom == "") {
            partFrom = "1215710010-00";
        }
        if (partTo == "") {
            partTo = "C92290K010-00";
        }

        //console.log(partTo + " ", partFrom + " ");

        // console.log(devDate + " devDate ", toDate + " toDate ", type + " type ", supFrom + " supfrom ", supTo + " supto ");

        var reportUrl = "http://hmmt-app03/Reportserver/report/KB3/";

        //window.location.href = reportUrl + 'KBNRC160' + '?DateFrom=' + devDate + '&DateTo=' + toDate +
        //    '&PartFrom=' + partFrom + '&PartTo=' + partTo + '&UserName=' + userName;

        window.open(
            _REPORTINGSERVER_ + '%2fKB3%2f' + 'KBNRC160' + '&rs:Command=Render'
            + '&DateFrom=' + devDate
            + '&DateTo=' + toDate
            + '&PartFrom=' + partFrom
            + '&PartTo=' + partTo
            + '&UserName=' + userName
            , '_blank'
        );
    });
});